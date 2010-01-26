namespace TubeGuardian
{
    partial class frmSysTray
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSysTray));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.mainMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuObserverEnable = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEnableAnalyzer = new System.Windows.Forms.ToolStripMenuItem();
            this.sep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuObserverStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolbox = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAllVideosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllVideosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuOpenLogfile = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearErrorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.mainMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TubeGuardianDeux";
            this.notifyIcon1.Visible = true;
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSettings,
            this.toolStripSeparator1,
            this.mnuObserverEnable,
            this.mnuEnableAnalyzer,
            this.sep,
            this.mnuObserverStatus,
            this.mnuStatistics,
            this.mnuToolbox,
            this.toolStripSeparator2,
            this.mnuQuit,
            this.toolStripSeparator4,
            this.toolStripMenuItem1});
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(161, 226);
            this.mainMenu.Opening += new System.ComponentModel.CancelEventHandler(this.mainMenu_Opening);
            // 
            // mnuSettings
            // 
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(160, 22);
            this.mnuSettings.Text = "&Settings";
            this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // mnuObserverEnable
            // 
            this.mnuObserverEnable.CheckOnClick = true;
            this.mnuObserverEnable.Name = "mnuObserverEnable";
            this.mnuObserverEnable.Size = new System.Drawing.Size(160, 22);
            this.mnuObserverEnable.Text = "Enable &Observer";
            this.mnuObserverEnable.Click += new System.EventHandler(this.mnuObserverEnable_Click);
            // 
            // mnuEnableAnalyzer
            // 
            this.mnuEnableAnalyzer.CheckOnClick = true;
            this.mnuEnableAnalyzer.Name = "mnuEnableAnalyzer";
            this.mnuEnableAnalyzer.Size = new System.Drawing.Size(160, 22);
            this.mnuEnableAnalyzer.Text = "Enable &Guardian";
            // 
            // sep
            // 
            this.sep.Name = "sep";
            this.sep.Size = new System.Drawing.Size(157, 6);
            // 
            // mnuObserverStatus
            // 
            this.mnuObserverStatus.Name = "mnuObserverStatus";
            this.mnuObserverStatus.Size = new System.Drawing.Size(160, 22);
            this.mnuObserverStatus.Text = "Observer Status";
            this.mnuObserverStatus.Click += new System.EventHandler(this.mnuObserverStatus_Click);
            // 
            // mnuStatistics
            // 
            this.mnuStatistics.Name = "mnuStatistics";
            this.mnuStatistics.Size = new System.Drawing.Size(160, 22);
            this.mnuStatistics.Text = "Statistics";
            this.mnuStatistics.Click += new System.EventHandler(this.mnuStatistics_Click);
            // 
            // mnuToolbox
            // 
            this.mnuToolbox.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableAllVideosToolStripMenuItem,
            this.disableAllVideosToolStripMenuItem,
            this.toolStripSeparator3,
            this.mnuOpenLogfile,
            this.clearLogfileToolStripMenuItem,
            this.clearErrorLogToolStripMenuItem,
            this.saveLogfileToolStripMenuItem});
            this.mnuToolbox.Name = "mnuToolbox";
            this.mnuToolbox.Size = new System.Drawing.Size(160, 22);
            this.mnuToolbox.Text = "&Toolbox";
            this.mnuToolbox.Click += new System.EventHandler(this.mnuToolbox_Click);
            // 
            // enableAllVideosToolStripMenuItem
            // 
            this.enableAllVideosToolStripMenuItem.Name = "enableAllVideosToolStripMenuItem";
            this.enableAllVideosToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.enableAllVideosToolStripMenuItem.Text = "&Enable All Videos";
            this.enableAllVideosToolStripMenuItem.Click += new System.EventHandler(this.enableAllVideosToolStripMenuItem_Click);
            // 
            // disableAllVideosToolStripMenuItem
            // 
            this.disableAllVideosToolStripMenuItem.Name = "disableAllVideosToolStripMenuItem";
            this.disableAllVideosToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.disableAllVideosToolStripMenuItem.Text = "&Disable All Videos";
            this.disableAllVideosToolStripMenuItem.Click += new System.EventHandler(this.disableAllVideosToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuOpenLogfile
            // 
            this.mnuOpenLogfile.Name = "mnuOpenLogfile";
            this.mnuOpenLogfile.Size = new System.Drawing.Size(219, 22);
            this.mnuOpenLogfile.Text = "Open Collector Log";
            this.mnuOpenLogfile.Click += new System.EventHandler(this.mnuOpenLogfile_Click);
            // 
            // clearLogfileToolStripMenuItem
            // 
            this.clearLogfileToolStripMenuItem.Name = "clearLogfileToolStripMenuItem";
            this.clearLogfileToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.clearLogfileToolStripMenuItem.Text = "Clear Collector Log";
            this.clearLogfileToolStripMenuItem.Click += new System.EventHandler(this.clearLogfileToolStripMenuItem_Click);
            // 
            // clearErrorLogToolStripMenuItem
            // 
            this.clearErrorLogToolStripMenuItem.Name = "clearErrorLogToolStripMenuItem";
            this.clearErrorLogToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.clearErrorLogToolStripMenuItem.Text = "Clear Error Log";
            this.clearErrorLogToolStripMenuItem.Click += new System.EventHandler(this.clearErrorLogToolStripMenuItem_Click);
            // 
            // saveLogfileToolStripMenuItem
            // 
            this.saveLogfileToolStripMenuItem.Name = "saveLogfileToolStripMenuItem";
            this.saveLogfileToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.saveLogfileToolStripMenuItem.Text = "Save Current Collector Data";
            this.saveLogfileToolStripMenuItem.Click += new System.EventHandler(this.saveLogfileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // mnuQuit
            // 
            this.mnuQuit.Name = "mnuQuit";
            this.mnuQuit.Size = new System.Drawing.Size(160, 22);
            this.mnuQuit.Text = "&Quit";
            this.mnuQuit.Click += new System.EventHandler(this.mnuQuit_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem1.Text = "Donate!";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // frmSysTray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSysTray";
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmSysTray";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.frmSysTray_Load);
            this.mainMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuObserverEnable;
        private System.Windows.Forms.ToolStripMenuItem mnuEnableAnalyzer;
        private System.Windows.Forms.ToolStripSeparator sep;
        private System.Windows.Forms.ToolStripMenuItem mnuObserverStatus;
        private System.Windows.Forms.ToolStripMenuItem mnuStatistics;
        private System.Windows.Forms.ToolStripMenuItem mnuToolbox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuQuit;
        private System.Windows.Forms.ToolStripMenuItem enableAllVideosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableAllVideosToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenLogfile;
        private System.Windows.Forms.ToolStripMenuItem saveLogfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLogfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearErrorLogToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;

    }
}