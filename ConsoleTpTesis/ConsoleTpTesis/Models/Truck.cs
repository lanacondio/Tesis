﻿using System;
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
        public bool IsFinished { get; set; }

        public int AddToTravel(Arc arc)
        {
            var result = 0;
            var nodeToAdd = arc.first.Id == ActualNode ? arc.second : arc.first;

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
