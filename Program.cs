using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NZWalks.data;
using NZWalks.Mapping;
using NZWalks.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Serilog;
using NZWalks.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

//Added SeriLog
var logger = new LoggerConfiguration()
    .WriteTo.Console() // this is to have or show log in console output
    .WriteTo.File("Logs\\NZWalks_log.txt",rollingInterval:RollingInterval.Day) //path where log will store and rollling interval to just remove the log basd on time so that it won't overhead
    .MinimumLevel.Information()
    .CreateLogger();

// Pass logger variable to serilog in builder  log after clearing provider

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// For upload image and we need to inject same where we are calling inside repository
builder.Services.AddHttpContextAccessor(); 

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
// Authorization using JWT Token
builder.Services.AddSwaggerGen(Options =>
{
    //Version name should  match excatly same v1 otherwise it might give error
    Options.SwaggerDoc("v1", new OpenApiInfo { Title = "NZ Walks API", Version = "v1" });

    //Add Authorization Header
    Options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    //Add Security Requirment
    Options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                },

                Scheme="Oauth2",
                Name= JwtBearerDefaults.AuthenticationScheme,
                In= ParameterLocation.Header

            },

            new List<string>()
        }
    });
});

//for Db Context Injection for NZWalks
builder.Services.AddDbContext<NZWalksDbContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));

// //for Db Context Injection for NZWalksAuth
builder.Services.AddDbContext<NZWalkAuthDbContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString"))
    );

//inject Sql Region repository as well
builder.Services.AddScoped<IRegionRepository, SqlRegionRepository>();
//inject Sql Walks repository as well
builder.Services.AddScoped<IWalkRepository, SqlWalkRepository>();
//inject automapper service for this we need to install automapper and automapper dependenct injection package
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Inject IToken Repository
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Inject IImage Repository

builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

//Identity solution

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
    .AddEntityFrameworkStores<NZWalkAuthDbContext>()
    .AddDefaultTokenProviders();

//For password
builder.Services.Configure<IdentityOptions>(options =>
{ 
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

//For authentication we need to AddAuthentication 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(Options =>
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // validate server that generate the token
        ValidateAudience = true, // validate the receipent of token is authroise to recieve
        ValidateLifetime = true, // check is token is not expired and signing key os issuer is valid
        ValidateIssuerSigningKey = true, // validate the signature of token
        ValidIssuer= builder.Configuration["Jwt:Issuer"], // get the Jwt issue from appsetting.json and validate the issuer
        ValidAudience = builder.Configuration["Jwt:Audience"], // get jwt audience from appsetting.json and validate validate audience
        IssuerSigningKey= new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // get the jwt key from appsetting.json for issue signing key
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

//middle ware for gloal exception handler
app.UseMiddleware<ExceptionHandleMiddleware>();

// to use Authentication we need to use below

app.UseAuthentication();

// for Authorization
app.UseAuthorization();

// We can use middleware to server static file which we need to use it here as below
app.UseStaticFiles(new StaticFileOptions 
{ 
    FileProvider= new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Images")),
    RequestPath="/Images"
});

app.MapControllers();
Migration();

app.Run();

void Migration()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<NZWalkAuthDbContext>();
        if (db.Database.GetPendingMigrations().Count()>=1)
        {
            db.Database.Migrate();   
        }

        var db2 = scope.ServiceProvider.GetRequiredService<NZWalksDbContext>();
        if (db2.Database.GetPendingMigrations().Count() >= 1)
        {
            db2.Database.Migrate();
        }
    }
    
}
