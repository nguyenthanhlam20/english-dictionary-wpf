using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FinancialWPFApp.UI
{
    public class ReplayCommand : ICommand
    {

        [MaybeNull]
        readonly Action<object> _execute;

        [MaybeNull]
        readonly Predicate<object> _canExecute;

       

        public ReplayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public ReplayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
           _execute.Invoke(parameter);
        }
    }
}
