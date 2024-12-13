
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Security.Claims;
using VetClinicCapstone.BackgroundServices;
using VetClinicCapstone.EmailNotication;
using VetClinicCapstone.EmailSettings;
using VetClinicCapstone.Models;
using VetClinicCapstone.ReportServices;
using VetClinicCapstone.Repository;
var builder = WebApplication.CreateBuilder(args);




//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//        .AddCookie(options =>
//        {
//            options.Cookie.Name = "MyCookies";
//            options.Cookie.HttpOnly = true;
//            options.ExpireTimeSpan = TimeSpan.FromHours(1); // Cookie expiration time
//            options.SlidingExpiration = true; // Extend expiration on activity
//            options.LoginPath = "/AdminLogin/Index";
//            options.AccessDeniedPath = "/Error/AccessDenied";
//        });
//builder.Services.AddAuthorization(options =>
//{
//	options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
//	options.AddPolicy("Doctor", policy => policy.RequireClaim(ClaimTypes.Role, "Doctor"));
//});


builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = "UserScheme";
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie("UserScheme", options =>
{
	options.Cookie.Name = "UserCookie";
	options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(360);
    options.SlidingExpiration = true;
	options.LoginPath = "/UserLogin/Index";
	options.AccessDeniedPath = "/Error/AccessDenied";
})
.AddCookie("AdminScheme", options =>
{
	options.Cookie.Name = "AdminCookie";
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromDays(360);
	options.SlidingExpiration = true;
	options.LoginPath = "/AdminLogin/Index";
	options.AccessDeniedPath = "/Error/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
	options.AddPolicy("DoctorOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Doctor"));
	options.AddPolicy("StaffOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Staff"));
	options.AddPolicy("DoctorOrStaffOnly", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && (c.Value == "Doctor" || c.Value == "Staff")));
    });
});


// Connection String
builder.Services.AddDbContext<ApplicationDbContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));



builder.Services.AddTransient<IRepository<UserTbl>,Repository<UserTbl>>();
builder.Services.AddTransient<IRepository<AdminTbl>, Repository<AdminTbl>>();
builder.Services.AddTransient<IRepository<AppointmentTbl>, Repository<AppointmentTbl>>();
builder.Services.AddTransient<IRepository<PatientTbl>, Repository<PatientTbl>>();
builder.Services.AddTransient<IRepository<OwnerTbl>, Repository<OwnerTbl>>();
builder.Services.AddTransient<IRepository<ConsultationTbl>, Repository<ConsultationTbl>>();
builder.Services.AddTransient<IRepository<PrescriptionTbl>, Repository<PrescriptionTbl>>();
builder.Services.AddTransient<IRepository<PrescriptionDetailTbl>, Repository<PrescriptionDetailTbl>>();
builder.Services.AddTransient<IRepository<LaboratoryTbl>, Repository<LaboratoryTbl>>();
builder.Services.AddTransient<IRepository<AdminInfoTbl>, Repository<AdminInfoTbl>>();
builder.Services.AddTransient<IRepository<ServiceTbl>, Repository<ServiceTbl>>();
builder.Services.AddTransient<ReportService>();



// Add services to the container.

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<AppointmentNotificationService>();
builder.Services.AddHostedService<AppointmentNotificationBackgroundService>();


builder.Services.AddSession();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AdminLogin}/{action=Index}/{id?}");

app.Run();
