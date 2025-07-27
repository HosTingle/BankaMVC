using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BankaMVC.Controllers
{
    using Banka.Varlıklar.DTOs;
    using BankaMVC.Filters;
    using BankaMVC.Models.DTOs;
    using BankaMVC.Models.Result;
    using BankaMVC.Models.Somut;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Text;

    namespace BankingMVC.Controllers
    {
        public class ParaIslemController : Controller
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            public ParaIslemController(IHttpContextAccessor httpContextAccessor) 
            {
                _httpContextAccessor = httpContextAccessor;
            }

            [RoleAuthorize("Müşteri")]
            public async Task<IActionResult> Index()
            {
                var model = new ParaIslemViewModel
                {
                    Hesaplar =await HesaplariGetirAsync(),
                    Kartlar = await KartlariGetirAsync(),
                };
                return View(model);
            }

        
            [HttpPost]
            public async Task<IActionResult> ParaCekYatir(ParaIslemViewModel paraIslemViewModel)
            {
                if (!ModelState.IsValid)
                {
                    return View("Basarisiz",paraIslemViewModel);
                }
                ParaCekYatirDto paraCekYatirDto=new ParaCekYatirDto();
                if (paraIslemViewModel.AracTuru =="kart")
                {
                    paraCekYatirDto = new ParaCekYatirDto
                    {

                        Tutar = paraIslemViewModel.Tutar ?? paraIslemViewModel.YTutar ?? 0,
                        IslemTipi = paraIslemViewModel.IslemTuru,
                        HesapId =  paraIslemViewModel.SecilenKartId,
                        Aciklama = "",
                  IslemTuru=paraIslemViewModel.AracTuru,


                };
                }
                else
                {
                    paraCekYatirDto = new ParaCekYatirDto
                    {

                        Tutar = paraIslemViewModel.Tutar ?? paraIslemViewModel.YTutar ?? 0,
                        IslemTipi = paraIslemViewModel.IslemTuru,
                        HesapId = paraIslemViewModel.SecilenHesapId!.Value.ToString(),
                        Aciklama = "",
                     IslemTuru= paraIslemViewModel.AracTuru,   
                

                };
                }
            
            
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

                    var apiUrl = StaticSettings.ApiBaseUrl + "Islem/paracekyatir";

                    var content = new StringContent(JsonConvert.SerializeObject(paraCekYatirDto), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";
                        return View("Basarili", paraIslemViewModel);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Hata Kodu: " + response.StatusCode);
                        Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                        Console.WriteLine("Hata İçeriği: " + errorContent);
                        TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                        return View("Basarisiz",paraIslemViewModel);
                    }
                }
            }
            public IActionResult Basarili()
            {
                if (TempData["IslemTuru"] == null)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.IslemTuru = TempData["IslemTuru"];
                ViewBag.Tutar = TempData["Tutar"];
                ViewBag.IslemNo = TempData["IslemNo"];

                return View();
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
        }
    }
}
