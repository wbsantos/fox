using System;
using System.Data;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Dapper;
using Npgsql;

namespace DB.Fox;

public class DBConnection
{
	private IDbConnection Instance { get; set; }

	public DBConnection(DBSettings settings)
	{
		Instance = new NpgsqlConnection(settings.ConnectionString);
	}

	public IEnumerable<T> Procedure<T>(string procedureName, object parameters)
	{
        return Instance.Query<T>(BuildProcedureSQL(procedureName, parameters),
				   			     param: parameters,
								 commandType: CommandType.Text);
    }

    public T ProcedureFirst<T>(string procedureName, object parameters)
    {
        return Instance.QueryFirst<T>(BuildProcedureSQL(procedureName, parameters),
									  param: parameters,
									  commandType: CommandType.Text);
    }

    private string BuildProcedureSQL(string procedureName, object parameters)
	{
		return $"SELECT {procedureName}({GetParametersNames(parameters)})";
    }

	private string GetParametersNames(object parameters)
	{
		if (parameters == null)
			return string.Empty;
		string[] names = parameters.GetType()
								   .GetProperties()
						           .Select(p => $"{p.Name} => @{p.Name}")
								   .ToArray();
		return string.Join(',', names);
	}
}

