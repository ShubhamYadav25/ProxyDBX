using ProxyDBX.Database.Shard;

namespace ProxyDBX.Middleware
{
    public class ShardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ShardManager _shardManager;

        public ShardMiddleware(RequestDelegate next, ShardManager shardManager)
        {
            _next = next;
            _shardManager = shardManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Get)
            {
                string? username = context.Request.Query["userNameQuery"];

                if (!string.IsNullOrEmpty(username))
                {
                    int shardNumber = _shardManager.GetShardNumber(username);
                    string dbPath = _shardManager.GetShardPath(shardNumber);

                    context.Items["ShardDbPath"] = dbPath;
                }
            }

            await _next(context);
        }
    }
}
