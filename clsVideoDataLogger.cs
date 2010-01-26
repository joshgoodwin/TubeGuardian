using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.YouTube;
using Google.GData.Extensions;
using Google.GData.Client;
using System.IO;

namespace TubeGuardian
{
    public enum enumStatField
    {
        E_UNKNOWN = 0,
        E_VIEWS,
        E_COMMENTS,
        E_FAVORITES,
        E_RATINGS,
        E_AVERAGE_RATING
    }
    public class clsLightVideoData
    {
        private string _video_id = string.Empty;
        private enumStatField _field = enumStatField.E_UNKNOWN;
        private double _new_value = 0;
        private double _old_value = 0;
        private DateTime _time = DateTime.Now;
        private string _title = string.Empty;

        public clsLightVideoData(string video_id, enumStatField field, double old_val, double new_val, string title)
        {
            _video_id = video_id;
            _field = field;
            _new_value = new_val;
            _old_value = old_val;
            _time = DateTime.Now;
            _title = title;
        }
        public clsLightVideoData(clsDataUpdate u)
        {
            _video_id = u.VideoData.VideoEntry.VideoId;
            _field = u.Field;
            _new_value = u.NewValue;
            _old_value = u.OldValue;
            _time = DateTime.Now;
            _title = u.VideoData.VideoEntry.Title.Text;
        }

        public string VideoID
        {
            get { return _video_id; }
        }
        public enumStatField Field
        {
            get { return _field; }
        }
        public double OldValue
        {
            get { return _old_value; }
        }
        public double NewValue
        {
            get { return _new_value; }
        }
        public DateTime Time
        {
            get { return _time; }
        }
        public string Title {
            get { return _title; }
        }
        public string FieldString
        {
            get 
            {
                switch (this._field)
                {
                    case enumStatField.E_UNKNOWN:
                        return "Unknown";
                    case enumStatField.E_VIEWS:
                        return "Views";
                    case enumStatField.E_RATINGS:
                        return "Ratings";
                    case enumStatField.E_FAVORITES:
                        return "Favorites";
                    case enumStatField.E_COMMENTS:
                        return "Comments";
                    case enumStatField.E_AVERAGE_RATING:
                        return "Average Rating";
                    default:
                        return "Error";
                }
            }
        }
    }
    public class clsHistoricalData
    {
        private Dictionary<string, List<clsLightVideoData>> _data_points = new Dictionary<string, List<clsLightVideoData>>();

        public void add_data(clsLightVideoData d)
        {
            if (_data_points.ContainsKey(d.VideoID))
                _data_points[d.VideoID].Add(d);
            else
            {
                _data_points.Add(d.VideoID, new List<clsLightVideoData>());
                _data_points[d.VideoID].Add(d);
            }
        }
        public void add_data(clsDataUpdate d)
        {
            clsLightVideoData ld = new clsLightVideoData(d);
            if (_data_points.ContainsKey(ld.VideoID))
                _data_points[ld.VideoID].Add(ld);
            else
            {
                _data_points.Add(ld.VideoID, new List<clsLightVideoData>());
                _data_points[ld.VideoID].Add(ld);            
            }
        }
        public List<clsLightVideoData> getDataByVideoID(string video_id) 
        {
            if ( _data_points.ContainsKey(video_id) ) 
                return _data_points[video_id];
            else 
                return null; 

        }

        public Dictionary<string, List<clsLightVideoData>> AllData {
            get { return _data_points; }
        }

    }
    public class clsVideoDataEntry
    {
        // private member data
        private YouTubeEntry _entry = null;
        private int _delta_views = 0;
        private int _delta_comments = 0;
        private int _delta_favorites = 0;
        private int _delta_ratings = 0;
        private double _delta_average_rating = 0;
        private DateTime _last_update = DateTime.Now;

        // constructors
        public clsVideoDataEntry(YouTubeEntry video_entry)
        {
            _entry = video_entry;
        }

        // override compareTo (==)
        public override bool Equals(object obj)
        {
            clsVideoDataEntry e = (clsVideoDataEntry)obj;
            return this._entry.VideoId.Equals(e._entry.VideoId);
        }
        public override int GetHashCode()
        {
            return this._entry.VideoId.GetHashCode();
        }
        public override string ToString()
        {
            return "[i] V=" + _entry.VideoId + ",Views=" + this.Views.ToString() + ",Comments=" + this.Comments.ToString() + ",Favorites=" + this.Favorites.ToString() + ",Ratings=" + this.Ratings.ToString() + ",ARating=" + this.AverageRating.ToString();
        }

        // public properties
        public YouTubeEntry VideoEntry
        {
            get { return _entry; }
            set { _entry = value; }
        }
        public int DeltaViews
        {
            get { return _delta_views; }
            set { _delta_views = value; }
        }
        public int DeltaComments
        {
            get { return _delta_comments; }
            set { _delta_comments = value; }
        }
        public int DeltaFavorites
        {
            get { return _delta_favorites; }
            set { _delta_favorites = value; }
        }
        public int DeltaRatings
        {
            get { return _delta_ratings; }
            set { _delta_ratings = value; }
        }
        public double DeltaAverageRating
        {
            get { return _delta_average_rating; }
            set { _delta_average_rating = value; }
        }
        public DateTime LastUpdate {
            get {return _last_update; }
            set { _last_update = value; }
        }
        public int Views
        {
            get { if (_entry.Statistics != null) return int.Parse(_entry.Statistics.ViewCount); else return 0; }
        }
        public int Comments
        {
            get { if (_entry.Comments != null) return _entry.Comments.FeedLink.CountHint; else return 0; }
        }
        public int Favorites
        {
            get
            {
                if (_entry.Statistics != null) return int.Parse(_entry.Statistics.FavoriteCount); else return 0;
            }
        }
        public int Ratings
        {
            get { if (_entry.Rating != null) return _entry.Rating.NumRaters; else return 0; }
        }
        public double AverageRating
        {
            get { if (_entry.Rating != null) return _entry.Rating.Average; else return 0; }
        }

    }
    public class clsDataUpdate
    {
        private double _old_value = 0;
        private double _new_value = 0;
        private enumStatField _field = enumStatField.E_UNKNOWN;
        private clsVideoDataEntry _video_data = null;

        public clsDataUpdate(clsVideoDataEntry video_data, double old_value, double new_value, enumStatField field)
        {
            _old_value = old_value;
            _new_value = new_value;
            _field = field;
            _video_data = video_data;
        }

        public override string ToString()
        {
            return "[u] DT="+DateTime.Now.ToShortDateString() + " -- " + DateTime.Now.ToShortTimeString() + " V="+_video_data.VideoEntry.VideoId+",field=" + this.Field.ToString() + ",init_v=" + this.OldValue.ToString() + ",new_v=" + this.NewValue.ToString();
        }

        public double OldValue
        {
            get { return _old_value; }
        }
        public double NewValue
        {
            get { return _new_value; }
        }
        public enumStatField Field
        {
            get { return _field; }
        }
        public string FieldString
        {
            get 
            {
                switch (this._field)
                {
                    case enumStatField.E_UNKNOWN:
                        return "Unknown";
                    case enumStatField.E_VIEWS:
                        return "Views";
                    case enumStatField.E_RATINGS:
                        return "Ratings";
                    case enumStatField.E_FAVORITES:
                        return "Favorites";
                    case enumStatField.E_COMMENTS:
                        return "Comments";
                    case enumStatField.E_AVERAGE_RATING:
                        return "Average Rating";
                    default:
                        return "Error";
                }
            }
        }
        public double Delta
        {
            get { return _new_value - _old_value; }
        }
        public clsVideoDataEntry VideoData
        {
            get { return _video_data; }
        }
    }
    public class clsVideoDataLogger
    {
        // private member data
        private List<clsVideoDataEntry> _current_data_set = new List<clsVideoDataEntry>();
        private List<clsVideoDataEntry> _initial_data_set = null;
        private clsHistoricalData _historical_data = new clsHistoricalData();
        private clsLogToFile _file_logger = null;

        // constructor
        public clsVideoDataLogger(List<YouTubeEntry> initial_data_set)
        {
            foreach (YouTubeEntry e in initial_data_set)
                _current_data_set.Add(new clsVideoDataEntry(e));
        }

        // public methods
        public void Compare(List<YouTubeEntry> new_data_set)
        {
            List<clsVideoDataEntry> updated_data_set = new List<clsVideoDataEntry>();
            foreach (YouTubeEntry e in new_data_set)
            {
                if (e == null)
                    continue;

                clsVideoDataEntry entry = new clsVideoDataEntry(e);
                int index = -1;
                for (int i = 0; i < _current_data_set.Count - 1; i++)
                {
                    if (_current_data_set[i].VideoEntry.VideoId == entry.VideoEntry.VideoId)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    if (_current_data_set[index].Views != entry.Views)
                    {
                        entry.DeltaViews = entry.Views - _current_data_set[index].Views;
                        EntryUpdate(new clsDataUpdate(entry, _current_data_set[index].Views, entry.Views, enumStatField.E_VIEWS));
                    }
                    if (_current_data_set[index].Comments != entry.Comments)
                    {
                        entry.DeltaComments = entry.Comments - _current_data_set[index].Comments;
                        EntryUpdate(new clsDataUpdate(entry, _current_data_set[index].Comments, entry.Comments, enumStatField.E_COMMENTS));
                    }
                    if (_current_data_set[index].Favorites != entry.Favorites)
                    {
                        entry.DeltaFavorites = entry.Favorites - _current_data_set[index].Favorites;
                        EntryUpdate(new clsDataUpdate(entry, _current_data_set[index].Favorites, entry.Favorites, enumStatField.E_FAVORITES));
                    }
                    if (_current_data_set[index].Ratings != entry.Ratings)
                    {
                        entry.DeltaRatings = entry.Ratings - _current_data_set[index].Ratings;
                        EntryUpdate(new clsDataUpdate(entry, _current_data_set[index].Ratings, entry.Ratings, enumStatField.E_RATINGS));
                    }
                    if (_current_data_set[index].AverageRating != entry.AverageRating)
                    {
                        entry.DeltaAverageRating = entry.AverageRating - _current_data_set[index].AverageRating;
                        EntryUpdate(new clsDataUpdate(entry, _current_data_set[index].AverageRating, entry.AverageRating, enumStatField.E_AVERAGE_RATING));
                    } 
                }
                updated_data_set.Add(entry);
            }
            if (_initial_data_set == null)
            {
                _initial_data_set = new List<clsVideoDataEntry>(_current_data_set);
                if (_file_logger != null)
                    foreach (clsVideoDataEntry e in _initial_data_set)
                        _file_logger.appendFile(e.ToString());
            }

            _current_data_set = new List<clsVideoDataEntry>(updated_data_set);
        }

        // file logging stuff
        public void LogToFile(string filename)
        {
            _file_logger = new clsLogToFile(filename);
            _file_logger.OnError += new clsLogToFile.ErrorEventHandler(_file_logger_OnError);
            _file_logger.Initialize();
        }
        void _file_logger_OnError(object sender, IOException e)
        {
            _file_logger = null;
        }

        // events
        public delegate void EntryUpdateEventHandler(object sender, clsDataUpdate update);
        public event EntryUpdateEventHandler OnItemUpdated;
        protected virtual void EntryUpdate(clsDataUpdate update)
        {
            _historical_data.add_data(update);
            
            if (_file_logger != null)
                _file_logger.appendFile(update.ToString());

            if (OnItemUpdated != null)
                OnItemUpdated(this, update);
        }

        // properties
        public List<clsVideoDataEntry> CurrentDataSet
        {
            get { return new List<clsVideoDataEntry>(_current_data_set); }
        }
        public List<clsVideoDataEntry> InitialDataSet
        {
            get { return new List<clsVideoDataEntry>(_initial_data_set); }
        }
        public clsHistoricalData HistoricalData {
            get { return _historical_data; }
        }
        

    }
    public class clsLogToFile
    {
        private string _file_name = string.Empty;

        public delegate void ErrorEventHandler(object sender, IOException e);
        public event ErrorEventHandler OnError;
        protected virtual void Error(IOException e)
        {
            if (OnError != null)
                OnError(this, e);
        }

        public clsLogToFile(string filename)
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
}
