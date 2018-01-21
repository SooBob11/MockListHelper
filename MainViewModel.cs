using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MockHelper.Annotations;


namespace MockHelper
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            AvailableTypes = GetAvialibleTypes();
            SelectedType = AvailableTypes[0];
            SqlStatement = "SELECT * FROM ....";
            ConnectionString = "Data Source=localhost;Initial Catalog=fotosteam;Integrated Security=true";

            SqlCommand = new DelegateCommand(ExecuteSqlCommand, c => true);
            GenerateCommand = new DelegateCommand(ExecuteGenerateCommand, CanExecuteGenerateCommand);
        }

        public bool CanExecuteGenerateCommand(object parameter)
        {           
            return Data != null && Data.Rows.Count > 0; 
        }

        private List<string> GetAvialibleTypes()
        {
            return new List<string>() { "Test" };
        }

        private DataTable _data;
        private string _listDefinition;
        public string SqlStatement { get; set; }

        public DataTable Data
        {
            get => _data;
            set
            {

                _data = value;
                OnPropertyChanged();
                (GenerateCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }

        private void ExecuteSqlCommand(object parameter)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    using (var command = new SqlCommand(SqlStatement))
                    {
                        command.Connection = connection;
                        connection.Open();
                        using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            var data = new DataTable();
                            data.Load(reader);
                            Data = data;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ListDefinition = ex.ToString();
            }
        }

        private void ExecuteGenerateCommand(object parameter)
        {
            var builder = new StringBuilder();
            var startIndex = SqlStatement.ToLower().IndexOf("from ", StringComparison.Ordinal) + 5;
            var endIndex = SqlStatement.IndexOf(" ", startIndex + 1, StringComparison.Ordinal);
            if (endIndex <= 0)
                endIndex = SqlStatement.Length;

            var type = SqlStatement.Substring(startIndex, (endIndex - startIndex));

            builder.Append($"new List<{type}>{{");
            foreach (DataRow row in Data.Rows)
            {
                builder.Append($"new {type} {{");
                foreach (DataColumn column in Data.Columns)
                {
                    if (row[column.ColumnName] is DBNull)
                        continue;

                    builder.Append($"{column.ColumnName} = ");
                    if (column.DataType == typeof(string) || column.DataType == typeof(Guid))
                    {
                        builder.Append($"\"{row[column.ColumnName]}\", ");
                    }
                    else if (column.DataType == typeof(Boolean))
                    {
                        builder.Append($" {row[column.ColumnName].ToString().ToLower()}, ");
                    }
                    else
                    {
                        builder.Append($" {row[column.ColumnName]}, ");
                    }
                }
                builder.Remove(builder.Length - 2, 2);
                builder.Append("},\n");
            }
            builder.Remove(builder.Length - 2, 2);
            builder.Append("});");
            ListDefinition = builder.ToString();
            Clipboard.SetText(ListDefinition);
        }
        public List<string> AvailableTypes { get; set; }
        public string SelectedType { get; set; }

        public string ListDefinition
        {
            get => _listDefinition;
            set
            {
                _listDefinition = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand SqlCommand { get; }
        public ICommand GenerateCommand { get; }

        public string ConnectionString { get; set; }
    }
}
