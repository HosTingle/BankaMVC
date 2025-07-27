using BankaMVC.Filters;
using BankaMVC.Models.DTOs;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace BankaMVC.Controllers
{
    public class SonHareketlerController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SonHareketlerController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
   
        [RoleAuthorize("Müşteri")]
        public async Task<IActionResult> Index(string tab = "vadesiz")
        {
            var model = new SonHareketlerViewModel
            {
                AktifTab = tab,
                HesapIslemler = await SonHaraketlerHesapGetirAsync(),
                KrediKartiIslemleri = await SonHaraketlerKartGetirAsync(),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetIslemler(string tip)
        {
            if (tip == "kredi")
            {
                var krediIslemleri =await SonHaraketlerKartGetirAsync();
                return PartialView("_IslemListesi", krediIslemleri);
            }
            else
            {
                var hesapIslemler = await SonHaraketlerHesapGetirAsync();
                return PartialView("_IslemListesi", hesapIslemler);
            }
        }
        [HttpGet]
        private async Task<List<SonHareketlerDto>> SonHaraketlerHesapGetirAsync()  
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
                    return new List<SonHareketlerDto>(); 
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = StaticSettings.ApiBaseUrl + "Islem/son4islemgetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<SonHareketlerDto>>>(json, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    return result?.Data ?? new  List < SonHareketlerDto >();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata Kodu: " + response.StatusCode);
                    Console.WriteLine("Hata Açıklaması: " + response.ReasonPhrase);
                    Console.WriteLine("Hata İçeriği: " + errorContent);
                    TempData["Error"] = "Talep oluşturulurken bir hata oluştu.";
                    return new List<SonHareketlerDto>();
                }
       
            }
        }
        [HttpGet]
        private async Task<List<SonHareketlerDto>> SonHaraketlerKartGetirAsync() 
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
                    return new List<SonHareketlerDto>();
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = StaticSettings.ApiBaseUrl + "KartIslem/son4islemgetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<List<SonHareketlerDto>>>(json, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    return result?.Data ?? new List<SonHareketlerDto>();
                }

                return new List<SonHareketlerDto>();
            }
        }
    }
}
