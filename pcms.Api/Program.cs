using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using pcms.Application.Interfaces;
using pcms.Application.Services;
using pcms.Domain.Interfaces;
using pcms.Infra;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWorkRepo, UnitOfWorkRepo>();
builder.Services.AddScoped<IMemberServiceRepo, MemberServiceRepo>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHangfireServer();

// Register Services
builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
builder.Services.AddSingleton<IFailedTransactionHandler, FailedTransactionHandler>();
builder.Services.AddSingleton<IServerFilter, HangfireFailedJobListener>();

var app = builder.Build();
GlobalJobFilters.Filters.Add(app.Services.GetRequiredService<IServerFilter>());

app.UseHangfireDashboard();
app.MapHangfireDashboard();

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
