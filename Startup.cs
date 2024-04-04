using AutoMapper;
using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

public class Startup
{
    private IConfiguration _configuration { get;}
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if(_env.IsProduction()){     
            Console.WriteLine("--> Using SQL Server Database...");
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(
                _configuration.GetConnectionString("CommandsDB")
            ));
        }
        else{
            Console.WriteLine("--> Using Im-Memory Database...");
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }   
        services.AddScoped<ICommandRepo, CommandRepo>();
        services.AddControllers();
        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddHostedService<MessageBusSubscriber>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommandsService", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommandsService"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });

        PrepDb.PrepPopulation(app, env.IsProduction());
    }
}