using Foliofy.DataBase;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Supabase;

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

// Connection to supabase
builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase"));

SupabaseSettings? supabaseSettings = builder.Configuration.GetSection("Supabase").Get<SupabaseSettings>();
var supabaseClient = new Client(supabaseSettings.Url, supabaseSettings.ServiceRoleKey);
await supabaseClient.InitializeAsync();

builder.Services.AddSingleton(supabaseClient);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
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
