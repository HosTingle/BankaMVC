namespace BankaMVC.Models.Somut
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace BankaYonetimPaneli.Models
    {
        public class LimitArtirmaIstegi
        {
            public int Id { get; set; }

            [Display(Name = "Müşteri ID")]
            public int MusteriId { get; set; }

            [Display(Name = "Müşteri Adı")]
            public string MusteriAdiSoyadi { get; set; }

            [Display(Name = "Kart No")]
            public string KartNo { get; set; }

            [Display(Name = "Mevcut Limit")]
            public decimal MevcutLimit { get; set; }

            [Display(Name = "Talep Edilen Limit")]
            public decimal TalepEdilenLimit { get; set; }

            [Display(Name = "Başvuru Tarihi")]
            public DateTime BasvuruTarihi { get; set; }

            [Display(Name = "Durum")]
            public IstekDurumu Durum { get; set; }


            [Display(Name = "Onaylanan Limit")]
            public decimal? OnaylananLimit { get; set; }
        }
    }

}
