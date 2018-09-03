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

        public void AddToTravel(Arc arc)
        {
            var nodeToAdd = arc.first.Id == ActualNode ? arc.second : arc.first;

            this.Travel.Add(nodeToAdd);
            this.ActualNode = nodeToAdd.Id;
            this.Capacity -= arc.Demand;
            this.TimeLimit -= arc.Cost;

            arc.Profit = 0;
            arc.Demand = 0;
            arc.ROI = 0;
            
        }
    }
}
