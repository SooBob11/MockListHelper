using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;


namespace MockHelper
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MainModel _model = new MainModel();

        public MainViewModel()
        {
            SqlCommand = new DelegateCommand(ExecuteSqlCommand, c => true);
            GenerateCommand = new DelegateCommand(ExecuteGenerateCommand, CanExecuteGenerateCommand);
        }

        public string ConnectionString
        {
            get => _model.ConnectionString;
            set => _model.ConnectionString = value;
        }

        public bool CanExecuteGenerateCommand(object parameter)
        {
            return Data != null && Data.Rows.Count > 0;
        }

        
        public string SqlStatement
        {
            get => _model.SqlStatement;
            set => _model.SqlStatement = value;
        }

        public DataTable Data
        {
            get => _model.Data;
            set
            {

                _model.Data = value;
                OnPropertyChanged();
                (GenerateCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }

        private void ExecuteSqlCommand(object parameter)
        {
            _model.ExecuteSqlCommand();
            Data = _model.Data;
        }

        private void ExecuteGenerateCommand(object parameter)
        {
            _model.GenerateList();
            ListDefinition = _model.ListDefinition;
            Clipboard.SetText(ListDefinition);
        }
        public List<string> AvailableTypes { get; set; }
        public string SelectedType { get; set; }

        public string ListDefinition
        {
            get => _model.ListDefinition;
            set
            {
                _model.ListDefinition = value;
                OnPropertyChanged();
            }
        }



        public ICommand SqlCommand { get; }
        public ICommand GenerateCommand { get; }

    }
}
