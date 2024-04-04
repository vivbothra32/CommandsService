using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope()){
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd){

            if(isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try{
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Could not run migrations: {ex.Message}");
                }
            }
                if(!context.Platforms.Any())
                {
                    Console.WriteLine("Seeding data...");
                    context.Platforms.AddRange(
                        new Platform{ Name="Dot Net", ExternalId = 11},
                        new Platform{ Name="Docker", ExternalId=12 },
                        new Platform{ Name="SQL Server", ExternalId=13},
                        new Platform{ Name="Azure", ExternalId=14}
                    );
                    context.Commands.AddRange(
                        new Command { HowTo="Add Package", CommandLine="dotnet add package <package-name>", PlatformId = 1 },
                        new Command { HowTo="Build", CommandLine="dotnet build", PlatformId = 1 },
                        new Command { HowTo="Run", CommandLine="dotnet run", PlatformId = 1 },
                        new Command { HowTo="Push docker image", CommandLine="docker push <dockerhub-id>/<imagename>:<imageversion>", PlatformId = 2 }
                    );
                    context.SaveChanges();
                }
                else{
                    Console.WriteLine("Data already exists! ");
                }            
        }
    }
}