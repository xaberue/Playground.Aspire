using RabbitMQ.Client;
using System.Text;

namespace Xaberue.Playground.HospitalManager.Appointments.Shared;

public static class InfrastructureHelper
{
    public class Constants
    {
        internal const string ExchangeSuffix = "exchange";
        internal const string QueueSuffix = "queue";
        internal const string DeadLetterSuffix = "DeadLetter";

        public const string AppointmentRegistered = "appointment_registered";
        public const string AppointmentAdmitted = "appointment_admitted";
        public const string AppointmentCompleted = "appointment_completed";

        public const string AppointmentUpdated = "appointment_updated";
    }

    public static string GetExchangeName(string baseName)
    {
        ValidateName(baseName);

        return $"{baseName}_{Constants.ExchangeSuffix}";
    }

    public static string GetQueueName(string baseName, string? customSuffix = null)
    {
        ValidateName(baseName);

        var builder = new StringBuilder(baseName);
        if (!string.IsNullOrEmpty(customSuffix))
            builder.Append($"_{customSuffix}");
        builder.Append($"_{Constants.QueueSuffix}");

        return builder.ToString();
    }

    public static string GetDeadLetterQueueName(string baseName, string? customSuffix = null)
    {
        ValidateName(baseName);

        var builder = new StringBuilder(baseName);
        if (!string.IsNullOrEmpty(customSuffix))
            builder.Append($"_{customSuffix}");
        builder.Append($"_{Constants.QueueSuffix}");
        builder.Append($".{Constants.DeadLetterSuffix}");

        return builder.ToString();
    }

    public static string DeclareQueue(this IModel channel, string baseName)
    {
        var queueName = GetQueueName(baseName);
        var deadLetterQueueName = GetDeadLetterQueueName(baseName);
        channel.QueueDeclare(queue: deadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var mainQueueArguments = new Dictionary<string, object>
         {
             { "x-dead-letter-exchange", "" },
             { "x-dead-letter-routing-key", deadLetterQueueName }
         };

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);

        return queueName;
    }

    public static string DeclareExchange(this IModel channel, string baseName)
    {
        var exchangeName = GetExchangeName(baseName);

        channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

        return exchangeName;
    }

    public static string DeclareSubscriptionQueue(this IModel channel, string baseName, string? customSuffix = null)
    {
        var exchangeName = GetExchangeName(baseName);
        var queueName = GetQueueName(baseName, customSuffix);
        var deadLetterQueueName = GetDeadLetterQueueName(baseName, customSuffix);
        channel.QueueDeclare(queue: deadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var mainQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", deadLetterQueueName }
        };
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        return queueName;
    }


    private static void ValidateName(string baseName)
    {
        if (baseName.EndsWith(Constants.ExchangeSuffix) || baseName.EndsWith(Constants.QueueSuffix) || baseName.EndsWith(Constants.DeadLetterSuffix))
            throw new ArgumentException($"RabbitMQ element should not end with {Constants.ExchangeSuffix}, {Constants.QueueSuffix} or {Constants.DeadLetterSuffix} in its base declaration");
    }

}
