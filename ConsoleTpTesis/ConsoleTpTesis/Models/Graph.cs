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

        public Graph Clone()
        {
            var arcs = new List<Arc>();

            foreach (var arc in this.Arcs)
            {
                arcs.Add(new Arc()
                {
                    Cost = arc.Cost,
                    Demand = arc.Demand,
                    first = arc.first,
                    Profit = arc.Profit,
                    ROI = arc.ROI,
                    second = arc.second
                });
            }

            var result = new Graph()
            {
                Arcs = arcs,
                Nodes = this.Nodes
            };            

            return result;            
        }

    }
}
