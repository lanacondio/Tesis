using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int TimeLimit { get; set; }
        public int ActualNode { get; set; }
        public List<Node> Travel { get; set; }
    }
}
