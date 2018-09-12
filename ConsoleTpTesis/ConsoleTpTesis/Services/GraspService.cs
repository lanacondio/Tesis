using ConsoleTpTesis.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpMetaheuristica.Services
{
    public class GraspService
    {
        private static bool Finished { get; set; }
        private static int AccumulatedProfit { get; set; }
        private static Random r = new Random();
        public void GetResult(GraphEnvironment environment)
        {
            this.CalculateInitialROI(environment);

            var result = environment;

            var trucksToRoad = environment.Trucks;

            while (!Finished)
            {
                foreach(var truck in trucksToRoad)
                {
                    this.MakeNextStep(truck, environment.Graph);
                    truck.PrintStatus();
                }

                CheckStatus(trucksToRoad);
            
            }

            environment.AccumulatedProfit = AccumulatedProfit;
        }
        

        private void CheckStatus(IList<Truck> trucks)
        {
            trucks = trucks.Where(x => !x.IsFinished).ToList();
            Finished = trucks.Count == 0;
        }

        private Arc GetBestsArcsWithRandomPercentage(IList<Arc> arcs)
        {
            Arc result = null;
            try
            {
                var randomPercentage = int.Parse(ConfigurationManager.AppSettings["RandomPercentage"]);
                var resultLenght = int.Parse(Math.Round((decimal)(arcs.Count * randomPercentage / 100)).ToString());
                
                var bestPercentage = arcs.OrderByDescending(x => x.ROI).Take(resultLenght).ToList();
                if(bestPercentage.Count == 0)
                {
                    result = arcs.FirstOrDefault();
                }
                else
                {
                    result = GetRandomArcFromList(bestPercentage);
                }
                
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            

        }

        private Arc GetRandomArcFromList(IList<Arc> arcs)
        {
            int idx = r.Next(arcs.Count);
            return arcs[idx];
        }
        
        private bool CanVisit(Truck truck, Graph graph, Node dest)
        {
            var origin = truck.Travel.Last();

            var fstNode = origin.Id < dest.Id ? origin : dest;
            var sndNode = origin.Id == fstNode.Id ? dest : origin;

            var arc = graph.Arcs.Where(x => x.first.Id == fstNode.Id && x.second.Id == sndNode.Id).FirstOrDefault();
            return arc != null && arc.Cost <= truck.TimeLimit && dest.ShortestRoute <= (truck.TimeLimit - arc.Cost);  //arc <= truck.Capacity;

        }

        public void CalculateInitialROI(GraphEnvironment environment)
        {
            var costRelation = int.Parse(ConfigurationManager.AppSettings["CostRelation"]);
            var demandRelation = 100 - costRelation;
            foreach(var arc in environment.Graph.Arcs)
            {
                arc.ROI = ((arc.Profit / arc.Cost) * costRelation / 100)   + ((arc.Profit / arc.Demand) * demandRelation / 100);
            }            
        }

        private void MakeNextStep(Truck truck, Graph graph)
        {
            var nextArcs = GetNextArcs(truck.ActualNode, graph.Arcs);
            var availableArcs = nextArcs.Where(x => this.CanVisit(truck, graph, x.first.Id == truck.ActualNode ? x.second : x.first)).ToList();

            if(availableArcs.Count > 0)
            {
                var bestArc = this.GetBestsArcsWithRandomPercentage(availableArcs);
                bestArc.Print();
                AccumulatedProfit += truck.AddToTravel(bestArc);
            }
            else
            {
                truck.IsFinished = true;
            }
            
        }
        

        private IList<Arc> GetNextArcs(int nodeId, IList<Arc> arcs)
        {
            return arcs.Where(x => x.first.Id == nodeId || x.second.Id == nodeId).ToList();
        }

    }
}
