using Application;
using Identity.Models;
using Identity.Seeds;
using Microsoft.AspNetCore.Identity;
using Persistance;
using Shared;
using WebAPI.Extensions;
using Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationLayer();
builder.Services.AddApiVersioningExtension();

var configuration = builder.Configuration;

builder.Services.AddPersistanceInfraestructure(configuration);
builder.Services.AddSharedInfraestructure(configuration);
builder.Services.AddIdentityInfraestructure(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseErrorHandlingMiddleware();

app.MapControllers();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
	try
	{
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

		await DefaultRoles.SeedAsync(userManager, roleManager);
		await DefaultAdminUser.SeedAsync(userManager, roleManager);
		await DefaultBasicUser.SeedAsync(userManager, roleManager);
	}
	catch (Exception ex)
	{

		throw new Exception(ex.Message);
	}
}

app.Run();
