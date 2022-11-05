using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using User_Service.Data;
using User_Service.Interfaces;
using User_Service.Interfaces.IRepositories;
using User_Service.Interfaces.IServices;
using User_Service.Middlewares;
using User_Service.Services;

var builder = WebApplication.CreateBuilder(args);

//---------Comment out if you want to run without authentication-------------
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//---------------------------------------------------------------------------

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<IOrganisationService, OrganisationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPatientGroupService, PatientGroupService>();
builder.Services.AddTransient<INatsService, NatsService>();

builder.Services.AddCors();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

//---------Comment out if you want to run without authentication-------------
//builder.Services.AddSwaggerGen(setup =>
//{
//    var jwtSecurityScheme = new OpenApiSecurityScheme
//    {
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        Name = "Authentication",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,

//        Reference = new OpenApiReference
//        {
//            Id = JwtBearerDefaults.AuthenticationScheme,
//            Type = ReferenceType.SecurityScheme
//        }
//    };

//    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

//    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        { jwtSecurityScheme, Array.Empty<string>() }
//    });
//});
//-------------------------------------------------------------------------------

builder.Services.AddDbContext<DatabaseContext>(options =>
    options
        .UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("UserServiceContext") ?? string.Empty));

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DatabaseContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

//----Comment out if you want to run without authentication----
//app.UseAuthentication();
//-------------------------------------------------------------

app.UseAuthorization();

//----Comment out if you want to run without authentication----
//app.UseOrganizationAuthorization();
//-------------------------------------------------------------

app.MapControllers();

app.Run();