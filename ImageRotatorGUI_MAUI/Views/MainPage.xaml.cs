using ImageRotatorGUI_MAUI.ViewModels;
using ImageRotatorBackend.Services;
using System.Diagnostics;

namespace ImageRotatorGUI_MAUI.Views;

public partial class MainPage : ContentPage
{
    #region CONSTRUCTORS
    public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
    #endregion
}

