using BankaMVC.Models.DTOs;

namespace BankaMVC.Models.ViewModels
{
    public class VarliklarViewModel
    {
        public decimal ToplamPara { get; set; }

        public decimal ToplamBorc { get; set; }

        public List<HesapDto> Hesaplar { get; set; }

        public List<KartDto> Kartlar { get; set; }
    }
}
