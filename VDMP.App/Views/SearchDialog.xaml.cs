using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VDMP.App.Views
{
    public sealed partial class SearchDialog : ContentDialog
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Enter title", typeof(string), typeof(SearchDialog), new PropertyMetadata(default(string)));

        public SearchDialog()
        {
            InitializeComponent();
        }

        public string TextInput
        {
            get => GetValue(UserInput.Text);
            set => SetValue(TextProperty, value);
        }

        private string GetValue(string text)
        {
            return text;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
