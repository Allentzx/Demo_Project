using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Mapper;
using ITC.Core.Service;
using ITC.Core.Utilities;
using ITC.Core.Utilities.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using ITC.Core.Configurations;
using Microsoft.Extensions.FileProviders;
using ITC.Core.Utilities.Quartz;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using ITC.Core.Utilities.Quartz.Job;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GoogleAuthConfiguration>(builder.Configuration.GetSection("GoogleAuthentication"));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

builder.Services.AddMvc();

builder.Services.AddMemoryCache();
builder.Services.AddDirectoryBrowser();
// builder.Services.AddHostedService<QuartzHostedService>();
IWebHostEnvironment environment = builder.Environment;

//Database
builder.Services.AddDbContext<ITCDBContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     b => b.MigrationsAssembly("ITC-BE")));

//Mapper
builder.Services.AddSingleton(new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
}).CreateMapper());

//Service
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddTransient<IUserContextService, UserContextService>();
builder.Services.AddTransient<IStaffService, StaffService>();
builder.Services.AddTransient<IDeputyService, DeputyService>();
builder.Services.AddTransient<ICourseService, CourseService>();
builder.Services.AddTransient<IStudentService, StudentService>();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient<ISyllabusService, SyllabusService>();
builder.Services.AddTransient<ISlotService, SlotService>();
builder.Services.AddTransient<IPartnerService, PartnerService>();
builder.Services.AddTransient<ICampusService, CampusService>();
builder.Services.AddTransient<IAuditService, AuditService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddTransient<IReasonService, ReasonService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IMajorService, MajorService>();
builder.Services.AddTransient<IPhaseService, PhaseService>();
builder.Services.AddTransient<ICancelService, CancelService>();
builder.Services.AddTransient<IFirebaseFCMService, FirebaseFCMService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IConfigService, ConfigService>();
builder.Services.AddTransient<IProgramService, ProgramService>();
builder.Services.AddTransient<IDocumentService, DocumentService>();
builder.Services.AddTransient<IRegistrationService, RegistrationService>();
builder.Services.AddTransient<IFeedBackService, FeedBackService>();


// add Quartz serivce
// builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
// builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
// builder.Services.AddSingleton<Action00AMJob>();
// builder.Services.AddSingleton(new JobSchedule(
//     jobType: typeof(Action00AMJob),
//     cronExpression: "3 0 0 ? * *"));






//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IC", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

//Authentication JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["GoogleAuthentication:ClientId"];
        options.ClientSecret = builder.Configuration["GoogleAuthentication:ClientSecret"];
    })
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles(); 

app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
                        Path.Combine(environment.ContentRootPath, "Excel")),
    RequestPath = new PathString("/Excel")
});
app.UseHttpsRedirection();

app.UseFileServer(enableDirectoryBrowsing: true);


app.UseAuthorization();

app.MapControllers();

app.Run();

