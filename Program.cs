using System.Net.Mime;
using System.Text.Json;
using Catalogue.Repositories;
using Catalogue.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
// Add services to the container.
//builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection)
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.AddSingleton<IMongoClient>(ServiceProvider=>{
    
    return new MongoClient(mongoDbSettings.ConnectionString);
});
//builder.Services.AddSingleton<IItemsRepo, InMemItemsRepo>();
builder.Services.AddSingleton<IItemsRepo, MongoDbItemsRepo>();
builder.Services.AddControllers(options => {
    options.SuppressAsyncSuffixInActionNames=false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddMongoDb(
        mongoDbSettings.ConnectionString,
        name: "mongoDb",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[]{"ready"}
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/ready", new HealthCheckOptions{
    Predicate=(check)=> check.Tags.Contains("ready"),
    ResponseWriter= async(context, report)=>{
        var result = JsonSerializer.Serialize(
            new {
                status=report.Status.ToString(),
                checks=report.Entries.Select(entry=>new{
                    name=entry.Key,
                    status=entry.Value.Status.ToString(),
                    exception=entry.Value.Exception!=null ? entry.Value.Exception.Message : "none",
                    duration=entry.Value.Duration.ToString()
                })
            }
        );
        context.Response.ContentType=MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    }
});
app.MapHealthChecks("/health/live", new HealthCheckOptions{
    Predicate=(_)=> false
});

app.Run();
