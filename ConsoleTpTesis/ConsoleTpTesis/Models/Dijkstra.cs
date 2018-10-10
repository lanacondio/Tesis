using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Models
{
    public class Dijkstra
    {
        //similar a los defines de C++
        private int INF = 1 << 30;  //definimos un valor grande que represente la distancia infinita inicial, basta conque sea superior al maximo valor del peso en alguna de las aristas

        private Dictionary<int, List<Node>> ady = new Dictionary<int,List<Node>>(); //lista de adyacencia
        private int[] distancia = new int[10005];          //distancia[ u ] distancia de vértice inicial a vértice con ID = u
        private bool[] visitado = new bool[10005];   //para vértices visitados
        private List<Node> Q = new List<Node>(); //priority queue propia de Java, usamos el comparador definido para que el de menor valor este en el tope
        private int V;                                      //numero de vertices
        private int[] previo = new int[10005];              //para la impresion de caminos
        private bool dijkstraEjecutado;

        public Dijkstra()
        {            
            dijkstraEjecutado = false;
        }

        private void Inicio(Graph graph)
        {
            this.V = graph.Nodes.Count;
            for (int i = 0; i <= V; ++i)
            {
                distancia[i] = INF;  //inicializamos todas las distancias con valor infinito
                visitado[i] = false; //inicializamos todos los vértices como no visitados
                previo[i] = -1;      //inicializamos el previo del vertice i con -1
            }

            foreach (var node in graph.Nodes)
            {
                var arcs = graph.Arcs.Where(x => x.first.Id == node.Id
                 || x.second.Id == node.Id).ToList();
                var adyacencias = arcs.Select(y =>
                {
                    if (y.first.Id == node.Id)
                    {
                        return y.second;
                    }
                    else
                    {
                        return y.first;
                    } }).ToList();

                ady.Add(node.Id, adyacencias);
            }
            
        }

        //Paso de relajacion
        private void relajacion(int actual, Node adyacente, int peso)
        {
            //Si la distancia del origen al vertice actual + peso de su arista es menor a la distancia del origen al vertice adyacente
            if (distancia[actual] + peso < distancia[adyacente.Id])
            {
                distancia[adyacente.Id] = distancia[actual] + peso;  //relajamos el vertice actualizando la distancia
                previo[adyacente.Id] = actual;                         //a su vez actualizamos el vertice previo
                Q.Add(adyacente); //agregamos adyacente a la cola de prioridad
            }
        }

        public void Run(int inicial, Graph graph)
        {
            Inicio(graph); //inicializamos nuestros arreglos
            Q.Add(graph.Nodes.Where(x => x.Id == inicial).FirstOrDefault()); //Insertamos el vértice inicial en la Cola de Prioridad
            distancia[inicial] = 0;      //Este paso es importante, inicializamos la distancia del inicial como 0
            int  adyacente, peso;
            Node actual;
            while (Q.Any())
            {                   //Mientras cola no este vacia
                actual = Q.Where(y => y.ShortestRoute == Q.Min(x => x.ShortestRoute)).FirstOrDefault();             //Obtengo de la cola el nodo con menor peso, en un comienzo será el inicial
                Q.Remove(Q.Where(x=> x.Id == actual.Id).FirstOrDefault());                           //Sacamos el elemento de la cola
                if (visitado[actual.Id]) continue; //Si el vértice actual ya fue visitado entonces sigo sacando elementos de la cola
                visitado[actual.Id] = true;         //Marco como visitado el vértice actual

                for (int i = 0; i < ady[actual.Id].Count; ++i)
                { //reviso sus adyacentes del vertice actual
                    adyacente = ady[actual.Id][i].Id;   //id del vertice adyacente
                    peso = graph.Arcs.Where(x => (x.first.Id == actual.Id && x.second.Id ==adyacente)
                    || (x.first.Id == adyacente && x.second.Id == actual.Id)).FirstOrDefault().Cost;        //peso de la arista que une actual con adyacente ( actual , adyacente )

                    if (!visitado[adyacente])
                    {        //si el vertice adyacente no fue visitado
                        relajacion(actual.Id, graph.Nodes.Where(x => x.Id == adyacente).FirstOrDefault(), peso); //realizamos el paso de relajacion
                    }
                }
            }

            foreach (var node in graph.Nodes)
            {
                node.ShortestPredecesor = graph.Nodes.Where(x => x.Id == previo[node.Id]).FirstOrDefault();
                node.ShortestRoute = distancia[node.Id];
            }

            dijkstraEjecutado = true;
        }

        public IList<Arc> RoadFromTo(Node first,Node second,Graph graph)
        {
            Inicio(graph);
            Q.Add(graph.Nodes.Where(x => x.Id == first.Id).FirstOrDefault()); //Insertamos el vértice inicial en la Cola de Prioridad
            distancia[first.Id] = 0;      //Este paso es importante, inicializamos la distancia del inicial como 0
            int adyacente, peso;
            Node actual;
            while (Q.Any())
            {                   //Mientras cola no este vacia
                actual = Q.Where(y => y.ShortestRoute == Q.Min(x => x.ShortestRoute)).FirstOrDefault();             //Obtengo de la cola el nodo con menor peso, en un comienzo será el inicial
                if (actual == null) actual = first;
                Q.Remove(Q.Where(x => x.Id == actual.Id).FirstOrDefault());                           //Sacamos el elemento de la cola
                if (visitado[actual.Id]) continue; //Si el vértice actual ya fue visitado entonces sigo sacando elementos de la cola
                visitado[actual.Id] = true;         //Marco como visitado el vértice actual

                for (int i = 0; i < ady[actual.Id].Count; ++i)
                { //reviso sus adyacentes del vertice actual
                    adyacente = ady[actual.Id][i].Id;   //id del vertice adyacente
                    peso = graph.Arcs.Where(x => (x.first.Id == actual.Id && x.second.Id == adyacente)
                    || (x.first.Id == adyacente && x.second.Id == actual.Id)).FirstOrDefault().Cost;        //peso de la arista que une actual con adyacente ( actual , adyacente )

                    if (!visitado[adyacente])
                    {        //si el vertice adyacente no fue visitado
                        relajacion(actual.Id, graph.Nodes.Where(x => x.Id == adyacente).FirstOrDefault(), peso); //realizamos el paso de relajacion
                    }
                }
            }

            foreach (var node in graph.Nodes)
            {
                node.ShortestPredecesor = graph.Nodes.Where(x => x.Id == previo[node.Id]).FirstOrDefault();
                node.ShortestRoute = distancia[node.Id];
            }

            var resultReversed = new List<Arc>();
            var actualNode = graph.Nodes.Where(x => x.Id == second.Id).FirstOrDefault();
            while(actualNode.Id != first.Id)
            {
                if(actualNode.Id == first.Id)
                {
                    resultReversed.Reverse();
                }

                if (actualNode.ShortestPredecesor != null)
                {
                    var arcToAdd = graph.Arcs.Where(x => (x.first.Id == actualNode.Id && x.second.Id == actualNode.ShortestPredecesor.Id)
                                                    || (x.second.Id == actualNode.Id && x.first.Id == actualNode.ShortestPredecesor.Id)).FirstOrDefault();

                    resultReversed.Add(arcToAdd);
                    actualNode = actualNode.ShortestPredecesor;
                }
                else
                {
                    return new List<Arc>();
                }
              
            }

            resultReversed.Reverse();

            return resultReversed;            
        }
    };

}
