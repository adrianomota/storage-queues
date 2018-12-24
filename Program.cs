using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace storage_queues
{
    class Program
    {
        const string name = "envia-email";

        static async Task Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=photoswebappapi;AccountKey=IzZr19NI92nQbw2J+IdVNOG38Jw6FfnCPWQ6FH707RrGd9J0YTkqJtWo1A9e6PnkR5DpUxMyNVKS2JVhZY4nsw==;EndpointSuffix=core.windows.net");

            var queueClient = storageAccount.CreateCloudQueueClient();

            // for(int i = 0; i <= 30; i++)
            // {
            //   await EnviarMensagem(queueClient);
            // }

            await ContaMensagemNaFila(queueClient);

            await ProcessarMensagem(queueClient);

            await ContaMensagemNaFila(queueClient);
        }



        private static async Task EnviarMensagem(CloudQueueClient queueClient)
        {
            var queue = queueClient.GetQueueReference(name);
            await queue.CreateIfNotExistsAsync();

            var message = new CloudQueueMessage($"clientid- { new Random().Next(100)}");


            //deve-se serializar o objeto para json ou xml (formato string) e enviar
            await queue.AddMessageAsync(message);

            Console.WriteLine($"Mensagem enviada para a fila: {message.Id}");
        }

        static async Task  ContaMensagemNaFila(CloudQueueClient queueClient)
        {
            var queue = queueClient.GetQueueReference(name);
            await queue.FetchAttributesAsync();
            Console.WriteLine($"Mensagem(s) na fila: {queue.ApproximateMessageCount}");
        }

        private static async Task ProcessarMensagem(CloudQueueClient queueClient)
        {
            var queue = queueClient.GetQueueReference(name);

            var messages = await queue.GetMessagesAsync(10);

            foreach(var msg in messages)
            {
                //envio de e-mail
                var content = msg.AsString;//

                //deserializar o json ou xml
                Console.WriteLine($"Enviando email do cliente: {content}");

                await Task.Delay(TimeSpan.FromSeconds(1));

                Console.WriteLine("Mensagem enviada!");

                //apaga mensagem da queue
                await queue.DeleteMessageAsync(msg);
            }

        }
    }

}
