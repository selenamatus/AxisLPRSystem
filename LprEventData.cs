using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace AxisLPRSystem
{
    public class LprEventData
    {
        public string PlateNumber { get; set; }
        public DateTime EventDateTime { get; set; }
        public int Lane { get; set; }
        public string ImagePath { get; set; }
        public Image LPRImage { get; set; }

        public string EventDateTimeToString()
        {
            return EventDateTime.ToString("yyyyMMddHHmmss");
        }

        public override string ToString()
        {
            return $"Plate: {PlateNumber}, Time: {EventDateTime}, Lane: {Lane}, ImagePath: {ImagePath}";
        }
    }
}
