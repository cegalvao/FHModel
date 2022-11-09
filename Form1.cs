using System.Linq.Expressions;
using System.Text;


namespace FHModel
{
    public partial class Form1 : Form
    {
        public string NameInst = "Form1";
        public int NumConstraints = ToolBox.Constants.Constraints.Length;

        public bool[] ListConstraints = new bool[21];//NumConstraints];
        public bool[] ListSlcExc = new bool[21];//NumConstraints];
        public bool ControlConst = false;
        public bool ControlSlcExc = false;
        public List<int> GoodConstraints = new() { 0, 1, 2, 3, 4, 6, 8, 9, 12, 13, 17 };
        public List<int> SlcExcConstraints_04 = new() { 10, 14};
        public List<int> SlcExcConstraints_06 = new() { 15, 17, 19, 20};
        public List<int> SlcExcConstraints_10 = new() { 10, 14 };
        public List<int> SlcExcConstraints_21 = new() { 10, 14 };
        public string unity = "C";
        public string address = ToolBox.Constants.PathBase;
        public bool SM = false;

        private FHStandards Kitt04;
        private FHStandards Kitt06;
        private FHStandards Kitt10;
        private FHStandards Kitt21;
        private Instance K04;
        private Instance K06;
        private Instance K10;
        private Instance K21;

        public Form1()
        {
            InitializeComponent();

            Kitt04 = new("Kitt04", FHData.Adj_04, FHData.FMU_04, FHData.AreaAge_04,
                         FHData.Vol_04, FHData.Edges_04, FHData.Prf_04, FHData.Par_04);
            Kitt06 = new("Kitt06", FHData.Adj_06, FHData.FMU_06, FHData.AreaAge_06,
                         FHData.Vol_06, FHData.Edges_06, FHData.Prf_06, FHData.Par_06);
            Kitt10 = new("Kitt10", FHData.Adj_10, FHData.FMU_10, FHData.AreaAge_10,
                          FHData.Vol_10, FHData.Edges_10, FHData.Prf_10, FHData.Par_10);
            Kitt21 = new("Kitt21", FHData.Adj_21, FHData.FMU_21, FHData.AreaAge_21,
                         FHData.Vol_21, FHData.Edges_21, FHData.Prf_21, FHData.Par_21);
            K04 = new Instance(Kitt04);
            K06 = new Instance(Kitt06);
            K10 = new Instance(Kitt10);
            K21 = new Instance(Kitt21);
            K04.SaveInstance();
            K06.SaveInstance();
            K10.SaveInstance();
            K21.SaveInstance();

            chk_ControlConst.CheckState = CheckState.Unchecked;
            clb_Constraints.Enabled = false;
            clb_SlcExc.Enabled = false;
            btn_AllSE.Enabled = false;
            ControlConst = false;
            ControlSlcExc = false;
            cmB_Unity.SelectedItem = "C";
            this.lbl_Status.Text = "Pronto";
        }
        private void BtnRInstance_Click(object sender, EventArgs e)
        {
            RInstance(true);
        }
        private void RInstance(bool Hyb)
        {
            Color OriginalColor = this.BackColor;
            this.BackColor = Color.FromArgb(64, 127, 192);
            string tit;
            if (Hyb)
            {
                tit = "R_Instance_Hyb";
            }
            else
            {
                tit = "R_Instance_NonHyb";
            }

            this.lbl_Status.Text = tit;

            CheckConstraints();
            CheckSlkExc();
            ModelParameters parameters = SetParameters(Hyb);
            string Pasta = parameters.DataInst.Std.PathInstance;
            string Agora = parameters.DataInst.Agora;
            ToolBox.ifMess(parameters.ShowMessages, Pasta, "Read from");
            WriteParameters(parameters);
            _ = new ModelRotaFH(parameters);
            (string, string) Resolvido = ($"Resolvido {Agora}", $"{tit} {Agora}");            
            ToolBox.ifMess(parameters.ShowMessages, Resolvido.Item1, Resolvido.Item2, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);            
            //Application.Exit();
            //_ = MessageBox.Show("Fechar");
            this.lbl_Status.Text = "Pronto";
            this.BackColor = OriginalColor;
        }
        public void CheckConstraints()
        {
            for (int i = 0; i < clb_Constraints.Items.Count; i++)
            {
                ListConstraints[i] = clb_Constraints.GetItemCheckState(i) == CheckState.Checked;
            }
            ControlConst = (chk_ControlConst.CheckState == CheckState.Checked);
        }
        public void CheckSlkExc()
        {
            for (int i = 0; i < clb_SlcExc.Items.Count; i++)
            {
                ListSlcExc[i] = clb_SlcExc.GetItemCheckState(i) == CheckState.Checked;
            }
            ControlSlcExc = (chk_SlackExcess.CheckState == CheckState.Checked);
        }

        private Instance SelectData(string val)
        {
            switch (val)
            {
                case "Kitt10":
                    return K10;
                case "Kitt21":
                    return K21;
                case "Kitt06":
                    return K06;
                default:
                    return K04;
            }
        }

        private ModelParameters SetParameters(bool Hyb = true)
        {
            return new ModelParameters( 
                data: SelectData((string)cmb_Instance.SelectedItem),
                useConstraints: ListConstraints,
                showmess: SM,
                showMess: chk_ShowMess.Checked,
                modelSenseMax: true,
                slackExcess: chk_SlackExcess.Checked,
                slackExcessVars: ListSlcExc,
                hybrid: Hyb,
                uselog: chk_log.Checked,
                uselazy: chk_Lazys.Checked,
                controlConst: ControlConst,
                Usesoltest: chk_SolTest.Checked); 
            //{
            //    XVar,
            //    AlphaVar,
            //    ModelSenseMax,
            //    BinaryVars,
            //    OldVersion,
            //};
        }
        private void WriteParameters(ModelParameters parameters)
        {
            this.lbl_Status.Text = $"Write {parameters.fileparam}";
            ToolBox.ifMess(parameters.ShowMessages, parameters.fileparam);             
            ToolBox.FileTxt(parameters.fileparam, parameters.ToString(), false);
        }
        public CheckState BoolToCheck(bool value)
        {
            return value ? CheckState.Checked : CheckState.Unchecked;
        }
        public bool CheckToBool(CheckState Check)
        {
            return (Check == CheckState.Checked);
        }
        public void StressTest(object sender, EventArgs e)
        {
            this.lbl_Status.Text = "Stress Test";
            List<List<(int, bool)>> UseSLC = new List<List<(int, bool)>> {
                new List<(int, bool)> {(0, false)},
                new List<(int, bool)> {(0, true)},
                new List<(int, bool)> {(0, true), (1, false)},
                new List<(int, bool)> {(0, true), (1, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, true), (18, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, true), (18, true), (19, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, true), (18, true), (19, true)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, true), (18, true), (19, true), 
                                       (20, false)},
                new List<(int, bool)> {(0, true), (1, true), (2, true), (3, true),
                                       (4, true), (5, true), (6, true), (7, true),
                                       (8, true), (9, true), (10, true), (11, true),
                                       (12, true), (13, true), (14, true), (15, true),
                                       (16, true), (17, true), (18, true), (19, true), 
                                       (20, true)}
                };

            foreach (List<(int, bool)> L in UseSLC)
            {
                ClearConstraints();
                this.Text = $"Stress Test: Contrs {L.Count - 1} - Last SE {L.Last().Item2.ToString()}";
                foreach ((int, bool) T in L)
                {
                    clb_Constraints.SetItemCheckState(T.Item1, BoolToCheck(true));
                    clb_SlcExc.SetItemCheckState(T.Item1, BoolToCheck(T.Item2));
                }
                BtnRInstance_Click(sender, e);
            }
            ToolBox.ifMess(CheckToBool(this.chk_ShowMess.CheckState),"Fechar StressTest");
            this.lbl_Status.Text = "Pronto";
        }
        public void ClearConstraints()
        {
            for (int i = 0; i < clb_Constraints.Items.Count; i++)
            {
                clb_Constraints.SetItemCheckState(i, BoolToCheck(false));
                clb_SlcExc.SetItemCheckState(i, BoolToCheck(false));
            }
        }
        private void btnRInstance_Fase_Click(object sender, EventArgs e)
        {
            RInstance(false);
        }
        public void cmb_Instance_SelectedIndexChanged(object sender, EventArgs e)
        {
            NameInst = cmb_Instance.SelectedItem.ToString();
            chk_SlackExcess_CheckStateChanged(sender, e);
        }
        private void btn_MarkDesmark_Click(object sender, EventArgs e)
        {
            bool CheckThem = clb_Constraints.CheckedItems.Count == 0;

            for (int i = 0; i <= (clb_Constraints.Items.Count - 1); i++)
            {
                clb_Constraints.SetItemCheckState(i, BoolToCheck(CheckThem));
            }
        }
        public void chk_ControlConst_CheckStateChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= (clb_Constraints.Items.Count - 1); i++)
            {
                clb_Constraints.SetItemCheckState(i, BoolToCheck(chk_ControlConst.Checked));
                clb_Constraints.Enabled = chk_ControlConst.Checked;
            }
        }
        public void chk_SlackExcess_CheckStateChanged(object sender, EventArgs e)
        {
            List<int> SlcExcConstraints = new List<int>();
            for (int i = 0; i < clb_SlcExc.Items.Count; i++)
            {
                clb_SlcExc.SetItemCheckState(i, CheckState.Unchecked);
            }
            switch (cmb_Instance.SelectedItem.ToString())
            {
                case "Kitt04":
                    SlcExcConstraints = SlcExcConstraints_04;
                    break;
                case "Kitt06":
                    SlcExcConstraints = SlcExcConstraints_06;
                    break;
                case "Kitt10":
                    SlcExcConstraints = SlcExcConstraints_10;
                    break;
                case "Kitt21":
                    SlcExcConstraints = SlcExcConstraints_21;
                    break;
                default:
                    break;
            }

            if (chk_SlackExcess.Checked)
            {
                for (int i = 0; i <= (clb_SlcExc.Items.Count - 1); i++)
                {
                    bool val = (SlcExcConstraints.IndexOf(i) > -1) && (clb_Constraints.GetItemCheckState(i) == CheckState.Checked);
                    clb_SlcExc.SetItemCheckState(i, BoolToCheck(val));
                }
            }
            else
            {
                for (int i = 0; i <= (clb_SlcExc.Items.Count - 1); i++)
                {
                    clb_SlcExc.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
            clb_SlcExc.Enabled = chk_SlackExcess.Checked;
            btn_AllSE.Enabled = chk_SlackExcess.Checked;
        }
        private void chk_Good_CheckedChanged(object sender, EventArgs e)
        {
            cmb_Instance.SelectedItem = cmb_Instance.Items[1];
            cmb_Instance_SelectedIndexChanged(sender, e);

            chk_ControlConst.CheckState = chk_Good.CheckState;
            chk_ControlConst_CheckStateChanged(sender, e);

            for (int i = 0; i < clb_Constraints.Items.Count; i++)
            {
                CheckState C = GoodConstraints.Contains(i) 
                    ? chk_Good.CheckState 
                    : CheckState.Unchecked;
                clb_Constraints.SetItemCheckState(i, C);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cmb_Instance.SelectedItem = "Kitt06";
            chk_Lazys.CheckState = CheckState.Unchecked;
            chk_SolTest.CheckState = CheckState.Unchecked;

            chk_ControlConst.CheckState = CheckState.Checked;
            chk_ControlConst_CheckStateChanged(sender, e);

            chk_SlackExcess.CheckState = CheckState.Checked;
            chk_SlackExcess_CheckStateChanged(sender, e);
            //StressTest(sender, e);

        }
        private void cmB_Unity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmB_Unity.SelectedItem.ToString() == "C")
            {
                this.unity = "C";
                this.Text = ToolBox.Constants.PathBase;
                this.address = ToolBox.Constants.PathBase;
            }
            else
            {
                this.unity = "D";
                //this.Text = ToolBox.Constants.PathBaseD;
                //this.address = ToolBox.Constants.PathBaseD;
            }
        }
        private void btn_1by1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clb_Constraints.Items.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    ClearConstraints();
                    bool SE = j > 0;
                    this.Text = $"testes - i {i}; SE {SE}";
                    this.lbl_Status.Text = $"testes - i {i}; SE {SE}";
                    clb_Constraints.SetItemCheckState(i, BoolToCheck(true));
                    chk_SlackExcess.CheckState = BoolToCheck(SE);
                    clb_SlcExc.SetItemCheckState(i, BoolToCheck(SE));
                    BtnRInstance_Click(sender, e);
                }
                ToolBox.ifMess(CheckToBool(this.chk_ShowMess.CheckState),$"Restr {i}");
            }
            this.Text = "Form1";
        }

        private void btn_AllSE_Click(object sender, EventArgs e)
        {
            CheckState C = BoolToCheck(clb_SlcExc.CheckedItems.Count == 0);
            for (int i = 0; i <= (clb_SlcExc.Items.Count - 1); i++)
            {
                clb_SlcExc.SetItemCheckState(i, C);
            }
        }
    }
}
