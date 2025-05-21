using ASP_MongoDB.Areas.Admin.EmailManager;
using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoConnection"));

builder.Services.AddSingleton<MongoDBContext>();

// Configure MongoDB Identity
builder.Services.AddIdentity<ApplicationUser, MongoIdentityRole<Guid>>()    
    .AddMongoDbStores<ApplicationUser, MongoIdentityRole<Guid>, Guid>(
        builder.Configuration.GetValue<string>("MongoConnection:ConnectionString"),
        builder.Configuration.GetValue<string>("MongoConnection:DatabaseName"))
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.ClaimsIdentity.RoleClaimType = "Role";
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/AccountAdmin/LoginAdmin";
    options.AccessDeniedPath = "/Admin/AccountAdmin/AccessDenied"; // optional
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

// cấu hình session 

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

// Sử dụng Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();



// Routing
app.MapControllerRoute(
    name: "category",
    pattern: "/category/{id?}",
    defaults: new { controller = "Category", action = "Index" });

app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{id?}",
    defaults: new { controller = "Brand", action = "Index" });

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "user",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
