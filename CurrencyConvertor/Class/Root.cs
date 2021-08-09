using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvertor.Class
{
    public class Root : Rate
    {
        public Rate rates { get; set; }
        public long timestamp;
        public string license;
    }
}
