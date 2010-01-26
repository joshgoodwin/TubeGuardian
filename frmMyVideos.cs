using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YouTube.VideoFeedReader;
using System.Web;
using mshtml;

namespace TubeGuardian
{
    public partial class frmMyVideos : Form
    {
        public string dev_key = "AI39si5Son6sjRyMLwFF4GQPxxgQMwAc9pkbUjy0W4qAvb4om_FuAakEWPKMS6rrh2A85zpY31CVlbEP5uwOmpgog4YAuBJXmQ";
        public string app_name = "TubeGuardian";
        public clsVideoFeedReader feedReader;

        public void do_events()
        {
            Application.DoEvents();

        }
        public frmMyVideos()
        {
            InitializeComponent();
        }

        private void frmMyVideos_Load(object sender, EventArgs e)
        {
        }
        private void frmMyVideos_Unload(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        void feedReader_OnVideoAdded(object sender, Google.GData.YouTube.YouTubeEntry video)
        {
            if (video == null)
                return;
            clsVideoDataEntry e = new clsVideoDataEntry(video);
            string[] item = { e.VideoEntry.VideoId.ToString(), e.VideoEntry.Title.Text, e.Views.ToString(), e.Comments.ToString(), e.Favorites.ToString(), e.Ratings.ToString(), e.AverageRating.ToString() };
            lvVideos.Items.Add(new ListViewItem(item));
        }
        
        void feedReader_OnRequestException(object sender, Google.GData.Client.GDataRequestException e)
        {
            lblStatus.Text = "Request exception encountered!";
        }

        void feedReader_OnQueryStarted(object sender, string query)
        {
            lblStatus.Text = "Starting video query.";
        }

        void feedReader_OnQueryProgress(object sender, int current, int total)
        {
            pbStatus.Maximum = total;
            pbStatus.Value = current;
        }

        void feedReader_OnQueryEnded(object sender, string query)
        {
            lblStatus.Text = "Query completed.";
        }

        void feedReader_OnGeneralException(object sender, Exception e)
        {
            lblStatus.Text = "General exception encountered!";
        }

        void feedReader_OnAuthenticationException(object sender, Google.GData.Client.AuthenticationException e)
        {
            lblStatus.Text = "Authentication exception encountered!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lvVideos.Items.Clear();
            if (Program._main.settings.txtPassword.Text != string.Empty)
                feedReader = new clsVideoFeedReader(dev_key, app_name, Program._main.settings.txtUsername.Text, Program._main.settings.txtPassword.Text);
            else
            {
                feedReader = new clsVideoFeedReader(dev_key, app_name);
                feedReader.Username = Program._main.settings.txtUsername.Text.ToString();
            }
            feedReader.OnAuthenticationException += new clsVideoFeedReader.AuthenticationExceptionEventHandler(feedReader_OnAuthenticationException);
            feedReader.OnGeneralException += new clsVideoFeedReader.GeneralExceptionEventHandler(feedReader_OnGeneralException);
            feedReader.OnQueryEnded += new clsVideoFeedReader.EndQueryEventHandler(feedReader_OnQueryEnded);
            feedReader.OnQueryProgress += new clsVideoFeedReader.ProgressUpdateEventHandler(feedReader_OnQueryProgress);
            feedReader.OnQueryStarted += new clsVideoFeedReader.StartQueryEventHandler(feedReader_OnQueryStarted);
            feedReader.OnRequestException += new clsVideoFeedReader.RequestExceptionEventHandler(feedReader_OnRequestException);
            feedReader.OnVideoAdded += new clsVideoFeedReader.AddVideoEventHandler(feedReader_OnVideoAdded);
            feedReader.DoEvents = do_events;
            feedReader.getVideos();
            pbStatus.Value = 0;
            lblStatus.Text = "Idle.";
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {           
            ListView.SelectedListViewItemCollection items = lvVideos.SelectedItems;
            if (items.Count <= 0)
                return;

            clsVideoSettingsUpdate update = new clsVideoSettingsUpdate(wb);
            update.DoEvents = do_events;
            update.OnStatusUpdated += new clsVideoSettingsUpdate.StatusEventHandler(update_OnStatusUpdated);
            update.OnCompleted += new clsVideoSettingsUpdate.CompletedEventHandler(update_OnCompleted);

            lblStatus.Text = "Loading login page";
            update.GotoLoginPage();
            if (update.IsLoginPage)
                update.Login(Program._main.settings.txtUsername.Text.ToString(), Program._main.settings.txtPassword.Text.ToString());
            else
                return;

            foreach (ListViewItem i in items)
            {
                string V = i.SubItems[0].Text.ToString();
                if (update.IsLoggedIn)
                    update.AllowRatings(V, false);
                else
                {
                    update.GotoLoginPage();
                    if (update.IsLoginPage)
                        update.Login(Program._main.settings.txtUsername.Text.ToString(), Program._main.settings.txtPassword.Text.ToString());
                    else
                        return;
                    if (update.IsLoggedIn)
                        update.AllowRatings(V, false);
                    else
                        return;
                }
            }
        }

        void update_OnCompleted(object sender, bool success, string error)
        {
            lblStatus.Text = (success ? "Done." : "An error occurred: " + error);
        }

        void update_OnStatusUpdated(object sender, string status)
        {
            lblStatus.Text = status;
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lvVideos.SelectedItems;
            if (items.Count <= 0)
                return;

            clsVideoSettingsUpdate update = new clsVideoSettingsUpdate(wb);
            update.DoEvents = do_events;
            update.OnStatusUpdated += new clsVideoSettingsUpdate.StatusEventHandler(update_OnStatusUpdated);
            update.OnCompleted += new clsVideoSettingsUpdate.CompletedEventHandler(update_OnCompleted);

            lblStatus.Text = "Loading login page";
            update.GotoLoginPage();
            if (update.IsLoginPage)
                update.Login(Program._main.settings.txtUsername.Text.ToString(), Program._main.settings.txtPassword.Text.ToString());
            else
                return;

            foreach (ListViewItem i in items)
            {
                string V = i.SubItems[0].Text.ToString();
                if (update.IsLoggedIn)
                    update.AllowRatings(V, true);
                else
                {
                    update.GotoLoginPage();
                    if (update.IsLoginPage)
                        update.Login(Program._main.settings.txtUsername.Text.ToString(), Program._main.settings.txtPassword.Text.ToString());
                    else
                        return;
                    if (update.IsLoggedIn)
                        update.AllowRatings(V, true);
                    else
                        return;
                }
            } 
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in lvVideos.Items)
                i.Selected = true;
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in lvVideos.Items)
                i.Selected = false;
        }
    }
}
