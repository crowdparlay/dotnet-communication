using CrowdParlay.Communication.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureRabbitMqCommunication(this IServiceCollection services, string amqpServerUrl) => services
        .AddSingleton<IConnectionFactory>(new ConnectionFactory { Uri = new Uri(amqpServerUrl) })
        .AddScoped<IMessageBroker, RabbitMqMessageBroker>();
}