using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using VDMP.App.ViewModels;
using VDMP.DBmodel;

namespace VDMP.App.Views
{
    public sealed partial class LibraryMenuPage : Page
    {
        public LibraryMenuPage()
        {
            InitializeComponent();
        }

        public LibraryMenuViewModel ViewModel { get; } = new LibraryMenuViewModel();


        private async void AddLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            var searchDialog = new SearchDialog
            {
                Title = "Name of library?",
                PrimaryButtonText = "Ok"
            };
            var result = await searchDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var text = searchDialog.TextInput;

                await ViewModel.AddANewLibraryAsync(text);
            }
        }

        private async void ShowOptions(object sender, RightTappedRoutedEventArgs e)
        {
            var menu = new PopupMenu();
            menu.Commands.Add(new UICommand("Rename?", command =>
            {
                if ((sender as RelativePanel).DataContext is Library librarySelected)
                    ViewModel.UpdateSelectedLibraryAsync(librarySelected);
            }));
            menu.Commands.Add(new UICommandSeparator());
            menu.Commands.Add(new UICommand("Delete", command =>
            {
                if ((sender as RelativePanel).DataContext is Library librarySelected)
                    ViewModel.DeleteCommand.Execute(librarySelected);
            }));

            await menu.ShowAsync(e.GetPosition(null));
        }
    }
}
