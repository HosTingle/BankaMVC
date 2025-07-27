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
    public class HesapIstekleriController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HesapIstekleriController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [RoleAuthorize("Yönetici")]
        public async Task<ActionResult> Index()
        {
            var result = await HesaplariGetirAsync();
          var model = result.Select(r => new HesapAcmaIstegi
    {
        BasvuruTarihi = r.BasvuruTarihi,
              Durum = Enum.TryParse<IstekDurumu>(r.Durum, out var parsedDurum) ? parsedDurum : IstekDurumu.Beklemede, // varsayılan Bekliyor
              Telefon = r.Telefon,
        Id = r.Id,
        MusteriAdi = r.AdSoyad,
        MusteriSoyadi = "", // Gerekirse ekle
        HesapNo=r.HesapNo,
        Eposta=r.Eposta,
       
    }).ToList();
            return View(model);
        }
     
        public async Task<ActionResult> Detay(int id)
        {
            var r = await HesapGetirAsync(id);

            if (r == null)
            {
                return HttpNotFound();
            }

            var model = new HesapAcmaIstegi
            {
                BasvuruTarihi = r.BasvuruTarihi,
                Durum = Enum.TryParse<IstekDurumu>(r.Durum, out var parsedDurum) ? parsedDurum : IstekDurumu.Beklemede,
                Telefon = r.Telefon,
                Id = r.Id,
                MusteriAdi = r.AdSoyad,
                MusteriSoyadi = "", // Gerekirse ekle
                Eposta = r.Eposta,
                HesapNo = r.HesapNo,
            };

            return View(model);
        }
        [HttpGet]
        private async Task<HesapIstekleriDto> HesapGetirAsync(int id) 
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
                    return new HesapIstekleriDto(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/idilegetir/"+id;

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<HesapIstekleriDto>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new HesapIstekleriDto();
                }

                return new HesapIstekleriDto();
            }
        }
        [HttpGet]
        private async Task<List<HesapIstekleriDto>> HesaplariGetirAsync()
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
                    return new List<HesapIstekleriDto>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/hesapistekgetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<HesapIstekleriDto>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<HesapIstekleriDto>();
                }

                return new List<HesapIstekleriDto>();
            }
        }
   

        private ActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        // Hesap isteği onayla
        [HttpPost]
        public async Task<ActionResult> Onayla(int id)
        {
            var veri = new DurumuGuncelleDto
            {
                Id = id,
                Durum = "Onaylandi",

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

                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/hesapdurumguncelle";

                var content = new StringContent(JsonConvert.SerializeObject(veri), Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiUrl, content);
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

        // Hesap isteği reddet
        [HttpPost]
        public async Task<ActionResult> Reddet(int id)
        {
            var veri = new DurumuGuncelleDto 
            {
                Id = id,
                Durum = "Reddedildi",

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

                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/hesapdurumguncelle";

                var content = new StringContent(JsonConvert.SerializeObject(veri), Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiUrl, content);
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
