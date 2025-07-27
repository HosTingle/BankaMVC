namespace BankaMVC.Models.Somut
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace BankaYonetimPaneli.Models
    {
        public class DestekTaleb
        {
            public int Id { get; set; }

            [Display(Name = "Müşteri ID")]
            [Required(ErrorMessage = "Müşteri ID zorunludur.")]
            public int MusteriId { get; set; }

            [Display(Name = "Müşteri Adı")]
            [Required(ErrorMessage = "Müşteri adı boş olamaz.")]
            [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir.")]
            public string MusteriAdiSoyadi { get; set; }

            [Display(Name = "Konu")]
            [Required(ErrorMessage = "Konu boş olamaz.")]
            [StringLength(150, ErrorMessage = "Konu en fazla 150 karakter olabilir.")]
            public string Konu { get; set; }

            [Display(Name = "Açıklama")]
            [Required(ErrorMessage = "Açıklama boş olamaz.")]
            [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
            public string Aciklama { get; set; }

            [Display(Name = "Talep Tarihi")]
            public DateTime TalepTarihi { get; set; } = DateTime.Now;

            [Display(Name = "Durum")]
            [Required(ErrorMessage = "Durum seçilmelidir.")]
            public DestekDurumu Durum { get; set; }

            [Display(Name = "Yanıt")]
            public string? Yanit { get; set; }
        }
    }

}
