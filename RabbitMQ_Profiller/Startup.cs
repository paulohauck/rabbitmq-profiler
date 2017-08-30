using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RabbitMQ_Profiller
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Infrastructure
            //services.AddTransient(s => new ConnectionFactory { HostName = Configuration.GetConnectionString("RabbitMqHostName") });
            //services.AddTransient<IBus, RabbitMqBus>();
            //services.AddDbContext<MyDatabaseContext>(b => b.UseSqlServer(Configuration.GetConnectionString("IntegrationServiceSqlServer")), ServiceLifetime.Transient);


            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime, IBus bus)
        {
            //applicationLifetime.ApplicationStopping.Register(() => OnShutdown(bus));

            //SubscribeEvents(bus);

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseMvc();
        }

        //private void SubscribeEvents(IBus bus)
        //{
           
        //}

        //private void OnShutdown(IBus bus)
        //{
        //    bus.Close();
        //}
    }
}
