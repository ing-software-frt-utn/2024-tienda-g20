using Microsoft.EntityFrameworkCore;
using IDS_TFI;
using System.Globalization;
using System.Text;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddPolicy(MyAllowSpecificOrigins,
		policy =>
			{
				policy.AllowAnyHeader();
				policy.AllowAnyMethod();
				policy.AllowCredentials();
				policy.SetIsOriginAllowed((origin) =>
				{
					_ = origin;
					return true;
				});
			}
		);
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext") ?? throw new InvalidOperationException("Connection string 'DataContext' not found.")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(1800);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.UseSession();

app.Run();
