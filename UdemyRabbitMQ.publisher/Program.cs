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


channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);


Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
{
    var routeKey = $"route-{x}";
    var queueName = $"direkt-queu{x}";
    channel.QueueDeclare(queueName, true, false, false);

    channel.QueueBind(queueName, "logs-direct", routeKey, null);
});


Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    LogNames log = (LogNames)new Random().Next(1, 5);


    string message = $"log-type: {log}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    var routeKey = $"route-{log}";

    channel.BasicPublish("logs-direct", routeKey, null, messageBody);


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
