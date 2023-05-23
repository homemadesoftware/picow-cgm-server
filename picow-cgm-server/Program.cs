using System;
using System.Threading.Tasks;

namespace picow_cgm_server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            Task.WaitAll(ImageServer.StartServerAsync(5002));
        }
    }
}