using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// Add using clause for GPIO
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409


namespace BlinkCSharpStyle
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Create brushes for painting contols
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.Gray);

        // Add a Dispatch Timer
        private DispatcherTimer blinkTimer;

        // Add a variable to control button
        private Boolean blinking = false;

        // Add variables for the GPIO
        private const int LED_PIN = 4;
        private GpioPin pin;
        private GpioPinValue pinValue;

        public MainPage()
        {
            this.InitializeComponent();
            // Add code to initialize the controls
            this.led_indicator.Fill = grayBrush;
            // Add code to setup timer
            this.blinkTimer = new DispatcherTimer();
            this.blinkTimer.Interval = TimeSpan.FromMilliseconds(1000);
            this.blinkTimer.Tick += BlinkTimer_Tick;
            this.blinkTimer.Stop();
            // Initalize GPIO
            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio_ctrl = GpioController.GetDefault();

            // Check GPIO state
            if (gpio_ctrl == null)
            {
                this.pin = null;
                this.status.Text = "ERROR: No GPIO controller found!";
                return;
            }
            // Setup the GPIO pin
            this.pin = gpio_ctrl.OpenPin(LED_PIN);
            // Check to see that pin is Ok
            if (pin == null)
            {
                this.status.Text = "ERROR: Can't get pin!";
                return;
            }
            this.pin.SetDriveMode(GpioPinDriveMode.Output);
            this.pinValue = GpioPinValue.Low; // turn off
            this.pin.Write(this.pinValue);
            this.status.Text = "Good to go!";
        }

        private void BlinkTimer_Tick(object sender, object e)
        {
            // If pin is on, turn it off
            if (this.pinValue == GpioPinValue.High)
            {
                this.led_indicator.Fill = grayBrush;
                this.pinValue = GpioPinValue.Low;
            }
            // else turn it on
            else
            {
                this.led_indicator.Fill = greenBrush;
                this.pinValue = GpioPinValue.High;
            }
            this.pin.Write(this.pinValue);
        }

        private void start_stop_button_Click(object sender, RoutedEventArgs e)
        {
            this.blinking = !this.blinking;
            if (this.blinking)
            {
                this.start_stop_button.Content = "Stop";
                this.led_indicator.Fill = greenBrush;
                this.blinkTimer.Start();
            }
            else
            {
                this.start_stop_button.Content = "Start";
                this.led_indicator.Fill = grayBrush;
                this.blinkTimer.Stop();
                this.pinValue = GpioPinValue.Low;
                this.pin.Write(this.pinValue);
            }
        }

    }
}
