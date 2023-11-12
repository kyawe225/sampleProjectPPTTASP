using System.Text;
using BookingAppllicaiton.Context;
using BookingAppllicaiton.Repository;
using BookingAppllicaiton.Tables;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Log/aspnet2.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10, shared: true)
    .WriteTo.Console().CreateLogger();



// Add services to the container.
builder.Services.AddStackExchangeRedisCache(p =>
{
    p.Configuration = builder.Configuration.GetConnectionString("RedisLocal");
    p.InstanceName = "sample_";
});

builder.Services.AddScoped<PackagesRepository>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo() { Title = "My API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please Enter Token without bearer",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "Bearer",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[]{}
        }
    });
});



builder.Services
    .AddDbContext<DatabaseContext>(p=> p.UseMySql(builder.Configuration.GetConnectionString("Local"),new MariaDbServerVersion(new Version(11,1,2))));

builder.Services.AddHangfire(c =>
    {
        c.UseRedisStorage(builder.Configuration.GetConnectionString("RedisConnection"));
    }
);
builder.Services.AddHangfireServer();



builder.Services.AddAuthentication(p =>
{
    p.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    p.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(p =>
{
    p.SaveToken = true;
    p.Challenge = JwtBearerDefaults.AuthenticationScheme;
    p.RefreshInterval = TimeSpan.FromDays(1);
    p.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangFireDash");

// BackgroundJob.Enqueue(() =>
// {
//     using (DatabaseContext context = new DatabaseContext())
//     {
//         IEnumerable<ClassTable> tmpclass=context.Classes.Where(p => p.EndDateTime > DateTime.Now).ToList();
//         foreach (ClassTable classTable in tmpclass)
//         {
//             var schedules=context.Schedules.Where(p => p.RegisteredClassId == classTable.Id && (p.Type == 3 || p.Type == 1)).ToList();
//             foreach (var i in schedules)
//             {
//                 i.Type = 4;
//                 var j=context.PackageUsers.Where(p => p.UserId == i.UserId).FirstOrDefault();
//                 if (j != null)
//                 {
//                     j.Credit += classTable.Credit;
//                     context.Update(j);
//                 }
//
//                 context.Update(i);
//             }
//             context.SaveChanges();
//         }
//     }
// }, Cron.Hourly);
app.Run();