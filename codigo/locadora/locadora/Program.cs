using locadora.Models;
using Microsoft.EntityFrameworkCore;

namespace locadora
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=locadoraDB;Trusted_Connection=True;TrustServerCertificate=True;"));

            services.AddAuthorization(); 

        }
    }
}
