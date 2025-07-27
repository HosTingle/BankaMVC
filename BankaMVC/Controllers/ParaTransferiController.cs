using Microsoft.AspNetCore.Mvc;

namespace BankaMVC.Controllers
{
    using Banka.Varlıklar.DTOs;
    using BankaMVC.Filters;
    using BankaMVC.Models.DTOs;
    using BankaMVC.Models.Result;
    using BankaMVC.Models.Somut;
    using BankaMVC.Models.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Text;

    namespace ParaTransfer.Controllers
    {
        public class ParaTransferiController : Controller
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            public ParaTransferiController(IHttpContextAccessor httpContextAccessor) 
            {
                _httpContextAccessor = httpContextAccessor;
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var model = new ParaTransferiViewModel
                {
                    Hesaplar = await HesaplariGetirAsync(),
                    Kartlar = await KartlariGetirAsync(),
                    IslemTarihi = DateTime.Today
                };

                return View(model);
            }
            [RoleAuthorize("Müşteri")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Index(ParaTransferiViewModel model)
            {
                
                if (model.OdemeAraci == "hesap" && string.IsNullOrEmpty(model.SecilenHesapId))
                {
                    ModelState.AddModelError("SecilenHesapId", "Lütfen bir hesap seçiniz");
                }
                else if (model.OdemeAraci == "kart" && string.IsNullOrEmpty(model.SecilenKartId))
                {
                    ModelState.AddModelError("SecilenKartId", "Lütfen bir kredi kartı seçiniz");
                }

               
                if (model.IslemTarihi < DateTime.Today)
                {
                    ModelState.AddModelError("IslemTarihi", "İşlem tarihi bugünden önce olamaz");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                     
                        var sonuc = TransferIslemiGerceklestir(model);

                        if (sonuc)
                        {
                            TempData["SuccessMessage"] = "Transfer işlemi başarıyla gerçekleştirildi!";
                            return RedirectToAction("Basarili");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Transfer işlemi sırasında bir hata oluştu");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Transfer işlemi sırasında bir hata oluştu: " + ex.Message);
                    }
                }

                model.Hesaplar = await HesaplariGetirAsync();
                model.Kartlar = await KartlariGetirAsync();

                return View(model);
            }

            [HttpGet]
            public IActionResult Basarili()
            {
                return View();
            }

       

            private bool TransferIslemiGerceklestir(ParaTransferiViewModel model)
            {
        
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            [HttpPost]
            public async Task<IActionResult> ParaGonderme(ParaTransferiViewModel paraTransferiViewModel)   
            {
                if (!ModelState.IsValid)
                {
                    return View("Basarisiz", paraTransferiViewModel);
                }
                ParaGondermeDto paraGonderme = new ParaGondermeDto();
                if (paraTransferiViewModel.OdemeAraci == "kart")
                {
                    paraGonderme = new ParaGondermeDto
                    {
                        Aciklama = paraTransferiViewModel.Aciklama!,
                        AliciHesapId = paraTransferiViewModel.AliciHesapNo,
                        GonderenHesapId = paraTransferiViewModel.SecilenKartId!,
                        IslemTipi = "Para Transferi",
                        Tutar = paraTransferiViewModel.Tutar,
                        OdemeAraci=paraTransferiViewModel.OdemeAraci

                    };
                }
                else
                {
                    paraGonderme = new ParaGondermeDto
                    {
                        Aciklama = paraTransferiViewModel.Aciklama!,
                        AliciHesapId = paraTransferiViewModel.AliciHesapNo,
                        GonderenHesapId = paraTransferiViewModel.SecilenHesapId!,
                        IslemTipi = "Para Transferi",
                        Tutar = paraTransferiViewModel.Tutar,
                        OdemeAraci = paraTransferiViewModel.OdemeAraci


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

                    var apiUrl = StaticSettings.ApiBaseUrl + "Islem/paragonderme";

                    var content = new StringContent(JsonConvert.SerializeObject(paraGonderme), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Talebiniz başarıyla oluşturuldu.";
                        return View("Basarili", paraTransferiViewModel);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Hata Kodu: " + response.StatusCode);
                        Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                        Console.WriteLine("Hata İçeriği: " + errorContent);
                        TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                        return View("Basarisiz", paraTransferiViewModel);
                    }
                }
            }

            [HttpGet]
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
                        return new List<Kart>(); 
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
