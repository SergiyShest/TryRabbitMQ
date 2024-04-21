using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace my
{
    public class Programm
    {

        static void Main(string[] args)
        {
            var ms = new MessageReceiver("localhost");
            ms.AddListener("my2");
            Console.ReadLine();
        }
    }
}