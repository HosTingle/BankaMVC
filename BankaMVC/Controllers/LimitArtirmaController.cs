using BankaMVC.Filters;
using BankaMVC.Models.DTOs;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.Somut.BankaYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BankaMVC.Controllers
{
    public class LimitArtirmaController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LimitArtirmaController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [RoleAuthorize("Yönetici")]
        public async Task<ActionResult> Index()
        {
            var result = await LimitArtirmaIstekleriGetirAsync();

            var model = result.Select(r => new LimitArtirmaIstegi
            {
               BasvuruTarihi=r.BasvuruTarihi,
                Durum = Enum.TryParse<IstekDurumu>(r.Durum, out var parsedDurum) ? parsedDurum : IstekDurumu.Beklemede,
               KartNo=r.KartNo,
               MevcutLimit=r.MevcutLimit!.Value,
               MusteriAdiSoyadi=r.AdSoyad,
               TalepEdilenLimit=r.TalepEdilenLimit!.Value,
               MusteriId=r.KullaniciId,
               Id=r.Id,    
               
            }).ToList();

            return View(model);

        }
        [HttpGet]
        private async Task<LimitArtırmaDto> LimitArtirmaGetirAsync(int id) 
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
                    return new LimitArtırmaDto(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "LimitArtirma/idilegetir/" + id;

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<LimitArtırmaDto>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new LimitArtırmaDto();
                }

                return new LimitArtırmaDto();
            }
        }
        [HttpGet]
        private async Task<List<LimitArtırmaDto>> LimitArtirmaIstekleriGetirAsync()
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
                    return new List<LimitArtırmaDto>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "LimitArtirma/kartlimitisteklerigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<LimitArtırmaDto>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<LimitArtırmaDto>();
                }

                return new List<LimitArtırmaDto>();
            }
        }
  
        public async Task<ActionResult> Detay(int id)
        {
            var r = await LimitArtirmaGetirAsync(id);
            if (r == null)
            {
                return HttpNotFound();
            }
            var limitartirmaistegi = new LimitArtirmaIstegi 
            {
                BasvuruTarihi = r.BasvuruTarihi,
                Durum = Enum.TryParse<IstekDurumu>(r.Durum, out var parsedDurum) ? parsedDurum : IstekDurumu.Beklemede,
                KartNo = r.KartNo,
                MevcutLimit = r.MevcutLimit!.Value,
                MusteriAdiSoyadi = r.AdSoyad,
                TalepEdilenLimit = r.TalepEdilenLimit!.Value,
                MusteriId = r.KullaniciId,
                Id = r.Id,
            };
            return View(limitartirmaistegi);
        }
        [HttpPost("sil/{id}")]
        public async Task<IActionResult> Sil(int id)
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
                    TempData["Error"] = "Giriş yapmanız gerekiyor.";
                    return RedirectToAction("Giris", "Hesap");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // ✅ Buradaki id parametresini interpolasyonla geçiriyoruz
                var apiUrl = $"{StaticSettings.ApiBaseUrl}LimitArtirma/sil/{id}";

                // ✅ Silme işlemi için DELETE metodu kullanılmalı
                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Silme işlemi başarılı.";
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Silme işlemi başarısız oldu.";
                }

                return RedirectToAction("Index");
            }
        }

        private ActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

      
        [HttpPost]
        public async Task<ActionResult> Onayla(int id, decimal onaylananLimit, string kartNo,decimal mevcutLimit)
        {
            var veri = new LimitArtirmaEkleDto
            { 
                Id = id,
                TalepEdilenLimit = onaylananLimit,
                Durum= "Onaylandi",
                KartNo=kartNo  ,
                MevcutLimit=mevcutLimit,

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

                var apiUrl = StaticSettings.ApiBaseUrl + "LimitArtirma/limitguncelle";

                var content = new StringContent(JsonConvert.SerializeObject(veri), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";

                    return RedirectToAction("Index");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";

                    return RedirectToAction("Index");
                }
            }

        }

   
        [HttpPost]
        public async Task<ActionResult> Reddet(int id, decimal onaylananLimit, string kartNo)
        {

            var veri = new LimitArtirmaEkleDto
            {
                Id = id,
                TalepEdilenLimit = onaylananLimit,
                Durum = "Reddedildi",
                KartNo = kartNo,

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

                var apiUrl = StaticSettings.ApiBaseUrl + "LimitArtirma/limitguncelle";

                var content = new StringContent(JsonConvert.SerializeObject(veri), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";

                    return RedirectToAction("Index");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";

                    return RedirectToAction("Index");
                }
            }
        }
     
    }
}
