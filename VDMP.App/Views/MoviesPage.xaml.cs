using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using VDMP.App.Services;
using VDMP.App.ViewModels;
using VDMP.DBmodel;

namespace VDMP.App.Views
{
    public sealed partial class MoviesPage : Page
    {
        public MoviesPage()
        {
            InitializeComponent();
            DataContext = ViewModel.CollectionMovies;
        }

        public MovieViewModel ViewModel { get; } = new MovieViewModel();


        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AddCollection();
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AddSingleItem();
        }


        private async void ShowOptions(object sender, RightTappedRoutedEventArgs e)
        {
            var menu = new PopupMenu();
            menu.Commands.Add(new UICommand("Update", command =>
            {
                if ((sender as StackPanel).DataContext is Movie mediaSelected) UpdateSelectedMedia(mediaSelected);
            }));
            menu.Commands.Add(new UICommandSeparator());
            menu.Commands.Add(new UICommand("Delete", command =>
            {
                if ((sender as StackPanel).DataContext is Movie mediaSelected)
                    ViewModel.DeleteCommand.Execute(mediaSelected);
            }));

            await menu.ShowAsync(e.GetPosition(null));
        }

        private async void UpdateSelectedMedia(Movie mediaSelected)
        {
            // Query user for what name should be used for the search
            var searchDialog = new SearchDialog {Title = mediaSelected.TitleOfMovie};
            var result = await searchDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var text = searchDialog.TextInput;
                var item = ViewModel.CollectionMovies.FirstOrDefault(i => i.PathToVideo == mediaSelected.PathToVideo);
                ViewModel.UpdateSingleItem(item, text);
                //Bindings.Update();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int LibraryId) ViewModel.Initialize(LibraryId);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
        }
    }
}
