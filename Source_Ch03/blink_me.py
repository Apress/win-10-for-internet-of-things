
#
# Windows 10 for the IOT
#
# Raspberry Pi Python GPIO Example
#
# This script blinks an LED placed with the negative lead on pin 6 (GND)
# and the pin 7 connected to a 220 resitor, which is connected to the
# positive lead on the LED.
#
# Created by Dr. Charles Bell
#
import RPi.GPIO as GPIO          # Raspberry Pi GPIO library
import sys                       # System library
import time                      # Used for timing (sleep)

ledPin = 7                       # Set LED positive pin to pin 7 on GPIO
GPIO.setmode(GPIO.BOARD)         # Setup the GPIO 
GPIO.setup(ledPin, GPIO.OUT)     # Set LED pin as output 
GPIO.output(ledPin, GPIO.LOW)    # Turn off the LED pin

print("Let blinking commence!")

for i in range(1,20):
  GPIO.output(ledPin, GPIO.HIGH) # Turn on the LED pin
  time.sleep(0.25)
  sys.stdout.write(".")
  sys.stdout.flush()
  GPIO.output(ledPin, GPIO.LOW)  # Turn off the LED pin
  time.sleep(0.25)

GPIO.cleanup()                   # Shutdown GPIO

print("\nThanks for blinking!")
