using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TubeGuardian
{
    public partial class frmSettings : Form
    {
        private clsActors _actors;

        public frmSettings(clsActors Actors)
        {
            _actors = Actors;
            InitializeComponent();
        }

        private void hRelevant_Scroll(object sender, ScrollEventArgs e)
        {
            lblRelevantRatings.Text = hRelevant.Value.ToString();
        }

        private void hRelevant_ValueChanged(object sender, EventArgs e)
        {
            lblRelevantRatings.Text = hRelevant.Value.ToString();
            _actors.Settings.Analyzer_Relevant_Entries = hRelevant.Value;
        }

        private void hAverageVote_Scroll(object sender, ScrollEventArgs e)
        {
            lblAverageStars.Text = ((double)hAverageVote.Value / (double)10).ToString();
        }

        private void hAverageVote_ValueChanged(object sender, EventArgs e)
        {
            lblAverageStars.Text = ((double)hAverageVote.Value / (double)10).ToString();
            _actors.Settings.Analyzer_Vote_Threshold = ((double)hAverageVote.Value / (double)10);
        }

        private void btnAnalyzerHelp_Click(object sender, EventArgs e)
        {
            string message = string.Empty;

            message = "Everytime you revieve new votes, the analyzer will determine what their average value was. ";
            message += "When a vote bot is attacking, you might be getting ratings from legitimate viewers at ";
            message += "the same time. So, instead of just looking for one star ratings, you can tell the analyzer ";
            message += "consider the votes an attack based on an average rating value of your choice. The number of ";
            message += "relevant ratings serves two purposes. The first is to determine the minimum number of ratings ";
            message += "that can trigger the protection. This way, the program wont consider a few bad ratings in a ";
            message += "to be an attack.\n\nCurrent reccommended settings are:\n\nVote threshold: 10\nRating threshold: 2.0\n\nPlease note that if you get a high volume of positive ratings, this number may need to come up a bit.";

            MessageBox.Show(message, "Tube Guardian Deux");
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            this.Text = "Tube Guardian Deux";
            foreach (clsCredentials acct in _actors.Settings.Accounts)
                lstAccounts.Items.Add(acct.Username);
            hRelevant.Value = _actors.Settings.Analyzer_Relevant_Entries;
            hRefresh.Value = _actors.Settings.Collect_Interval;
            hAverageVote.Value = (int)(_actors.Settings.Analyzer_Vote_Threshold * 10);
            chkDisableAffected.Checked = _actors.Settings.Analyzer_Disable_Affected;
            chkDisableAll.Checked = _actors.Settings.Analyzer_Disable_All;

            lblAverageStars.Text = ((double)hAverageVote.Value / (double)10).ToString();
            lblRefresh.Text = hRefresh.Value.ToString();
            lblRelevantRatings.Text = hRelevant.Value.ToString();

            txtLogfile.Text = _actors.Settings.Collect_Log_File;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _actors.Settings.SaveSettings();
            this.Close();
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text != string.Empty && txtPassword.Text != string.Empty)
            {
                _actors.Settings.AddAccount(new clsCredentials(txtUsername.Text, txtPassword.Text));
                lstAccounts.Items.Clear();
                foreach (clsCredentials acct in _actors.Settings.Accounts)
                    lstAccounts.Items.Add(acct.Username);
            }
            txtUsername.Text = txtPassword.Text = string.Empty;
        }

        private void btnRemoveAccount_Click(object sender, EventArgs e)
        {
            if (lstAccounts.SelectedItem != null)
            {
                clsCredentials acct = _actors.Settings.GetAccountByUsername(lstAccounts.SelectedItem.ToString());
                if (acct != null)
                    _actors.Settings.RemoveAccount(acct);
                lstAccounts.Items.Clear();
                foreach (clsCredentials acc in _actors.Settings.Accounts)
                    lstAccounts.Items.Add(acc.Username);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the entire list?", "Tube Guardian Deux", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            int i = 0;
            while (i < _actors.Settings.Accounts.Count)
            {
                _actors.Settings.RemoveAccount(_actors.Settings.Accounts[i]);
            }
            lstAccounts.Items.Clear();
            foreach (clsCredentials acc in _actors.Settings.Accounts)
                lstAccounts.Items.Add(acc.Username);
        }

        private void hRefresh_Scroll(object sender, ScrollEventArgs e)
        {
            lblRefresh.Text = hRefresh.Value.ToString();
        }

        private void hRefresh_ValueChanged(object sender, EventArgs e)
        {
            lblRefresh.Text = hRefresh.Value.ToString();
            _actors.Settings.Collect_Interval = hRefresh.Value;
        }

        private void txtLogfile_TextChanged(object sender, EventArgs e)
        {
            _actors.Settings.Collect_Log_File = txtLogfile.Text;
        }

        private void chkDisableAll_CheckedChanged(object sender, EventArgs e)
        {
            _actors.Settings.Analyzer_Disable_All = chkDisableAll.Checked;
        }

        private void chkDisableAffected_CheckedChanged(object sender, EventArgs e)
        {
            _actors.Settings.Analyzer_Disable_Affected = chkDisableAffected.Checked;
        }

        private void toolTip2_Popup(object sender, PopupEventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtUsername.Text != string.Empty && txtPassword.Text != string.Empty)
                {
                    _actors.Settings.AddAccount(new clsCredentials(txtUsername.Text, txtPassword.Text));
                    lstAccounts.Items.Clear();
                    foreach (clsCredentials acct in _actors.Settings.Accounts)
                        lstAccounts.Items.Add(acct.Username);
                }
                txtUsername.Text = txtPassword.Text = string.Empty;
                e.Handled = true;
            }
        }
    }
}
