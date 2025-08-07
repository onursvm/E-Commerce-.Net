using E_Commerce.Business.Manager;
using E_Commerce.Business.Services.IdentityServices.Interfaces;
using E_Commerce.Business.Services.IdentityServices.Service;
using E_Commerce.Business.Services.PropertyServices.Interfaces;
using E_Commerce.Business.Services.PropertyServices.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Servisleri 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IPropertyStatusService, PropertyStausService>();
builder.Services.AddScoped<IPropertyTypeService, PropertyTypeService>();
builder.Services.AddScoped<IPropertyPhotoService, PropertyPhotoService>();

// ServiceManager'ý kaydedin
builder.Services.AddScoped<IServiceManager, ServiceManager>();
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
