using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.ViewModels.Items;

public partial class PurchaseThemeOverlayViewModel : ObservableObject
{
    private ThemeItemViewModel? _themeItem;
    public ThemeItemViewModel? ThemeItem
    {
        get { return _themeItem; }
        set { _themeItem = value; OnPropertyChanged(); }
    }

    public IRelayCommand? BuyCommand { get; set; }
}
