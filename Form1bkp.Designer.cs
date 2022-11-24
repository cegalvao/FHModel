
namespace FHModel
{
    partial class Form1bkp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSolve_Instance = new System.Windows.Forms.Button();
            this.btnSolve_Sched = new System.Windows.Forms.Button();
            this.chk_SlackExcess = new System.Windows.Forms.CheckBox();
            this.chk_log = new System.Windows.Forms.CheckBox();
            this.chk_ShowMess = new System.Windows.Forms.CheckBox();
            this.cmb_Instance = new System.Windows.Forms.ComboBox();
            this.lbl01 = new System.Windows.Forms.Label();
            this.clb_Constraints = new System.Windows.Forms.CheckedListBox();
            this.btn_MarkDesmark = new System.Windows.Forms.Button();
            this.chk_ControlConst = new System.Windows.Forms.CheckBox();
            this.chk_SolTest = new System.Windows.Forms.CheckBox();
            this.chk_Good = new System.Windows.Forms.CheckBox();
            this.chk_Lazys = new System.Windows.Forms.CheckBox();
            this.clb_SlcExc = new System.Windows.Forms.CheckedListBox();
            this.lbl_clb_Constraints = new System.Windows.Forms.Label();
            this.lbl_clb_SlcExc = new System.Windows.Forms.Label();
            this.cmB_Unity = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_StressTest = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.btn_1by1 = new System.Windows.Forms.Button();
            this.btn_AllSE = new System.Windows.Forms.Button();
            this.btnSolve_Flow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSolve_Instance
            // 
            this.btnSolve_Instance.Location = new System.Drawing.Point(12, 324);
            this.btnSolve_Instance.Name = "btnSolve_Instance";
            this.btnSolve_Instance.Size = new System.Drawing.Size(100, 40);
            this.btnSolve_Instance.TabIndex = 108;
            this.btnSolve_Instance.Text = "Solve Hybrid";
            this.btnSolve_Instance.UseVisualStyleBackColor = true;
            this.btnSolve_Instance.Click += new System.EventHandler(this.BtnRInstance_Click);
            // 
            // btnSolve_Sched
            // 
            this.btnSolve_Sched.Location = new System.Drawing.Point(12, 364);
            this.btnSolve_Sched.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSolve_Sched.Name = "btnSolve_Sched";
            this.btnSolve_Sched.Size = new System.Drawing.Size(100, 40);
            this.btnSolve_Sched.TabIndex = 100;
            this.btnSolve_Sched.Text = "Solve Only Scheduling";
            this.btnSolve_Sched.UseVisualStyleBackColor = true;
            this.btnSolve_Sched.Click += new System.EventHandler(this.BtnRInstance_Fase_Click);
            // 
            // chk_SlackExcess
            // 
            this.chk_SlackExcess.AutoSize = true;
            this.chk_SlackExcess.Location = new System.Drawing.Point(13, 90);
            this.chk_SlackExcess.Name = "chk_SlackExcess";
            this.chk_SlackExcess.Size = new System.Drawing.Size(90, 19);
            this.chk_SlackExcess.TabIndex = 101;
            this.chk_SlackExcess.Text = "Slack Excess";
            this.chk_SlackExcess.UseVisualStyleBackColor = true;
            // 
            // chk_log
            // 
            this.chk_log.AutoSize = true;
            this.chk_log.Location = new System.Drawing.Point(13, 116);
            this.chk_log.Name = "chk_log";
            this.chk_log.Size = new System.Drawing.Size(74, 19);
            this.chk_log.TabIndex = 102;
            this.chk_log.Text = "Only Log";
            this.chk_log.UseVisualStyleBackColor = true;
            // 
            // chk_ShowMess
            // 
            this.chk_ShowMess.AutoSize = true;
            this.chk_ShowMess.Location = new System.Drawing.Point(13, 65);
            this.chk_ShowMess.Name = "chk_ShowMess";
            this.chk_ShowMess.Size = new System.Drawing.Size(82, 19);
            this.chk_ShowMess.TabIndex = 103;
            this.chk_ShowMess.Text = "ShowMess";
            this.chk_ShowMess.UseVisualStyleBackColor = true;
            // 
            // cmb_Instance
            // 
            this.cmb_Instance.FormattingEnabled = true;
            this.cmb_Instance.Items.AddRange(new object[] {
            "Kitt04",
            "Kitt06",
            "Kitt10",
            "Kitt21",
            "PL"});
            this.cmb_Instance.Location = new System.Drawing.Point(13, 30);
            this.cmb_Instance.Name = "cmb_Instance";
            this.cmb_Instance.Size = new System.Drawing.Size(106, 23);
            this.cmb_Instance.TabIndex = 104;
            this.cmb_Instance.SelectedIndexChanged += new System.EventHandler(this.Cmb_Instance_SelectedIndexChanged);
            // 
            // lbl_cmb_Instance
            // 
            this.lbl01.AutoSize = true;
            this.lbl01.Location = new System.Drawing.Point(13, 9);
            this.lbl01.Name = "lbl_cmb_Instance";
            this.lbl01.Size = new System.Drawing.Size(51, 15);
            this.lbl01.TabIndex = 105;
            this.lbl01.Text = "Instance";
            // 
            // clb_Constraints
            // 
            this.clb_Constraints.FormattingEnabled = true;
            this.clb_Constraints.Items.AddRange(new object[] {
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
            "20_B_AgC_nearconsec"});
            this.clb_Constraints.Location = new System.Drawing.Point(137, 87);
            this.clb_Constraints.Name = "clb_Constraints";
            this.clb_Constraints.Size = new System.Drawing.Size(162, 382);
            this.clb_Constraints.TabIndex = 109;
            // 
            // btn_MarkDesmark
            // 
            this.btn_MarkDesmark.Location = new System.Drawing.Point(12, 244);
            this.btn_MarkDesmark.Name = "btn_MarkDesmark";
            this.btn_MarkDesmark.Size = new System.Drawing.Size(100, 40);
            this.btn_MarkDesmark.TabIndex = 110;
            this.btn_MarkDesmark.Text = "Select/Unselect all Constr.";
            this.btn_MarkDesmark.UseVisualStyleBackColor = true;
            this.btn_MarkDesmark.Click += new System.EventHandler(this.Btn_MarkDesmark_Click);
            // 
            // chk_ControlConst
            // 
            this.chk_ControlConst.AutoSize = true;
            this.chk_ControlConst.Location = new System.Drawing.Point(13, 142);
            this.chk_ControlConst.Name = "chk_ControlConst";
            this.chk_ControlConst.Size = new System.Drawing.Size(97, 19);
            this.chk_ControlConst.TabIndex = 111;
            this.chk_ControlConst.Text = "ControlConst";
            this.chk_ControlConst.UseVisualStyleBackColor = true;
            this.chk_ControlConst.CheckStateChanged += new System.EventHandler(this.Chk_ControlConst_CheckStateChanged);
            // 
            // chk_SolTest
            // 
            this.chk_SolTest.AutoSize = true;
            this.chk_SolTest.Location = new System.Drawing.Point(13, 168);
            this.chk_SolTest.Name = "chk_SolTest";
            this.chk_SolTest.Size = new System.Drawing.Size(65, 19);
            this.chk_SolTest.TabIndex = 112;
            this.chk_SolTest.Text = "Sol.Test";
            this.chk_SolTest.UseVisualStyleBackColor = true;
            // 
            // chk_Good
            // 
            this.chk_Good.AutoSize = true;
            this.chk_Good.Location = new System.Drawing.Point(13, 194);
            this.chk_Good.Name = "chk_Good";
            this.chk_Good.Size = new System.Drawing.Size(86, 19);
            this.chk_Good.TabIndex = 113;
            this.chk_Good.Text = "GoodConst";
            this.chk_Good.UseVisualStyleBackColor = true;
            this.chk_Good.CheckedChanged += new System.EventHandler(this.Chk_Good_CheckedChanged);
            // 
            // chk_Lazys
            // 
            this.chk_Lazys.AutoSize = true;
            this.chk_Lazys.Location = new System.Drawing.Point(13, 220);
            this.chk_Lazys.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chk_Lazys.Name = "chk_Lazys";
            this.chk_Lazys.Size = new System.Drawing.Size(54, 19);
            this.chk_Lazys.TabIndex = 114;
            this.chk_Lazys.Text = "Lazys";
            this.chk_Lazys.UseVisualStyleBackColor = true;
            // 
            // clb_SlcExc
            // 
            this.clb_SlcExc.FormattingEnabled = true;
            this.clb_SlcExc.Items.AddRange(new object[] {
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
            "20_B_AgC_nearconsec"});
            this.clb_SlcExc.Location = new System.Drawing.Point(316, 87);
            this.clb_SlcExc.Name = "clb_SlcExc";
            this.clb_SlcExc.Size = new System.Drawing.Size(162, 382);
            this.clb_SlcExc.TabIndex = 115;
            // 
            // lbl_clb_Constraints
            // 
            this.lbl_clb_Constraints.AutoSize = true;
            this.lbl_clb_Constraints.Location = new System.Drawing.Point(137, 66);
            this.lbl_clb_Constraints.Name = "lbl_clb_Constraints";
            this.lbl_clb_Constraints.Size = new System.Drawing.Size(67, 15);
            this.lbl_clb_Constraints.TabIndex = 116;
            this.lbl_clb_Constraints.Text = "Constraints";
            // 
            // lbl_clb_SlcExc
            // 
            this.lbl_clb_SlcExc.AutoSize = true;
            this.lbl_clb_SlcExc.Location = new System.Drawing.Point(316, 66);
            this.lbl_clb_SlcExc.Name = "lbl_clb_SlcExc";
            this.lbl_clb_SlcExc.Size = new System.Drawing.Size(79, 15);
            this.lbl_clb_SlcExc.TabIndex = 117;
            this.lbl_clb_SlcExc.Text = "Slack / Excess";
            // 
            // cmB_Unity
            // 
            this.cmB_Unity.FormattingEnabled = true;
            this.cmB_Unity.Items.AddRange(new object[] {
            "C"});
            this.cmB_Unity.Location = new System.Drawing.Point(431, 30);
            this.cmB_Unity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmB_Unity.Name = "cmB_Unity";
            this.cmB_Unity.Size = new System.Drawing.Size(47, 23);
            this.cmB_Unity.TabIndex = 118;
            this.cmB_Unity.SelectedIndexChanged += new System.EventHandler(this.CmB_Unity_SelectedIndexChanged);
            // 
            // lbl_Unity
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(389, 32);
            this.label1.Name = "lbl_Unity";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 119;
            this.label1.Text = "Unity";
            // 
            // btn_StressTest
            // 
            this.btn_StressTest.Location = new System.Drawing.Point(137, 31);
            this.btn_StressTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_StressTest.Name = "btn_StressTest";
            this.btn_StressTest.Size = new System.Drawing.Size(83, 22);
            this.btn_StressTest.TabIndex = 120;
            this.btn_StressTest.Text = "Stress";
            this.btn_StressTest.UseVisualStyleBackColor = true;
            this.btn_StressTest.Click += new System.EventHandler(this.StressTest);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(13, 481);
            this.lbl_Status.MaximumSize = new System.Drawing.Size(525, 15);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(43, 15);
            this.lbl_Status.TabIndex = 121;
            this.lbl_Status.Text = "Pronto";
            // 
            // btn_1by1
            // 
            this.btn_1by1.Location = new System.Drawing.Point(226, 31);
            this.btn_1by1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_1by1.Name = "btn_1by1";
            this.btn_1by1.Size = new System.Drawing.Size(83, 22);
            this.btn_1by1.TabIndex = 122;
            this.btn_1by1.Text = "testes 1por1";
            this.btn_1by1.UseVisualStyleBackColor = true;
            this.btn_1by1.Click += new System.EventHandler(this.Btn_1by1_Click);
            // 
            // btn_AllSE
            // 
            this.btn_AllSE.Location = new System.Drawing.Point(12, 284);
            this.btn_AllSE.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btn_AllSE.Name = "btn_AllSE";
            this.btn_AllSE.Size = new System.Drawing.Size(100, 40);
            this.btn_AllSE.TabIndex = 123;
            this.btn_AllSE.Text = "Sel./Unsel all SE Constr.";
            this.btn_AllSE.UseVisualStyleBackColor = true;
            this.btn_AllSE.Click += new System.EventHandler(this.Btn_AllSE_Click);
            // 
            // btnSolve_Flow
            // 
            this.btnSolve_Flow.Location = new System.Drawing.Point(12, 404);
            this.btnSolve_Flow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSolve_Flow.Name = "btnSolve_Flow";
            this.btnSolve_Flow.Size = new System.Drawing.Size(100, 40);
            this.btnSolve_Flow.TabIndex = 124;
            this.btnSolve_Flow.Text = "Solve Only Flow";
            this.btnSolve_Flow.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 522);
            this.Controls.Add(this.btnSolve_Flow);
            this.Controls.Add(this.btn_AllSE);
            this.Controls.Add(this.btn_1by1);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.btn_StressTest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmB_Unity);
            this.Controls.Add(this.lbl_clb_SlcExc);
            this.Controls.Add(this.lbl_clb_Constraints);
            this.Controls.Add(this.clb_SlcExc);
            this.Controls.Add(this.chk_Lazys);
            this.Controls.Add(this.chk_Good);
            this.Controls.Add(this.chk_SolTest);
            this.Controls.Add(this.chk_ControlConst);
            this.Controls.Add(this.btn_MarkDesmark);
            this.Controls.Add(this.clb_Constraints);
            this.Controls.Add(this.lbl01);
            this.Controls.Add(this.cmb_Instance);
            this.Controls.Add(this.chk_ShowMess);
            this.Controls.Add(this.chk_log);
            this.Controls.Add(this.chk_SlackExcess);
            this.Controls.Add(this.btnSolve_Sched);
            this.Controls.Add(this.btnSolve_Instance);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSolve_Instance;
        private System.Windows.Forms.Button btnSolve_Sched;
        private System.Windows.Forms.CheckBox chk_SlackExcess;
        private System.Windows.Forms.CheckBox chk_log;
        private System.Windows.Forms.CheckBox chk_ShowMess;
        private System.Windows.Forms.ComboBox cmb_Instance;
        private System.Windows.Forms.Label lbl01;
        private System.Windows.Forms.CheckedListBox clb_Constraints;
        private System.Windows.Forms.Button btn_MarkDesmark;
        private System.Windows.Forms.CheckBox chk_ControlConst;
        private System.Windows.Forms.CheckBox chk_SolTest;
        private System.Windows.Forms.CheckBox chk_Good;
        private System.Windows.Forms.CheckBox chk_Lazys;
        private System.Windows.Forms.CheckedListBox clb_SlcExc;
        private System.Windows.Forms.Label lbl_clb_Constraints;
        private System.Windows.Forms.Label lbl_clb_SlcExc;
        private System.Windows.Forms.ComboBox cmB_Unity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_StressTest;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Button btn_1by1;
        private Button btn_AllSE;
        private Button btnSolve_Flow;
    }
}