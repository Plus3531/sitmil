namespace SituatorMilestone
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIncidentId = new System.Windows.Forms.TextBox();
            this.textBoxMilestones = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSitWebApi = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxPrognose = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxProgDuur = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxProgHGI = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxProgType = new System.Windows.Forms.ComboBox();
            this.buttonUpdateProg = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.dateTimePickerDateProg = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxProgRedenWijziging = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTisWijzigingsType = new System.Windows.Forms.ComboBox();
            this.comboBoxTis = new System.Windows.Forms.ComboBox();
            this.buttonUpdateTis = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerDateTis = new System.Windows.Forms.DateTimePicker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.groupBoxPrognose.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Prognose && TIS Milestones";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "incidentId";
            // 
            // textBoxIncidentId
            // 
            this.textBoxIncidentId.Location = new System.Drawing.Point(67, 51);
            this.textBoxIncidentId.Name = "textBoxIncidentId";
            this.textBoxIncidentId.Size = new System.Drawing.Size(67, 20);
            this.textBoxIncidentId.TabIndex = 3;
            this.textBoxIncidentId.Text = "81124";
            // 
            // textBoxMilestones
            // 
            this.textBoxMilestones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMilestones.Location = new System.Drawing.Point(0, 173);
            this.textBoxMilestones.Multiline = true;
            this.textBoxMilestones.Name = "textBoxMilestones";
            this.textBoxMilestones.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxMilestones.Size = new System.Drawing.Size(929, 374);
            this.textBoxMilestones.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Situator Web Api";
            // 
            // textBoxSitWebApi
            // 
            this.textBoxSitWebApi.Location = new System.Drawing.Point(8, 25);
            this.textBoxSitWebApi.Name = "textBoxSitWebApi";
            this.textBoxSitWebApi.Size = new System.Drawing.Size(252, 20);
            this.textBoxSitWebApi.TabIndex = 6;
            this.textBoxSitWebApi.Text = "http://eo-iis-dev/SituatorWebAPI";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxPrognose);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBoxSitWebApi);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBoxIncidentId);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(929, 173);
            this.panel1.TabIndex = 8;
            // 
            // groupBoxPrognose
            // 
            this.groupBoxPrognose.Controls.Add(this.label10);
            this.groupBoxPrognose.Controls.Add(this.comboBoxProgDuur);
            this.groupBoxPrognose.Controls.Add(this.label9);
            this.groupBoxPrognose.Controls.Add(this.comboBoxProgHGI);
            this.groupBoxPrognose.Controls.Add(this.label8);
            this.groupBoxPrognose.Controls.Add(this.comboBoxProgType);
            this.groupBoxPrognose.Controls.Add(this.buttonUpdateProg);
            this.groupBoxPrognose.Controls.Add(this.label5);
            this.groupBoxPrognose.Controls.Add(this.dateTimePickerDateProg);
            this.groupBoxPrognose.Controls.Add(this.label6);
            this.groupBoxPrognose.Controls.Add(this.textBoxProgRedenWijziging);
            this.groupBoxPrognose.Location = new System.Drawing.Point(549, 25);
            this.groupBoxPrognose.Name = "groupBoxPrognose";
            this.groupBoxPrognose.Size = new System.Drawing.Size(368, 129);
            this.groupBoxPrognose.TabIndex = 11;
            this.groupBoxPrognose.TabStop = false;
            this.groupBoxPrognose.Text = "Update prognose";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(207, 70);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Duur";
            this.toolTip1.SetToolTip(this.label10, "Handmatig gezet indicatie");
            // 
            // comboBoxProgDuur
            // 
            this.comboBoxProgDuur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProgDuur.FormattingEnabled = true;
            this.comboBoxProgDuur.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "90",
            "120",
            "180",
            "300",
            "480"});
            this.comboBoxProgDuur.Location = new System.Drawing.Point(239, 67);
            this.comboBoxProgDuur.Name = "comboBoxProgDuur";
            this.comboBoxProgDuur.Size = new System.Drawing.Size(89, 21);
            this.comboBoxProgDuur.TabIndex = 25;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(207, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "HGI";
            this.toolTip1.SetToolTip(this.label9, "Handmatig gezet indicatie");
            // 
            // comboBoxProgHGI
            // 
            this.comboBoxProgHGI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProgHGI.FormattingEnabled = true;
            this.comboBoxProgHGI.Items.AddRange(new object[] {
            "ja",
            "nee"});
            this.comboBoxProgHGI.Location = new System.Drawing.Point(239, 40);
            this.comboBoxProgHGI.Name = "comboBoxProgHGI";
            this.comboBoxProgHGI.Size = new System.Drawing.Size(89, 21);
            this.comboBoxProgHGI.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(207, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Type";
            // 
            // comboBoxProgType
            // 
            this.comboBoxProgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProgType.FormattingEnabled = true;
            this.comboBoxProgType.Items.AddRange(new object[] {
            "Initieel",
            "Aangepast",
            "Definitief"});
            this.comboBoxProgType.Location = new System.Drawing.Point(239, 13);
            this.comboBoxProgType.Name = "comboBoxProgType";
            this.comboBoxProgType.Size = new System.Drawing.Size(89, 21);
            this.comboBoxProgType.TabIndex = 21;
            // 
            // buttonUpdateProg
            // 
            this.buttonUpdateProg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUpdateProg.Location = new System.Drawing.Point(304, 95);
            this.buttonUpdateProg.Name = "buttonUpdateProg";
            this.buttonUpdateProg.Size = new System.Drawing.Size(58, 23);
            this.buttonUpdateProg.TabIndex = 15;
            this.buttonUpdateProg.Text = "Update";
            this.buttonUpdateProg.UseVisualStyleBackColor = true;
            this.buttonUpdateProg.Click += new System.EventHandler(this.buttonUpdateProg_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Planned end time";
            // 
            // dateTimePickerDateProg
            // 
            this.dateTimePickerDateProg.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDateProg.Location = new System.Drawing.Point(20, 42);
            this.dateTimePickerDateProg.Name = "dateTimePickerDateProg";
            this.dateTimePickerDateProg.Size = new System.Drawing.Size(167, 20);
            this.dateTimePickerDateProg.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Reden wijziging";
            // 
            // textBoxProgRedenWijziging
            // 
            this.textBoxProgRedenWijziging.Location = new System.Drawing.Point(20, 95);
            this.textBoxProgRedenWijziging.Name = "textBoxProgRedenWijziging";
            this.textBoxProgRedenWijziging.Size = new System.Drawing.Size(167, 20);
            this.textBoxProgRedenWijziging.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxTisWijzigingsType);
            this.groupBox1.Controls.Add(this.comboBoxTis);
            this.groupBox1.Controls.Add(this.buttonUpdateTis);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dateTimePickerDateTis);
            this.groupBox1.Location = new System.Drawing.Point(266, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 129);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Update scenario (TIS)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Wijzigingstype";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "TIS";
            // 
            // comboBoxTisWijzigingsType
            // 
            this.comboBoxTisWijzigingsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTisWijzigingsType.FormattingEnabled = true;
            this.comboBoxTisWijzigingsType.Items.AddRange(new object[] {
            "initieel",
            "opschaling",
            "afschaling"});
            this.comboBoxTisWijzigingsType.Location = new System.Drawing.Point(87, 102);
            this.comboBoxTisWijzigingsType.Name = "comboBoxTisWijzigingsType";
            this.comboBoxTisWijzigingsType.Size = new System.Drawing.Size(89, 21);
            this.comboBoxTisWijzigingsType.TabIndex = 18;
            // 
            // comboBoxTis
            // 
            this.comboBoxTis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTis.FormattingEnabled = true;
            this.comboBoxTis.Items.AddRange(new object[] {
            "1.0",
            "1.1",
            "2.3",
            "3.4",
            "4.5"});
            this.comboBoxTis.Location = new System.Drawing.Point(87, 79);
            this.comboBoxTis.Name = "comboBoxTis";
            this.comboBoxTis.Size = new System.Drawing.Size(89, 21);
            this.comboBoxTis.TabIndex = 17;
            // 
            // buttonUpdateTis
            // 
            this.buttonUpdateTis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUpdateTis.Location = new System.Drawing.Point(193, 92);
            this.buttonUpdateTis.Name = "buttonUpdateTis";
            this.buttonUpdateTis.Size = new System.Drawing.Size(58, 23);
            this.buttonUpdateTis.TabIndex = 16;
            this.buttonUpdateTis.Text = "Update";
            this.buttonUpdateTis.UseVisualStyleBackColor = true;
            this.buttonUpdateTis.Click += new System.EventHandler(this.buttonUpdateTis_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Planned end time";
            // 
            // dateTimePickerDateTis
            // 
            this.dateTimePickerDateTis.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDateTis.Location = new System.Drawing.Point(20, 42);
            this.dateTimePickerDateTis.Name = "dateTimePickerDateTis";
            this.dateTimePickerDateTis.Size = new System.Drawing.Size(156, 20);
            this.dateTimePickerDateTis.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 547);
            this.Controls.Add(this.textBoxMilestones);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxPrognose.ResumeLayout(false);
            this.groupBoxPrognose.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIncidentId;
        private System.Windows.Forms.TextBox textBoxMilestones;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSitWebApi;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxPrognose;
        private System.Windows.Forms.Button buttonUpdateProg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dateTimePickerDateProg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxProgRedenWijziging;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonUpdateTis;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePickerDateTis;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTisWijzigingsType;
        private System.Windows.Forms.ComboBox comboBoxTis;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox comboBoxProgDuur;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxProgHGI;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxProgType;
    }
}

