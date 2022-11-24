using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;  
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using Gurobi;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace FHModel
{
#pragma warning disable CA1822 // Marcar membros como estáticos
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula
    class Node
    {
        public string? NodeName;
        public int Index;
        public double CoordX;
        public double CoordY;
        public bool IsFMU;
        public bool IsExit;

        public Node(double x, double y, int ind=0, bool isfmu = false, bool isexit = false, string name = "")
        {
            NodeName = name;
            Index = ind;
            CoordX = x;
            CoordY = y;
            IsFMU = isfmu;
            IsExit = isexit;
        }
        public override string ToString()
        {
            return $"Node {Index} ({CoordX},{CoordY}) FMU {IsFMU}";
        }
    }

    class FMU
    {
        public string Name;
        public int IndexFMU;
        public double InitialAge;
        public double Area;
        public Node node;
        public List<int> AdjFMus;
        public List<double> Vols;
        public List<double> Prof;
        public List<int> TrucksByPeriod;
        public double[]? LPV;
        public bool IsFirstBlock = false;

        public FMU(string name, double area, double initialAge, double X, double Y, 
                   int ind=0, int ind_node = 0)
        {
            Name = name;
            InitialAge = initialAge;
            Area = area;
            IndexFMU = ind;
            node = new Node(X, Y, ind_node, true);
            AdjFMus = new List<int>();
            Vols = new List<double>();
            Prof = new List<double>();
            TrucksByPeriod = new List<int>();
        }
        public FMU(string name, double area, double initialAge, Node Node, int ind = 0)
        {
            Name = name;
            InitialAge = initialAge;
            Area = area;
            IndexFMU = ind;
            node = Node;
            AdjFMus = new List<int>();
            Vols = new List<double>();
            Prof = new List<double>();
            TrucksByPeriod = new List<int>();
        }
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("FMU Name: \t" + Name);
            sb.AppendLine(" Index: \t" + IndexFMU.ToString());
            sb.AppendLine(" Initial Age: \t" + InitialAge.ToString());
            sb.AppendLine(" Area: \t" + Area.ToString());
            sb.AppendLine(" Node: \t" + node.ToString());
            return sb.ToString();
        }
        public void AddAdjFMU(int otherindex)
        {
            if (!(AdjFMus.Contains(otherindex)))
            {
                AdjFMus.Add(otherindex);
            }
        }
        public void RemoveAdjFMU(int otherindex)
        {
            if (AdjFMus.Contains(otherindex))
            {
                AdjFMus.Remove(otherindex);
            }
        }
        public void AddVols(double[] vols)
        {
            Vols.AddRange(vols);
        }
        public void ClearVols()
        {
            Vols.Clear();
        }
        public void AddProf(double[] prof)
        {
            Prof.AddRange(prof);
            this.LPV = new double[this.Prof.Count];
        }
        public void ClearProf()
        {
            Prof.Clear();
            LPV = Array.Empty<double>();
        }

        public void SetLPV(double[] TravelCost)
        {
            this.LPV[0] = this.Prof[0];
            for(int p = 0; p< this.Prof.Count - 1; p++)
            {
                this.LPV[p+1] = this.Prof[p+1] - TravelCost[p] * this.TrucksByPeriod[p]; 
            }
        }

    }
    class Adj
    {
        public Node? Node1;
        public Node? Node2;

        public Adj(Node? node1, Node? node2)
        {
            if (node1 is not null && node2 is not null)
            {
                Node1 = node1;
                Node2 = node2;
            }
        }

        public override string? ToString()
        {
            return $"\t{Node1}\n\t{Node2}";
        }
    }

        class Edge
    {
        public Node Node1;
        public Node Node2;
        public int? Index;
        public string Name;
        public double Lenght;
        public double Value;
        public bool PreExist;
        //public List<double> DemandByPeriod;

        public Edge(Node Start, Node End, double len, double val, bool preexist)//, List<double> demandbyperiod)
        {
            Node1 = Start;
            Node2 = End;
            Lenght = len;
            Value = val;
            PreExist = preexist;
            //DemandByPeriod = demandbyperiod;
            Name = $"({Start.NodeName},{End.NodeName})";
        }

        public override string ToString()
        {
            return $"Edge {Name}\tLenght {Lenght}\tValue {Value}\n\t{Node1}\n\t{Node2}\n";
        }
    }

    class DataMatrixPar
    {
        public double Cost_RoadOpening_ByUnity;
        public double Cost_RoadMaintenance_ByUnity;
        public double Cost_TravelFix_ByUnity;
        public double Cost_TravelsIJ_ByUnity;
        public int MaintenanceOpeningPeriods;
        public double VolumeMin;
        public double VolumeMax;
        public int Periods;
        public int MaxNumBlocks;
        public int[] FirstFMUs;
        public double[] MaximalHarvestArea;
        public double[] MinimalHarvestArea;
        public double[] Cost_RoadOpening_ByPeriod;
        public double[] Cost_RoadMaintenance_ByPeriod;
        public double[] Cost_TravelFix_ByPeriod;
        public double[] BigMS;
        public double[] BigME;
        public double[] SortimentDemand;
        public List<(int, int, int)>? SolTest;

        public DataMatrixPar(double cost_RoadOpening_ByUnity, double cost_RoadMaintenance_ByUnity,
                             double cost_TravelFix_ByUnity, double cost_TravelsIJ_ByUnity,
                             int maintenanceOpeningPeriods, double volumeMin, double volumeMax,
                             List<(string, double, double)> AreaAge,
                             double[] cost_RoadOpening_ByPeriod, double[] cost_RoadMaintenance_ByPeriod,
                             double[] cost_TravelFix_ByPeriod, double[] bigms, double[] bigme, double[] demand, 
                             List<int> firstFMUs, int periods, int numblocks, List<(int, int, int)>? solTest)
        {
            Cost_RoadOpening_ByUnity = cost_RoadOpening_ByUnity;
            Cost_RoadMaintenance_ByUnity = cost_RoadMaintenance_ByUnity;
            Cost_TravelFix_ByUnity = cost_TravelFix_ByUnity;
            Cost_TravelsIJ_ByUnity = cost_TravelsIJ_ByUnity;
            MaintenanceOpeningPeriods = maintenanceOpeningPeriods;
            MaxNumBlocks = numblocks>0 ? numblocks: Convert.ToInt16((AreaAge.Count / 2) + 1);
            Periods = periods;
            VolumeMin = volumeMin;
            VolumeMax = volumeMax;
            Cost_RoadOpening_ByPeriod = cost_RoadOpening_ByPeriod;
            Cost_RoadMaintenance_ByPeriod = cost_RoadMaintenance_ByPeriod;
            Cost_TravelFix_ByPeriod = cost_TravelFix_ByPeriod;
            double MinArea = AreaAge.Min(fmu => fmu.Item2);
            double TotArea = AreaAge.Sum(fmu => fmu.Item2);
            double MaxArea = TotArea / (double)MaxNumBlocks;
            MinimalHarvestArea = new double[Periods];
            MaximalHarvestArea = new double[Periods];
            MinimalHarvestArea[0] = 0;
            MaximalHarvestArea[0] = TotArea;
            BigMS = bigms;
            BigME = bigme;
            SortimentDemand = demand;
            for (int p=1; p <Periods; p++)
            {
                MinimalHarvestArea[p] = MinArea;
                MaximalHarvestArea[p] = MaxArea;
            }
            FirstFMUs = firstFMUs.ToArray();
            SolTest = solTest;
        }

        public override string ToString()
        {
            StringBuilder resp = new();
            StringBuilder L = new();
            resp.AppendLine("DataMatrixPar:");
            resp.AppendLine($"Cost_RoadOpening_ByUnity\t{Cost_RoadOpening_ByUnity.ToString()}");
            resp.AppendLine($"Cost_RoadMaintenance_ByUnity\t{Cost_RoadMaintenance_ByUnity.ToString()}");
            resp.AppendLine($"Cost_TravelFix_ByUnity\t{Cost_TravelFix_ByUnity.ToString()}");
            resp.AppendLine($"Cost_TravelsIJ_ByUnity\t{Cost_TravelsIJ_ByUnity.ToString()}");
            resp.AppendLine($"MaintenanceOpeningPeriods\t{MaintenanceOpeningPeriods.ToString()}");
            resp.AppendLine($"VolumeMin\t{VolumeMin.ToString()}");
            resp.AppendLine($"VolumeMax\t{VolumeMax.ToString()}");
            resp.AppendLine($"Periods\t{Periods.ToString()}");
            resp.AppendLine($"MaxNumBlocks\t{MaxNumBlocks.ToString()}");
            foreach(int f in FirstFMUs)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"FirstFMUs\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in MaximalHarvestArea)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"MaximalHarvestArea\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in MinimalHarvestArea)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"MinimalHarvestArea\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in Cost_RoadOpening_ByPeriod)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"Cost_RoadOpening_ByPeriod\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in Cost_RoadMaintenance_ByPeriod)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"Cost_RoadMaintenance_ByPeriod\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in Cost_TravelFix_ByPeriod)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"Cost_TravelFix_ByPeriod\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in BigMS)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"BigMS\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in BigME)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"BigME\t[{L.ToString()}]");
            L.Clear();
            foreach (double f in SortimentDemand)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"SortimentDemand\t[{L.ToString()}]");
            L.Clear(); 
            foreach (double f in SortimentDemand)
            {
                L.Append($"{f} ");
            }
            resp.AppendLine($"SortimentDemand\t[{L.ToString()}]");
            L.Clear();
            if (SolTest.Count>0)
            {
                foreach ((int, int, int) s in SolTest)
                {
                    L.Append($"({s.Item1}, {s.Item2}, {s.Item3}) ");
                }
                resp.AppendLine($"SolTest\t[{L.ToString()}]");
                L.Clear();
            }

            return resp.ToString();
        }
    }


    class FHStandards
    {
        public string NameInstance;
        public string PathInstance;
        public List<Node>? Nodes = new();
        public List<FMU>? FMUs = new();
        public List<Edge>? Edges = new();
        public List<Adj> Adjacentes = new();
        public DataMatrixPar MatrixPar;

        public FHStandards(string nameInstance,
                           List<(string, string)> adj,
                           List<(string, double, double, bool, bool)> Fmu,
                           List<(string, double, double)> AreaAge,
                           List<(string, List<double>)> Vol,
                           List<(string, string, double, double, bool)> Edg,
                           List<(string, List<double>)> Prf,
                           DataMatrixPar matrixPar)
        {
            NameInstance = nameInstance;
            PathInstance = ToolBox.Path(nameInstance, "C");
            MatrixPar = matrixPar;

            FillNodes(Fmu);
            AddAdjs(adj, Nodes);
            AddFmus(Nodes, AreaAge, MatrixPar);
            SetFMUVolume(Vol, FMUs);
            SetFMUProfit(Prf, FMUs);
            Edges = FillEdges(Edg, Nodes);
        }

        public FHStandards(string nameInstance,
                           List<(int, int)> adj,
                           List<(string, double, double, bool, bool)> Fmu,
                           List<(string, double, double)> AreaAge,
                           List<(string, List<double>)> Vol,
                           List<(string, string, double, double, bool)> Edg,
                           List<(string, List<double>)> Prf,
                           DataMatrixPar matrixPar)
        {
            NameInstance = nameInstance;
            PathInstance = ToolBox.Path(nameInstance, "C");
            MatrixPar = matrixPar;

            FillNodes(Fmu);
            AddAdjs(adj, Nodes);
            AddFmus(Nodes, AreaAge, MatrixPar);            
            SetFMUVolume(Vol, FMUs);
            SetFMUProfit(Prf, FMUs);
            Edges = FillEdges(Edg, Nodes);
        }

        private void FillNodes(List<(string, double, double, bool, bool)> FList)
        {
            //Setting Node list
            foreach ((string, double, double, bool, bool) F in FList)
            {
                Node N = new(F.Item2, F.Item3, Nodes.Count, F.Item4, F.Item5, F.Item1);
                Nodes.Add(N);
            }
        }

        private void AddAdjs(List<(int, int)> adj, List<Node> nodes)
        {
            Adjacentes = new();
            foreach ((int, int) a in adj)
            {
                Node? N1 = GetNodeByIndex(a.Item1, nodes);
                Node? N2 = GetNodeByIndex(a.Item2, nodes);
                if (N1 is not null && N2 is not null)
                {
                    Adj A1 = new(N1, N2);
                    Adj A2 = new(N2, N1);
                    if (!Adjacentes.Contains(A1))
                    {
                        Adjacentes.Add(A1);
                    }
                    if (!Adjacentes.Contains(A2))
                    {
                        Adjacentes.Add(A2);
                    }
                }
            }
        }

        private void AddAdjs(List<(string, string)> adj, List<Node> nodes)
        {
            Adjacentes = new();
            foreach ((string, string) a in adj)
            {
                Node? N1 = GetNodeByName(a.Item1, nodes);
                Node? N2 = GetNodeByName(a.Item2, nodes);
                if (N1 is not null && N2 is not null)
                {
                    Adj A1 = new(N1, N2);
                    Adj A2 = new(N2, N1);
                    if (!Adjacentes.Contains(A1))
                    {
                        Adjacentes.Add(A1);
                    }
                    if (!Adjacentes.Contains(A2))
                    {
                        Adjacentes.Add(A2);
                    }
                }
            }
        }

        private void AddFmus(List<Node> nodes, List<(string, double, double)> areaAge, DataMatrixPar matrixPar)
        {
            //Setting FMU list
            IEnumerable<Node> FmuNodes = nodes.Where(node => node.IsFMU);
            foreach (Node n in FmuNodes)
            {
                if (areaAge.Where(T => T.Item1 == n.NodeName).Any())
                {
                    (string, double, double) AA = areaAge.Where(T => T.Item1 == n.NodeName).First();
                    FMU F = new(AA.Item1, AA.Item2, AA.Item3, n, FMUs.Count)
                    {
                        //Setting if FMU is for First Block
                        IsFirstBlock = matrixPar.FirstFMUs.Contains(n.Index)
                    };
                    if (FMUs.Contains(F))
                    {
                        throw new Exception("FMU repetido");
                    }
                    else
                    {
                        FMUs.Add(F);
                    }
                }
            }
        }

        private void SetFMUVolume(List<(string, List<double>)> vol, List<FMU> fMUs)
        {
            //Setting Vol lits
            foreach ((string, List<double>) v in vol)
            {
                FMU? F = GetFMUByName(v.Item1, fMUs);
                if (F is not null)
                {
                    F.AddVols(v.Item2.ToArray());
                }
            }
        }

        private void SetFMUProfit(List<(string, List<double>)> prf, List<FMU> fmus)
        {
            //Setting Prf
            foreach ((string, List<double>) p in prf)
            {
                FMU? F = GetFMUByName(p.Item1, fmus);
                if (F is not null)
                {
                    F.AddProf(p.Item2.ToArray());
                }
            }
        }

        public List<Edge> FillEdges(List<(string, string, double, double, bool)> edg, List<Node> nodes)
        {
            List<Edge> Ed = new();
            //Setting Edges List
            foreach ((string, string, double, double, bool) e in edg)
            {
                Node? Start = GetNodeByName(e.Item1, nodes);
                Node? End = GetNodeByName(e.Item2, nodes);
                if (Start is not null && End is not null)
                {
                    Edge E0 = new(Start, End, e.Item3, e.Item4, e.Item5);//, Demand);
                    Edge E1 = new(End, Start, e.Item3, e.Item4, e.Item5);//, Demand);
                    foreach (Edge E in new List<Edge> { E0, E1 })
                    {
                        if (Ed.Contains(E))
                        {
                            throw new Exception($"{E.Name} repetido");
                        }
                        else
                        {
                            E.Index = Edges.Count;
                            Ed.Add(E);
                        }
                    }
                }
                else
                {
                    if (Start is null)
                    {   
                        throw new Exception($"Node {e.Item1} nao encontrado");
                    }
                    else
                    {
                        throw new Exception($"Node {e.Item2} nao encontrado");
                    }
                }
            }

            return Ed;
        }

        public Adj? GetAdjNodes(string name1, string name2, List<Node> NodeList)
        {
            try
            {
                Node? N1 = GetNodeByName(name1, NodeList);
                Node? N2 = GetNodeByName(name2, NodeList);
                return new Adj(N1, N2);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public (Node?, Node?) GetAdjNodes(int index1, int index2, List<Node> NodeList)
        {
            Node? N1 = GetNodeByIndex(index1, NodeList);
            Node? N2 = GetNodeByIndex(index2, NodeList);
            return (N1, N2);
        }
        public Node? GetNodeByName(string name, List<Node> NodeList)
        {
            try
            {
                var pickN = from nd in NodeList
                        where nd.NodeName == name
                        select nd;
                if (pickN.Any())
                {
                    return pickN.First();
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
        public Node? GetNodeByIndex(int Ind, List<Node>? NodeList)
        {
            try
            {
                var pickN = from nd in NodeList
                        where nd.Index == Ind
                        select nd;
                if (pickN.Any())
                {
                    return pickN.First();
                }
                else
                {
                    return null;
                    }
                }
                catch (ArgumentNullException)
                {
                    return null;
                }
            }
        public Node? GetNodeByIndex(string Ind, List<Node> NodeList)
        {
            return GetNodeByIndex(int.Parse(Ind), NodeList);
        }
        public FMU? GetFMUByName(string? name, List<FMU> fmus)
        {
            try
            {
                var pickN = from fm in fmus
                            where fm.Name == name
                            select fm;
                if (pickN.Any())
                {
                    return pickN.First();
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }

        }
        public FMU? GetFMUByNode(Node n, List<FMU> fmus)
        {
            try
            {
                var pickN = from fm in fmus
                            where fm.node == n
                            select fm;
                if (pickN.Any())
                {
                    return pickN.First();
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }

        }
        public FMU? GetFMUByIndex(int? Ind, List<FMU>? fmus)
        {
            try { 
                var pickN = from fm in fmus
                            where fm.IndexFMU == Ind
                            select fm;
                if (pickN.Any())
                {
                    return pickN.First();
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
        public FMU? GetFMUByNodeIndex(int? Ind, List<FMU>? fmus)
        {
            try
            {
                var pickN = from fm in fmus
                            where fm.node.Index == Ind
                            select fm;
                if (pickN.Any())
                {
                    return pickN.First();
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
        public FMU? GetFMUByIndex(string Ind, List<FMU> fmus)
        {
            return GetFMUByIndex(int.Parse(Ind), fmus);
        }

        public override string ToString()
        {
            StringBuilder TS = new();

            TS.AppendLine($"NameInstance\t{NameInstance}");
            TS.AppendLine($"PathInstance\t{PathInstance}");
            TS.AppendLine("FMUs:");
            if (FMUs.Count > 0)
            {
                foreach (FMU F in FMUs)
                {
                    TS.AppendLine($"\t\t{F.ToString()}");
                }
            }
            else
            {
                TS.AppendLine("Zero FMUs.");
            }
            TS.AppendLine("Nodes:");
            if (Nodes.Count > 0)
            {
                foreach(Node N in Nodes)
                {
                    if (!(N.IsFMU))
                    {
                        TS.AppendLine($"\t\t{N.ToString()}");
                    }                    
                }
            }
            else
            {
                TS.AppendLine("Zero Nodes.");
            }
            TS.AppendLine("Edges:");
            if (Edges.Count > 0)
            {
                foreach (Edge E in Edges)
                {
                    TS.AppendLine($"\t\t{E.ToString()}");
                }
            }
            else
            {
                TS.AppendLine("Zero Edges.");
            }
            TS.AppendLine("Adjacentes:");
            if (Adjacentes.Count > 0)
            {
                foreach (Adj a in Adjacentes)
                {
                    Node? N1 = a.Node1;
                    Node? N2 = a.Node2;
                    if (N1 is not null && N2 is not null)
                    {
                        TS.AppendLine($"\t\tN1: {N1}");
                        TS.AppendLine($"\t\tN2: {N2}\n");
                    }
                }
            }
            else
            {
                TS.AppendLine("Zero Adjs.");
            }
            TS.AppendLine($"\t{MatrixPar.ToString()}");

            return TS.ToString();
        }
    }

    class Instance
    {
        public string NameInstance;
        public string Agora;
        public FHStandards? Std;
        public GRBAuxVars Vars;
        public double DeltaDistance;
        public int NumNodes;
        public int NumEdges;
        public int NumPeriods;
        public double[,]? FMUDistances;
        public double[,]? FMUPredecessor;
        public List<int>[,]? Traj;
        public int[,]? SizeTraj;
        public double[,]? LiquidPresentValues;
        public double[,,]? SortimentVolumes;
        public double[,]? SortimentDemands;
        public double[,]? CostMatrix;
        public double[,,]? DemandMatrix;
        //Parameters
        public double OpenNewRoad = FHData.Par_OpenNewRoad;
        public double VolumeVariation = FHData.Par_VolumeVariation;
        public double DemFactor = FHData.Par_DemFactor;
        public double LoadingUnloadingCost = FHData.Par_LoadingUnloadingCost;
        public int MaxDistInBlock = FHData.Par_MaxDistInBlock;
        public int MaxDistConsecutivePeriod = FHData.Par_MaxDistConsecutivePeriod;
        public int MaxDistOutBlockInPeriod = FHData.Par_MaxDistOutBlockInPeriod;
        public int MinimalHarvestAge = FHData.Par_MinimalHarvestAge;
        public double NewBlockPercent = FHData.Par_NewBlockPercent;
        public int NumSortiments = FHData.Par_NumSortiments;
        public double TolFactor = FHData.Par_TolFactor;
        public int TruckCapacity = FHData.Par_TruckCapacity;
        public char[] charSeparators = FHData.Par_charSeparators;

        public Instance(FHStandards S)
        {
            Std = S;
            NameInstance = S.NameInstance;
            Agora = ToolBox.GetNow();
            Vars = new GRBAuxVars(S);

            for (int f = 0; f < Std.FMUs.Count; f++) {
                FMU F = Std.FMUs[f];
                F.TrucksByPeriod.Add(0);
                F.TrucksByPeriod.AddRange(from double vvol in F.Vols
                                          select ToolBox.TrucksByFMU(TruckCapacity, vvol));

                F.SetLPV(S.MatrixPar.Cost_TravelFix_ByPeriod);
            }

            NumNodes = Std.Nodes.Count;
            NumEdges = Std.Edges.Count;
            NumPeriods = Std.MatrixPar.Periods;
            (FMUDistances, FMUPredecessor, Traj, SizeTraj) = FW.DistFW(NumNodes, Std.Edges, NameInstance, $"({Agora})");
            // SetSortimentDemand(this.VolumeVariation);
            ReadSortimentDemand();
            SetDistances();
            SettingCostAndDemandMatrix();
        }

        private void ReadSortimentDemand()
        {
            DataMatrixPar DMP = this.Std.MatrixPar;
            SortimentDemands = new double[NumSortiments, NumPeriods];

            for (int s = 0; s < NumSortiments; s++)
            {
                foreach (int p in Enumerable.Range(0, DMP.SortimentDemand.Length))
                {
                    SortimentDemands[s, p] = DMP.SortimentDemand[p];
                }
            }
            ToolBox.FileTxt($"{this.NameInstance}[read_demand]{this.Agora}.txt",
                            ToolBox.WriteTable(SortimentDemands),
                            false);
        }

        public double CalcCost_TravelsIJ(int i, int j)
        {
            DataMatrixPar DMP = this.Std.MatrixPar;
            double resp = 0;
            if (Enumerable.Range(0, this.NumNodes).Contains(i)
                && Enumerable.Range(0, this.NumNodes).Contains(j)
                && i != j)
            {
                resp = FMUDistances[i, j] * DMP.Cost_TravelsIJ_ByUnity + DMP.Cost_TravelFix_ByUnity;
            }
            return resp;
        }
        public double CalcCost_RoadMaintenance(int i, int j, int p)
        {
            DataMatrixPar DMP = this.Std.MatrixPar;
            double resp = 0;
            if (Enumerable.Range(0, this.NumNodes).Contains(i)
                && Enumerable.Range(0, this.NumNodes).Contains(j)
                && Enumerable.Range(0, DMP.Cost_RoadMaintenance_ByPeriod.Length).Contains(p)
                && i != j)
            {
                resp = FMUDistances[i, j] * DMP.Cost_RoadMaintenance_ByUnity + DMP.Cost_RoadMaintenance_ByPeriod[p];
            }
            return resp;
        }
        public double CalcCost_RoadOpening(int i, int j, int p)
        {
            DataMatrixPar DMP = this.Std.MatrixPar;
            double resp = 0;
            if (Enumerable.Range(0, this.NumNodes).Contains(i)
                && Enumerable.Range(0, this.NumNodes).Contains(j)
                && Enumerable.Range(0, DMP.Cost_RoadOpening_ByPeriod.Length).Contains(p)
                && i != j)
            {
                resp = FMUDistances[i, j] * DMP.Cost_RoadOpening_ByUnity + DMP.Cost_RoadOpening_ByPeriod[p];
            }
            return resp;
        }
        
        private void SetDistances()
        {
            double TotalDistance = 0;
            double MinDistance = double.MaxValue;
            double MaxDistance = double.MinValue;

            for (int i = 0; i < NumNodes; i++)
            {
                for (int j = 0; j < NumNodes; j++)
                {
                    if (FMUDistances[i, j] < double.MaxValue)
                    {
                        if (FMUDistances[i, j] < MinDistance)
                        {
                            MinDistance = FMUDistances[i, j];
                        }
                        if (FMUDistances[i, j] > MaxDistance)
                        {
                            MaxDistance = FMUDistances[i, j];
                        }
                        TotalDistance += FMUDistances[i, j];
                    }
                }
            }
            DeltaDistance = MaxDistance - MinDistance;
        }
        public void SettingCostAndDemandMatrix()
        {
            CostMatrix = new double[this.NumNodes, this.NumNodes];
            //DemandMatrix = new double[this.NumPeriods, this.NumEdges, this.NumEdges];
            for(int i=0; i< this.NumNodes; i++)
            {
                CostMatrix[i, i] = OpenNewRoad;
                for (int j = i+1; j < this.NumNodes; j++)
                {
                    CostMatrix[i, j] = OpenNewRoad;
                    CostMatrix[j, i] = OpenNewRoad;
                }
            }

            foreach (Edge E in Std.Edges)
            {
                int E1 = E.Node1.Index;
                int E2 = E.Node2.Index;
                CostMatrix[E1, E2] = E.Value;
                CostMatrix[E2, E1] = E.Value;
                //foreach (int p in Enumerable.Range(0, this.NumPeriods))
                //{
                //    DemandMatrix[p, E1, E2] += E.DemandByPeriod.ToArray()[p];
                //    DemandMatrix[p, E2, E1] += E.DemandByPeriod.ToArray()[p];
                //}
            }
        }
        public void SetSortimentDemand(double DemandFactor)
        {
            SortimentDemands = new double[NumSortiments, NumPeriods];

            for (int s = 0; s < NumSortiments; s++)
            {
                foreach (int p in Enumerable.Range(0, NumPeriods))
                {
                    List<double> demand = (from FMU Unit in Std.FMUs
                                           let a = Unit.Vols.ToArray()[p]
                                           where a > 0
                                           select a).ToList();
                    SortimentDemands[s, p] = demand.Sum()>0? DemandFactor * demand.Average() : 0; // DemandFactor
                }
            }
            ToolBox.FileTxt($"{this.NameInstance}[demand]{this.Agora}.txt", 
                            ToolBox.WriteTable(SortimentDemands), 
                            false);
        }

        public override string ToString()
        {
            StringBuilder TS = new();
            TS.AppendLine($"NameInstance\t{NameInstance}");
            TS.AppendLine($"Agora\t{Agora}");
            TS.AppendLine($"------\nStd:\n{Std.ToString()}\n------");
            TS.AppendLine($"------\nVars:\n{Vars.ToString()}\n------");
            TS.AppendLine($"DeltaDistance\t{DeltaDistance}");
            TS.AppendLine($"NumNodes\t{NumNodes}");
            TS.AppendLine($"NumEdges\t{NumEdges}");
            TS.AppendLine($"NumPeriods\t{NumPeriods}");
            if (FMUDistances is not null)
            {
                TS.AppendLine($"FMUDistances\n{ToolBox.WriteTable(FMUDistances)}");
            }
            if (FMUPredecessor is not null)
            {
                TS.AppendLine($"FMUPredecessor\n{ToolBox.WriteTable(FMUPredecessor)}");
            }
            if (LiquidPresentValues is not null)
            {
                TS.AppendLine($"LiquidPresentValues\n{ToolBox.WriteTable(LiquidPresentValues)}");
            }
            if (SortimentVolumes is not null)
            {
                TS.AppendLine($"SortimentVolumes\n{ToolBox.WriteTable(SortimentVolumes)}");
            }
            if (SortimentDemands is not null)
            {
                TS.AppendLine($"SortimentDemands\n{ToolBox.WriteTable(SortimentDemands)}");
            }
            if (CostMatrix is not null)
            {
                TS.AppendLine($"CostMatrix\n{ToolBox.WriteTable(CostMatrix)}");
            }
            if (DemandMatrix is not null)
            {
                TS.AppendLine($"DemandMatrix\n{ToolBox.WriteTable(DemandMatrix)}");
            }
            TS.AppendLine("------\nParameters");
            TS.AppendLine("VolumeVariation = 0.1");
            TS.AppendLine("DemFactor = 0.2");
            TS.AppendLine("LoadingUnloadingCost = 3");
            TS.AppendLine("MaxDistInBlock = 2");
            TS.AppendLine("MaxDistConsecutivePeriod = 2");
            TS.AppendLine("MaxDistOutBlockInPeriod = 3");
            TS.AppendLine("MinimalHarvestAge = 2");
            TS.AppendLine("NewBlockPercent = 0.15");
            TS.AppendLine("NumSortiments = 1");
            TS.AppendLine("TolFactor = 0.5");
            TS.AppendLine("TruckCapacity = 12\n------");
            return TS.ToString();
        }

        public void SaveInstance()
        {
            ToolBox.FileTxt($"{NameInstance}_SaveInstance.txt", this.ToString(), false);
        }
#pragma warning restore CA1822 // Marcar membros como estáticos
#pragma warning restore CS8602
    }
}
