using jeyZ.Core.Entities;
using jeyZ.Core.Repositories;
using jeyZ.Core.Services;
using jeyZ.Infraestructure.Auth;
using jeyZ.Infraestructure.MessageBus;
using jeyZ.Infraestructure.Repositories;
using jeyZ.Infraestructure.Subscribers;
using RabbitMQ.Client;
using System.Text;

namespace jeyZ.API.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
            };
            var connection = connectionFactory.CreateConnection();
            services.AddSingleton(new ProducerConnection(connection));
            services.AddSingleton<IMessageBusClient, RabbitMqClient>();
            return services;
        }

        public static IServiceCollection AddSubscribers(this IServiceCollection services)
        {
            services.AddHostedService<OrderCreatedSubscriber>();
            return services;
        }
    }
}
