using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using VDMP.App.Helpers;
using VDMP.App.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VDMP.App.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResponse : ContentDialog
    {
        private ICommand _itemClickCommand;


        public ObservableCollection<Result> Source;

        public SearchResponse()
        {
            InitializeComponent();
            Source = new ObservableCollection<Result>();
        }

        public IAsyncInfo Info { get; set; }

        public ICommand ItemClickCommand =>
            _itemClickCommand ?? (_itemClickCommand = new RelayCommand<Result>(OnItemClick));

        public Result Result { get; private set; }

        private void OnItemClick(Result obj)
        {
            var obj1 = obj;
        }


        private void close()
        {
            Info.Close();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Result = (Result) e.ClickedItem;
        }


        private void SearchResponse_OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
