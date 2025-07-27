
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka.Varlıklar.DTOs
{
    public class HesapOlusturDto
    {
        public int KullaniciId { get; set; }  
        public string HesapTipi { get; set; }

        public string ParaBirimi {  get; set; }

    }
}
