using Microsoft.Data.Sqlite;
using Dapper;
using System;
using System.Collections.Generic;
using ProxyDBX.SQLParser;

namespace ProxyDBX.Database.SQL;

public class SQLManager
{
    private readonly Dictionary<int, string> _shardMap = new()
    {
        { 1, "Data/shard1.db" }, // A-E
        { 2, "Data/shard2.db" }, // F-J
        { 3, "Data/shard3.db" }, // K-O
        { 4, "Data/shard4.db" }, // P-T
        { 5, "Data/shard5.db" }  // U-Z
    };

    private string GetShardDatabase(string username)
    {
        char firstLetter = char.ToUpper(username[0]);
        int shardKey = firstLetter switch
        {
            >= 'A' and <= 'E' => 1,
            >= 'F' and <= 'J' => 2,
            >= 'K' and <= 'O' => 3,
            >= 'P' and <= 'T' => 4,
            >= 'U' and <= 'Z' => 5,
            _ => throw new Exception("Invalid username")
        };

        return _shardMap[shardKey];
    }

    // Execute SQL query on the correct shard
    public IEnumerable<dynamic> ExecuteQuery(string sqlQuery, string username)
    {
        string optimizedQuery = SqlOptimizer.OptimizeQuery(sqlQuery);
        string dbPath = GetShardDatabase(username);

        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        return connection.Query(optimizedQuery);
    }
}
