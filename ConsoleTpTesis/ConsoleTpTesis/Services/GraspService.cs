using ConsoleTpTesis.Models;
using ConsoleTpTesis.Services;
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
        private static Random r = new Random();
        private SortedList<string, string> tabooList = new SortedList<string, string>();

        public GraphEnvironment GetResult(GraphEnvironment environment)
        {
            var results = new List<GraphEnvironment>();
            var seedsResult = new List<GraphEnvironment>();
            var finalSeedsResult = new List<GraphEnvironment>();
            var backupResult = new List<GraphEnvironment>();
            var originalGraphs = new Queue<Graph>();

            this.CalculateInitialROI(environment);
            
            
            var maxSeedIteration = int.Parse(ConfigurationManager.AppSettings["MaxSeedIteration"]);
            var ImprovementIterationPercentage = int.Parse(ConfigurationManager.AppSettings["ImprovementIterationPercentage"]); 

            var originalCapacity = environment.Trucks.FirstOrDefault().Capacity;
            var originalTimeLimit = environment.Trucks.FirstOrDefault().TimeLimit;
            var originalGraph = environment.Graph.Clone();

            
            var greedyEnv = environment.Clone();
            var auxGraph = environment.Graph.Clone();
            
            var iterationCount = 0;
            var finalIterationCount = 0;
            var localIterationAverage = 0;

            this.MakeSeedResult(greedyEnv);
            this.CheckTabu(greedyEnv);

            while (iterationCount < maxSeedIteration)
            {
                var isnew = false;
                var auxEnv = new GraphEnvironment();

                while (!isnew)
                {
                    auxEnv = environment.Clone();
                    this.MakeSeedResult(auxEnv);
                    isnew = this.CheckTabu(auxEnv);
                }
                
                //revisar
                var worker = new LocalSearchWorker(auxEnv, originalGraph, originalCapacity, originalTimeLimit);
                worker.Run();

                var environmentsImprovement = CalculateImprovement(greedyEnv, worker.resultSeed);

                localIterationAverage += worker.resultSeed.LocalIterationsCount;

                if (greedyEnv.AccumulatedProfit < worker.resultSeed.AccumulatedProfit)
                {
                    greedyEnv = worker.resultSeed;
                }

                if (environmentsImprovement >= ImprovementIterationPercentage)
                {
                    finalIterationCount = iterationCount;
                    iterationCount = 0;
                }
                iterationCount++;
            }


            greedyEnv.SeedsCount = finalIterationCount + iterationCount;
            localIterationAverage = localIterationAverage / greedyEnv.SeedsCount;
            greedyEnv.LocalIterationsAverage = localIterationAverage;
            
            return greedyEnv;
                        
        }


        private double CalculateImprovement(GraphEnvironment originalEnv, GraphEnvironment newEnv)
        {
            var result = 0;
            if(originalEnv.AccumulatedProfit < newEnv.AccumulatedProfit)
            {
                var difference = newEnv.AccumulatedProfit - originalEnv.AccumulatedProfit;
                result = difference * 100 / originalEnv.AccumulatedProfit;
            }

            return result;
        }
        

        private decimal getAverageDiference(List<GraphEnvironment> results, List<GraphEnvironment> backupResult)
        {
            var diferences = new List<decimal>();

            for (int i= 0; i< results.Count; i++)
            {
                var diference = results[i].AccumulatedProfit - backupResult[i].AccumulatedProfit;
                var diferencePercentage = 0;
                if (results[i].AccumulatedProfit > 0)
                {
                    diferencePercentage = diference * 100 / results[i].AccumulatedProfit;
                }                
                diferences.Add(diferencePercentage);
            }

            return diferences.Average();
        }

        private void MakeSeedResult(GraphEnvironment environment)
        {
            Finished = false;
            var trucksToRoad = environment.Trucks;

            while (!Finished)
            {
                foreach (var truck in trucksToRoad)
                {
                    this.MakeNextStep(truck, environment);                    
                }

                CheckStatus(trucksToRoad);
            }
            
        }

        private void CheckStatus(IList<Truck> trucks)
        {
            trucks = trucks.Where(x => !x.IsFinished).ToList();
            Finished = trucks.Count == 0;
        }

        private Arc GetBestsArcsWithRandomPercentage(IList<Arc> arcs, IList<Node> travel, Truck truck, int minimumDemand)
        {
            Arc result = null;
            try
            {
                if (truck.Capacity < minimumDemand)
                {
                    return arcs.OrderByDescending(x => this.GetMinimumArcDistance(x, truck)).LastOrDefault();
                }


                var randomPercentage = int.Parse(ConfigurationManager.AppSettings["RandomPercentage"]);
                var resultLenght = int.Parse(Math.Round((decimal)(arcs.Count * randomPercentage / 100)).ToString());
                
                var bestPercentage = arcs.Where(x => !truck.TabooArcs.Contains(x.Id)).OrderByDescending(x => x.ROI).Take(resultLenght).ToList();
   
                if(bestPercentage.Count == 0)
                {
                    result = arcs.FirstOrDefault();
                }
                else
                {                                    
                    if (travel.Count > 1)
                    {
                        var lastNodes = travel.Reverse().Take(2).ToList();
                        
                        if (bestPercentage.Count > 1 )                                                
                        {
                            var repitedArc = bestPercentage.Where(x =>
                                (x.first.Id == lastNodes.First().Id || x.first.Id == lastNodes.Last().Id) &&
                                (x.second.Id == lastNodes.First().Id || x.second.Id == lastNodes.Last().Id))
                                .FirstOrDefault();

                            if(repitedArc != null)
                            {
                                bestPercentage.Remove(repitedArc);
                            }
                            
                        }
                    }
                   
                    result = GetRandomArcFromList(bestPercentage);
                }
                
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private int GetMinimumArcDistance(Arc arc, Truck truck)
        {
            var otherNode = arc.first.Id == truck.ActualNode ?
                arc.second : arc.first;

            return otherNode.ShortestRoute;

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
            double costRelation = int.Parse(ConfigurationManager.AppSettings["CostRelation"]);
            double demandRelation = 100 - costRelation;
            
            foreach(var arc in environment.Graph.Arcs)
            {
                double profit = arc.Profit;
                double cost = arc.Cost;
                double demand = arc.Demand;

                arc.ROI = ((profit / cost) * costRelation )   + ((profit / demand) * demandRelation);
            }            
        }

        private void MakeNextStep(Truck truck, GraphEnvironment graphEnvironment)
        {
            var graph = graphEnvironment.Graph;
            var nextArcs = GetNextArcs(truck.ActualNode, graph.Arcs);
            var availableArcs = nextArcs.Where(x => this.CanVisit(truck, graph, x.first.Id == truck.ActualNode ? x.second : x.first)).ToList();

            if (availableArcs.Count > 0
              && !(truck.Capacity < graphEnvironment.MinimumDemand
              && truck.ActualNode == 1))
            {
                var bestArc = this.GetBestsArcsWithRandomPercentage(availableArcs, truck.Travel, truck, graphEnvironment.MinimumDemand);              
                graphEnvironment.AccumulatedProfit += truck.AddToTravel(bestArc);
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


        private bool CheckTabu(GraphEnvironment environment)
        {
            var key = environment.Trucks.Select(x => string.Join("", x.Travel.Select(y => y.Id).ToArray())).ToList().Aggregate((i, j) => i + j);
            
            var result = !tabooList.Keys.Contains(key);
            if (result) { tabooList.Add(key, key); } 

            return result;
        }

    }
}
