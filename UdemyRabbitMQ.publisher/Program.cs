// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Collections;
using System.Text;




var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://hlzbdrdx:IZ7hjd746v3qYvfPDia_l12RouFKPUHP@toad.rmq.cloudamqp.com/hlzbdrdx");//appsetting de tutulur.

using var connection = factory.CreateConnection();

//rabbit mq ye kanal üzerinden bağlanıyorum.
var channel = connection.CreateModel();

//ilk başta kuyruk oluşturulmalı yoksa mesajlar boşa gider. Kuyruk ismi verdik.(hello-queue),
//durable propertisini false yaparsak memory e kaydedir. Bilgisayar kapanınca uçar. true yaparsak uçmaz.
//exclusive => kuyruğa sadece bu kanal üzerinden bağlantı sağlar true dersek. Ama sunccriber üzerinden bağlanılsık istiyoruz. Bu nedenle false yaptık. Farklı kanallar üzerinden bağlanılması için false.
//autoDelete => subsriber down olursa kuyrulta silinir ondan dolayı false yaptık otomatik olarak silinmesin.


channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

Random rnd = new Random();

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    LogNames log1 = (LogNames)rnd.Next(1, 5);
    LogNames log2 = (LogNames)rnd.Next(1, 5);
    LogNames log3 = (LogNames)rnd.Next(1, 5);

    var routeKey = $"{log1}.{log2}.{log3}";
    string message = $"log-type: {log1}-{log2}-{log3}";
    var messageBody = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("logs-topic", routeKey, null, messageBody);


    Console.WriteLine($"Log gönderilmiştir . {message}");

});


Console.ReadLine();



public enum LogNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4

}
