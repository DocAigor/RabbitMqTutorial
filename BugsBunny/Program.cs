using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BugsBunny
{
    class Program
    {
        static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(args[0])) return;

            var messageToReceive = string.Empty;


            var factory = new ConnectionFactory();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(args[0], false, false, false, null);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(args[0], true, consumer);
                    Console.WriteLine(string.Format("Attendo messaggio su {0}", args[0]));
                    while (true)
                    {
                        var ba = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        Console.WriteLine("[{0}]-I receive {1}", DateTime.Now.ToShortDateString(), Encoding.UTF8.GetString(ba.Body));
                    }
                }
            }
        }
    }
}
