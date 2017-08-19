using System.ComponentModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Codon;

namespace Outcoder.Cryptography.HiddenTextApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
			var vm = Dependency.Resolve<MainViewModel>();
	        ViewModel = vm;
			vm.PropertyChanged += HandleViewModelPropertyChanged;
	        DataContext = vm;
			
            InitializeComponent();

	        encodedTextBox.SelectionHighlightColorWhenNotFocused = new SolidColorBrush(Colors.LawnGreen);
	        encodedTextBox.SelectionHighlightColor = new SolidColorBrush(Colors.LawnGreen);
		}

	    void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
	    {
		    var propertyName = e.PropertyName;
		    if (propertyName == nameof(MainViewModel.PlainText) 
				|| propertyName == nameof(MainViewModel.UseEncryption))
		    {
			    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			    {
					encodedTextBox.SelectAll();
				});
			}
	    }

	    public MainViewModel ViewModel { get; }
    }
}
