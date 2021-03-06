﻿using System;
using System.Windows.Input;

namespace MockHelper
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute): this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public virtual void Execute(object parameter)
        {
            _execute(parameter);
        }

        public virtual void Execute()
        {
            _execute(null);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
