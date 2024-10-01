using ead_backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Builder;
using ead_backend.Services.ServiceImpl;
using ead_backend.Services;
using ead_backend.Healpers;
using System.Security.Claims;

namespace ead_backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // MongoDB configuration
            services.AddSingleton(sp =>
                new MongoDbContext(
                    Configuration.GetConnectionString("MongoDbConnection"),
                    Configuration.GetValue<string>("MongoDbDatabase")
                )
            );

            // JWT Authentication Configuration
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("VendorOrAdmin", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && (c.Value == "vendor" || c.Value == "admin")))));
            });

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<JwtHelper>();

            // Add services for dependency injection
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVendorRatingService, VendorRatingService>();


            // Add CORS policy to allow frontend to access API
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins("http://localhost:5173")  // Replace with your frontend origin
                           .AllowAnyHeader()                       // Allow any headers (e.g., for tokens)
                           .AllowAnyMethod();                      // Allow any HTTP method (GET, POST, PUT, DELETE)
                });
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Apply CORS policy before Authentication & Authorization
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
