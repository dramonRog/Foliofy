using Microsoft.EntityFrameworkCore;
using Foliofy.DataBase;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Connection to database
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Database>(options => options.UseSqlServer(connection));

// Add authorization
builder.Services.AddAuthentication(options => { options.DefaultScheme = "MyCookieAuth"; }).AddCookie("MyCookieAuth", options => {
    options.Cookie.Name = "MyAuthCookie";
    options.LoginPath = "/AccountActions/login";
    options.LogoutPath = "/AccountActions/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
