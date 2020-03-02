using System.Linq;
using MediatR;
using MediatrTest.Commands;
using MediatrTest.Controllers;
using MediatrTest.Extensions;
using MediatrTest.Pipeline;
using MediatrTest.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoreLinq;

namespace MediatrTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(IOptional<>), typeof(Optional<>));
            services.AddControllers();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            
            typeof(Startup).Assembly.FindDerivedTypes(typeof(IBaseCommand)).ForEach(commandType =>
            {
                var args = commandType.GetInterface(typeof(ICommand<,>).Name).GenericTypeArguments.ToArray();
                services.AddScoped(typeof(IRequestHandler<,>).MakeGenericType(commandType,
                        typeof(ICommandResponse<>).MakeGenericType(args[1])), 
                    typeof(CommandTransactionHandler<,,>).MakeGenericType(commandType, args[0], args[1]));   
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}