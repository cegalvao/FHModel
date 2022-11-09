using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gurobi;
using static System.Net.Mime.MediaTypeNames;

namespace FHModel
{
    class AuxVars
    {
        public GRBVar[,] v;
        public string name;
        public string grp;
        public double[,]? vsol;

        public override string ToString()
        {
            return name;
        }

        public AuxVars(GRBVar[,] v, string name, string group)
        {
            this.v = v ?? throw new ArgumentNullException(nameof(v));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.grp = group ?? throw new ArgumentNullException(nameof(group));
            this.vsol = new double[v.GetLength(0), v.GetLength(1)] 
                ?? throw new ArgumentNullException(nameof(v));
        }
    }
    class GRBAuxVars
    {
        public GRBVar[,,] X;
        public GRBVar[,,] Y;
        public GRBVar[,,] Z;
        public GRBVar[,,] W;
        public GRBVar[,,] U;
        public GRBVar[,] A;
        public GRBVar[] vars;
        public List<AuxVars> Aux;
        //public List<(string, int, int)>? indvars;
        public int NumNodes;
        public int NumPeriods;
        public int MaxNumBlocks;
        public int varslenght;

        public GRBAuxVars(FHStandards Std)
        {
            //int ind0 = 0;
            //indvars = new List<(string, int, int)>();
            this.NumNodes = Std.Nodes.Count; // dims[0];
            this.NumPeriods = Std.MatrixPar.Periods; // dims[1];
            this.MaxNumBlocks = Std.MatrixPar.MaxNumBlocks; // dims[2];
            this.varslenght = this.NumPeriods * ((this.NumNodes + 1) * this.MaxNumBlocks + 4 * this.NumNodes * this.NumNodes);
            this.vars = new GRBVar[this.varslenght];

            //X_FMU(i,p,b) = 1 if fmu i is harvested at p period and block b; 0 c.c.
            //Var X is dimensioned by Nodes. Will need set to Zero all non-FMUs Nodes
            this.X = new GRBVar[this.NumNodes, this.NumPeriods, this.MaxNumBlocks];
            //Y_RUse(i,j,p) = 1 if is used the road connecting stands i and j in period p; 0 c.c.
            this.Y = new GRBVar[this.NumNodes, this.NumNodes, this.NumPeriods];
            //Z_RMaint(i,j,p) = 1 if is maintained the road connecting stands i and j in period p; 0 c.c.
            this.Z = new GRBVar[this.NumNodes, this.NumNodes, this.NumPeriods];
            //W_ROpen(i,j,p) = 1 if there is an open road connecting stands i and j in period p; 0 c.c.
            this.W = new GRBVar[this.NumNodes, this.NumNodes, this.NumPeriods];
            //U_Trav(i,j,p) = amount of vehicles from i to j in period p;
            this.U = new GRBVar[this.NumNodes, this.NumNodes, this.NumPeriods];
            //Alpha(p,b) = 1 if is created block b in period p; 0 c.c.
            this.A = new GRBVar[this.NumPeriods, this.MaxNumBlocks];
            this.Aux = new List<AuxVars>();

        }
        public void FillAux() { 
            //Xip, Xib, Xpb; Wij, Wip, Wjp; Zij, Zip, Zjp; Yij, Yip, Yjp; Uij, Uip, Ujp, A
            int v = 0;
            for (int b = 0; b < this.MaxNumBlocks; b++)
            {
                GRBVar[,] _tempX = new GRBVar[this.NumNodes, NumPeriods];
                for (int i = 0; i < this.NumNodes; i++)
                {
                    for (int p = 0; p < NumPeriods; p++)
                    {
                        _tempX[i, p] = X[i, p, b];
                        vars[v] = X[i, p, b];
                        v++;
                    }
                }
                Aux.Add(new AuxVars(_tempX, $"Xip{b}", "Xip"));
            }
            for (int p = 0; p < NumPeriods; p++)
            {
                GRBVar[,] _tempX = new GRBVar[this.NumNodes, MaxNumBlocks];
                for (int i = 0; i < this.NumNodes; i++)
                {
                    for (int b = 0; b < MaxNumBlocks; b++)
                    {
                        _tempX[i, b] = X[i, p, b];
                    }
                }
                Aux.Add(new AuxVars(_tempX, $"Xi{p}b", "Xib"));
            }
            //indvars.Add(("Xib", ind0, v));
            //ind0 = v+1;
            for (int i = 0; i < this.NumNodes; i++)
            {
                GRBVar[,] _tempX = new GRBVar[NumPeriods, MaxNumBlocks];
                for (int p = 0; p < NumPeriods; p++)
                {
                    for (int b = 0; b < MaxNumBlocks; b++)
                    {
                        _tempX[p, b] = X[i, p, b];
                    }
                }
                Aux.Add(new AuxVars(_tempX, $"X{i}pb", "Xpb"));
            }
            //indvars.Add(("Xpb", ind0, v));
            //ind0 = v+1;
            GRBVar[,] _tempA = new GRBVar[NumPeriods, MaxNumBlocks];
            for (int p = 0; p < NumPeriods; p++)
            {
                for (int b = 0; b < MaxNumBlocks; b++)
                {
                    _tempA[p, b] = A[p, b];
                    vars[v] = A[p, b];
                    v++;
                }
            }
            Aux.Add(new AuxVars(_tempA, "A", "A"));
            //indvars.Add(("A", ind0, v));
            //ind0 = v + 1;

            for (int p = 0; p < NumPeriods; p++)
            {
                GRBVar[,] _tempW = new GRBVar[this.NumNodes, this.NumNodes];
                GRBVar[,] _tempY = new GRBVar[this.NumNodes, this.NumNodes];
                GRBVar[,] _tempZ = new GRBVar[this.NumNodes, this.NumNodes];
                GRBVar[,] _tempU = new GRBVar[this.NumNodes, this.NumNodes];
                for (int i = 0; i < this.NumNodes; i++)
                {
                    for (int j = 0; j < this.NumNodes; j++)
                    {
                        _tempW[i, j] = W[i, j, p];
                        _tempY[i, j] = Y[i, j, p];
                        _tempZ[i, j] = Z[i, j, p];
                        _tempU[i, j] = U[i, j, p];
                        vars[v] = W[i, j, p];
                        v++;
                        vars[v] = Y[i, j, p];
                        v++;
                        vars[v] = Z[i, j, p];
                        v++;
                        vars[v] = U[i, j, p];
                        v++;
                    }
                }
                Aux.Add(new AuxVars(_tempW, $"Wij{p}", "Wij"));
                Aux.Add(new AuxVars(_tempY, $"Yij{p}", "Yij"));
                Aux.Add(new AuxVars(_tempZ, $"Zij{p}", "Zij"));
                Aux.Add(new AuxVars(_tempU, $"Uij{p}", "Uij"));
            }
            //indvars.Add(("WYZU", ind0, v));
            //ind0 = v + 1;
            for (int i = 0; i < NumNodes; i++)
            {
                GRBVar[,] _tempWi = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempYi = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempZi = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempUi = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempWj = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempYj = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempZj = new GRBVar[this.NumNodes, this.NumPeriods];
                GRBVar[,] _tempUj = new GRBVar[this.NumNodes, this.NumPeriods];
                for (int j = 0; j < this.NumNodes; j++)
                {
                    for (int p = 0; p < NumPeriods; p++)
                    {
                        _tempWi[j, p] = W[i, j, p];
                        _tempYi[j, p] = Y[i, j, p];
                        _tempZi[j, p] = Z[i, j, p];
                        _tempUi[j, p] = U[i, j, p];
                        _tempWj[j, p] = W[j, i, p];
                        _tempYj[j, p] = Y[j, i, p];
                        _tempZj[j, p] = Z[j, i, p];
                        _tempUj[j, p] = U[j, i, p];
                    }
                }
                Aux.Add(new AuxVars(_tempWi, $"W{i}jp", "Wjp"));
                Aux.Add(new AuxVars(_tempYi, $"Y{i}jp", "Yjp"));
                Aux.Add(new AuxVars(_tempZi, $"Z{i}jp", "Zjp"));
                Aux.Add(new AuxVars(_tempUi, $"U{i}jp", "Ujp"));
                Aux.Add(new AuxVars(_tempWj, $"Wi{i}p", "Wip"));
                Aux.Add(new AuxVars(_tempYj, $"Yi{i}p", "Yip"));
                Aux.Add(new AuxVars(_tempZj, $"Zi{i}p", "Zip"));
                Aux.Add(new AuxVars(_tempUj, $"Ui{i}p", "Uip"));
            }
            NewVSol();
        }
        public void NewVSol()
        {
            foreach (AuxVars grbv in Aux)
            {
                grbv.vsol = new double[grbv.v.GetLength(0), grbv.v.GetLength(1)];
            }
        }

        public GRBVar[][,] GetVarsByGroup(string grp)
            {
                IEnumerable<GRBVar[,]> R = from av in Aux
                                           where av.grp == grp
                                           select av.v;
                return R.ToArray();
            }
        public double[][,] GetVSolByGroup(string grp)
        {
            IEnumerable<double[,]> R = from av in Aux
                                        where av.grp == grp
                                        select av.vsol;
            return R.ToArray();
        }

        public override string? ToString()
        {
            return $"Qtde Vars\t{varslenght}";
        }
    }

    class ModelRotaFH
    {
        //Variaveis de geracao do modelo Gurobi
        public GRBEnv? Ambiente; //{ get; set; }
        public GRBModel? Modelo; //{ get; set; }
        //Variável de contagem do tempo de execução
        //public Stopwatch Cronometro; //{ get; set; }

        //Resultado do Modelo
        public Tuple<bool, string, string> resp = new Tuple<bool, string, string>(false, "Modelo Novo não resolvido", "New");
        public double ModelObjVal;
        public Tuple<bool, string, string> resp_relax = new Tuple<bool, string, string>(false, "Modelo Relaxado Novo não resolvido", "New Relax");
        //public double ModelObjVal_relax;

        //Parametros do Problema;
        public ModelParameters MP;
        public string[] NameVar = ToolBox.Constants.NameVar;
        public int NI;
        public int NP;
        public int NB;
        public int NE;
        public int NS;
        public int MOP; //MaintenanceOpeningPeriods
        public ModelRotaFH(ModelParameters Param)
        {
            MP = Param;
            NI = MP.DataInst.Std.Nodes.Count; //NumNodes
            NP = MP.DataInst.Std.MatrixPar.Periods; //Periods
            NB = MP.DataInst.Std.MatrixPar.MaxNumBlocks; //MaxNumBlocks
            NE = MP.DataInst.Std.Edges.Count; //NumEdges
            NS = 1; //NumSortimens
            MOP = MP.DataInst.Std.MatrixPar.MaintenanceOpeningPeriods; //MaintenanceOpeningPeriods
            CreateModelRotaFH();
        }

        public void WriteLog(bool UseLog, StringBuilder[] sb, string text)
        {
            if (UseLog)
            {
                ToolBox.AppendLineSbCapacity(sb, text);
            }
        }

        public void CreateModelRotaFH()
        {
            //LogTxt
            StringBuilder[] logtxt = new StringBuilder[5];
            for (int i = 0; i < logtxt.Length; i++)
            {
                logtxt[i] = new StringBuilder();
            }

            //Var aux
            Instance I = MP.DataInst;
            FHStandards S = I.Std;
            List<(string, int)> AuxIndexes;
            string ConstName;
            bool HarvestSchedulingOptimize = true;
            int[] CoefSlackExcess = new int[MP.SlackExcessVars.Length];
            for (int i = 0; i < MP.SlackExcessVars.Length; i++)
            {
                CoefSlackExcess[i] = MP.SlackExcessVars[i] ? 1 : 0;
            }

            char VarType = GRB.CONTINUOUS;
            if (MP.BinaryVars)
            {
                VarType = GRB.BINARY;
            }


            string NomeArquivo;
            //Arquivos do Modelo
            if (MP.Hybrid)
            {
                NomeArquivo = "_Hyb_" + I.NameInstance;
            }
            else
            {
                if (HarvestSchedulingOptimize)
                {
                    NomeArquivo = "_HSO_" + I.NameInstance;
                }
                else
                {
                    NomeArquivo = "_VRP_" + I.NameInstance;
                }
            }

            WriteLog(MP.Uselog, logtxt, $"Nome: {NomeArquivo}");
            WriteLog(MP.Uselog, logtxt, $"Data: {I.Agora}");

            //StringBuilder CountConstraint = new StringBuilder();
            int[] CounterConstraint = new int[MP.UseConstraints.Length];
            int[] CounterVar = new int[NameVar.Length];
            //StringBuilder CountName = new StringBuilder();

            //using StreamWriter fileall = new($"{S.PathInstance}[all]{NomeArquivo}{I.Agora}0.txt");
            //using StreamWriter fileCountConst = new($"{S.PathInstance}[CountConst]{NomeArquivo}{I.Agora}.txt");

            Ambiente = new GRBEnv
            {
                LogFile = $"{S.PathInstance}{NomeArquivo}{I.Agora}.log"
            };

            Modelo = new GRBModel(Ambiente)
            {
                ModelSense = GRB.MINIMIZE,
                ModelName = NomeArquivo + I.Agora
            };

            if (MP.ModelSenseMax)
            {
                Modelo.ModelSense = GRB.MAXIMIZE;
            }

            // Must set LazyConstraints parameter when using lazy constraints

            Modelo.Parameters.LazyConstraints = 1;

            //Create Variables
            //X_FMU(i,p,b) = 1 if fmu i is harvested at p period and block b; 0 c.c.
            GRBVar[,,] X_FMU = I.Vars.X;

            //W_ROpen(i,j,p) = 1 if there is an open road connecting stands i and j in period p; 0 c.c.
            GRBVar[,,] W_ROpen = I.Vars.W;

            //Z_RMaint(i,j,p) = 1 if is maintained the road connecting stands i and j in period p; 0 c.c.
            GRBVar[,,] Z_RMaint = I.Vars.Z;

            //Y_RUse(i,j,p) = 1 if is used the road connecting stands i and j in period p; 0 c.c.
            GRBVar[,,] Y_RUse = I.Vars.Y;

            //U_Trav(i,j,p) = amount of vehicles from i to j in period p;
            GRBVar[,,] U_Trav = I.Vars.U;

            //Alpha(p,b) = 1 if is created block b in period p; 0 c.c.
            GRBVar[,] Alpha = I.Vars.A;

            //Variaveis Falta e excesso

            GRBVar[,,] Slk_M_FirstOpen = new GRBVar[NI, NI, NP];
            GRBVar[,,] Exc_M_FirstOpen = new GRBVar[NI, NI, NP];

            GRBVar[,,] Slk_M_Maintenance = new GRBVar[NI, NI, NP];
            GRBVar[,,] Exc_M_Maintenance = new GRBVar[NI, NI, NP];

            GRBVar[,,] Slk_M_maint_open = new GRBVar[NI, NI, NP];
            GRBVar[,,] Exc_M_maint_open = new GRBVar[NI, NI, NP];

            GRBVar[,,] Slk_M_vehicle = new GRBVar[NI, NI, NP];
            GRBVar[,,] Exc_M_vehicle = new GRBVar[NI, NI, NP];

            GRBVar[,,] Slk_M_use = new GRBVar[NI, NI, NP];
            GRBVar[,,] Exc_M_use = new GRBVar[NI, NI, NP];

            GRBVar[,] Slk_M_acess = new GRBVar[NI, NP];
            GRBVar[,] Exc_M_acess = new GRBVar[NI, NP];

            GRBVar[,] Slk_M_flow = new GRBVar[NI, NP];
            GRBVar[,] Exc_M_flow = new GRBVar[NI, NP];

            GRBVar[] Slk_M_AgC_qtharvest = new GRBVar[NI];
            GRBVar[] Exc_M_AgC_qtharvest = new GRBVar[NI];

            GRBVar[] Slk_M_AgC_initialblock = new GRBVar[NI];
            GRBVar[] Exc_M_AgC_initialblock = new GRBVar[NI];

            GRBVar[,,] Slk_M_AgC_ageblock = new GRBVar[NI, NP, NB];
            GRBVar[,,] Exc_M_AgC_ageblock = new GRBVar[NI, NP, NB];

            GRBVar[,] Slk_M_AgC_varvolmin = new GRBVar[NS, NP];
            GRBVar[,] Exc_M_AgC_varvolmin = new GRBVar[NS, NP];

            GRBVar[,] Slk_M_AgC_varvolmax = new GRBVar[NS, NP];
            GRBVar[,] Exc_M_AgC_varvolmax = new GRBVar[NS, NP];

            GRBVar[,] Slk_M_AgC_maxarea = new GRBVar[NB, NP];
            GRBVar[,] Exc_M_AgC_maxarea = new GRBVar[NB, NP];

            GRBVar[,] Slk_M_AgC_minarea = new GRBVar[NB, NP];
            GRBVar[,] Exc_M_AgC_minarea = new GRBVar[NB, NP];

            GRBVar[] Slk_B_AgC_firstblock = new GRBVar[NP];
            GRBVar[] Exc_B_AgC_firstblock = new GRBVar[NP];

            GRBVar[,,] Slk_B_AgC_createblock = new GRBVar[NS, NP, NB];
            GRBVar[,,] Exc_B_AgC_createblock = new GRBVar[NS, NP, NB];

            GRBVar[,,,] Slk_B_AgC_nearblock = new GRBVar[NI, NI, NP, NB];
            GRBVar[,,,] Exc_B_AgC_nearblock = new GRBVar[NI, NI, NP, NB];

            GRBVar[,,,,] Slk_B_AgC_nearyear = new GRBVar[NI, NI, NP, NB, NB];
            GRBVar[,,,,] Exc_B_AgC_nearyear = new GRBVar[NI, NI, NP, NB, NB];

            GRBVar[,,,,] Slk_B_AgC_sequence = new GRBVar[NI, NI, NP, NB, NB];
            GRBVar[,,,,] Exc_B_AgC_sequence = new GRBVar[NI, NI, NP, NB, NB];

            GRBVar[,,,,] Slk_B_AgC_consecutive = new GRBVar[NI, NI, NP, NB, NB];
            GRBVar[,,,,] Exc_B_AgC_consecutive = new GRBVar[NI, NI, NP, NB, NB];

            GRBVar[,,,,] Slk_B_AgC_nearconsec = new GRBVar[NI, NI, NP, NB, NB];
            GRBVar[,,,,] Exc_B_AgC_nearconsec = new GRBVar[NI, NI, NP, NB, NB];

            //Variables type and Objective Function coefficients
            //Unit Harvesting
            WriteLog(MP.Uselog, logtxt, "Vars X");

            for (int i = 0; i < NI; i++)
            {
                Node N = S.Nodes[i];
                if (N.IsFMU)
                {
                    //if this node is a FMU, we need X Var as FMU
                    FMU? Unit = S.GetFMUByNode(N, S.FMUs);
                    WriteLog(MP.Uselog, logtxt, $"FMU {Unit}");
                    //Coef. for FMU X Var
                    double[]? LPV = Unit is null ? Array.Empty<double>() : Unit.LPV;
                    WriteLog(MP.Uselog && LPV.Any(), logtxt, $"LPVs: {LPV}");
                    for (int p = 0; p < NP; p++)
                    {
                        for (int b = 0; b < NB; b++)
                        {
                            AuxIndexes = new List<(string, int)> {
                            ("I", i), ("P", p), ("B", b) };
                            string Xname = ToolBox.ConstraintName(NameVar[0], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, Xname);
                            if (!MP.Uselog)
                            {
                                X_FMU[i, p, b] = Modelo.AddVar(0, 1, LPV[p], VarType, Xname.ToString());
                                CounterVar[0]++;
                            }
                        }
                    }

                } else
                {
                    //this is not a FMU, but a Node from graph.
                    WriteLog(MP.Uselog, logtxt, $"Node {N}.");
                    for (int p = 0; p < NP; p++)
                    {
                        for (int b = 0; b < NB; b++)
                        {
                            AuxIndexes = new List<(string, int)> {
                            ("I", i), ("P", p), ("B", b) };
                            string Xname = ToolBox.ConstraintName(NameVar[0], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, Xname);
                            if (!MP.Uselog)
                            {
                                X_FMU[i, p, b] = Modelo.AddVar(0, 1, 0, VarType, Xname.ToString());
                                CounterVar[0]++;
                            }
                        }
                    }
                }
            }
            //Block Variables
            WriteLog(MP.Uselog, logtxt, "Block Vars");

            for (int p = 0; p < NP; p++)
            {
                for (int b = 0; b < NB; b++)
                {
                    AuxIndexes = new List<(string, int)> { ("P", p), ("B", b) };
                    string Alphaname = ToolBox.ConstraintName(NameVar[5], AuxIndexes);
                    WriteLog(MP.Uselog, logtxt, Alphaname);
                    if (!MP.Uselog)
                    {
                        Alpha[p, b] = Modelo.AddVar(0, 1, 0, VarType, Alphaname.ToString());
                    }
                    CounterVar[5]++;
                }
            }
            //VRP Variables
            WriteLog(MP.Uselog, logtxt, "VRP Vars");

            for (int i = 0; i < NI; i++)
            {
                for (int j = 0; j < NI; j++)
                {
                    for (int p = 0; p < NP; p++)
                    {
                        AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p) };
                        string Wname = ToolBox.ConstraintName(NameVar[1], AuxIndexes);
                        string Zname = ToolBox.ConstraintName(NameVar[2], AuxIndexes);
                        string Yname = ToolBox.ConstraintName(NameVar[3], AuxIndexes);
                        string Uname = ToolBox.ConstraintName(NameVar[4], AuxIndexes);
                        if (!MP.Uselog)
                        {
                            W_ROpen[i, j, p] = Modelo.AddVar(0, 1, -I.CalcCost_RoadOpening(i, j, p), GRB.BINARY, Wname.ToString());
                            Z_RMaint[i, j, p] = Modelo.AddVar(0, 1, -I.CalcCost_RoadMaintenance(i, j, p), GRB.BINARY, Zname.ToString());
                            Y_RUse[i, j, p] = Modelo.AddVar(0, 1, 0, GRB.BINARY, Yname.ToString());
                            U_Trav[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, -I.CalcCost_TravelsIJ(i, j), GRB.INTEGER, Uname.ToString());
                        }
                        WriteLog(MP.Uselog, logtxt, $"{Wname}\n{Zname}\n{Yname}\n{Uname}\n");
                        CounterVar[1]++;//W
                        CounterVar[2]++;//Z
                        CounterVar[3]++;//Y
                        CounterVar[4]++;//U
                    }
                }
            }
            int ind_cst = 0;
            WriteLog(MP.Uselog, logtxt, "Slack & Excess Vars");
            //Slack & Excess Constraints 0
            // M_first_open
            //z_{ ijp} & \leq \sum_{ p'=1}^{p-1} w_{ijp'} & \forall i, j \in N, \forall p \in P  \label{ eq: M_first_open}  
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk & Exc {Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_M_FirstOpen[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_M_FirstOpen[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }

                        }
                    }
                }
            }
            ind_cst++;

            //Slack & Excess Constraints 1
            // M_Maintenance
            //z_{ijp} & \leq \sum_{p'\in P_p} w_{ijp'} & \forall i,j \in F, \forall p > 2 \in P_p
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk & Exc {Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_M_Maintenance[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_M_Maintenance[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }
                        }
                    }
                }
            }
            ind_cst++;
            //Slack & Excess Constraints 2
            // M_Maint_open
            //w_{ijp} + z_{ijp} & \leq 1  & \forall i,j \in F, \forall p \in P,  \label{eq:M_manut_abert}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_M_maint_open[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_M_maint_open[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }
                        }
                    }
                }
            }
            ind_cst++;
            //Slack & Excess Constraints 3
            // M_vehicle
            //y_{ijp} & \leq w_{ijp} + z_{ijp}  & \forall i,j \in F, \forall p \in P,  \label{eq:M_veiculo}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_M_vehicle[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_M_vehicle[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 4
            // M_use
            //y_{ijp} & \leq u_{ijp}  & \forall i,j \in F, \forall p \in P,  \label{eq:M_uso} 
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int p = 0; p < NP; p++)
                    {
                        for (int j = 0; j < NI; j++)
                        {
                            AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_M_use[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_M_use[i, j, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 5
            // M_acess
            //\sum_{b\in B} x_{ipb} & \leq \sum_{j\in F} y_{ijp}  & \forall i \in F, \forall p \in P,  \label{eq:M_acesso}}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int p = 0; p < NP; p++)
                    {
                        AuxIndexes = new List<(string, int)> { ("I", i), ("P", p) };
                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                        if (!MP.Uselog)
                        {
                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                            Slk_M_acess[i, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                            Exc_M_acess[i, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 6
            // M_flow
            //\sum_{j\in F} u_{jip} + \varepsilon_{ip}\left(\sum_{b\in B} x_{ipb}\right) = \sum_{j\in F} u_{ijp}
            // \forall i \in F, \\ \forall p \in P   \label{eq:M_fluxo}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int p = 0; p < NP; p++)
                    {
                        AuxIndexes = new List<(string, int)> { ("I", i), ("P", p) };
                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                        if (!MP.Uselog)
                        {
                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                            Slk_M_flow[i, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                            Exc_M_flow[i, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 7
            // M_AgC_qtharvest
            //\sum_{b\in B}\sum_{p\in P} x_{ipb} & \leq 1 & \forall i \in F \label{eq:M_AgC_qtcolh}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    AuxIndexes = new List<(string, int)> { ("I", i) };
                    string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                    WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                    if (!MP.Uselog)
                    {
                        double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                        double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                        Slk_M_AgC_qtharvest[i] = Modelo.AddVar(0, 1, costS, GRB.BINARY, "Slk" + Varname);
                        Exc_M_AgC_qtharvest[i] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                    }
                }
            }
            ind_cst++;
            //Slack & Excess Constraints 8
            // M_AgC_initialblock
            //x_{i11} & = 1 & \forall i\in F_1 \label{eq:M_AgC_inibloco}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    AuxIndexes = new List<(string, int)> { ("I", i) };
                    string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                    WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                    if (!MP.Uselog)
                    {
                        double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                        double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                        Slk_M_AgC_initialblock[i] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                        Exc_M_AgC_initialblock[i] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                    }
                }
            }
            ind_cst++;
            //Slack & Excess Constraints 9
            // M_AgC_ageblock
            //(n_i+p-1)x_{ipb} & \geq \eta_i x_{ipb} &
            //\forall i \in F, \forall p \in P, \\ \forall b \in B_p  \label{eq:M_AgC_idadebloco}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int p = 0; p < NP; p++)
                    {
                        for (int b = 0; b < NB; b++)
                        {
                            AuxIndexes = new List<(string, int)> { ("I", i), ("P", p), ("B", b) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_M_AgC_ageblock[i, p, b] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_M_AgC_ageblock[i, p, b] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }
                        }
                    }
                }
            }
            ind_cst++;
            //Slack & Excess Constraints 10
            // M_AgC_varvolmin
            //\sum_{b\in B}\sum_{i \in F} \upsilon_{isp}x_{ipb}  \geq (1-\sigma)d_{sp}
            //\forall s\in S, \forall p\in P \label{eq:M_AgC_varvolmin}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int p = 0; p < NP; p++)
                {
                    for (int s = 0; s < NS; s++)
                    {
                        AuxIndexes = new List<(string, int)> { ("S", s), ("P", p) };
                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                        if (!MP.Uselog)
                        {
                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                            Slk_M_AgC_varvolmin[s, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                            Exc_M_AgC_varvolmin[s, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                        }
                    }
                }
            }
            ind_cst++;
            //Slack & Excess Constraints 11
            // M_AgC_varvolmax
            //\sum_{b\in B}\sum_{i \in F} \upsilon_{isp}x_{ipb} \leq (1+\sigma)d_{sp}
            //\forall s\in S, \forall p\in P \label{eq:M_AgC_varvolmax}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int p = 0; p < NP; p++)
                {
                    for (int s = 0; s < NS; s++)
                    {
                        AuxIndexes = new List<(string, int)> { ("S", s), ("P", p) };
                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                        if (!MP.Uselog)
                        {
                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                            Slk_M_AgC_varvolmax[s, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                            Exc_M_AgC_varvolmax[s, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 12
            // M_AgC_maxarea
            //\sum_{i \in F} a_i x_{ipb} & \leq \overline{A_p}\alpha_{pb}
            //\forall p \in P, \forall b \in B_p \label{eq:M_AgC_maxarea}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int p = 0; p < NP; p++)
                {
                    for (int b = 0; b < NB; b++)
                    {
                        AuxIndexes = new List<(string, int)> { ("P", p), ("B", b) };
                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                        if (!MP.Uselog)
                        {
                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                            Slk_M_AgC_maxarea[b, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                            Exc_M_AgC_maxarea[b, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 13
            // M_AgC_minarea
            //\sum_{i\in F} a_i x_{ipb} & \geq \underline{A_p}\alpha_{pb}
            //\forall p \in P, \forall b \in B_p \label{eq:M_AgC_minarea}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int p = 0; p < NP; p++)
                {
                    for (int b = 0; b < NB; b++)
                    {
                        AuxIndexes = new List<(string, int)> { ("P", p), ("B", b) };
                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                        if (!MP.Uselog)
                        {
                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                            Slk_M_AgC_minarea[b, p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                            Exc_M_AgC_minarea[b, p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 14
            //Creating Blocks //First Block
            // B_AgC_firstblock
            //\alpha_{p1} & = 1 & \forall p \in P \label{eq:B_AgC_primbloco}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int p = 0; p < NP; p++)
                {
                    AuxIndexes = new List<(string, int)> { ("P", p) };
                    string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                    WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                    if (!MP.Uselog)
                    {
                        double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                        double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                        Slk_B_AgC_firstblock[p] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                        Exc_B_AgC_firstblock[p] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 15
            //Creating Blocks
            //other blocks
            // B_AgC_createblock
            //\somat{h}{1}{b-1} \somat{i}{1}{N} \upsilon_{isp}x_{iph} + \tau(1+\sigma)d_{sp} \alpha_{pb} \leq (1+\sigma)d_{sp}
            //& \forall s\in S, \forall p \in P, b = 2,\ldots, |B| %\label{eq:B_AgC_criabloco}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int s = 0; s < NS; s++)
                {
                    for (int p = 0; p < NP; p++)
                    {
                        for (int b = 1; b < NB; b++)
                        {
                            AuxIndexes = new List<(string, int)> { ("S", s), ("P", p), ("B", b) };
                            string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                            WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                            if (!MP.Uselog)
                            {
                                double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                Slk_B_AgC_createblock[s, p, b] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                Exc_B_AgC_createblock[s, p, b] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 16
            // B_AgC_nearblock
            //\delta_{ij}\PG{x_{ipb}+x_{jpb}-1} & \leq \Delta_{b}
            //\forall i \in F, \forall j > i \in F, \\ \forall p \in P, \forall b \in B_p \label{eq:B_AgC_proxbloco}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            for (int b = 0; b < NB; b++)
                            {
                                AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b) };
                                string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                                WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                                if (!MP.Uselog)
                                {
                                    double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                    double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                    Slk_B_AgC_nearblock[i, j, p, b] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                    Exc_B_AgC_nearblock[i, j, p, b] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 17
            // B_AgC_nearyear
            //\delta_{ij}\PG{x_{ipb}+x_{jph}-1} & \leq \Delta_{a}
            //\forall i \in F, \forall j > i \in F, \forall p \in P, \forall b,h \in B_p, h \neq b \label{eq:B_AgC_proxano}
            // NI, NI,  NP,  NB, NB 
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            for (int b = 0; b < NB; b++)
                            {
                                for (int h = 0; h < NB; h++)
                                {
                                    AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) };
                                    if (h != b)
                                    {
                                        string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                                        WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                                        if (!MP.Uselog)
                                        {
                                            double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                            double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                            Slk_B_AgC_nearyear[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                            Exc_B_AgC_nearyear[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 18
            // B_AgC_sequence 
            //x_{ipb}+x_{jph} &\leq 1
            //\forall i \in F, \forall j \in A_i,\\ \forall p \in P, \forall b \in B_p, \\ h = b+1,\ldots,|B| \label{eq:B_AgC_seq}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                foreach ((int, int) adjs in I.Std.Adjacentes)
                {
                    int i = adjs.Item1;
                    int j = adjs.Item2;
                    for (int p = 0; p < NP; p++)
                    {
                        for (int b = 0; b < NB; b++)
                        {
                            for (int h = b + 1; h < NB; h++)
                            {
                                AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) };
                                string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                                WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                                if (!MP.Uselog)
                                {
                                    double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                    double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                    Slk_B_AgC_sequence[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                    Exc_B_AgC_sequence[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 19
            // B_AgC_consecutive 
            //x_{ipb}+x_{j(p+1)h} &\leq 1
            //\forall i \in F, \forall j \in A_i,\\ \forall b \in B_p,  \forall h \in B_p, \\ p = 1,\ldots,HP-1 \label{eq:B_AgC_consec}
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                foreach ((int, int) adjs in I.Std.Adjacentes)
                {
                    int i = adjs.Item1;
                    int j = adjs.Item2;
                    for (int p = 0; p < NP; p++)
                    {
                        for (int b = 0; b < NB; b++)
                        {
                            for (int h = 0; h < NB; h++)
                            {
                                AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) };
                                string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                                WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                                if (!MP.Uselog)
                                {
                                    double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                    double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                    Slk_B_AgC_consecutive[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                    Exc_B_AgC_consecutive[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            //Slack & Excess Constraints 20// B_AgC_nearconsec
            //\delta_{ij}\PG{x_{ipb}+x_{j(p+1)h}} \leq 2\Delta_{c}
            //\forall i,j \in F, \forall b \in B_p, \forall h \in B_p,  p = 1, \ldots, HP-1 \label{eq:B_AgC_proxconsec}%
            if (MP.UseConstraints[ind_cst] && MP.SlackExcessVars[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)
                        {
                            for (int b = 0; b < NB; b++)
                            {
                                for (int h = 0; h < NB; h++)
                                {
                                    AuxIndexes = new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) };
                                    string Varname = ToolBox.ConstraintName(ToolBox.Constants.Constraints[ind_cst], AuxIndexes);
                                    WriteLog(MP.Uselog, logtxt, $"Slk e Exc{Varname}");
                                    if (!MP.Uselog)
                                    {
                                        double costS = MP.DataInst.Std.MatrixPar.BigMS[ind_cst];
                                        double costE = MP.DataInst.Std.MatrixPar.BigME[ind_cst];
                                        Slk_B_AgC_nearconsec[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costS, GRB.CONTINUOUS, "Slk" + Varname);
                                        Exc_B_AgC_nearconsec[i, j, p, b, h] = Modelo.AddVar(0, GRB.INFINITY, costE, GRB.CONTINUOUS, "Exc" + Varname);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Constraints
            //SelfFlow

            GRBLinExpr expr = new();
            List<string> ConstraintNames = new();
            WriteLog(MP.Uselog, logtxt, "Constraints");
            for (int p = 0; p < NP; p++)
            {
                for (int i = 0; i < NI; i++)
                {
                    List<(string, GRBVar[,,])> Vs = new List<(string, GRBVar[,,])> { 
                        ("W", W_ROpen), ("Z", Z_RMaint), ("Y", Y_RUse), ("U", U_Trav) };
                    if (!MP.Uselog)
                    { 
                        foreach((string, GRBVar[,,]) v in Vs)
                        {
                            ConstName = ToolBox.ConstraintName($"R_M_selfflow_{v.Item1}",
                                        new List<(string, int)> { ("I", i), ("P", p) });
                            expr = R_M_selfflow(i, p, v.Item2);
                            if (!(ConstraintNames.Contains(ConstName)))
                            {//Avoid repeated constraints
                                Modelo.AddConstr(expr == 0, ConstName);
                                ConstraintNames.Add(ConstName);
                            }
                        }
                        //Modelo.AddConstr(W_ROpen[i, i, p] == 0, $"R_M_selfflow_W_I{i:00}_P{p:00}");
                        //Modelo.AddConstr(Z_RMaint[i, i, p] == 0, $"R_M_selfflow_Z_I{i:00}_P{p:00}");
                        //Modelo.AddConstr(Y_RUse[i, i, p] == 0, $"R_M_selfflow_Y_I{i:00}_P{p:00}");
                        //Modelo.AddConstr(U_Trav[i, i, p] == 0, $"R_M_selfflow_U_I{i:00}_P{p:00}");
                        //W_ROpen[i, i, p].UB = 0;
                        //Z_RMaint[i, i, p].UB = 0;
                        //Y_RUse[i, i, p].UB = 0;
                        //U_Trav[i, i, p].UB = 0;
                    }
                    else
                    {
                        foreach((string, GRBVar[,,]) v in Vs)
                        {
                            ConstName = ToolBox.ConstraintName($"R_M_selfflow_{v.Item1}",
                                        new List<(string, int)> { ("I", i), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                        }
                    }
                }
            }
            ConstraintNames.Clear();
            expr.Clear();

            //Maintenance at p=0
            for (int i = 0; i < NI; i++)
            {
                for (int j = 0; j < NI; j++)
                {
                    if (!MP.Uselog)
                    {
                        ConstName = ToolBox.ConstraintName("R_M_MaintenanceAtZero",
                                    new List<(string, int)> { ("I", i), ("J", j) });
                        expr.AddTerm(1, Z_RMaint[i, j, 0]);


                        if (!(ConstraintNames.Contains(ConstName)))
                        {//Avoid repeated constraints
                            Modelo.AddConstr(expr == 0, ConstName);
                            ConstraintNames.Add(ConstName);
                            expr.Clear();
                        }
                    }
                    else
                    {
                        ConstName = ToolBox.ConstraintName("R_M_MaintenanceAtZero",
                                       new List<(string, int)> { ("I", i), ("J", j) });
                        WriteLog(MP.Uselog, logtxt, ConstName);
                    }
                }
            }
            ConstraintNames.Clear();
            ind_cst = 0;
            // \label{ eq: M_first_open}  
            //z_{ ijp} & \leq \sum_{ p'=1}^{p-1} w_{ijp'} & \forall i, j \in N, \forall p \in P  

            if (MP.UseConstraints[ind_cst]) 
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 1; p < NP; p++)
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                        new List<(string, int)> { ("I", i), ("J", j), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {

                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_M_first_open(i, j, p, W_ROpen, Z_RMaint, 0)
                                    : R_M_first_open(i, j, p, W_ROpen, Z_RMaint,
                                                     Slk_M_FirstOpen,
                                                     Exc_M_FirstOpen,
                                                     CoefSlackExcess[ind_cst], 0);

                                if (!(ConstraintNames.Contains(ConstName)))
                                {//Avoid repeated constraints
                                    Modelo.AddConstr(expr <= 0, ConstName);
                                    ConstraintNames.Add(ConstName);
                                    CounterConstraint[ind_cst]++;
                                }
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // M_Maintenance
            //z_{ijp} & \leq \sum_{p'\in P_p} w_{ijp'} & \forall i,j \in F, \forall p > 2 \in P_p
            if (MP.UseConstraints[ind_cst]) // && P.IndexPeriod > 2)
            {
                //for (int e = 0; e < NE; e++)//foreach edge
                //{
                //Edge E = S.Edges[e];
                //int i = E.Node1.Index;
                //int j = E.Node2.Index;
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = MOP; p < NP; p++)// MOP - Maintenance Opening Period
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                        new List<(string, int)> { ("I", i), ("J", j), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                Range Pp = (p - MOP)..p;// MOP periods before actual p.

                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_M_Maintenance(i, j, Pp, W_ROpen, Z_RMaint, 0)
                                    : R_M_Maintenance(i, j, Pp, W_ROpen, Z_RMaint,
                                                      Slk_M_Maintenance,
                                                      Exc_M_Maintenance,
                                                      CoefSlackExcess[0], 0);

                                if (!(ConstraintNames.Contains(ConstName)))
                                {//Avoid repeated constraints
                                    Modelo.AddConstr(expr <= 0, ConstName);
                                    ConstraintNames.Add(ConstName);
                                    CounterConstraint[ind_cst]++;
                                }
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_Maint_open
            // w_{ ijp} +z_{ ijp} & \leq 1 & \forall i, j \in F, \forall p \in P,  \label{ eq: M_manut_abert}
            // Leitura(\ref{ eq: M_manut_abert}): Uma estrada não pode receber manutenção no mesmo período que tiver sido aberta
            if (MP.UseConstraints[ind_cst])
            {
                //for (int e = 0; e < NE; e++)//foreach edge
                //{
                //    Edge E = S.Edges[e];
                //    int i = E.Node1.Index;
                //    int j = E.Node2.Index;
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 0; p < NP; p++)//trocado p de 1 para 0. 
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                new List<(string, int)> { ("I", i), ("J", j), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                expr = CoefSlackExcess[1] == 0
                                    ? R_M_maint_open(i, j, p, W_ROpen, Z_RMaint, 1)
                                    : R_M_maint_open(i, j, p, W_ROpen, Z_RMaint,
                                                     Slk_M_maint_open,
                                                     Exc_M_maint_open,
                                                     CoefSlackExcess[ind_cst], 1);

                                if (!(ConstraintNames.Contains(ConstName)))
                                {
                                    Modelo.AddConstr(expr <= 0, ConstName);
                                    ConstraintNames.Add(ConstName);
                                    CounterConstraint[ind_cst]++;
                                }
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_vehicle
            //y_{ijp} & \leq w_{ijp} + z_{ijp}  & \forall i,j \in F, \forall p \in P,  \label{eq:M_veiculo}
            if (MP.UseConstraints[ind_cst])
            {
                //for (int e = 0; e < NE; e++)//foreach edge
                //{
                //    Edge E = S.Edges[e];
                //    int i = E.Node1.Index;
                //    int j = E.Node2.Index;
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 1; p < NP; p++)//p=0 nao tem restrição de uso
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                new List<(string, int)> { ("I", i), ("J", j), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_M_vehicle(i, j, p, W_ROpen, Z_RMaint, Y_RUse, 0)
                                    : R_M_vehicle(i, j, p, W_ROpen, Z_RMaint, Y_RUse,
                                                  Slk_M_vehicle,
                                                  Exc_M_vehicle,
                                                  CoefSlackExcess[ind_cst], 0);

                                if (!(ConstraintNames.Contains(ConstName)))
                                {
                                    Modelo.AddConstr(expr <= 0, ConstName);
                                    ConstraintNames.Add(ConstName);
                                    CounterConstraint[ind_cst]++;
                                }
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_use
            //y_{ijp} & \leq u_{ijp}  & \forall i,j \in F, \forall p \in P,  \label{eq:M_uso}
            if (MP.UseConstraints[ind_cst])
            {
                //for (int e = 0; e < NE; e++)//foreach edge
                //{
                //    Edge E = S.Edges[e];
                //    int i = E.Node1.Index;
                //    int j = E.Node2.Index;
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        for (int p = 1; p < NP; p++)//p=0 não tem restrição
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                        new List<(string, int)> { ("I", i), ("J", j), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_M_use(i, j, p, Y_RUse, U_Trav, 0)
                                    : R_M_use(i, j, p, Y_RUse, U_Trav,
                                              Slk_M_use,
                                              Exc_M_use,
                                              CoefSlackExcess[ind_cst], 0);

                                if (!(ConstraintNames.Contains(ConstName)))
                                {
                                    Modelo.AddConstr(expr <= 0, ConstName);
                                    ConstraintNames.Add(ConstName);
                                    CounterConstraint[ind_cst]++;
                                }
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_acess
            //\sum_{b\in B} x_{ipb} & \leq \sum_{j\in N} y_{ijp}  & \forall i \in N, \forall p \in P,  \label{eq:M_acesso}  
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int p = 1; p < NP; p++)//p=0 não tem restrição
                    {
                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                            new List<(string, int)> { ("I", i), ("P", p) });
                        WriteLog(MP.Uselog, logtxt, ConstName);
                        if (!MP.Uselog)
                        {
                            expr = CoefSlackExcess[ind_cst] == 0
                                ? R_M_acess(i, p, X_FMU, Y_RUse)
                                : R_M_acess(i, p, X_FMU, Y_RUse,
                                            Slk_M_acess, Exc_M_acess,
                                            CoefSlackExcess[ind_cst], 0);
                            Modelo.AddConstr(expr <= 0, ConstName);
                            CounterConstraint[ind_cst]++; 
                            expr.Clear();
                            //fileall.WriteLine(ConstName);
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_flow
            //\sum_{j\in F} u_{jip} + \varepsilon_{ip}\left(\sum_{b\in B} x_{ipb}\right) & = \sum_{j\in N} u_{ijp}
            // \forall i \in N, \\ \forall p \in P  \label{eq:M_fluxo}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    if (!S.Nodes[i].IsExit) 
                    {
                        for (int p = 1; p < NP; p++)//p=0 não tem restrição
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                        new List<(string, int)> { ("I", i), ("P", p) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_M_flow(i, p, X_FMU, U_Trav, I)
                                    : R_M_flow(i, p, X_FMU, U_Trav, I,
                                              Slk_M_flow, Exc_M_flow,
                                              CoefSlackExcess[ind_cst]);
                                Modelo.AddConstr(expr <= 0, ConstName);
                                CounterConstraint[ind_cst]++;
                                expr.Clear();
                            }
                            //fileall.WriteLine(ConstName);
                        } 
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_AgC_qtharvest
            //\sum_{b\in B}\sum_{p\in P} x_{ipb} & \leq 1
            //\forall i \in F \label{eq:M_AgC_qtcolh}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    Node N = S.Nodes[i];
                    if (N.IsFMU)
                    {
                        FMU? f = S.GetFMUByNode(N, S.FMUs);
                        if (f is not null)
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst], ("I", i));
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_M_AgC_qtharvest(i, X_FMU, 1)
                                    : R_M_AgC_qtharvest(i, X_FMU, Slk_M_AgC_qtharvest,
                                                        Exc_M_AgC_qtharvest,
                                                        CoefSlackExcess[ind_cst], 1);
                                Modelo.AddConstr(expr <= 0, ConstName);
                                CounterConstraint[ind_cst]++;
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_AgC_initialblock
            //x_{i11} & = 1 & \forall i\in F_1 \label{eq:M_AgC_inibloco}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    Node N = S.Nodes[i];
                    if (N.IsFMU)
                    {
                        FMU? f = I.Std.GetFMUByNode(S.Nodes[i], S.FMUs);
                        if (f is not null)
                        {
                            if (f.IsFirstBlock)
                            {
                                ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst], ("I", i));
                                WriteLog(MP.Uselog, logtxt, ConstName);
                                if (!MP.Uselog)
                                {
                                    expr = CoefSlackExcess[ind_cst] == 0
                                        ? R_M_AgC_initialblock(i, X_FMU)
                                        : R_M_AgC_initialblock(i, X_FMU,
                                                  Slk_M_AgC_initialblock,
                                                  Exc_M_AgC_initialblock,
                                                  CoefSlackExcess[ind_cst]);
                                    Modelo.AddConstr(expr == 0, ConstName);
                                    CounterConstraint[ind_cst]++;
                                    expr.Clear();
                                    //fileall.WriteLine(ConstName);
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_AgC_ageblock
            //(n_i+p-1)x_{ipb} & \geq \eta_i x_{ipb}
            //& \forall i \in F, \forall p \in P, \forall b \in B_p  \label{eq:M_AgC_idadebloco}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    Node N = S.Nodes[i];
                    if (N.IsFMU)
                    {
                        FMU? f = I.Std.GetFMUByNode(S.Nodes[i], S.FMUs);
                        if (f is not null)
                        {
                            for (int p = 1; p < NP; p++)//Período 0 não tem restrição
                            {
                                if (f.InitialAge + p - 1 - I.MinimalHarvestAge < 0)
                                {// Se a idade de colheita é igual à idade mínima, o 
                                 // coeficiente da restrição é nulo e fica apenas ">=0".
                                 // Se a idade for maior, a variável pode ser 1 ou 0 que é atendida.
                                    for (int b = 0; b < NB; b++)
                                    {
                                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                                    new List<(string, int)> { ("I", i), ("P", p), ("B", b) });
                                        WriteLog(MP.Uselog, logtxt, ConstName);
                                        if (!MP.Uselog)
                                        {
                                            expr = CoefSlackExcess[ind_cst] == 0
                                                ? R_M_AgC_ageblock(i, p, b, f.InitialAge, I.MinimalHarvestAge, X_FMU)
                                                : R_M_AgC_ageblock(i, p, b, f.InitialAge, I.MinimalHarvestAge, X_FMU,
                                                                   Slk_M_AgC_ageblock, Exc_M_AgC_ageblock,
                                                                   CoefSlackExcess[ind_cst]);
                                            Modelo.AddConstr(expr >= 0, ConstName);
                                            CounterConstraint[ind_cst]++;
                                            expr.Clear();
                                            //fileall.WriteLine(ConstName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // M_AgC_varvolmin
            //  \sum_{b\in B}\sum_{i \in F} \upsilon_{isp}x_{ipb} & \geq (1-\sigma)d_{sp}
            //  & \forall s\in S, \forall p\in P \label{eq:M_AgC_varvolmin}
            if (MP.UseConstraints[ind_cst])
            {
                for (int s = 0; s < NS; s++)
                {
                    for (int p = 1; p < NP; p++)//Período 0 não tem restrição
                    {
                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                    new List<(string, int)> { ("S", s), ("P", p) });
                        WriteLog(MP.Uselog, logtxt, ConstName);
                        if (!MP.Uselog)
                        {
                            expr = CoefSlackExcess[ind_cst] == 0
                                ? R_M_AgC_varvolmin(s, p, I, X_FMU)
                                : R_M_AgC_varvolmin(s, p, I, X_FMU, 
                                                    Slk_M_AgC_varvolmin,
                                                    Exc_M_AgC_varvolmin, 
                                                    CoefSlackExcess[ind_cst]);
                            Modelo.AddConstr(expr >= 0, ConstName);
                            CounterConstraint[ind_cst]++;
                            expr.Clear();
                            //fileall.WriteLine(ConstName);
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // M_AgC_varvolmax
            //  \sum_{b\in B}\sum_{i \in F} \upsilon_{isp}x_{ipb} & \leq (1+\sigma)d_{sp}
            //  & \forall s\in S, \forall p\in P \label{eq:M_AgC_varvolmax}}
            if (MP.UseConstraints[ind_cst])
            {
                for (int s = 0; s < NS; s++)
                {
                    for (int p = 1; p < NP; p++)//trocado p, Período 0 não tem restrição
                    {
                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                    new List<(string, int)> { ("S", s), ("P", p) });
                        WriteLog(MP.Uselog, logtxt, ConstName);
                        if (!MP.Uselog)
                        {
                            expr = CoefSlackExcess[ind_cst] == 0
                                ? R_M_AgC_varvolmax(s, p, X_FMU, I)
                                : R_M_AgC_varvolmax(s, p, X_FMU, I, 
                                                    Slk_M_AgC_varvolmax,
                                                    Exc_M_AgC_varvolmax, 
                                                    CoefSlackExcess[ind_cst]);
                            Modelo.AddConstr(expr <= 0, ConstName);
                            CounterConstraint[ind_cst]++;
                            expr.Clear();
                            //fileall.WriteLine(ConstName);
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // M_AgC_maxarea
            // \sum_{i \in F} a_i x_{ipb} & \leq \overline{A_p}\alpha_{pb}
            // & \forall p \in P, \forall b \in B_p \label{eq:M_AgC_maxarea}
            if (MP.UseConstraints[ind_cst])
            {
                for (int p = 1; p < NP; p++)//p=0 não tem restrição
                {
                    for (int b = 0; b < NB; b++)
                    {
                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                new List<(string, int)> { ("P", p), ("B", b) });
                        WriteLog(MP.Uselog, logtxt, ConstName);
                        if (!MP.Uselog)
                        {
                            expr = CoefSlackExcess[ind_cst] == 0
                                ? R_M_AgC_maxarea(p, b, X_FMU, Alpha, I)
                                : R_M_AgC_maxarea(p, b, X_FMU, Alpha, I,
                                                  Slk_M_AgC_maxarea, 
                                                  Exc_M_AgC_maxarea,
                                                  CoefSlackExcess[ind_cst]);
                            Modelo.AddConstr(expr <= 0, ConstName);
                            expr.Clear();
                            //fileall.WriteLine(ConstName);
                        }
                        CounterConstraint[ind_cst]++;
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // M_AgC_minarea
            //      \sum_{i\in F} a_i x_{ipb} & \geq \underline{A_p}\alpha_{pb}
            //      & \forall p \in P, \forall b \in B_p \label{eq:M_AgC_minarea}}
            if (MP.UseConstraints[ind_cst])
            {
                for (int p = 1; p < NP; p++)//p= 0 não tem restrição
                {
                    for (int b = 0; b < NB; b++)
                    {
                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                    new List<(string, int)> { ("P", p), ("B", b) });
                        WriteLog(MP.Uselog, logtxt, ConstName);
                        if (!MP.Uselog)
                        {
                            expr = CoefSlackExcess[ind_cst] == 0
                                ? R_M_AgC_minarea(p, b, X_FMU, Alpha, I)
                                : R_M_AgC_minarea(p, b, X_FMU, Alpha, I,
                                                  Slk_M_AgC_minarea, 
                                                  Exc_M_AgC_minarea, 
                                                  CoefSlackExcess[ind_cst]);
                            Modelo.AddConstr(expr >= 0, ConstName);
                            expr.Clear();
                            //fileall.WriteLine(ConstName);
                        }
                        CounterConstraint[ind_cst]++;
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // B_AgC_firstblock
            if (MP.UseConstraints[ind_cst])
            {
                for (int p = 1; p < NP; p++)//p=0 não tem restrição
                {
                    ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst], ("P", p));
                    WriteLog(MP.Uselog, logtxt, ConstName);
                    if (!MP.Uselog)
                    {
                        expr = CoefSlackExcess[ind_cst] == 0
                            ? R_B_AgC_firstblock(p, Alpha)
                            : R_B_AgC_firstblock(p, Alpha, 
                                                 Slk_B_AgC_firstblock,
                                                 Exc_B_AgC_firstblock, 
                                                 CoefSlackExcess[ind_cst]);

                        Modelo.AddConstr(expr == 0, ConstName);
                        expr.Clear();
                        //fileall.WriteLine(ConstName);
                    }
                    CounterConstraint[ind_cst]++;
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // B_AgC_createblock
            //      \somat{h}{1}{b-1} \somat{i \in F} \upsilon_{isp}x_{iph} + \tau(1+\sigma)d_{sp} \alpha_{pb} \leq (1+\sigma)d_{sp}
            //      & \forall s\in S, \forall p \in P, b = 2,\ldots, |B| %\label{eq:B_AgC_criabloco}
            if (MP.UseConstraints[ind_cst])
            {
                for (int s = 0; s < NS; s++)
                {
                    for (int p = 1; p < NP; p++)//p=0 não tem restrição
                    {
                        for (int b = 1; b < NB; b++)//b=0 é tratado no "first_block"
                        {
                            ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                    new List<(string, int)> { ("S", s), ("P", p), ("B", b) });
                            WriteLog(MP.Uselog, logtxt, ConstName);
                            if (!MP.Uselog)
                            {
                                expr = CoefSlackExcess[ind_cst] == 0
                                    ? R_B_AgC_createblock(s, p, b, X_FMU, Alpha, I)
                                    : R_B_AgC_createblock(s, p, b, X_FMU, Alpha, I,
                                                          Slk_B_AgC_createblock, 
                                                          Exc_B_AgC_createblock, 
                                                          CoefSlackExcess[ind_cst]);
                                Modelo.AddConstr(expr <= 0, ConstName);
                                expr.Clear();
                                //fileall.WriteLine(ConstName);
                            }
                            CounterConstraint[ind_cst]++;
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();
            // B_AgC_nearblock
            //      \delta_{ij}\PG{x_{ipb}+x_{jpb}-1} & \leq \Delta_{b}
            //      & \forall i \in F, \forall j > i \in F, \forall p \in P, \forall b \in B_p \label{eq:B_AgC_proxbloco}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = i + 1; j < NI; j++)
                    {
                        Node Ni = S.Nodes[i];
                        Node Nj = S.Nodes[j];
                        if (Ni.IsFMU && Nj.IsFMU)
                        {
                            FMU? fi = S.GetFMUByNode(Ni, S.FMUs);
                            FMU? fj = S.GetFMUByNode(Nj, S.FMUs);
                            if (fi is not null && fj is not null)
                            {
                                for (int p = 1; p < NP; p++)//p=0 não tem restrição
                                {
                                    for (int b = 0; b < NB; b++)
                                    {
                                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                                        new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b) });
                                        WriteLog(MP.Uselog, logtxt, ConstName);
                                        if (!MP.Uselog)
                                        {
                                            expr = CoefSlackExcess[ind_cst] == 0
                                                ? R_B_AgC_nearblock(i, j, p, b, X_FMU, I)
                                                : R_B_AgC_nearblock(i, j, p, b, X_FMU, I,
                                                                    Slk_B_AgC_nearblock,
                                                                    Exc_B_AgC_nearblock,
                                                                    CoefSlackExcess[ind_cst]);
                                            Modelo.AddConstr(expr <= 0, ConstName);
                                            expr.Clear();
                                            //fileall.WriteLine(ConstName);
                                        }
                                        CounterConstraint[ind_cst]++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // B_AgC_nearyear
            //  \delta_{ij}\PG{x_{ipb}+x_{jph}-1} & \leq \Delta_{a}
            //  & \forall i \in F, \forall j > i \in F, \\ \forall p \in P, \forall b,h \in B_p,
            //  \\ h \neq b \label{eq:B_AgC_proxano}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = i + 1; j < NI; j++)
                    {
                        Node Ni = S.Nodes[i];
                        Node Nj = S.Nodes[j];
                        if (Ni.IsFMU && Nj.IsFMU)
                        {
                            FMU? fi = S.GetFMUByNode(Ni, S.FMUs);
                            FMU? fj = S.GetFMUByNode(Nj, S.FMUs);
                            if (fi is not null && fj is not null)
                            {
                                for (int p = 1; p < NP; p++)//p=0 não tem restrição
                                {
                                    for (int b = 0; b < NB; b++)
                                    {
                                        for (int h = 0; h < NB; h++)
                                        {
                                            if (h != b)
                                            {
                                                ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                                    new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) });

                                                WriteLog(MP.Uselog, logtxt, ConstName);
                                                if (!MP.Uselog)
                                                {
                                                    expr = CoefSlackExcess[ind_cst] == 0
                                                        ? R_B_AgC_nearyear(i, j, p, b, h, X_FMU, I)
                                                        : R_B_AgC_nearyear(i, j, p, b, h, X_FMU, I,
                                                                           Slk_B_AgC_nearyear, 
                                                                           Exc_B_AgC_nearyear, 
                                                                           CoefSlackExcess[ind_cst]);
                                                    Modelo.AddConstr(expr <= 0, ConstName);
                                                    expr.Clear();
                                                    //fileall.WriteLine(ConstName);
                                                }
                                                CounterConstraint[ind_cst]++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // B_AgC_sequence 
            //  x_{ipb}+x_{jph} &\leq 1
            //  \forall i \in F, \forall j \in A_i,\\ \forall p \in P,
            //  \forall b \in B_p, \\ h = b+1,\ldots,|B| \label{eq:B_AgC_seq}
            if (MP.UseConstraints[ind_cst])
            {
                foreach ((int, int) adjs in S.Adjacentes)
                {
                    int i = adjs.Item1;
                    int j = adjs.Item2;
                    Node Ni = S.Nodes[i];
                    Node Nj = S.Nodes[j];
                    if (Ni.IsFMU && Nj.IsFMU)
                    {
                        FMU? fi = S.GetFMUByNode(Ni, S.FMUs);
                        FMU? fj = S.GetFMUByNode(Nj, S.FMUs);
                        if (fi is not null && fj is not null)
                        {
                            for (int p = 1; p < NP; p++)//p=0 não tem restrição                            {
                            { 
                                for (int b = 0; b < NB - 1; b++)
                                {
                                    for (int h = b + 1; h < NB; h++)
                                    {
                                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                                new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) });
                                        WriteLog(MP.Uselog, logtxt, ConstName);
                                        if (!MP.Uselog)
                                        {
                                            expr = CoefSlackExcess[ind_cst] == 0
                                                ? R_B_AgC_sequence(i, j, p, b, h, X_FMU)
                                                : R_B_AgC_sequence(i, j, p, b, h, X_FMU,
                                                                   Slk_B_AgC_sequence,
                                                                   Exc_B_AgC_sequence,
                                                                   CoefSlackExcess[ind_cst]);
                                            Modelo.AddConstr(expr <= 0, ConstName);
                                            expr.Clear();
                                            //fileall.WriteLine(ConstName);
                                        }
                                        CounterConstraint[ind_cst]++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // B_AgC_consecutive 
            //  x_{ipb}+x_{j(p+1)h} &\leq 1
            //  & \forall i \in F, \forall j \in A_i, \forall b \in B_p,
            //  \forall h \in B_p, \\ p = 1,\ldots,HP-1 \label{eq:B_AgC_consec}
            if (MP.UseConstraints[ind_cst])
            {
                foreach ((int, int) adjs in S.Adjacentes)
                {
                    int i = adjs.Item1;
                    int j = adjs.Item2;
                    Node Ni = S.Nodes[i];
                    Node Nj = S.Nodes[j];
                    if (Ni.IsFMU && Nj.IsFMU)
                    {
                        FMU? fi = S.GetFMUByNode(Ni, S.FMUs);
                        FMU? fj = S.GetFMUByNode(Nj, S.FMUs);
                        if (fi is not null && fj is not null)
                        {
                            for (int p = 1; p < NP; p++)//p=0 não tem restrição
                            {
                                for (int b = 0; b < NB; b++)
                                {
                                    for (int h = 0; h < NB; h++)
                                    {
                                        ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                                    new List<(string, int)> { ("I", i), ("J", j),
                                                ("P", p), ("B", b), ("H", h) });
                                        WriteLog(MP.Uselog, logtxt, ConstName);

                                        if (!MP.Uselog)
                                        {
                                            expr = CoefSlackExcess[ind_cst] == 0
                                                ? R_B_AgC_consecutive(i, j, p, b, h, X_FMU)
                                                : R_B_AgC_consecutive(i, j, p, b, h, X_FMU,
                                                Slk_B_AgC_consecutive, Exc_B_AgC_consecutive,
                                                CoefSlackExcess[ind_cst]);
                                            Modelo.AddConstr(expr <= 0, ConstName);
                                            expr.Clear();
                                            //fileall.WriteLine(ConstName);
                                        }
                                        CounterConstraint[ind_cst]++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ind_cst++;
            ConstraintNames.Clear();

            // B_AgC_nearconsec
            //  \delta_{ij}\PG{x_{ipb}+x_{j(p+1)h}} & \leq 2\Delta_{c}
            //  & \forall i,j \in F, \forall b \in B_p, \forall h \in B_p,
            //  p = 1, \ldots, HP-1 \label{eq:B_AgC_proxconsec}
            if (MP.UseConstraints[ind_cst])
            {
                for (int i = 0; i < NI; i++)
                {
                    for (int j = 0; j < NI; j++)
                    {
                        if (i != j)
                        {
                            Node Ni = S.Nodes[i];
                            Node Nj = S.Nodes[j];
                            if (Ni.IsFMU && Nj.IsFMU)
                            {
                                FMU? fi = S.GetFMUByNode(Ni, S.FMUs);
                                FMU? fj = S.GetFMUByNode(Nj, S.FMUs);
                                if (fi is not null && fj is not null)
                                {
                                    for (int p = 1; p < NP - 1; p++)//p=0 não tem restrição
                                    {
                                        for (int b = 0; b < NB; b++)
                                        {
                                            for (int h = 0; h < NB; h++)
                                            {
                                                ConstName = ToolBox.ConstraintName("R" + ToolBox.Constants.Constraints[ind_cst],
                                                        new List<(string, int)> { ("I", i), ("J", j),
                                                    ("P", p), ("B", b), ("H", h) });
                                                WriteLog(MP.Uselog, logtxt, ConstName);

                                                if (!MP.Uselog)
                                                {
                                                    expr = CoefSlackExcess[ind_cst] == 0
                                                        ? R_B_AgC_nearconsec(i, j, p, b, h, X_FMU, I)
                                                        : R_B_AgC_nearconsec(i, j, p, b, h, X_FMU, I,
                                                        Slk_B_AgC_nearconsec, Exc_B_AgC_nearconsec, CoefSlackExcess[ind_cst]);
                                                    Modelo.AddConstr(expr <= 0, ConstName);
                                                    expr.Clear();
                                                    //fileall.WriteLine(ConstName);
                                                }
                                                CounterConstraint[ind_cst]++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (MP.UseSoltest && !MP.Uselog)
            {
                foreach((int, int, int) solt in MP.DataInst.Std.MatrixPar.SolTest)
                {
                    X_FMU[solt.Item1, solt.Item2, solt.Item3].LB = 1; 
                }
                //if (!(MST == null))
                //{
                //    Modelo.Update();
                //Modelo.Read(I.MP.mst_file);                    
                //}
                //else 
                //if (S.NameInstance.IndexOf("Kitt21") > -1)
                //{
                //    X_FMU[0, 2, 1].LB = 1;
                //    X_FMU[1, 4, 1].LB = 1;
                //    X_FMU[2, 1, 1].LB = 1;
                //    X_FMU[3, 3, 2].LB = 1;
                //    X_FMU[4, 1, 1].LB = 1;
                //    X_FMU[5, 5, 2].LB = 1;
                //    X_FMU[6, 1, 2].LB = 1;
                //    X_FMU[7, 3, 1].LB = 1;
                //    X_FMU[8, 5, 1].LB = 1;
                //    X_FMU[9, 2, 2].LB = 1;
                //    X_FMU[10, 5, 1].LB = 1;
                //    X_FMU[11, 2, 2].LB = 1;
                //    X_FMU[12, 4, 2].LB = 1;
                //    X_FMU[13, 3, 3].LB = 1;
                //    X_FMU[14, 2, 1].LB = 1;
                //    X_FMU[15, 4, 1].LB = 1;
                //    X_FMU[16, 1, 1].LB = 1;
                //    X_FMU[17, 4, 2].LB = 1;
                //    X_FMU[18, 3, 3].LB = 1;
                //    X_FMU[19, 1, 2].LB = 1;
                //    X_FMU[20, 2, 3].LB = 1;
                //    Alpha[1, 1].LB = 1;
                //    Alpha[1, 2].LB = 1;
                //    Alpha[2, 1].LB = 1;
                //    Alpha[2, 2].LB = 1;
                //    Alpha[2, 3].LB = 1;
                //    Alpha[3, 1].LB = 1;
                //    Alpha[3, 2].LB = 1;
                //    Alpha[3, 3].LB = 1;
                //    Alpha[4, 1].LB = 1;
                //    Alpha[4, 2].LB = 1;
                //    Alpha[5, 1].LB = 1;
                //    Alpha[5, 2].LB = 1;
                //}
                //else if (S.NameInstance.IndexOf("Kitt10") > -1)
                //{
                    // Solução Teste X[i][p][b]
                //    X_FMU[2, 1, 0].LB = 1;
                //    X_FMU[3, 1, 0].LB = 1;
                //    X_FMU[9, 1, 1].LB = 1;
                //    X_FMU[1, 2, 0].LB = 1;
                //    X_FMU[4, 2, 0].LB = 1;
                //    X_FMU[8, 2, 1].LB = 1;
                //    X_FMU[0, 3, 0].LB = 1;
                //    X_FMU[5, 3, 1].LB = 1;
                //    X_FMU[6, 4, 0].LB = 1;
                //    X_FMU[7, 4, 1].LB = 1;
                //    Alpha[1, 0].LB = 1;
                //    Alpha[1, 1].LB = 1;
                //    Alpha[2, 0].LB = 1;
                //    Alpha[2, 1].LB = 1;
                //    Alpha[3, 0].LB = 1;
                //    Alpha[3, 1].LB = 1;
                //    Alpha[4, 0].LB = 1;
                //    Alpha[4, 1].LB = 1;
                //    Y_RUse[2, 22, 1].LB = 1; //# F03 -> T04
                //    Y_RUse[13, 22, 1].LB = 1; //# T04 -> F
                //    Y_RUse[13, 27, 1].LB = 1; //# F -> S
                //    Y_RUse[3, 23, 1].LB = 1; //# F04 -> T05
                //    Y_RUse[13, 23, 1].LB = 1; //# T05 -> F
                //    Y_RUse[9, 23, 1].LB = 1; //# F10 -> T05
                //    Y_RUse[1, 21, 2].LB = 1; //# F02 -> T03
                //    Y_RUse[11, 21, 2].LB = 1; //# T03 -> D
                //    Y_RUse[11, 22, 2].LB = 1; //# D -> T04
                //    Y_RUse[13, 22, 2].LB = 1; //# T04 -> F
                //    Y_RUse[13, 27, 2].LB = 1; //# F -> S
                //    Y_RUse[3, 4, 2].LB = 1; //# F05 -> F04
                //    Y_RUse[3, 23, 2].LB = 1; // # F04 -> T05
                //    Y_RUse[13, 23, 2].LB = 1; // # T05 -> F
                //    Y_RUse[8, 24, 2].LB = 1; // # F09 -> T06
                //    Y_RUse[14, 24, 2].LB = 1; // # T06 -> G
                //    Y_RUse[14, 23, 2].LB = 1; // # G -> T05
                //    Y_RUse[0, 19, 3].LB = 1; // # F01 -> T01
                //    Y_RUse[19, 20, 3].LB = 1; // # T01 -> T02
                //    Y_RUse[20, 21, 3].LB = 1; // # T02 -> T03
                //    Y_RUse[11, 21, 3].LB = 1; // # T03 -> D
                //    Y_RUse[11, 22, 3].LB = 1; // # D -> T04
                //    Y_RUse[13, 22, 3].LB = 1; // # T04 -> F
                //    Y_RUse[13, 27, 3].LB = 1; // # F -> S
                //    Y_RUse[5, 24, 3].LB = 1; // # F06 -> T06
                //    Y_RUse[14, 24, 3].LB = 1; // # T06 -> G
                //    Y_RUse[14, 23, 3].LB = 1; // # G -> T05
                //    Y_RUse[13, 23, 3].LB = 1; // # T05 -> F
                //    Y_RUse[6, 25, 4].LB = 1; // # F07 -> T07
                //    Y_RUse[18, 25, 4].LB = 1; // # T07 -> K
                //    Y_RUse[15, 18, 4].LB = 1; // # K -> H
                //    Y_RUse[15, 24, 4].LB = 1; // # H -> T06
                //    Y_RUse[14, 24, 4].LB = 1; // # T06 -> G
                //    Y_RUse[14, 23, 4].LB = 1; // # G -> T05
                //    Y_RUse[13, 23, 4].LB = 1; // # T05 -> F
                //    Y_RUse[13, 27, 4].LB = 1; // # F -> S
                //    Y_RUse[7, 26, 4].LB = 1; // # F08 -> T08
                //    Y_RUse[18, 26, 4].LB = 1; // # T08 -> K

                //    W_ROpen[2, 22, 1].LB = 1; //# 
                //    W_ROpen[13, 22, 1].LB = 1; //# 
                //    W_ROpen[13, 27, 1].LB = 1; //# 
                //    W_ROpen[3, 23, 1].LB = 1; //# 
                //    W_ROpen[13, 23, 1].LB = 1; //# 
                //    W_ROpen[9, 23, 1].LB = 1; //# 

                //    W_ROpen[1, 21, 2].LB = 1; // # F02 -> T03
                //    W_ROpen[11, 21, 2].LB = 1; // # T03 -> D
                //    W_ROpen[11, 22, 2].LB = 1; // # D -> T04
                //    Z_RMaint[13, 22, 2].LB = 1; // # T04 -> F
                //    Z_RMaint[13, 27, 2].LB = 1; // # F -> S
                //    W_ROpen[3, 4, 2].LB = 1; // #; // F05 -> F04
                //    Z_RMaint[3, 23, 2].LB = 1; // #; // F04 -> T05
                //    Z_RMaint[13, 23, 2].LB = 1; // # T05 -> F
                //    W_ROpen[8, 24, 2].LB = 1; // # F09 -> T06
                //    W_ROpen[14, 24, 2].LB = 1; // # T06 -> G
                //    W_ROpen[14, 23, 2].LB = 1; // # G -> T05
                //
                //    W_ROpen[0, 19, 3].LB = 1; // # F01 -> T01
                //    W_ROpen[19, 20, 3].LB = 1; // # T01 -> T02
                //    W_ROpen[20, 21, 3].LB = 1; // # T02 -> T03
                //    Z_RMaint[11, 21, 3].LB = 1; // # T03 -> D
                //    Z_RMaint[11, 22, 3].LB = 1; // # D -> T04
                //    Z_RMaint[13, 22, 3].LB = 1; // # T04 -> F
                //    Z_RMaint[13, 27, 3].LB = 1; // # F -> S
                //    W_ROpen[5, 24, 3].LB = 1; // # F06 -> T06
                //    Z_RMaint[14, 24, 3].LB = 1; // # T06 -> G
                //    Z_RMaint[14, 23, 3].LB = 1; // # G -> T05
                //    Z_RMaint[13, 23, 3].LB = 1; // # T05 -> F
                //
                //    W_ROpen[6, 25, 4].LB = 1; // # F07 -> T07
                //    W_ROpen[18, 25, 4].LB = 1; // # T07 -> K
                //    W_ROpen[15, 18, 4].LB = 1; // # K -> H
                //    W_ROpen[15, 24, 4].LB = 1; // # H -> T06
                //    Z_RMaint[14, 24, 4].LB = 1; // # T06 -> G
                //    Z_RMaint[14, 23, 4].LB = 1; // # G -> T05
                //    Z_RMaint[13, 23, 4].LB = 1; // # T05 -> F
                //    Z_RMaint[13, 27, 4].LB = 1; // # F -> S
                //    W_ROpen[7, 26, 4].LB = 1; // # F08 -> T08
                //    W_ROpen[18, 26, 4].LB = 1; // # T08 -> K
                //
                //    U_Trav[2, 22, 1].LB = 1; // # F03 -> T04
                //    U_Trav[13, 22, 1].LB = 1; // # T04 -> F
                //    U_Trav[13, 27, 1].LB = 1; // # F -> S
                //    U_Trav[3, 23, 1].LB = 1; // # F04 -> T05
                //    U_Trav[13, 23, 1].LB = 1; // # T05 -> F
                //    U_Trav[9, 23, 1].LB = 1; // # F10 -> T05
                //    U_Trav[1, 21, 2].LB = 1; // # F02 -> T03
                //    U_Trav[11, 21, 2].LB = 1; // # T03 -> D
                //    U_Trav[11, 22, 2].LB = 1; // # D -> T04
                //    U_Trav[13, 22, 2].LB = 1; // # T04 -> F
                //    U_Trav[13, 27, 2].LB = 1; // # F -> S
                //    U_Trav[3, 4, 2].LB = 1; // # F05 -> F04
                //    U_Trav[3, 23, 2].LB = 1; // # F04 -> T05
                //    U_Trav[13, 23, 2].LB = 1; // # T05 -> F
                //    U_Trav[8, 24, 2].LB = 1; // # F09 -> T06
                //    U_Trav[14, 24, 2].LB = 1; // # T06 -> G
                //    U_Trav[14, 23, 2].LB = 1; // # G -> T05
                //    U_Trav[0, 19, 3].LB = 1; // # F01 -> T01
                //    U_Trav[19, 20, 3].LB = 1; // # T01 -> T02
                //    U_Trav[20, 21, 3].LB = 1; // # T02 -> T03
                //    U_Trav[11, 21, 3].LB = 1; // # T03 -> D
                //    U_Trav[11, 22, 3].LB = 1; // # D -> T04
                //    U_Trav[13, 22, 3].LB = 1; // # T04 -> F
                //    U_Trav[13, 27, 3].LB = 1; // # F -> S
                //    U_Trav[5, 24, 3].LB = 1; // # F06 -> T06
                //    U_Trav[14, 24, 3].LB = 1; // # T06 -> G
                //    U_Trav[14, 23, 3].LB = 1; // # G -> T05
                //    U_Trav[13, 23, 3].LB = 1; // # T05 -> F
                //    U_Trav[6, 25, 4].LB = 1; // # F07 -> T07
                //    U_Trav[18, 25, 4].LB = 1; // # T07 -> K
                //    U_Trav[15, 18, 4].LB = 1; // # K -> H
                //    U_Trav[15, 24, 4].LB = 1; // # H -> T06
                //    U_Trav[14, 24, 4].LB = 1; // # T06 -> G
                //    U_Trav[14, 23, 4].LB = 1; // # G -> T05
                //    U_Trav[13, 23, 4].LB = 1; // # T05 -> F
                //    U_Trav[13, 27, 4].LB = 1; // # F -> S
                //    U_Trav[7, 26, 4].LB = 1; // # F08 -> T08
                //    U_Trav[18, 26, 4].LB = 1; // # T08 -> K 
                //}
            }

            string Pasta = MP.DataInst.Std.PathInstance;
            string Agora = MP.DataInst.Agora;

            StringBuilder constr = new StringBuilder();
            GRBGenConstr[] GConstr = Modelo.GetGenConstrs();
            for (int i = 0; i< GConstr.Length; i++)
            {
                constr.AppendLine(GConstr[i].ToString());
            }
            ToolBox.FileTxt($"{Pasta}{NomeArquivo}{Agora}[Constr].txt", constr, MP.ShowMessages);
            constr.Clear();

            Int64 val = 0;
            for (int i = 0; i < CounterVar.Length; i++)
            {
                WriteLog(MP.Uselog, logtxt, $"{NameVar[i]}\t{CounterVar[i]}");
            }
            WriteLog(MP.Uselog, logtxt, $"Total Vars: {val}\n");
            val = 0;
            for (int i = 0; i < CounterConstraint.Length; i++)
            {
                if (CounterConstraint[i] > 0)
                {
                    WriteLog(MP.Uselog, logtxt, $"R{i:00}\t{ToolBox.Constants.Constraints[i]}:\t {CounterConstraint[i]}");
                }
                else
                {
                    WriteLog(MP.Uselog, logtxt, $"R{i:00}\t{ToolBox.Constants.Constraints[i]}:\t Empty");
                }
                val += CounterConstraint[i];
            }
            WriteLog(MP.Uselog, logtxt, $"Total: {val}");

            if (!MP.Uselog)
            {
                StringBuilder fileCountConst = new();
                for (int i = 0; i < CounterConstraint.Length; i++)
                {
                    if (CounterConstraint[i] > 0)
                    {
                        fileCountConst.AppendLine($"R{ToolBox.Constants.Constraints[i]}:\t {CounterConstraint[i]}");
                    }
                }
                ToolBox.FileTxt($"{S.PathInstance}[CountConst]{NomeArquivo}{I.Agora}.txt", fileCountConst, MP.ShowMessages);
                fileCountConst.Clear();
            }
            

            if (MP.OldVersion)
            {
                NomeArquivo += "[orig]";
            }
            else
            {
                NomeArquivo += "[adap]";
            }

            
            GRBModel ModeloRelax = Modelo.Relax();
            //GRBAuxVars MV = I.Vars;

            List<(GRBModel, bool, string)> Models = new List<(GRBModel, bool, string)> { (Modelo, MP.ShowMessages, NomeArquivo) };//, (ModeloRelax, MP.ShowMessages, "(relax)" + NomeArquivo) };

            foreach ((GRBModel, bool, string) TM in Models)
            {
                if (MP.Uselog)
                {
                    WriteLog(MP.Uselog, logtxt, $"Model {TM.Item1}, ShowMess {TM.Item2}");
                }
                else
                {
                    //Chama a Otimização
                    OptimizeModel(TM.Item3, Pasta, Agora, TM.Item1, TM.Item2, MP);

                    WriteSolution(I, TM.Item1);
                }
            }
            foreach (FMU fmu in S.FMUs)
            {
                WriteLog(MP.Uselog, logtxt, fmu.ToString());
            }

            if (MP.Uselog)
            {
                string _namefile = @$"{Pasta}{Agora}_log.txt";
                using StreamWriter fs = new StreamWriter(_namefile);
                for (int l = 0; l < logtxt.Length; l++)
                {
                    fs.Write($"{l} - L:{logtxt[l].Capacity}\n");
                }
                foreach (StringBuilder lt in logtxt)
                {
                    fs.Write(lt);
                }
                fs.Close();
            }

            foreach ((GRBModel, bool, string) TM in Models)
            {
                TM.Item1.Dispose();
            }
        }


        /*
public void SolveFromFile(bool ShowMess, string MPSFile, string MSTFile = "")
{
   string tipo = "FromFile";
   string[] SplitName = MPSFile.Split('\\');
   StringBuilder sbAux = new StringBuilder();
   foreach (string str in SplitName)
   {
       if (SplitName.Last() != str)
       {
           sbAux.Append(str + "\\");
       }
   }
   string path = sbAux.ToString();

   string[] GetAgora = SplitName.Last().Split(new char[] { '(', ')' });
   sbAux.Clear();
   sbAux.Append(tipo);
   sbAux.Append('(');

   foreach (string ga in GetAgora)
   {
       if ((ga.Contains("_AM")) || (ga.Contains("_PM")))
       {
           string Agora = ga;
       }
       else if (GetAgora.Last() != ga)
       {
           sbAux.Append(ga);
       }
   }

   sbAux.Append(')');
   sbAux.Append("solved");

   string problem = sbAux.ToString();
   sbAux.Clear();

   Ambiente = new GRBEnv();

   GRBModel ModFromFile = new GRBModel(Ambiente, MPSFile);
   if (MSTFile.Length > 0)
   {
       ModFromFile.Read(MSTFile);
   }

   OptimizeModel(problem, path, ToolBox.GetNow(), ModFromFile, ShowMess);
}*/
        public void OptimizeModel(string NomeArq, string Pasta, string Agora, 
                                  GRBModel OpModelo, bool ShowMess, ModelParameters MP,
                                  bool relax = false)
        {
            Instance I = MP.DataInst;
            // Must set LazyConstraints parameter when using lazy constraints

            OpModelo.Parameters.LazyConstraints = 1;

            if (MP.Uselazy)
            {
                Callback_FH cb = new Callback_FH(MP);

                OpModelo.SetCallback(cb);
            }

            List<string> TypeModelsExt = new List<string> { "PL", "MPS"};//, "HNT", "BAS", "PRM", "ATTR" };
            //Escrita do Modelo
            foreach (string typefile in TypeModelsExt)
            {
                ToolBox.GravaModelo(typefile, Pasta, NomeArq, Agora, OpModelo, ShowMess);
            }


            DialogResult result = DialogResult.Yes;
            if (ShowMess)
            {
                result = MessageBox.Show("Calcular?", "Calcular", MessageBoxButtons.YesNo);
            }

            if (result == DialogResult.Yes)
            {
                OpModelo.Optimize();

                resp = ToolBox.AvaliaModelo(OpModelo.Status, ShowMess);
                try
                {
                    ModelObjVal = OpModelo.ObjVal;
                }
                catch (Exception)
                {
                    ModelObjVal = 0;
                    //throw;
                }
                TreatingOptimizeModel(resp, NomeArq, Pasta, Agora, OpModelo, relax);
            }
        }


        public void TreatingOptimizeModel(Tuple<bool, string, string> respfromModel, string NomeArq, string Pasta, string Agora, GRBModel OptModelo, bool ShowMess, bool relax = false)
        {
            //List<LazyType> Lazys = Callback_FH.ChoiceLazy(OptModelo.GetVars(), 20);

            //Teste de Erro
            string title;
            if (!(respfromModel.Item1))
            {
                StringBuilder msgErro = new StringBuilder();
                msgErro.Append(respfromModel.Item2);
                if (relax)
                {
                    title = string.Format("(relax-{0}){1}", resp_relax.Item3, NomeArq);
                }
                else
                {
                    title = string.Format("({0}){1}", respfromModel.Item3, NomeArq);
                }
                ToolBox.GravaMsgTxt(Pasta, title, Agora, 0, msgErro, ShowMess);
            }
            else
            {
                if (relax)
                {
                    title = "(relax)_res_" + NomeArq;
                }
                else
                {
                    title = "_res_" + NomeArq;
                }

                List<string> TypeModelsExt = new List<string> { "SOL", "MST", "DLP", "JSON" };//, "HNT", "BAS", "PRM", "ATTR" };
                //Escrita do Modelo
                foreach (string typefile in TypeModelsExt)
                {
                    ToolBox.GravaModelo(typefile, Pasta, title, Agora, OptModelo, ShowMess);
                }
            }
        }

        private double GetLPVUnit(Node unit, int p, int NS, bool hybrid, bool harvestSchedulingOptimize)
        {
            double LPV = 0;
            if (unit.IsFMU)
            {
                FMU? F = this.MP.DataInst.Std.GetFMUByNode(unit, this.MP.DataInst.Std.FMUs);
                //this.MP.DataInst.Std.GetFMUByNodeIndex(unit.Index, this.MP.DataInst.Std.FMUs);
                if (F is not null)
                {
                    for (int s = 0; s < NS; s++)
                    {
                        //add liquid profit value for harvest Unit
                        LPV += F.LPV.Sum();
                        if (hybrid || !harvestSchedulingOptimize)
                        {
                            //discounting minimum number of trucks if Unit is harvest at P times relative costs (load/unload)
                            LPV -= F.TrucksByPeriod[p] * this.MP.DataInst.Std.MatrixPar.Cost_TravelFix_ByPeriod[p];
                        }
                    }
                }
            }
            return LPV;
        }

        public GRBLinExpr R_M_first_open(int fmu_i, int fmu_j, int period_p, GRBVar[,,] W, GRBVar[,,] Z,
                                         double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, Z[fmu_i, fmu_j, period_p]);
            for (int p = 0; p<period_p; p++)
            {
                expr.AddTerm(-1, W[fmu_i, fmu_j, p]);
            }

            expr.AddConstant(-term);

            return expr;
        }

        public GRBLinExpr R_M_first_open(int fmu_i, int fmu_j, int period_p, GRBVar[,,] W, GRBVar[,,] Z,
                                          GRBVar[,,] Slk, GRBVar[,,] Exc,
                                          int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_first_open(fmu_i, fmu_j, period_p, W, Z, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p]);
            }

            return expr; 
        }

        //GRBVar[,,] Slk_M_Maintenance = new GRBVar[NI, NI, NP];
        //GRBVar[,,] Exc_M_Maintenance = new GRBVar[NI, NI, NP];
        // z_{ijp} & \leq \sum_{p'\in P_p} w_{ijp'} & \forall i, j \in F, \forall p > 2 \in P_p

        public GRBLinExpr R_M_Maintenance(int fmu_i, int fmu_j, Range periods_p, GRBVar[,,] W, GRBVar[,,] Z,
                                          double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            int period_p = periods_p.End.Value;
            int size_period_p = period_p - periods_p.Start.Value;
            expr.AddTerm(1, Z[fmu_i, fmu_j, period_p]);

            for (int p = period_p - size_period_p; p< size_period_p; p++) 
            { 
                expr.AddTerm(-1, W[fmu_i, fmu_j, p]);
                expr.AddTerm(-1, Z[fmu_i, fmu_j, p]);
            }

            expr.AddConstant(-term);

            return expr; //Revisado
        }
        public GRBLinExpr R_M_Maintenance(int fmu_i, int fmu_j, Range periods_p, GRBVar[,,] W, GRBVar[,,] Z,
                                          GRBVar[,,] Slk, GRBVar[,,] Exc,
                                          int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_Maintenance(fmu_i, fmu_j, periods_p, W, Z, term);
            int period_p = periods_p.End.Value;

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,] Slk_M_maint_open = new GRBVar[NI, NI, NP];
        //GRBVar[,] Exc_M_maint_open = new GRBVar[NI, NI, NP];
        //w_{ ijp}+z_{ ijp}& \leq 1 & \forall i, j \in F, \forall p \in P,  \label{eq:M_manut_abert} 

        public static GRBLinExpr R_M_maint_open(int fmu_i, int fmu_j, int period_p, 
                                                GRBVar[,,] W, GRBVar[,,] Z,
                                                double term = 1)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, W[fmu_i, fmu_j, period_p]);
            expr.AddTerm(1, Z[fmu_i, fmu_j, period_p]);

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public static GRBLinExpr R_M_maint_open(int fmu_i, int fmu_j, int period_p, 
                                                GRBVar[,,] W, GRBVar[,,] Z,
                                                GRBVar[,,] Slk, GRBVar[,,] Exc,
                                                int CoefSlackExcess = 0, double term = 1)
        {
            GRBLinExpr expr = R_M_maint_open(fmu_i, fmu_j, period_p, W, Z, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p]);
            }

            return expr; //Revisado
        }
        
        //GRBVar[,,] Slk_M_vehicle = new GRBVar[NI, NI, NP];
        //GRBVar[,,] Exc_M_vehicle = new GRBVar[NI, NI, NP];
        //y_{ ijp}& \leq w_ { ijp }+z_{ ijp}& \forall i, j \in F, \forall p \in P,  \label{eq:M_veiculo}  

        public static GRBLinExpr R_M_vehicle(int fmu_i, int fmu_j, int period_p,
                                             GRBVar[,,] W, GRBVar[,,] Z, GRBVar[,,] Y,
                                             double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, Y[fmu_i, fmu_j, period_p]);
            expr.AddTerm(-1, W[fmu_i, fmu_j, period_p]);
            expr.AddTerm(-1, Z[fmu_i, fmu_j, period_p]);
            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public static GRBLinExpr R_M_vehicle(int fmu_i, int fmu_j, int period_p,
                                             GRBVar[,,] W, GRBVar[,,] Z, GRBVar[,,] Y,
                                             GRBVar[,,] Slk, GRBVar[,,] Exc,
                                             int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_vehicle(fmu_i, fmu_j, period_p, W, Z, Y, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,,] Slk_M_use = new GRBVar[NI, NI, NP];
        //GRBVar[,,] Exc_M_use = new GRBVar[NI, NI, NP];
        //y_{ ijp}& \leq u_ { ijp }& \forall i, j \in F, \forall p \in P,  \label{eq:M_uso} 

        public static GRBLinExpr R_M_use(int fmu_i, int fmu_j, int period_p,
                                         GRBVar[,,] Y, GRBVar[,,] U,
                                         double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, Y[fmu_i, fmu_j, period_p]);
            expr.AddTerm(-1, U[fmu_i, fmu_j, period_p]);

            expr.AddConstant(-term);

            return expr; //Revisado
        }
        public static GRBLinExpr R_M_use(int fmu_i, int fmu_j, int period_p,
                                         GRBVar[,,] Y, GRBVar[,,] U,
                                         GRBVar[,,] Slk, GRBVar[,,] Exc,
                                         int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_use(fmu_i, fmu_j, period_p, Y, U, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,] Slk_M_acess = new GRBVar[NI, NP];
        //GRBVar[,] Exc_M_acess = new GRBVar[NI, NP];
        //    \sum_{ b\in B}x_{ ipb}& \leq \sum_{ j\in F}y_{ ijp}& \forall i \in F, \forall p \in P,  \label{eq:M_acesso}  %

        public GRBLinExpr R_M_acess(int fmu_i, int period_p,
                                    GRBVar[,,] X, GRBVar[,,] Y, 
                                    double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            for (int b = 0; b < NB; b++)
            {
                expr.AddTerm(1, X[fmu_i, period_p, b]);
            }

            //foreach (Edge E in I.Std.Edges.Where(e => e.Node1.Index == fmu_i))
            //{
            //int i = E.Node1.Index;
            //int j = E.Node2.Index;
            for (int j = 0; j < NI; j++)
            {
                if (fmu_i != j)
                {
                    expr.AddTerm(-1, Y[fmu_i, j, period_p]);
                }
            }
            //}

            expr.AddConstant(-term);

            return expr; //Revisado
        }
        public GRBLinExpr R_M_acess(int fmu_i, int period_p,
                                    GRBVar[,,] X, GRBVar[,,] Y, 
                                    GRBVar[,] Slk, GRBVar[,] Exc,
                                    int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_acess(fmu_i, period_p, X, Y, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,] Slk_M_flow = new GRBVar[NI, NP];
        //GRBVar[,] Exc_M_flow = new GRBVar[NI, NP];
        //    \sum_{ j\in F}u_{ jip}+ \varepsilon_{ ip}\left(\sum_{ b\in B}x_{ ipb}\right) & = \sum_{ j\in F}u_{ ijp}&
        //    \forall i \in F, \\ \forall p \in P  \label{ eq: M_fluxo}  %

        public GRBLinExpr R_M_flow(int fmu_i, int period_p,
                                   GRBVar[,,] X, GRBVar[,,] U,
                                   Instance I,
                                   double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            for(int j=0; j<I.Std.Nodes.Count; j++)
            {
                if (j != fmu_i)
                {
                    expr.AddTerm(1, U[j, fmu_i, period_p]);
                    expr.AddTerm(-1, U[fmu_i, j, period_p]);
                }
            }
            //foreach (Edge E in I.Std.Edges.Where(e => e.Node1.Index == fmu_i))
            //{
            //    int i = E.Node1.Index;
            //    int j = E.Node2.Index;
            //    expr.AddTerm(1, U[j, fmu_i, period_p]);
            //    expr.AddTerm(-1, U[fmu_i, j, period_p]);                
            //}
            FMU? F = I.Std.GetFMUByNodeIndex(fmu_i, I.Std.FMUs);

            if (F is not null)
            {
                int[] trucks = F.TrucksByPeriod.ToArray();

                for (int b = 0; b < NB; b++)
                {
                    for (int s = 0; s < NS; s++)
                    {
                        expr.AddTerm(trucks[period_p], X[fmu_i, period_p, b]);
                    }
                }
            } 

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_M_flow(int fmu_i, int period_p,
                                   GRBVar[,,] X, GRBVar[,,] U, Instance I,
                                   GRBVar[,] Slk, GRBVar[,] Exc,
                                   int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_flow(fmu_i, period_p, X, U, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, period_p]);
            }

            return expr; //Revisado
        }

        public GRBLinExpr R_M_selfflow(int fmu_i, int period_p, GRBVar[,,] V)
        {
            GRBLinExpr expr = new GRBLinExpr();
            
            expr.AddTerm(1, V[fmu_i, fmu_i, period_p]);

            return expr; //Revisado
        }

        //GRBVar[] Slk_M_AgC_qtharvest = new GRBVar[NI];
        //GRBVar[] Exc_M_AgC_qtharvest = new GRBVar[NI];
        //    \sum_{ b\in B}\sum_{ p\in P}x_{ ipb}& \leq 1 & \forall i \in F \label{eq:M_AgC_qtcolh} %

        public GRBLinExpr R_M_AgC_qtharvest(int fmu_i, GRBVar[,,] X, double term = 1)
        {
            GRBLinExpr expr = new GRBLinExpr();

            for (int b = 0; b < NB; b++)
            {
                for (int p = 0; p < NP; p++)//Aqui inclui-se p=0 para permitir os casos de talhões não colhidos
                {
                    expr.AddTerm(1, X[fmu_i, p, b]);
                }
            }

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_M_AgC_qtharvest(int fmu_i, GRBVar[,,] X,
                                            GRBVar[] Slk, GRBVar[] Exc,
                                            int CoefSlackExcess = 0, double term = 1)
        {
            GRBLinExpr expr = R_M_AgC_qtharvest(fmu_i, X, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i]);
            }

            return expr; //Revisado
        }        

        //GRBVar[] Slk_M_AgC_initialblock = new GRBVar[NI];
        //GRBVar[] Exc_M_AgC_initialblock = new GRBVar[NI];
        //&x_{ i11} = 1 & \forall i\in F_1 \label{eq:M_AgC_inibloco}

        public static GRBLinExpr R_M_AgC_initialblock(int fmu_i, GRBVar[,,] X,
                                                double term = 1)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, X[fmu_i, 1, 0]);

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public static GRBLinExpr R_M_AgC_initialblock(int fmu_i, GRBVar[,,] X,
                                               GRBVar[] Slk, GRBVar[] Exc,
                                               int CoefSlackExcess = 0, double term = 1)
        {
            GRBLinExpr expr = R_M_AgC_initialblock(fmu_i, X, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i]);
            }

            return expr; //Revisado
        }
        //GRBVar[,,] Slk_M_AgC_ageblock = new GRBVar[NI, NP, NB];
        //GRBVar[,,] Exc_M_AgC_ageblock = new GRBVar[NI, NP, NB];
        // (n_i + p - 1)x_{ ipb} \geq \eta_i x_ { ipb }
        // \forall i \in F, \forall p \in P, \\ \forall b \in B_p  \label{ eq: M_AgC_idadebloco}%\\[2mm]

        public static GRBLinExpr R_M_AgC_ageblock(int fmu_i, int period_p, int block_b,
                                                  double FMU_InitialAge, int FMU_HarvestAge,
                                                  GRBVar[,,] X, double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();
            expr.AddTerm(FMU_InitialAge + period_p - 1 - FMU_HarvestAge, X[fmu_i, period_p, block_b]);
            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public static GRBLinExpr R_M_AgC_ageblock(int fmu_i, int period_p, int block_b,
                                                  double FMU_InitialAge, int FMU_HarvestAge,
                                                  GRBVar[,,] X, GRBVar[,,] Slk, GRBVar[,,] Exc,
                                                  int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_AgC_ageblock(fmu_i, period_p, block_b, FMU_InitialAge,
                FMU_HarvestAge, X, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, period_p, block_b]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, period_p, block_b]);
            }

            return expr; //Revisado
        }

        //GRBVar[,] Slk_M_AgC_varvolmin = new GRBVar[NS, NP];
        //GRBVar[,] Exc_M_AgC_varvolmin = new GRBVar[NS, NP];
        //& \sum_{ b\in B}\sum_{ i \in F}\upsilon_{ isp}x_{ ipb} \geq(1 -\sigma)d_{ sp}
        //& \forall s\in S, \forall p\in P \label{eq:M_AgC_varvolmin}%\\

        public GRBLinExpr R_M_AgC_varvolmin(int sort_s, int period_p, Instance I,
                                            GRBVar[,,] X, double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int b = 0; b < NB; b++)
            {
                for (int i = 0; i < NI; i++)
                {
                    if (I.Std.Nodes[i].IsFMU)
                    {
                        FMU? Unit = I.Std.GetFMUByNode(I.Std.Nodes[i], I.Std.FMUs);
                        if (Unit is not null)
                        {
                            expr.AddTerm(Unit.Vols[period_p], X[i, period_p, b]);
                        }                        
                    }
                }
            }

            expr.AddConstant(-(1 - I.DemFactor) * I.SortimentDemands[sort_s, period_p]);

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_M_AgC_varvolmin(int sort_s, int period_p, Instance I, 
                                            GRBVar[,,] X, GRBVar[,] Slk, GRBVar[,] Exc,
                                            int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_AgC_varvolmin(sort_s, period_p, I, X,term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[sort_s, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[sort_s, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,] Slk_M_AgC_varvolmax = new GRBVar[NS, NP];
        //GRBVar[,] Exc_M_AgC_varvolmax = new GRBVar[NS, NP];
        //& \sum_{ b\in B}\sum_{ i \in F}\upsilon_{ isp}x_{ ipb} \leq(1 +\sigma)d_{ sp}& \forall s\in S, \forall p\in P \label{eq:M_AgC_varvolmax}%\\

        public GRBLinExpr R_M_AgC_varvolmax(int sort_s, int period_p, GRBVar[,,] X,  
                                            Instance I, double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();            
            for (int b = 0; b < NB; b++)
            {
                for (int i = 0; i < NI; i++)
                {
                    if (I.Std.Nodes[i].IsFMU)
                    {
                        FMU? Unit = I.Std.GetFMUByNode(I.Std.Nodes[i], I.Std.FMUs);
                        if (Unit is not null)
                        {
                            expr.AddTerm(Unit.Vols[period_p], X[i, period_p, b]);
                        }
                    }                    
                }
            }
            expr.AddConstant(-(1 + I.DemFactor) * I.SortimentDemands[sort_s, period_p]);
            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_M_AgC_varvolmax(int sort_s, int period_p, GRBVar[,,] X, Instance I,
                                            GRBVar[,] Slk, GRBVar[,] Exc, int CoefSlackExcess = 0,
                                            double term = 0)
        {
            GRBLinExpr expr = R_M_AgC_varvolmax(sort_s, period_p, X, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[sort_s, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[sort_s, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,] Slk_M_AgC_maxarea = new GRBVar[NB, NP];
        //GRBVar[,] Exc_M_AgC_maxarea = new GRBVar[NB, NP];
        //    & \sum_{i \in F} a_i x_{ipb} \leq \overline{A_p}\alpha_{pb} & \forall p \in P, \forall b \in B_p \label{eq:M_AgC_maxarea}%\\

        public GRBLinExpr R_M_AgC_maxarea(int period_p, int block_b,
                                          GRBVar[,,] X, GRBVar[,] A,
                                          Instance I,
                                          double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            for (int i = 0; i < NI; i++)
            {
                if (I.Std.Nodes[i].IsFMU) {
                    FMU Unit = I.Std.GetFMUByNode(I.Std.Nodes[i], I.Std.FMUs);
                    if (Unit is not null)
                    {
                        double Areafmu = Unit.Area;
                        expr.AddTerm(Areafmu, X[i, period_p, block_b]);
                    }
                }
            }

            expr.AddTerm(-I.Std.MatrixPar.MaximalHarvestArea[period_p], A[period_p, block_b]);

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_M_AgC_maxarea(int period_p, int block_b,
                                          GRBVar[,,] X, GRBVar[,] A, Instance I,
                                          GRBVar[,] Slk, GRBVar[,] Exc,
                                          int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_AgC_maxarea(period_p, block_b, X, A, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[block_b, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[block_b, period_p]);
            }

            return expr; //Revisado
        }


        //GRBVar[,] Slk_M_AgC_minarea = new GRBVar[NB, NP];
        //GRBVar[,] Exc_M_AgC_minarea = new GRBVar[NB, NP];
        //    & \sum_{i\in F} a_i x_{ipb} \geq \underline{A_p}\alpha_{pb} & \forall p \in P, \forall b \in B_p \label{eq:M_AgC_minarea}%\\

        public GRBLinExpr R_M_AgC_minarea(int period_p, int block_b,
                                          GRBVar[,,] X, GRBVar[,] A, 
                                          Instance I,
                                          double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            for (int i = 0; i < NI; i++)
            {
                if (I.Std.Nodes[i].IsFMU) { 
                    FMU Unit = I.Std.GetFMUByNode(I.Std.Nodes[i], I.Std.FMUs);
                    if (Unit is not null)
                    {
                        double Areafmu = Unit.Area;
                        expr.AddTerm(Areafmu, X[i, period_p, block_b]);
                    }
                }
            }

            expr.AddTerm(-I.Std.MatrixPar.MinimalHarvestArea[period_p], A[period_p, block_b]);
            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_M_AgC_minarea(int period_p, int block_b,
                                          GRBVar[,,] X, GRBVar[,] A, Instance I,
                                          GRBVar[,] Slk, GRBVar[,] Exc,
                                          int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_M_AgC_minarea(period_p, block_b, X, A, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[block_b, period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[block_b, period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[] Slk_B_AgC_firstblock = new GRBVar[NP];
        //GRBVar[] Exc_B_AgC_firstblock = new GRBVar[NP];
        //    & \alpha_{p1} = 1 & \forall p \in P \label{eq:B_AgC_primbloco}%\\

        public static GRBLinExpr R_B_AgC_firstblock(int period_p, GRBVar[,] A,
                                                    double term = 1)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, A[period_p, 0]);
            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public static GRBLinExpr R_B_AgC_firstblock(int period_p, GRBVar[,] A,
                                                    GRBVar[] Slk, GRBVar[] Exc,
                                                    int CoefSlackExcess = 0, double term = 1)
        {
            GRBLinExpr expr = R_B_AgC_firstblock(period_p, A, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[period_p]);
                expr.AddTerm(-CoefSlackExcess, Exc[period_p]);
            }

            return expr; //Revisado
        }

        //GRBVar[,,] Slk_B_AgC_createblock = new GRBVar[NS, NP, NB];
        //GRBVar[,,] Exc_B_AgC_createblock = new GRBVar[NS, NP, NB];
        //    \displaystyle \alpha_{pb} \leq \dfrac{1}{\tau}\PG{\dfrac{(1+\sigma)d_{sp} - \sum_{h=1}^{b-1}\sum_{i\in F}\upsilon_{isp}x_{iph} }{(1+\sigma)d_{sp}}} \nonumber
        //    \forall s\in S, \forall p \in P, b = 2,\ldots, |B| \label{eq:B_AgC_criabloco}%\\
        //    \somat{h}{1}{b-1} \sum_{i\in F} \upsilon_{isp}x_{iph} + \tau(1+\sigma)d_{sp} \alpha_{pb} \leq (1+\sigma)d_{sp}
        //    & \forall s\in S, \forall p \in P, b = 2,\ldots, |B|
        public GRBLinExpr R_B_AgC_createblock(int sort_s, int period_p, int block_b,
                                              GRBVar[,,] X, GRBVar[,] A, Instance I,
                                              double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            for (int h = 0; h < block_b; h++)
            {
                for (int i = 0; i < NI; i++)
                {
                    if (I.Std.Nodes[i].IsFMU)
                    {
                        FMU Unit = I.Std.GetFMUByNode(I.Std.Nodes[i], I.Std.FMUs);
                        if (Unit is not null)
                        {
                            double UnitVol = Unit.Vols[period_p];
                            expr.AddTerm(UnitVol, X[i, period_p, h]);
                        }
                    }
                }
            }

            expr.AddTerm(I.NewBlockPercent * (1 + I.DemFactor) * I.SortimentDemands[sort_s, period_p], A[period_p, block_b]);

            expr.AddConstant(-(1 + I.DemFactor) * I.SortimentDemands[sort_s, period_p]);
            expr.AddConstant(-term);

            return expr;//Revisado
        }

        public GRBLinExpr R_B_AgC_createblock(int sort_s, int period_p, int block_b,
                                              GRBVar[,,] X, GRBVar[,] A, Instance I,
                                              GRBVar[,,] Slk, GRBVar[,,] Exc,
                                              int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_B_AgC_createblock(sort_s, period_p, block_b, X, A, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[sort_s, period_p, block_b]);
                expr.AddTerm(-CoefSlackExcess, Exc[sort_s, period_p, block_b]);
            }

            return expr;//Revisado
        }

        //GRBVar[,,,] Slk_B_AgC_nearblock = new GRBVar[NI, NI, NP, NB];
        //GRBVar[,,,] Exc_B_AgC_nearblock = new GRBVar[NI, NI, NP, NB];
        //    & \delta_{ij}\PG{x_{ipb}+x_{jpb}-1} \leq \Delta_{b}
        //    \forall i \in F, \forall j > i \in F, \\ \forall p \in P, \forall b \in B_p \label{eq:B_AgC_proxbloco}

        public GRBLinExpr R_B_AgC_nearblock(int fmu_i, int fmu_j, int period_p, int block_b,
                                            GRBVar[,,] X, Instance I, double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(I.SizeTraj[fmu_i, fmu_j], X[fmu_i, period_p, block_b]);
            expr.AddTerm(I.SizeTraj[fmu_i, fmu_j], X[fmu_j, period_p, block_b]);
            expr.AddConstant(-I.SizeTraj[fmu_i, fmu_j]);

            //expr.AddTerm(I.FMUDistances[fmu_i, fmu_j], X[fmu_i, period_p, block_b]);
            //expr.AddTerm(I.FMUDistances[fmu_i, fmu_j], X[fmu_j, period_p, block_b]);
            //expr.AddConstant(-I.FMUDistances[fmu_i, fmu_j]);
            expr.AddConstant(-I.MaxDistInBlock);

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_B_AgC_nearblock(int fmu_i, int fmu_j, int period_p, int block_b,
                                            GRBVar[,,] X, Instance I, GRBVar[,,,] Slk, GRBVar[,,,] Exc,
                                            int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_B_AgC_nearblock(fmu_i, fmu_j, period_p, block_b, X, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p, block_b]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p, block_b]);
            }

            return expr; //Revisado
        }

        //GRBVar[,,,,] Slk_B_AgC_nearyear = new GRBVar[NI, NI, NP, NB, NB];
        //GRBVar[,,,,] Exc_B_AgC_nearyear = new GRBVar[NI, NI, NP, NB, NB];
        //     \delta_{ij}\PG{x_{ipb}+x_{jph}-1} \leq \Delta_{p}
        //     \forall i \in F, \forall j > i \in F, \forall p \in P\\ \forall b \in B_p, h \neq b \in B_p \label{eq:B_AgC_proxano}

        public GRBLinExpr R_B_AgC_nearyear(int fmu_i, int fmu_j, int period_p, int block_b, int block_h,
                                           GRBVar[,,] X, Instance I, double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(I.SizeTraj[fmu_i, fmu_j], X[fmu_i, period_p, block_b]);
            expr.AddTerm(I.SizeTraj[fmu_i, fmu_j], X[fmu_j, period_p, block_h]);
            expr.AddConstant(-I.SizeTraj[fmu_i, fmu_j]);

            //expr.AddTerm(I.FMUDistances[fmu_i, fmu_j], X[fmu_i, period_p, block_b]);
            //expr.AddTerm(I.FMUDistances[fmu_i, fmu_j], X[fmu_j, period_p, block_h]);
            //expr.AddConstant(-I.FMUDistances[fmu_i, fmu_j]);
            expr.AddConstant(-I.MaxDistOutBlockInPeriod);

            expr.AddConstant(-term);

            return expr; //Revisado
        }

        public GRBLinExpr R_B_AgC_nearyear(int fmu_i, int fmu_j, int period_p, int block_b, int block_h,
                                           GRBVar[,,] X, Instance I, GRBVar[,,,,] Slk, GRBVar[,,,,] Exc,
                                           int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_B_AgC_nearyear(fmu_i, fmu_j, period_p, block_b, block_h, X, I, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p, block_b, block_h]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p, block_b, block_h]);
            }

            return expr; //Revisado
        }

        //GRBVar[,,,,] Slk_B_AgC_sequence = new GRBVar[NI, NI, NP, NB, NB];
        //GRBVar[,,,,] Exc_B_AgC_sequence = new GRBVar[NI, NI, NP, NB, NB];
        //    x_{ipb}+x_{jph} \leq 1
        //    \begin{matrix} \forall (i,j) \in A, \forall p \in P\\ \forall b \in B_p, h = b+1,\ldots,|B| \label{eq:B_AgC_seq}

        public static GRBLinExpr R_B_AgC_sequence(int fmu_i, int fmu_j, int period_p, int block_b, 
                                                  int block_h, GRBVar[,,] X, double term = 1)
        {
            GRBLinExpr expr = new GRBLinExpr();

            expr.AddTerm(1, X[fmu_i, period_p, block_b]);
            expr.AddTerm(1, X[fmu_j, period_p, block_h]);

            expr.AddConstant(-term);

            return expr; //Revisada
        }

        public static GRBLinExpr R_B_AgC_sequence(int fmu_i, int fmu_j, int period_p, int block_b, 
                                                  int block_h, GRBVar[,,] X, GRBVar[,,,,] Slk, 
                                                  GRBVar[,,,,] Exc, int CoefSlackExcess = 0, 
                                                  double term = 1)
        {
            GRBLinExpr expr = R_B_AgC_sequence(fmu_i, fmu_j, period_p, block_b, block_h, X, term);

            if (CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p, block_b, block_h]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p, block_b, block_h]);
            }

            return expr; //Revisada
        }

        //GRBVar[,,,,] Slk_B_AgC_consecutive = new GRBVar[NI, NI, NP, NB, NB];
        //GRBVar[,,,,] Exc_B_AgC_consecutive = new GRBVar[NI, NI, NP, NB, NB];
        //    & x_{ipb}+x_{j(p+1)h} \leq 1
        //    & \forall (i,j) \in A, \forall b \in B_p,  \forall h \in B_p, \\ p = 1,\ldots,HP-1 \label{eq:B_AgC_consec}

        public GRBLinExpr R_B_AgC_consecutive(int fmu_i, int fmu_j, int period_p, int block_b, int block_h,
                                              GRBVar[,,] X, double term = 1)
        {
            GRBLinExpr expr = new GRBLinExpr();
            if (period_p < NP - 1)
            {
                expr.AddTerm(1, X[fmu_i, period_p, block_b]);
                expr.AddTerm(1, X[fmu_j, period_p + 1, block_h]);

                expr.AddConstant(-term);
            }
            return expr; //Revisado
        }

        public GRBLinExpr R_B_AgC_consecutive(int fmu_i, int fmu_j, int period_p, int block_b, int block_h,
                                              GRBVar[,,] X, GRBVar[,,,,] Slk, GRBVar[,,,,] Exc,
                                              int CoefSlackExcess = 0, double term = 1)
        {
            GRBLinExpr expr = R_B_AgC_consecutive(fmu_i, fmu_j, period_p, block_b, block_h, X, term);
            if (period_p < NP - 1 && CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p, block_b, block_h]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p, block_b, block_h]);
            }
            return expr; //Revisado
        }

        //GRBVar[,,,,] Slk_B_AgC_nearconsec = new GRBVar[NI, NI, NP, NB, NB];
        //GRBVar[,,,,] Exc_B_AgC_nearconsec = new GRBVar[NI, NI, NP, NB, NB];
        //    & \delta_{ij}\PG{x_{ipb}+x_{j(p+1)h}} \leq 2\Delta_{c}
        //    & \forall i,j \in F, \forall b \in B_p, \forall h \in B_p, \\  p = 1, \ldots, HP-1 \label{eq:B_AgC_proxconsec}

        public GRBLinExpr R_B_AgC_nearconsec(int fmu_i, int fmu_j, int period_p, int block_b, int block_h,
                                             GRBVar[,,] X, Instance I, double term = 0)
        {
            GRBLinExpr expr = new GRBLinExpr();

            if (period_p < NP - 1)
            {
                expr.AddTerm(I.SizeTraj[fmu_i, fmu_j], X[fmu_i, period_p, block_b]);
                expr.AddTerm(I.SizeTraj[fmu_i, fmu_j], X[fmu_j, period_p + 1, block_h]);
                //expr.AddTerm(I.FMUDistances[fmu_i, fmu_j], X[fmu_i, period_p, block_b]);
                //expr.AddTerm(I.FMUDistances[fmu_i, fmu_j], X[fmu_j, period_p + 1, block_h]);

                expr.AddConstant(-2 * I.MaxDistConsecutivePeriod);
                expr.AddConstant(-term);
            }
            return expr;//Revisado
        }

        public GRBLinExpr R_B_AgC_nearconsec(int fmu_i, int fmu_j, int period_p, int block_b, int block_h,
                                             GRBVar[,,] X, Instance I, GRBVar[,,,,] Slk, GRBVar[,,,,] Exc,
                                             int CoefSlackExcess = 0, double term = 0)
        {
            GRBLinExpr expr = R_B_AgC_nearconsec(fmu_i, fmu_j, period_p, block_b, block_h, X, I, term);

            if (period_p < NP - 1 && CoefSlackExcess != 0)
            {
                expr.AddTerm(CoefSlackExcess, Slk[fmu_i, fmu_j, period_p, block_b, block_h]);
                expr.AddTerm(-CoefSlackExcess, Exc[fmu_i, fmu_j, period_p, block_b, block_h]);

            }
            return expr;//Revisado
        }

        public void WriteSolution(Instance I, GRBModel M)
        {
            using StreamWriter fileSol = new($"{I.Std.PathInstance}[Sol]{I.Std.NameInstance}{I.Agora}.txt");

            StringBuilder[] sb = new StringBuilder[2];
            sb[0] = new StringBuilder();
            sb[1] = new StringBuilder();

            fileSol.WriteLine($"{I.Std.NameInstance}\n{I.Agora}");
            fileSol.WriteLine($"\t\\item Nodes: {I.NumNodes}");
            fileSol.WriteLine($"\t\\item FMUs: {I.Std.FMUs.Count}");
            fileSol.WriteLine($"\t\\item Edges: {I.NumEdges}");
            fileSol.WriteLine($"\t\\item NumVars: {M.NumVars}");
            fileSol.WriteLine($"\t\\item NumBinVars: {M.NumBinVars}");
            fileSol.WriteLine($"\t\\item NumConstrs: {M.NumConstrs}");

            Tuple<bool, string, string> resp = ToolBox.AvaliaModelo(M.Status, false);

            if (resp.Item1)
            {

                using StreamWriter VarCSV = new($"{I.Std.PathInstance}{I.Std.NameInstance}{I.Agora}Vars.tsv");
                StringBuilder sbvarcsv = new StringBuilder();
                sbvarcsv.Append("VarX\ti1\ti2\ti3\ti4\tVal\n");
                GRBVar[] GB = M.GetVars();
                string GBVal = M.ObjVal.ToString("F04");
                string GBRuntime = M.Runtime.ToString("E");
                sb[0].AppendLine($"Obj: {GBVal}\nRuntime: {GBRuntime}");
                sb[0].AppendLine("Não Nulos");
                foreach (GRBVar gb in GB)
                {
                    //Solution File
                    try
                    {
                        if (!gb.VarName.Contains("Slk") && !gb.VarName.Contains("Exc"))
                        {
                            string[] indices = gb.VarName.Split('_');
                            string line = $"{gb.VarName}\t{gb.X.ToString("F04")}";
                            if (gb.X == 0)
                            {
                                sb[1].AppendLine(line);
                            }
                            else
                            {
                                sb[0].AppendLine(line);
                            }                            
                        }
                    }
                    catch (GRBException e)
                    {
                        sb[0].AppendLine($"{gb.VarName}\tErro {e.Message}");
                    }

                    try
                    {
                        string v0 = gb.VarName.Replace('_', '\t');
                        string v1 = gb.X.ToString("F04", new CultureInfo("pt-br"));
                        int sizev0 = 5 - v0.Split("\t").Length;
                        StringBuilder vsb = new StringBuilder();
                        vsb.Append(v0);
                        if (sizev0 > 0)
                        {
                            for(int s=0; s<sizev0; s++)
                            {
                                vsb.Append("\t");
                            }
                        }
                        vsb.Append($"\t{v1}");
                        sbvarcsv.AppendLine(vsb.ToString());
                        vsb.Clear();
                    }
                    catch (GRBException e)
                    {
                        string v0 = gb.VarName.Replace('_', '\t');
                        sbvarcsv.AppendLine($"{v0},Erro {e.Message}");
                    }
                }
                VarCSV.WriteLine(sbvarcsv.ToString());
                VarCSV.Close();
                sbvarcsv.Clear();
            }
            else
            {
                sb[0].AppendLine($"{resp.Item2}\n{resp.Item3}");
            }

            fileSol.WriteLine(sb[0].ToString());
            if (sb[1].Length > 0)
            {
                fileSol.WriteLine("Nulos");
                fileSol.WriteLine(sb[1].ToString());
            }

            fileSol.Close();
        }
    }
}
