using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace VDMP.DBmodel
{
    /// <summary>Movie class used in database and app</summary>
    /// <remarks>Inherits from the Video class </remarks>
    [Table("Movie")]
    public class Movie : Video, INotifyPropertyChanged
    {
        public enum GenreTypes
        {
            Action = 28,
            Adventure = 12,
            Animation = 16,
            Comedy = 35,
            Crime = 80,
            Documentary = 99,
            Drama = 18,
            Family = 10751,
            Fantasy = 14,
            History = 36,
            Horror = 27,
            Music = 10402,
            Mystery = 9648,
            Romance = 10749,
            ScienceFiction = 878,
            TVMovie = 10770,
            Thriller = 53,
            War = 10752,
            Western = 37
        }

        public Movie(string fileName, string pathToVideo)
        {
            FileName = fileName;
            PathToVideo = pathToVideo;
        }

        public Movie()
        {
        }

        [Key] [JsonProperty(null)] public int MovieId { get; set; }

        [ForeignKey("LibraryId")] public int LibraryId { get; set; }

        [JsonProperty("backdrop_path")] public string BackdropPath { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("id")]
        public int TMDbId { get; set; }

        [JsonProperty("original_language")] public string Language { get; set; }

        [JsonProperty("original_title")] public string OriginalTitle { get; set; }

        [JsonProperty("overview")] public string Overview { get; set; }

        [JsonProperty("poster_path")] public string PosterPath { get; set; }

        private string _gridPosterImageSource { get; set; }

        private string _backdropImageSource { get; set; }

        [JsonProperty("release_date")] public DateTimeOffset ReleaseDate { get; set; }

        [JsonProperty("runtime")] public string Runtime { get; set; }

        [JsonProperty("tagline")] public string TagLine { get; set; }

        [JsonProperty("title")] private string _title { get; set; }

        [NotMapped] [JsonProperty("genres")] public Genre[] GenresMovie { get; set; }

        public int genre { get; set; }

        public GenreTypes type { get; set; }

        public string TitleOfMovie
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string GridPosterImageSource
        {
            get => _gridPosterImageSource;
            set
            {
                _gridPosterImageSource = value;
                OnPropertyChanged();
            }
        }

        public string BackdropImageSource
        {
            get => _backdropImageSource;
            set
            {
                _backdropImageSource = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}