using AspNetCore.Unobtrusive.Ajax;
using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Middleware;
using Web.Model.Common;
using Web.Repository;
using Web.Repository.Implement;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddSingleton<DapperContext>(); 

// Register use DI (Dependency Injection)     
builder.Services.AddScoped<IAdminMenuRepository, AdminMenuRepository>();
builder.Services.AddScoped<IUserAdminRepository, UserAdminRepository>();
builder.Services.AddScoped<IGroupUserRepository, GroupUserRepository>();
builder.Services.AddScoped<ICacheRepository, CacheRepository>(); 

// change code html 
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession
(
  options =>
  {
      // Set a short timeout for easy testing.
      options.IdleTimeout = TimeSpan.FromMinutes(30);
      options.Cookie.HttpOnly = true;
  }
); 

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
               options =>
               {
                   options.LoginPath = "/Admin/Account/Login";
                   options.LogoutPath = "/Admin/Account/logout";
                   options.AccessDeniedPath = "/Admin/Home/AccessDenied";
               });

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddMvc();
builder.Services.AddUnobtrusiveAjax();

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
//app.UseAppAuthenticate();
app.UseAuthentication();
app.UseAuthorization(); 

app.UseEndpoints
(
  endpoints =>
  {
      endpoints.MapControllerRoute(
        name: "areaRoute",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
      );

      endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
      );
  }
);

app.Run();
