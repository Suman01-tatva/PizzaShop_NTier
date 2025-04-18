using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Service.Implementations;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PizzashopContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));

DependencyInjection.RegisterServices(builder.Services, builder.Configuration.GetConnectionString("DatabaseConnection")!);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepostory>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenDataService, TokenDataService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
builder.Services.AddScoped<IMenuItemsRepository, MenuItemRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IMenuModifierGroupRepository, MenuModifierGroupRepository>();
builder.Services.AddScoped<IMenuModifierRepository, MenuModifierRepository>();
builder.Services.AddScoped<IMenuModifierService, MenuModifierService>();

builder.Services.AddDataProtection().SetApplicationName("PizzaShop");
builder.Services.AddControllersWithViews();
// builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

// Add authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Redirect to login if not authenticated
        options.LogoutPath = "/Auth/Login";
        options.AccessDeniedPath = "/Home/"; // Redirect if unauthorized
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePages(async context =>
   {
       if (context.HttpContext.Response.StatusCode == 404)
       {
           context.HttpContext.Response.Redirect("/Home/NotFound");
       }
   });
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add the authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();