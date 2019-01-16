using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class FloydWarshall
    {

        public const int INF = 99999;

        public FloydWarshall()
        {            
        
        }

        
        private static void Print(int[,] distance, int verticesCount)
        {
            Console.WriteLine("Shortest distances between every pair of vertices:");

            for (int i = 0; i < verticesCount; ++i)
            {
                for (int j = 0; j < verticesCount; ++j)
                {
                    if (distance[i, j] == INF)
                        Console.Write("INF".PadLeft(7));
                    else
                        Console.Write(distance[i, j].ToString().PadLeft(7));
                }

                Console.WriteLine();
            }
        }

        public void Run(Graph graph, int verticesCount)
        {
            int[,] distance = new int[verticesCount, verticesCount];

            for (int i = 0; i < verticesCount; ++i)
                for (int j = 0; j < verticesCount; ++j)
                {
                    var arc = graph.Arcs.Where(x => (x.first.Id == i && x.second.Id == j)
                    || (x.first.Id == j && x.second.Id == i)).FirstOrDefault();

                    if(arc != null)
                    {
                        distance[i, j] = arc.Cost;
                    }
                    else
                    {
                        distance[i, j] = INF;
                    }
                }
                    
                    
                    //ver con  null
                
            for (int k = 0; k < verticesCount; ++k)
            {
                for (int i = 0; i < verticesCount; ++i)
                {
                    for (int j = 0; j < verticesCount; ++j)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                            distance[i, j] = distance[i, k] + distance[k, j];
                    }
                }
            }

            //revisar
            for (int k = 0; k < verticesCount; ++k)
            {
                distance[k, k] = 0;
            }


            for (int i= 0; i<verticesCount; i++)
            {
                graph.Nodes[i].Distances = new List<int>();
                for(int j=0; j < verticesCount; j++)
                {
                    graph.Nodes[i].Distances.Add(distance[i, j]);
                }

            }
            
        }

    };

}
