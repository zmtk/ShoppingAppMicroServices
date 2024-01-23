using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderingApi.Data;
using OrderingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string connString()
{
    var csbuilder = new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("OrderingPostgresConn"))
    {
        Password = "LONG1SH_pa55w0rd"
    };
    return csbuilder.ConnectionString;
}

builder.Services.AddCors();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connString()));
builder.Services.AddScoped<IOrderRepository,OrderRepository>();
builder.Services.AddScoped<IGrpcClient,GrpcClient>();

builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGrpcService<OrderingGrpcService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options => options
    .WithOrigins(["http://localhost:3000"])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

app.UseAuthorization();

app.MapControllers();

PrepDb.MigrateDb(app);

app.Run();
