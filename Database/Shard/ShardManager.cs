using Microsoft.Data.Sqlite;
using Dapper;
using ProxyDBX.Database.Domain;

namespace ProxyDBX.Database.Shard
{
    public class ShardManager
    {
        // Sharding by first letter of username
        private static readonly Dictionary<int, string> _shards = new Dictionary<int, string>
        {
            { 1, "Data/shard1.db" }, // A-E
            { 2, "Data/shard2.db" }, // F-J
            { 3, "Data/shard3.db" }, // K-O
            { 4, "Data/shard4.db" }, // P-T
            { 5, "Data/shard5.db" }  // U-Z
        };

        public ShardManager()
        {
            foreach (var dbPath in _shards.Values)
            {
                if (!File.Exists(dbPath))
                {
                    CreateDatabase(dbPath);
                }
            }
        }

        private void CreateDatabase(string dbPath)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Name TEXT NOT NULL,
                        Age INTEGER NOT NULL,
                        Location TEXT NOT NULL,
                        Country TEXT NOT NULL,
                        CreatedDate TEXT NOT NULL DEFAULT (datetime('now'))
                    )";
                command.ExecuteNonQuery();
            }
        }

        private int GetShardNumber(string username)
        {
            char firstLetter = char.ToLower(username[0]);
            if (firstLetter >= 'a' && firstLetter <= 'e') return 1;
            if (firstLetter >= 'f' && firstLetter <= 'j') return 2;
            if (firstLetter >= 'k' && firstLetter <= 'o') return 3;
            if (firstLetter >= 'p' && firstLetter <= 't') return 4;
            
            // last shard U-Z
            return 5;
        }

        public void AddUser(User user)
        {
            int shardNumber = GetShardNumber(user.Username);
            string dbPath = _shards[shardNumber];

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                connection.Execute(
                    "INSERT INTO Users (Username, Name, Age, Location, Country, CreatedDate) VALUES (@Username, @Name, @Age, @Location, @Country, @CreatedDate)",
                    user);
            }
        }

        public List<User> GetUsersByShard(int shardNumber)
        {
            if (!_shards.ContainsKey(shardNumber)) return new List<User>();

            string dbPath = _shards[shardNumber];
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                return connection.Query<User>("SELECT * FROM Users").ToList();
            }
        }
    }
}
