using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AxisLPRSystem
{
    internal class Axis_SDKManager
    {
        private readonly HttpListener _listener;
        private readonly Axis_Camera _camera;
        public event Action<LprEventData> LicensePlateDetected;


        public Axis_SDKManager(string cameraIP, int port, string saveImagePath)
        {
            _camera = new Axis_Camera(cameraIP, saveImagePath);
            _camera.LprEventReceived += OnLprEventReceived;

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/api/LicensePlate/");
        }

        public async void StartListening()
        {
            _listener.Start();
            Console.WriteLine("Listening for events...");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                Task.Run(() => ProcessRequest(context));
            }
        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(context.Request.InputStream))
                    {
                        string data = await reader.ReadToEndAsync();
                        Console.WriteLine($"Received License Plate Data: {data}");

                        // פענוח ה-JSON
                        var jsonData = JObject.Parse(data);
                        string plateNumber = jsonData["plate"]?.ToString();
                        string lane = jsonData["lane"]?.ToString();
                        string time = jsonData["time"]?.ToString();

                        // יצירת אובייקט של LprEventData
                        var eventData = new LprEventData
                        {
                            PlateNumber = plateNumber,
                            Lane = int.Parse(lane),
                            EventDateTime = DateTime.Parse(time)
                        };

                        // הפעלת האירוע
                        LicensePlateDetected?.Invoke(eventData);
                    }
                }

                var response = context.Response;
                string responseString = "Event received successfully.";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private void OnLprEventReceived(LprEventData eventData)
        {
            Console.WriteLine($"Plate: {eventData.PlateNumber}, Saved at: {eventData.ImagePath}");
        }

        public void StopListening()
        {
            _listener.Stop();
            Console.WriteLine("Stopped listening.");
        }
    }
}