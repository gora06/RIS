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
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


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

        public void Process()
        {

        NetworkStream stream = null;

            try
            {
                stream = client.GetStream();

                byte[] data = new byte[1024];
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string[] pos = builder.ToString().Split();


                    Process proc = System.Diagnostics.Process.GetCurrentProcess();
                    IntPtr id = proc.MainWindowHandle;
                    Rect rec = new Rect();
                    GetWindowRect(id, ref rec);

                    MoveWindow(id, Convert.ToInt32(pos[1]), Convert.ToInt32(pos[0]), Convert.ToInt32(pos[2]) - Convert.ToInt32(pos[1]), Convert.ToInt32(pos[3]) - Convert.ToInt32(pos[0]), true);
                    Console.WriteLine(pos[0] + " " + pos[1] + " " + pos[2] + " " + pos[3]);
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
    }

    

    class Program
    {
        static TcpListener listener;

        

        static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(Dns.GetHostEntry("localhost").AddressList[0], 8000);
                listener.Start();
                Console.WriteLine("Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("New connection enstabliched");
                    ClientObject clientObject = new ClientObject(client);

                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
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
        }
    }
}
