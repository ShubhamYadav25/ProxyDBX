using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using ProxyDBX.Database.SQL;
using ProxyDBX.SQLParser;

namespace ProxyDBX.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlQueryController : ControllerBase
    {
        private readonly SQLManager _sqlManager;

        public SqlQueryController()
        {
            _sqlManager = new SQLManager();
        }

        [HttpPost("execute")]
        public IActionResult ExecuteQuery([FromBody] SqlQueryRequest request)
        {
            try
            {
                IEnumerable<dynamic> result = _sqlManager.ExecuteQuery(request.SqlQuery, request.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

public class SqlQueryRequest
{
    public string SqlQuery { get; set; }
    public string Username { get; set; }
}
