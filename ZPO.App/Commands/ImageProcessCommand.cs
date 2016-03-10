using System;
using System.Windows.Input;
using ZPO.App.ViewModels;

namespace ZPO.App.Commands
{
    public abstract class ImageProcessCommand : ICommand
    {
        protected readonly MainViewModel ViewModel;

        protected ImageProcessCommand(MainViewModel viewModel)
        {
            ViewModel = viewModel;

            ViewModel.CurrentImageChanged += 
                (sender, args) => OnExecuteChanged();
        }

        public virtual bool CanExecute(object parameter = null) => ViewModel?.CurrentImage != null;

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        protected void OnExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}