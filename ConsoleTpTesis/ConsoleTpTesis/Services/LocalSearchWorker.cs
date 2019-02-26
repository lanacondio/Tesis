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
                var localSolution = this.MakeLocalSolution(resultSeed, originalCapacity, originalTimeLimit);
                if(localSolution != null)
                {
                    localSolution.SimulateTravel(this.originalGraph, this.originalCapacity, this.originalTimeLimit);
                }

                if (localSolution != null && localSolution.IsBetter(resultSeed)) resultSeed = localSolution;                
            }
            
        }

        public GraphEnvironment MakeLocalSolution(GraphEnvironment environment, int originalCapacity, int originalTimeLimit)
        {
            try
            {
                var result = environment.Clone();

                var auxtrucks = new List<Truck>();
                foreach (var truck in environment.Trucks)
                {
                    var travel = new List<Node>();
                    var arcsTravel = new List<Arc>();
                    foreach (var arc in truck.ArcsTravel)
                    {
                        arcsTravel.Add(arc);
                    }

                    foreach (var node in truck.Travel)
                    {
                        travel.Add(node);
                    }

                    var auxTruck = new Truck()
                    {
                        ActualNode = 1,
                        ArcsTravel = arcsTravel,
                        Capacity = originalCapacity,
                        Id = truck.Id,
                        IsFinished = truck.IsFinished,
                        TimeLimit = originalTimeLimit,
                        Travel = travel
                    };

                    //auxTruck.Travel.Add(truck.Travel.Where(y => y.Id == auxTruck.ActualNode).FirstOrDefault());
                    auxtrucks.Add(auxTruck);

                }

                result.Trucks = auxtrucks;
                var randomTruck = result.Trucks[r.Next(0, environment.Trucks.Count)];

                /*agregar logica de nodos*/
                /*elijo el 1ero al azar*/
                var firstRandomNodeIndex = r.Next(0, randomTruck.Travel.Count);

                /*elijo el segundo entre los que no estan dentro del viaje*/
                var notSelectedNodes = environment.Graph.Nodes.Where(x => !randomTruck.Travel.Contains(x))
                                        .ToList();

                var secondRandomNodeIndex = r.Next(0, notSelectedNodes.Count);



                /*

                var firstRandomArcIndex = r.Next(0, randomTruck.ArcsTravel.Count);
                var secondRandomArcIndex = r.Next(0, randomTruck.ArcsTravel.Count);

                //ver que pasa con pocos arcos
                while(secondRandomArcIndex == firstRandomArcIndex)
                {
                    secondRandomArcIndex = r.Next(0, randomTruck.ArcsTravel.Count);
                }

                if(secondRandomArcIndex < firstRandomArcIndex)
                {
                    var auxidx = firstRandomArcIndex;
                    firstRandomArcIndex = secondRandomArcIndex;
                    secondRandomArcIndex = auxidx;
                }

                var indexesToRemove = GetArcsIndexBetween(firstRandomArcIndex,
                    secondRandomArcIndex);

                //var randomArcIndex = r.Next(0, randomTruck.ArcsTravel.Count);

                //if(randomArcIndex == 0 || randomArcIndex == randomTruck.ArcsTravel.Count - 1)
                //{
                //    return null;
                //}

                //var randomArc = randomTruck.ArcsTravel[randomArcIndex];
                //revisar caso de arcos repetidos, hay q revisar de cual viene realmente
                */
                var startNode = randomTruck.Travel[firstRandomNodeIndex]; //GetStartNodeFrom(firstRandomArcIndex, randomTruck.ArcsTravel);
                var endNode = notSelectedNodes[secondRandomNodeIndex]; // GetEndNodeFrom(secondRandomArcIndex, randomTruck.ArcsTravel);

                var auxTravel = new List<Arc>();

                for (int i = 0; i < firstRandomNodeIndex - 1; i++)
                {
                    auxTravel.Add(randomTruck.ArcsTravel[i]);
                }


                //randomTruck.ArcsTravel = auxTravel;
                //indexesToRemove.ForEach(x => randomTruck.ArcsTravel.RemoveAt(x));
                //randomTruck.ArcsTravel.RemoveAt(randomArcIndex);

                //devolver lista de arcos con camino de first a second

                //IList<Arc> newRoad = this.GetRoadFromTo(randomTruck.Travel[firstRandomNodeIndex],
                //    randomTruck.Travel[secondRandomNodeIndex],environment.Graph);
                IList<Arc> newRoad = this.GetRoadFromTo(startNode, endNode, environment.Graph);
                IList<Arc> newReturnRoad = this.GetRoadFromTo(endNode, environment.Graph.Nodes.Where(x => x.Id == 1).FirstOrDefault(), environment.Graph);

                if (newRoad.Count == 0) { return null; }

                var firstRoad = auxTravel;
                var sndRoad = newRoad;
                var thirdRoad = newReturnRoad;

                var otherList = new List<Arc>();

                foreach (var arc in firstRoad)
                {
                    otherList.Add(arc);
                }

                foreach (var arc in sndRoad)
                {
                    otherList.Add(arc);
                }

                foreach (var arc in thirdRoad)
                {
                    otherList.Add(arc);
                }

                randomTruck.ArcsTravel = otherList;

                result.Trucks.ToList().ForEach(x => this.RemakeNodeTravel(x));

                //checkear environment
                if (!IsFeasible(result)) { result = null; }

                return result;

            }
            catch(Exception ex)
            {
                return null;
            }
            
            
        }

        private Node GetStartNodeFrom(int arcIndex,List<Arc> arcs)
        {
            Node result;
            if(arcIndex == 0 || arcIndex == arcs.Count-1)
            {
                result =  arcs[0].first;
            }
            else
            {
                var arc = arcs[arcIndex];
                var prevArc = arcs[arcIndex - 1];

                if(arc.first.Id == prevArc.second.Id 
                    || arc.first.Id == prevArc.first.Id)
                {
                    result = arc.first;
                }
                else
                {
                    result = arc.second;
                }
            }

            return result;
        }

        private Node GetEndNodeFrom(int arcIndex, List<Arc> arcs)
        {
            Node result;
            if (arcIndex == 0 || arcIndex == arcs.Count - 1)
            {
                result = arcs[0].first;
            }
            else
            {
                var arc = arcs[arcIndex];
                var nextArc = arcs[arcIndex + 1];

                if (arc.first.Id == nextArc.second.Id
                    || arc.first.Id == nextArc.first.Id)
                {
                    result = arc.first;
                }
                else
                {
                    result = arc.second;
                }
            }

            return result;
        }


        private IList<Arc> GetRoadFromTo(Node first, Node second, Graph graph)
        {
            //return DijkstraShortestRouteService.CalculateRoadFromTo(first, second, graph);
            return this.CalculateRoadFromTo(first, second, graph);
        }

        private IList<Arc> CalculateRoadFromTo(Node first, Node second, Graph graph)
        {
            //revisar
            var result = new List<Arc>();
            var end = false;
            var actualNode = first;

            if (second.Id == 1) return result;

            while (!end)
            {
                var availableArcs = graph.Arcs.Where(x => x.first.Id == actualNode.Id || x.second.Id == actualNode.Id).ToList();

                var bestArc = availableArcs.FirstOrDefault();
                var minDistance = bestArc.first.Id == actualNode.Id ? bestArc.second.Distances[second.Id-1] :
                     bestArc.first.Distances[second.Id-1];

                for (int i = 0; i < availableArcs.Count(); i++)
                {
                    var actualDistance = availableArcs[i].first.Id == actualNode.Id ?
                        availableArcs[i].second.Distances[second.Id-1] :
                        availableArcs[i].first.Distances[second.Id-1];

                    if (actualDistance < minDistance)
                    {
                        minDistance = actualDistance;
                        bestArc = availableArcs[i];
                    }
                }

                result.Add(bestArc);

                if((bestArc.first.Id == actualNode.Id && bestArc.second.Distances[second.Id-1] >= actualNode.Distances[second.Id - 1])
                    ||
                    (bestArc.second.Id == actualNode.Id && bestArc.first.Distances[second.Id - 1] >= actualNode.Distances[second.Id - 1])
                    ) { return new List<Arc>(); }

                actualNode = bestArc.first.Id == actualNode.Id ? bestArc.second : bestArc.first;

                if (actualNode.Id == second.Id) end = true;
            }

            return result;
        }

        private List<int> GetArcsIndexBetween(int fstIndex,int sndIndex)
        {
            var result = new List<int>();

            int i = fstIndex;
            while(i <= sndIndex)
            {
                result.Add(i);
                i++;
            }

            return result;
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

            //return truck.Capacity >= totaldemand && truck.TimeLimit >= totalcost;
            return truck.TimeLimit >= totalcost && CheckedArcs(truck.ArcsTravel);
        }

        private bool CheckedArcs(List<Arc> travel)
        {
            var result = true;
            if((travel.First().first.Id != 1 && travel.First().second.Id != 1)
                ||(travel.Last().first.Id != 1 && travel.Last().second.Id != 1))
            {
                return false;
            }

            var actualNodeId = 1;
            for(int i = 0; i< travel.Count; i++)
            {
                if(travel[i].first.Id != actualNodeId 
                    && travel[i].second.Id != actualNodeId) { return false; }

                if (travel[i].first.Id == actualNodeId)
                {
                    actualNodeId = travel[i].second.Id;
                }
                else
                {
                    actualNodeId = travel[i].first.Id;
                }
            }

            return result;
        }

        private void RemakeNodeTravel(Truck truck)
        {

            var result = new List<Node>();

            Node last = null;
            var isFirst = true;
            truck.ArcsTravel.ForEach(x =>
            {
                if (isFirst)
                {
                    result.Add(x.first);
                    result.Add(x.second);
                    last = x.second;
                    isFirst = false;
                }
                else
                {
                    if (last.Id == x.first.Id)
                    {
                        result.Add(x.second);
                        last = x.second;
                    }
                    else
                    {
                        result.Add(x.first);
                        last = x.first;
                    }
                }

            });

            truck.Travel = result;

        }

    }
}
