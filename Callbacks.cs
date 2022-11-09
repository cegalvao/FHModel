using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FHModel
{
    class LazyType
    {
        public GRBLinExpr lazy;
        public char sense;
        public double rhs;
        public string desc;

        public LazyType(GRBLinExpr lazy, char sense = GRB.EQUAL, double rhs = 0, string desc = "")
        {
            this.lazy = lazy ?? throw new ArgumentNullException(nameof(lazy));
            this.sense = sense;
            this.rhs = rhs;
            this.desc = desc;
        }
        public override string ToString()
        {
            return $"{desc} {sense.ToString()} {rhs}";
        }
    }

    class EvalConstr
    {
        public bool ok;
        public GRBLinExpr linExpr;
        public char sense;
        public double rhs;
        private string name;
        private string descr;
        //private ModelVars V;

        public EvalConstr(bool _ok, GRBLinExpr _linExpr, char _sense,
                          double _rhs, string _name, string _descr)//, ModelVars _v)
        {
            ok = _ok;
            linExpr = _linExpr;
            sense = _sense;
            rhs = _rhs;
            name = _name;
            descr = _descr;
            //V = _v;
        }

        public override string ToString()
        {
            return $"Const {name} ({ok}): {descr.Replace('\n',' ')}\n";
        }

        public LazyType ToLazyType(char Sense = GRB.EQUAL, double Rhs = 0)
        {
            return new LazyType(linExpr, Sense, Rhs, this.ToString());
        }
    }

    class Callback_FH : GRBCallback
    {
        private double lastiter;
        private double lastnode;
        private GRBAuxVars vars;
        private StreamWriter logfile;
        private Instance RI;
        private ModelParameters MP;
        public string[] NameConstraints = ToolBox.Constants.Constraints;
        public string[] NameVar = ToolBox.Constants.NameVar;

        public Callback_FH(ModelParameters M)
        {
            lastiter = lastnode = -GRB.INFINITY;
            MP = M;
            RI = M.DataInst;
            vars = RI.Vars;
            logfile = new($"{RI.Std.PathInstance}{RI.Std.NameInstance}{ToolBox.GetNow()}[cblog].log");
        }

        protected override void Callback()
        {
            try
            {
                if (where == GRB.Callback.POLLING)
                {
                    // Ignore polling callback
                }
                else if (where == GRB.Callback.PRESOLVE)
                {
                    string str_calllog = $"{RI.Std.PathInstance}_{RI.Std.NameInstance}_{ToolBox.GetNow()}_Presolve_call.log";
                    //ToolBox.ifMess(MP.ShowMessages, str_calllog);
                    StringBuilder calllog = new();

                    // Presolve callback
                    int cdels = GetIntInfo(GRB.Callback.PRE_COLDEL);
                    int rdels = GetIntInfo(GRB.Callback.PRE_ROWDEL);
                    if (cdels != 0 || rdels != 0)
                    {
                        Console.WriteLine(cdels + " columns and " + rdels
                            + " rows are removed");
                        calllog.AppendLine(cdels + " columns and " + rdels
                            + " rows are removed");
                    }
                    if (calllog.Length > 0)
                    {
                        ToolBox.FileTxt(str_calllog, calllog, false);
                        calllog.Clear();
                    }
                }
                else if (where == GRB.Callback.SIMPLEX)
                {
                    string str_calllog = $"{RI.Std.PathInstance}_{RI.Std.NameInstance}_{ToolBox.GetNow()}_SIMPLEX_call.log";
                    //ToolBox.ifMess(MP.ShowMessages, str_calllog);
                    StringBuilder calllog = new();

                    // Simplex callback
                    double itcnt = GetDoubleInfo(GRB.Callback.SPX_ITRCNT);
                    if (itcnt - lastiter >= 100)
                    {
                        lastiter = itcnt;
                        double obj = GetDoubleInfo(GRB.Callback.SPX_OBJVAL);
                        int ispert = GetIntInfo(GRB.Callback.SPX_ISPERT);
                        double pinf = GetDoubleInfo(GRB.Callback.SPX_PRIMINF);
                        double dinf = GetDoubleInfo(GRB.Callback.SPX_DUALINF);
                        char ch;
                        if (ispert == 0) ch = ' ';
                        else if (ispert == 1) ch = 'S';
                        else ch = 'P';
                        string msg = $"{itcnt}  {obj}{ch} {pinf} {dinf}\n";
                        Console.WriteLine(msg);
                        calllog.AppendLine(msg);
                    }
                    if (calllog.Length > 0)
                    {
                        ToolBox.FileTxt(str_calllog, calllog, false);
                        calllog.Clear();
                    }
                }
                else if (where == GRB.Callback.MIP)
                {
                    string str_calllog = $"{RI.Std.PathInstance}_{RI.Std.NameInstance}_{ToolBox.GetNow()}_MIP_call.log";
                    //ToolBox.ifMess(MP.ShowMessages, str_calllog);
                    StringBuilder calllog = new();
                    // General MIP callback
                    double nodecnt = GetDoubleInfo(GRB.Callback.MIP_NODCNT);
                    double objbst = GetDoubleInfo(GRB.Callback.MIP_OBJBST);
                    double objbnd = GetDoubleInfo(GRB.Callback.MIP_OBJBND);
                    int solcnt = GetIntInfo(GRB.Callback.MIP_SOLCNT);
                    if (nodecnt - lastnode >= 100)
                    {
                        lastnode = nodecnt;
                        int actnodes = (int)GetDoubleInfo(GRB.Callback.MIP_NODLFT);
                        int itcnt = (int)GetDoubleInfo(GRB.Callback.MIP_ITRCNT);
                        int cutcnt = GetIntInfo(GRB.Callback.MIP_CUTCNT);
                        string msg = nodecnt + " " + actnodes + " "
                            + itcnt + " " + objbst + " " + objbnd + " "
                            + solcnt + " " + cutcnt;
                        Console.WriteLine(msg);
                        calllog.AppendLine(msg);
                    }
                    if (Math.Abs(objbst - objbnd) < 0.01 * (1.0 + Math.Abs(objbst)))
                    {
                        string msg = "Stop early - 1% gap achieved";
                        Console.WriteLine(msg);
                        calllog.AppendLine(msg);
                        logfile.Close();
                        ToolBox.ifMess(false, msg);
                        if (calllog.Length > 0)
                        {
                            ToolBox.FileTxt(str_calllog, calllog, false);
                            calllog.Clear();
                        }
                        ToolBox.FileTxt($"{RI.Std.PathInstance}_{RI.Std.NameInstance}_{ToolBox.GetNow()}_MIP_msg.txt", msg, false);
                        Abort();
                    }
                    if (nodecnt >= 1000000 && solcnt > 0)
                    {
                        string msg = "Stop early - 1000000 nodes explored";
                        Console.WriteLine(msg);
                        calllog.AppendLine(msg);
                        logfile.Close();
                        ToolBox.ifMess(false, msg);
                        if (calllog.Length > 0)
                        {
                            ToolBox.FileTxt(str_calllog, calllog, false);
                            calllog.Clear();
                        }
                        ToolBox.FileTxt($"{RI.Std.PathInstance}_{RI.Std.NameInstance}_{ToolBox.GetNow()}_MIP_msg.txt", msg, false);
                        Abort();

                    }
                    if (calllog.Length > 0)
                    {
                        ToolBox.FileTxt(str_calllog, calllog, false);
                        calllog.Clear();
                    }
                }
                else if (where == GRB.Callback.MIPSOL)
                {
                    string str_calllog = $"{RI.Std.PathInstance}_{RI.Std.NameInstance}_{ToolBox.GetNow()}_MIPSOL_call.log";
                    //ToolBox.ifMess(MP.ShowMessages, str_calllog);
                    StringBuilder calllog = new();
                    // MIP solution callback
                    vars.FillAux();//Criação da lista de variáveis auxiliares no MIPSOL
                    foreach (AuxVars grbv in vars.Aux)
                    {
                        grbv.vsol = GetSolution(grbv.v);
                    }
                    List<LazyType> Lazys = ChoiceLazy(vars, 20);
                    if (Lazys.Count > 0)
                    {
                        foreach (LazyType L in Lazys)
                        {
                            if (MP.Uselazy)
                            {
                                AddLazy(L.lazy, L.sense, L.rhs);
                            }
                            calllog.AppendLine(L.ToString());
                        }
                    }
                    if (calllog.Length > 0)
                    {
                        ToolBox.FileTxt(str_calllog, calllog, false);
                        calllog.Clear();
                    }

                    int nodecnt = (int)GetDoubleInfo(GRB.Callback.MIPSOL_NODCNT);
                    double obj = GetDoubleInfo(GRB.Callback.MIPSOL_OBJ);
                    int solcnt = GetIntInfo(GRB.Callback.MIPSOL_SOLCNT);
                    double[] x = GetSolution(vars.vars);
                    if (x != null)
                    {
                        string msg = $"**** New solution at node {nodecnt}"
                            + $", obj {obj}, sol {solcnt}"
                            + $", x[0] = {x[0]} ****\n";
                        Console.WriteLine(msg);
                        //ToolBox.ifMess(true, mess);
                        if (msg != null) logfile.Write(msg);
                    }
                    else
                    {
                        string msg = $"**** WARNING at node {nodecnt}"
                            + $", obj {obj}, sol {solcnt}"
                            + ", x[0] = NULL  ****\n";
                        Console.WriteLine(msg);
                        //ToolBox.ifMess(true, mess);
                        if (msg != null) logfile.Write(msg);
                    }
                }
                else if (where == GRB.Callback.MIPNODE)
                {
                    // MIP node callback
                    Console.WriteLine("**** New node ****");
                    if (GetIntInfo(GRB.Callback.MIPNODE_STATUS) == GRB.Status.OPTIMAL)
                    {
                        double[] x = GetNodeRel(vars.vars);
                        SetSolution(vars.vars, x);
                    }
                }
                else if (where == GRB.Callback.BARRIER)
                {
                    // Barrier callback
                    int itcnt = GetIntInfo(GRB.Callback.BARRIER_ITRCNT);
                    double primobj = GetDoubleInfo(GRB.Callback.BARRIER_PRIMOBJ);
                    double dualobj = GetDoubleInfo(GRB.Callback.BARRIER_DUALOBJ);
                    double priminf = GetDoubleInfo(GRB.Callback.BARRIER_PRIMINF);
                    double dualinf = GetDoubleInfo(GRB.Callback.BARRIER_DUALINF);
                    double cmpl = GetDoubleInfo(GRB.Callback.BARRIER_COMPL);

                    string msg = $"{itcnt} {primobj} {dualobj} {priminf} "+
                        $"{dualinf} {cmpl}\n";
                    Console.WriteLine(msg);
                    if (msg != null) logfile.Write(msg);

                }
                else if (where == GRB.Callback.MESSAGE)
                {
                    // Message callback
                    string msg = GetStringInfo(GRB.Callback.MSG_STRING);
                    if (msg != null) logfile.Write(msg);
                    //ToolBox.ifMess(msg != null, msg);
                }

            }
            catch (GRBException e)
            {
                Console.WriteLine("Error code: " + e.ErrorCode);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during callback");
                Console.WriteLine(e.StackTrace);
            }

        }

        public List<LazyType> ChoiceLazy(GRBAuxVars xvars, int LenghtRandom)
        {

            List<LazyType> lazysAll = new List<LazyType>();
            lazysAll.AddRange(Lazy_M_Flow_List(xvars));
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_M_AgC_initialblock(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_M_AgC_varvolmax(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_M_AgC_maxarea(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_B_AgC_createblock(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_B_AgC_nearblock(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_B_AgC_nearyear(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_B_AgC_consecutive(xvars));
            //}
            //if (lazysAll.Count == 0)
            //{
                lazysAll.AddRange(Lazy_B_AgC_nearconsec(xvars));
            //}

            int limitlist = lazysAll.Count < LenghtRandom ? lazysAll.Count : LenghtRandom;
            List<string> lazynames = new List<string>();

            foreach(LazyType l in lazysAll)
            {
                lazynames.Add(l.ToString());
            }

            List<LazyType> lazys = new List<LazyType>();
            Random random = new Random(0);
            for (int i = 0; i < limitlist; i++)
            {
                lazys.Add(lazysAll[random.Next(lazysAll.Count)]);
            }
            ToolBox.FileTxt($"Lazys_{ToolBox.GetNow()}.txt", lazynames, false);
            return lazys;
        }

        public EvalConstr Eval_R_M_Flow(GRBAuxVars V, int i, int p, double term = 0, char sense = GRB.EQUAL, double rhs = 0)
        {
            //\sum_{j\in F} u_{jip} + \varepsilon_{ip}\left(\sum_{b\in B} x_{ipb}\right) & = \sum_{j\in F} u_{ijp}
            // \forall i \in F, \\ \forall p \in P  \label{eq:M_fluxo}

            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[5],
                            new List<(string, int)> { ("I", i), ("P", p) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();

            double NumTravels = 0.0;
            FMU? F = RI.Std.GetFMUByNodeIndex(i, RI.Std.FMUs);

            if (F is not null)
            {
                NumTravels = (double)F.TrucksByPeriod.ToArray().Sum();
            }

            double[,] Uij = V.GetVSolByGroup("Uij")[p]; //GetSolution(V.GetVarsByGroup("Uij")[p]);
            double FinalVal = 0.0;

            for (int j = 0; j < RI.NumNodes; j++)
            {
                desc.AppendLine(" - " + V.U[i, j, p].VarName + $"({Uij[i, j]})");

                FinalVal -= Uij[i, j];//V.U[i, j, p].X;
                linExpr.AddTerm(1, V.U[i, j, p]);

                desc.AppendLine(" + " + V.U[j, i, p].VarName + $"({Uij[j, i]})");
                FinalVal += Uij[j, i];// V.U[j, i, p].X;
                linExpr.AddTerm(1, V.U[j, i, p]);
            }

            double[,] Xib = V.GetVSolByGroup("Xib")[p]; // GetSolution(V.GetVarsByGroup("Xib")[p]);

            for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
            {
                desc.AppendLine($" + {NumTravels} * {V.X[i, p, b].VarName}" + $"({Xib[i, b]})");
                FinalVal += NumTravels * Xib[i, b];//V.X[i, p, b].X;
                linExpr.AddTerm(NumTravels, V.X[i, p, b]);
            }

            desc.AppendLine($" - {term} {sense} {rhs}");
            FinalVal -= term;
            linExpr.AddConstant(-term);

            ok = (FinalVal == rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());
        }
        public EvalConstr Eval_R_M_AgC_initialblock(GRBAuxVars V, int i, double term = 1, char sense = GRB.EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[7],
                            new List<(string, int)> { ("I", i) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xp = V.GetVSolByGroup("Xib")[0]; //

            linExpr.AddTerm(1, V.X[i, 1, 0]);
            desc.AppendLine($" {V.X[i, 1, 0].VarName} ({Xp[i, 1]})");
            FinalVal += Xp[i, 1];

            linExpr.AddConstant(-term);
            desc.AppendLine($" - {term} {sense} {rhs}");
            FinalVal -= term;

            ok = (FinalVal == rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_M_AgC_varvolmax(GRBAuxVars V, int s, int p, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[10],
                            new List<(string, int)> { ("S", s), ("P", p) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xib = V.GetVSolByGroup("Xib")[p]; //

            for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
            {
                for (int i = 0; i < RI.NumNodes; i++)
                {
                    if (RI.Std.Nodes[i].IsFMU)
                    {
                        FMU Unit = RI.Std.GetFMUByIndex(i, RI.Std.FMUs);
                        linExpr.AddTerm(Unit.Vols[p], V.X[i, p, b]);
                        desc.AppendLine($" + {Unit.Vols[p]} {V.X[i, p, b].VarName} ({Xib[i, b]})");
                        FinalVal += Unit.Vols[p] * Xib[i, b];
                    }
                }
            }

            linExpr.AddConstant(-(1 + RI.DemFactor) * RI.SortimentDemands[s, p]);
            desc.AppendLine($"- {(1 + RI.DemFactor)} * {RI.SortimentDemands[s, p]}");
            FinalVal -= (1 + RI.DemFactor) * RI.SortimentDemands[s, p];

            linExpr.AddConstant(-term);
            desc.AppendLine($" - {term} {sense} {rhs}");
            FinalVal -= term;

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_M_AgC_maxarea(GRBAuxVars V, int p, int b, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[11],
                            new List<(string, int)> { ("P", p), ("B", b) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            //int period_p, int block_b, GRBVar[,,] X, GRBVar[,] A,double term = 0)

            double[,] Xib = V.GetVSolByGroup("Xib")[p]; //
            double[,] A = V.GetVSolByGroup("A")[0];

            for (int i = 0; i < RI.NumNodes; i++)
            { 
                double Areafmu = RI.Std.Nodes[i].IsFMU ? RI.Std.GetFMUByNodeIndex(i, RI.Std.FMUs).Area : 0;
                linExpr.AddTerm(Areafmu, V.X[i, p, b]);
                desc.AppendLine($" + {Areafmu} * {V.X[i, p, b].VarName} ({Xib[i, b]})");
                FinalVal += (Areafmu) * Xib[i, b];
            }

            linExpr.AddTerm(-RI.Std.MatrixPar.MaximalHarvestArea[p], V.A[p, b]);
            desc.AppendLine($"- {RI.Std.MatrixPar.MaximalHarvestArea[p]} * {V.A[p, b].VarName} ({A[p, b]})");
            FinalVal -= RI.Std.MatrixPar.MaximalHarvestArea[p] * A[p, b];

            linExpr.AddConstant(-term);
            desc.AppendLine($" - {term} {sense} {rhs}");
            FinalVal -= term;

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_B_AgC_createblock(GRBAuxVars V, int s, int p, int b, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[14],
                            new List<(string, int)> { ("S", s), ("P", p), ("B", b) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xip = V.GetVSolByGroup("Xip")[p];
            double[,] A = V.GetVSolByGroup("A")[0];

            for (int h = 0; h < b; h++)
            {
                for (int i = 0; i < RI.NumNodes; i++)
                {
                    if (RI.Std.Nodes[i].IsFMU)
                    {
                        double UnitVol = RI.Std.GetFMUByNodeIndex(i, RI.Std.FMUs).Vols[p];
                        linExpr.AddTerm(UnitVol, V.X[i, p, h]);
                        desc.AppendLine($" + {UnitVol}*{V.X[i, p, h].VarName} ({Xip[i, p]})");
                        FinalVal += UnitVol * Xip[i, p];
                    }
                }
            }

            linExpr.AddTerm(RI.NewBlockPercent * (1 + RI.DemFactor) * RI.SortimentDemands[s, p], V.A[p, b]);
            desc.AppendLine($"+ {RI.NewBlockPercent} * {(1 + RI.DemFactor)} * {RI.SortimentDemands[s, p]} * {V.A[p, b].VarName}");
            FinalVal += (1 + RI.DemFactor) * RI.SortimentDemands[s, p] * A[p, b];

            linExpr.AddConstant(-(1 + RI.DemFactor) * RI.SortimentDemands[s, p] - term);
            desc.AppendLine($"- {(1 + RI.DemFactor)} * {RI.SortimentDemands[s, p]} - {term} {sense} {rhs}");
            FinalVal -= (1 + RI.DemFactor) * RI.SortimentDemands[s, p] + term;

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_B_AgC_nearblock(GRBAuxVars V, int i, int j, int p, int b, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[15],
                            new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xib = V.GetVSolByGroup("Xib")[p];

            linExpr.AddTerm(RI.FMUDistances[i, j], V.X[i, p, b]);
            desc.AppendLine($"+ {RI.FMUDistances[i, j]} * {V.X[i, p, b].VarName} ({Xib[i, b]})");
            FinalVal += (RI.FMUDistances[i, j]) * Xib[i, b];

            linExpr.AddTerm(RI.FMUDistances[i, j], V.X[j, p, b]);
            desc.AppendLine($"+ {RI.FMUDistances[i, j]} * {V.X[j, p, b].VarName} ({Xib[j, b]})");
            FinalVal += (RI.FMUDistances[i, j]) * Xib[j, b];

            linExpr.AddConstant(-RI.FMUDistances[i, j] - RI.MaxDistInBlock - term);
            desc.AppendLine($"- {RI.FMUDistances[i, j]} - {RI.MaxDistInBlock} - {term} {sense} {rhs}");
            FinalVal -= RI.FMUDistances[i, j] + RI.MaxDistInBlock + term;

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_B_AgC_nearyear(GRBAuxVars V, int i, int j, int p, int b, int h, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[16],
                            new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xib = V.GetVSolByGroup("Xib")[p];

            linExpr.AddTerm(RI.FMUDistances[i, j], V.X[i, p, b]);
            desc.AppendLine($" + {RI.FMUDistances[i, j]} * {V.X[i, p, b].VarName} ({Xib[i, b]})");
            FinalVal += (RI.FMUDistances[i, j] * Xib[i, b]);

            linExpr.AddTerm(RI.FMUDistances[i, j], V.X[j, p, h]);
            desc.AppendLine($" + {RI.FMUDistances[i, j]} * {V.X[j, p, h].VarName} ({Xib[j, h]})");
            FinalVal += (RI.FMUDistances[i, j] * Xib[j, h]);

            linExpr.AddConstant(-RI.FMUDistances[i, j] - RI.MaxDistOutBlockInPeriod - term);
            desc.AppendLine($" - {RI.FMUDistances[i, j]} - {RI.MaxDistOutBlockInPeriod} - {term} {sense} {rhs}");
            FinalVal -= (RI.FMUDistances[i, j] + RI.MaxDistOutBlockInPeriod + term);

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_B_AgC_consecutive(GRBAuxVars V, int i, int j, int p, int b, int h, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[18],
                            new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xib = V.GetVSolByGroup("Xib")[p];
            double[,] Xib2 = V.GetVSolByGroup("Xib")[p + 1];

            if (p < RI.NumPeriods - 1)
            {
                linExpr.AddTerm(1, V.X[i, p, b]);
                desc.AppendLine($" + {V.X[i, p, b].VarName} ({Xib[i, b]})");
                FinalVal += Xib[i, b];

                linExpr.AddTerm(1, V.X[j, p + 1, h]);
                desc.AppendLine($" + {V.X[j, p + 1, h].VarName} ({Xib2[j, h]})");
                FinalVal += Xib2[j, h];

                linExpr.AddConstant(-term);
                desc.AppendLine($" - {term} {sense} {rhs}");
                FinalVal -= term;
            }

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }
        public EvalConstr Eval_R_B_AgC_nearconsec(GRBAuxVars V, int i, int j, int p, int b, int h, double term = 0, char sense = GRB.LESS_EQUAL, double rhs = 0)
        {
            bool ok;
            string name = ToolBox.ConstraintName("R" + NameConstraints[19],
                            new List<(string, int)> { ("I", i), ("J", j), ("P", p), ("B", b), ("H", h) });
            StringBuilder desc = new StringBuilder();
            GRBLinExpr linExpr = new GRBLinExpr();
            double FinalVal = 0.0;

            double[,] Xib = V.GetVSolByGroup("Xib")[p];
            double[,] Xib2 = V.GetVSolByGroup("Xib")[p + 1];

            if (p < RI.NumPeriods - 1)
            {
                linExpr.AddTerm(RI.FMUDistances[i, j], V.X[i, p, b]);
                desc.AppendLine($" + {RI.FMUDistances[i, j]} * {V.X[i, p, b].VarName} ({Xib[i, b]})");
                FinalVal += RI.FMUDistances[i, j] * Xib[i, b];

                linExpr.AddTerm(RI.FMUDistances[i, j], V.X[j, p + 1, h]);
                desc.AppendLine($" + {RI.FMUDistances[i, j]} * {V.X[j, p + 1, h].VarName} ({Xib2[j, h]})");
                FinalVal += RI.FMUDistances[i, j] * Xib2[j, h];

                linExpr.AddConstant(-2 * RI.MaxDistConsecutivePeriod - term);
                desc.AppendLine($" - {2 * RI.MaxDistConsecutivePeriod} - {term} {sense} {rhs}");
                FinalVal -= 2 * RI.MaxDistConsecutivePeriod + term;
            }

            ok = (FinalVal <= rhs);

            return new EvalConstr(ok, linExpr, sense, rhs, name, desc.ToString());//, V);
        }

        public static GRBVar GetVarByName(GRBVar[] xvars, string name)
        {
            var pickN = from v in xvars
                        where v.VarName == name
                        select v;
            if (pickN.Any())
            {
                return pickN.First();
            }
            else
            {
                return null;
            }
        }
        public List<LazyType> Lazy_M_Flow_List(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            for (int p = 0; p < RI.NumPeriods; p++)
            {
                for (int i = 0; i < RI.NumNodes; i++)
                {
                    if (RI.Std.Nodes[i].IsFMU)
                    {
                        EvalConstr eval = Eval_R_M_Flow(xvars, i, p, 0);
                        if (!eval.ok)
                        {
                            lazys.Add(eval.ToLazyType());
                        }
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_M_AgC_initialblock(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            for (int i = 0; i < RI.NumNodes - 1; i++)
            {
                if (RI.Std.Nodes[i].IsFMU)
                {
                    if (RI.Std.GetFMUByNodeIndex(i, RI.Std.FMUs).IsFirstBlock)
                    {
                        EvalConstr eval = Eval_R_M_AgC_initialblock(xvars, i);
                        if (!eval.ok)
                        {
                            lazys.Add(eval.ToLazyType());
                        }
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_M_AgC_varvolmax(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            for (int s = 0; s < RI.NumSortiments; s++)
            {
                for (int p = 0; p < RI.NumPeriods; p++)
                {
                    EvalConstr eval = Eval_R_M_AgC_varvolmax(xvars, s, p);
                    if (!eval.ok)
                    {
                        lazys.Add(eval.ToLazyType());
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_M_AgC_maxarea(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            for (int p = 0; p < RI.NumPeriods; p++)
            {
                for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
                {
                    EvalConstr eval = Eval_R_M_AgC_maxarea(xvars, p, b);
                    if (!eval.ok)
                    {
                        lazys.Add(eval.ToLazyType());
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_B_AgC_createblock(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            for (int s = 0; s < RI.NumSortiments; s++)
            {
                for (int p = 0; p < RI.NumPeriods; p++)
                {
                    for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
                    {
                        EvalConstr eval = Eval_R_B_AgC_createblock(xvars, s, p, b);
                        if (!eval.ok)
                        {
                            lazys.Add(eval.ToLazyType());
                        }
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_B_AgC_nearblock(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            for (int i = 0; i < RI.NumNodes - 1; i++)
            {
                if (RI.Std.GetNodeByIndex(i, RI.Std.Nodes).IsFMU)
                {
                    for (int j = i + 1; j < RI.NumNodes; j++)
                    {
                        if (RI.Std.GetNodeByIndex(j, RI.Std.Nodes).IsFMU)
                        {
                            for (int p = 0; p < RI.NumPeriods - 1; p++)
                            {
                                for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
                                {
                                    EvalConstr eval = Eval_R_B_AgC_nearblock(xvars, i, j, p, b);
                                    if (!eval.ok)
                                    {
                                        lazys.Add(eval.ToLazyType());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_B_AgC_nearyear(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();

            for (int i = 0; i < RI.NumNodes; i++)
            {
                if (RI.Std.Nodes[i].IsFMU)
                {
                    for (int j = i + 1; j < RI.NumNodes; j++)
                    {
                        if (RI.Std.Nodes[j].IsFMU)
                        {
                            for (int p = 1; p < RI.NumPeriods; p++)
                            {
                                for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
                                {
                                    for (int h = 0; h < RI.Std.MatrixPar.MaxNumBlocks; h++)
                                    {
                                        if (h != b)
                                        {
                                            EvalConstr eval = Eval_R_B_AgC_nearyear(xvars, i, j, p, b, h);
                                            if (!eval.ok)
                                            {
                                                lazys.Add(eval.ToLazyType());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_B_AgC_consecutive(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();
            foreach ((int, int) adjs in RI.Std.Adjacentes)
            {
                int i = adjs.Item1;
                int j = adjs.Item2;
                for (int p = 1; p < RI.NumPeriods; p++)
                {
                    for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks - 1; b++)
                    {
                        for (int h = b + 1; h < RI.Std.MatrixPar.MaxNumBlocks; h++)
                        {
                            EvalConstr eval = Eval_R_B_AgC_consecutive(xvars, i, j, p, b, h);
                            if (!eval.ok)
                            {
                                lazys.Add(eval.ToLazyType());
                            }
                        }
                    }
                }
            }
            return lazys;
        }
        public List<LazyType> Lazy_B_AgC_nearconsec(GRBAuxVars xvars)
        {
            List<LazyType> lazys = new List<LazyType>();

            for (int i = 0; i < RI.NumNodes; i++)
            {
                if (RI.Std.Nodes[i].IsFMU)
                {
                    for (int j = i + 1; j < RI.NumNodes; j++)
                    {
                        if (RI.Std.Nodes[j].IsFMU)
                        {
                            for (int p = 1; p < RI.NumPeriods; p++)
                            {
                                for (int b = 0; b < RI.Std.MatrixPar.MaxNumBlocks; b++)
                                {
                                    for (int h = 0; h < RI.Std.MatrixPar.MaxNumBlocks; h++)
                                    {
                                        if (h != b)
                                        {
                                            EvalConstr eval = Eval_R_B_AgC_nearconsec(xvars, i, j, p, b, h);
                                            if (!eval.ok)
                                            {
                                                lazys.Add(eval.ToLazyType());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lazys;
        }
    }
}
