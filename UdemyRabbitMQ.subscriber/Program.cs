// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://hlzbdrdx:IZ7hjd746v3qYvfPDia_l12RouFKPUHP@toad.rmq.cloudamqp.com/hlzbdrdx");//appsetting de tutulur.

using var connection = factory.CreateConnection();

//rabbit mq ye kanal üzerinden bağlanıyorum.
var channel = connection.CreateModel();



channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);

var queueName = channel.QueueDeclare().QueueName;
//var routeKey = "*.Error.*";
//var routeKey = "*.*.Warning";
var routeKey = "Info.#";//başı ifo olsun devamı önemli değil.

channel.QueueBind(queueName, "logs-topic", routeKey);

channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar dinleniyor...");

consumer.Received += (object sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    Thread.Sleep(1500);
    Console.WriteLine("Gelen mesaj " + message);

    //File.AppendAllText("log-critical.txt", message + "\n");

    channel.BasicAck(e.DeliveryTag, false);//ilgili mesajı bildirir.
};







Console.ReadLine();

