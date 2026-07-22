using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using Game2048.Maui.Interfaces;

namespace Game2048.Maui.ViewModels;

public class LangSelectViewModel : ObservableObject
{
    private readonly ISettingsManager _settingsManager;
    public event Action? IsLangSelected;
    public IRelayCommand SelectLanguageCommand { get; private set; }

    public LangSelectViewModel(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        SelectLanguageCommand = new RelayCommand<string>(SelectLanguage);
    }

    private void SelectLanguage(string? langCode)
    {
        if (langCode is null) return;
        _settingsManager.SetLanguage(langCode);

        IsLangSelected?.Invoke();
    }
}
