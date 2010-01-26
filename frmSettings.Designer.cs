namespace TubeGuardian
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemoveAccount = new System.Windows.Forms.Button();
            this.btnAddAccount = new System.Windows.Forms.Button();
            this.lstAccounts = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtLogfile = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblRefresh = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.hRefresh = new System.Windows.Forms.HScrollBar();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkDisableAffected = new System.Windows.Forms.CheckBox();
            this.chkDisableAll = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAverageStars = new System.Windows.Forms.Label();
            this.hAverageVote = new System.Windows.Forms.HScrollBar();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRelevantRatings = new System.Windows.Forms.Label();
            this.hRelevant = new System.Windows.Forms.HScrollBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAnalyzerHelp = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(6, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(358, 234);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtPassword);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.txtUsername);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.btnClear);
            this.tabPage1.Controls.Add(this.btnRemoveAccount);
            this.tabPage1.Controls.Add(this.btnAddAccount);
            this.tabPage1.Controls.Add(this.lstAccounts);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(350, 208);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Accounts";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(22, 70);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(125, 20);
            this.txtPassword.TabIndex = 7;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Password";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(22, 31);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(125, 20);
            this.txtUsername.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Username:";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(20, 170);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(145, 27);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear Accounts";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemoveAccount
            // 
            this.btnRemoveAccount.Location = new System.Drawing.Point(20, 137);
            this.btnRemoveAccount.Name = "btnRemoveAccount";
            this.btnRemoveAccount.Size = new System.Drawing.Size(145, 27);
            this.btnRemoveAccount.TabIndex = 2;
            this.btnRemoveAccount.Text = "Remove Account";
            this.btnRemoveAccount.UseVisualStyleBackColor = true;
            this.btnRemoveAccount.Click += new System.EventHandler(this.btnRemoveAccount_Click);
            // 
            // btnAddAccount
            // 
            this.btnAddAccount.Location = new System.Drawing.Point(20, 104);
            this.btnAddAccount.Name = "btnAddAccount";
            this.btnAddAccount.Size = new System.Drawing.Size(145, 27);
            this.btnAddAccount.TabIndex = 1;
            this.btnAddAccount.Text = "Add Account";
            this.btnAddAccount.UseVisualStyleBackColor = true;
            this.btnAddAccount.Click += new System.EventHandler(this.btnAddAccount_Click);
            // 
            // lstAccounts
            // 
            this.lstAccounts.FormattingEnabled = true;
            this.lstAccounts.Location = new System.Drawing.Point(192, 11);
            this.lstAccounts.Name = "lstAccounts";
            this.lstAccounts.Size = new System.Drawing.Size(147, 186);
            this.lstAccounts.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtLogfile);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.lblRefresh);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.hRefresh);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(350, 208);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Collector";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtLogfile
            // 
            this.txtLogfile.Enabled = false;
            this.txtLogfile.Location = new System.Drawing.Point(21, 159);
            this.txtLogfile.Name = "txtLogfile";
            this.txtLogfile.Size = new System.Drawing.Size(208, 20);
            this.txtLogfile.TabIndex = 4;
            this.txtLogfile.TextChanged += new System.EventHandler(this.txtLogfile_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(150, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Log data to file (blank if none):";
            // 
            // lblRefresh
            // 
            this.lblRefresh.AutoSize = true;
            this.lblRefresh.Location = new System.Drawing.Point(279, 49);
            this.lblRefresh.Name = "lblRefresh";
            this.lblRefresh.Size = new System.Drawing.Size(13, 13);
            this.lblRefresh.TabIndex = 2;
            this.lblRefresh.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(177, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Refresh video data every (minutes) :";
            // 
            // hRefresh
            // 
            this.hRefresh.Location = new System.Drawing.Point(21, 47);
            this.hRefresh.Name = "hRefresh";
            this.hRefresh.Size = new System.Drawing.Size(209, 15);
            this.hRefresh.TabIndex = 0;
            this.hRefresh.ValueChanged += new System.EventHandler(this.hRefresh_ValueChanged);
            this.hRefresh.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hRefresh_Scroll);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkDisableAffected);
            this.tabPage3.Controls.Add(this.chkDisableAll);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.lblAverageStars);
            this.tabPage3.Controls.Add(this.hAverageVote);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.lblRelevantRatings);
            this.tabPage3.Controls.Add(this.hRelevant);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.btnAnalyzerHelp);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(350, 208);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Guardian";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkDisableAffected
            // 
            this.chkDisableAffected.AutoSize = true;
            this.chkDisableAffected.Location = new System.Drawing.Point(21, 152);
            this.chkDisableAffected.Name = "chkDisableAffected";
            this.chkDisableAffected.Size = new System.Drawing.Size(159, 17);
            this.chkDisableAffected.TabIndex = 12;
            this.chkDisableAffected.Text = "Disable only affected videos";
            this.chkDisableAffected.UseVisualStyleBackColor = true;
            this.chkDisableAffected.CheckedChanged += new System.EventHandler(this.chkDisableAffected_CheckedChanged);
            // 
            // chkDisableAll
            // 
            this.chkDisableAll.AutoSize = true;
            this.chkDisableAll.Location = new System.Drawing.Point(21, 129);
            this.chkDisableAll.Name = "chkDisableAll";
            this.chkDisableAll.Size = new System.Drawing.Size(228, 17);
            this.chkDisableAll.TabIndex = 11;
            this.chkDisableAll.Text = "Disable all videos on the attacked account";
            this.chkDisableAll.UseVisualStyleBackColor = true;
            this.chkDisableAll.CheckedChanged += new System.EventHandler(this.chkDisableAll_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Respond by:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(295, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "stars.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(289, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "ratings";
            // 
            // lblAverageStars
            // 
            this.lblAverageStars.AutoSize = true;
            this.lblAverageStars.Location = new System.Drawing.Point(262, 74);
            this.lblAverageStars.Name = "lblAverageStars";
            this.lblAverageStars.Size = new System.Drawing.Size(13, 13);
            this.lblAverageStars.TabIndex = 7;
            this.lblAverageStars.Text = "0";
            // 
            // hAverageVote
            // 
            this.hAverageVote.Location = new System.Drawing.Point(123, 72);
            this.hAverageVote.Maximum = 50;
            this.hAverageVote.Minimum = 1;
            this.hAverageVote.Name = "hAverageVote";
            this.hAverageVote.Size = new System.Drawing.Size(115, 15);
            this.hAverageVote.TabIndex = 6;
            this.hAverageVote.Value = 1;
            this.hAverageVote.ValueChanged += new System.EventHandler(this.hAverageVote_ValueChanged);
            this.hAverageVote.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hAverageVote_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "average less than:";
            // 
            // lblRelevantRatings
            // 
            this.lblRelevantRatings.AutoSize = true;
            this.lblRelevantRatings.Location = new System.Drawing.Point(262, 45);
            this.lblRelevantRatings.Name = "lblRelevantRatings";
            this.lblRelevantRatings.Size = new System.Drawing.Size(13, 13);
            this.lblRelevantRatings.TabIndex = 4;
            this.lblRelevantRatings.Text = "0";
            // 
            // hRelevant
            // 
            this.hRelevant.Location = new System.Drawing.Point(123, 45);
            this.hRelevant.Minimum = 1;
            this.hRelevant.Name = "hRelevant";
            this.hRelevant.Size = new System.Drawing.Size(115, 15);
            this.hRelevant.TabIndex = 3;
            this.hRelevant.Value = 1;
            this.hRelevant.ValueChanged += new System.EventHandler(this.hRelevant_ValueChanged);
            this.hRelevant.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hRelevant_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "The last:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Consider a video under attack if:";
            // 
            // btnAnalyzerHelp
            // 
            this.btnAnalyzerHelp.Location = new System.Drawing.Point(279, 173);
            this.btnAnalyzerHelp.Name = "btnAnalyzerHelp";
            this.btnAnalyzerHelp.Size = new System.Drawing.Size(59, 23);
            this.btnAnalyzerHelp.TabIndex = 0;
            this.btnAnalyzerHelp.Text = "Help!";
            this.btnAnalyzerHelp.UseVisualStyleBackColor = true;
            this.btnAnalyzerHelp.Click += new System.EventHandler(this.btnAnalyzerHelp_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(209, 245);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(155, 28);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Save & Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // toolTip2
            // 
            this.toolTip2.AutomaticDelay = 300;
            this.toolTip2.AutoPopDelay = 3000;
            this.toolTip2.InitialDelay = 100;
            this.toolTip2.ReshowDelay = 60;
            this.toolTip2.ToolTipTitle = "Please note, you must restart the program for this setting change to apply.";
            this.toolTip2.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip2_Popup);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 281);
            this.ControlBox = false;
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettings";
            this.Text = "frmSettings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox lstAccounts;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnAnalyzerHelp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRelevantRatings;
        private System.Windows.Forms.HScrollBar hRelevant;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAverageStars;
        private System.Windows.Forms.HScrollBar hAverageVote;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDisableAffected;
        private System.Windows.Forms.CheckBox chkDisableAll;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnAddAccount;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemoveAccount;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblRefresh;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.HScrollBar hRefresh;
        private System.Windows.Forms.TextBox txtLogfile;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolTip toolTip2;
    }
}