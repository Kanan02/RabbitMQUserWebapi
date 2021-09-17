using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentification.RabbitClient
{
    public class RabbitMQClient:IRabbitMQClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQClient(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("main-queue",
              durable: true,
              exclusive: false,
              autoDelete: false,
              arguments: null

              );
            _channel.ConfirmSelect();
          
        }

        public void Publish(string exchange, string routingKey, byte[] body)
        {
            _channel.BasicPublish(exchange, routingKey, null, body);
            _channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
        }

        public void CloseConnection()
        {
            _connection?.Close();
        }

    }
}
