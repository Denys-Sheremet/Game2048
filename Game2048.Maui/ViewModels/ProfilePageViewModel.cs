using Game2048.Maui.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.ViewModels;

public partial class ProfilePageViewModel : BindableObject
{
    private readonly IProfileManager _profileManager;
    private readonly IThemesManager _themesManager;



    public ProfilePageViewModel(IProfileManager profileManager, IThemesManager themeManager)
    {
        _profileManager = profileManager;
        _themesManager = themeManager;
    }


}
