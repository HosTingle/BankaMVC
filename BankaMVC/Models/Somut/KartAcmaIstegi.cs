namespace BankaMVC.Models.Somut
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace BankaYonetimPaneli.Models
    {
        // Kart açma isteği modeli
        public class KartAcmaIstegi
        {
            public int Id { get; set; }

            [Display(Name = "Müşteri ID")]
            public int MusteriId { get; set; }

            [Display(Name = "Müşteri Adı")]
            public string MusteriAdiSoyadi { get; set; }

            [Display(Name = "Kart Tipi")]
            public KartTipi KartTipi { get; set; }

            [Display(Name = "Talep Edilen Limit")]
            public decimal TalepEdilenLimit { get; set; }

            [Display(Name = "Başvuru Tarihi")]
            public DateTime BasvuruTarihi { get; set; }

            [Display(Name = "Durum")]
            public IstekDurumu Durum { get; set; }

            [Display(Name = "İşlem Tarihi")]
            public DateTime? IslemTarihi { get; set; }

            [Display(Name = "İşlem Yapan")]
            public string IslemYapan { get; set; }
        }
    }

}
