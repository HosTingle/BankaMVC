using Banka.Varlıklar.DTOs;
using BankaMVC.Filters;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace BankaMVC.Controllers
{
    public class DestekController : Controller
    {    
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DestekController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [RoleAuthorize("Müşteri")]
        public async Task<IActionResult> Index(string durum = "tum", string arama = "")
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Giriş yapmanız gerekiyor.";
                return RedirectToAction("Giris", "Hesap");
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{StaticSettings.ApiBaseUrl}DestekTalebi/filtre?durum={durum}&arama={arama}";

                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Destek talepleri alınamadı.";
                    return View(new DestekViewModel());
                }

                var talepler = await response.Content.ReadFromJsonAsync<List<DestekTalebi>>();

                var viewModel = new DestekViewModel
                {
                    Talepler = talepler.OrderByDescending(t => t.OlusturmaTarihi).ToList(),
                    DurumFiltre = durum,
                    AramaMetni = arama,
                };

                return View(viewModel);
            }
        }





        public IActionResult YeniTalep()
        {
            return View(new DestekTalebi());
        }

        [HttpPost]
        public async Task<IActionResult> YeniTalep(DestekTalebiOlusturDto model, string Kategori)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


  

            // API'ye gönder
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

                var apiUrl = StaticSettings.ApiBaseUrl + "DestekTalebi/destektalebiolustur";

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                    return View(model);
                }
            }
        }

        private async Task<List<DestekTalebi>> DestekGetirAsync() 
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
                    return new List<DestekTalebi>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "DestekTalebi/idilehepsinigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<DestekTalebi>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<DestekTalebi>();
                }

                return new List<DestekTalebi>();
            }
        }
    }
}
