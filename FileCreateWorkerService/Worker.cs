using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Data;
using System.Text;
using System.Text.Json;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private RabbitMQClientService _rabbitmqClientService;
        private readonly IServiceProvider _serviceProvider;

        private IModel _channel;

        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;

            _rabbitmqClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClientService.Connect();
            _channel.BasicQos(0, 1, false);//(boyut öenmli deðil,Bir bir gönder,

            return base.StartAsync(cancellationToken);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;


            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            var createExcelMesage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var ms = new MemoryStream();

            var wb = new XLWorkbook();
            var ds = new DataSet();
            ds.Tables.Add(GetDataTable("Orders"));

            wb.Worksheets.Add(ds);
            wb.SaveAs(ms);


            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");//Buradaki name upload daki parametre adý ile ayný olmak zorunda


            var baseUrl = "https://localhost:44366/api/files";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{baseUrl}?fileId={createExcelMesage.FileId}", multipartFormDataContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"File (Id : {createExcelMesage.FileId}) was created by succesful");
                    _channel.BasicAck(@event.DeliveryTag, false);

                }
            }


        }

    
        private DataTable GetDataTable(string tableNAme)
        {
            List<Order> orders = new List<Order>();

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NorthwindContext>();
                orders = context.Orders.ToList();
            }

            DataTable table = new DataTable() { TableName = tableNAme };

            table.Columns.Add("OrderId", typeof(int));
            table.Columns.Add("CustomerId", typeof(string));
            table.Columns.Add("EmployeeId", typeof(int));
            table.Columns.Add("ShipName", typeof(string));
            table.Columns.Add("ShipRegion", typeof(string));
            table.Columns.Add("ShipCountry", typeof(string));
            orders.ForEach(o =>
            {
                table.Rows.Add(o.OrderId, o.CustomerId, o.EmployeeId, o.ShipName, o.ShipRegion, o.ShipCountry);
            });
            return table;
        }

    }
}
