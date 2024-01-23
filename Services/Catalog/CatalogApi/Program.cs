using CatalogApi.AsyncDataServices;
using CatalogApi.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



string connString()
{
    var csbuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("CatalogMssqlConn"))
    {
        Password = "LONG3ST_pa55w0rd"
    };

    return csbuilder.ConnectionString;
}


builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connString()));
builder.Services.AddCors();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IStoreRepo, StoreRepo>();
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();
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

app.UseCors(options => options
    .WithOrigins(new[] { "http://localhost:3000" })
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

app.UseAuthorization();

app.MapControllers();

PrepDb.PopulateDb(app);

app.Run();
