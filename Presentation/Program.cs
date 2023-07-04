using Infrastructure.Data;
using Infrastructure.Data.Interface;
using Infrastructure.Data.Repository;
using Presentation.Controller;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();
app.ConfigureUserProfileRouter();
app.ConfigureTableRouter();
app.ConfigureAuthenticationRouter();

app.Run();