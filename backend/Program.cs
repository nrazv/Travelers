using backend.Data;
using backend.Test;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{

    var envPath = Path.Combine(
       builder.Environment.ContentRootPath,
       "..",
       ".env"
    );
    Env.Load(envPath);

    string db_name = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
    string id = Environment.GetEnvironmentVariable("DB_ID") ?? "";
    string db_password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
    string db_server = "localhost";



    var connectionString = $"Server={db_server},1433;Database={db_name};User Id={id};Password={db_password};TrustServerCertificate=true;Encrypt=False";
    builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(connectionString));
}

if (builder.Environment.IsProduction())
{

    Env.Load();
    builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}


builder.Services.AddOpenApi();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

    if (!db.Datas.Any())
    {
        db.Datas.Add(new ApiTestData
        {
            Id = Guid.NewGuid(),
            TestData = "Hello from API"
        });

        db.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("api/data", async (ApplicationDBContext db) =>
{
    var data = await db.Datas.ToListAsync();
    return data.ToList();
}).WithName("TestData");

app.Run();