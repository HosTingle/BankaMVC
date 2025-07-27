namespace BankaMVC.Models.DTOs
{
    public class LimitArtirmaEkleDto 
    {

        public decimal MevcutLimit { get; set; }

        public decimal TalepEdilenLimit { get; set; }

        public string KartNo { get; set; }

        public string? Durum {  get; set; }

        public int Id { get; set; }

        public int KartId { get; set; } 
    }
}
