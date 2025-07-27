
using Banka.Varlıklar.DTOs;
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

    public class DestekTalepleriController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DestekTalepleriController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
        }
        [RoleAuthorize("Yönetici")]
        public async Task<ActionResult> Index()
        {
            var result = await DestekTalebleriGetirAsync();
            var model = result.Select(r => new DestekTaleb
            {
                Durum= Enum.TryParse<DestekDurumu>(r.Durum, out var parsedDurum) ? parsedDurum : DestekDurumu.Kapatildi,
                Aciklama=r.Mesaj,
                Konu=r.Konu,
                MusteriAdiSoyadi=r.AdSoyad,
                MusteriId=r.KullaniciId,
                TalepTarihi=r.Tarih.Value,
                Yanit=r.Yanit,
                 Id = r.Id
                

            }).ToList();
            return View(model);
        }


        public async Task<ActionResult> Detay(int id)
        {
            var d = await DestekTalebiGetirAsync(id);
            if (d == null)
            {
                return HttpNotFound();
            }
            var destekTaleb = new DestekTaleb
            {
                Durum = Enum.TryParse<DestekDurumu>(d.Durum, out var parsedDurum) ? parsedDurum : DestekDurumu.Kapatildi,
                Aciklama = d.Mesaj,
                Konu = d.Konu,
                MusteriAdiSoyadi = d.AdSoyad,
                MusteriId = d.KullaniciId,
                TalepTarihi = d.Tarih.Value,
                Yanit=d.Yanit,
                Id = d.Id

            };
            return View(destekTaleb);
        }
        [HttpGet]
        private async Task<DestekTalebiOlusturDto> DestekTalebiGetirAsync(int id) 
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
                    return new DestekTalebiOlusturDto(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "DestekTalebi/idilegetir/" + id;

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<DestekTalebiOlusturDto>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new DestekTalebiOlusturDto();
                }

                return new DestekTalebiOlusturDto();
            }
        }
        [HttpPost("silme/{id}")]
        public async Task<IActionResult> Silme(int id) 
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

             
                var apiUrl = $"{StaticSettings.ApiBaseUrl}DestekTalebi/sil/{id}";

              
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

        // Destek talebi durumunu güncelle
        [HttpPost]
        public async Task<ActionResult> DurumGuncelle(int id, DestekDurumu durum, string yanit)
        {
            var veri = new DestekTalebiGuncelleDto
            {
                Id = id,
                Durum = durum.ToString(),
                Yanit =yanit

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

                var apiUrl = StaticSettings.ApiBaseUrl + "DestekTalebi/destektalebiguncelle";

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

        [HttpGet]
        private async Task<List<DestekTalebiOlusturDto>> DestekTalebleriGetirAsync() 
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
                    return new List<DestekTalebiOlusturDto>(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = StaticSettings.ApiBaseUrl + "DestekTalebi/destekisteklerigetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<DestekTalebiOlusturDto>>>(
            json,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new List<DestekTalebiOlusturDto>();
                }

                return new List<DestekTalebiOlusturDto>();
            }
        }
       
        }
    }

