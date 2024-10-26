using IAndOthers.Core.Configs;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IAndOthers.Core.Data.Services;
using Hangfire;
using IAndOthers.Core.Jobs;
using IAndOthers.Core.Helpers;
using MassTransit;
using IAndOthers.Core.MassTransit;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext for MSSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true; // Enable email confirmation
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//// Configure Jwt
//var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
//builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
//    };
//});

// Configure Smtp
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("SmtpConfig"));

// Add application services
IOIoCRegistrar.RegisterDependencies(builder.Services);
// Add repository services
builder.Services.AddScoped(typeof(IIORepository<,>), typeof(IORepositoryBase<,>));
// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();
// Redis Configuration
//builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection("RedisConfig"));
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var redisConfig = sp.GetRequiredService<IOptions<RedisConfig>>().Value;

//    var options = new ConfigurationOptions
//    {
//        EndPoints = { redisConfig.Host },
//        Password = redisConfig.Password,
//        User = redisConfig.User,
//        Ssl = redisConfig.Ssl,
//        SslHost = redisConfig.SslHost,
//        AbortOnConnectFail = redisConfig.AbortOnConnectFail
//    };

//    return ConnectionMultiplexer.Connect(options);
//});

// Configure RabbitMQ with MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMqConnection"));
    });
});
// Add MassTransit hosted service
builder.Services.AddMassTransitHostedService();

// Configure Hangfire with MSSQL
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
);
builder.Services.AddHangfireServer();

var app = builder.Build();

var defaultCulture = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

// Configure IBusControl after app starts
MassTransitHelper.ConfigureBusControl(app.Services);

// Set the service provider
IODependencyResolver.SetServiceProvider(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHangfireDashboard("/hangfire");
app.UseHangfireServer();
// Use Hangfire recurring tasks
RegisterRecurringJobs(app.Services);

app.Run();

// Method to find and register all IORecurringJob classes with Hangfire
static void RegisterRecurringJobs(IServiceProvider services)
{
    var recurringJobTypes = IOAssemblyHelper.FindClassesTypeOf<IORecurringJob>();

    foreach (var jobType in recurringJobTypes)
    {
        var jobInstance = (IORecurringJob)Activator.CreateInstance(jobType);
        RecurringJob.AddOrUpdate(jobInstance.Code, () => jobInstance.Execute(), jobInstance.Cron);

        Console.WriteLine($"[Hangfire] Registered recurring job: {jobInstance.Code} with Cron: {jobInstance.Cron}");
    }
}