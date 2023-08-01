using Infrastructure.Data;
using Infrastructure.Data.Interface;
using Infrastructure.Data.Repository;
using Presentation.Infrastructure;
using Presentation.Routers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddSingleton<ITableRepository, TableRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.ConfigureUserProfileRouter();
app.ConfigureTableRouter();
app.ConfigureAuthenticationRouter();

app.Run();