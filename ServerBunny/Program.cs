using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using System.Linq;
using System.Reflection;

namespace ServerBunny
{
    class Program
    {
        private const int DEFAULT_TIME = 30000;
        private const string messagesResource = "messages.dat";
        private const string queueResource = "queue.dat";
        private static List<string> queue;
        private static List<string> messages;

        static void Main(string[] args)
        {
            var time = 0L;
            if (!Int64.TryParse(args[0], out time)) time = DEFAULT_TIME;
            queue = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/" + queueResource).ToList<string>();
            messages = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/" + messagesResource).ToList<string>();

            Timer timer = new Timer(time);
            timer.Elapsed += OnTimedEvent;
            timer.Start();
            Console.WriteLine(@"Press any key to kill the rabbit \|. ");
            Console.ReadKey();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var factory = new ConnectionFactory();
            var randomQueue = new Random();
            var randomMessage = new Random();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var q = queue.ElementAt(randomQueue.Next(0, queue.Count + 1));
                    var m = messages.ElementAt(randomMessage.Next(0, messages.Count + 1));
                    channel.QueueDeclare(q, false, false, false, null);
                    Console.WriteLine(string.Format("\t[{2}] - I say: {0} to {1}", m, q, DateTime.Now.ToShortDateString()));
                    channel.BasicPublish("", q, null, Encoding.UTF8.GetBytes(m));
                }
            }
        }
    }
}
