using Microsoft.AspNetCore.Authentication.Cookies;
using ResumeGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Auth/Login";
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(); // För att spara 'state' under flödet
builder.Services.AddScoped<GithubServices>();
builder.Services.AddScoped<GeminiServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Resume}/{action=Index}/{id?}")
    .WithStaticAssets();





app.Run();
