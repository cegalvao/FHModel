using Gurobi;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
//using System.Management.Instrumentation;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace FHModel
{
    public static class ToolBox
    {
        public static class Constants
        {
            public const string PathBase = "C:\\Users\\Usuario\\OneDrive - ufpr.br\\VS_HP\\";
            // public const string PathBaseD = "D:\\OneDrive - ufpr.br\\Visual Studio 2019\\";
            public static string[] NameVar = { 
                "X_FMU", "W_ROp", "Z_RMn", "Y_RUs", "U_Trv", "Alpha" };
            public static string[] Constraints =
            {
            "00_M_First_open",
            "01_M_Maintenance",
            "02_M_maint_open",
            "03_M_vehicle",
            "04_M_use",
            "05_M_acess",
            "06_M_flow",
            "07_M_AgC_qtharvest",
            "08_M_AgC_initialblock",
            "09_M_AgC_ageblock",
            "10_M_AgC_varvolmin",
            "11_M_AgC_varvolmax",
            "12_M_AgC_maxarea",
            "13_M_AgC_minarea",
            "14_B_AgC_firstblock",
            "15_B_AgC_createblock",
            "16_B_AgC_nearblock",
            "17_B_AgC_nearyear",
            "18_B_AgC_sequence",
            "19_B_AgC_consecutive",
            "20_B_AgC_nearconsec"
            };
        }

        public static void IfMess(bool ShowMessage, string Message)
        {
            if (ShowMessage)
            {
                MessageBox.Show(Message);
            }
        }
        public static void IfMess(bool ShowMessage, string Message, string Title)
        {
            if (ShowMessage)
            {
                MessageBox.Show(Message, Title);
            }
        }

        public static void IfMess(bool ShowMessage, string Message, string Title, MessageBoxButtons Button)
        {
            if (ShowMessage)
            {
                MessageBox.Show(Message, Title, Button);
            }
        }

        public static void IfMess(bool ShowMessage, string Message, string Title, MessageBoxButtons Button, MessageBoxIcon Icon)
        {
            if (ShowMessage)
            {
                MessageBox.Show(Message, Title, Button, Icon);
            }
        }

        public static bool IfNull(object? t)
        {
            return t is null;
        }

        public static bool IfNotNull(object? t)
        {
            return t is not null;
        }
        
        public static object? CondtionalNull(object? t, object NotNullAns)
        {
            object? resp = t is not null ? NotNullAns : null;
            return resp;
        }

        public static object CondtionalNull(object? t, object NotNullAns, object NullAns)
        {
            object resp = t is not null ? NotNullAns : NullAns;
            return resp;
        }

        public static string? CondtionalNull(object? t, string? NotNullAns)
        {
            string? resp = t is not null ? NotNullAns : null;
            return resp;
        }

        public static string CondtionalNull(object? t, string NotNullAns, string NullAns)
        {
            string resp = t is not null ? NotNullAns : NullAns;
            return resp;
        }

        public static GRBVar? CondtionalNull(object? t, GRBVar NotNullAns)
        {
            GRBVar? resp = t is not null ? NotNullAns : null;
            return resp;
        }

        public static GRBVar CondtionalNull(object? t, GRBVar NotNullAns, GRBVar NullAns)
        {
            GRBVar resp = t is not null ? NotNullAns : NullAns;
            return resp;
        }

        public static void AddL<T>(List<T> L, T t)
        {
            if (L is null)
            {
                throw new ArgumentNullException(nameof(L));
            }

            if (t is null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            if (!L.Contains(t))
            {
                L.Add(t);
            }
        }
        public static string Path(string p = "", string unity = "D")
        {
            if (unity == "C")
            {
                return Constants.PathBase + p;
            }
            else
            {
                return Constants.PathBase + p;//Constants.PathBaseD + p;
            }
        }

        public static double Distancia2Pontos(int x1, int x2, int y1, int y2)
        { //Distancia Euclidiana entre dois pontos
            double DeltaX = x2 - (double)x1;
            double DeltaY = y2 - (double)y1;

            return Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

        }
        public static double Distancia2Pontos(double x1, double x2, double y1, double y2)
        { //Distancia Euclidiana entre dois pontos
            double DeltaX = (double)x2 - (double)x1;
            double DeltaY = (double)y2 - (double)y1;

            return Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

        }
        public static List<Color> CriarCoresAleatoriasParaPontos(int numpontos = 1)
        {
            List<Color> CoresPontos = new();
            Random Aleatorio = new(1);
            for (int j = 0; j < numpontos; j++)
            {
                int A = Aleatorio.Next(100, 181);
                int R = Aleatorio.Next(0, 256);
                int G = Aleatorio.Next(0, 256);
                int B = Aleatorio.Next(0, 256);
                CoresPontos.Add(Color.FromArgb(A, R, G, B));
            }
            return CoresPontos;
        }
        public static List<List<double>> CriarListaDePontos(int numPontos, int dimensao, double lb, double ub, int randomseed = 0)
        {
            Random Aleatorio = new(randomseed);
            List<List<double>> Pontos = new();
            for (int i = 0; i < numPontos; i++)
            {
                List<double> Ponto = new();
                for (int j = 0; j < dimensao; j++)
                {
                    double coord = lb + (ub - lb) * Aleatorio.NextDouble();
                    Ponto.Add(coord);
                }
                Pontos.Add(Ponto);
                Thread.Sleep(Convert.ToInt32(Ponto[dimensao - 1] % 5) + 1);
            }
            return Pontos;
        }
        public static List<(double, double)> CriarListaDePontos(int numPontos, double lbx, double ubx, double lby, double uby, int randomseed = 0)
        {
            Random Aleatorio = new(randomseed);
            List<(double, double)> Pontos = new();
            //double xmax = 0;
            //double ymax = 0;
            //double xmin = 0;
            //double ymin = 0;
            for (int i = 0; i < numPontos; i++)
            {
                double coordx = lbx + (ubx - lbx) * Aleatorio.NextDouble();
                double coordy = lby + (uby - lby) * Aleatorio.NextDouble();
                /*
                if (i == 0)
                {
                    xmax = coordx;
                    xmin = coordx;
                    ymax = coordy;
                    ymin = coordy;
                }
                else
                {
                    xmax = coordx > xmax ? coordx : xmax;
                    xmin = coordx < xmin ? coordx : xmin;
                    ymax = coordy > ymax ? coordy : ymax;
                    ymin = coordy < ymin ? coordy : ymin;
                }*/
                Pontos.Add((coordx, coordy));
                Thread.Sleep(Convert.ToInt32(coordx % 5) + 1);
            }
            //Pontos.Add((0.50000000000000001 * (xmax + xmin), 0.5*(ymax + ymin)));
            return Pontos;
        }
        public static (List<(double, double)>, List<int>) CriarPontosPoligonoRegular(int numPontos, double radius = 250, double Xcenter = 250, double Ycenter = 250)
        {
            List<(double, double)> Pontos = new();
            List<int> LIndex = new();
            for (int i = 0; i < numPontos; i++)
            {
                double alpha = i * 2 * Math.PI / numPontos;
                double pX = -Math.Cos(alpha) * radius + Xcenter;
                double pY = -Math.Sin(alpha) * radius + Ycenter;
                Pontos.Add((pX, pY));
                LIndex.Add(i);
            }

            return (Pontos, LIndex);
        }
        public static List<List<double>> CriarListaDePontosCirc(int numPontos, double lb, double ub)
        {
            List<List<double>> pts = new();
            double XCent = lb + (ub - lb) / 2;
            double YCent = lb + (ub - lb) / 2;
            double gdeRaio = (ub - lb) / 2;

            for (int i = 0; i < numPontos; i++)
            {
                List<double> Ponto = new();
                double alpha = i * 2 * Math.PI / numPontos;
                double pX = -Math.Cos(alpha) * gdeRaio + XCent;
                Ponto.Add(pX);
                double pY = -Math.Sin(alpha) * gdeRaio + YCent;
                Ponto.Add(pY);
                pts.Add(Ponto);
            }
            return pts;
        }
        public static List<List<double>> CriarListaDePontosRect(int numPontos, double Xmin, double Ymin, double width, double height)
        {
            List<List<double>> pts = new();
            double perimeter = 2 * (width + height);
            double part = perimeter / numPontos;

            for (int i = 0; i < numPontos; i++)
            {
                List<double> Ponto = new();
                double lenpart = i * part;

                if (lenpart < width)
                {//Primeiro Eixo
                    Ponto.Add(Xmin + lenpart);
                    Ponto.Add(Ymin);
                    pts.Add(Ponto);
                }
                else if (lenpart < width + height)
                {//Segundo Eixo
                    Ponto.Add(Xmin + width);
                    Ponto.Add(Ymin + lenpart - width);
                    pts.Add(Ponto);
                }
                else if (lenpart < 2 * width + height)
                {//Terceiro Eixo
                    Ponto.Add(Xmin + width - (lenpart - width - height));
                    Ponto.Add(Ymin + height);
                    pts.Add(Ponto);
                }
                else
                {//Quarto Eixo
                    Ponto.Add(Xmin);
                    Ponto.Add(Ymin + height - (lenpart - 2 * width - height));
                    pts.Add(Ponto);
                }

            }
            return pts;
        }
        public static void GravaModelo(string tipo, string path, string problem, string Agora, GRBModel Model, bool ShowMess)
        {
            var NomeBase = tipo + problem + Agora;
            string ext = SwitchExt(tipo);

            string Nome = path + NomeBase + ext;

            try
            {
                Model.Write(Nome);
            }
            catch (GRBException E)
            {
                StringBuilder msg = new();
                msg.Append(Nome + " gerou erro Gurobi: \n" + E.Message);
                if (ShowMess)
                {
                    MessageBox.Show(msg.ToString());
                }
                GravaMsgTxt(path, problem + "[erro" + tipo + "]", Agora, 0, msg, ShowMess);
            }


            if (tipo == "SOL" || tipo == "ATTR")
            {
                SolutionFile(path, NomeBase + ext, ShowMess);
            }
            if (ShowMess)
            {
                MessageBox.Show(Nome + "\nGravado", "Grava Modelo");
            }
        }
        public static void GravaModelo(string tipo, string path, string[] OtherParameters, GRBModel Model, bool ShowMess)
        {
            StringBuilder NomeBase = new();
            NomeBase.Append(tipo);
            foreach (string str in OtherParameters)
            {
                NomeBase.Append(str + '_');
            }

            string ext = SwitchExt(tipo);

            NomeBase.Append(ext);

            string Nome = path + NomeBase.ToString();

            try
            {
                Model.Write(Nome);
            }
            catch (GRBException E)
            {
                MessageBox.Show(Nome + " gerou erro Gurobi: \n" + E.Message);
                throw;
            }

            if (tipo == "SOL")
            {
                SolutionFile(path, NomeBase.ToString(), ShowMess);
            }

            if (ShowMess)
            {
                MessageBox.Show(Nome + "\nGravado", "Grava Modelo");
            }
        }
        public static string SwitchExt(string tipo)
        {
            string ext = "";
            switch (tipo)
            {
                case "PL":
                    //.mps, .rew, .lp, or.rlp for writing the model itself
                    ext = ".lp";
                    break;
                case "SOL":
                    //.sol for writing the current solution
                    ext = ".sol";
                    break;
                case "MPS":
                    //.mps, .rew, .lp, or.rlp for writing the model itself
                    ext = ".mps";
                    break;
                case "MST":
                    ext = ".mst";
                    break;
                case "ILP":
                    //.ilp for writing just the IIS associated with an infeasible model(see Model.computeIIS for further information)
                    ext = ".ilp";
                    break;
                case "HNT":
                    //.hnt for writing a hint file
                    ext = ".hnt";
                    break;
                case "BAS":
                    //.bas for writing an LP basis
                    ext = ".bas";
                    break;
                case "PRM":
                    //.prm for writing modified parameter settings
                    ext = ".prm";
                    break;
                case "ATTR":
                    //.attr for writing model attributes
                    ext = ".attr";
                    break;
                case "JSON":
                    //.json for writing solution
                    ext = ".json";
                    break;
                case "DUA":
                case "DLP":
                    //.dua or .dlp for dual LP
                    ext = ".dlp";
                    break;
            }
            return ext;
        }
        public static void FileTxt(string Nome, string strcontent, bool ShowMess)
        {
            StringBuilder content = new();
            content.AppendLine(strcontent);
            FileTxt(Nome, content, ShowMess);
        }
        public static void FileTxt(string Nome, string[] strcontent, bool ShowMess)
        {
            StringBuilder content = new();
            foreach (string s in strcontent)
            {
                content.AppendLine(s);
            } 
            FileTxt(Nome, content, ShowMess);
        }
        public static void FileTxt(string Nome, List<string> listcontent, bool ShowMess)
        {
            StringBuilder text = new();
            foreach (string s in listcontent)
            {
                text.AppendLine(s);
            }

            FileTxt(Nome, text, ShowMess);
        }
        public static void FileTxt(string Nome, StringBuilder content, bool ShowMess)
        {
            Nome = TestPath(Nome);

            using (StreamWriter writer = new(Nome))
            {
                writer.WriteLine(content.ToString());
            }
            if (ShowMess)
            {
                MessageBox.Show(Nome + "\nGravado", "Grava Modelo");
            }
        }

        private static string TestPath(string nome)
        {
            string Nome = nome;
            if (!(nome.Contains(ToolBox.Constants.PathBase)))
            {
                Nome = $"{ToolBox.Constants.PathBase}\\{nome}";
            }
            return Nome;
        }

        public static void GravaMsgTxt(string path, string problem, string Agora, int ord, StringBuilder text, bool ShowMess)
        {
            string Nome = path + problem + Agora + ord.ToString() + ".txt";

            using (StreamWriter writer = new(Nome))
            {
                writer.WriteLine(text.ToString());
            }
            if (ShowMess)
            {
                MessageBox.Show(Nome + "\nGravado", "Grava Modelo");
            }
        }
        public static void GravaMsgTxt(string path, string[] OtherParameters, StringBuilder text, bool ShowMess)
        {
            StringBuilder sbAux = new();
            sbAux.Append(path);
            foreach (string str in OtherParameters)
            {
                sbAux.Append(str + '_');
            }
            sbAux.Append(".txt");

            string Nome = sbAux.ToString();

            using (StreamWriter writer = new(Nome))
            {
                writer.WriteLine(text.ToString());
            }
            if (ShowMess)
            {
                MessageBox.Show(Nome + "\nGravado", "Grava Modelo");
            }
        }
        public static void SolutionFile(string path, string NameFileSolution, bool ShowMess)
        {
            string[] AllDatFile = File.ReadAllLines(path + NameFileSolution);
            string Nome = path + "Filt_" + NameFileSolution;

            if (AllDatFile.Length > 0)
            {
                using StreamWriter writer = new(Nome);
                writer.WriteLine(AllDatFile[0]);
                for (int i = 1; i < AllDatFile.Length; i++)
                {
                    if (AllDatFile[i].IndexOf(" 0") != AllDatFile[i].Length - 2)
                    {
                        writer.WriteLine(AllDatFile[i].Replace(' ', '\t'));
                    }
                }
            }
            if (ShowMess)
            {
                MessageBox.Show(Nome + "\nGravado", "Grava Modelo");
            }
        }
        public static double DistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            double RaioTerra = 6.371e3; // km
            double AnguloPhi1 = lat1 * Math.PI / 180; // φ, λ in radians
            double AnguloPhi2 = lat2 * Math.PI / 180;
            double DeltaPhi = (lat2 - lat1) * Math.PI / 180;
            double DeltaLambda = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(DeltaPhi / 2) * Math.Sin(DeltaPhi / 2) +
                      Math.Cos(AnguloPhi1) * Math.Cos(AnguloPhi2) *
                      Math.Sin(DeltaLambda / 2) * Math.Sin(DeltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RaioTerra * c; // in metres
        }
        public static double DistanciaHaversine(int lat1G, int lat1M, double lat1S, string lat1Q,
                                                int lon1G, int lon1M, double lon1S, string lon1Q,
                                                int lat2G, int lat2M, double lat2S, string lat2Q,
                                                int lon2G, int lon2M, double lon2S, string lon2Q, bool ShowMess)
        {
            Dictionary<string, List<string>> Quadrantes = new()
            {
                ["lat"] = new List<string>(),
                ["lon"] = new List<string>()
            };
            Quadrantes["lat"].Add("N");
            Quadrantes["lat"].Add("S");
            Quadrantes["lon"].Add("E");
            Quadrantes["lon"].Add("W");
            Quadrantes["lon"].Add("L");
            Quadrantes["lon"].Add("O");

            List<string> VarQuadLat = new();
            List<string> VarQuadLon = new();
            VarQuadLat.Add(lat1Q);
            VarQuadLat.Add(lat2Q);
            VarQuadLon.Add(lon1Q);
            VarQuadLon.Add(lon2Q);
            foreach (string q in VarQuadLat)
            {
                if (!Quadrantes["lat"].Contains(q))
                {
                    if (ShowMess)
                    {
                        MessageBox.Show(q + " nao e´ latitude válida. \n Use N ou S.");
                    }
                }
            }

            foreach (string q in VarQuadLon)
            {
                if (!Quadrantes["lon"].Contains(q))
                {
                    MessageBox.Show(q + " nao e´ longitude válida. \n Use E/L ou W/O.");
                }
            }

            /*double int_lat1S = Math.Truncate(lat1S);
            double decimal_lat1S = lat1S - int_lat1S;

            double int_lon1S = Math.Truncate(lon1S);
            double decimal_lon1S = lon1S - int_lon1S;

            double int_lat2S = Math.Truncate(lat2S);
            double decimal_lat2S = lat2S - int_lat2S;

            double int_lon2S = Math.Truncate(lon2S);
            double decimal_lon2S = lon2S - int_lon2S;*/

            double lat1 = Convert.ToDouble(lat1G) + Convert.ToDouble(lat1M / 60) + Convert.ToDouble(lat1S / 3600);
            double lat2 = Convert.ToDouble(lat2G) + Convert.ToDouble(lat2M / 60) + Convert.ToDouble(lat2S / 3600);
            double lon1 = Convert.ToDouble(lon1G) + Convert.ToDouble(lon1M / 60) + Convert.ToDouble(lon1S / 3600);
            double lon2 = Convert.ToDouble(lon2G) + Convert.ToDouble(lon2M / 60) + Convert.ToDouble(lon2S / 3600);

            if (lat1Q == "S")
            {
                lat1 *= -1;
            }
            if (lat2Q == "S")
            {
                lat2 *= -1;
            }

            if (lon1Q == "W" | lon1Q == "O")
            {
                lon1 *= -1;
            }
            if (lon2Q == "W" | lon2Q == "O")
            {
                lon2 *= -1;
            }

            double RaioTerra = 6.371e3; // km
            double AnguloPhi1 = lat1 * Math.PI / 180; // φ, λ in radians
            double AnguloPhi2 = lat2 * Math.PI / 180;
            double DeltaPhi = (lat2 - lat1) * Math.PI / 180;
            double DeltaLambda = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(DeltaPhi / 2) * Math.Sin(DeltaPhi / 2) +
                      Math.Cos(AnguloPhi1) * Math.Cos(AnguloPhi2) *
                      Math.Sin(DeltaLambda / 2) * Math.Sin(DeltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RaioTerra * c; // in km
        }
        public static Tuple<bool, string, string> AvaliaModelo(int status, bool ShowMess)
        {
            string msg = "";
            string msgtit = "";
            switch (status)
            {
                case 1:
                    msg = "Modelo Carregado, sem solução disponível!";
                    msgtit = "Carregado";
                    break;
                case 2:
                    msg = "Modelo Resolvido com ótimo disponível!";
                    msgtit = "Sucesso";
                    break;
                case 3:
                    msg = "Modelo infactível";
                    msgtit = "Infactível";
                    break;
                case 4:
                    msg = "Modelo infactível ou ilimitado \n Para uma conclusão definitiva, defina o parâmetro DualReductions = 0 e reotimize.";
                    msgtit = "Infactível ou Ilimitado";
                    break;
                case 5:
                    msg = "Modelo ilimitado";
                    msgtit = "Ilimitado";
                    break;
                case 6:
                    msg = "Modelo se mostra pior que o valor especificado no Parâmtero CutOff. \n Sem Solução disponível";
                    msgtit = "Parâmtero CutOff";
                    break;
                case 7:
                    msg = "Otimização terminada por exceder o limite de iterações \n especificado no parâmetro IterationLimit.";
                    msgtit = "Limite de iterações";
                    break;
                case 8:
                    msg = "Otimização terminada por exceder o limite de nós explorados \n no processo branch-and-cut especificado no parâmetro NodeLimit.";
                    msgtit = "Limite de nós";
                    break;
                case 9:
                    msg = "Otimização terminada por exceder o limite de tempo \n especificado no parâmetro TimeLimit.";
                    msgtit = "Limite de tempo";
                    break;
                case 10:
                    msg = "Otimização terminada por exceder o limite de soluções encontradas \n especificado no parâmetro SolutionLimit.";
                    msgtit = "Limite de tempo";
                    break;
                case 11:
                    msg = "Otimização interrompida pelo usuário";
                    msgtit = "Usuário";
                    break;
                case 12:
                    msg = "Otimização interrompida por dificuldades \n numéricas irrecuperáveis";
                    msgtit = "Dificuldades Numéricas";
                    break;
                case 13:
                    msg = "Incapaz de satisfazer as tolerâncias de otimização. \n Uma solução abaixo do ideal está disponível";
                    msgtit = "Sub-ótimo";
                    break;
                case 14:
                    msg = "Chamada de Otimização assíncrona realizada, mas o processo de otimização não terminou.";
                    msgtit = "Em progresso";
                    break;
                case 15:
                    msg = "O limite especificado foi alcançado";
                    msgtit = "Objetivo alcançado";
                    break;
            }
            bool segue;
            //msgs
            switch (status)
            {
                case 1:
                case 2:
                case 15:
                    segue = true;
                    if (ShowMess)
                    {
                        MessageBox.Show(msg, msgtit, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    break;
                case 13:
                    segue = false;
                    if (ShowMess)
                    {
                        MessageBox.Show(msg, msgtit, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    break;
                default:
                    //case 3:
                    //case 4:
                    //case 5:
                    //case 6:
                    //case 7:
                    //case 8:
                    //case 9:
                    //case 10:
                    //case 11:
                    //case 12:
                    //case 14:
                    segue = false;
                    if (ShowMess)
                    {
                        MessageBox.Show(msg, msgtit, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
            }
            Tuple<bool, string, string> resp = new(segue, msg, msgtit);
            return resp;
        }
        public static List<T> Join<T>(this List<T> first, List<T> second)
        {
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }

            return first.Concat(second).ToList();
        }
        public static string TupleToString(Tuple<string, object> Tup)
        {
            StringBuilder final = new();
            final.Append(Tup.Item1);
            final.Append(":\t");
            final.AppendLine(Tup.Item2.ToString());
            return final.ToString();

        }
        public static string LabelValueToString(List<Tuple<string, object>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, object> T in Tuples)
            {
                final.Append(TupleToString(T));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, char[]>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, char[]> Tup in Tuples)
            {
                final.Append(Tup.Item1);
                final.Append(":\t");
                StringBuilder NewT2 = new();
                foreach (char c in Tup.Item2)
                {
                    NewT2.Append(c);
                }
                final.AppendLine(NewT2.ToString());
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, ulong>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, ulong> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, uint>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, uint> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, ushort>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, ushort> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, decimal>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, decimal> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, double>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, double> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, float>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, float> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, int>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, int> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, short>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, short> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, char>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, char> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, long>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, long> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, sbyte>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, sbyte> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string LabelValueToString(List<Tuple<string, bool>> Tuples)
        {
            StringBuilder final = new();
            foreach (Tuple<string, bool> Tup in Tuples)
            {
                Tuple<string, object> NewTup = new(Tup.Item1, Tup.Item2.ToString());
                final.Append(TupleToString(NewTup));
            }
            return final.ToString();
        }
        public static string ConstraintName(string label, List<(string, int)> indexes)
        {
            StringBuilder ConstName = new();
            ConstName.Append(label);
            foreach ((string, int) C in indexes)
            {
                ConstName.Append('_');
                ConstName.Append(C.Item1);
                if (C.Item2 < 10)
                {
                    ConstName.Append(0);
                }
                ConstName.Append(C.Item2);
            }
            return ConstName.ToString();
        }
        public static string ConstraintName(string label, (string, int) indexes)
        {
            StringBuilder ConstName = new();
            ConstName.Append(label);
            ConstName.Append('_');
            ConstName.Append(indexes.Item1);
            if (indexes.Item2 < 10)
            {
                ConstName.Append(0);
            }
            ConstName.Append(indexes.Item2);
            return ConstName.ToString();
        }
        public static string InQuotes(string value)
        {
            return '"' + value + '"';
        }
        public static string ObjToJson(object Instance, string Pasta, string _fileInst)
        {
            string _namefile = string.Format(@"{0}{1}_escrito.json", Pasta, _fileInst);
            using (FileStream fs = File.Create(_namefile))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                byte[] jsonUtf8Bytes = new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(Instance, options));
                fs.Write(jsonUtf8Bytes, 0, jsonUtf8Bytes.Length);
            }
            return _namefile;
        }
        public static bool AppendLineSbCapacity(StringBuilder[] sbs, string ToAppend)
        {
            bool atributed = false;
            for (int i = 0; i < sbs.Length; i++)
            {
                if (sbs[i] == null)
                {
                    sbs[i].AppendLine(ToAppend);
                    atributed = true;
                }
                if (!atributed && sbs[i].Length + ToAppend.Length < sbs[i].MaxCapacity - 10000)
                {
                    sbs[i].AppendLine(ToAppend);
                    atributed = true;
                }
            }

            if (!atributed)
            {
                MessageBox.Show(string.Format("ToAppend: {0} \n sbs cheias", ToAppend));
                throw new OutOfMemoryException("StringBilders over capacity");
            }
            else
            {
                return atributed;
            }
        }
        public static string GroupingListIntToString(List<int> ListObj)
        {
            StringBuilder StringList = new();
            StringList.Append('(');
            foreach (int L in ListObj)
            {
                StringList.Append(L);
                if (!(L == ListObj.Last()))
                {
                    StringList.Append(',');
                }
                else
                {
                    StringList.Append(')');
                }
            }
            return StringList.ToString();
        }
        public static bool All(List<bool> L)
        {
            //true if all item are true, false if one item are false
            bool val = true;
            foreach (bool l in L)
            {
                val &= l;
            }
            return val;
        }
        public static bool Any(List<bool> L)
        {
            //true if one item are true, false if all item are false
            bool val = false;
            foreach (bool l in L)
            {
                val |= l;
            }
            return val;
        }
        public static string GetNow()
        {
            //Gera data e hora atual para o nome do arquivo
            DateTime localDate = DateTime.Now;
            var cultureName = "en-US";
            CultureInfo culture = new(cultureName);
            return localDate.ToString(culture).Replace("/", "_").Replace(" ", "_").Replace(":", "-");
        }
        public static string AddLogLine(string val)
        {
            return GetNow() + "\t" + val + "\n";
        }
        public static string AddLogLine((string, string) val)
        {
            return GetNow() + "\t" + val.Item1 + "\t" + val.Item2 + "\n";
        }
        public static string AddLogLine(List<string> val)
        {
            string resp = "";
            foreach (string s in val)
            {
                resp += "\t" + s;
            }
            return GetNow() + resp + "\n";
        }
        public static string AddLogLine(string[] val)
        {
            string resp = "";
            foreach (string s in val)
            {
                resp += "\t" + s;
            }
            return GetNow() + resp + "\n";
        }
        public static string WriteTable(double[,] table)
        {
            StringBuilder rows = new();
            rows.Append('\n');
            for (int r = 0; r < table.GetLength(0); r++)
            {
                StringBuilder cols = new();
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    cols.Append(table[r, c]);
                    if (c + 1 < table.GetLength(1))
                    {
                        cols.Append('\t');
                    }
                }
                rows.AppendLine(cols.ToString());
            }
            return rows.ToString();
        }
        public static string WriteTable(double[,,] table)
        {
            StringBuilder rows = new();
            rows.Append('\n');
            for (int r = 0; r < table.GetLength(0); r++)
            {
                rows.Append(r);
                double[,] Aux = new double[table.GetLength(1), table.GetLength(2)];
                for (int c1 = 0; c1 < table.GetLength(1); c1++)
                {
                    for (int c2 = 0; c2 < table.GetLength(2); c2++)
                    {
                        Aux[c1, c2] = table[r, c1, c2];
                    }
                }
                rows.AppendLine(WriteTable(Aux));
            }
            return rows.ToString();
        }
        public static string WriteTable(decimal[,] table)
        {
            StringBuilder rows = new();
            for (int r = 0; r < table.GetLength(0); r++)
            {
                StringBuilder cols = new();
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    cols.Append(table[r, c]);
                    if (c + 1 < table.GetLength(1))
                    {
                        cols.Append('\t');
                    }
                }
                rows.AppendLine(cols.ToString());
            }
            return rows.ToString();
        }
        public static string WriteTable(bool[,] table)
        {
            StringBuilder rows = new();
            for (int r = 0; r < table.GetLength(0); r++)
            {
                StringBuilder cols = new();
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    cols.Append(table[r, c]);
                    if (c + 1 < table.GetLength(1))
                    {
                        cols.Append('\t');
                    }
                }
                rows.AppendLine(cols.ToString());
            }
            return rows.ToString();
        }
        public static string WriteTable(int[,] table)
        {
            StringBuilder rows = new();
            for (int r = 0; r < table.GetLength(0); r++)
            {
                StringBuilder cols = new();
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    cols.Append(table[r, c]);
                    if (c + 1 < table.GetLength(1))
                    {
                        cols.Append('\t');
                    }
                }
                rows.AppendLine(cols.ToString());
            }
            return rows.ToString();
        }
        public static bool[,] LessThan(int[,] table, int crit)
        {
            bool[,] resp = new bool[table.GetLength(0), table.GetLength(1)];

            for (int r = 0; r < table.GetLength(0); r++)
            {
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    resp[r, c] = table[r, c] < crit;
                }
            }
            return resp;
        }
        public static bool[,] LessEqThan(int[,] table, int crit)
        {
            bool[,] resp = new bool[table.GetLength(0), table.GetLength(1)];
            for (int r = 0; r < table.GetLength(0); r++)
            {
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    resp[r, c] = table[r, c] <= crit;
                }
            }
            return resp;
        }
        public static bool[,] LessThan(double[,] table, double crit)
        {
            bool[,] resp = new bool[table.GetLength(0), table.GetLength(1)];

            for (int r = 0; r < table.GetLength(0); r++)
            {
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    resp[r, c] = table[r, c] < crit;
                }
            }
            return resp;
        }
        public static bool[,] LessEqThan(double[,] table, double crit)
        {
            bool[,] resp = new bool[table.GetLength(0), table.GetLength(1)];
            for (int r = 0; r < table.GetLength(0); r++)
            {
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    resp[r, c] = table[r, c] <= crit;
                }
            }
            return resp;
        }
        public static double ReadDouble(string str)
        {
            if (str.Length > 0)
            {
                if (str.Contains('.'))
                {
                    return Convert.ToDouble(str.Replace('.', ','));
                }
                else
                {
                    return Convert.ToDouble(str);
                }
            }
            else
            {
                return Convert.ToDouble(0);
            }

        }
        public static decimal ReadDecimal(string str)
        {
            if (str.Length > 0)
            {
                if (str.Contains('.'))
                {
                    return Convert.ToDecimal(str.Replace('.', ','));
                }
                else
                {
                    return Convert.ToDecimal(str);
                }
            }
            else
            {
                return Convert.ToDecimal(0);
            }

        }
        public static (double, double) GetMinMaxCoordPoints(List<(double, double)> pts, bool min = true)
        {//min true returns min; min false return max

            (double, double) resp;
            if (min)
            {
                resp = (double.MaxValue, double.MaxValue);
                foreach ((double, double) pt in pts)
                {
                    if (pt.Item1 < resp.Item1)
                    {
                        resp.Item1 = pt.Item1;
                    }
                    if (pt.Item2 < resp.Item2)
                    {
                        resp.Item2 = pt.Item2;
                    }
                }
            }
            else
            {
                resp = (double.MinValue, double.MinValue);

                foreach ((double, double) pt in pts)
                {
                    if (pt.Item1 > resp.Item1)
                    {
                        resp.Item1 = pt.Item1;
                    }
                    if (pt.Item2 > resp.Item2)
                    {
                        resp.Item2 = pt.Item2;
                    }
                }
            }
            return resp;
        }
        public static List<(double, double)> NormalizePoints(List<(double, double)> pts, (double, double) newMax, (double, double) newMin)
        {
            (double, double) minatual = GetMinMaxCoordPoints(pts);
            (double, double) maxatual = GetMinMaxCoordPoints(pts, false);

            (double, double) a = (0, 0);
            (double, double) b = (0, 0);
            if (maxatual.Item1 != minatual.Item1)
            {
                a.Item1 = (newMax.Item1 - newMin.Item1) / (maxatual.Item1 - minatual.Item1);
                b.Item1 = newMax.Item1 - a.Item1 * maxatual.Item1;
            }
            if (maxatual.Item2 != minatual.Item2)
            {
                a.Item2 = (newMax.Item2 - newMin.Item2) / (maxatual.Item2 - minatual.Item2);
                b.Item2 = newMax.Item2 - a.Item2 * maxatual.Item2;
            }
            List<(double, double)> resp = new();
            foreach ((double, double) pt in pts)
            {
                resp.Add((a.Item1 * pt.Item1 + b.Item1, a.Item2 * pt.Item2 + b.Item2));
            }
            return resp;
        }
        public static Bitmap DrawSquareGuide((double, double) mins, (double, double) maxs, int width = 500, int height = 500, int R = 0, int G = 0, int B = 0)
        {
            List<(double, double)> pts_orig = new() { mins, maxs };
            List<(double, double)> pts_norm = NormalizePoints(pts_orig, (width - 25, height - 25), (25, 25));

            Color ColorLine = Color.FromArgb(R, G, B);
            Bitmap Desenho = new(width, height);
            Graphics g = Graphics.FromImage(Desenho);
            g.FillRectangle(Brushes.Transparent, 0, 0, width, height);
            SolidBrush drawLineBrush = new(ColorLine); //Pincel de Desenho
            Pen drawPen = new(drawLineBrush); //Caneta para desenhar linha
            _ = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Center //Uso o formato para alinhar txt no centro
            }; //Formato de String
            List<(double, double, double, double)> retas = new()
            {
                (0, pts_norm[0].Item2, width, pts_norm[0].Item2),
                (0, pts_norm[1].Item2, width, pts_norm[1].Item2),
                (pts_norm[0].Item1, 0, pts_norm[0].Item1, height),
                (pts_norm[1].Item1, 0, pts_norm[1].Item1, height)
            };

            foreach ((double, double, double, double) coord in retas)
            {
                float X1 = Convert.ToSingle(coord.Item1);
                float Y1 = Convert.ToSingle(coord.Item2);
                float X2 = Convert.ToSingle(coord.Item3);
                float Y2 = Convert.ToSingle(coord.Item4);

                PointF P1 = new(X1, Y1);
                PointF P2 = new(X2, Y2);
                g.DrawLine(drawPen, P1, P2);
            }
            return Desenho;
        }
        public static Bitmap DrawListPoints(List<(double, double)> pts, List<int> inds, int width = 500, int height = 500, int R = 0, int G = 0, int B = 0, float PointRadius = 5, bool drawlines = true, int fontsize = 7, bool transparent = false)
        {
            if (inds.Count < 1 || inds.Count != pts.Count)
            {
                inds.Clear();
                for (int i = 0; i < pts.Count; i++)
                {
                    inds.Add(i);
                }
            }
            Color ColorLine = Color.FromArgb(R, G, B);
            Bitmap Desenho = new(width, height);
            Graphics g = Graphics.FromImage(Desenho);
            if (!transparent)
            {
                g.FillRectangle(Brushes.White, 0, 0, width, height);
            }
            Font drawFont = new("Arial", fontsize); //Escrita de texto no Desenho
            SolidBrush drawBrush = new(Color.Black); //Pincel de Desenho
            SolidBrush drawLineBrush = new(ColorLine); //Pincel de Desenho
            Pen drawPen = new(drawLineBrush); //Caneta para desenhar linha
            _ = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Center //Uso o formato para alinhar txt no centro
            }; //Formato de String

            //float PointRadius = 5; //Tamanho dos Vertices

            //Desenho de Pontos e Arestas
            for (int i = 0; i < pts.Count; i++)
            {
                float X = Convert.ToSingle(pts[i].Item1) - PointRadius;
                float Y = Convert.ToSingle(pts[i].Item2) - PointRadius;
                PointF Top = new(X, Y);
                SizeF Tam = new(2 * PointRadius, 2 * PointRadius);
                RectangleF Rec = new(Top, Tam);
                g.DrawEllipse(drawPen, Rec);

                PointF Cnt = new(X + PointRadius, Y + PointRadius);
                g.DrawString(inds[i].ToString(), drawFont, drawBrush, Cnt);

                if (drawlines)
                {
                    //Draw Line
                    int j = 0;

                    if (i < pts.Count - 1)
                    {
                        j = i + 1;
                    }

                    float X1 = Convert.ToSingle(pts[i].Item1);
                    float Y1 = Convert.ToSingle(pts[i].Item2);
                    float X2 = Convert.ToSingle(pts[j].Item1);
                    float Y2 = Convert.ToSingle(pts[j].Item2);

                    PointF P1 = new(X1, Y1);
                    PointF P2 = new(X2, Y2);
                    PointF PMed = new((X1 + X2) * 50 / 100, (Y1 + Y2) * 50 / 100);

                    g.DrawLine(drawPen, P1, P2);
                    g.DrawString(inds[i].ToString() + "->" + inds[j].ToString(), drawFont, drawLineBrush, PMed);
                }
            }

            return Desenho;
        }
        public static Bitmap Draw2points(int qtdPts, bool random = true, int randomseed = 0)
        {
            List<(double, double)> pts;
            List<int> ind = new();
            double XCent = 250;
            double YCent = 250;
            double gdeRaio = 250;

            if (random)
            {
                pts = CriarListaDePontos(qtdPts, 10, 490, 10, 490, randomseed);
                for (int i = 0; i < qtdPts; i++)
                {
                    ind.Add(i);
                }
            }
            else
            {
                (pts, ind) = CriarPontosPoligonoRegular(qtdPts, gdeRaio, XCent, YCent);
            }

            return DrawListPoints(pts, ind);
        }
        public static void Substring_lps(string filePL, string fileMST, bool ShowMess)
        {
            string[] FileMSTdata = File.ReadAllLines(fileMST);
            List<(string, string)> MSTData = new();
            for (int i = 1; i < FileMSTdata.Length; i++)
            {
                (string, string) newtuple = (FileMSTdata[i].Split(' ')[0], "* " + FileMSTdata[i].Split(' ')[1]);
                Console.WriteLine(newtuple);
                MSTData.Add(newtuple);
            }
            MSTData.Add((" = ", "\t=\t"));
            MSTData.Add((" <= ", "\t<=\t"));
            MSTData.Add((" >= ", "\t>=\t"));
            MSTData.Add((".", ","));
            MSTData.Add((":", ":\t"));
            MSTData.Add((" + * ", " + "));
            MSTData.Add(("\nMinimize ", "\nMin\t"));
            MSTData.Add(("\nMaximize ", "\nMax\t"));
            MSTData.Add(("\n   ", " "));

            string[] FilePLdata = File.ReadAllLines(filePL);
            string NewName = filePL.Replace(".lp", "recalc(" + GetNow() + ").tsv");
            string val;
            Console.WriteLine(NewName);
            using (StreamWriter writer = new(NewName))
            {
                for (int i = 0; i < FilePLdata.Length; i++)
                {
                    Console.WriteLine("Linha " + (i + 1).ToString() + ".\n" + FilePLdata[i] + "\nFaltam " + (FilePLdata.Length - i - 1).ToString());
                    if (FilePLdata[i].IndexOf('\\') == 0)
                    {
                        writer.WriteLine(FilePLdata[i]);
                    }
                    else
                    {
                        val = FilePLdata[i];
                        foreach ((string, string) dat in MSTData)
                        {
                            val = val.Replace(dat.Item1, dat.Item2);
                        }
                        writer.Write(val);
                        Console.WriteLine("res: " + val + '\n');
                    }
                }
            }
            if (ShowMess)
            {
                _ = MessageBox.Show(NewName + "\nGravado", "Grava Modelo");
            }
        }
        public static string[] SplitFileLine(string line, char[] charSeparators)
        {
            return line.TrimStart().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
        }
        public static int TrucksByFMU(double TruckCapacity, double volume)
        {
            int resp = 0;
            if (volume > 0)
            {
                double quoc = (volume / TruckCapacity);
                resp = (int)Math.Floor(quoc);
                if (resp * TruckCapacity < volume)
                {
                    resp += 1;
                }
            }
            return resp;
        }
        public static int TrucksByFMU(int TruckCapacity, double volume)
        {
            return TrucksByFMU((double)TruckCapacity, volume);
        }
        public static int TrucksByFMU(int TruckCapacity, int volume)
        {
            return TrucksByFMU((double)TruckCapacity, (double)volume);
        }
        public static int TrucksByFMU(double TruckCapacity, int volume)
        {
            return TrucksByFMU(TruckCapacity, (double)volume);
        }
        public static List<int> ReservoirSample(List<int> Source, int SampleSize)
        {
            //(*Source has items to sample, Resp will contain the result *)
            Random random = new(0);
            if (Source.Count > SampleSize)
            {
                List<int> Resp = new(); // the reservoir array
                // fill the reservoir array
                foreach (int i in Enumerable.Range(0, SampleSize))
                {
                    Resp.Add(Source[i]);
                }
                //(*random() generates a uniform(0, 1) random number *)
                double W = Math.Exp(Math.Log(random.NextDouble()) / SampleSize);

                int j = SampleSize;
                while (j < Source.Count)
                {
                    j += (int)(Math.Floor(Math.Log(random.NextDouble()) / Math.Log(1 - W))) + 1;
                    if(j < Source.Count)
                    {
                        int i = random.Next(SampleSize); // random index between 0 and k, exclusive
                        Resp[i] = Source[j];
                        W *= Math.Exp(Math.Log(random.NextDouble()) / SampleSize);
                    }
                }
                return Resp;
            } else
            {
                return Source;
            }
        }
         
    }
}
