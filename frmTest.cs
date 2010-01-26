using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using mshtml;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace TubeGuardian
{
    public partial class frmTest : Form
    {
        private clsStatMonger sm;
        private clsStatMasher mash;


        public frmTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lb3.Items.Clear();
            clsStatMasher masher = new clsStatMasher(sm.InitialDataSet, sm.HistoricalDataPoints);
            foreach (clsVideoEntry entry in masher.InitialDataSet)
            {
                lb3.Items.Add(entry.VideoID + " - " + masher.AverageNewRatingByID(entry.VideoID).ToString() + " - " + masher.AverageNewRatingByID(entry.VideoID, 1).ToString());
            }
        }

        void _reader_OnTotalProgress(object Sender, int Current, int Total)
        {
            pb2.Maximum = Total;
            pb2.Value = Current;
        }

        void _reader_OnStatusChange(object Sender, clsVideoFeedReader.enumFeedReaderState NewState)
        {
            lb.Items.Add(NewState.ToString());
        }

        void _reader_OnQueryRetry(object Sender, Exception e)
        {
            lb.Items.Add("Query retry because: " + e.Message.ToString());
        }

        void _reader_OnException(object Sender, Exception e)
        {
            lb.Items.Add("Exception: " + e.Message.ToString());
        }

        void _reader_OnEntryFetched(object Sender, clsVideoEntry Entry)
        {
            lb2.Items.Add(Entry.Title);
        }

        private class test1
        {
            public int num = 0;
        }
        private class test2
        {
            public test1 test_nam;
        }
        private void frmTest_Load(object sender, EventArgs e)
        {
            test1 o1 = new test1();
            o1.num = 50;
            test2 o2 = new test2();
            o2.test_nam = o1;

            o1.num = 10;
            //MessageBox.Show(o2.test_nam.num.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        delegate void GenericDelegate();
        void sm_OnEntryUpdated(object Sender, clsDataPoint DataPoint, clsVideoEntry Entry)
        {
            if (this.InvokeRequired)
            {
                GenericDelegate dlg = delegate() { lb2.Items.Add(Entry.Title + ", " + DataPoint.Field.ToString() + ", " + DataPoint.Old.ToString() + " => " + DataPoint.New.ToString() ); };
                lb2.Invoke(dlg);
            }
            else
                lb2.Items.Add(DataPoint.Field.ToString());
        }

        void sm_OnEntryAdded(object Sender, clsVideoEntry Entry)
        {
            lb.Items.Add(Entry.Title);
        }

        private void do_stuff()
        {
            StreamReader stream = null;
            Stream post_stream = null;

            string url = "http://www.youtube.com/login?next=/";

            CookieContainer _cookie_jar = new CookieContainer();
            HttpWebResponse _response = null;
            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(url);

            _request.CookieContainer = _cookie_jar;
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.13) Gecko/2009073022 Firefox/3.0.13 (.NET CLR 3.5.30729)";
            _request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _request.KeepAlive = false;
            _request.Method = "GET";

            _response = (HttpWebResponse)_request.GetResponse();
            stream = new StreamReader(_response.GetResponseStream(), System.Text.Encoding.ASCII);
            string result = stream.ReadToEnd();
            stream.Close();

            debug_cookies(_cookie_jar, _request);
            debug_cookies(_cookie_jar, _response);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(get_post_from_form(result));

            url = "https://www.google.com/accounts/ServiceLoginAuth?service=youtube";
            _request = (HttpWebRequest)WebRequest.Create(url);

            _request.ContentLength = data.Length;
            _request.CookieContainer = _cookie_jar;
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.13) Gecko/2009073022 Firefox/3.0.13 (.NET CLR 3.5.30729)";
            _request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _request.KeepAlive = false;
            _request.ContentType = "application/x-www-form-urlencoded";
            _request.Method = "POST";
            post_stream = _request.GetRequestStream();
            post_stream.Write(data, 0, data.Length);
            post_stream.Close();

            _response = (HttpWebResponse)_request.GetResponse();

            stream = new StreamReader(_response.GetResponseStream(), System.Text.Encoding.ASCII);
            result = stream.ReadToEnd();
            stream.Close();

            // IHTMLDocument3 domDocument = getDocumentFromHTML(result) as IHTMLDocument3;

            debug_cookies(_cookie_jar, _request);
            debug_cookies(_cookie_jar, _response);

            url = get_redirect_url(result);
            if (url == null)
                return;

            _request = (HttpWebRequest)WebRequest.Create(url);

            _request.CookieContainer = _cookie_jar;
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.13) Gecko/2009073022 Firefox/3.0.13 (.NET CLR 3.5.30729)";
            _request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _request.KeepAlive = false;
            _request.Method = "GET";

            _response = (HttpWebResponse)_request.GetResponse();
            //stream = new StreamReader(_response.GetResponseStream(), System.Text.Encoding.ASCII);
            //result = stream.ReadToEnd();
            //stream.Close();

            // domDocument = getDocumentFromHTML(result) as IHTMLDocument3;

            debug_cookies(_cookie_jar, _request);
            debug_cookies(_cookie_jar, _response);
            if (true) ;
        }
        private void debug_cookies(CookieContainer _cookie_jar, HttpWebRequest _request)
        {

            foreach (Cookie c in _cookie_jar.GetCookies(_request.RequestUri))
            {
                Console.WriteLine("Cookie['" + c.Name + "']: " + c.Value);
                //if (c.Name == "LOGIN_INFO")
                    //MessageBox.Show("FUCK YEA!");
            }
        }
        private void debug_cookies(CookieContainer _cookie_jar, HttpWebResponse _request)
        {

            foreach (Cookie c in _cookie_jar.GetCookies(_request.ResponseUri))
            {
                Console.WriteLine("Cookie['" + c.Name + "']: " + c.Value);
            }
        }
        private string get_post_from_form(string html)
        {
            IHTMLDocument3 domDocument = getDocumentFromHTML(html) as IHTMLDocument3;
            IHTMLFormElement _form = (IHTMLFormElement)domDocument.getElementById("gaia_loginform");

            if (_form == null)
                return null;

            string post_data = string.Empty;

            IHTMLInputElement _continue = GetElementByName(domDocument, "continue") as IHTMLInputElement;
            IHTMLInputElement _service = GetElementByName(domDocument, "service") as IHTMLInputElement;
            IHTMLInputElement _GALX = GetElementByName(domDocument, "GALX") as IHTMLInputElement;
            IHTMLInputElement _rmShown = GetElementByName(domDocument, "rmShown") as IHTMLInputElement;
            IHTMLInputElement _asts = GetElementByName(domDocument, "asts") as IHTMLInputElement;
            IHTMLInputElement _ltmpl = GetElementByName(domDocument, "ltmpl") as IHTMLInputElement;
            IHTMLInputElement _uilel = GetElementByName(domDocument, "uilel") as IHTMLInputElement;
            IHTMLInputElement _hl = GetElementByName(domDocument, "hl") as IHTMLInputElement;
            IHTMLInputElement _ltmpl2 = GetElementByName(domDocument, "ltmpl") as IHTMLInputElement;

            post_data = "Email=" + textBox1.Text + "&Passwd=" + textBox2.Text + "&PersistentCookie=yes&signIn=Sign+in";
            post_data += "&" + _continue.name + "=" + _continue.value;
            post_data += "&" + _service.name + "=" + _service.value;
            post_data += "&" + _GALX.name + "=" + _GALX.value;
            post_data += "&" + _rmShown.name + "=" + _rmShown.value;
            post_data += "&" + _asts.name + "=" + _asts.value;
            post_data += "&" + _ltmpl.name + "=" + _ltmpl.value;
            post_data += "&" + _uilel.name + "=" + _uilel.value;
            post_data += "&" + _hl.name + "=" + _hl.value;
            post_data += "&" + _ltmpl2.name + "=" + _ltmpl2.value;

            return post_data;
        }
        private string get_redirect_url(string html)
        {
            string search_right = "'";
            string search_left = "url='";
            int redir_start = html.IndexOf(search_left);
            if (redir_start < 0)
            {
                redir_start = html.IndexOf("url=&#39;");
                if (redir_start > 0) {
                    search_right = "&#39;";
                    search_left = "url=&#39;";
                }
            }
            if (redir_start >= 0)
            {
                redir_start += search_left.Length;
                int redir_end = html.IndexOf(search_right, redir_start);
                if (redir_end != -1)
                {
                    return html.Substring(redir_start, redir_end - redir_start);
                }
            }
            return null;
        }

        private IHTMLElement GetElementByName(IHTMLDocument3 doc, string name) 
        {
            IHTMLElementCollection elements = doc.getElementsByName(name);
            foreach (IHTMLElement element in elements)
                return element;
            return null;
        }
        private HTMLDocumentClass getDocumentFromHTML(string html)
        {
            object[] oPageText = { html };
            HTMLDocumentClass myDoc = new HTMLDocumentClass();
            IHTMLDocument2 oMyDoc = (IHTMLDocument2)myDoc;
            oMyDoc.write(oPageText);
            oMyDoc.close();
            return oMyDoc as HTMLDocumentClass;
        }

        private clsSettingsManager set;
        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            set.DisableVideo("EkgDZKnxyPI");
        }

        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        // This method requests the home page content for the specified server.
        private static string SocketSendReceive(string server, int port, string request)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];

            // Create a socket connection with the specified server and port.
            Socket s = ConnectSocket(server, port);

            if (s == null)
                return ("Connection failed");

            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);

            // Receive the server home page content.
            int bytes = 0;
            string page = "Default HTML page on " + server + ":\r\n";

            // The following will block until te page is transmitted.
            do
            {
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            return page;
        }

        private void frmTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<clsVideoEntry> init = new List<clsVideoEntry>();

            string log = textBox3.Text;
            Regex r = new Regex("V=(.*),Views=(.*),Comments=(.*),Favorites=(.*),Ratings=(.*),ARating=(.*)");
            MatchCollection matches = r.Matches(textBox3.Text);
            foreach (Match m in matches)
            {
                Google.GData.YouTube.YouTubeEntry entry = new Google.GData.YouTube.YouTubeEntry();
                entry.VideoId = m.Groups[1].Value;
                Google.GData.Extensions.FeedLink feedlink = new Google.GData.Extensions.FeedLink();
                feedlink.CountHint = int.Parse(m.Groups[3].Value);
                entry.Comments = new Google.GData.Extensions.Comments();
                entry.Comments.FeedLink = feedlink;
                entry.Statistics = new Google.GData.YouTube.Statistics();
                entry.Statistics.ViewCount = m.Groups[2].Value;
                entry.Statistics.FavoriteCount = m.Groups[4].Value;
                entry.Rating = new Google.GData.Extensions.Rating();
                entry.Rating.NumRaters = int.Parse(m.Groups[5].Value);
                entry.Rating.Average = double.Parse(m.Groups[6].Value);
                init.Add(new clsVideoEntry(entry));
            }

            Dictionary<string, List<clsDataPoint>> hist = new Dictionary<string, List<clsDataPoint>>();
            r = new Regex("DT=.*V=(.*),field=(.*),init_v=(.*),new_v=(.*)");
            matches = r.Matches(textBox3.Text);
            foreach (Match m in matches)
            {
                string v = m.Groups[1].Value;
                string f = m.Groups[2].Value;
                double Iv = double.Parse(m.Groups[3].Value);
                double Nv = double.Parse(m.Groups[4].Value);
                if (!hist.ContainsKey(v))
                    hist.Add(v, new List<clsDataPoint>());

                clsDataPointField field = new clsDataPointField();
                switch (f)
                {
                    case "E_VIEWS":
                        field.Field = clsDataPointField.VideoDataFields.VIEWS;
                        break;
                    case "E_RATINGS":
                        field.Field = clsDataPointField.VideoDataFields.RATERS;
                        break;
                    case "E_AVERAGE_RATING":
                        field.Field = clsDataPointField.VideoDataFields.AVERAGE_RATING;
                        break;
                    case "E_COMMENTS":
                        field.Field = clsDataPointField.VideoDataFields.COMMENT_COUNT;
                        break;
                    case "E_FAVORITES":
                        field.Field = clsDataPointField.VideoDataFields.FAVORITED_COUNT;
                        break;
                }
                hist[v].Add(new clsDataPoint(Iv, Nv, field, v));
            }

            lb.Items.Clear();
            lb3.Items.Clear();

            mash = new clsStatMasher(init, hist);

            foreach (clsVideoEntry en in init)
            {
                lb.Items.Add(en.VideoID);
                lb3.Items.Add(mash.AverageNewRatingByID(en.VideoID, 10));
            }
        }

        private void lb_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }
        
    }
}
