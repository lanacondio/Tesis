using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class Arc
    {
        public Node first { get; set; }
        public Node second { get; set; }
        public int Cost { get; set; }
        public int Demand { get; set; }
        public int Profit { get; set; }
    }
}
