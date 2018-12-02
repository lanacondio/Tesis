using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class Arc
    {
        public int Id { get; set; }
        public Node first { get; set; }
        public Node second { get; set; }
        public int Cost { get; set; }
        public int Demand { get; set; }
        public int Profit { get; set; }
        public double ROI { get; set; }

        public void Print()
        {
          
            Console.WriteLine(this.first.Id.ToString()+"----C:"+this.Cost+"--D:"+this.Demand+"--P:"+this.Profit+"--ROI:"+this.ROI+"---->"+ this.second.Id.ToString());
            
        }
    }
}
