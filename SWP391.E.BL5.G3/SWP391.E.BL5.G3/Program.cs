using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.Controllers;
using SWP391.E.BL5.G3.Extensions;
using SWP391.E.BL5.G3.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "devquyduy";
    options.IdleTimeout = new TimeSpan(0, 30, 0);

});
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<RoleFilter>();
});
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddDbContext<traveltestContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
               {
                   option.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidIssuer = builder.Configuration["TokenBearer:Issuer"],
                       ValidateIssuer = true,
                       ValidAudience = builder.Configuration["TokenBearer:Audience"],
                       ValidateAudience = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenBearer:SignatureKey"])),
                       ValidateLifetime = true
                   };

                   option.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           var token = context.Request.Cookies["accessToken"];


                           if (!string.IsNullOrEmpty(token))
                           {

                               context.Token = token;

                               context.Token = token;

                           }

                           return Task.CompletedTask;
                       }
                   };
               });

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
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
