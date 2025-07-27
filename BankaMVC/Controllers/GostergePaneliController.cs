using BankaMVC.Filters;
using BankaMVC.Models.DTOs;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BankaMVC.Controllers
{
    [ServiceFilter(typeof(TokenCheckFilter))]
    public class GostergePaneliController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GostergePaneliController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [RoleAuthorize("Müşteri")]
        public async Task<IActionResult> Index()
        {
            var model = new GostergePaneliViewModel();
            model.Kartlar = await KartlariGetirAsync();
            model.Hesaplar = await HesaplariGetirAsync();
            model.Kullanici = await KullaniciBilgileriGetirAsync();
            return View(model);
        }
        public async Task<IActionResult> Cikis()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                var token = Request.Cookies["AuthToken"];

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Zaten çıkış yapmışsınız.";
                    return RedirectToAction("Index", "Giris");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = StaticSettings.ApiBaseUrl + "Auth/cikis";

                var response = await client.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    // MVC tarafında da çerezi güvenli şekilde temizle
                    Response.Cookies.Delete("AuthToken");
                    TempData["Success"] = "Çıkış yapıldı.";
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata: " + errorContent);
                    TempData["Error"] = "Çıkış işlemi başarısız oldu.";
                }

                return RedirectToAction("Index", "Giris");
            }
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
        [HttpGet]
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
        private async Task<KullaniciBilgileriDto> KullaniciBilgileriGetirAsync()  
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
                    return new KullaniciBilgileriDto(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Kullanici/kullanicigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<KullaniciBilgileriDto>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new KullaniciBilgileriDto();
                }

                return new KullaniciBilgileriDto();
            }
        }


    }
}
