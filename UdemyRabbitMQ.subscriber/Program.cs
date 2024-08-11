// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://hlzbdrdx:IZ7hjd746v3qYvfPDia_l12RouFKPUHP@toad.rmq.cloudamqp.com/hlzbdrdx");//appsetting de tutulur.

using var connection = factory.CreateConnection();

//rabbit mq ye kanal üzerinden bağlanıyorum.
var channel = connection.CreateModel();

//bu kodu yazarsan  bu kuyruk oluşmamaışsa kod hata vermez. Kuyuruk oluşumundan eminsen kullanmana gerek yok.
//channel.QueueDeclare("hello-queue", true, false, false);//parametreler aynı olmalı.

//BasicConsume ()
//consumer tarafı
//autoAck => true ise mesaj iletildikten sonra kuyruktan direk olarak siler. False ise kuyruktan silme ben sana haber verince sil.
//IBasicConsumer


//BasicQos(hernagi bir boyuttaki mesaj(mesaj boyutu),mesaj kaç kaz gitsin, global olursa (false seçersek toplam mesaj sayısını subsriberler eşit olarak dağıtmaya çalışır.false seçiğim için 6 ar şekilde subcriberlara gönderecek.) )
channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume("hello-queue", false, consumer);

consumer.Received += (object sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    Console.WriteLine("Gelen mesaj " + message);

    channel.BasicAck(e.DeliveryTag, false);//ilgili mesajı bildirir.
};







Console.ReadLine();

