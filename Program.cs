
using lulu_diary_backend.Authorization;
using lulu_diary_backend.Context;
using lulu_diary_backend.Middleware;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace lulu_diary_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add .env configurations
            DotNetEnv.Env.Load();
            builder.Configuration.AddEnvironmentVariables();

            // Add AppDbVontext w/ postgresql connection string
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add repositories
            builder.Services.AddScoped<ProductsRepository>();
            builder.Services.AddScoped<DiariesRepository>();
            builder.Services.AddScoped<ProfilesRepository>();
            builder.Services.AddScoped<CommentsRepository>();
            builder.Services.AddScoped<DiaryReactionsRepository>();
            builder.Services.AddScoped<CommentReactionsRepository>();
            builder.Services.AddScoped<FollowersRepository>();
            builder.Services.AddScoped<FriendsRepository>();
            builder.Services.AddScoped<FriendRequestsRepository>();

            // Add UserContext service
            builder.Services.AddScoped<UserContext>();

            // Add Authorization Handlers
            builder.Services.AddScoped<IAuthorizationHandler, IsOwnerAuthorizationHandler>();

            // Add web controllers
            builder.Services.AddControllers();

            // Add OpenAPI
            builder.Services.AddOpenApi();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") 
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });

            // Add Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = builder.Configuration["JWT_SECRET"]
                        ?? throw new ArgumentNullException("Symmetric Security key is missing");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            // Add Authorization Policies
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("IsOwner", policy => policy.Requirements.Add(new IsOwnerRequirement()));

            // Build, setup and run app
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseMiddleware<UserContextMiddleware>();
            app.UseAuthorization();
            app.MapControllers();


            app.Run();
        }
    }
}
