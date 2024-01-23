using IdentityApi.Data;
using IdentityApi.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connString()
{
    var csbuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("IdentityMssqlConn"))
    {
        Password = "LONG3ST_pa55w0rd"
    };
    return csbuilder.ConnectionString;
}

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connString()));
// builder.Services.AddDbContext<AddressContext>(opt => opt.UseSqlServer(connString()));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGrpcService<IdentityGrpcService>();

app.UseHttpsRedirection();

app.UseCors(options => options
    .WithOrigins(["http://localhost:3000",
                  "https://127.0.0.1:3000"])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

app.UseAuthorization();

app.MapControllers();

PrepDb.PopulateDb(app);

app.Run();
