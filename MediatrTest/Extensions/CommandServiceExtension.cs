using System.Linq;
using System.Reflection;
using MediatR;
using MediatrTest.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;

namespace MediatrTest.Extensions
{
    public static class CommandServiceExtension
    {
        public static IServiceCollection AddCommands(this IServiceCollection services, params Assembly[] assemblies)
        {
            assemblies.ForEach(assembly =>
            {
                assembly.FindDerivedTypes(typeof(IBaseCommand)).ForEach(commandType =>
                {
                    var args = commandType.GetInterface(typeof(ICommand<,>).Name).GenericTypeArguments.ToArray();
                    var aggregateType = args[0];
                    var responseType = args[1];
                    services.AddScoped(typeof(IRequestHandler<,>).MakeGenericType(commandType,
                            typeof(ICommandResponse<>).MakeGenericType(args[1])),
                        typeof(CommandTransactionHandler<,,>).MakeGenericType(commandType, aggregateType,
                            responseType));
                    var iExecutionBehaviorType =
                        typeof(ICommandExecutionBehavior<,>).MakeGenericType(commandType, aggregateType);
                    var executionBehaviorType = assembly.FindDerivedTypes(iExecutionBehaviorType).FirstOrDefault();
                    if (executionBehaviorType != null)
                        services.AddScoped(iExecutionBehaviorType, executionBehaviorType);
                    var iStoreType =
                        typeof(ICommandStoreBehavior<,>).MakeGenericType(aggregateType, responseType);
                    var storeType = assembly.FindDerivedTypes(iExecutionBehaviorType).FirstOrDefault();
                    if (storeType != null)
                        services.AddScoped(iStoreType, storeType);
                    var iContextType =
                        typeof(ICommandStoreBehavior<,>).MakeGenericType(aggregateType, responseType);
                    var contextType = assembly.FindDerivedTypes(iExecutionBehaviorType).FirstOrDefault();
                    if (contextType != null)
                        services.AddScoped(iContextType, contextType);
                    
                });
            });
            
            return services;
        }
    }
}