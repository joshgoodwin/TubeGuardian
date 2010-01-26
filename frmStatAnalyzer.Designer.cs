namespace TubeGuardian
{
    partial class frmStatAnalyzer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStatAnalyzer));
            this.btnLoad = new System.Windows.Forms.Button();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.graphIndividual = new ZedGraph.ZedGraphControl();
            this.panelAll = new System.Windows.Forms.Panel();
            this.lblFavsMin = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFavsCount = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblViewersPerMin = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblViewerCount = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblRatersPerMin = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblRatersTotal = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblANR10 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblANR = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDataPointCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbVideo = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAddData = new System.Windows.Forms.Button();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelAll.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(9, 7);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(84, 24);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // openDialog
            // 
            this.openDialog.FileName = "CollectData.log";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            this.tableLayoutPanel1.Controls.Add(this.graphIndividual, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelAll, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbVideo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(747, 425);
            this.tableLayoutPanel1.TabIndex = 2;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // graphIndividual
            // 
            this.graphIndividual.AutoSize = true;
            this.graphIndividual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphIndividual.Location = new System.Drawing.Point(3, 28);
            this.graphIndividual.Name = "graphIndividual";
            this.graphIndividual.ScrollGrace = 0;
            this.graphIndividual.ScrollMaxX = 0;
            this.graphIndividual.ScrollMaxY = 0;
            this.graphIndividual.ScrollMaxY2 = 0;
            this.graphIndividual.ScrollMinX = 0;
            this.graphIndividual.ScrollMinY = 0;
            this.graphIndividual.ScrollMinY2 = 0;
            this.graphIndividual.Size = new System.Drawing.Size(594, 354);
            this.graphIndividual.TabIndex = 3;
            // 
            // panelAll
            // 
            this.panelAll.Controls.Add(this.lblFavsMin);
            this.panelAll.Controls.Add(this.label2);
            this.panelAll.Controls.Add(this.lblFavsCount);
            this.panelAll.Controls.Add(this.label11);
            this.panelAll.Controls.Add(this.lblViewersPerMin);
            this.panelAll.Controls.Add(this.label9);
            this.panelAll.Controls.Add(this.lblViewerCount);
            this.panelAll.Controls.Add(this.label8);
            this.panelAll.Controls.Add(this.lblRatersPerMin);
            this.panelAll.Controls.Add(this.label7);
            this.panelAll.Controls.Add(this.lblRatersTotal);
            this.panelAll.Controls.Add(this.label6);
            this.panelAll.Controls.Add(this.lblANR10);
            this.panelAll.Controls.Add(this.label5);
            this.panelAll.Controls.Add(this.lblANR);
            this.panelAll.Controls.Add(this.label4);
            this.panelAll.Controls.Add(this.lblDataPointCount);
            this.panelAll.Controls.Add(this.label3);
            this.panelAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAll.Location = new System.Drawing.Point(603, 28);
            this.panelAll.Name = "panelAll";
            this.panelAll.Size = new System.Drawing.Size(141, 354);
            this.panelAll.TabIndex = 2;
            // 
            // lblFavsMin
            // 
            this.lblFavsMin.Location = new System.Drawing.Point(40, 329);
            this.lblFavsMin.Name = "lblFavsMin";
            this.lblFavsMin.Size = new System.Drawing.Size(93, 16);
            this.lblFavsMin.TabIndex = 19;
            this.lblFavsMin.Text = "0";
            this.lblFavsMin.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 316);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Total Favs/min:";
            // 
            // lblFavsCount
            // 
            this.lblFavsCount.Location = new System.Drawing.Point(40, 300);
            this.lblFavsCount.Name = "lblFavsCount";
            this.lblFavsCount.Size = new System.Drawing.Size(93, 16);
            this.lblFavsCount.TabIndex = 17;
            this.lblFavsCount.Text = "0";
            this.lblFavsCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 287);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Total Favorites:";
            // 
            // lblViewersPerMin
            // 
            this.lblViewersPerMin.Location = new System.Drawing.Point(40, 271);
            this.lblViewersPerMin.Name = "lblViewersPerMin";
            this.lblViewersPerMin.Size = new System.Drawing.Size(93, 16);
            this.lblViewersPerMin.TabIndex = 15;
            this.lblViewersPerMin.Text = "0";
            this.lblViewersPerMin.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 258);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Total Viewers/min:";
            // 
            // lblViewerCount
            // 
            this.lblViewerCount.Location = new System.Drawing.Point(40, 242);
            this.lblViewerCount.Name = "lblViewerCount";
            this.lblViewerCount.Size = new System.Drawing.Size(93, 16);
            this.lblViewerCount.TabIndex = 13;
            this.lblViewerCount.Text = "0";
            this.lblViewerCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 229);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Total Viewers:";
            // 
            // lblRatersPerMin
            // 
            this.lblRatersPerMin.Location = new System.Drawing.Point(40, 213);
            this.lblRatersPerMin.Name = "lblRatersPerMin";
            this.lblRatersPerMin.Size = new System.Drawing.Size(93, 16);
            this.lblRatersPerMin.TabIndex = 11;
            this.lblRatersPerMin.Text = "0";
            this.lblRatersPerMin.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 200);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Total Raters/min:";
            // 
            // lblRatersTotal
            // 
            this.lblRatersTotal.Location = new System.Drawing.Point(40, 184);
            this.lblRatersTotal.Name = "lblRatersTotal";
            this.lblRatersTotal.Size = new System.Drawing.Size(93, 16);
            this.lblRatersTotal.TabIndex = 9;
            this.lblRatersTotal.Text = "0";
            this.lblRatersTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 171);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Total Raters:";
            // 
            // lblANR10
            // 
            this.lblANR10.Location = new System.Drawing.Point(40, 155);
            this.lblANR10.Name = "lblANR10";
            this.lblANR10.Size = new System.Drawing.Size(93, 16);
            this.lblANR10.TabIndex = 7;
            this.lblANR10.Text = "0";
            this.lblANR10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "ANR10:";
            // 
            // lblANR
            // 
            this.lblANR.Location = new System.Drawing.Point(40, 126);
            this.lblANR.Name = "lblANR";
            this.lblANR.Size = new System.Drawing.Size(93, 16);
            this.lblANR.TabIndex = 5;
            this.lblANR.Text = "0";
            this.lblANR.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "ANR:";
            // 
            // lblDataPointCount
            // 
            this.lblDataPointCount.Location = new System.Drawing.Point(40, 97);
            this.lblDataPointCount.Name = "lblDataPointCount";
            this.lblDataPointCount.Size = new System.Drawing.Size(93, 16);
            this.lblDataPointCount.TabIndex = 3;
            this.lblDataPointCount.Text = "0";
            this.lblDataPointCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Data Points:";
            // 
            // cbVideo
            // 
            this.cbVideo.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbVideo.FormattingEnabled = true;
            this.cbVideo.Location = new System.Drawing.Point(3, 3);
            this.cbVideo.Name = "cbVideo";
            this.cbVideo.Size = new System.Drawing.Size(594, 21);
            this.cbVideo.TabIndex = 4;
            this.cbVideo.SelectedIndexChanged += new System.EventHandler(this.cbVideo_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnAddData);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 388);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(594, 34);
            this.panel1.TabIndex = 6;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(189, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 24);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAddData
            // 
            this.btnAddData.Location = new System.Drawing.Point(99, 7);
            this.btnAddData.Name = "btnAddData";
            this.btnAddData.Size = new System.Drawing.Size(84, 24);
            this.btnAddData.TabIndex = 2;
            this.btnAddData.Text = "Add";
            this.btnAddData.UseVisualStyleBackColor = true;
            this.btnAddData.Click += new System.EventHandler(this.btnAddData_Click);
            // 
            // frmStatAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 425);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmStatAnalyzer";
            this.Text = "Statistcal Analyzer";
            this.Load += new System.EventHandler(this.frmStatAnalyzer_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelAll.ResumeLayout(false);
            this.panelAll.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelAll;
        private System.Windows.Forms.Label lblANR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblDataPointCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblViewersPerMin;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblViewerCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblRatersPerMin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblRatersTotal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblANR10;
        private System.Windows.Forms.Label label5;
        private ZedGraph.ZedGraphControl graphIndividual;
        private System.Windows.Forms.ComboBox cbVideo;
        private System.Windows.Forms.Label lblFavsMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFavsCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddData;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFile;
    }
}