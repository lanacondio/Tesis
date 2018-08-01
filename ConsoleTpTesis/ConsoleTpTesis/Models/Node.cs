using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class Node
    {
        public int Id { get; set; }
        public int ShortestRoute { get; set; }
        public Node ShortestPredecesor { get; set; }
    }
}
