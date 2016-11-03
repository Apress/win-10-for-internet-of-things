using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices;
using System.Diagnostics;                  // add this for debugging
using Microsoft.IoT.Lightning.Providers;   // add for Lightning inteface


namespace NightLight
{
    public sealed partial class MainPage : Page
    {
        // Constants and variables for pin 
        private const int LED_PIN = 27;
        private LED_Fade fader = new LED_Fade(LED_PIN);

        // Timer to refresh the LED brightness
        private DispatcherTimer refreshTimer;

        // Add the new ADC Chip class
        private MCP3008 adc = new MCP3008();

        // Channel to read the photocell with initial min/max settings
        private const int LDR_CHANNEL = 0;
        private int max_LDR = 300;   // Tune this to match lighting 
        private int min_LDR = 100;   // Tune this to match lighting 

        public MainPage()
        {
            InitializeComponent();       // init UI
            InitLightningProvider();     // setup lightning provider
            fader.Initialize();          // setup PWM
            adc.Initialize();            // setup ADC

            // Add code to setup timer
            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromMilliseconds(500);
            refreshTimer.Tick += refreshTimer_Tick;
            refreshTimer.Start();
        }

        private void InitLightningProvider()
        {
            // Set the Lightning Provider as the default if Lightning driver is enabled on the target device
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }
        }

        private void refreshTimer_Tick(object sender, object e)
        {
            float valRead = 0;
            float brightness = 0;

            Debug.WriteLine("Timer has fired the refreshTimer_Tick() method.");

            // Read value from the ADC
            valRead = adc.getValue(LDR_CHANNEL);
            Debug.Write("Val read = ");
            Debug.WriteLine(valRead);
            status.Text = "LDR Value = " + valRead;

            // Get min, max from sliders
            min_LDR = (int)ldrLow.Value;
            max_LDR = (int)ldrHigh.Value;
            Debug.WriteLine("Min LDR = " + min_LDR);
            Debug.WriteLine("Max LDR = " + max_LDR);

            // Calculate the brightness
            brightness = ((valRead - min_LDR) / (max_LDR - min_LDR));
            // Make sure the range stays 0.0 - 1.0
            if (brightness > 1)
            {
                brightness = (float)1.0;  
            }
            else if (brightness < 0)
            {
                brightness = (float)0.0;
            }
            Debug.Write("Brightness percent = ");
            Debug.WriteLine(brightness);

            // Set the brightness
            fader.set_fade(brightness);

            // For extra credit, show the voltage returned from the LDR
            // convert the ADC readings to voltages to make them more friendly.
            float ldrVolts = adc.ADCToVoltage((int)valRead);

            // Let us know what was read in.
            Debug.WriteLine(String.Format("Voltage for value read: {0}, {1}v",
                                          valRead, ldrVolts));
        }

        private void ldrLow_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            // Make sure the low value doesn't exceed the high value
            if (ldrLow.Value > ldrHigh.Value)
            {
                ldrHigh.Value = ldrLow.Value + 1;
            }
        }
    }
}
