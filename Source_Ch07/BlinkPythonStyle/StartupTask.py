import _wingpio as gpio # The GPIO library
import time             # Time functions

BLINK_TIME = 30.0       # blink for 30 seconds
GPIO_PIN = 4            # GPIO4 (not pin #4)

pin_val = gpio.HIGH     # Store pin value (high/low)
elapsed_time = 0        # Store time in seconds

# Setup the GPIO
gpio.setup(GPIO_PIN, gpio.OUT, gpio.PUD_OFF, gpio.HIGH)

# Run the blink series for 
while (elapsed_time < BLINK_TIME):
    # If LED is on, turn it off
    if pin_val == gpio.HIGH:
        pin_val = gpio.LOW
        gpio.output(GPIO_PIN, pin_val)
    # else, turn it on
    else:
        pin_val = gpio.HIGH
        gpio.output(GPIO_PIN, pin_val)

    # Sleep for 1/2 second
    time.sleep(1)
    elapsed_time = elapsed_time + 1

# Close down and cleanup the GPIO 
gpio.cleanup()