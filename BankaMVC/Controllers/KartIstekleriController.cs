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
    public class KartIstekleriController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public KartIstekleriController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [RoleAuthorize("Yönetici")]
        public async Task<ActionResult> Index()
        {
            var result = await KartlariGetirAsync();

            var model = result.Select(r => new KartAcmaIstegi
            {
                BasvuruTarihi = r.Tarih,
                Durum = Enum.TryParse<IstekDurumu>(r.Durum, out var parsedDurum) ? parsedDurum : IstekDurumu.Beklemede,
                Id = r.Id,
                IslemTarihi = DateTime.Now,
                IslemYapan = "ahmet",
                KartTipi = r.KartTipi switch
                {
                    "Kredi Kartı" => KartTipi.Kredi,
                    "Banka Kartı" => KartTipi.Banka,
                    _ => KartTipi.Kredi 
                },

                MusteriAdiSoyadi = r.AdSoyad,
                TalepEdilenLimit = r.Limit ?? 0, // null kontrolü
                MusteriId = r.Id
            }).ToList();

            return View(model);
        }

        public async Task<ActionResult> Detay(int id)
        {
            var k = await KartGetirAsync(id);

             
            if (k == null)
            {
                return HttpNotFound();
            }
            var kartacamaistegi = new KartAcmaIstegi 
            {
                BasvuruTarihi = k.Tarih,
                Durum = Enum.TryParse<IstekDurumu>(k.Durum, out var parsedDurum) ? parsedDurum : IstekDurumu.Beklemede,
                Id = k.Id,
                IslemTarihi = DateTime.Now,
                IslemYapan = "ahmet",
                KartTipi = k.KartTipi switch
                {
                    "Kredi Kartı" => KartTipi.Kredi,
                    "Banka Kartı" => KartTipi.Banka,
                    _ => KartTipi.Kredi
                },

                MusteriAdiSoyadi = k.AdSoyad,
                TalepEdilenLimit = k.Limit ?? 0, 
                MusteriId = k.Id
            };
            return View(kartacamaistegi);
        }

        [HttpGet]
        private async Task<KartIstekleriDto> KartGetirAsync(int id) 
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
                    return new KartIstekleriDto(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Kart/idilegetir/" + id;

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<KartIstekleriDto>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new KartIstekleriDto();
                }

                return new KartIstekleriDto();
            }
        }
        [HttpGet]
        private async Task<List<KartIstekleriDto>> KartlariGetirAsync() 
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
                    return new List<KartIstekleriDto>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "Kart/kartistekgetir"; 

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<KartIstekleriDto>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<KartIstekleriDto>();
                }

                return new List<KartIstekleriDto>();
            }
        }
        private ActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

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

                var apiUrl = StaticSettings.ApiBaseUrl + "Kart/kartdurumguncelle";

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

                var apiUrl = StaticSettings.ApiBaseUrl + "Kart/kartdurumguncelle";

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
