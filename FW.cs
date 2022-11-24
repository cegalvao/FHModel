using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Globalization;

namespace FHModel
{
    class FW
    {
        public static (double[,], double[,], List<int>[,], int[,]) DistFW(int Dimensao, List<Edge> Edges, string name, string Agora)
        {
            StreamWriter FWFile = new StreamWriter($"{ToolBox.Constants.PathBase}\\FW_{name}{Agora}.txt");
            foreach (Edge E in Edges)
            {
                FWFile.WriteLine(E.ToString());
            }
            List<(int, int)> ListEdges = new List<(int, int)>();
            List<double> DistEdges = new List<double>();
            List<int>[,] Traj = new List<int>[Dimensao, Dimensao];
            int[,] sizetraj = new int[Dimensao, Dimensao];
            double[,] MatrizDistancias = new double[Dimensao, Dimensao];
            double[,] CopiaMatrizDistancias = new double[Dimensao, Dimensao];
            double[,] MatrizPredecessor = new double[Dimensao, Dimensao];
            //MatrizDistancias[0, 0] = 0; MatrizDistancias[0, 1] = 10; MatrizDistancias[0, 2] = 1000; MatrizDistancias[0, 3] = 1; MatrizDistancias[0, 4] = 1000; MatrizDistancias[0, 5] = 1000;
            //MatrizDistancias[1, 0] = 10; MatrizDistancias[1, 1] = 0; MatrizDistancias[1, 2] = 2; MatrizDistancias[1, 3] = 1000; MatrizDistancias[1, 4] = 4; MatrizDistancias[1, 5] = 1000;
            //MatrizDistancias[2, 0] = 1000; MatrizDistancias[2, 1] = 2; MatrizDistancias[2, 2] = 0; MatrizDistancias[2, 3] = 1000; MatrizDistancias[2, 4] = 1000; MatrizDistancias[2, 5] = 3;
            //MatrizDistancias[3, 0] = 1; MatrizDistancias[3, 1] = 1000; MatrizDistancias[3, 2] = 1000; MatrizDistancias[3, 3] = 0; MatrizDistancias[3, 4] = 1; MatrizDistancias[3, 5] = 1000;
            //MatrizDistancias[4, 0] = 1000; MatrizDistancias[4, 1] = 4; MatrizDistancias[4, 2] = 1000; MatrizDistancias[4, 3] = 1; MatrizDistancias[4, 4] = 0; MatrizDistancias[4, 5] = 2;
            //MatrizDistancias[5, 0] = 1000; MatrizDistancias[5, 1] = 1000; MatrizDistancias[5, 2] = 3; MatrizDistancias[5, 3] = 1000; MatrizDistancias[5, 4] = 2; MatrizDistancias[5, 5] = 0;

            double M = 0;

            foreach (Edge E in Edges)
            {
                ListEdges.Add((E.Node1.Index, E.Node2.Index));
                DistEdges.Add(E.Lenght);
                M += 2 * E.Lenght;
                if (!(ListEdges.Contains((E.Node2.Index, E.Node1.Index))))
                {
                    ListEdges.Add((E.Node2.Index, E.Node1.Index));
                    DistEdges.Add(E.Lenght);
                    M += 2 * E.Lenght;
                }
            }


            //Random Aleatorio = new Random(1);
            for (int i = 0; i < Dimensao; i++)
            {
                for (int j = 0; j < Dimensao; j++)
                {
                    if (i == j)
                    {
                        MatrizDistancias[i, j] = M;
                        MatrizPredecessor[i, j] = -1;
                    }
                    else
                    {
                        (int, int) pair = (i, j);
                        if (ListEdges.Contains(pair))
                        {
                            int EdgeIndex = ListEdges.IndexOf(pair);
                            MatrizDistancias[i, j] = DistEdges[EdgeIndex];
                            MatrizPredecessor[i, j] = j;
                        }
                        else
                        {
                            MatrizDistancias[i, j] = M;
                            MatrizPredecessor[i, j] = -1;
                        }
                    }
                    CopiaMatrizDistancias[i, j] = MatrizDistancias[i, j];
                }
            }
            FWFile.WriteLine("A matriz de distância original é:\n");
            FWFile.WriteLine(EscreveMatriz(MatrizDistancias));
            FWFile.WriteLine("A matriz de predecessores original é:");
            FWFile.WriteLine(EscreveMatriz(MatrizPredecessor));
            FWFile.WriteLine("--------------------------------------------------------------------------------------------------------------");
            //Console.WriteLine("Pressione qualquer tecla para continuar");
            //Console.ReadKey();
            //Console.WriteLine("inicio da execucao");
            //Stopwatch Cronometro = new Stopwatch();
            //Cronometro.Start();
            for (int k = 0; k < Dimensao; k++)
            {
                for (int i = 0; i < Dimensao; i++)
                {
                    for (int j = 0; j < Dimensao; j++)
                    {
                        if (k != i && k != j && i != j)
                        {
                            //Console.WriteLine("A menor distância conhecida para ir do ponto " + i.ToString() + " para o ponto " + j.ToString() + " é " + MatrizDistancias[i, j]);
                            //Console.WriteLine("Se passar pelo ponto " + k.ToString() + ", a distância fica " + MatrizDistancias[i, k] + "+" + MatrizDistancias[k, j] + "=" + (MatrizDistancias[i, k] + MatrizDistancias[k, j]).ToString());
                            if (MatrizDistancias[i, k] + MatrizDistancias[k, j] < MatrizDistancias[i, j])
                            {
                                MatrizDistancias[i, j] = MatrizDistancias[i, k] + MatrizDistancias[k, j];
                                MatrizPredecessor[i, j] = MatrizPredecessor[i, k];
                                //Console.WriteLine("Passar pelo ponto " + k.ToString() + " gera economia");
                                //Console.WriteLine("A nova matriz de distância é:");
                                //EscreveMatriz(MatrizDistancias);
                                //Console.WriteLine("A nova matriz de predecessores é:");
                                //EscreveMatriz(MatrizPredecessor);
                                //Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                                //Console.WriteLine("Pressione qualquer tecla para continuar");
                                //Console.ReadKey();
                            }
                            //Console.WriteLine();
                        }
                    }
                }
            }
            //Cronometro.Stop();
            //Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            //Console.WriteLine("--------------------------------------------------------------------------------------------------------------");

            //Console.WriteLine("Fim do algoritmo");
            //Console.WriteLine("O tempo de execucao foi de " + Cronometro.ElapsedMilliseconds + " milissegundos");

            (Traj, sizetraj) = EncontraTrajetorias(MatrizPredecessor);
            FWFile.WriteLine("A matriz final de distâncias mínimas entre os pontos é:");
            FWFile.WriteLine(EscreveMatriz(MatrizDistancias));
            FWFile.WriteLine("A matriz final de predecessores é:");
            FWFile.WriteLine(EscreveMatriz(MatrizPredecessor));
            FWFile.WriteLine("A matriz de Trajetórias é:");
            FWFile.WriteLine(EscreveMatriz(Traj));
            FWFile.WriteLine("A distância em arestas por blocos é:");
            FWFile.WriteLine(EscreveMatriz(sizetraj));
            FWFile.WriteLine("--------------------------------------------------------------------------------------------------------------");
            //Console.WriteLine("--------------------------------------------------------------------------------------------------------------");

            /*
            List<int>[,] MinhasTrajetorias = EncontraTrajetorias(MatrizPredecessor);
            for (int i = 0; i < Dimensao; i++)
            {
                for (int j = 0; j < Dimensao; j++)
                {
                    double CustoTotal = 0;
                    for (int p = 0; p < MinhasTrajetorias[i, j].Count - 1; p++)
                    {
                        CustoTotal += CopiaMatrizDistancias[MinhasTrajetorias[i, j][p], MinhasTrajetorias[i, j][p + 1]];
                    }
                    if (CustoTotal != MatrizDistancias[i, j])
                    {
                        Console.WriteLine("xxxxxxxxxxxx PROBLEMA xxxxxxxxxxxx");
                    }
                }
            }
            Console.WriteLine("Pressione qualquer tecla para encerrar");
            Console.ReadKey();*/
            FWFile.Close();
            return (MatrizDistancias, MatrizPredecessor, Traj, sizetraj);
        }
        static string EscreveMatriz(List<int>[,] Matriz)
        {
            StringBuilder Mat = new StringBuilder();
            for (int i = 0; i < Matriz.GetLength(0); i++)
            {
                for (int j = 0; j < Matriz.GetLength(1); j++)
                {
                    Mat.Append(ToolBox.GroupingListIntToString(Matriz[i, j]) + "\t");
                }
                Mat.Append('\n');
            }
            return Mat.ToString();
        }

        static string EscreveMatriz(int[,] Matriz)
        {
            StringBuilder Mat = new StringBuilder();
            for (int i = 0; i < Matriz.GetLength(0); i++)
            {
                for (int j = 0; j < Matriz.GetLength(1); j++)
                {
                    Mat.Append(Matriz[i, j].ToString() + "\t");
                }
                Mat.Append('\n');
            }
            return Mat.ToString();
        }

        static string EscreveMatriz(double[,] Matriz)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture("pt-br");
            StringBuilder Mat = new StringBuilder();
            for (int i = 0; i < Matriz.GetLength(0); i++)
            {
                for (int j = 0; j < Matriz.GetLength(1); j++)
                {
                    Mat.Append(Matriz[i, j].ToString("F04", ci) + "\t");
                }
                Mat.Append('\n');
            }
            return Mat.ToString();
        }
        public static (List<int>[,], int[,]) EncontraTrajetorias(double[,] matrizPred)
        {
            int Dimensao = matrizPred.GetLength(0);
            List<int>[,] Trajetorias = new List<int>[Dimensao, Dimensao];
            int[,] DistTraj = new int[Dimensao, Dimensao];
            for (int i = 0; i < Dimensao; i++)
            {
                for (int j = 0; j < Dimensao; j++)
                {
                    if (i == j)
                    {
                        Trajetorias[i, j] = new List<int>
                        {
                            -1
                        };
                    }
                    else
                    {
                        Trajetorias[i, j] = new List<int>
                        {
                            i
                        };
                        int UltimoAdicionado = i;
                        while (UltimoAdicionado != -1 && matrizPred[UltimoAdicionado, j] != j)
                        {
                            Trajetorias[i, j].Add((int)matrizPred[UltimoAdicionado, j]);
                            UltimoAdicionado = (int)matrizPred[UltimoAdicionado, j];
                        }
                        Trajetorias[i, j].Add(j);
                    }
                    DistTraj[i, j] = Trajetorias[i, j].Count - 1;
                }
            }
            return (Trajetorias, DistTraj);
        }
    }
}
