using System.Threading;
namespace my
{
    public class Programm {

        static void Main(string[] args) {
            var ms = new MessageSender("localhost");
            var message = $"tiks:{DateTime.Now.Ticks} \n message send {DateTime.Now.Year} {DateTime.Now.Minute}  {DateTime.Now.Second}   !";
            ms.SendMessage("my2", message);

            Thread.Sleep(1000);
            message = $"tiks:{DateTime.Now.Ticks} \n message send {DateTime.Now.Year} {DateTime.Now.Minute}  {DateTime.Now.Second}   !";
            ms.SendMessage("my2", message);
          
        }
    } 
}


