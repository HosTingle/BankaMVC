
using Banka.Varlıklar.DTOs;
using BankaMVC.Filters;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BankaMVC.Controllers
{
    public class HesaplarController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HesaplarController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [RoleAuthorize("Müşteri")]
        public async Task<IActionResult> Index(string sekme = "tum")
        {
            var hesaplar = new HesaplarViewModel();
            hesaplar.Hesaplar = await HesaplariGetirAsync();

            IEnumerable<Hesap> filtrelenmisHesaplar = hesaplar.Hesaplar;

            if (sekme == "vadesiz")
                filtrelenmisHesaplar = hesaplar.Hesaplar.Where(h => h.HesapTipi == "Vadesiz");
            else if (sekme == "vadeli")
                filtrelenmisHesaplar = hesaplar.Hesaplar.Where(h => h.HesapTipi == "Vadeli");

            var viewModel = new HesaplarViewModel
            {
                Hesaplar = filtrelenmisHesaplar.ToList(),
                AktifSekme = sekme
            };

            return View(viewModel);
        }
        [AllowAnonymous] 
        public IActionResult Yetkisiz()
        {
            return View();
        }
        private async Task<List<Hesap>> HesaplariGetirAsync()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

                if (string.IsNullOrEmpty(token))
                {
                    return new List<Hesap>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/idilehepsinigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<Hesap>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<Hesap>();
                }

                return new List<Hesap>();
            }
        }

        [HttpGet]
        public IActionResult YeniHesap()
        {
            var model = new YeniHesapViewModel
            {
                HesapTipleri = new List<HesapTipiOption>
                {
                    new HesapTipiOption
                    {
                        Value = "Vadesiz",
                        Text = "Vadesiz Hesap",
                        Description = "Günlük işlemleriniz için ideal, istediğiniz zaman para çekebilirsiniz",
                        Features = new List<string> { "7/24 Para Çekme", "Online İşlemler", "Kart İle Alışveriş" }
                    },
                    new HesapTipiOption
                    {
                        Value = "Vadeli",
                        Text = "Vadeli Hesap",
                        Description = "Paranızı değerlendirin, yüksek faiz oranlarından yararlanın",
                        Features = new List<string> { "Yüksek Faiz", "Sabit Getiri", "Vade Seçenekleri" }
                    }
                },
                ParaBirimleri = new List<ParaBirimiOption>
                {
                    new ParaBirimiOption
                    {
                        Value = "TL",
                        Text = "Türk Lirası",
                        Symbol = "₺",
                        Description = "Türkiye'deki tüm işlemleriniz için"
                    },
                    new ParaBirimiOption
                    {
                        Value = "USD",
                        Text = "Amerikan Doları",
                        Symbol = "$",
                        Description = "Uluslararası işlemler ve döviz yatırımı"
                    },
                    new ParaBirimiOption
                    {
                        Value = "EUR",
                        Text = "Euro",
                        Symbol = "€",
                        Description = "Avrupa işlemleri ve döviz çeşitliliği"
                    }
                }
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> YeniHesapOlustur(YeniHesapViewModel yeniHesapViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Basarisiz", yeniHesapViewModel);
            }


            var hesapOlusturDto = new HesapOlusturDto
            {

                HesapTipi = yeniHesapViewModel.HesapTipi,
                ParaBirimi = yeniHesapViewModel.ParaBirimi,


            };



            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Giriş yapmanız gerekiyor.";
                    return RedirectToAction("Giris", "Hesap");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/otomatikhesapolustur";
                var content = new StringContent(JsonConvert.SerializeObject(yeniHesapViewModel), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";
                    return View("Basarili", yeniHesapViewModel);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                    return View("Basarisiz", yeniHesapViewModel);
                }
            }
        }
    }
    
}
