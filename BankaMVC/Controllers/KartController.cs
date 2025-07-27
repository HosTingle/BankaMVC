using Banka.Varlıklar.DTOs;
using BankaMVC.Models.DTOs;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BankaMVC.Controllers
{
    public class KartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public KartController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index(string sekme = "tum")
        {
            var kartlar = new KartlarViewModel();
            kartlar.Kartlar = await KartlariGetirAsync();

            IEnumerable<Kart> filtrelenmisKartlar = kartlar.Kartlar; 

            if (sekme == "banka")
                filtrelenmisKartlar = kartlar.Kartlar.Where(h => h.KartTipi == "Banka Kartı"); 
            else if (sekme == "kredi")
                filtrelenmisKartlar = kartlar.Kartlar.Where(h => h.KartTipi == "Kredi Kartı");

            var viewModel = new KartlarViewModel
            {
                Kartlar = filtrelenmisKartlar.ToList(),
                AktifSekme = sekme
            };

            return View(viewModel);
        }
   
        private async Task<List<Kart>> KartlariGetirAsync()
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
                    return new List<Kart>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = StaticSettings.ApiBaseUrl + "Kart/idilehepsinigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<Kart>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<Kart>();
                }

                return new List<Kart>();
            }
        }
        [HttpPost]
        public async Task<IActionResult> YeniKartOlustur(YeniKartViewModel yeniKartViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Basarisiz");
            }
            var veri = new KartOlusturDto
            {
                KartTipi = yeniKartViewModel.KartTipi,
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

                var apiUrl = StaticSettings.ApiBaseUrl + "Kart/otomatikkartolustur";

                var content = new StringContent(JsonConvert.SerializeObject(veri), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";
                    return View("Basarili", veri);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                    return View("Basarisiz", veri);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> LimitAritrmaEkle(int KartId, decimal MevcutLimit, int YeniLimit)
        {
            var veri = new LimitArtirmaTalepDto
            {
                 KartId= KartId,
                MevcutLimit = MevcutLimit,
                YeniLimit = YeniLimit
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

                var apiUrl = StaticSettings.ApiBaseUrl + "LimitArtirma/limitartirmaekle";

                var content = new StringContent(JsonConvert.SerializeObject(veri), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";
                    return View("Basarili", veri);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                    return View("Basarisiz", veri);
                }
            }
        }

        [HttpGet]
        public IActionResult YeniKart() 
        {

            var model = new YeniKartViewModel(); 
            return View(model);
        }
     

    }
}
