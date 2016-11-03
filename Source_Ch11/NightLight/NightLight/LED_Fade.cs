using System;
using System.Diagnostics;    // add for Debug.Write()
using Windows.Devices.Pwm;   // add for PWM control (10586 and newer)
using Microsoft.IoT.Lightning.Providers;  // add for Lightning driver for Pwm

namespace NightLight
{
    class LED_Fade
    {
        // Variables for controlling the PWM class
        private int LED_pin;
        private PwmPin Pwm;

        public LED_Fade(int pin_num = 27)
        {
            LED_pin = pin_num;   // GPIO pin 
            Debug.WriteLine("New class instance of LED_Fade created.");
        }

        // Initialize the PwmController class instance
        public async void Initialize()
        {
            try
            {
                var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
                var pwmController = pwmControllers[1];    // the device controller
                pwmController.SetDesiredFrequency(50);

                Pwm = pwmController.OpenPin(LED_pin);
                Pwm.SetActiveDutyCyclePercentage(0);  // start at 0%
                Pwm.Start();
                if (Pwm == null)
                {
                    Debug.WriteLine("ERROR! Pwm device {0} may be in use.");
                    return;
                }
                Debug.WriteLine("GPIO pin setup for Pwm.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXEPTION CAUGHT: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
        }

        // Set percentage of brightness (or how many cycles are pulsed) where
        // 100 is fast or "bright" and 0 is slow or "dim"
        public void set_fade(float percent)
        {
            if (Pwm != null)
            {
                Pwm.SetActiveDutyCyclePercentage(percent);
                Debug.WriteLine("Pwm set.");
            }
            else
            {
                Debug.WriteLine("Cannot trigger Pwm.");
            }
        }
    }
}
