using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.YouTube;
using Google.GData.Extensions;
using Google.YouTube;
using System.Threading;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using System.Web;
using mshtml;
using System.Text.RegularExpressions;

namespace TubeGuardian
{
    // Hack-a-licious http 1.1 request using tcp sockets
    public class GetSocket
    {
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
        public static string SocketSendReceive(string server, int port, string headers, string data)
        {
            Byte[] bytesData = Encoding.ASCII.GetBytes(data);
            headers = headers + "\nContent-Length: " + bytesData.Length;

            string request = headers + "\n\n" + data;
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
            string page = "";

            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);

            return page;
        }
    }

    /* Bot-ish class that contantly maintains an updated dictionary of clsDataPoint objects */
    public class clsStatMonger
    {
        // private data members
        private List<clsCredentials> _accounts = new List<clsCredentials>();
        private List<clsVideoFeedReader> _feed_readers = new List<clsVideoFeedReader>();
        private List<clsVideoEntry> _initial_dataset = new List<clsVideoEntry>();
        private List<clsVideoEntry> _current_dataset = new List<clsVideoEntry>();
        private Dictionary<string, List<clsDataPoint>> _historical_data = new Dictionary<string, List<clsDataPoint>>();
        private System.Timers.Timer _update_timer;
        private clsFileLogger _file_logger = null;
        private string _dev_key = string.Empty;
        private string _app_name = string.Empty;
        private clsSettings _settings;

        // constructor
        public clsStatMonger(string DeveloperKey, string ApplicationName, List<clsCredentials> Credentials, clsSettings settings)
        {
            _file_logger = new clsFileLogger("CollectorData.log");
            _settings = settings;
            _settings.OnAccountAdded += new clsSettings.AccountAddedHandler(_settings_OnAccountAdded);
            _settings.OnAccountRemoved += new clsSettings.AccountRemovedHandler(_settings_OnAccountRemoved);
            
            _dev_key = DeveloperKey;
            _app_name = ApplicationName;

            this.Enabled = false;

            _update_timer = new System.Timers.Timer();
            _update_timer.Enabled = false;
            _update_timer.Interval = _settings.Collect_Interval * 1000 * 60;
            _update_timer.Elapsed += new ElapsedEventHandler(_update_timer_Elapsed);

            _accounts = Credentials;
            foreach (clsCredentials c in _accounts)
            {
                clsVideoFeedReader new_feed = new clsVideoFeedReader(DeveloperKey, ApplicationName, c.Username);
                if (c.Password != string.Empty && c.Password != "-")
                    new_feed.SetCredentials(c.Username, c.Password);
                new_feed.OnEntryFetched += new clsVideoFeedReader.EntryFetchedHandler(new_feed_OnEntryFetched);
                new_feed.OnStatusChange += new clsVideoFeedReader.StatusChangeHandler(new_feed_OnStatusChange);
                _feed_readers.Add(new_feed);
            }
        }
        public clsStatMonger(string DataFile)
        {
            LoadDataFile(DataFile);
        }

        void _settings_OnAccountRemoved(object sender, clsCredentials Account)
        {
            RemoveAccount(Account);
        }
        void _settings_OnAccountAdded(object sender, clsCredentials Account)
        {
            AddAccount(Account);
        }

        // public methods
        public void Enable()
        {
            if (this.Enabled)
                return;
            this.Enabled = true;
            _update_videos();
            _update_timer.Interval = _settings.Collect_Interval;
            _update_timer.Enabled = true;
        }
        public void Disable()
        {
            this.Enabled = false;
            _update_timer.Enabled = false;
        }
        public void Update()
        {
            _update_videos();
        }
        public void AddAccount(clsCredentials Account) 
        {
            foreach (clsVideoFeedReader r in _feed_readers)
                if (r.Username == Account.Username)
                    return;

            clsVideoFeedReader new_feed = new clsVideoFeedReader(_dev_key, _app_name, Account.Username);
            if (Account.Password != "-")
                new_feed.SetCredentials(Account.Username, Account.Password);

            new_feed.OnEntryFetched += new clsVideoFeedReader.EntryFetchedHandler(new_feed_OnEntryFetched);
            new_feed.OnStatusChange += new clsVideoFeedReader.StatusChangeHandler(new_feed_OnStatusChange);
            new_feed.OnException += new clsVideoFeedReader.ExceptionHandler(new_feed_OnException);
            _feed_readers.Add(new_feed);
        }
        public void RemoveAccount(clsCredentials Account) 
        {
            int i = 0;
            while (i < _feed_readers.Count)
            {
                if (_feed_readers[i].Username == Account.Username)
                {
                    _feed_readers[i].Dispose();
                    _feed_readers.RemoveAt(i);
                }
                else
                    i++;
            }
        }
        public void AddDataset(List<clsVideoEntry> InitialDataset, Dictionary<string, List<clsDataPoint>> HistoricalData)
        {
            List<clsDataPoint> new_datapoints = new List<clsDataPoint>();
            int i = 0;
            while (i<InitialDataset.Count)
            {
                clsVideoEntry new_entry = InitialDataset[i];
                i++;
                if (new_entry == null)
                    continue;
                clsVideoEntry old_entry = _GetEntryByIdFromList(_initial_dataset, new_entry.VideoID);
                if (old_entry == null)
                {
                    _initial_dataset.Add(new_entry);
                    try { _historical_data.Add(new_entry.VideoID, new List<clsDataPoint>()); }
                    catch { }
                    continue;
                }
                List<clsDataPoint> new_dps = _compare_entities(new_entry, old_entry);
                if (new_entry.Time < old_entry.Time)
                    old_entry = new_entry;
                foreach (clsDataPoint dp in new_dps)
                    _historical_data[new_entry.VideoID].Add(new clsDataPoint(dp));
                
            }
            foreach (KeyValuePair<string, List<clsDataPoint>> kvp in HistoricalDataPoints)
            {
                if (!_historical_data.ContainsKey(kvp.Key))
                {
                    _historical_data.Add(kvp.Key, new List<clsDataPoint>(kvp.Value));
                    continue;
                }
                _historical_data[kvp.Key].AddRange(new List<clsDataPoint>(kvp.Value));
            }
            _sort_historical_data();
        }

        public void LoadDataFile(string LogFileContents)
        {
            List<clsVideoEntry> init = new List<clsVideoEntry>();
            Dictionary<string, List<clsDataPoint>> hist = new Dictionary<string, List<clsDataPoint>>();

            string log = LogFileContents;
            Regex r = new Regex("init : .+?{(.+?)}.+?{(.+?)}.+?{(.+?)}.+?{(.+?)}.+?{(.+?)}.+?{(.+?)}.+?{(.+?)}.+?{(.+?)}.+?{(.+?)}");
            MatchCollection matches = r.Matches(log);
            foreach (Match m in matches)
            {
                Google.GData.YouTube.YouTubeEntry entry = new Google.GData.YouTube.YouTubeEntry();
                entry.VideoId = m.Groups[3].Value;
                Google.GData.Extensions.FeedLink feedlink = new Google.GData.Extensions.FeedLink();
                feedlink.CountHint = int.Parse(m.Groups[8].Value);
                entry.Comments = new Google.GData.Extensions.Comments();
                entry.Comments.FeedLink = feedlink;
                entry.Statistics = new Google.GData.YouTube.Statistics();
                entry.Statistics.ViewCount = m.Groups[7].Value;
                entry.Statistics.FavoriteCount = m.Groups[9].Value;
                entry.Rating = new Google.GData.Extensions.Rating();
                entry.Rating.NumRaters = int.Parse(m.Groups[5].Value);
                entry.Rating.Average = double.Parse(m.Groups[6].Value);
                entry.Title = new Google.GData.Client.AtomTextConstruct(Google.GData.Client.AtomTextConstructElementType.Title, m.Groups[4].Value);
                clsVideoEntry new_e = new clsVideoEntry(entry);
                new_e.Time = DateTime.Parse(m.Groups[1].Value + " " + m.Groups[2].Value);
                clsVideoEntry old_entry = _GetEntryByIdFromList(init, new_e.VideoID);
                if ( old_entry == null)
                    init.Add(new_e);
                else
                {
                    List<clsDataPoint> new_dps = _compare_entities(new_e, old_entry);
                    if (new_e.Time < old_entry.Time)
                        old_entry = new_e;
                    foreach (clsDataPoint dp in new_dps)
                    {
                        if (!hist.ContainsKey(new_e.VideoID))
                            hist.Add(new_e.VideoID, new List<clsDataPoint>());
                        hist[new_e.VideoID].Add(dp);
                    }
                }

            }

            r = new Regex("upd : d{(.*)},t{(.*)},vId{(.*),(.*)},old{(.*)},new{(.*)}");
            matches = r.Matches(log);
            foreach (Match m in matches)
            {
                string v = m.Groups[3].Value;
                string f = m.Groups[4].Value;
                double Iv = double.Parse(m.Groups[5].Value);
                double Nv = double.Parse(m.Groups[6].Value);
                if (!hist.ContainsKey(v))
                    hist.Add(v, new List<clsDataPoint>());

                clsDataPointField field = new clsDataPointField();
                switch (f)
                {
                    case "VIEWS":
                        field.Field = clsDataPointField.VideoDataFields.VIEWS;
                        break;
                    case "RATERS":
                        field.Field = clsDataPointField.VideoDataFields.RATERS;
                        break;
                    case "AVERAGE_RATING":
                        field.Field = clsDataPointField.VideoDataFields.AVERAGE_RATING;
                        break;
                    case "COMMENT_COUNT":
                        field.Field = clsDataPointField.VideoDataFields.COMMENT_COUNT;
                        break;
                    case "FAVORITED_COUNT":
                        field.Field = clsDataPointField.VideoDataFields.FAVORITED_COUNT;
                        break;
                }
                clsDataPoint new_dp = new clsDataPoint(Iv, Nv, field, v);
                new_dp.Time = DateTime.Parse(m.Groups[1].Value + " " + m.Groups[2].Value);
                hist[v].Add(new_dp);
            }

            _initial_dataset = init;
            _historical_data = hist;
            _sort_historical_data();
        }

        private void _sort_historical_data()
        {
            int i = 0;
            while (i < _historical_data.Count)
            {
                List<clsDataPoint> current_dps = _historical_data.ElementAt(i).Value;
                i++;
                current_dps.Sort(delegate(clsDataPoint d1, clsDataPoint d2)
                                 { return d1.Time.CompareTo(d2.Time); }
                                 );
                //current_dps = _remove_duplicates(current_dps);
            }
        }
        private static List<clsDataPoint> _remove_duplicates(List<clsDataPoint> input)
        {
            List<clsDataPoint> unique = new List<clsDataPoint>();
            foreach (clsDataPoint dp in input)
                if (unique.FindIndex(delegate(clsDataPoint d) { return ((dp.VideoID.Equals(d.VideoID)) && (dp.Time.Ticks.Equals(d.Time.Ticks)) && (dp.Field.Field.Equals(d.Field.Field)) && (dp.New.Equals(d.New)) && (dp.Old.Equals(d.Old))); }) == -1)
                    unique.Add(dp);
            return unique;
        }
        private static List<clsDataPoint> _compare_entities(clsVideoEntry e_adding, clsVideoEntry e_init)
        {
            clsDataPoint new_dp = null;
            List<clsDataPoint> ret = new List<clsDataPoint>();

            if (e_adding.Time < e_init.Time)
            {
                if (e_adding.ViewsCount != e_adding.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_adding.ViewsCount, (double)e_init.ViewsCount, new clsDataPointField(clsDataPointField.VideoDataFields.VIEWS), e_init.VideoID);
                    new_dp.Time = e_init.Time;
                    ret.Add(new_dp);
                }
                if (e_adding.ViewsCount != e_adding.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_adding.Raters, (double)e_init.Raters, new clsDataPointField(clsDataPointField.VideoDataFields.RATERS), e_init.VideoID);
                    new_dp.Time = e_init.Time;
                    ret.Add(new_dp);
                }
                if (e_adding.ViewsCount != e_adding.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_adding.FavoritedCount, (double)e_init.FavoritedCount, new clsDataPointField(clsDataPointField.VideoDataFields.FAVORITED_COUNT), e_init.VideoID);
                    new_dp.Time = e_init.Time;
                    ret.Add(new_dp);
                }
                if (e_adding.ViewsCount != e_adding.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_adding.CommentCount, (double)e_init.CommentCount, new clsDataPointField(clsDataPointField.VideoDataFields.COMMENT_COUNT), e_init.VideoID);
                    new_dp.Time = e_init.Time;
                    ret.Add(new_dp);
                }
                if (e_adding.ViewsCount != e_adding.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_adding.AverageRating, (double)e_init.AverageRating, new clsDataPointField(clsDataPointField.VideoDataFields.AVERAGE_RATING), e_init.VideoID);
                    new_dp.Time = e_init.Time;
                    ret.Add(new_dp);
                }
                e_init = e_adding;
            }
            else
            {
                if (e_init.ViewsCount != e_init.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_init.ViewsCount, (double)e_adding.ViewsCount, new clsDataPointField(clsDataPointField.VideoDataFields.VIEWS), e_adding.VideoID);
                    new_dp.Time = e_adding.Time;
                    ret.Add(new_dp);
                }
                if (e_init.ViewsCount != e_init.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_init.Raters, (double)e_adding.Raters, new clsDataPointField(clsDataPointField.VideoDataFields.RATERS), e_adding.VideoID);
                    new_dp.Time = e_adding.Time;
                    ret.Add(new_dp);
                }
                if (e_init.ViewsCount != e_init.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_init.FavoritedCount, (double)e_adding.FavoritedCount, new clsDataPointField(clsDataPointField.VideoDataFields.FAVORITED_COUNT), e_adding.VideoID);
                    new_dp.Time = e_adding.Time;
                    ret.Add(new_dp);
                }
                if (e_init.ViewsCount != e_init.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_init.CommentCount, (double)e_adding.CommentCount, new clsDataPointField(clsDataPointField.VideoDataFields.COMMENT_COUNT), e_adding.VideoID);
                    new_dp.Time = e_adding.Time;
                    ret.Add(new_dp);
                }
                if (e_init.ViewsCount != e_init.ViewsCount)
                {
                    new_dp = new clsDataPoint((double)e_init.AverageRating, (double)e_adding.AverageRating, new clsDataPointField(clsDataPointField.VideoDataFields.AVERAGE_RATING), e_adding.VideoID);
                    new_dp.Time = e_adding.Time;
                    ret.Add(new_dp);
                }
            }
            return ret;

        }
        private YouTubeEntry _new_youtube_entry(string title, double views, double raters, double favs, double avgR, double comments)
        {
            YouTubeEntry e = new YouTubeEntry();
            e.Rating = new Rating();
            e.Comments.FeedLink = new FeedLink();
            e.Statistics = new Statistics();
            e.Title = new Google.GData.Client.AtomTextConstruct();

            e.Statistics.FavoriteCount = favs.ToString();
            e.Statistics.ViewCount = views.ToString();
            e.Rating.Average = avgR;
            e.Rating.NumRaters = (int)raters;
            e.Comments.FeedLink.CountHint = (int)comments;
            e.Title.Text = title;
            return e;
        }

        // public events
        public delegate void EntryAddedEventHandler(object Sender, clsVideoEntry Entry);
        public event EntryAddedEventHandler OnEntryAdded;
        private void _entry_added(clsVideoEntry Entry)
        {
            if (_file_logger != null)
                _file_logger.appendFile(Entry.ToString());
            if (OnEntryAdded != null)
                OnEntryAdded(this, Entry);
        }

        public delegate void EntryUpdatedEventHandler(object Sender, clsDataPoint DataPoint, clsVideoEntry Entry);
        public event EntryUpdatedEventHandler OnEntryUpdated;
        private void _entry_updated(clsDataPoint DataPoint, clsVideoEntry Entry)
        {
            if (_file_logger != null)
                _file_logger.appendFile(DataPoint.ToString());
            if (OnEntryUpdated != null)
                OnEntryUpdated(this, DataPoint, Entry);
        }

        public delegate void FeedReaderUpdatedEventHandler(object Sender, clsVideoFeedReader.enumFeedReaderState status);
        public event FeedReaderUpdatedEventHandler OnFeedReaderUpdated;
        private void _feed_reader_updated(object sender, clsVideoFeedReader.enumFeedReaderState status)
        {
            if (OnFeedReaderUpdated != null)
                OnFeedReaderUpdated(sender, status);
        }

        public delegate void FeedReaderExceptionEventHandler(object Sender, string exception);
        public event FeedReaderExceptionEventHandler OnFeedReaderException;
        private void _feed_reader_exception(object sender, string exception)
        {
            if (OnFeedReaderException != null)
                OnFeedReaderException(sender, exception);
        }

        // private methods
        private void _update_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.Enabled == false)
                return;
            _update_timer.Enabled = false;
            _update_timer.Interval = _settings.Collect_Interval * 1000 * 60;
            _update_timer.Enabled = true;
            _update_videos();
        }
        private void new_feed_OnStatusChange(object Sender, clsVideoFeedReader.enumFeedReaderState NewState)
        {
            _feed_reader_updated(Sender, NewState);
        }
        private void new_feed_OnEntryFetched(object Sender, clsVideoEntry Entry)
        {
            clsVideoEntry initial_entry;
            if ((initial_entry = _GetEntryByIdFromList(_initial_dataset, Entry.VideoID)) == null)
            {
                _initial_dataset.Add(Entry);
                _current_dataset.Add(Entry);
                _entry_added(Entry);
            }
            else
            {
                clsVideoEntry CurrentEntry = _GetEntryByIdFromList(_current_dataset, Entry.VideoID);
                if (CurrentEntry == null)
                {
                    _current_dataset.Add(Entry);
                    _compare_entries(initial_entry, Entry);
                }
                else
                {
                    _current_dataset.Remove(CurrentEntry);
                    _current_dataset.Add(Entry);
                    _compare_entries(CurrentEntry, Entry);
                }
            }
        }
        private void new_feed_OnException(object Sender, Exception e)
        {
            _feed_reader_exception(Sender, e.Message);
        }
        private void _compare_entries(clsVideoEntry OldEntry, clsVideoEntry NewEntry)
        {
            if (OldEntry.VideoID != NewEntry.VideoID)
                return;

            _compare_stat(OldEntry.AverageRating, NewEntry.AverageRating, clsDataPointField.VideoDataFields.AVERAGE_RATING, NewEntry);
            _compare_stat(OldEntry.CommentCount, NewEntry.CommentCount, clsDataPointField.VideoDataFields.COMMENT_COUNT, NewEntry);
            _compare_stat(OldEntry.FavoritedCount, NewEntry.FavoritedCount, clsDataPointField.VideoDataFields.FAVORITED_COUNT, NewEntry);
            _compare_stat(OldEntry.Raters, NewEntry.Raters, clsDataPointField.VideoDataFields.RATERS, NewEntry);
            _compare_stat(OldEntry.ViewsCount, NewEntry.ViewsCount, clsDataPointField.VideoDataFields.VIEWS, NewEntry);

        }
        private void _compare_stat(double Old, double New, clsDataPointField.VideoDataFields Field, clsVideoEntry Entry)
        {
            if (Old == New)
                return;
            else
            {
                clsDataPoint new_datapoint = new clsDataPoint(Old, New, new clsDataPointField(Field), Entry.VideoID);
                if (_historical_data.ContainsKey(Entry.VideoID))
                    _historical_data[Entry.VideoID].Add(new_datapoint);
                else
                {
                    List<clsDataPoint> new_dp_list = new List<clsDataPoint>();
                    new_dp_list.Add(new_datapoint);
                    _historical_data.Add(Entry.VideoID, new_dp_list);
                }
                _entry_updated(new_datapoint, Entry);
            }
        }
        private clsVideoEntry _GetEntryByIdFromList(List<clsVideoEntry> Entries, string VideoID)
        {
            int index = Entries.FindIndex(delegate(clsVideoEntry e) { return e.VideoID.Equals(VideoID); });
            return (index == -1) ? null : Entries[index];
        }
        private void _update_videos()
        {
            foreach (clsVideoFeedReader r in _feed_readers)
                if (!r.IsBusy)
                    r.GetVideosModifiedSince(r.Updated);
        }

        // public properties
        public bool Enabled { get; private set; }
        public List<clsVideoEntry> CurrentDataSet
        {
            get { return new List<clsVideoEntry>(_current_dataset); }
        }
        public List<clsVideoEntry> InitialDataSet
        {
            get { return _initial_dataset; }
        }
        public Dictionary<string, List<clsDataPoint>> HistoricalDataPoints
        {
            get { return _historical_data; }
        }
        public clsFileLogger FileLogger
        {
            get { return _file_logger; }
            set { _file_logger = value; }
        }
        public List<clsVideoEntry> GetVideosByUsername(string username)
        {
            List<clsVideoEntry> ret = new List<clsVideoEntry>();
            foreach (clsVideoEntry e in _current_dataset)
                if (e.Account.Username == username)
                    ret.Add(e);
            return ret;
        }
        public List<clsVideoFeedReader> FeedReaders { get { return _feed_readers; } }
    }

    /* Type-ish class to hold youtube usernames, and passwords */
    public class clsCredentials
    {
        public clsCredentials(string Username, string Password) { this.Username = Username; this.Password = Password; }
        public clsCredentials() { }
        public string Username
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
    }
    
    /* Type-ish class to hold data points (old/new value + data field) */
    public class clsDataPoint
    {
        public clsDataPoint() 
        { 
            this.Old = this.New = 0; 
            this.Field = new clsDataPointField(); 
            this.Time = DateTime.Now; 
        }
        public clsDataPoint(double OldValue, double NewValue, clsDataPointField DataField, string VideoID)
        {
            this.Old = OldValue;
            this.New = NewValue;
            this.Field = DataField;
            this.Time = DateTime.Now;
            this.VideoID = VideoID;
        }
        public clsDataPoint(clsDataPoint dp)
        {
            this.Old = dp.Old;
            this.New = dp.New;
            this.Field = dp.Field;
            this.Time = dp.Time;
            this.VideoID = dp.VideoID;
        }
        public override string ToString()
        {
            string[] values = {"upd : d{" + Time.ToShortDateString() + "}", "t{" + Time.ToShortTimeString() + "}", "vId{" + VideoID, Field.Field.ToString() + "}", "old{" + Old.ToString() + "}", "new{" + New.ToString() + "}" }; 
            return string.Join(",", values);
        }
        
        public clsDataPointField Field { get; set; }
        public double Old { get; set; }
        public double New { get; set; }
        public double Delta { get { return New - Old; } }
        public DateTime Time { get; set; }
        public string VideoID { get; set; }
    }
    
    /* Type-ish class to hold data field values and convert to string equivalents */
    public class clsDataPointField
    {
        public enum VideoDataFields
        {
            UNKNOWN = -1,
            VIEWS,
            RATERS,
            AVERAGE_RATING,
            COMMENT_COUNT,
            FAVORITED_COUNT
        }
        public clsDataPointField() { this.Field = VideoDataFields.UNKNOWN; }
        public clsDataPointField(VideoDataFields f) { this.Field = f; }
        public VideoDataFields Field { get; set; }
        public override string ToString()
        {
            switch (this.Field)
            {
                case VideoDataFields.AVERAGE_RATING:
                    return "Average Rating";
                case VideoDataFields.COMMENT_COUNT:
                    return "Comment Count";
                case VideoDataFields.FAVORITED_COUNT:
                    return "Favorited Count";
                case VideoDataFields.RATERS:
                    return "Raters";
                case VideoDataFields.UNKNOWN:
                    return "Unknown";
                case VideoDataFields.VIEWS:
                    return "Views";
                default:
                    return "Error";
            }
        }
    }
    
    /* Wrapper class to access the information needed from YouTubeEntry objects */
    public class clsVideoEntry
    {
        private YouTubeEntry _yt_entry = null;
      
        // constructor
        public clsVideoEntry(YouTubeEntry e)
        {
            _yt_entry = e;
        }

        public override string ToString()
        {
            string[] values = { "init : t{" + Time.ToShortDateString() + "}", "d{" + Time.ToShortTimeString() + "}","vIf{" + VideoID + "}","ti{" + Title + "}", "r#{" + Raters.ToString() +"}", "ar{" + AverageRating.ToString() + "}", "v#{" + ViewsCount.ToString() + "}", "c#{" + CommentCount.ToString() + "}", "f#{" + FavoritedCount.ToString() + "}" };
            return string.Join(",", values);
        }
        public override bool Equals(object obj)
        {
            clsVideoEntry e = (clsVideoEntry)obj;
            return this.VideoID.Equals(e.VideoID);
        }
        public override int GetHashCode()
        {
            return this.Account.Username.GetHashCode();
        }
        // public properties
        public bool IsNull
        {
            get { return (_yt_entry == null); }
        }
        public int Raters
        {
            get { try { return _yt_entry.Rating.NumRaters; } catch { return 0; } }
        }
        public double AverageRating
        {
            get { try { return _yt_entry.Rating.Average; } catch { return 0; } }
        }
        public int ViewsCount
        {
            get { try { return int.Parse(_yt_entry.Statistics.ViewCount); } catch { return 0; } }
        }
        public int CommentCount
        {
            get { try { return _yt_entry.Comments.FeedLink.CountHint; } catch { return 0; } }
        }
        public int FavoritedCount
        {
            get { try { return int.Parse(_yt_entry.Statistics.FavoriteCount); } catch { return 0; } }
        }
        public string VideoID
        {
            get { try { return _yt_entry.VideoId; } catch { return string.Empty; } }
        }
        public string Title
        {
            get { try { return _yt_entry.Title.Text; } catch { return string.Empty; } }
        }
        public YouTubeEntry YouTubeEntry
        {
            get { return _yt_entry; }
        }
        public DateTime Time { get; set; }
        public clsCredentials Account { get; set; }
    }

    /* Utility class for reading video feeds asynchronously for a single account */
    public class clsVideoFeedReader
    {
        private class QueryArgs
        {
            public YouTubeService Service { get; set; }
            public YouTubeQuery Query { get; set; }
        }
        private class QueryReturn
        {
            public YouTubeFeed Feed { get; set; }
            public Exception Exception { get; set; }
            public YouTubeQuery NextQuery { get; set; }
        }
        public enum enumFeedReaderState
        {
            ERROR = -1,
            IDLE,
            GETTING_FIRST_CHUNK,
            GOT_FIRST_CHUNK,
            GETTING_ANOTHER_CHUNK,
            GOT_ANOTHER_CHUUNK
        }

        private enumFeedReaderState _state = enumFeedReaderState.IDLE;
        private YouTubeService _yt_service = null;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _dev_key = string.Empty;
        private string _app_name = string.Empty;
        private int _retrieved = 0;
        private int _max_retry_query = 3;
        private int _current_retry_query = 1;
        private System.ComponentModel.BackgroundWorker _query_thread = new System.ComponentModel.BackgroundWorker();
        private DateTime _last_updated = DateTime.MinValue;

        public clsVideoFeedReader(string DevelopersKey, string ApplicationName, string Username)
        {
            _dev_key = DevelopersKey;
            _app_name = ApplicationName;
            _username = Username;
            _yt_service = new YouTubeService(ApplicationName, string.Empty, DevelopersKey);
            _query_thread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(_query_thread_RunWorkerCompleted);
            _query_thread.DoWork += new System.ComponentModel.DoWorkEventHandler(_query_thread_DoWork);
        }
        public clsVideoFeedReader(string DevelopersKey, string ApplicationName, string Username, string Password)
            : this(DevelopersKey, ApplicationName, Username)
        {
            if (_password != "-")
            {
                _password = Password;
                SetCredentials(Username, Password);
            }
        }

        // public methods
        public void SetCredentials(string Username, string Password)
        {
            _username = Username;
            _password = Password;
            _yt_service.setUserCredentials(Username, Password);
        }
        public void GetVideos()
        {
            YouTubeQuery query = new YouTubeQuery(YouTubeQuery.CreateUserUri(_username));

            query.NumberToRetrieve = 50;
            _retrieved = 0;
            _current_retry_query = 1;
            _status_changed(enumFeedReaderState.GETTING_FIRST_CHUNK);
            _last_updated = DateTime.Now;
            _do_query(query);
        }
        public void GetVideosModifiedSince(DateTime When)
        {
            YouTubeQuery query = new YouTubeQuery(YouTubeQuery.CreateUserUri(_username));

            query.NumberToRetrieve = 50;
            query.ModifiedSince = When;
            _retrieved = 0;
            _current_retry_query = 1;
            _status_changed(enumFeedReaderState.GETTING_FIRST_CHUNK);
            _last_updated = DateTime.Now;
            _do_query(query);
        }
        public void Dispose()
        {
            _query_thread.Dispose();
            OnEntryFetched = null;
            OnException = null;
            OnProgress = null;
            OnQueryRetry = null;
            OnStatusChange = null;
        }

        // private methods
        private void _do_query(YouTubeQuery q)
        {
            if (_query_thread.IsBusy)
                return;
            QueryArgs qa = new QueryArgs();
            qa.Query = q;
            qa.Service = _yt_service;
            _query_thread.RunWorkerAsync(qa);
        }
        private void _query_thread_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            QueryArgs qa = e.Argument as QueryArgs;
            QueryReturn qr = new QueryReturn();

            qr.Exception = null;
            qr.Feed = null;

            try
            {
                YouTubeFeed feed = qa.Service.Query(qa.Query);
                qr.Feed = feed;
            }
            catch (Exception exception)
            {
                qr.Exception = exception;
                qr.NextQuery = qa.Query;
            }
            e.Result = qr;
        }
        private void _query_thread_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            QueryReturn qr = e.Result as QueryReturn;
            YouTubeFeed feed = qr.Feed;

            if (feed != null)
            {
                _retrieved += feed.Entries.Count;

                foreach (YouTubeEntry entry in feed.Entries)
                    _entry_fetched(entry);

                if (_state == enumFeedReaderState.GETTING_FIRST_CHUNK || _state == enumFeedReaderState.GETTING_ANOTHER_CHUNK)
                    _status_changed(_state + 1);

                if (feed.NextChunk != null)
                {
                    _progress(_retrieved, feed.TotalResults);
                    _status_changed(enumFeedReaderState.GETTING_ANOTHER_CHUNK);
                    _do_query(new YouTubeQuery(feed.NextChunk));
                }
                else
                {
                    _progress(1, 1);
                    _status_changed(enumFeedReaderState.IDLE);
                }
            }
            else
            {
                _exception(qr.Exception);
                if (_current_retry_query < _max_retry_query)
                {
                    _current_retry_query++;
                    if (qr.NextQuery != null)
                    {
                        _status_changed(_state);
                        _do_query(new YouTubeQuery(qr.NextQuery.Uri.AbsoluteUri));
                    }
                    else
                    {
                        _progress(1, 1);
                        _status_changed(enumFeedReaderState.ERROR);
                    }
                }
                else
                {
                    _progress(1, 1);
                    _status_changed(enumFeedReaderState.ERROR);
                }
            }

        }

        // public events
        public delegate void StatusChangeHandler(object Sender, enumFeedReaderState NewState);
        public event StatusChangeHandler OnStatusChange;
        private void _status_changed(enumFeedReaderState state)
        {
            _state = state;
            if (OnStatusChange != null)
                OnStatusChange(this, state);
        }

        public delegate void EntryFetchedHandler(object Sender, clsVideoEntry Entry);
        public event EntryFetchedHandler OnEntryFetched;
        private void _entry_fetched(YouTubeEntry Entry)
        {
            if (OnEntryFetched != null)
            {
                clsVideoEntry nEntry = new clsVideoEntry(Entry);
                nEntry.Time = DateTime.Now;
                nEntry.Account = new clsCredentials(_username, _password);
                OnEntryFetched(this, nEntry);
            }
        }

        public delegate void ProgressHandler(object Sender, int Current, int Total);
        public event ProgressHandler OnProgress;
        private void _progress(int Current, int Total)
        {
            if (OnProgress != null)
                OnProgress(this, Current, Total);
        }

        public delegate void QueryRetryHandler(object Sender, Exception e);
        public event QueryRetryHandler OnQueryRetry;
        private void _query_retry(Exception e)
        {
            if (OnQueryRetry != null)
                OnQueryRetry(this, e);
        }

        public delegate void ExceptionHandler(object Sender, Exception e);
        public event ExceptionHandler OnException;
        private void _exception(Exception e)
        {
            if (OnException != null)
                OnException(this, e);
        }

        // public properties
        public string StateString
        {
            get
            {
                switch (_state)
                {
                    case enumFeedReaderState.ERROR:
                        return "Error";
                    case enumFeedReaderState.GETTING_ANOTHER_CHUNK:
                        return "Getting another chunk";
                    case enumFeedReaderState.GETTING_FIRST_CHUNK:
                        return "Getting first chunk";
                    case enumFeedReaderState.GOT_ANOTHER_CHUUNK:
                        return "Got another chunk";
                    case enumFeedReaderState.GOT_FIRST_CHUNK:
                        return "Got first chunk";
                    case enumFeedReaderState.IDLE:
                        return "Idle";
                    default:
                        return "Error";
                }
            }
        }
        public enumFeedReaderState State
        {
            get { return _state; }
        }
        public int MaxRetries
        {
            get { return _max_retry_query; }
            set { _max_retry_query = value; }
        }
        public bool IsBusy
        {
            get { return _query_thread.IsBusy; }
        }
        public DateTime Updated
        {
            get { return _last_updated; }
        }
        public string Username
        {
            get { return _username; }
        }
    }
    
    /* Math class for crunching all of the numbers in any given data set */
    public class clsStatMasher
    {
        private List<clsVideoEntry> _initial_dataset = null;
        private Dictionary<string, List<clsDataPoint>> _historical_data = null;

        public clsStatMasher() {}
        public clsStatMasher(List<clsVideoEntry> InitialDataSet, Dictionary<string, List<clsDataPoint>> HistoricalData)
        {
            _initial_dataset = InitialDataSet;
            _historical_data = HistoricalData;
        }
        
        // public methods
        public List<clsDataPoint> GetDataPointsByID(string VideoID)
        {
            if (_historical_data.ContainsKey(VideoID))
                return _historical_data[VideoID];
            else
                return null;
        }
        public clsVideoEntry GetInitialDataByID(string VideoID)
        {
            if (_initial_dataset == null)
                return null;
            int index = _initial_dataset.FindIndex(delegate(clsVideoEntry e) { return e.VideoID.Equals(VideoID); });
            if (index >= 0)
                return _initial_dataset[index];
            else
                return null;
        }
        public double AverageNewRatingByID(string VideoID)
        {
            clsVideoEntry InitialData = GetInitialDataByID(VideoID);
            List<clsDataPoint> HistoricalData = GetDataPointsByID(VideoID);

            if (InitialData == null || HistoricalData == null || HistoricalData.Count == 0)
                return 0;

            double[] A = { InitialData.AverageRating, -1 };
            double[] N = { InitialData.Raters, -1 };
            int counted = 0;

            for (int i = HistoricalData.Count - 1; i >= 0; i--)
            {
                if (A[1] != -1 && N[1] != -1)
                    break;

                if (HistoricalData[i].Field.Field == clsDataPointField.VideoDataFields.RATERS)
                {
                    if (N[1] == -1)
                        N[1] = HistoricalData[i].New;
                    counted += (int)HistoricalData[i].Delta;
                }
                if (HistoricalData[i].Field.Field == clsDataPointField.VideoDataFields.AVERAGE_RATING && A[1] == -1)
                    A[1] = HistoricalData[i].New;
                
            }

            if (counted == 0)
                return 0;

            if (A[1] == -1)
                return 0;

            return _calc_average_new_rating(A[0], A[1], N[0], N[1]);
        }
        public KeyValuePair<int,double> AverageNewRatingByID(string VideoID, int numMostRecentVotes)
        {
            clsVideoEntry InitialData = GetInitialDataByID(VideoID);
            List<clsDataPoint> HistoricalData = GetDataPointsByID(VideoID);

            if (InitialData == null || HistoricalData == null || HistoricalData.Count == 0)
                return new KeyValuePair<int,double>(0,0);

            double[] A = { 0, -1 };
            double[] N = { 0, -1 };
            int counted = 0;

            for (int i = HistoricalData.Count - 1; i >= 0; i--)
            {
                if ((counted >= numMostRecentVotes) && A[1] != -1 && N[1] != 1)
                    break;
                if (HistoricalData[i].Field.Field == clsDataPointField.VideoDataFields.RATERS)
                {   
                    if( N[1] == -1)
                        N[1] = HistoricalData[i].New;
                    N[0] = HistoricalData[i].Old;
                    counted += (int)HistoricalData[i].Delta;
                }
                if (HistoricalData[i].Field.Field == clsDataPointField.VideoDataFields.AVERAGE_RATING)
                {
                    if (A[1] == -1)
                        A[1] = HistoricalData[i].New;
                    A[0] = HistoricalData[i].Old;
                }
            }

            if (counted == 0)
                return new KeyValuePair<int, double>(0, 0);

            if (A[1] == -1)
                return new KeyValuePair<int, double>(0, 0);

            return new KeyValuePair<int,double>(counted, _calc_average_new_rating(A[0], A[1], N[0], N[1]));
        }
        public void CleanUp()
        {
            _initial_dataset = null;
            _historical_data = null;
        }
        public double GetMovedStatByField(string VideoID, clsDataPointField.VideoDataFields field)
        {
            clsVideoEntry InitialData = GetInitialDataByID(VideoID);
            List<clsDataPoint> HistoricalData = GetDataPointsByID(VideoID);

            if (InitialData == null || HistoricalData == null || HistoricalData.Count == 0)
                return 0;

            double ret = 0;
            int num = 0;
            foreach (clsDataPoint d in HistoricalData)
            {
                if (d.Field.Field == field)
                {
                    if (d.Field.Field == clsDataPointField.VideoDataFields.AVERAGE_RATING)
                        num++;
                    else
                        ret += d.Delta;
                }
            }
            if (field == clsDataPointField.VideoDataFields.AVERAGE_RATING)
                return num;
            else
                return ret;
        }

        // private methods
        private double _calc_average_new_rating(double old_average, double new_average, double old_raters, double new_raters)
        {
            System.Diagnostics.Debug.Print((((new_average * new_raters) - (old_average * old_raters)) / (new_raters - old_raters)).ToString());
            return ((new_average * new_raters) - (old_average * old_raters)) / (new_raters - old_raters); 
        }
        
        // public properties
        public List<clsVideoEntry> InitialDataSet
        {
            get { return _initial_dataset; }
            set { _initial_dataset = value; }
        }
        public Dictionary<string, List<clsDataPoint>> HistoricalDataPoints
        {
            get { return _historical_data; }
            set { _historical_data = value; }
        }
    }

    /* Utility class to check video settings */
    public class clsSettingsManager
    {
         [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(
        int FeatureEntry,
        [MarshalAs(UnmanagedType.U4)] int dwFlags,
        bool fEnable);

        private const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private const int SET_FEATURE_ON_PROCESS = 0x00000002;

        // private enums
        public enum InternalState
        {
            unknown = -1,
            idle,
            login_step_1,
            login_step_2,
            login_step_3,
            login_check,
            get_settings,
            save_settings
        }
        public enum FailureCodes
        {
            FAILURE_LOGGING_IN,
            FAILURE_CHANGING_SETTINGS
        }
        private class Action
        {
            public string VideoID { get; set; }
            public bool Enable { get; set; }
            public Action(string ID, bool act)
            {
                this.VideoID = ID;
                this.Enable = act;
            }
        }

        // private member variable
        private InternalState _state = InternalState.idle;
        private HttpWebRequest _last_request = null;
        private CookieContainer _cookie_jar = null;
        private Cookie _login_info = null;
        private int _request_timeout = 20 * 1000;       // 20 seconds default
        private System.ComponentModel.BackgroundWorker _response_fetcher = new System.ComponentModel.BackgroundWorker();
        private System.ComponentModel.BackgroundWorker _post_response_fetcher = new System.ComponentModel.BackgroundWorker();
        private bool _enable = false;
        private string _video_id = string.Empty;
        private System.Timers.Timer _activity_timer = new System.Timers.Timer();
        private List<Action> _action_list = new List<Action>();
        private Action _current_action;
        private int _current_action_fail_count = 0;

        public void Dispose()
        {
            _cookie_jar = null;
            OnException = null;
            OnFailure = null;
            OnStatusChange = null;
            OnSuccess = null;
            _activity_timer.Enabled = false;
            _response_fetcher.Dispose();
            _post_response_fetcher.Dispose();
            _activity_timer.Dispose();
            _action_list.Clear();
        }

        // public events
        public delegate void ExceptionEventHandler(object sender, Exception e);
        public event ExceptionEventHandler OnException;
        private void _raise_exception(Exception e)
        {
            if (OnException != null)
                OnException(this, e);
        }

        public delegate void StatusEventHandler(object sender, InternalState s);
        public event StatusEventHandler OnStatusChange;
        private void _raise_status_change(InternalState s)
        {
            _state = s;
            if (OnStatusChange != null)
                OnStatusChange(this, s);

            if (s == InternalState.idle)
                if (_action_list.Count > 0)
                    _do_next_action();
                else 
                    _activity_timer.Enabled = true;
        }

        public delegate void FailureEventHandler(object sender, FailureCodes f);
        public event FailureEventHandler OnFailure;
        private void _raise_failure(FailureCodes f)
        {
            _current_action_fail_count++;
            if (_current_action_fail_count < 3)
                _action_list.Add(_current_action);
            else
                _current_action_fail_count = 0;

            if (OnFailure != null)
                OnFailure(this, f);
        }

        public delegate void SuccessEventHandler(object sender, FailureCodes f);
        public event SuccessEventHandler OnSuccess;
        private void _raise_success(FailureCodes f)
        {
            _current_action_fail_count = 0;
            if (OnSuccess != null)
                OnSuccess(this, f);
        }

        // contructors
        public clsSettingsManager() : this(string.Empty, string.Empty) { }
        public clsSettingsManager(string Username, string Password)
        {
            int feature = FEATURE_DISABLE_NAVIGATION_SOUNDS;
            CoInternetSetFeatureEnabled(feature, SET_FEATURE_ON_PROCESS, true);

            this.Username = Username;
            this.Password = Password;

            _activity_timer.Interval = 8000;

            _activity_timer.Elapsed += new ElapsedEventHandler(_activity_timer_Elapsed);
            _response_fetcher.DoWork += new System.ComponentModel.DoWorkEventHandler(_worker_thread_get_response);
            _response_fetcher.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(_worker_thread_got_response);
            _response_fetcher.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(_worker_thread_getting_response);
            _post_response_fetcher.DoWork += new System.ComponentModel.DoWorkEventHandler(_post_response_fetcher_DoWork);
            _post_response_fetcher.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(_post_response_fetcher_RunWorkerCompleted);
            _post_response_fetcher.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(_post_response_fetcher_ProgressChanged);

            _activity_timer.Enabled = true;
        }

        void _activity_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _do_next_action();
        }
        private void _do_next_action()
        {
            if (this.Username == string.Empty || this.Username == null || this.Password == string.Empty || this.Password == null)
                return;
            if (_state != InternalState.idle)
                return;
            if (_action_list.Count == 0)
                return;

            _state = InternalState.unknown;
            _activity_timer.Enabled = false;

            Action a = _action_list[0];
            _action_list.Remove(a);
            _current_action = a;

            if (a.VideoID == null)
            {
                _get_cookies();
                return;
            }
            else if (_cookie_jar == null)
            {
                _get_cookies();
                _current_action = new Action(null, false);
                _action_list.Add(a);
                return;
            }

            _enable = a.Enable;
            _raise_status_change(InternalState.get_settings);
            _video_id = a.VideoID;
            _do_get_request("http://www.youtube.com/my_videos_edit?video_id=" + a.VideoID, _cookie_jar);
        }

        // public methods
        public void DisableVideo(string VideoID)
        {
            _remove_action(VideoID);
            _action_list.Add(new Action(VideoID, false));           
        }
        public void EnableVideo(string VideoID)
        {
            _remove_action(VideoID);
            _action_list.Add(new Action(VideoID, true));
        }
        public void StopActing()
        {
            _action_list.Clear();
            _state = InternalState.idle;
        }

        // private methods
        private void _remove_action(string VideoID)
        {
            int i = 0;

            while (i < _action_list.Count)
            {
                if (_action_list[i].VideoID == VideoID)
                    _action_list.Remove(_action_list[i]);
                else
                    i++;
            }
        }
        private void _check_login_info()
        {
            if (_login_info == null || _cookie_jar.Count <= 0)
                return;

            _do_get_request("http://www.youtube.com/my_videos", _cookie_jar);
            //return response.StatusCode == HttpStatusCode.OK;
        }
        private void _get_cookies()
        {
            if (_response_fetcher.IsBusy || _post_response_fetcher.IsBusy)
            {
                _raise_exception(new Exception("Thread is busy!"));
                _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                _raise_status_change(InternalState.idle);
                return;
            }
            CookieContainer cookie_jar = new CookieContainer();

            string url = "https://www.google.com/accounts/ServiceLogin?uilel=3&service=youtube&passive=true&continue=http%3A%2F%2Fwww.youtube.com%2Fsignin%3Faction_handle_signin%3Dtrue%26nomobiletemp%3D1%26hl%3Den_US%26next%3D%252Findex&hl=en_US&ltmpl=sso";
            _raise_status_change(InternalState.login_step_1);
            _do_get_request(url, cookie_jar);
        }
        private void _do_get_request(string url, CookieContainer cookieJar)
        {
            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(url);
            _request.Timeout = _request_timeout;
            _request.CookieContainer = cookieJar;
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.13) Gecko/2009073022 Firefox/3.0.13 (.NET CLR 3.5.30729)";
            _request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _request.KeepAlive = false;
            _request.Method = "GET";

            _last_request = _request;
            _response_fetcher.RunWorkerAsync(_request);
        }

        private class post_arguments
        {
            public HttpWebRequest Request { get; set; }
            public byte[] post_data { get; set; }
            public post_arguments(HttpWebRequest r, byte[] d) { this.Request = r; this.post_data = d; }
        }
        private void _do_post_request(string url, string post_data, CookieContainer CookieJar)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = Encoding.UTF8.GetBytes(post_data);

            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(url);

            _request.ContentLength = data.Length;
            _request.CookieContainer = CookieJar;
            _request.Timeout = _request_timeout;
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.13) Gecko/2009073022 Firefox/3.0.13 (.NET CLR 3.5.30729)";
            _request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _request.Headers.Add(HttpRequestHeader.KeepAlive, "300");
            _request.KeepAlive = false;
            _request.ProtocolVersion = HttpVersion.Version10;
            _request.ContentType = "application/x-www-form-urlencoded";
            _request.Referer = _last_request.RequestUri.ToString();
            _request.Method = "POST";
        
            _last_request = _request;
            _post_response_fetcher_DoWork(this, new System.ComponentModel.DoWorkEventArgs(new post_arguments(_request, data)));
        }
        private string _get_login_form(string html)
        {
            IHTMLDocument3 domDocument = _getDocumentFromHTML(html) as IHTMLDocument3;
            IHTMLFormElement _form = (IHTMLFormElement)domDocument.getElementById("gaia_loginform");

            if (_form == null)
                return null;

            string post_data = string.Empty;

            IHTMLInputElement _continue = _getElementByName(domDocument, "continue") as IHTMLInputElement;
            IHTMLInputElement _service = _getElementByName(domDocument, "service") as IHTMLInputElement;
            IHTMLInputElement _GALX = _getElementByName(domDocument, "GALX") as IHTMLInputElement;
            IHTMLInputElement _rmShown = _getElementByName(domDocument, "rmShown") as IHTMLInputElement;
            IHTMLInputElement _asts = _getElementByName(domDocument, "asts") as IHTMLInputElement;
            IHTMLInputElement _ltmpl = _getElementByName(domDocument, "ltmpl") as IHTMLInputElement;
            IHTMLInputElement _uilel = _getElementByName(domDocument, "uilel") as IHTMLInputElement;
            IHTMLInputElement _hl = _getElementByName(domDocument, "hl") as IHTMLInputElement;
            IHTMLInputElement _ltmpl2 = _getElementByName(domDocument, "ltmpl") as IHTMLInputElement;

            post_data = "Email=" + Username + "&Passwd=" + Password + "&PersistentCookie=yes&signIn=Sign+in";
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
        private string _get_settings_form(string html, bool Enabled)
        {
            //'XSRF_TOKEN': 'cH-siYb9w2jglfqsArlTaZCRj2l8MTI2NDMxMzU0Nw=='
            string session = string.Empty;
            int start = 0; int end = 0;
            start = html.IndexOf("'XSRF_TOKEN': '");
            if (start > -1)
            {
                    start += "'XSRF_TOKEN': '".Length;
                    end = html.IndexOf("',", start + 1);
                    if (end > -1)
                        session = html.Substring(start, end - start);                
            } 
            //return "ns=1&next=&video_id=whqvyRfwwiw&action_videosave=1&ignore_broadcast_settings=0&field_myvideo_title=123abc&field_myvideo_descr=fuck+yea&field_myvideo_keywords=nada&field_myvideo_categories=22&field_current_still_id=2&field_still_id=2&field_privacy=private&private_share_entity=&allow_comments=No&allow_comment_ratings=No&allow_responses=Kinda&allow_ratings=No&allow_embedding=Yes&allow_syndication=Yes&field_date_mon=0&field_date_day=0&field_date_yr=0&location=&altitude=&session_token=" + session.Replace("=", "%3D");

            string field_myvideo_descr, field_myvideo_keywords;
            field_myvideo_descr = _getTextAreaElement(html, "field_myvideo_descr");
            field_myvideo_keywords = _getTextAreaElement(html, "field_myvideo_keywords");

            string ns, next, video_id, action_videosave, ignore_broadcast_settings, field_myvideo_title, field_current_still_id,
                   field_still_id, location, altitude;
            ns = _getInputElement(html, "ns");
            next = _getInputElement(html, "next");
            video_id = _getInputElement(html, "video_id");
            action_videosave = _getInputElement(html, "action_videosave");
            ignore_broadcast_settings = _getInputElement(html, "ignore_broadcast_settings");
            field_myvideo_title = _getInputElement(html, "field_myvideo_title");
            field_current_still_id = _getInputElement(html, "field_current_still_id");
            field_still_id = _getInputElement(html, "field_still_id");
            location = _getInputElement(html, "location");
            altitude = _getInputElement(html, "altitude");

            string field_myvideo_categories, month, day, year;
            field_myvideo_categories = _getSelectedElement(html, "field_myvideo_categories");
            month = _getSelectedElement(html, "field_date_mon");
            day = _getSelectedElement(html, "field_date_day");
            year = _getSelectedElement(html, "field_date_yr");

            if (month == null)
                month = "0";
            if (day == null)
                day = "0";
            if (year == null)
                year = "0";

            string privacy_selected, comments_selected, comment_ratings_selected, responses_selected,
                   embedding_selected, syndication_selected;
            privacy_selected = _getSelectedRadioElement(html, "field_privacy");
            comments_selected = _getSelectedRadioElement(html, "allow_comments");
            comment_ratings_selected = _getSelectedRadioElement(html, "allow_comment_ratings");
            responses_selected = _getSelectedRadioElement(html, "allow_responses");
            embedding_selected = _getSelectedRadioElement(html, "allow_embedding");
            syndication_selected = _getSelectedRadioElement(html, "allow_syndication");

            string post_data = string.Empty;
            post_data = "ns=" + ns + "&next=&video_id=" + System.Web.HttpUtility.UrlEncode(video_id) + "&action_videosave=1&ignore_broadcast_settings=" + System.Web.HttpUtility.UrlEncode(ignore_broadcast_settings) + "&field_myvideo_title=" + System.Web.HttpUtility.UrlEncode(field_myvideo_title) + "&field_myvideo_descr=" + System.Web.HttpUtility.UrlEncode(field_myvideo_descr.Replace("&amp;", "&")) + "&field_myvideo_keywords=" + System.Web.HttpUtility.UrlEncode(field_myvideo_keywords) + "&field_myvideo_categories=" + System.Web.HttpUtility.UrlEncode(field_myvideo_categories);
            post_data += "&field_current_still_id=" + System.Web.HttpUtility.UrlEncode(field_current_still_id) + "&field_still_id=" + System.Web.HttpUtility.UrlEncode(field_still_id) + "&field_privacy=" + System.Web.HttpUtility.UrlEncode(privacy_selected) + "&private_video_token=&private-url=&allow_comments=" + System.Web.HttpUtility.UrlEncode(comments_selected) + "&allow_comment_ratings=" + System.Web.HttpUtility.UrlEncode(comment_ratings_selected) + "&allow_responses=" + System.Web.HttpUtility.UrlEncode(responses_selected) + "&allow_ratings=" + System.Web.HttpUtility.UrlEncode(Enabled ? "Yes" : "No");
            post_data += "&allow_embedding=" + System.Web.HttpUtility.UrlEncode(embedding_selected) + "&allow_syndication=" + System.Web.HttpUtility.UrlEncode(syndication_selected) + "&field_date_mon=" + System.Web.HttpUtility.UrlEncode(month) + "&field_date_day=" + System.Web.HttpUtility.UrlEncode(day) + "&field_date_yr=" + System.Web.HttpUtility.UrlEncode(year) + "&location=" + System.Web.HttpUtility.UrlEncode(location) + "&altitude=" + System.Web.HttpUtility.UrlEncode(altitude) + "&session_token=" + session.Replace("=", "%3D");

            System.Console.Write(post_data);
            return post_data;

            /*

            IHTMLDocument3 domDocument = _getDocumentFromHTML(html) as IHTMLDocument3;
            IHTMLFormElement _form = (IHTMLFormElement)domDocument.getElementById("video-details-form");

            if (_form == null)
                return null;


            // text inputs
            IHTMLTextAreaElement field_myvideo_descr = _getElementByName(domDocument, "field_myvideo_descr") as IHTMLTextAreaElement;
            IHTMLTextAreaElement field_myvideo_keywords = _getElementByName(domDocument, "field_myvideo_keywords") as IHTMLTextAreaElement;
            if (field_myvideo_descr == null || field_myvideo_keywords == null)
                return null;

            IHTMLInputElement ns = _getElementByName(domDocument, "ns") as IHTMLInputElement;
            IHTMLInputElement next = _getElementByName(domDocument, "next") as IHTMLInputElement;
            IHTMLInputElement video_id = _getElementByName(domDocument, "video_id") as IHTMLInputElement;
            IHTMLInputElement action_videosave = _getElementByName(domDocument, "action_videosave") as IHTMLInputElement;
            IHTMLInputElement ignore_broadcast_settings = _getElementByName(domDocument, "ignore_broadcast_settings") as IHTMLInputElement;
            IHTMLInputElement field_myvideo_title = _getElementByName(domDocument, "field_myvideo_title") as IHTMLInputElement;
            IHTMLInputElement field_current_still_id = _getElementByName(domDocument, "field_current_still_id") as IHTMLInputElement;
            IHTMLInputElement field_still_id = _getElementByName(domDocument, "field_still_id") as IHTMLInputElement;
            IHTMLInputElement location = _getElementByName(domDocument, "location") as IHTMLInputElement;
            IHTMLInputElement altitude = _getElementByName(domDocument, "altitude") as IHTMLInputElement;
            if (ns == null || next == null || video_id == null || action_videosave == null
                           || ignore_broadcast_settings == null || field_myvideo_keywords == null
                           || field_current_still_id == null || field_still_id == null
                           || location == null || altitude == null)
                return null;

            // selects
            IHTMLSelectElement field_myvideo_categories = _getElementByName(domDocument, "field_myvideo_categories") as IHTMLSelectElement;
            IHTMLSelectElement categories = _getElementByName(domDocument, "field_myvideo_categories") as IHTMLSelectElement;
            IHTMLSelectElement month = _getElementByName(domDocument, "field_date_mon") as IHTMLSelectElement;
            IHTMLSelectElement day = _getElementByName(domDocument, "field_date_day") as IHTMLSelectElement;
            IHTMLSelectElement year = _getElementByName(domDocument, "field_date_yr") as IHTMLSelectElement;
            if (field_myvideo_categories == null || categories == null || month == null || day == null || year == null)
                return null;

            // radio options
            IHTMLElementCollection options = domDocument.getElementsByName("field_privacy");
            IHTMLInputElement privicy_selected = _get_selected_option(options);
            options = domDocument.getElementsByName("allow_comments");
            IHTMLInputElement comments_selected = _get_selected_option(options);
            options = domDocument.getElementsByName("allow_comment_ratings");
            IHTMLInputElement comment_ratings_selected = _get_selected_option(options);
            options = domDocument.getElementsByName("allow_responses");
            IHTMLInputElement responses_selected = _get_selected_option(options);
            options = domDocument.getElementsByName("allow_embedding");
            IHTMLInputElement embedding_selected = _get_selected_option(options);
            options = domDocument.getElementsByName("allow_syndication");
            IHTMLInputElement syndication_selected = _get_selected_option(options);
            if (privicy_selected == null || comments_selected == null || comment_ratings_selected == null ||
                responses_selected == null || embedding_selected == null || syndication_selected == null)
                return null;
            

            // fucking WORKS!!! return "ns=1&next=&video_id=whqvyRfwwiw&action_videosave=1&ignore_broadcast_settings=0&field_myvideo_title=123abc&field_myvideo_descr=fuck+yea&field_myvideo_keywords=nada&field_myvideo_categories=22&field_current_still_id=2&field_still_id=2&field_privacy=private&private_share_entity=00DDB08E54250E2A&allow_comments=No&allow_comment_ratings=No&allow_responses=Kinda&allow_ratings=No&allow_embedding=Yes&allow_syndication=Yes&field_date_mon=0&field_date_day=0&field_date_yr=0&location=&altitude=&session_token=" + session.Replace("=", "%3D");
            try
            {
                post_data = "ns=1&next=&video_id=" + System.Web.HttpUtility.UrlEncode(video_id.value) + "&action_videosave=1&ignore_broadcast_settings=" + System.Web.HttpUtility.UrlEncode(ignore_broadcast_settings.value) + "&field_myvideo_title=" + System.Web.HttpUtility.UrlEncode(field_myvideo_title.value) + "&field_myvideo_descr=" + System.Web.HttpUtility.UrlDecode(field_myvideo_descr.value) + "&field_myvideo_keywords=" + System.Web.HttpUtility.UrlDecode(field_myvideo_keywords.value) + "&field_myvideo_categories=" + System.Web.HttpUtility.UrlEncode(field_myvideo_categories.value);
                post_data += "&field_current_still_id=" + System.Web.HttpUtility.UrlEncode(field_current_still_id.value) + "&field_still_id=" + System.Web.HttpUtility.UrlEncode(field_still_id.value) + "&field_privacy=" + System.Web.HttpUtility.UrlEncode(privicy_selected.value) + "&allow_comments=" + System.Web.HttpUtility.UrlEncode(comments_selected.value) + "&allow_comment_ratings=" + System.Web.HttpUtility.UrlEncode(comment_ratings_selected.value) + "&allow_responses=" + System.Web.HttpUtility.UrlEncode(responses_selected.value) + "&allow_ratings=" + System.Web.HttpUtility.UrlEncode(Enabled ? "Yes" : "No");
                post_data += "&allow_embedding=" + System.Web.HttpUtility.UrlEncode(embedding_selected.value) + "&allow_syndication=" + System.Web.HttpUtility.UrlEncode(syndication_selected.value) + "&field_date_mon=" + System.Web.HttpUtility.UrlEncode(month.value) + "&field_date_day=" + System.Web.HttpUtility.UrlEncode(day.value) + "&field_date_yr=" + System.Web.HttpUtility.UrlEncode(year.value) + "&location=" + System.Web.HttpUtility.UrlEncode(location.value) + "&altitude=" + System.Web.HttpUtility.UrlEncode(altitude.value) + "&session_token=" + session.Replace("=", "%3D");
                return post_data;
            }
            catch (Exception ex)
            {
                _raise_exception(ex);
                return null; 
            }
            */
        }

        private string _getTextAreaElement(string html, string name)
        {
            Regex r = new Regex("<textarea.*?name=\"" + name + "\".*?>(.*?)</textarea>", RegexOptions.Singleline);
            Match m = r.Match(html);
            if (!m.Success)
                return null;
            else
                return m.Groups[1].Value;
        }
        private string _getInputElement(string html, string name) 
        {
            Regex r = new Regex("<input.*?type=(\"hidden\"|\"text\").*?name=\"" + name + "\".*?value=\"(.*?)\"" );

            Match m = r.Match(html);
            if (!m.Success)
                return null;
            else
                return m.Groups[2].Value;

        }
        private string _getSelectedElement(string html, string name)
        {
            Regex _regX_select = new Regex("<select name=\"" + name + "\".+?>(.+)</select>", RegexOptions.Singleline);
            Match m = _regX_select.Match(html);
            if (!m.Success)
                return null;
            string options = m.Groups[1].Value;

            Regex _regX_selected = new Regex("<option value=\"(.+?)\" selected>");
            m = _regX_selected.Match(options);
            if (!m.Success)
                return null;
            else
                return m.Groups[1].Value;
        }
        private string _getSelectedRadioElement(string html, string name)
        {
            Regex _regX_Radio = new Regex("<input type=\"radio\".+?name=\"" + name + "\".+?value=\"(.+?)\".+?checked");
            Match m = _regX_Radio.Match(html);
            if (!m.Success)
                return null;
            else
                return m.Groups[1].Value;
        }

        private IHTMLInputElement _get_selected_option(IHTMLElementCollection options)
        {
            foreach (IHTMLInputElement o in options)
                if (o.@checked == true)
                    return o;
            return null;
        }
        private string _get_redirect_url(string html)
        {
            string search_right = "'";
            string search_left = "url='";
            int redir_start = html.IndexOf(search_left);
            if (redir_start < 0)
            {
                redir_start = html.IndexOf("url=&#39;");
                if (redir_start > 0)
                {
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
        private IHTMLElement _getElementByName(IHTMLDocument3 doc, string name)
        {
            IHTMLElementCollection elements = doc.getElementsByName(name);
            foreach (IHTMLElement element in elements)
                return element;
            return null;
        }

        private HTMLDocumentClass _getDocumentFromHTML(string html)
        {
            
            //html = html.Replace("<SCRIPT", "<SC");
            object[] oPageText = { html };
            HTMLDocumentClass myDoc = new HTMLDocumentClass();
            IHTMLDocument2 oMyDoc = (IHTMLDocument2)myDoc;
            oMyDoc.write(oPageText);
            oMyDoc.close();
            return oMyDoc as HTMLDocumentClass;
        }

        // private threading methods
        void _post_response_fetcher_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void _post_response_fetcher_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void _post_response_fetcher_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            post_arguments args = e.Argument as post_arguments;
            HttpWebRequest _request = args.Request;
            byte[] data = args.post_data;

            switch (_state)
            {
                case InternalState.save_settings:
                    int tries = 0;
                    Stream post_stream = null;

                        try
                        {
                            _request.Expect = string.Empty;
                            post_stream = _request.GetRequestStream();
                        }
                        catch (Exception ex)
                        {
                            _raise_exception(ex);
                            _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                            _raise_status_change(InternalState.idle);
                            return;
                        }
                    if (post_stream == null)
                        return;

                    try
                    {
                        post_stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                        _raise_status_change(InternalState.idle);
                        post_stream.Close();
                        post_stream.Dispose();
                        return;
                    }
                    finally
                    {
                        post_stream.Close();
                        post_stream.Dispose();
                    }

                    try 
                    { 
                        HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
                        StreamReader stream = new StreamReader(_response.GetResponseStream(), System.Text.Encoding.ASCII);
                        string html = string.Empty;
                        try
                        {
                            html = stream.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            _raise_exception(ex);
                            _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                            _raise_status_change(InternalState.idle);
                            stream.Close();
                            stream.Dispose();
                            _response.Close();
                            return;
                        }
                        finally
                        {
                            stream.Close();
                            stream.Dispose();
                            _response.Close();
                        }

                        _raise_success(FailureCodes.FAILURE_CHANGING_SETTINGS);
                        _raise_status_change(InternalState.idle);
                    }
                    catch (Exception ex) 
                    {
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                        _raise_status_change(InternalState.idle);
                        post_stream.Close();
                        post_stream.Dispose();
                        return;
                    }


                    break;
                case InternalState.login_step_2:
                    try 
                    { 
                        post_stream = _request.GetRequestStream(); 
                    }
                    catch (Exception ex) 
                    {                        
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                        _raise_status_change(InternalState.idle);
                        return;
                    }

                    try
                    {
                        post_stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                        _raise_status_change(InternalState.idle);
                        post_stream.Close();
                        post_stream.Dispose();
                        return;
                    }
                    finally
                    {
                        post_stream.Close();
                        post_stream.Dispose();
                    }

                    try
                    {
                        HttpWebResponse _res = (HttpWebResponse)_request.GetResponse();
                        StreamReader str = new StreamReader(_res.GetResponseStream(), System.Text.Encoding.ASCII);
                        string htm = string.Empty;
                        try
                        {
                            htm = str.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            _raise_exception(ex);
                            _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                            _raise_status_change(InternalState.idle);
                            str.Close();
                            str.Dispose();
                            _res.Close();
                            return;
                        }
                        finally
                        {
                            str.Close();
                            str.Dispose();
                            _res.Close();
                        }

                        foreach (Cookie c in _last_request.CookieContainer.GetCookies(new Uri("http://www.youtube.com")))
                        {
                            if (c.Name == "LOGIN_INFO")
                            {
                                _cookie_jar = _last_request.CookieContainer;
                                _login_info = c;
                                _raise_success(FailureCodes.FAILURE_LOGGING_IN);
                                _raise_status_change(InternalState.idle);
                                return;
                            }
                        }

                        string url = _get_redirect_url(htm);
                        if (url == null || url == string.Empty)
                        {
                            _raise_status_change(InternalState.idle);
                            _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                            _raise_exception(new Exception("_post_response_fetcher_DoWork: failed to get redirect url from step 2."));
                            return;
                        }
                        _raise_status_change(InternalState.login_step_3);
                        _do_get_request(url, _request.CookieContainer);
                    }
                    catch ( Exception ex ) 
                    {
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                        _raise_status_change(InternalState.idle);
                        post_stream.Close();
                        post_stream.Dispose();
                        return;
                    }


                    break;
                default:
                    _raise_exception(new Exception("_post_response_fetcher_DoWork: failed to select state."));
                    _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                    _raise_status_change(InternalState.idle);
                    break;
            }
        }

        void _worker_thread_getting_response(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void _worker_thread_got_response(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            HttpWebResponse response = e.Result as HttpWebResponse;

            switch (_state)
            {
                case InternalState.get_settings:
                    StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
                    string html = null;
                    try
                    {
                        html = stream.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                        stream.Close();
                        stream.Dispose();
                        response.Close();
                        _raise_status_change(InternalState.idle);
                        return;
                    }
                    finally
                    {
                        stream.Close();
                        stream.Dispose();
                        response.Close();
                    }

                    if (html == string.Empty || html == null)
                    {
                        _raise_exception(new Exception("_worker_thread_got_response: failed to get html in get_settings step 1"));
                        _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                        _raise_status_change(InternalState.idle);
                        return;
                    }

                    string post_data = _get_settings_form(html, _enable);
                    string url = "http://www.youtube.com/my_videos_edit";

                    if (post_data == string.Empty || post_data == null)
                    {
                        _raise_exception(new Exception("_worker_thread_got_response: failed to get post_data in get_settings step 1"));
                        _raise_failure(FailureCodes.FAILURE_CHANGING_SETTINGS);
                        _raise_status_change(InternalState.idle);
                        return;
                    }

                    _raise_status_change(InternalState.save_settings);
                    System.Console.Write(_login_info.ToString());
                    
                    //string host = "www.youtube.com";
                    //string headers = "POST /my_videos_edit HTTP/1.1\nHost: www.youtube.com\nUser-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7 (.NET CLR 3.5.30729)\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\nCookie: "+_cookie_jar.GetCookieHeader(new Uri("http://www.youtube.com")) + "\nContent-Type: application/x-www-form-urlencoded";
                    //string result = GetSocket.SocketSendReceive(host, 80, headers, post_data);

                    //_raise_success(FailureCodes.FAILURE_CHANGING_SETTINGS);
                    //_raise_status_change(InternalState.idle);

                    _do_post_request(url, post_data, _last_request.CookieContainer);
                    break;
                case InternalState.login_step_1:
                    try
                    {
                        stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
                        html = null;
                        html = stream.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        _raise_exception(ex);
                        _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                        _raise_status_change(InternalState.idle);
                        response.Close();
                        return;
                    }
                    finally
                    {
                        response.Close();
                    }

                    if (html == string.Empty || html == null)
                    {
                        _raise_exception(new Exception("_worker_thread_got_response: failed to get html in step 1"));
                        _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                        _raise_status_change(InternalState.idle);
                        return;
                    }

                    post_data = _get_login_form(html);
                    url = "https://www.google.com/accounts/ServiceLoginAuth?service=youtube";

                    _raise_status_change(InternalState.login_step_2);
                    _do_post_request(url, post_data, _last_request.CookieContainer);
                    break;
                case InternalState.login_step_3:
                    foreach (Cookie c in _last_request.CookieContainer.GetCookies(_last_request.RequestUri))
                    {
                        if (c.Name == "LOGIN_INFO")
                        {
                            _cookie_jar = _last_request.CookieContainer;
                            _login_info = c;
                            _raise_success(FailureCodes.FAILURE_LOGGING_IN);
                            _raise_status_change(InternalState.idle);
                            response.Close();
                            return;
                        }
                    }
                    _raise_exception(new Exception("Failed to get login cookies"));
                    _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                    _raise_status_change(InternalState.idle);
                    response.Close();
                    break;
                default:
                    _raise_exception(new Exception("worker_thread_got_response: failed to determine thread state."));
                    _raise_failure(FailureCodes.FAILURE_LOGGING_IN);
                    _raise_status_change(InternalState.idle);
                    break;
            }
            response.Close();
        }
        void _worker_thread_get_response(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            HttpWebRequest r = (HttpWebRequest)e.Argument;
            e.Result = r.GetResponse();
        }

        // public properties
        public string Username { get; set; }
        public string Password { get; set; }
        public InternalState State { get { return _state; } }
        public int ActivityListCount { get { return _action_list.Count; } }
        public string VideoID { get { return _video_id; } }
    }

    /* Utility class to handle multiple clsSettingsManager objects */
    public class clsED
    {
        private List<clsSettingsManager> _settings_managers = new List<clsSettingsManager>();
        private clsFileLogger _logger = null;
        private clsSettings _settings;
        private bool _in_action = false;

        public clsED()
        {
        }
        public clsED(clsSettings Settings)
        {
            _settings = Settings;
            _settings.OnAccountAdded += new clsSettings.AccountAddedHandler(_settings_OnAccountAdded);
            _settings.OnAccountRemoved += new clsSettings.AccountRemovedHandler(_settings_OnAccountRemoved);

            if (_settings.ED_Log_File != null && _settings.ED_Log_File != string.Empty)
                _logger = new clsFileLogger(_settings.ED_Log_File);

            foreach (clsCredentials Account in _settings.Accounts)
            {
                if (Account.Password == "-")
                    continue;
                clsSettingsManager sm = new clsSettingsManager(Account.Username, Account.Password);
                sm.OnException += new clsSettingsManager.ExceptionEventHandler(sm_OnException);
                sm.OnFailure += new clsSettingsManager.FailureEventHandler(sm_OnFailure);
                sm.OnStatusChange += new clsSettingsManager.StatusEventHandler(sm_OnStatusChange);
                sm.OnSuccess += new clsSettingsManager.SuccessEventHandler(sm_OnSuccess);
                _settings_managers.Add(sm);
            }
        }

        void _settings_OnAccountRemoved(object sender, clsCredentials Account)
        {
            RemoveAccount(Account);
        }
        void _settings_OnAccountAdded(object sender, clsCredentials Account)
        {
            AddAccount(Account);
        }

        void sm_OnSuccess(object sender, clsSettingsManager.FailureCodes f)
        {
            clsSettingsManager thisSM = (clsSettingsManager)sender;
            if (_logger != null)
                _logger.appendFile(DateTime.Now.ToString() + "-" + thisSM.Username + " Success @ " + f.ToString());
        }
        void sm_OnStatusChange(object sender, clsSettingsManager.InternalState s)
        {
            _in_action = false;
            if (s == clsSettingsManager.InternalState.idle)
            {
                foreach (clsSettingsManager sm in _settings_managers)
                {
                    _in_action = (sm.State != clsSettingsManager.InternalState.idle); 
                    break;
                }
            }
            clsSettingsManager thisSM = (clsSettingsManager)sender;
            System.Diagnostics.Debug.WriteLine(thisSM.Username + " Status changed to " + s.ToString());
        }
        void sm_OnFailure(object sender, clsSettingsManager.FailureCodes f)
        {
            clsSettingsManager thisSM = (clsSettingsManager)sender;
            if (_logger != null)
                _logger.appendFile(DateTime.Now.ToString() + "-" + thisSM.Username + " Failed @ " + f.ToString());
        }
        void sm_OnException(object sender, Exception e)
        {
            clsSettingsManager thisSM = (clsSettingsManager)sender;
            if (_logger != null)
                _logger.appendFile(DateTime.Now.ToString() + "-" + thisSM.Username + " Exception: " + e.Message);
        }

        private void AddAccount(clsCredentials Account)
        {
            foreach (clsSettingsManager sm in _settings_managers)
                if (sm.Username == Account.Username)
                    return;

            if (Account.Password == "-")
                return;

            clsSettingsManager newSM = new clsSettingsManager(Account.Username, Account.Password);
            newSM.OnSuccess += new clsSettingsManager.SuccessEventHandler(sm_OnSuccess);
            newSM.OnFailure += new clsSettingsManager.FailureEventHandler(sm_OnFailure);
            newSM.OnException += new clsSettingsManager.ExceptionEventHandler(sm_OnException);
            newSM.OnStatusChange += new clsSettingsManager.StatusEventHandler(sm_OnStatusChange);

            _settings_managers.Add(newSM);
        }
        private void RemoveAccount(clsCredentials Account)
        {
            int i = 0;
            while (i < _settings_managers.Count)
            {
                if (_settings_managers[i].Username == Account.Username)
                {
                    _settings_managers[i].Dispose();
                    _settings_managers.RemoveAt(i);
                }
                else
                    i++;
            }
        }
        public void ChangeAccountRatings(clsCredentials Account, string VideoID, bool Enable)
        {
            foreach (clsSettingsManager sm in _settings_managers)
            {
                if (sm.Username == Account.Username)
                {
                    if (Enable)
                        sm.EnableVideo(VideoID);
                    else
                        sm.DisableVideo(VideoID);
                    _raise_started_action();
                    return;
                }
            }
        }
        public void ChangeAccountRatings(clsCredentials Account, List<clsVideoEntry> Videos, bool Enable)
        {
            foreach (clsSettingsManager sm in _settings_managers)
            {
                if (sm.Username == Account.Username)
                {
                    if (Enable)
                    {
                        foreach (clsVideoEntry e in Videos)
                            sm.EnableVideo(e.VideoID);
                    }
                    else
                    {
                        foreach (clsVideoEntry e in Videos)
                            sm.DisableVideo(e.VideoID);
                    }
                    _raise_started_action();
                    return;
                }
            }
        }

        public delegate void StartedActionEventHandler(object Sender);
        public event StartedActionEventHandler OnStartedAction;
        private void _raise_started_action()
        {
            if (_in_action == true)
                return;
            if (OnStartedAction != null)
                OnStartedAction(this);
        }

        public List<clsSettingsManager> SettingsManagers { get { return _settings_managers; } } 
        public clsFileLogger Logger { set { _logger = value; } }
    }

    /* Utility class that write to file */
    public class clsFileLogger
    {
        private string _file_name = string.Empty;

        public delegate void ErrorEventHandler(object sender, IOException e);
        public event ErrorEventHandler OnError;
        protected virtual void Error(IOException e)
        {
            if (OnError != null)
                OnError(this, e);
        }

        public clsFileLogger(string filename)
        {
            _file_name = filename;
        }

        public void Initialize()
        {
            try
            {
                FileStream file = new FileStream(_file_name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                file.Close();
            }
            catch (IOException e)
            { Error(e); }
        }
        public string readFile()
        {
            FileStream file = new FileStream(_file_name, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string ret = sr.ReadToEnd();
            sr.Close();
            file.Close();
            return ret;
        }
        public void appendFile(string data)
        {
            StreamWriter sw = File.AppendText(_file_name);
            sw.WriteLine(data);
            sw.Flush();
            sw.Close();
        }
    }

    /*Class to maintain settings */
    public class clsSettings
    {
        private const string DEFAULT_FILENAME = "TubeGuardian.log";
        private const string DEFAULT_SETTINGS_FILENAME = "TubeGuardian.settings";
        private string _settings_file_name;

        private int _collect_interval;
        private string _collect_log_file;
        private int _analyzer_relevant_entries;
        private double _analyzer_vote_threshold;
        private bool _analyzer_disable_affected;
        private bool _analyzer_disable_all;
        private int _analyzer_disable_duration;
        private string _ed_log_file;

        // Gatherer settings
        public int Collect_Interval { get { return _collect_interval; } set { _collect_interval = value; _event_settings_changed(); } }
        public string Collect_Log_File { get { return _collect_log_file; } set { _collect_log_file = value; _event_settings_changed(); } }

        // Account settings
        public List<clsCredentials> Accounts { get; private set; }

        // Analyzer settings
        public int Analyzer_Relevant_Entries { get { return _analyzer_relevant_entries; } set { _analyzer_relevant_entries = value; _event_settings_changed(); } }
        public double Analyzer_Vote_Threshold { get { return _analyzer_vote_threshold; } set { _analyzer_vote_threshold = value; _event_settings_changed(); } }
        public bool Analyzer_Disable_Affected { get { return _analyzer_disable_affected; } set { _analyzer_disable_affected = value; _event_settings_changed(); } }
        public bool Analyzer_Disable_All { get { return _analyzer_disable_all; } set { _analyzer_disable_all = value; _event_settings_changed(); } }
        public int Analyzer_Disable_Duration { get { return _analyzer_disable_duration; } set { _analyzer_disable_duration = value; _event_settings_changed(); } }

        // ED settinds
        public string ED_Log_File { get { return _ed_log_file; } set { _ed_log_file = value; _event_settings_changed(); } }

        public clsSettings(string filename)
        {
            _settings_file_name = filename;
            LoadSettings();
        }
        public clsSettings() : this(DEFAULT_SETTINGS_FILENAME) { }

        public void LoadSettings()
        {
            this.Collect_Interval = 10;
            this.Collect_Log_File = DEFAULT_FILENAME;
            this.Analyzer_Vote_Threshold = 2.5;
            this.Analyzer_Relevant_Entries = 10;
            this.Analyzer_Disable_Duration = 300;
            this.Analyzer_Disable_All = false;
            this.Analyzer_Disable_Affected = true;
            this.Accounts = new List<clsCredentials>();
            this.ED_Log_File = DEFAULT_FILENAME;

            /*
            FileStream file = new FileStream(_settings_file_name, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string data = sr.ReadToEnd();
            sr.Close();
            file.Close();
            */

            Properties.Settings.Default.Reload();
            string data = Properties.Settings.Default.AppSettings;
            if (data == string.Empty)
                return;
            else
            {
                string accounts = _get_section(data, "ACCOUNTS {", "}");
                if (accounts != null)
                {
                    string[] individuals = accounts.Split(',');
                    foreach (string acct in individuals)
                    {
                        string[] UP = acct.Split(':');
                        AddAccount(new clsCredentials(UP[0], UP[1]));
                    }
                }
                try { this.Collect_Interval = int.Parse(_get_section(data, "C_I{", "}")); }
                catch { }
                try { this.Collect_Log_File = _get_section(data, "C_L{", "}"); }
                catch { }
                try { this.Analyzer_Vote_Threshold = double.Parse(_get_section(data, "A_V{", "}")); }
                catch { }
                try { this.Analyzer_Relevant_Entries = int.Parse(_get_section(data, "A_R{", "}")); }
                catch { }
                try { this.Analyzer_Disable_All = bool.Parse(_get_section(data, "A_D1{", "}")); }
                catch { }
                try { this.Analyzer_Disable_Affected = bool.Parse(_get_section(data, "A_D2{", "}")); }
                catch { }
                try { this.ED_Log_File = _get_section(data, "ED_L{", "}"); }
                catch { }
            }
        }
        public void SaveSettings()
        {
            string data = null;

            data = "ACCOUNTS {";
            foreach (clsCredentials account in Accounts)
                data += account.Username + ":" + account.Password + ",";
            data = data.Substring(0, data.Length - 1);
            data += "}";

            data += "C_I{" + this.Collect_Interval.ToString() + "}\n";
            data += "C_L{" + this.Collect_Log_File + "}\n";
            data += "A_V{" + this.Analyzer_Vote_Threshold.ToString() + "}\n";
            data += "A_R{" + this.Analyzer_Relevant_Entries.ToString() + "}\n";
            data += "A_D1{" + this.Analyzer_Disable_All.ToString() + "}\n";
            data += "A_D2{" + this.Analyzer_Disable_Affected.ToString() + "}\n";
            data += "A_D3{" + this.Analyzer_Disable_Duration.ToString() + "}\n";
            data += "ED_L{" + this.ED_Log_File + "}\n";

            Properties.Settings.Default.AppSettings = data;
            Properties.Settings.Default.Save();
            //File.WriteAllText(_settings_file_name, data);
        }
        public void AddAccount(clsCredentials Account)
        {
            if (Account == null)
                return;
            RemoveAccount(Account);
            this.Accounts.Add(Account);
            _event_account_added(Account);
        }
        public void RemoveAccount(clsCredentials Account)
        {
            if (Account == null)
                return;

            int i = 0;
            while (i < Accounts.Count)
            {
                if (Accounts[i].Username == Account.Username)
                {
                    _event_account_removed(Accounts[i]);
                    Accounts.RemoveAt(i);
                }
                else i++;
            }
        }
        private string _get_section(string data, string left, string right)
        {
            int i_left = -1;
            int i_right = -1;

            i_left = data.IndexOf(left);
            
            if (i_left == -1)
                return null;

            i_left += left.Length;

            i_right = data.IndexOf(right, i_left);

            if (i_right == -1)
                return null;

            return data.Substring(i_left, i_right - i_left);
        }

        // public methods
        public clsCredentials GetAccountByUsername( string username ) 
        {
            foreach (clsCredentials acct in Accounts)
                if (acct.Username == username)
                    return acct;
            return null;
        }

        // events!
        public delegate void AccountAddedHandler(object sender, clsCredentials Account);
        public event AccountAddedHandler OnAccountAdded;
        private void _event_account_added(clsCredentials Account)
        {
            if (OnAccountAdded != null)
                OnAccountAdded(this, Account);
        }

        public delegate void AccountRemovedHandler(object sender, clsCredentials Account);
        public event AccountRemovedHandler OnAccountRemoved;
        private void _event_account_removed(clsCredentials Account)
        {
            if (OnAccountRemoved != null)
                OnAccountRemoved(this, Account);
        }

        public delegate void SettingsChangedHandler(object sender);
        public event SettingsChangedHandler OnSettingsChanged;
        private void _event_settings_changed()
        {
            if (OnSettingsChanged != null)
                OnSettingsChanged(this);
        }
    }

}
