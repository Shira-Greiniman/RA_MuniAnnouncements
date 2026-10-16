using RA_MuniAnnouncements.Api.Clients;
using RA_MuniAnnouncements.Api.Repositories;
using RA_MuniAnnouncements.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddPolicy(
	"AngularDev",
	policy => policy
		.WithOrigins("http://localhost:4200")
		.AllowAnyHeader()
		.AllowAnyMethod()));

// Swapping to real implementations is a one-line change here.
builder.Services.AddScoped<ICmsClient, MockCmsClient>();
builder.Services.AddScoped<ISiteSettingsRepository, MockSiteSettingsRepository>();
builder.Services.AddSingleton<IAnnouncementNormalizer, AnnouncementNormalizer>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
