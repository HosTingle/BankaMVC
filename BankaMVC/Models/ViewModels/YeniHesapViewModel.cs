using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.ViewModels
{
    public class YeniHesapViewModel
    {
        [Required(ErrorMessage = "Hesap tipi seçimi zorunludur")]
        [Display(Name = "Hesap Tipi")]
        public string HesapTipi { get; set; }

        [Required(ErrorMessage = "Para birimi seçimi zorunludur")]
        [Display(Name = "Para Birimi")]
        public string ParaBirimi { get; set; }


        public List<HesapTipiOption> HesapTipleri { get; set; } = new List<HesapTipiOption>();
        public List<ParaBirimiOption> ParaBirimleri { get; set; } = new List<ParaBirimiOption>();
    }
    public class HesapTipiOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; } = new List<string>();
    }

    public class ParaBirimiOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
    }
}
