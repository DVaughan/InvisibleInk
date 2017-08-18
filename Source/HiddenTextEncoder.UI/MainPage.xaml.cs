using Windows.UI.Xaml.Controls;
using Codon;

namespace HiddenTextEncoder.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
	        ViewModel = Dependency.Resolve<MainViewModel>();

            this.InitializeComponent();
        }

	    public MainViewModel ViewModel { get; }
    }
}
