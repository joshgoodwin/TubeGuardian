namespace TubeGuardian
{
    partial class frmED
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmED));
            this.lvSMs = new System.Windows.Forms.ListView();
            this.colAccount = new System.Windows.Forms.ColumnHeader();
            this.colVideoID = new System.Windows.Forms.ColumnHeader();
            this.colActionCount = new System.Windows.Forms.ColumnHeader();
            this.colAction = new System.Windows.Forms.ColumnHeader();
            this.btnCancelAll = new System.Windows.Forms.Button();
            this.btnCancelSelected = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvSMs
            // 
            this.lvSMs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAccount,
            this.colVideoID,
            this.colActionCount,
            this.colAction});
            this.lvSMs.FullRowSelect = true;
            this.lvSMs.Location = new System.Drawing.Point(10, 13);
            this.lvSMs.Name = "lvSMs";
            this.lvSMs.Size = new System.Drawing.Size(437, 152);
            this.lvSMs.TabIndex = 0;
            this.lvSMs.UseCompatibleStateImageBehavior = false;
            this.lvSMs.View = System.Windows.Forms.View.Details;
            // 
            // colAccount
            // 
            this.colAccount.Text = "Account";
            this.colAccount.Width = 100;
            // 
            // colVideoID
            // 
            this.colVideoID.Text = "Video ID";
            this.colVideoID.Width = 92;
            // 
            // colActionCount
            // 
            this.colActionCount.Text = "# Actions";
            // 
            // colAction
            // 
            this.colAction.Text = "Current Action";
            // 
            // btnCancelAll
            // 
            this.btnCancelAll.Location = new System.Drawing.Point(10, 171);
            this.btnCancelAll.Name = "btnCancelAll";
            this.btnCancelAll.Size = new System.Drawing.Size(145, 28);
            this.btnCancelAll.TabIndex = 1;
            this.btnCancelAll.Text = "Cancel all actions";
            this.btnCancelAll.UseVisualStyleBackColor = true;
            this.btnCancelAll.Click += new System.EventHandler(this.btnCancelAll_Click);
            // 
            // btnCancelSelected
            // 
            this.btnCancelSelected.Location = new System.Drawing.Point(161, 171);
            this.btnCancelSelected.Name = "btnCancelSelected";
            this.btnCancelSelected.Size = new System.Drawing.Size(145, 28);
            this.btnCancelSelected.TabIndex = 2;
            this.btnCancelSelected.Text = "Cancel selected actions";
            this.btnCancelSelected.UseVisualStyleBackColor = true;
            this.btnCancelSelected.Click += new System.EventHandler(this.btnCancelSelected_Click);
            // 
            // frmED
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 207);
            this.Controls.Add(this.btnCancelSelected);
            this.Controls.Add(this.btnCancelAll);
            this.Controls.Add(this.lvSMs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmED";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ED has been summoned!";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmED_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvSMs;
        private System.Windows.Forms.ColumnHeader colAccount;
        private System.Windows.Forms.ColumnHeader colVideoID;
        private System.Windows.Forms.ColumnHeader colActionCount;
        private System.Windows.Forms.Button btnCancelAll;
        private System.Windows.Forms.Button btnCancelSelected;
        private System.Windows.Forms.ColumnHeader colAction;
    }
}