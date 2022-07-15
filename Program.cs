using Catalogue.Repositories;
using Catalogue.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
// Add services to the container.
//builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection)
builder.Services.AddSingleton<IMongoClient>(ServiceProvider=>{
    var settings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
    return new MongoClient(settings.ConnectionString);
});
//builder.Services.AddSingleton<IItemsRepo, InMemItemsRepo>();
builder.Services.AddSingleton<IItemsRepo, MongoDbItemsRepo>();
builder.Services.AddControllers(options => {
    options.SuppressAsyncSuffixInActionNames=false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
