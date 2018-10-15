using ConsoleTpTesis.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Services
{
    public class LocalSearchWorker
    {
        public GraphEnvironment resultSeed;
        public Graph originalGraph;
        public int originalCapacity;
        public int originalTimeLimit;
        private Random r = new Random();
        public LocalSearchWorker(GraphEnvironment seed, Graph graph, int capacity, int timeLimit)
        {
            this.resultSeed = seed;
            this.originalGraph = graph;
            this.originalCapacity = capacity;
            this.originalTimeLimit = timeLimit;
        }

        public void Run()
        {
            var maxIterations = int.Parse(ConfigurationManager.AppSettings["MaxIterations"]);

            for (int j = 0; j < maxIterations; j++)
            {
                var localSolution = this.MakeLocalSolution(resultSeed);
                if(localSolution != null)
                {
                    localSolution.SimulateTravel(this.originalGraph, this.originalCapacity, this.originalTimeLimit);
                }

                if (localSolution != null && localSolution.IsBetter(resultSeed)) resultSeed = localSolution;                
            }
            
        }

        public GraphEnvironment MakeLocalSolution(GraphEnvironment environment)
        {
            var result = environment.Clone();

            var randomTruck = environment.Trucks[r.Next(0, environment.Trucks.Count)];
            var randomArcIndex = r.Next(0, randomTruck.ArcsTravel.Count);
            var randomArc = randomTruck.ArcsTravel[randomArcIndex];

            randomTruck.ArcsTravel.Remove(randomArc);

            //devolver lista de arcos con camino de first a second
            IList<Arc> newRoad = this.GetRoadFromTo(randomArc.first, randomArc.second, environment.Graph);

            if(newRoad.Count == 0) { return null; }

            var firstRoad = randomTruck.ArcsTravel.Take(randomArcIndex-1);
            var sndRoad = randomTruck.ArcsTravel.Skip(randomArcIndex-1);

            var otherList = new List<Arc>();
            foreach(var arc in firstRoad)
            {
                otherList.Add(arc);
            }

            foreach (var arc in newRoad)
            {
                otherList.Add(arc);
            }

            foreach (var arc in sndRoad)
            {
                otherList.Add(arc);
            }
            
            randomTruck.ArcsTravel = otherList;

            this.RemakeNodeTravel(randomTruck);

            //checkear environment
            if (!IsFeasible(environment)) { result = null; }

            return result;
            
        }

        private IList<Arc> GetRoadFromTo(Node first, Node second, Graph graph)
        {
            return DijkstraShortestRouteService.CalculateRoadFromTo(first, second, graph);
        }


        private bool IsFeasible(GraphEnvironment environment)
        {
            var result = true;
            
            foreach(var truck in environment.Trucks)
            {
                if (!IsValidTravel(truck, environment)) return false ;
            }
            
            return result;
        }

        private bool IsValidTravel(Truck truck,  GraphEnvironment environment)
        {
            var totalcost = truck.ArcsTravel.Sum(x => x.Cost);
            var totaldemand = truck.ArcsTravel.Sum(x => x.Demand);

            return truck.Capacity <= totaldemand && truck.TimeLimit <= totalcost;
        }


        private void RemakeNodeTravel(Truck truck)
        {

            var result = new List<Node>();

            truck.ArcsTravel.ForEach(x=> 
            {
                if (truck.ArcsTravel.IndexOf(x) == 0)
                {
                    result.Add(x.first);
                    result.Add(x.second);
                }
                else
                {
                    result.Add(x.second);
                }
                
            });

            truck.Travel = result;

        }

    }
}
