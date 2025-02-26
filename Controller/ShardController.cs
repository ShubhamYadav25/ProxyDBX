using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using ProxyDBX.Database.Domain;
using ProxyDBX.Database.Shard;

namespace ProxyDBX.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShardController : ControllerBase
    {
        private readonly ShardManager _shardManager;

        public ShardController()
        {
            _shardManager = new ShardManager();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Invalid user data.");

            _shardManager.AddUser(user);
            return Ok("User added successfully.");
        }

        [HttpGet("{shardNumber}")]
        public IActionResult GetUsersByShard(int shardNumber)
        {
            var users = _shardManager.GetUsersByShard(shardNumber);
            return Ok(users);
        }

        [HttpPost("add")]
        public IActionResult AddUser([FromQuery] string userNameQuery, [FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Username))
                return BadRequest("Username is required");

            // Get shard DB path from middleware
            string? dbPath = HttpContext.Items["ShardDbPath"] as string;
            if (dbPath == null)
                return BadRequest("Could not determine shard database");

            _shardManager.AddUser(user, databasePath: dbPath);

            return Ok("User added successfully.");
        }

    }
}
