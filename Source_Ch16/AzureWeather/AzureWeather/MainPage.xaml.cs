using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;        // Add for Task.Run()
using System.Diagnostics;            // Add for debugging
using Glovebox.IoT.Devices.Sensors;  // Add for BMP280 (or BME280)

namespace AzureWeather
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer bmpTimer;    // Timer
        private BMP280 tempAndPressure;      // Instance of BMP280 class

        public MainPage()
        {
            this.InitializeComponent();

            // Instantiate a new BMP280 class instance
            tempAndPressure = new BMP280();

            this.bmpTimer = new DispatcherTimer();
            this.bmpTimer.Interval = TimeSpan.FromMilliseconds(5000);
            this.bmpTimer.Tick += BmpTimer_Tick;
            this.bmpTimer.Start();
        }

        private void BmpTimer_Tick(object sender, object e)
        {
            var t = Task.Run(() => getData());
        }

        public async void getData()
        {
            // Read data from the sensor
            WeatherData data = new WeatherData();
            data.Temperature = tempAndPressure.Temperature.DegreesCelsius; 
            data.Pressure = tempAndPressure.Pressure.Bars;

            // Send data to the cloud
            await AzureIoTHub.SendDeviceToCloudMessageAsync(data);

            Debug.WriteLine(String.Format("Data sent: {0}, {1}",
                data.Temperature, data.Pressure));
        }
    }
}
