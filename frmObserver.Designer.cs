namespace TubeGuardian
{
    partial class frmObserver
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmObserver));
            this.label1 = new System.Windows.Forms.Label();
            this.lvCurrentDataset = new System.Windows.Forms.ListView();
            this.colAccount = new System.Windows.Forms.ColumnHeader();
            this.colVideoID = new System.Windows.Forms.ColumnHeader();
            this.colUpdated = new System.Windows.Forms.ColumnHeader();
            this.colANR = new System.Windows.Forms.ColumnHeader();
            this.colRaters = new System.Windows.Forms.ColumnHeader();
            this.colANR10 = new System.Windows.Forms.ColumnHeader();
            this.colViews = new System.Windows.Forms.ColumnHeader();
            this.colAR = new System.Windows.Forms.ColumnHeader();
            this.colComments = new System.Windows.Forms.ColumnHeader();
            this.colFavorites = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.lvFRStatus = new System.Windows.Forms.ListView();
            this.colFRAccount = new System.Windows.Forms.ColumnHeader();
            this.colFRUpdated = new System.Windows.Forms.ColumnHeader();
            this.coFRStatus = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.lstEvents = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbUpdate = new System.Windows.Forms.ToolStripProgressBar();
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuVideoTitle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_multi = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menu.SuspendLayout();
            this.menu_multi.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Dataset:";
            // 
            // lvCurrentDataset
            // 
            this.lvCurrentDataset.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAccount,
            this.colVideoID,
            this.colUpdated,
            this.colANR,
            this.colRaters,
            this.colANR10,
            this.colViews,
            this.colAR,
            this.colComments,
            this.colFavorites});
            this.lvCurrentDataset.FullRowSelect = true;
            this.lvCurrentDataset.Location = new System.Drawing.Point(14, 26);
            this.lvCurrentDataset.Name = "lvCurrentDataset";
            this.lvCurrentDataset.Size = new System.Drawing.Size(707, 200);
            this.lvCurrentDataset.TabIndex = 1;
            this.lvCurrentDataset.UseCompatibleStateImageBehavior = false;
            this.lvCurrentDataset.View = System.Windows.Forms.View.Details;
            this.lvCurrentDataset.SelectedIndexChanged += new System.EventHandler(this.lvCurrentDataset_SelectedIndexChanged);
            this.lvCurrentDataset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvCurrentDataset_MouseUp);
            // 
            // colAccount
            // 
            this.colAccount.Text = "Account";
            this.colAccount.Width = 83;
            // 
            // colVideoID
            // 
            this.colVideoID.Text = "Video Title";
            this.colVideoID.Width = 182;
            // 
            // colUpdated
            // 
            this.colUpdated.Text = "Updated";
            this.colUpdated.Width = 65;
            // 
            // colANR
            // 
            this.colANR.Text = "ANR";
            this.colANR.Width = 55;
            // 
            // colRaters
            // 
            this.colRaters.Text = "Raters";
            this.colRaters.Width = 53;
            // 
            // colANR10
            // 
            this.colANR10.Text = "ANR10";
            this.colANR10.Width = 55;
            // 
            // colViews
            // 
            this.colViews.Text = "Views";
            this.colViews.Width = 49;
            // 
            // colAR
            // 
            this.colAR.Text = "A. Rating";
            // 
            // colComments
            // 
            this.colComments.Text = "Comments";
            this.colComments.Width = 52;
            // 
            // colFavorites
            // 
            this.colFavorites.Text = "Favs";
            this.colFavorites.Width = 47;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 238);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Feed Reader Status";
            // 
            // lvFRStatus
            // 
            this.lvFRStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFRAccount,
            this.colFRUpdated,
            this.coFRStatus});
            this.lvFRStatus.Location = new System.Drawing.Point(12, 263);
            this.lvFRStatus.Name = "lvFRStatus";
            this.lvFRStatus.Size = new System.Drawing.Size(418, 174);
            this.lvFRStatus.TabIndex = 3;
            this.lvFRStatus.UseCompatibleStateImageBehavior = false;
            this.lvFRStatus.View = System.Windows.Forms.View.Details;
            // 
            // colFRAccount
            // 
            this.colFRAccount.Text = "Account";
            // 
            // colFRUpdated
            // 
            this.colFRUpdated.Text = "Updated";
            // 
            // coFRStatus
            // 
            this.coFRStatus.Text = "Status";
            this.coFRStatus.Width = 293;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(439, 238);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Event Log:";
            // 
            // lstEvents
            // 
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.Location = new System.Drawing.Point(445, 263);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(276, 173);
            this.lstEvents.TabIndex = 5;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.pbUpdate});
            this.statusStrip1.Location = new System.Drawing.Point(0, 449);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(733, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(300, 17);
            this.lblStatus.Text = "Click here to update the page or updating in ...";
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // pbUpdate
            // 
            this.pbUpdate.Name = "pbUpdate";
            this.pbUpdate.Size = new System.Drawing.Size(366, 16);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuVideoTitle,
            this.toolStripSeparator1,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripSeparator2,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(234, 148);
            // 
            // mnuVideoTitle
            // 
            this.mnuVideoTitle.Enabled = false;
            this.mnuVideoTitle.Name = "mnuVideoTitle";
            this.mnuVideoTitle.Size = new System.Drawing.Size(233, 22);
            this.mnuVideoTitle.Text = "VIDEO TITLE";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(230, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(233, 22);
            this.toolStripMenuItem1.Text = "Open this video\'s edit page";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(233, 22);
            this.toolStripMenuItem2.Text = "Open this video\'s insight page";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(233, 22);
            this.toolStripMenuItem3.Text = "Open this video\'s main page";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(230, 6);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(233, 22);
            this.toolStripMenuItem4.Text = "Disable this video\'s ratings";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(233, 22);
            this.toolStripMenuItem5.Text = "Enable this video\'s ratings";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // menu_multi
            // 
            this.menu_multi.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.menu_multi.Name = "menu_multi";
            this.menu_multi.Size = new System.Drawing.Size(160, 48);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(159, 22);
            this.toolStripMenuItem6.Text = "Disable Selected";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(159, 22);
            this.toolStripMenuItem7.Text = "Enable Selected";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // frmObserver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 471);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lstEvents);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lvFRStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lvCurrentDataset);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmObserver";
            this.ShowIcon = false;
            this.Text = "Observer Status";
            this.Load += new System.EventHandler(this.frmObserver_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmObserver_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menu.ResumeLayout(false);
            this.menu_multi.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvCurrentDataset;
        private System.Windows.Forms.ColumnHeader colAccount;
        private System.Windows.Forms.ColumnHeader colVideoID;
        private System.Windows.Forms.ColumnHeader colUpdated;
        private System.Windows.Forms.ColumnHeader colANR;
        private System.Windows.Forms.ColumnHeader colRaters;
        private System.Windows.Forms.ColumnHeader colANR10;
        private System.Windows.Forms.ColumnHeader colViews;
        private System.Windows.Forms.ColumnHeader colAR;
        private System.Windows.Forms.ColumnHeader colComments;
        private System.Windows.Forms.ColumnHeader colFavorites;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvFRStatus;
        private System.Windows.Forms.ColumnHeader colFRAccount;
        private System.Windows.Forms.ColumnHeader colFRUpdated;
        private System.Windows.Forms.ColumnHeader coFRStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstEvents;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar pbUpdate;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem mnuVideoTitle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ContextMenuStrip menu_multi;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
    }
}