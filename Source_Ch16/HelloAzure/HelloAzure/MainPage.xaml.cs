using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;            // Add for debugging

namespace HelloAzure
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timerTick;
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Start();
        }

        private async void send_button_Click(object sender, RoutedEventArgs e)
        {
            // Read data from the simulated sensor
            SensorData data = new SensorData();
            data.Temperature = Convert.ToDouble(this.InTemperature.Text);
            data.Pressure = Convert.ToDouble(this.InPressure.Text);

            // Send data to the cloud
            await AzureIoTHub.SendDeviceToCloudMessageAsync(data);

            Debug.WriteLine(String.Format("Data sent: {0}, {1}",
                data.Temperature, data.Pressure));
        }

        private async void timerTick(object sender, object e)
        {
            this.OutAzure.Text = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
        }
    }
}
