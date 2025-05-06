using System.Data;
using Dapper;

namespace EAVPipeline.Core.Utilites;

public static class SqlUtility{
    public static void ExecuteNonQuery(IDbConnection connection, string sql, object? parameters = null){
        connection.Execute(sql,parameters);
        Console.WriteLine("[SQL EXECUTED] " + sql);
    }

    public static IEnumerable<T> Query<T>(IDbConnection connection, string sql, object? parameters=null){
        return connection.Query<T>(sql,parameters);
    }

    public static void PrintQueryResults(IDbConnection connection,string sql, object? parameters = null){
        var rows = connection.Query(sql,parameters);
        foreach (var row in rows) {
            Console.WriteLine(string.Join(" | ",((IDictionary<string,object>) row).Select(kv => $"{kv.Key}: {kv.Value}")));
        }
    }
}