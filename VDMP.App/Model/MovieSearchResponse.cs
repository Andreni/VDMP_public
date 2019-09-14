using System.Collections.Generic;

namespace VDMP.App.Model
{
    public class Result
    {
        public bool Adult { get; set; }
        public string Backdrop_path { get; set; }
        public int id { get; set; }
        public string Original_title { get; set; }
        public string Release_date { get; set; }
        public string Poster_path { get; set; }
        public double Popularity { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; }
        public double Vote_average { get; set; }
        public int Vote_count { get; set; }
    }

    public class RootObject
    {
        public int Page { get; set; }
        public List<Result> Results { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
    }
}