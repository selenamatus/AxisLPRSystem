using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Configuration;


namespace AxisLPRSystem
{
    public partial class Form1 : Form
    {
        private Axis_SDKManager _sdkManager;

        public Form1()
        {
            InitializeComponent();

            
            string cameraIP = ConfigurationManager.AppSettings["Camera1_IP"];
            string saveImagePath = ConfigurationManager.AppSettings["ImageSavePath"];
            int port = 8090;

            _sdkManager = new Axis_SDKManager(cameraIP, port, saveImagePath);

           
            _sdkManager.LicensePlateDetected += OnLicensePlateDetected;

            _sdkManager.StartListening();
        }

        private void OnLprEventReceived(LprEventData eventData)
        {
            Invoke(new Action(() =>
            {
                var listViewItem = new ListViewItem(eventData.EventDateTime.ToString());
                listViewItem.SubItems.Add(eventData.Lane.ToString());
                listViewItem.SubItems.Add(eventData.PlateNumber);
                listView1.Items.Add(listViewItem);
            }));
        }

        private void OnLicensePlateDetected(LprEventData eventData)
        {
            Invoke(new Action(() =>
            {
                listView1.Items.Add(new ListViewItem(new[]
                {
            eventData.EventDateTime.ToString("yyyy-MM-dd HH:mm:ss"), 
            eventData.Lane.ToString(),                              
            eventData.PlateNumber                                  
        }));
            }));
        }



    }
}

