namespace BankaMVC.Models.Somut
{
    using System;

    namespace BankaYonetimPaneli.Models
    {
        public class KullaniciLogDto
        {
            public int Id { get; set; }
            public int KullaniciId { get; set; }
            public string IpAdresi { get; set; }
            public bool Basarili { get; set; }
            public DateTime Zaman { get; set; }
        }
    }

}
