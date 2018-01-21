using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MockHelper
{
    internal class MainModel
    {
        internal MainModel()
        {
            AvailableTypes = LoadAvialibleTypes();
            SelectedType = AvailableTypes[0];
            SqlStatement = "SELECT * FROM ....";
            ConnectionString = "Data Source=localhost;Initial Catalog=fotosteam;Integrated Security=true";
        }

        internal bool HasData => Data != null && Data.Rows.Count > 0;

        private List<string> LoadAvialibleTypes()
        {
            return new List<string>() { "Test" };
        }

        internal string SqlStatement { get; set; }

        internal DataTable Data { get; set; }

        internal void ExecuteSqlCommand()
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

        internal void GenerateList()
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
        }
        internal List<string> AvailableTypes { get; set; }
        internal string SelectedType { get; set; }

        internal string ListDefinition { get; set; }


        internal string ConnectionString { get; set; }
    }
}

