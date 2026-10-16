using RA_MuniAnnouncements.Api.Clients;
using RA_MuniAnnouncements.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

// Swapping to real implementations is a one-line change here.
builder.Services.AddScoped<ICmsClient, MockCmsClient>();
builder.Services.AddScoped<ISiteSettingsRepository, MockSiteSettingsRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
