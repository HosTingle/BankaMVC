namespace BankaMVC.Models.DTOs
{
    public class ParaCekYatirDto
    {

        public decimal Tutar { get; set; }

        public string IslemTipi { get; set; }

        public string Aciklama { get; set; } 
        public string HesapId { get; set; }

        public string IslemTuru { get; set; }    
    }
}
