using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;
using System.Windows;
using Gym.UI.Services.Dialogs;

namespace Gym.UI.Views.Dialogs;

public partial class ModernDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private DialogType _type;
    [ObservableProperty] private bool _isConfirmation;
    [ObservableProperty] private bool _result;
    [ObservableProperty] private string _okText = string.Empty;
    [ObservableProperty] private string _cancelText = string.Empty;
    [ObservableProperty] private FlowDirection _flowDirection = FlowDirection.LeftToRight;

    public Brush IconBrush => Type switch
    {
        DialogType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E")),
        DialogType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")),
        DialogType.Error => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
        DialogType.Confirm => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1")),
        _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"))
    };

    [RelayCommand]
    private void Ok()
    {
        Result = true;
        CloseOwner(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        Result = false;
        CloseOwner(false);
    }

    private void CloseOwner(bool? dialogResult)
    {
        var win = System.Windows.Application.Current?.Windows
            .OfType<ModernDialogWindow>()
            .FirstOrDefault(w => w.DataContext == this);
        if (win != null)
        {
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
