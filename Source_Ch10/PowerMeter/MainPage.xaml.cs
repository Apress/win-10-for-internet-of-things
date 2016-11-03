using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;        // add this for GPIO pins
using System.Diagnostics;          // add this for debugging

namespace PowerMeter
{
    public sealed partial class MainPage : Page
    {
        // Power meter pins, variables
        private const int numLEDs = 5;
        private const float maxPotVal = 1023;
        private int[] METER_PINS = { 21, 20, 19, 18, 17 };
        private GpioPin[] Meter = new GpioPin[numLEDs];

        // Add a Dispatch Timer
        private DispatcherTimer meterTimer;

        // Instantiate the new ADC Chip class
        ADC_MCP3008 adc = new ADC_MCP3008();

        // Channel to read the potentiometer
        private const int POT_CHANNEL = 0;

        public MainPage()
        {
            this.InitializeComponent();  
            InitGPIO();                  // Initialize GPIO
            adc.Initialize();            // Call initialize() method for ADC

            // Setup the timer
            this.meterTimer = new DispatcherTimer();
            this.meterTimer.Interval = TimeSpan.FromMilliseconds(500); 
            this.meterTimer.Tick += meterTimer_Tick;
            this.meterTimer.Start();
        }

        // Setup the GPIO initial states
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                Debug.WriteLine("Sorry, the GPIO cannot be initialized. Drat.");
                return;
            }
            Debug.WriteLine("GPIO ready.");
            // Initialize the GPIO pins
            for (int i = 0; i < numLEDs; i++)
            {
                this.Meter[i] = gpio.OpenPin(METER_PINS[i]);
                this.Meter[i].SetDriveMode(GpioPinDriveMode.Output);
                this.Meter[i].Write(GpioPinValue.Low);
                Debug.Write("Setting up GPIO pin ");
                Debug.Write(METER_PINS[i]);
                Debug.WriteLine(".");
            }
        }

        private void meterTimer_Tick(object sender, object e)
        {
            float valRead = 0;
            int numLEDs_On = 0;

            Debug.WriteLine("Timer has fired the meterTimer_Tick() method.");

            // Read value from the ADC
            valRead = adc.getValue(POT_CHANNEL);
            Debug.Write("Val read = ");
            Debug.WriteLine(valRead);

            float percentCalc = (valRead / maxPotVal) * (float)10.0;
            numLEDs_On = (int)percentCalc / 2;
            Debug.Write("Number of LEDs to turn on = ");
            Debug.WriteLine(numLEDs_On);

            // Adjust power meter LEDs On or Off based on value read
            for (int i = 0; i < numLEDs_On; i++)
            {
                this.Meter[i].Write(GpioPinValue.High);
                Debug.Write("Setting pin ");
                Debug.Write(METER_PINS[i]);
                Debug.WriteLine(" HIGH.");
            }
            for (int i = numLEDs_On; i < numLEDs; i++)
            {
                this.Meter[i].Write(GpioPinValue.Low);
                Debug.Write("Setting pin ");
                Debug.Write(METER_PINS[i]);
                Debug.WriteLine(" LOW.");
            }
        }
    }
}
