namespace Gym.UI.Services.Dialogs;

public enum DialogType
{
    Info,
    Success,
    Warning,
    Error,
    Confirm
}

public interface IDialogService
{
    Task ShowAsync(string message, string? title = null, DialogType type = DialogType.Info);
    Task<bool> ConfirmAsync(string message, string? title = null);
}
