using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class Graph
    {
        public IList<Arc> Arcs { get; set; }
        public IList<Node> Nodes { get; set; }

    }
}
