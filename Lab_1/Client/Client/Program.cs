using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Client
{
    class Program
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        static void Main(string[] args)
        {
            TcpClient client = null;

            try
            {
                client = new TcpClient("localhost", 8000);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    Rect rect = new Rect();
                    Process process = Process.GetCurrentProcess();
                    IntPtr ptr = process.MainWindowHandle;
                    GetWindowRect(ptr, ref rect);

                    string message = rect.Top.ToString() + " " + rect.Left.ToString() + " " + rect.Right + " " + rect.Bottom + " " + Console.BufferHeight;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
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
        }
    }
}
