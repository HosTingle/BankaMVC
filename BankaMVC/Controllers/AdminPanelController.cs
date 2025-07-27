using BankaMVC.Models.DTOs;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut.BankaYonetimPaneli.Models;
using BankaMVC.Models.Somut;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using BankaMVC.Filters;

namespace BankaMVC.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminPanelController(IHttpContextAccessor httpContextAccessor)
        { 
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
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

        [HttpGet]
        private async Task<IstekSayilariDto> IstekSayisiGetirAsync()
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
                    return new IstekSayilariDto();
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/Isteksayilarigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<SuccessDataResult<IstekSayilariDto>>(json);

                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    return data.Data ?? new IstekSayilariDto();
                }

                return new IstekSayilariDto();
            }
        }
        [HttpGet]
        private async Task<List<GirisOlayi>> GirisOlayiGetirAsync()
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
                    return new List<GirisOlayi>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "GirisOlayi/hepsinigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<SuccessDataResult<List<GirisOlayi>>>(json);

                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    return data.Data ?? new List<GirisOlayi>();
                }

                return new List<GirisOlayi>();
            }
        }

        [RoleAuthorize("Yönetici")]
        public async Task<ActionResult> Index()
        {
            var veri = await IstekSayisiGetirAsync();
            var girislog =await GirisOlayiGetirAsync();
            var log = girislog.Select(g => new KullaniciLogDto
            {
             Basarili=g.Basarili,
             Id=g.Id,
             IpAdresi=g.IpAdresi,   
             KullaniciId=g.KullaniciId,
             Zaman = g.Zaman

            }).ToList();
   
            var model = new DashboardOzet
            {
                BekleyenHesapIstekleri = veri.HesapIstekleri,
                BekleyenKartIstekleri = veri.KartIstekleri,
                AcikDestekTalepleri = veri.DestekIstekleri,
                BekleyenLimitArtirmaIstekleri = veri.LimitArtirmaIstekleri ?? 0,
                KullaniciLoglari= log,
                
            };

            return View(model);
        }
    }
}
