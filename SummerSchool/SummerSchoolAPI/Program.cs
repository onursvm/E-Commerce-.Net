using Microsoft.EntityFrameworkCore;
using SummerSchoolAPI.Business.Interfaces;
using SummerSchoolAPI.Business.Services;
using SummerSchoolAPI.DataAccses;
using SummerSchoolAPI.DataAccses.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISummerSchoolRepository,SummerSchoolRepository>();
builder.Services.AddScoped<ISummerSchoolService, SummerSchoolService>();

builder.Services.AddDbContext<SummerSchoolDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<SummerSchoolDbContext>();
    context.Database.EnsureCreated();
}

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
