// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Collections;
using System.Text;




var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://hlzbdrdx:IZ7hjd746v3qYvfPDia_l12RouFKPUHP@toad.rmq.cloudamqp.com/hlzbdrdx");//appsetting de tutulur.

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();


channel.ExchangeDeclare("header-exhange", durable: true, type: ExchangeType.Headers);

Random rnd = new Random();

Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");


var properties = channel.CreateBasicProperties();
properties.Headers = headers;
channel.BasicPublish("header-exhange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));

Console.WriteLine("Mesaj Gönderilmiştir.");

Console.ReadLine();



public enum LogNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4

}
