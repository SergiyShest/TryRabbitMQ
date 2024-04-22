using System.Threading;
namespace my
{
    public class Programm {

        static void Main(string[] args)
        {

            for (int i = 0; i < 30; i++)
            {
                var ms = new MessageSender("localhost");
                var message = $"{i}.1) tiks:{DateTime.Now.Ticks} \n message send  nobody {DateTime.Now.Year} {DateTime.Now.Minute}  {DateTime.Now.Second}   !";
                ms.SendMessage("", message,"kk");

                Thread.Sleep(500);
                message = $"{i}.2) tiks:{DateTime.Now.Ticks} \n message Сергей Шестаков {DateTime.Now.Year} {DateTime.Now.Minute}  {DateTime.Now.Second}   !";
                ms.SendMessage("", message, "Сергей Шестаков");

            }
            Console.ReadLine();
        }
    } 
}


