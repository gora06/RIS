using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Server
{
    public class ClientObject
    {

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        public TcpClient client;

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        /*public void Process()
        {

            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();

                byte[] data = new byte[1024];
                while (true)
                {
                    string message;
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        message = Encoding.UTF8.GetString(data, 0, bytes);
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }*/

        static int state = 0;

        static public void ChangeState()
        {
            if (state == 1) state = 0;
            else state = 1;
        }
    }



    class Program
    {
        static TcpListener listener;
        static int state = 1;

        static public void ChangeState()
        {
            if (state == 1) state = 0;
            else state = 1;
        }

        static public void Maintance(object _client)
        {
            TcpClient client = (TcpClient)_client;
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[1024];
                while (true)
                {
                    string message;
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        message = Encoding.UTF8.GetString(data, 0, bytes);
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine(message);

                    if (message == "ChangeState")
                    {
                        ChangeState();
                        stream.Close();
                        client.Close();
                        listener.Stop();
                    }
                    break;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }


        static void Main(string[] args)
        {
            while (true)
            {
                switch (state)
                {
                    case 0:
                        {
                            try
                            {
                                Console.WriteLine("I am a server now");
                                Console.WriteLine("Waiting for connection...");

                                listener = new TcpListener(Dns.GetHostEntry("localhost").AddressList[0], 8000);
                                listener.Start();

                                while (state == 0)
                                {
                                    TcpClient client = listener.AcceptTcpClient();

                                    // NetworkStream stream = null;

                                    Thread thread = new Thread(new ParameterizedThreadStart(Maintance));
                                    thread.Start(client);


                                    /*try
                                    {
                                        stream = client.GetStream();

                                        byte[] data = new byte[1024];
                                        while (true)
                                        {
                                            string message;
                                            int bytes = 0;
                                            do
                                            {
                                                bytes = stream.Read(data, 0, data.Length);
                                                message = Encoding.UTF8.GetString(data, 0, bytes);
                                            }
                                            while (stream.DataAvailable);

                                            Console.WriteLine(message);

                                            if (message == "ChangeState")
                                            {
                                                ChangeState();
                                                stream.Close();
                                                client.Close();
                                                listener.Stop();
                                            }
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                    finally
                                    {
                                        if (stream != null)
                                            stream.Close();
                                        if (client != null)
                                            client.Close();
                                    }*/
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (listener != null)
                                    listener.Stop();
                            }
                            break;
                        }
                    case 1:
                        {
                            Console.WriteLine("I am a client now");
                            TcpClient client = null;
                            try
                            {
                                client = new TcpClient("localhost", 8000);
                                NetworkStream stream = client.GetStream();
                                string message;

                                while (state == 1)
                                {
                                    message = Console.ReadLine();
                                    byte[] data = Encoding.UTF8.GetBytes(message);
                                    stream.Write(data, 0, data.Length);

                                    if (message == "ChangeState")
                                    {
                                        ChangeState();
                                        stream.Close();
                                        client.Close();
                                    }
                                    break;

                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                client.Close();
                            }
                            break;
                        }
                }
            }
        }
    }
}
