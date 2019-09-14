using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using VDMP.DBmodel;

namespace VDMP.App.Model
{
    public class MoviesObservable<T> : ObservableCollection<Movie> where T : INotifyPropertyChanged
    {
        private MoviesObservable<Movie> _moviesObservableList;

        public MoviesObservable<Movie> MoviesObservableList
        {
            get => _moviesObservableList;
            set
            {
                _moviesObservableList = value;
                _moviesObservableList.CollectionChanged += OnObservableCollectionChanged;
            }
        }

        public void OnObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    ((INotifyPropertyChanged) item).PropertyChanged += OnItemPropertyChanged;

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    ((INotifyPropertyChanged) item).PropertyChanged -= OnItemPropertyChanged;
        }

        public void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender,
                    IndexOf((Movie) sender));
            OnCollectionChanged(args);
        }
    }
}