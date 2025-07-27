namespace BankaMVC.Models.DTOs
{
    public class IstekSayilariDto
    {
        public int HesapIstekleri { get; set; }

        public int KartIstekleri { get; set; }

        public int DestekIstekleri { get; set; } 

        public int? LimitArtirmaIstekleri { get; set; }
    }
}
