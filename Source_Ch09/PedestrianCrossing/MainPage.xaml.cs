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
using Windows.Devices.Gpio;    // GPIO header
using Windows.UI.Core;         // DispatcherTimer

namespace PedestrianCrossing
{
    public sealed partial class MainPage : Page
    {
        // Light constants
        private const int RED = 0;
        private const int YELLOW = 1;
        private const int GREEN = 2;

        // Traffic light pins
        private int[] TRAFFIC_PINS = { 4, 5, 6 };

        // Button pin
        private const int BUTTON_PIN = 19;

        // Walk light pins
        private int[] WALK_PINS = { 20, 21 };

        // State constants
        private const int GREEN_TO_YELLOW = 4;
        private const int YELLOW_TO_RED = 8;
        private const int WALK_ON = 12;
        private const int WALK_WARNING = 22;
        private const int WALK_OFF = 30;

        // Traffic light pin variables
        private GpioPin[] Traffic_light = new GpioPin[3];

        // Walk light pin variables
        private GpioPin[] Walk_light = new GpioPin[2];

        // Button pin variable
        private GpioPin Button;

        // Add a DispatcherTimer
        private DispatcherTimer walkTimer;

        // Variable for counting seconds elapsed
        private int secondsElapsed = 0;

        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();
            this.secondsElapsed = 0;

            // Add code to setup timer
            this.walkTimer = new DispatcherTimer();
            // doing 1/2 second ticks for demo purposes
            this.walkTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.walkTimer.Tick += WalkTimer_Tick;
            this.walkTimer.Stop();
        }

        // Setup the GPIO initial states
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Do nothing if there is no GPIO controller
            if (gpio == null)
            {
                return;
            }
            // Initialize the GPIO pins
            for (int i = 0; i < 3; i++)
            {
                this.Traffic_light[i] = gpio.OpenPin(TRAFFIC_PINS[i]);
                this.Traffic_light[i].SetDriveMode(GpioPinDriveMode.Output);
            }
            this.Button = gpio.OpenPin(BUTTON_PIN);
            for (int i = 0; i < 2; i++)
            {
                this.Walk_light[i] = gpio.OpenPin(WALK_PINS[i]);
                this.Walk_light[i].SetDriveMode(GpioPinDriveMode.Output);
            }
            this.Traffic_light[RED].Write(GpioPinValue.Low);
            this.Traffic_light[YELLOW].Write(GpioPinValue.Low);
            this.Traffic_light[GREEN].Write(GpioPinValue.High);
            this.Walk_light[RED].Write(GpioPinValue.High);
            this.Walk_light[YELLOW].Write(GpioPinValue.Low);

            // Check if input pull-up resistors are supported
            if (this.Button.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                this.Button.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                this.Button.SetDriveMode(GpioPinDriveMode.Input);

            // Set a debounce timeout to filter out switch bounce noise from a button press
            this.Button.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            // Register for the ValueChanged event so our Button_ValueChanged 
            // function is called when the button is pressed
            this.Button.ValueChanged += Button_ValueChanged;
        }

        // Detect button press event
        private void Button_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // Pedestrian has pushed the button. Start timer for going red.
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                // Start the timer if and only if not in a cycle
                if (this.secondsElapsed == 0)
                {
                    // need to invoke UI updates on the UI thread because this event
                    // handler gets invoked on a separate thread.
                    var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (e.Edge == GpioPinEdge.FallingEdge)
                        {
                            this.walkTimer.Start();
                        }
                    });
                }
            }
        }

        // Here we do the lights state change if and only if elapsed_seconds > 0
        private void WalkTimer_Tick(object sender, object e)
        {
            // Change green to yellow
            if (this.secondsElapsed == GREEN_TO_YELLOW)
            {
                this.Traffic_light[GREEN].Write(GpioPinValue.Low);
                this.Traffic_light[YELLOW].Write(GpioPinValue.High);
            }
            else if (this.secondsElapsed == YELLOW_TO_RED)
            {
                this.Traffic_light[YELLOW].Write(GpioPinValue.Low);
                this.Traffic_light[RED].Write(GpioPinValue.High);
            }
            else if (this.secondsElapsed == WALK_ON)
            {
                this.Walk_light[RED].Write(GpioPinValue.Low);
                this.Walk_light[YELLOW].Write(GpioPinValue.High);
            }
            else if ((this.secondsElapsed >= WALK_WARNING) && (this.secondsElapsed < WALK_OFF))
            {
                // Blink the walk warning light
                if ((secondsElapsed % 2) == 0)
                {
                    this.Walk_light[YELLOW].Write(GpioPinValue.Low);
                }
                else
                {
                    this.Walk_light[YELLOW].Write(GpioPinValue.High);
                }
            }
            else if (this.secondsElapsed == WALK_OFF)
            {
                this.Walk_light[YELLOW].Write(GpioPinValue.Low);
                this.Walk_light[RED].Write(GpioPinValue.High);
                this.Traffic_light[RED].Write(GpioPinValue.Low);
                this.Traffic_light[GREEN].Write(GpioPinValue.High);
                this.secondsElapsed = 0;
                this.walkTimer.Stop();
                return;
            }
            // increment the counter
            this.secondsElapsed += 1;
        }
    }
}
