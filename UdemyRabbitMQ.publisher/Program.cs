// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using Shared;
using System.Collections;
using System.Text;
using System.Text.Json;




var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://hlzbdrdx:IZ7hjd746v3qYvfPDia_l12RouFKPUHP@toad.rmq.cloudamqp.com/hlzbdrdx");//appsetting de tutulur.

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();


channel.ExchangeDeclare("header-exhange", durable: true, type: ExchangeType.Headers);


Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");


var properties = channel.CreateBasicProperties();
properties.Headers = headers;
properties.Persistent = true;//Mesajları kalıcı hale getirir.

var product = new Product
{
    Id = 1,
    Name = "Kalem",
    Price = 100,
    Stock = 10
};

var productJsonString = JsonSerializer.Serialize(product);

channel.BasicPublish("header-exhange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

Console.WriteLine("Mesaj Gönderilmiştir.");

Console.ReadLine();



public enum LogNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4

}
