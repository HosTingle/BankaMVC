namespace BankaMVC.Models.Somut
{
    using System.Collections.Generic;

    namespace BankaYonetimPaneli.Models
    {
        // Dashboard için özet model
        public class DashboardOzet
        {
            public int? BekleyenHesapIstekleri { get; set; }
            public int? BekleyenKartIstekleri { get; set; }
            public int? AcikDestekTalepleri { get; set; }
            public int? BekleyenLimitArtirmaIstekleri { get; set; }

            public List<KullaniciLogDto> KullaniciLoglari { get; set; } 
        }
    }

}
