using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using static BankaMVC.Controllers.GirisController;
using System.Security.Claims;
using System.Text;
using Banka.Varlıklar.DTOs;

namespace BankaMVC.Controllers
{
    public class KayitController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KayitController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (roleClaim != null)
                {
                    var userRole = roleClaim.Value;

                    if (userRole.Equals("Yönetici", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "AdminPanel");
                    }
                    else if (userRole.Equals("Müşteri", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "GostergePaneli");
                    }
                }
                return RedirectToAction("Index", "GostergePaneli");
            }
            KayitViewModel kayit=new KayitViewModel();
            kayit.Subeler = await SubelerGetirAsync();
            return View(kayit);
        }
        private async Task<List<SubeDto>> SubelerGetirAsync() 
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                var apiUrl = StaticSettings.ApiBaseUrl + "Sube/subelerigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<SubeDto>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<SubeDto>();
                }

                return new List<SubeDto>();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Kayit(KayitViewModel kayitViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                kayitViewModel.Subeler = await SubelerGetirAsync();
                return View("Basarisiz", kayitViewModel);
            }


            var dto = new KullaniciKayitDto
            {
                AdSoyad = kayitViewModel.AdSoyad,
                Email = kayitViewModel.Email,
                Telefon ="0"+ kayitViewModel.Telefon,
                Sifre = kayitViewModel.Sifre,
                Sube = kayitViewModel.SecilenSubeId,
                Aktif = true
            };


            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(StaticSettings.ApiBaseUrl + "Auth/kayit", dto); // URL'yi kendi backend'ine göre ayarla

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Giris");
            }

            // Hata mesajı ekle
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Kayıt işlemi başarısız: " + error);
            kayitViewModel.Subeler = await SubelerGetirAsync(); 
            return View("Basarisiz",kayitViewModel); 
        }




    }
}
