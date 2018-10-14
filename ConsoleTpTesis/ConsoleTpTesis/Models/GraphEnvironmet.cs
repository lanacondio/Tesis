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

            var finished = false;

            var trucksToRoad = other.Trucks;

            int iterationNumber = 0;
            while (!finished)
            {
                
                foreach (var truck in trucksToRoad)
                {
                    this.SimulateNextStep(truck, other.Graph, iterationNumber);                    
                }

                finished = CheckStatus(trucksToRoad, iterationNumber);
                iterationNumber++;
            }

            other.AccumulatedProfit = AccumulatedProfit;
            
            var fValue = this.AccumulatedProfit;
            var sValue = other.AccumulatedProfit;
            return fValue > sValue;
        }


        public void SimulateNextStep(Truck truck, Graph graph, int iterationNumber)
        {
            if (iterationNumber < truck.ArcsTravel.Count)
            {
                var arcToSimulate = truck.ArcsTravel[iterationNumber];
                AccumulatedProfit += truck.AddSimulatedArcToTravel(arcToSimulate);
            }           
        }

        private bool CheckStatus(IList<Truck> trucks, int iterationNumber)
        {
            return trucks.All(x => iterationNumber >= x.ArcsTravel.Count);

        }

        public GraphEnvironment Clone()
        {
            var result = new GraphEnvironment();

            result.AccumulatedProfit = this.AccumulatedProfit;
            result.Graph = this.Graph;
            result.Trucks = this.Trucks;
            
            return result;
        }



    }
}
