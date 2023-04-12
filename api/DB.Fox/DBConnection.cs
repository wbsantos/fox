using System;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;

namespace DB.Fox;

public class DBConnection
{
	private IDbConnection Instance { get; set; }

	public DBConnection(DBSettings settings)
	{
        Instance = new NpgsqlConnection(settings.ConnectionString);
		Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
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

	private static string GetParametersNames(object parameters)
	{
		if (parameters == null)
			return string.Empty;
		string[] names = parameters.GetType()
								   .GetProperties()
						           .Select(p => $"{ToSnakeCase(p.Name)} => @{p.Name}")
								   .ToArray();
		return string.Join(',', names);
	}

	private enum QueryType
	{
		Procedure,
		Function,
		Text
	}

	public static void Initialize(DBSettings settings, bool autoCreateProcedures, IEnumerable<Type>? customTypes)
	{
		if (customTypes != null)
		{
			foreach (var type in customTypes)
#pragma warning disable CS0618 // Type or member is obsolete
                NpgsqlConnection.GlobalTypeMapper.MapComposite(type, ToSnakeCase(type.Name));
#pragma warning restore CS0618 // Type or member is obsolete
        }
		if(autoCreateProcedures)
			new DBConnection(settings).CreateProcedures();
    }

	private void CreateProcedures()
	{
		string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location) ?? string.Empty, "database");
		string[] sqlFiles =  System.IO.Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories);
		IEnumerable<string> currentProcedures = GetCurrentProcedures();
		List<string> proceduresFailed = new List<string>();

		foreach (var sqlFile in sqlFiles)
		{
			string sql = File.ReadAllText(sqlFile);

            string pattern = @"(PROCEDURE|FUNCTION) +([a-z0-9_]+)";
			Match? match = Regex.Matches(sql, pattern, RegexOptions.IgnoreCase).FirstOrDefault();
			if (match == null)
				continue;

			if (!currentProcedures.Contains(match.Groups[2].Value))
			{
				try
				{
					Instance.Execute(sql);
				}
				catch
				{
					proceduresFailed.Add(match.Groups[2].Value);
				}
			}
        }

        currentProcedures = GetCurrentProcedures();
		proceduresFailed = proceduresFailed.Where(p => !currentProcedures.Contains(p)).ToList();
		if (proceduresFailed.Count > 0)
			throw new Exception($"The following procedures couldn't be created: {string.Join(", ", proceduresFailed.ToArray())}");
    }

	private IEnumerable<string> GetCurrentProcedures()
	{
		return Instance.Query<string>("SELECT routine_name FROM information_schema.routines WHERE routine_type in ('FUNCTION', 'PROCEDURE') AND routine_schema = 'public';", commandType: CommandType.Text);
	}

    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
			return input;
		return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}

