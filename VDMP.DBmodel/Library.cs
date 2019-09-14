using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using VDMP.DBmodel.Annotations;

namespace VDMP.DBmodel
{
    [Table("Libraries")]
    public class Library : INotifyPropertyChanged
    {
        private string _libraryName;

        public Library(string libraryName, string libraryType)
        {
            LibraryName = libraryName;
            LibraryType = libraryType;
        }

        [Key] public int LibraryId { get; set; }

        [Required]
        public string LibraryName
        {
            get => _libraryName;
            set
            {
                _libraryName = value;
                OnPropertyChanged(nameof(LibraryName));
            }
        }

        [ForeignKey("User")] // Add ForeignKey attribute
        public string UserId { get; set; }

        [Required] public string LibraryType { get; set; }

        public ICollection<Movie> LibraryOfMovies { get; } = new List<Movie>();
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}