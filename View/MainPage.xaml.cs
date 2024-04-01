using MonkeyFinder.ViewModel;

namespace MonkeyFinder
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MonkeysViewModel viewModel)
        {
            InitializeComponent();

            // Link view model to main page
            BindingContext = viewModel;
        }


    }

}
