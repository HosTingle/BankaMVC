namespace BankaMVC.Models.Somut
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace BankaYonetimPaneli.Models
    {
        public class HesapAcmaIstegi
        {
            public int Id { get; set; }

            [Display(Name = "Müşteri Adı")]
            public string MusteriAdi { get; set; }

            [Display(Name = "Müşteri Soyadı")]
            public string MusteriSoyadi { get; set; }

            [Display(Name = "TC Kimlik No")]
            public string? HesapNo { get; set; } 

            [Display(Name = "Doğum Tarihi")]
            [DataType(DataType.Date)]
            public DateTime? DogumTarihi { get; set; }

            [Display(Name = "Telefon")]
            public string Telefon { get; set; }

            [Display(Name = "E-posta")]
            [DataType(DataType.EmailAddress)]
            public string? Eposta { get; set; }

            [Display(Name = "Adres")]
            public string? Adres { get; set; }

            [Display(Name = "Başvuru Tarihi")]
            public DateTime BasvuruTarihi { get; set; }

            [Display(Name = "Durum")]
            public IstekDurumu Durum { get; set; }
        }
    }

}
