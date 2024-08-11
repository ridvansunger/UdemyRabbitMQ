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


channel.QueueDeclare("hello-queue", true, false, false);


Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"hello world{x}";
    //byte veri tipinde gödnerilir.
    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);


    Console.WriteLine($"Mesaj gönderilmiştir . {x}");

});


Console.ReadLine();
