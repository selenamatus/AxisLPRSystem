using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AxisLPRSystem
{
    internal class Axis_SDKManager
    {
        private readonly HttpListener _listener;
        private readonly int _port;

        
        public event Action<string> LicensePlateDetected;

        public Axis_SDKManager(int port)
        {
            _port = port;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{_port}/api/LicensePlate/"); 
        }

        
        public async void StartListening()
        {
            _listener.Start();
            Console.WriteLine($"Listening for events on port {_port}...");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                Task.Run(() => ProcessRequest(context));
            }
        }

        
        public void StopListening()
        {
            _listener.Stop();
            Console.WriteLine("Stopped listening for events.");
        }

        
        private async void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "POST")
                {
                    using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        string data = await reader.ReadToEndAsync();
                        Console.WriteLine($"Received License Plate Data: {data}");

                        
                        LicensePlateDetected?.Invoke(data);
                    }
                }

               
                var response = context.Response;
                string responseString = "Event received successfully";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
