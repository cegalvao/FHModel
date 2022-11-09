using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHModel
{
    class ModelParameters
    {
        public Instance DataInst;
        public string fileparam;
        public string? mst_file;
        public char[,,]? XVar;
        public char[,]? AlphaVar;
        public bool[]? UseConstraints;
        public double[] RHS;
        public bool ModelSenseMax;
        public bool BinaryVars;
        public bool SlackExcess;
        public bool[]? SlackExcessVars;
        public bool OldVersion;
        public bool Hybrid;
        public bool Uselog;
        public bool Uselazy;
        public bool ControlConst;
        public bool UseSoltest;
        public bool ShowMessages;

        public ModelParameters(Instance data, bool[] useConstraints, bool showmess, bool[] slackExcessVars, 
                               string mst = "", char[,,]? xVar = null, char[,]? alphaVar = null, 
                               bool showMess = true, bool modelSenseMax = true, bool binaryVars = true, 
                               bool slackExcess = true, bool oldVersion = false, bool hybrid = true, 
                               bool uselog = false,  bool uselazy = false, bool controlConst = false,
                               bool Usesoltest = true)
        {
            DataInst = data;
            DataInst.Agora = ToolBox.GetNow();
            string Pasta = DataInst.Std.PathInstance;
            string Agora = DataInst.Agora;
            fileparam = $"{Pasta}_Param_{Agora}.txt";
            ShowMessages = showmess;
            XVar = xVar;
            AlphaVar = alphaVar;
            UseConstraints = useConstraints;
            ModelSenseMax = modelSenseMax;
            BinaryVars = binaryVars;
            SlackExcess = slackExcess;
            SlackExcessVars = slackExcessVars;
            OldVersion = oldVersion;
            Hybrid = hybrid;
            Uselog = uselog;
            Uselazy = uselazy;
            ControlConst = controlConst;
            UseSoltest = Usesoltest;
            ShowMessages = showMess;
            mst_file = mst;
            RHS = new double[useConstraints.Length];
            for (int i = 0; i < useConstraints.Length; i++)
            {
                RHS[i] = 0;
            }
        }

        public override string ToString()
        {
            StringBuilder Par = new StringBuilder();
            Par.AppendLine($"Pasta: {DataInst.Std.PathInstance}");
            Par.AppendLine("UseConstraints: ");
            for (int i = 0; i < UseConstraints.Length; i++)
            {
                if (UseConstraints[i])
                {
                    Par.AppendLine($"\tR{ToolBox.Constants.Constraints[i]}");
                }
            }
            Par.AppendLine($"\nAgora: {DataInst.Agora}");
            Par.AppendLine($"Name: {DataInst.Std.NameInstance}");
            Par.AppendLine($"ShowMess: {ShowMessages}");
            Par.AppendLine($"SlackExcess: {SlackExcess}");
            Par.AppendLine("SlackExcessVars: ");
            for (int i = 0; i < SlackExcessVars.Length; i++)
            {
                if (SlackExcessVars[i])
                {
                    Par.AppendLine($"\tR{ToolBox.Constants.Constraints[i]}");
                }
            }
            Par.AppendLine($"\nHybrid: {Hybrid}");
            Par.AppendLine($"Uselog: {Uselog}");
            Par.AppendLine($"Uselazy: {Uselazy}");
            Par.AppendLine($"ControlConst: {ControlConst}");
            Par.AppendLine($"Usesoltest: {UseSoltest}");
            Par.AppendLine($"Início: {ToolBox.GetNow()}");
            Par.AppendLine($"Dados da Instancia:\n {DataInst.ToString()}");
            return Par.ToString();
        }
    }
}
