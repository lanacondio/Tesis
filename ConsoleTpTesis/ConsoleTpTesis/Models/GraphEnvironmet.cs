using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class GraphEnvironment
    {
        public Graph Graph { get; set; }
        public IList<Truck> Trucks { get; set; }

        public int AccumulatedProfit { get; set; }

        public bool IsBetter(GraphEnvironment other)
        {
            if (other == null) return false;
            
            var fValue = this.AccumulatedProfit;
            var sValue = other.AccumulatedProfit;
            return fValue > sValue;
        }


        public int SimulateNextStep(Truck truck, Graph graph, int iterationNumber)
        {
            var result = 0;
            if (iterationNumber < truck.ArcsTravel.Count)
            {
                var arcToSimulate = truck.ArcsTravel[iterationNumber];

                var originalArc = graph.Arcs.Where(x => x.first.Id == arcToSimulate.first.Id && x.second.Id == arcToSimulate.second.Id)
                    .FirstOrDefault();

                result = truck.AddSimulatedArcToTravel(originalArc);
            }
            return result;
        }

        public void SimulateTravel(Graph graph, int originalCapacity, int originalTimeLimit)
        {
            var accProfit = 0;
            this.Graph = graph.Clone();
            
            foreach (var truck in this.Trucks)
            {
                truck.Capacity = originalCapacity;
                truck.TimeLimit = originalTimeLimit;                
            }
            
            var finished = false;

            var trucksToRoad = this.Trucks;

            int iterationNumber = 0;
            while (!finished)
            {

                foreach (var truck in trucksToRoad)
                {
                    accProfit += this.SimulateNextStep(truck, this.Graph, iterationNumber);
                }

                finished = CheckStatus(trucksToRoad, iterationNumber);
                iterationNumber++;
            }

            this.AccumulatedProfit = accProfit;
        }


        private bool CheckStatus(IList<Truck> trucks, int iterationNumber)
        {
            return trucks.All(x => iterationNumber >= x.ArcsTravel.Count);

        }

        public GraphEnvironment Clone()
        {
            var result = new GraphEnvironment();

            result.AccumulatedProfit = this.AccumulatedProfit;
            result.Graph = this.Graph.Clone();

            var auxTrucks = new List<Truck>();

            foreach(var truck in this.Trucks)
            {
                auxTrucks.Add(truck.Clone());
            }

            result.Trucks = auxTrucks;
            
            return result;
        }



    }
}
