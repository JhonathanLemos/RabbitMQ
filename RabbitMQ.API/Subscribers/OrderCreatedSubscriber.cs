using jeyZ.Core.Repositories;
using jeyZ.Infraestructure.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jeyZ.Infraestructure.Subscribers
{
    public class OrderCreatedSubscriber : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string Queue = "order-service/order-created";
        private const string Exchange = "order-service";
        private const string RoutingKey = "order-created";


        public OrderCreatedSubscriber(IServiceProvider serviceProvider, ProducerConnection producerConnection)
        {
            _serviceProvider = serviceProvider;
            _connection = producerConnection.Connection;
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",

            };
            _connection = connectionFactory.CreateConnection("order-service");
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(Exchange, "topic", true);
            _channel.QueueDeclare(Queue, false, false, false);
            _channel.QueueBind(Queue, Exchange, RoutingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();

                var contentString = Encoding.UTF8.GetString(byteArray);
                var message = JsonConvert.DeserializeObject<PaymentAccepted>(contentString);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(Queue, false, consumer);
            return Task.CompletedTask;
        }

        //private async Task<bool> Update()
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var repo = scope.ServiceProvider.GetService<IUserRepository>();
        //        var 
        //    }
        //}

        public class PaymentAccepted
        {
            public Guid Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }
    }
}
