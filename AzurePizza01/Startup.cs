using System.Text;
using System.Threading.Tasks;
using AzurePizza01.Data;
using AzurePizza01.Models;
using AzurePizza01.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AzurePizza01
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
            // 1) EF Core + Azure SQL / LocalDB:
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                ));

            // 2) Identity: användare + roller
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // 3) JWT‐autentisering
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // 4) CORS (endast för utveckling)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            // 5) Controllers + Swagger med JWT-stöd
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                // Definiera “Bearer”–token i Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Sätt in din JWT-token här. Format: \"Bearer {din token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // 6) JWT-token‐service
            services.AddScoped<IJwtTokenService, JwtTokenService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 1) Seeda roller när appen startar:
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                SeedRolesAsync(roleManager).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowAll");

            // 2) Aktivera autentisering & auktorisering
            app.UseAuthentication();
            app.UseAuthorization();

            // 3) Swagger (dokumentation)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzurePizza01 v1"));

            app.UseEndpoints(endpoints =>
            {
                // Omdirigera "/" → "/swagger/index.html"
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger/index.html");
                    return Task.CompletedTask;
                });

                // Mappar alla API-controllers
                endpoints.MapControllers();
            });
        }

        private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Skapa roller: Admin, PremiumUser, RegularUser
            string[] roles = new[] { "Admin", "PremiumUser", "RegularUser" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
