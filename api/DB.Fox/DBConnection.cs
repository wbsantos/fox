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
        return Instance.Query<T>(BuildSQL(QueryType.Function, procedureName, parameters),
				   			     param: parameters,
								 commandType: CommandType.Text);
    }

    public IEnumerable<dynamic> Procedure(string procedureName, object parameters)
    {
        return Instance.Query(BuildSQL(QueryType.Function, procedureName, parameters),
                              param: parameters,
                              commandType: CommandType.Text);
    }

    public T ProcedureFirstOrDefault<T>(string procedureName, object parameters)
    {
        return Instance.QueryFirstOrDefault<T>(BuildSQL(QueryType.Function, procedureName, parameters),
											   param: parameters,
											   commandType: CommandType.Text);
		
    }

    public T ProcedureFirst<T>(string procedureName, object parameters)
    {
        return Instance.QueryFirst<T>(BuildSQL(QueryType.Function, procedureName, parameters),
									  param: parameters,
									  commandType: CommandType.Text);
    }

	public int ProcedureExecute(string procedureName, object parameters)
	{
		return Instance.Execute(BuildSQL(QueryType.Procedure, procedureName, parameters),
								param: parameters,
								commandType: CommandType.Text);

	}

    private string BuildSQL(QueryType queryType, string sqlQuery, object parameters)
	{
		switch (queryType)
		{
			case QueryType.Function:
                return $"SELECT * FROM {sqlQuery}({GetParametersNames(parameters)})";
			case QueryType.Procedure:
                return $"CALL {sqlQuery}({GetParametersNames(parameters)})";
            default:
				return sqlQuery;
		}
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

	private enum QueryType
	{
		Procedure,
		Function,
		Text
	}
}

