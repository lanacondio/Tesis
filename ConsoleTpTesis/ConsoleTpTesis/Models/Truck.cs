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
        public List<Arc> ArcsTravel { get; set; }
        public bool IsFinished { get; set; }

        public int AddToTravel(Arc arc)
        {
            var result = 0;
            var nodeToAdd = arc.first.Id == ActualNode ? arc.second : arc.first;
            this.ArcsTravel.Add(arc);
            this.Travel.Add(nodeToAdd);
            this.ActualNode = nodeToAdd.Id;

            this.TimeLimit -= arc.Cost;

            if (arc.Demand <= this.Capacity)
            {
                this.Capacity -= arc.Demand;
                result = arc.Profit;
                arc.Profit = 0;
                arc.Demand = 0;
                arc.ROI = 0;

            }

            return result;            
        }

        public int AddSimulatedArcToTravel(Arc arc)
        {
            var result = 0;            
            this.TimeLimit -= arc.Cost;

            if (arc.Demand <= this.Capacity)
            {
                this.Capacity -= arc.Demand;
                result = arc.Profit;
                arc.Profit = 0;
                arc.Demand = 0;
                arc.ROI = 0;
            }
            return result;
        }

        public Truck Clone()
        {
            var result = new Truck()
            {
                ActualNode = 1,
                ArcsTravel = new List<Arc>(),
                Capacity = this.Capacity,
                Id = this.Id,
                IsFinished = this.IsFinished,
                TimeLimit = this.TimeLimit,
                Travel = new List<Node>()               
            };

            result.Travel.Add(this.Travel.Where(y => y.Id == result.ActualNode).FirstOrDefault());
            
            return result;

        }


        public Truck CloneWithTravel()
        {
            var result = new Truck()
            {
                ActualNode = 1,
                ArcsTravel = new List<Arc>(),
                Capacity = this.Capacity,
                Id = this.Id,
                IsFinished = this.IsFinished,
                TimeLimit = this.TimeLimit,
                Travel = new List<Node>()
            };

            this.Travel.ForEach(x => result.Travel.Add(x));
            this.ArcsTravel.ForEach(x => result.ArcsTravel.Add(x));
            
            return result;

        }
        public void PrintStatus()
        {
            Console.WriteLine("Truck " + this.Id+ " Capacity: " +this.Capacity+" TimeLimit: "+this.TimeLimit);            
            Console.WriteLine("Travel: ");
            this.Travel.Select(x => x.Id).ToList().ForEach(x => Console.Write(x + "->"));
            Console.WriteLine("\n");
            Console.WriteLine("===============");
        }
    }
}
