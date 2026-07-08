using System;

namespace Gym.Web.Services
{
    public class ToastService
    {
        public event Action<ToastMessage>? OnShow;

        public void ShowSuccess(string message, string title = "نجاح")
        {
            ShowToast(ToastLevel.Success, message, title);
        }

        public void ShowError(string message, string title = "خطأ")
        {
            ShowToast(ToastLevel.Error, message, title);
        }

        public void ShowInfo(string message, string title = "معلومة")
        {
            ShowToast(ToastLevel.Info, message, title);
        }

        public void ShowWarning(string message, string title = "تحذير")
        {
            ShowToast(ToastLevel.Warning, message, title);
        }

        private void ShowToast(ToastLevel level, string message, string title)
        {
            OnShow?.Invoke(new ToastMessage { Id = Guid.NewGuid(), Level = level, Message = message, Title = title });
        }
    }

    public class ToastMessage
    {
        public Guid Id { get; set; }
        public ToastLevel Level { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public enum ToastLevel
    {
        Info,
        Success,
        Warning,
        Error
    }
}
