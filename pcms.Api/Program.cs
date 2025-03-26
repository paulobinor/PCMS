using FluentValidation;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using pcms.Api;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Application.Services;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;
using pcms.Infra;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().WriteTo.Console()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWorkRepo, UnitOfWorkRepo>();
builder.Services.AddScoped<IMemberServiceRepo, MemberServiceRepo>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IMemberContributionService, MemberContributionService>();
builder.Services.AddScoped<IPCMSBackgroundService, PCMSBackgroundService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IContributionRepo, ContributionRepo>();
builder.Services.AddScoped<IContributionService, ContributionService>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<BackGroundJobProcess>();
builder.Services.AddScoped<ModelValidationService>();
builder.Services.AddTransient<IValidator<AddContributionDto>, AddContributionDtoValidator>();
builder.Services.AddTransient<IValidator<UpdateContributionDto>, UpdateContributionDtoValidator>();
builder.Services.AddTransient<IValidator<AddMemberDto>, AddMemberDtoValidator>();
builder.Services.AddTransient<IValidator<UserLogin>, UserLoginDtoValidator>();
builder.Services.AddTransient<IValidator<RegisterUser>, RegisterUserDtoValidator>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHangfireServer();

// Register Services
builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
builder.Services.AddSingleton<IFailedTransactionHandler, FailedTransactionHandler>();
builder.Services.AddSingleton<IServerFilter, HangfireFailedJobListener>();


#region JWT Authentication Services
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "PCMS Api", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };

});

builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = false);
#endregion


var app = builder.Build();
GlobalJobFilters.Filters.Add(app.Services.GetRequiredService<IServerFilter>());

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    try
//    {
//        var jobService = services.GetRequiredService<BackGroundJobProcess>();
//       jobService.ProcessStartupTask(); // Call startup logic
//    }
//        catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while executing startup process.");
//    }
//}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetSection("HangFireOptions:User").Value,
            Pass = app.Configuration.GetSection("HangFireOptions:Pass").Value
        }
    }
});
app.MapHangfireDashboard();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
