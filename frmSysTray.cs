using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TubeGuardian
{
    public partial class frmSysTray : Form
    {
        private Timer _check_for_attack = new Timer();
        private Timer _check_for_reenable = new Timer();

        private Dictionary<clsVideoEntry, DateTime> Enable_List = new Dictionary<clsVideoEntry, DateTime>();

        private clsActors _actors;
        public frmSysTray(clsActors actors)
        {
            _check_for_attack.Interval = 60000; // check for attacks once per minute
            _check_for_attack.Tick += new EventHandler(_check_for_attack_Tick);
            _check_for_attack.Enabled = true;

            _check_for_reenable.Interval = 60000;
            _check_for_reenable.Tick += new EventHandler(_check_for_reenable_Tick);
            _check_for_reenable.Enabled = true;

            _actors = actors;
            
            InitializeComponent();
        }

        void _check_for_reenable_Tick(object sender, EventArgs e)
        {
            if (Enable_List.Count == 0)
                return;
            List<clsVideoEntry> remove = new List<clsVideoEntry>();

            foreach (KeyValuePair<clsVideoEntry, DateTime> kvp in Enable_List)
            {
                if (kvp.Value < DateTime.Now)
                {
                    _actors.ED.ChangeAccountRatings(kvp.Key.Account, kvp.Key.VideoID, true);
                    remove.Add(kvp.Key);
                }
            }
            foreach (clsVideoEntry v in remove)
            {
                if (Enable_List.ContainsKey(v))
                    Enable_List.Remove(v);
            }
        }

        private void _check_for_attack_Tick(object sender, EventArgs e)
        {
            if (!mnuEnableAnalyzer.Checked)
                return;

            foreach (clsVideoEntry v in _actors.Collector.CurrentDataSet)
            {
                if (Enable_List.ContainsKey(v))
                    continue;

                KeyValuePair<int, double> kvp = _actors.Analyzer.AverageNewRatingByID(v.VideoID, _actors.Settings.Analyzer_Relevant_Entries);
                
                // if (num ratings used in calculation) >= (setting num relevant AND (new avg rating) <= (settings avg rating threshold)
                if ( (kvp.Key >= _actors.Settings.Analyzer_Relevant_Entries) && (kvp.Value <= _actors.Settings.Analyzer_Vote_Threshold))
                {
                    // attack detected!
                    if (_actors.Settings.Analyzer_Disable_Affected)
                    {
                        _actors.ED.ChangeAccountRatings(v.Account, v.VideoID, false);
                        DateTime enable_time = DateTime.Now.AddMinutes((double)_actors.Settings.Analyzer_Disable_Duration);
                        add_to_enable_list(v, enable_time);
                    }
                    if (_actors.Settings.Analyzer_Disable_All)
                    {
                        _actors.ED.ChangeAccountRatings(v.Account, _actors.Collector.GetVideosByUsername(v.Account.Username), false);
                        DateTime enable_time = DateTime.Now.AddMinutes((double)_actors.Settings.Analyzer_Disable_Duration);
                        foreach (clsVideoEntry ve in _actors.Collector.GetVideosByUsername(v.Account.Username))
                            add_to_enable_list(ve, enable_time);
                    }
                }
            }
        }

        private void add_to_enable_list(clsVideoEntry e, DateTime t)
        {
            if (Enable_List.ContainsKey(e))
                return;
            Enable_List.Add(e, t);
        }
        private void mnuQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mainMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private frmSettings Settings;
        private void mnuSettings_Click(object sender, EventArgs e)
        {
            if (Settings == null)
                Settings = new frmSettings(_actors);

            try { Settings.Show(); }
            catch { Settings = new frmSettings(_actors); Settings.Show(); } 
        }

        private frmStatAnalyzer Statistics;
        private void mnuStatistics_Click(object sender, EventArgs e)
        {
            if (Statistics == null)
                Statistics = new frmStatAnalyzer(_actors);

            try { Statistics.Show(); }
            catch { Statistics = new frmStatAnalyzer(_actors); Statistics.Show(); }
        }

        private void mnuObserverEnable_Click(object sender, EventArgs e)
        {
            if (mnuObserverEnable.Checked)
                _actors.Collector.Enable();
            else
                _actors.Collector.Disable();
        }

        private frmObserver Observer;
        private void mnuObserverStatus_Click(object sender, EventArgs e)
        {
            if (Observer == null)
                Observer = new frmObserver(_actors);

            try { Observer.Show(); }
            catch { Observer = new frmObserver(_actors); Observer.Show(); }
        }

        private void frmSysTray_Load(object sender, EventArgs e)
        {
            _actors.ED.OnStartedAction += new clsED.StartedActionEventHandler(ED_OnStartedAction);

            if (_actors.Settings.Accounts.Count > 0)
            {
                _actors.Collector.Enable();
                mnuEnableAnalyzer.Checked = true;
                mnuObserverEnable.Checked = true;
            }
            
        }

        private frmED ED;
        void ED_OnStartedAction(object Sender)
        {
            if (ED == null)
                ED = new frmED(_actors);

            try { ED.Show(); }
            catch { ED = new frmED(_actors); ED.Show(); }
        }

        private void mnuToolbox_Click(object sender, EventArgs e)
        {   
        }

        private void enableAllVideosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (clsVideoEntry entry in _actors.Collector.InitialDataSet)
            {
                if (Enable_List.ContainsKey(entry))
                    continue;
                _actors.ED.ChangeAccountRatings(entry.Account, entry.VideoID, true);
            }
        }

        private void disableAllVideosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (clsVideoEntry entry in _actors.Collector.InitialDataSet)
            {
                if (Enable_List.ContainsKey(entry))
                    continue;
                _actors.ED.ChangeAccountRatings(entry.Account, entry.VideoID, false);
            }
        }

        private void mnuOpenLogfile_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", "CollectorData.log");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TubeGuedianDeux");
            }
        }

        private void saveLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsFileLogger fl;
            saveFile.CheckPathExists = true;
            saveFile.InitialDirectory = Application.StartupPath;
            saveFile.DefaultExt = "All files (*.*)|*.*|TubeGuardian Log (*.log)|*.log";
            saveFile.FileName = "BackupCollectorData.log";

            if (saveFile.ShowDialog() == DialogResult.OK)
                fl = new clsFileLogger(saveFile.FileName);
            else
                return;

            foreach (clsVideoEntry ve in _actors.Collector.InitialDataSet)
                fl.appendFile(ve.ToString());
            foreach (KeyValuePair<string, List<clsDataPoint>> kvp in _actors.Collector.HistoricalDataPoints)
                foreach (clsDataPoint dp in kvp.Value)
                    fl.appendFile(dp.ToString());

            MessageBox.Show("File save successfully.", "TubeGuardianDeux");
        }

        private void clearLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFile("CollectorData.log");
        }

        private static bool DeleteFile(string sFileName)
        {
            try
            {
                if (File.Exists(sFileName) == true)
                {
                    File.Delete(sFileName);
                    return true;
                }
                else
                {
                    MessageBox.Show("File doesnot exist", "TubeGuardianDeux");
                    return false;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // File in Use
                MessageBox.Show("You do not have required permission for this operation " + ex.Message, "TubeGuardianDeux");
                return false;
            }
            catch (IOException exIO)
            {
                // File in Use
                MessageBox.Show("File in use " + exIO.Message, "TubeGuardianDeux");
                return false;
            }
            catch (Exception ex)
            {//Common Exception
                MessageBox.Show(ex.Message, "TubeGuardianDeux");
                return false;
            }
        }

        private void clearErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFile("TubeGuardian.log");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7403004");
        }
    }
}
