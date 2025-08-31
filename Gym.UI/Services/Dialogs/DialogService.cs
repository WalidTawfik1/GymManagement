using System.Windows;
using Gym.UI.Views.Dialogs;
using Gym.UI.Services;

namespace Gym.UI.Services.Dialogs;

public class DialogService : IDialogService
{
    private readonly ILocalizationService _loc;

    public DialogService(ILocalizationService loc)
    {
        _loc = loc;
    }
    public Task ShowAsync(string message, string? title = null, DialogType type = DialogType.Info)
    {
        var owner = Application.Current?.MainWindow;
        var vm = new ModernDialogViewModel
        {
            Title = title ?? _loc.GetString(GetDefaultTitle(type)),
            Message = message,
            Type = type,
            IsConfirmation = false,
            OkText = _loc.GetString("OK"),
            CancelText = _loc.GetString("Cancel"),
            FlowDirection = _loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
        };
        var win = new ModernDialogWindow { Owner = owner, DataContext = vm };
        if (owner == null)
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        win.ShowDialog();
        return Task.CompletedTask;
    }

    public Task<bool> ConfirmAsync(string message, string? title = null)
    {
        var owner = Application.Current?.MainWindow;
        var vm = new ModernDialogViewModel
        {
            Title = title ?? _loc.GetString(GetDefaultTitle(DialogType.Confirm)),
            Message = message,
            Type = DialogType.Confirm,
            IsConfirmation = true,
            OkText = _loc.GetString("OK"),
            CancelText = _loc.GetString("Cancel"),
            FlowDirection = _loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
        };
        var win = new ModernDialogWindow { Owner = owner, DataContext = vm };
        if (owner == null)
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        var result = win.ShowDialog();
        return Task.FromResult(result == true && vm.Result);
    }

    private static string GetDefaultTitle(DialogType type) => type switch
    {
    DialogType.Success => "SuccessTitle",
    DialogType.Warning => "WarningTitle",
    DialogType.Error => "ErrorTitle",
    DialogType.Confirm => "ConfirmTitle",
    _ => "SuccessTitle"
    };
}
