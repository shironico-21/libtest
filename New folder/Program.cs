using finalProject;
using finalProject.Models;
using finalProject.Repositry.classes;
using finalProject.Repositry.interfaces;
using finalProject.Service.classes;
using finalProject.Service.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Add Identity with EF Core Stores
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Instructs Identity to use EF Core stores
.AddDefaultTokenProviders();
builder.Services.AddScoped<Ihotelrepo, HotelRepo>();         // Adjust 'HotelRepo' to your exact class name
builder.Services.AddScoped<IRoomRepo, RoomRepo>();           // Adjust 'RoomRepo' to your exact class name
builder.Services.AddScoped<IResrvationRepo, ReservationRepo>(); // Adjust 'ReservationRepo' to your exact class name
builder.Services.AddScoped<IRoomTypeRepo, RoomTypeRepo>();   // Adjust 'RoomTypeRepo' to your exact class name
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();   // Adjust 'CustomerRepo' to your exact class name
builder.Services.AddScoped<IHotelManage, HotelService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Set the login path
    options.AccessDeniedPath = "/Account/AccessDenied"; // Set the access denied path
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});
builder.Services.AddScoped<IUserContract, UserRepo>();
builder.Services.AddScoped<IAutthContract, AuthServiece>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontend"); // Apply the CORS policy
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
