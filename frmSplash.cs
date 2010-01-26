using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace TubeGuardian
{


    public partial class frmSplash : Form
    {
        public clsActors _actors = new clsActors("TubeGuardianDeux", "AI39si6KAyk2O5QnDJ7suAVV1ja2903FAAtHK96xEYH7j8oJzl3absfsoKbyRBM4uK80txtxzmMR-zEUPbXG-hu9cferIHyhog");

        public frmSplash()
        {
            InitializeComponent();
        }

        Timer move_on = new Timer();

        private frmSysTray trayMenu;

        private void frmSplash_Load(object sender, EventArgs e)
        {
            move_on.Tick += new EventHandler(move_on_Tick);
            move_on.Interval = 1500;
        }

        void move_on_Tick(object sender, EventArgs e)
        {
            move_on.Enabled = false;

            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                System.Net.WebClient client = new System.Net.WebClient();
                System.Console.WriteLine(asm.GetName().Version.ToString());
                if (client.DownloadString(new Uri("http://www.joshcgoodwin.com/tgv.dat")) != asm.GetName().Version.ToString())
                {
                    if (MessageBox.Show("Your TubeGuardian is out of date! Would you like to see the update page?", "TubeGuardianDeux", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("http://tg.leagueofreason.co.uk/");
                }
            }
            catch (Exception ex)
            {

            };

            Application.DoEvents();
            trayMenu = new frmSysTray(_actors);
            trayMenu.Visible = false;
            trayMenu.Show();

            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FadeEffect.FadeForm(this, 120);
            base.OnClosing(e);
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.youtube.com/user/joshTheGoods");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7402884");
        }

        private void frmSplash_Shown(object sender, EventArgs e)
        {
            move_on.Enabled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

    public class clsActors
    {
        private clsStatMonger _Collector;
        private clsStatMasher _Analyzer;
        private clsED _ED;
        private clsSettings _Settings;

        public clsActors(string ApplicationName, string DevKey)
        {
            _Settings = new clsSettings();
            _Collector = new clsStatMonger(DevKey, ApplicationName, _Settings.Accounts, _Settings);
            _Analyzer = new clsStatMasher(_Collector.InitialDataSet, _Collector.HistoricalDataPoints);
            _ED = new clsED(_Settings);
        }

        public clsSettings Settings { get { return _Settings; } }
        public clsStatMasher Analyzer { get { return _Analyzer; } }
        public clsStatMonger Collector { get { return _Collector; } }
        public clsED ED { get { return _ED; } }
    }
    public static class FadeEffect
    {
        public static void FadeForm(System.Windows.Forms.Form f, byte NumberOfSteps)
        {
            float StepVal = (float)(100f / NumberOfSteps);
            float fOpacity = 100f;
            for (byte b = 0; b < NumberOfSteps; b++)
            {
                f.Opacity = fOpacity / 100;
                f.Refresh();
                fOpacity -= StepVal;
            }
        }
    }
}



