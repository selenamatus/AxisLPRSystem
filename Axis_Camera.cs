using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace AxisLPRSystem
{
    public class Axis_Camera
    {
        private readonly string _cameraIP;
        private readonly string _saveImagePath;
        private readonly HttpClient _httpClient;

        public event Action<LprEventData> LprEventReceived;

        public Axis_Camera(string cameraIP, string saveImagePath)
        {
            _cameraIP = cameraIP;
            _saveImagePath = saveImagePath;

            
            if (!Directory.Exists(_saveImagePath))
                Directory.CreateDirectory(_saveImagePath);

            _httpClient = new HttpClient();
        }

        public async Task<string> DownloadImageAsync()
        {
            try
            {
                string imageUrl = $"http://{_cameraIP}/jpg/image.jpg";
                string fileName = $"LPR_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string saveFilePath = Path.Combine(_saveImagePath, fileName);

                using (var response = await _httpClient.GetStreamAsync(imageUrl))
                using (var fileStream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                {
                    await response.CopyToAsync(fileStream);
                }

                return saveFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download image: {ex.Message}");
                return null;
            }
        }

        public async Task ProcessLicensePlateAsync(string plateData, int lane)
        {
            string savedImagePath = await DownloadImageAsync();

            var lprEvent = new LprEventData
            {
                PlateNumber = plateData,
                EventDateTime = DateTime.Now,
                Lane = lane,
                ImagePath = savedImagePath
            };

            LprEventReceived?.Invoke(lprEvent);
        }
    }
}