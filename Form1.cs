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


namespace AxisLPRSystem
{
    public partial class Form1 : Form
    {
        private Axis_SDKManager _sdkManager;

        public Form1()
        {
            InitializeComponent();

           
            _sdkManager = new Axis_SDKManager(8090);

            
            _sdkManager.LicensePlateDetected += OnLicensePlateDetected;

            
            _sdkManager.StartListening();
        }


        private void OnLicensePlateDetected(string plateData)
        {
            try
            {
                
                var json = JObject.Parse(plateData);
                string plate = json["plate"]?.ToString() ?? "N/A";
                string lane = json["lane"]?.ToString() ?? "N/A";
                string time = DateTime.Now.ToString();

                
                Invoke(new Action(() =>
                {
                    listView1.Items.Add(new ListViewItem(new[]
                    {
                time,
                lane,
                plate
            }));
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _sdkManager.StopListening();
        }
    }
}

