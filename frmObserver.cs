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
    public partial class frmObserver : Form
    {
        private clsActors _actors;
        public frmObserver(clsActors Actors)
        {
            _actors = Actors;
            InitializeComponent();
        }

        private Timer _update;
        private int _update_count;
        private void frmObserver_Load(object sender, EventArgs e)
        {
            _update = new Timer();
            _update.Interval = 600;
            _update_count = 0;
            _update.Tick += new EventHandler(_update_Tick);
            _update.Enabled = true;

            foreach (clsVideoEntry video in _actors.Collector.CurrentDataSet)
            {
                string[] item = { video.Account.Username, video.Title, video.Time.ToShortTimeString(), Math.Round(_actors.Analyzer.AverageNewRatingByID(video.VideoID), 4).ToString(), video.Raters.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.RATERS) + ")", Math.Round(_actors.Analyzer.AverageNewRatingByID(video.VideoID, 10).Value, 4).ToString(), video.ViewsCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.VIEWS) + ")", Math.Round(video.AverageRating, 4).ToString() + " (" + Math.Round(_actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.AVERAGE_RATING), 4) + ")", video.CommentCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.COMMENT_COUNT) + ")", video.FavoritedCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.FAVORITED_COUNT) + ")" };
                ListViewItem new_item = new ListViewItem(item);
                new_item.Tag = video.VideoID;
                lvCurrentDataset.Items.Add(new_item);
            }

            foreach (clsVideoFeedReader fr in _actors.Collector.FeedReaders)
            {
                string[] item = { fr.Username, fr.Updated.ToShortTimeString(), fr.StateString };
                lvFRStatus.Items.Add(new ListViewItem(item));
            }
            _actors.Collector.OnEntryAdded += new clsStatMonger.EntryAddedEventHandler(Collector_OnEntryAdded);
            _actors.Collector.OnEntryUpdated += new clsStatMonger.EntryUpdatedEventHandler(Collector_OnEntryUpdated);
            _actors.Collector.OnFeedReaderUpdated += new clsStatMonger.FeedReaderUpdatedEventHandler(Collector_OnFeedReaderUpdated);
            _actors.Collector.OnFeedReaderException += new clsStatMonger.FeedReaderExceptionEventHandler(Collector_OnFeedReaderException);
        
        }
        void _update_Tick(object sender, EventArgs e)
        {
            _update_count++;
            pbUpdate.Value = _update_count;
            if (_update_count < 100)
                return;
            _update_count = 0;

            lvCurrentDataset.Items.Clear();
            foreach (clsVideoEntry video in _actors.Collector.CurrentDataSet)
            {
                string[] item = { video.Account.Username, video.Title, video.Time.ToShortTimeString(), Math.Round(_actors.Analyzer.AverageNewRatingByID(video.VideoID), 4).ToString(), video.Raters.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.RATERS) + ")", Math.Round(_actors.Analyzer.AverageNewRatingByID(video.VideoID, 10).Value, 4).ToString(), video.ViewsCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.VIEWS) + ")", Math.Round(video.AverageRating, 4).ToString() + " (" + Math.Round(_actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.AVERAGE_RATING), 4) + ")", video.CommentCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.COMMENT_COUNT) + ")", video.FavoritedCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.FAVORITED_COUNT) + ")" };
                ListViewItem new_item = new ListViewItem(item);
                new_item.Tag = video.VideoID;
                lvCurrentDataset.Items.Add(new_item);
            }
            lvFRStatus.Items.Clear();
            foreach (clsVideoFeedReader fr in _actors.Collector.FeedReaders)
            {
                string[] item = { fr.Username, fr.Updated.ToShortTimeString(), fr.StateString };
                lvFRStatus.Items.Add(new ListViewItem(item));
            }
        }

        delegate void GenericDelegate();

        void Collector_OnFeedReaderException(object Sender, string exception)
        {
            clsVideoFeedReader fr = (clsVideoFeedReader)Sender; 
            
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { lstEvents.Items.Add(fr.Username + " X:" + exception); };
                lstEvents.Invoke(dlg);
            }
            else
                lstEvents.Items.Add(fr.Username + " X:" + exception);
        }
        void Collector_OnFeedReaderUpdated(object Sender, clsVideoFeedReader.enumFeedReaderState status)
        {
            clsVideoFeedReader fr = (clsVideoFeedReader)Sender;
            
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { lstEvents.Items.Add(fr.Username + " U:" + status.ToString()); };
                lstEvents.Invoke(dlg);
            }
            else
                lstEvents.Items.Add(fr.Username + " U:" + status.ToString());
        }
        void Collector_OnEntryUpdated(object Sender, clsDataPoint DataPoint, clsVideoEntry Entry)
        {
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { lstEvents.Items.Add(Entry.Account.Username + " : Entry added"); };
                lstEvents.Invoke(dlg);
            }
            else
                lstEvents.Items.Add(Entry.Account.Username + " : Entry added");
            
        }
        void Collector_OnEntryAdded(object Sender, clsVideoEntry Entry)
        {
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { lstEvents.Items.Add(Entry.Account.Username + " : Entry updated"); };
                lstEvents.Invoke(dlg);
            }
            else
                lstEvents.Items.Add(Entry.Account.Username + " : Entry updated");
            
        }

        private void frmObserver_FormClosing(object sender, FormClosingEventArgs e)
        {
            _update.Enabled = false;
            _update.Dispose(); 
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {
            _update.Enabled = false;
            _update_count = 0;

            lvCurrentDataset.Items.Clear();
            foreach (clsVideoEntry video in _actors.Collector.CurrentDataSet)
            {
                string[] item = { video.Account.Username, video.Title, video.Time.ToShortTimeString(), Math.Round(_actors.Analyzer.AverageNewRatingByID(video.VideoID), 4).ToString(), video.Raters.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.RATERS) + ")", Math.Round(_actors.Analyzer.AverageNewRatingByID(video.VideoID, 10).Value, 4).ToString(), video.ViewsCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.VIEWS) + ")", Math.Round(video.AverageRating, 4).ToString() + " (" + Math.Round(_actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.AVERAGE_RATING), 4) + ")", video.CommentCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.COMMENT_COUNT) + ")", video.FavoritedCount.ToString() + " (" + _actors.Analyzer.GetMovedStatByField(video.VideoID, clsDataPointField.VideoDataFields.FAVORITED_COUNT) + ")" };
                ListViewItem new_item = new ListViewItem(item);
                new_item.Tag = video.VideoID;
                lvCurrentDataset.Items.Add(new_item);
            }
            lvFRStatus.Items.Clear();
            foreach (clsVideoFeedReader fr in _actors.Collector.FeedReaders)
            {
                string[] item = { fr.Username, fr.Updated.ToShortTimeString(), fr.StateString };
                lvFRStatus.Items.Add(new ListViewItem(item));
            }
            _update.Enabled = true;
        }

        private ListViewItem lvItem = null;
        private List<ListViewItem> lvItems = null;
        private void lvCurrentDataset_MouseUp(object sender, MouseEventArgs e)
        {
            lvItem = null;
            lvItems = new List<ListViewItem>();

            ListView listView = sender as ListView;

            if (e.Button == MouseButtons.Right)
            {
                if (lvCurrentDataset.SelectedItems.Count == 1)
                {
                    ListViewItem item = listView.GetItemAt(e.X, e.Y);
                    if (item != null)
                    {
                        lvItem = item;
                        mnuVideoTitle.Text = item.SubItems[1].Text;
                        menu.Show(lvCurrentDataset, new Point(e.X, e.Y));
                    }
                }
                else if (lvCurrentDataset.SelectedItems.Count > 1)
                {
                    ListView.SelectedListViewItemCollection selected = lvCurrentDataset.SelectedItems;
                    foreach (ListViewItem i in selected)
                    {
                        lvItems.Add(i);
                        menu_multi.Show(lvCurrentDataset, new Point(e.X, e.Y));
                    }
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (lvItem == null)
                return;
            System.Diagnostics.Process.Start("http://www.youtube.com/my_videos_edit?video_id=mFXQpv3Kpv4");
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (lvItem == null)
                return;
            System.Diagnostics.Process.Start("http://www.youtube.com/my_videos_insight?v=" + lvItem.Tag.ToString());
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
             if (lvItem == null)
                return;
             System.Diagnostics.Process.Start("http://www.youtube.com/watch?v=" + lvItem.Tag.ToString());
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (lvItem == null)
                return;

            clsCredentials account = null;
            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
            {
                if (sm.Username == lvItem.SubItems[0].Text)
                {
                    account = new clsCredentials(sm.Username, sm.Password);
                    break;
                }
            }
            _actors.ED.ChangeAccountRatings(account, lvItem.Tag.ToString(), false);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (lvItem == null)
                return;

            clsCredentials account = null;
            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
            {
                if (sm.Username == lvItem.SubItems[0].Text)
                {
                    account = new clsCredentials(sm.Username, sm.Password);
                    break;
                }
            }
            _actors.ED.ChangeAccountRatings(account, lvItem.Tag.ToString(), true);

        }

        private void lvCurrentDataset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (lvItems == null || lvItems.Count == 0)
                return;

            foreach (ListViewItem item in lvItems)
            {
                clsCredentials account = null;
                foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
                {
                    if (sm.Username == item.SubItems[0].Text)
                    {
                        account = new clsCredentials(sm.Username, sm.Password);
                        break;
                    }
                }
                if (account != null) 
                    _actors.ED.ChangeAccountRatings(account, item.Tag.ToString(), false);
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (lvItems == null || lvItems.Count == 0)
                return;

            foreach (ListViewItem item in lvItems)
            {
                clsCredentials account = null;
                foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
                {
                    if (sm.Username == item.SubItems[0].Text)
                    {
                        account = new clsCredentials(sm.Username, sm.Password);
                        break;
                    }
                }
                if (account != null)
                    _actors.ED.ChangeAccountRatings(account, item.Tag.ToString(), true);
            }
        }
    }
}
