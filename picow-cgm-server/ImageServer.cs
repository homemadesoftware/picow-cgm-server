using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Buffers.Binary;
using picow_cgm_core;
using System.IO;

namespace picow_cgm_server
{
    internal class ImageServer
    {
        static int version = 1;
        static byte[] previousImage = new byte[0];

        public static async Task StartServerAsync(int port)
        {
            using var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(10);

            while (true)
            {
                Console.WriteLine("Waiting for client connections...");
                var client = await server.AcceptAsync();
                Console.WriteLine("Client connected!");

                _ = Task.Run(() => HandleClient(client));
            }
        }

        private static void HandleClient(Socket client)
        {
            try
            {
                CGMImageGenerator imageGenerator = new CGMImageGenerator();
                imageGenerator.AcquireDataPoints();

                byte[] ePaperImage = imageGenerator.GenerateImageForEPaper();
                if (!ePaperImage.SequenceEqual(previousImage))
                {
                    File.WriteAllBytes("prev.bin", previousImage);
                    File.WriteAllBytes("paper.bin", ePaperImage);
                    previousImage = ePaperImage;
                    ++version;
                }

                List<byte> returnedResponse = new List<byte>();

                Span<byte> bytesSpan = new Span<byte>(new byte[4]);
                BinaryPrimitives.WriteInt32LittleEndian(bytesSpan, version);
                returnedResponse.AddRange(bytesSpan.ToArray());

                returnedResponse.AddRange(ePaperImage);

                client.Send(returnedResponse.ToArray());

                Console.WriteLine($"{DateTime.Now.ToLongTimeString()}. Sent back bitmap v{version}.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Dispose();
            }
        }

    }
}
