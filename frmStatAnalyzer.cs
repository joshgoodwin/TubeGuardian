using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Collections;

namespace TubeGuardian
{
    public partial class frmStatAnalyzer : Form
    {

        private clsActors _actors;
        private clsStatMonger _monger;

        public frmStatAnalyzer(clsActors actors)
        {
            _actors = actors;
            _monger = _actors.Collector;
            InitializeComponent();
        }

        private void frmStatAnalyzer_Load(object sender, EventArgs e)
        {
            refresh_form();
        }

        private void refresh_form()
        {
            cbVideo.Items.Clear();
            for (int i = 0; i < _monger.InitialDataSet.Count; i++ )
            {
                cbVideo.Items.Add(_monger.InitialDataSet[i].Title);
            }
            if (cbVideo.Items.Count > 0)
                cbVideo.SelectedIndex = 0;
            redraw_chart();
        }

        PointPairList ARdata;
        PointPairList ANRdata;
        PointPairList Rdata;
        PointPairList Cdata;
        PointPairList Vdata;
        PointPairList Fdata;
        private void redraw_chart()
        {
            ARdata = new PointPairList();
            ANRdata = new PointPairList();
            Rdata = new PointPairList();
            Cdata = new PointPairList();
            Vdata = new PointPairList();
            Fdata = new PointPairList();
            
            int index = cbVideo.SelectedIndex;
            if (index == -1)
                return;
            string key = _monger.InitialDataSet[index].VideoID;
            if (!_monger.HistoricalDataPoints.ContainsKey(key))
                return;
            List<clsDataPoint> dps = _monger.HistoricalDataPoints[key];
            clsStatMasher my_sm = new clsStatMasher(_monger.InitialDataSet, _monger.HistoricalDataPoints);

            double[] N = { -1, -1 };
            double[] A = { -1, -1 };

            int dp_total = dps.Count;
            double ANR = my_sm.AverageNewRatingByID(key);
            double ANR10 = my_sm.AverageNewRatingByID(key, 10).Value;
            int raters_total = 0;
            double raters_min = 0;
            int viewer_total = 0;
            double viewers_min = 0;
            int favs_total = 0;
            double favs_min = 0;

            for (int i = 0; i < dps.Count; i++)
            {
                switch (dps[i].Field.Field)
                {
                    case clsDataPointField.VideoDataFields.RATERS:
                        raters_total += (int)dps[i].Delta;
                        break;
                    case clsDataPointField.VideoDataFields.VIEWS:
                        viewer_total += (int)dps[i].Delta;
                        break;
                    case clsDataPointField.VideoDataFields.FAVORITED_COUNT:
                        favs_total += (int)dps[i].Delta;
                        break;
                }
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.AVERAGE_RATING)
                    ARdata.Add(new PointPair(dps[i].Time.Ticks, dps[i].New, dps[i].Time.ToString()));
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.AVERAGE_RATING)
                {
                    if (A[0] == -1)
                    {
                        A[0] = dps[i].Old;
                        A[1] = dps[i].New;
                    }
                    else
                    {
                        A[1] = dps[i].New;
                    }
                }
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.RATERS)
                {
                    if (N[0] == -1)
                    {
                        N[0] = dps[i].Old;
                        N[1] = dps[i].New;
                    }
                    else
                    {
                        N[1] = dps[i].New;
                    }
                }
                if (A[1] != -1 && A[0] != -1 && N[1] != -1 && N[0] != -1)
                {
                    double anr = ((A[1] * N[1]) - (A[0] * N[0])) / (N[1] - N[0]);
                    A[0] = A[1]; A[1] = -1;
                    N[0] = N[1]; N[1] = -1;
                    ANRdata.Add(new PointPair(dps[i].Time.Ticks, anr, dps[i].Time.ToString()));
                }
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.RATERS)
                    Rdata.Add(new PointPair(dps[i].Time.Ticks, dps[i].New, dps[i].Time.ToString()));
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.COMMENT_COUNT)
                    Cdata.Add(new PointPair(dps[i].Time.Ticks, dps[i].New, dps[i].Time.ToString()));
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.VIEWS)
                    Vdata.Add(new PointPair(dps[i].Time.Ticks, dps[i].New, dps[i].Time.ToString()));
                if (dps[i].Field.Field == clsDataPointField.VideoDataFields.FAVORITED_COUNT)
                    Fdata.Add(new PointPair(dps[i].Time.Ticks, dps[i].New, dps[i].Time.ToString()));
            }

            try
            {
                TimeSpan span = dps[dps.Count - 1].Time - dps[0].Time;
                int mins = span.Minutes;
                raters_min = Math.Round(raters_total / (double)mins, 4);
                viewers_min = Math.Round(viewer_total / (double)mins, 4);
                favs_min = Math.Round(favs_total / (double)mins, 4);
            }
            catch { }
            lblANR.Text = ANR.ToString();
            lblANR10.Text = ANR10.ToString();
            lblDataPointCount.Text = dp_total.ToString();
            lblFavsCount.Text = favs_total.ToString();
            lblFavsMin.Text = favs_min.ToString();
            lblRatersPerMin.Text = raters_min.ToString();
            lblRatersTotal.Text = raters_total.ToString();
            lblViewerCount.Text = viewer_total.ToString();
            lblViewersPerMin.Text = viewers_min.ToString();

            CreateChart(graphIndividual);         
        }

        // Call this method from the Form_Load method, passing your ZedGraphControl
        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = new GraphPane();
            zgc.GraphPane = myPane;
            zgc.IsShowPointValues = true;

            // Set the titles and axis labels
            myPane.Title.Text = "Data for: " + cbVideo.SelectedItem.ToString();
            myPane.XAxis.Title.Text = "Time, Ticks (hover mouse over a point for date/time)";

            // Hide the legend
            // myPane.Legend.IsVisible = false;

            // Add a curve
            LineItem curve = myPane.AddCurve("Average Rating", ARdata, Color.Red, SymbolType.Circle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.Blue);
            curve.Symbol.Size = 7;
            curve.IsY2Axis = true;

            // Add a curve
            curve = myPane.AddCurve("Average New Rating", ANRdata, Color.Green, SymbolType.Circle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.Blue);
            curve.Symbol.Size = 7;
            curve.IsY2Axis = true;

            // Add a curve
            curve = myPane.AddCurve("Raters", Rdata, Color.Blue, SymbolType.Triangle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.Red);
            curve.Symbol.Size = 7;
            // Add a curve
            curve = myPane.AddCurve("Views", Vdata, Color.Yellow, SymbolType.Triangle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.Red);
            curve.Symbol.Size = 7;
            // Add a curve
            curve = myPane.AddCurve("Comments", Cdata, Color.Magenta, SymbolType.Triangle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.Red);
            curve.Symbol.Size = 7;
            // Add a curve
            curve = myPane.AddCurve("Favorites", Fdata, Color.Black, SymbolType.Triangle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.Red);
            curve.Symbol.Size = 7;

            // Fill the axis background with a gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, Color.ForestGreen), 45.0F);

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.Text = "Numbers (Views, Comments, Raters, Favs)";
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;

            // Enable the Y2 axis display
            myPane.Y2Axis.IsVisible = true;
            // Make the Y2 axis scale blue
            myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Blue;
            myPane.Y2Axis.Title.FontSpec.FontColor = Color.Blue;
            myPane.Y2Axis.Title.Text = "Rating [1-5] (Avg, ANR)";
            // turn off the opposite tics so the Y2 tics don't show up on the Y axis
            myPane.Y2Axis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MinorTic.IsOpposite = false;
            // Display the Y2 axis grid lines
            myPane.Y2Axis.MajorGrid.IsVisible = true;
            // Align the Y2 axis labels so they are flush to the axis
            myPane.Y2Axis.Scale.Align = AlignP.Inside;
            myPane.Y2Axis.Scale.Min = 1;
            myPane.Y2Axis.Scale.Max = 5;

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
            zgc.Refresh();
            zgc.Update();
            this.Width = this.Width + 1;
            this.Width = this.Width - 1;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            _monger = null;
            string log_data = null;
            clsFileLogger fl = null;
            openDialog.InitialDirectory = Application.StartupPath;
            openDialog.Multiselect = false;
            openDialog.CheckFileExists = true;
            openDialog.DefaultExt = "All files (*.*)|*.*|TubeGuardian Log (*.log)|*.log";
            openDialog.FileName = "CollectorData.log";
            openDialog.Title = "Load a TubeGuardian Data Log";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                fl = new clsFileLogger(openDialog.FileName);
                log_data = fl.readFile();
            }
            if (log_data == null || log_data == string.Empty)
                return;

            _monger = new clsStatMonger(log_data);
            refresh_form();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbVideo_SelectedIndexChanged(object sender, EventArgs e)
        {
            redraw_chart();
        }

        private void cbField_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddData_Click(object sender, EventArgs e)
        {
            string log_data = null;
            clsFileLogger fl = null;
            openDialog.InitialDirectory = Application.StartupPath;
            openDialog.Multiselect = false;
            openDialog.CheckFileExists = true;
            openDialog.DefaultExt = "All files (*.*)|*.*|TubeGuardian Log (*.log)|*.log";
            openDialog.FileName = "CollectorData.log";
            openDialog.Title = "Load a TubeGuardian Data Log";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                fl = new clsFileLogger(openDialog.FileName);
                log_data = fl.readFile();
            }
            if (log_data == null || log_data == string.Empty)
                return;

            clsStatMonger new_monger = new clsStatMonger(log_data);
            _monger.AddDataset(new_monger.InitialDataSet, new_monger.HistoricalDataPoints);
            new_monger = null;
            refresh_form();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            clsFileLogger fl;
            saveFile.CheckPathExists = true;
            saveFile.InitialDirectory = Application.StartupPath;
            saveFile.DefaultExt = "All files (*.*)|*.*|TubeGuardian Log (*.log)|*.log";
            saveFile.FileName = "Stats.log";

            if (saveFile.ShowDialog() == DialogResult.OK)
                fl = new clsFileLogger(saveFile.FileName);
            else
                return;

            foreach (clsVideoEntry ve in _monger.InitialDataSet)
                fl.appendFile(ve.ToString());
            foreach (KeyValuePair<string, List<clsDataPoint>> kvp in _monger.HistoricalDataPoints)
                foreach (clsDataPoint dp in kvp.Value)
                    fl.appendFile(dp.ToString());

            MessageBox.Show("File save successfully.", "TubeGuardianDeux");
        }
    }
}
