using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pizzashop.Service.Implementations;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web;
using PizzaShop.Web.Middleware;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PizzashopContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));

DependencyInjection.RegisterServices(builder.Services, builder.Configuration.GetConnectionString("DatabaseConnection")!);

// Dependency Injection
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

// Add Permission Handler
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Data Protection
builder.Services.AddDataProtection().SetApplicationName("PizzaShop");
builder.Services.AddHttpContextAccessor();
// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Login";
        options.AccessDeniedPath = "/Home/";
    })
    // Add JWT Authentication
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["Token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Auth/Login");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.Response.Redirect("/Home/Error");
                return Task.CompletedTask;
            }
        };
    });

// Add Authorization with Permission Policies
builder.Services.AddAuthorization(options =>
{
    var permissions = new[]
    {
        "User.View", "User.Edit", "User.Delete",
        "Role.View", "Role.Edit", "Role.Delete",
        "Menu.View", "Menu.Edit", "Menu.Delete",
        "TableSection.View", "TableSection.Edit", "TableSection.Delete",
        "TaxFees.View", "TaxFees.Edit", "TaxFees.Delete",
        "Orders.View", "Orders.Edit", "Orders.Delete",
        "Customers.View", "Customers.Edit", "Customers.Delete"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 404 Status Page
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

app.UseAuthentication();  // Order is important
app.UseAuthorization();

// Add Middleware for First Login
app.UseMiddleware<IsFirstLoginMiddleware>(builder.Configuration["Jwt:Key"], "Token");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

    app.Run();
