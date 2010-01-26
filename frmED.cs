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
    public partial class frmED : Form
    {
        private clsActors _actors;
        public frmED(clsActors Actors)
        {
            _actors = Actors;
            InitializeComponent();
        }

        private void frmED_Load(object sender, EventArgs e)
        {
            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
            {
                string[] item = { sm.Username, sm.VideoID, sm.State.ToString(), sm.ActivityListCount.ToString() };
                lvSMs.Items.Add(new ListViewItem(item));
                sm.OnStatusChange += new clsSettingsManager.StatusEventHandler(sm_OnStatusChange);
                sm.OnSuccess += new clsSettingsManager.SuccessEventHandler(sm_OnSuccess);
                sm.OnFailure += new clsSettingsManager.FailureEventHandler(sm_OnFailure);
            }
        }

        delegate void GenericDelegate();

        void sm_OnFailure(object sender, clsSettingsManager.FailureCodes f)
        {
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { clsSettingsManager sm = (clsSettingsManager)sender; ListViewItem item = _index_of_lv_by_username(sm.Username); if (item == null) return; item.SubItems[3].Text = sm.ActivityListCount.ToString(); };
                lvSMs.Invoke(dlg);
            }
            else
            {
                clsSettingsManager sm = (clsSettingsManager)sender; 
                ListViewItem item = _index_of_lv_by_username(sm.Username); 
                if (item == null) 
                    return; 
                item.SubItems[3].Text = sm.ActivityListCount.ToString();
            }
        }

        void sm_OnSuccess(object sender, clsSettingsManager.FailureCodes f)
        {
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { clsSettingsManager sm = (clsSettingsManager)sender; ListViewItem item = _index_of_lv_by_username(sm.Username); if (item == null) return; item.SubItems[3].Text = sm.ActivityListCount.ToString(); };
                lvSMs.Invoke(dlg);
            }
            else
            {
                clsSettingsManager sm = (clsSettingsManager)sender; ListViewItem item = _index_of_lv_by_username(sm.Username); if (item == null) return; item.SubItems[3].Text = sm.ActivityListCount.ToString();
            }
        }

        void sm_OnStatusChange(object sender, clsSettingsManager.InternalState s)
        {
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { clsSettingsManager sm = (clsSettingsManager)sender; ListViewItem item = _index_of_lv_by_username(sm.Username); if (item == null) return; item.SubItems[2].Text = s.ToString(); item.SubItems[1].Text = sm.VideoID; };
                lvSMs.Invoke(dlg);
            }
            else
            {
                clsSettingsManager sm = (clsSettingsManager)sender; 
                ListViewItem item = _index_of_lv_by_username(sm.Username); 
                if (item == null) 
                    return; 
                item.SubItems[2].Text = s.ToString();
                item.SubItems[1].Text = sm.VideoID;
            }
        }
        private ListViewItem _index_of_lv_by_username(string username)
        {
            foreach (ListViewItem item in lvSMs.Items)
            {
                if (item.SubItems[0].Text == username)
                    return item;
            }
            return null;
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers) 
                sm.StopActing();

            lvSMs.Items.Clear();
            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
            {
                string[] item = { sm.Username, sm.VideoID, sm.State.ToString(), sm.ActivityListCount.ToString() };
                lvSMs.Items.Add(new ListViewItem(item));
            }
        }

        private void btnCancelSelected_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lvSMs.SelectedItems;
            if (items.Count <= 0)
                return;
            ListViewItem item = items[0];

            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
                if (sm.Username == item.SubItems[0].Text)
                    sm.StopActing();

            lvSMs.Items.Clear();
            foreach (clsSettingsManager sm in _actors.ED.SettingsManagers)
            {
                string[] litem = { sm.Username, sm.VideoID, sm.State.ToString(), sm.ActivityListCount.ToString() };
                lvSMs.Items.Add(new ListViewItem(litem));
            }
        }

    }
}
