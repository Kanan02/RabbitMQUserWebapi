using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentification.RabbitClient
{
    public interface IRabbitMQClient
    {
        public void Publish(string exchange, string routingKey, byte[] body);
        public void CloseConnection();
    }
}
