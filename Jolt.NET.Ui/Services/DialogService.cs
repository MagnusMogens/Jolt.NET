using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Views;

namespace Jolt.NET.UI.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            string message = string.Format("{0}\n\n{1}", error.Message, error);
            return Task.FromResult(ShowError(message, title, buttonText, afterHideCallback));
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            afterHideCallback?.Invoke();
            return Task.FromResult(result);
        }

        public Task ShowMessage(string message, string title)
        {
            return Task.FromResult(ShowMessage(message, title, "", null));
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            var result = MessageBox.Show(message, title);
            afterHideCallback?.Invoke();
            return Task.FromResult(result);
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            afterHideCallback?.Invoke(result);
            return Task.FromResult(result);
        }

        public Task ShowMessageBox(string message, string title)
        {
            return Task.FromResult(MessageBox.Show(message, title));
        }
    }
}
