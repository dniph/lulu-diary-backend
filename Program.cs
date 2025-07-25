
using lulu_diary_backend.Context;
using lulu_diary_backend.Repositories;
using Microsoft.EntityFrameworkCore;

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

            // Add web controllers
            builder.Services.AddControllers();

            // Add OpenAPI
            builder.Services.AddOpenApi();

            // Build, setup and run app
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
