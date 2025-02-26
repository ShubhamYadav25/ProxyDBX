using ProxyDBX.Database.Shard;
using ProxyDBX.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ShardManager>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseMiddleware<ShardMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();