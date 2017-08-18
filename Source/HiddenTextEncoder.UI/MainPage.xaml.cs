using System;
using System.ComponentModel;
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
			var vm = Dependency.Resolve<MainViewModel>();
	        ViewModel = vm;
			vm.PropertyChanged += HandleViewModelPropertyChanged;
	        DataContext = vm;
			

            this.InitializeComponent();
        }

	    void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
	    {
		    var propertyName = e.PropertyName;
		    if (propertyName == nameof(MainViewModel.EncodedText))
		    {
				encodedTextBox.SelectAll();
			}
	    }

	    public MainViewModel ViewModel { get; }
    }
}
