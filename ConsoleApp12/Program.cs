using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp12
{
    internal class Server
    {
        public static string SpaceMethod(string text) 
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty; // Или выбросить исключение ArgumentException, если пустой текст недопустим
            }

            // Разделяем текст на слова, используя пробелы и знаки пунктуации как разделители.
            // Удаляем пустые строки, которые могут возникнуть при наличии нескольких разделителей подряд.
            string[] words = text.Split(new char[] { ' ', '.', ',', ';', ':', '!', '?', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                return string.Empty; // Если в тексте нет слов
            }

            // Находим самое длинное слово, используя LINQ
            string longestWord = words.OrderByDescending(w => w.Length).FirstOrDefault();

            return longestWord;
        }

       
        static void Main(string[] args)
        {
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 7200);

            Socket sListener = new Socket(ipAddr.AddressFamily, 
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Ожидание соеденения через порт {0}", ipEndPoint);

                    Socket handler = sListener.Accept();
                    string data = null;

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    Console.Write("Полученный текст;" + data + "\n\n");

                    string spacereply = data;
                    string normalspacereply = SpaceMethod(spacereply);
               

                    string reply = "\nСамое длинное слово в этом тексте: " + normalspacereply;
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<The End>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соеденение с клиентом");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally 
            {
                Console.ReadLine();
            }
        }
    }
}
