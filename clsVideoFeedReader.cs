using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.YouTube;
using Google.GData.Extensions;
using Google.GData.Client;
using System.Threading;
using System.Windows.Forms;

namespace YouTube.VideoFeedReader
{
    public class clsVideoFeedReader
    {
        public delegate void application_doevents();
        public application_doevents DoEvents;

        // private member data
        private string _dev_key = string.Empty;
        private string _app_name = string.Empty;
        private YouTubeService _service = null;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private List<YouTubeEntry> _videos = new List<YouTubeEntry>();

        // separate thread data
        private YouTubeQuery _query = null;
        private YouTubeFeed _feed = null;

        // constructor
        public clsVideoFeedReader(string dev_key, string app_name)
        {
            _dev_key = dev_key;
            _app_name = app_name;
            Service = new YouTubeService(app_name, String.Empty, dev_key);
        }
        public clsVideoFeedReader(string dev_key, string app_name, string username, string password)
        {
            _username = username;
            _password = password;
            _dev_key = dev_key;
            _app_name = app_name;
            Service = new YouTubeService(app_name, string.Empty, dev_key);
            this.setCredentials();
        }

        // public methods
        public void setCredentials()
        {
            _service.setUserCredentials(_username, _password);
        }
        public void setCredentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.setCredentials();
        }
        public List<YouTubeEntry> getVideos()
        {
            _videos.Clear();
            YouTubeQuery query = new YouTubeQuery("http://gdata.youtube.com/feeds/api/users/" + _username + "/uploads");
            query.NumberToRetrieve = 50;
            _do_query(query);
            if (_feed != null)
                _add_videos_to_list(_feed.Entries);
            else
                return null;

            int total_results = _feed.TotalResults;
            if (total_results > 50)
            {
                ProgressUpdate(50, total_results);
                string backup_next_chunk = _feed.NextChunk;
                for (int i = 51; i < total_results; i += 50)
                {
                    if (_feed == null)
                        query = new YouTubeQuery(backup_next_chunk);
                    else
                    {
                        if (_feed.NextChunk != null)
                            query = new YouTubeQuery(_feed.NextChunk);
                        else break;
                    }

                    _do_query(query);
                    if (_feed != null)
                    {
                        _add_videos_to_list(_feed.Entries);
                        backup_next_chunk = _feed.NextChunk;
                    }
                    else { i -= 50; }
                    ProgressUpdate(i, total_results);
                }
            }
            ProgressUpdate(total_results, total_results);
            return _videos;
        }
        public void clearVideos()
        {
            _videos.Clear();
        }

        // public events
        public delegate void StartQueryEventHandler(object sender, string query);
        public delegate void EndQueryEventHandler(object sender, string query);
        public delegate void AddVideoEventHandler(object sender, YouTubeEntry video);
        public delegate void RequestExceptionEventHandler(object sender, GDataRequestException e);
        public delegate void AuthenticationExceptionEventHandler(object sender, AuthenticationException e);
        public delegate void GeneralExceptionEventHandler(object sender, Exception e);
        public delegate void ProgressUpdateEventHandler(object sender, int current, int total);
        public event StartQueryEventHandler OnQueryStarted;
        public event EndQueryEventHandler OnQueryEnded;
        public event AddVideoEventHandler OnVideoAdded;
        public event RequestExceptionEventHandler OnRequestException;
        public event AuthenticationExceptionEventHandler OnAuthenticationException;
        public event GeneralExceptionEventHandler OnGeneralException;
        public event ProgressUpdateEventHandler OnQueryProgress;
        protected virtual void QueryStarted(string query)
        {
            if (OnQueryStarted != null)
                OnQueryStarted(this, query);
        }
        protected virtual void QueryEnded(string query)
        {
            if (OnQueryEnded != null)
                OnQueryEnded(this, query);
        }
        protected virtual void VideoAdded(YouTubeEntry video)
        {
            if (OnVideoAdded != null)
            {
                if (video.Rating == null)
                {
                    Rating r = new Rating();
                    r.NumRaters = 0;
                    r.Average = 0;
                    video.Rating = r;
                }
                OnVideoAdded(this, video);
            }
        }
        protected virtual void RequestException(GDataRequestException exception)
        {
            if (OnRequestException != null)
                OnRequestException(this, exception);
        }
        protected virtual void AuthenticationException(AuthenticationException exception)
        {
            if (OnAuthenticationException != null)
                OnAuthenticationException(this, exception);
        }
        protected virtual void GeneralException(Exception exception)
        {
            if (OnGeneralException != null)
                OnGeneralException(this, exception);
        }
        protected virtual void ProgressUpdate(int current, int total)
        {
            if (OnQueryProgress != null)
                OnQueryProgress(this, current, total);
        }

        // private methods
        private YouTubeFeed _do_query(YouTubeQuery query)
        {
            _query = query;

            // create thread to run the query
            Thread query_thread = new Thread(new ThreadStart(_exec_query));

            // start thread execution
            QueryStarted(_query.Uri.OriginalString.ToString());
            query_thread.Start();

            // wait for thread to exist
            while (!query_thread.IsAlive) ;
            // wait for thread to finish
            while (query_thread.ThreadState != ThreadState.Stopped)
            {
                if (DoEvents != null)
                    DoEvents();
                Thread.Sleep(10);
            }
                
            // return feed
            QueryEnded(_query.Uri.OriginalString.ToString());
            return _feed;
        }
        private void _exec_query()
        {
            _feed = null;
            try { _feed = _service.Query(_query); }
            catch (GDataRequestException e) { RequestException(e); }
            catch (AuthenticationException e) { AuthenticationException(e); }
            catch (Exception e) { GeneralException(e); }
        }
        private void _add_videos_to_list(AtomEntryCollection entries)
        {
            foreach (YouTubeEntry e in entries)
            {
                _videos.Add(e);
                VideoAdded(e);
            }
        }

        // public properties
        public string DevKey
        {
            get { return _dev_key; }
            set { _dev_key = value; }
        }
        public string AppName
        {
            get { return _app_name; }
            set { _app_name = value; }
        }
        public YouTubeService Service
        {
            get { return _service; }
            set { _service = value; }
        }
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public List<YouTubeEntry> Videos
        {
            // you only get a copy!
            get { return new List<YouTubeEntry>(_videos); }
        }
    }
}
