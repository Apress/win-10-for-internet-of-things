//
// Colour Sensor Project - Arduino Wiring
//
// This sketch is an Arduino Wiring sketch for Windows 10 IOT Core.
//
// Developed by: Charles Bell with examples from Microsoft and Adafruit
//
#include <Wire.h>
#include "Adafruit_TCS34725.h"
#include <LiquidCrystal.h>

int enablePin = GPIO16;
int registerSelectPin = GPIO21;
int dataPin11 = GPIO17;
int dataPin12 = GPIO18;
int dataPin13 = GPIO19;
int dataPin14 = GPIO20;

//create a pointer to an instance of LiquidCrystal, yet to be created
LiquidCrystal *lcd;

Adafruit_TCS34725 colorSensor = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_700MS, TCS34725_GAIN_1X);
const int buttonPin = GPIO4;

void setup()
{
	//create the LiquidCrystal instance with the pins as set
	lcd = new LiquidCrystal(registerSelectPin, enablePin, dataPin11, dataPin12, dataPin13, dataPin14);

	// set up the LCD's number of columns and rows:
	lcd->begin(16, 2);

	lcd->clear();
	lcd->setCursor(0, 0);
	if (colorSensor.begin()) {
		lcd->print("Sensor found.");
		colorSensor.setInterrupt(true);
	}
	else {
		lcd->print("NO SENSOR!");
		while (1);
	}
	lcd->setCursor(0, 1);
	lcd->print("Ready!");
}

void read_colour() {
	uint16_t red, green, blue, raw, colourTemperature;

	// Read the values amd calculate the colour and lumens values
	colorSensor.getRawData(&red, &green, &blue, &raw);
	colourTemperature = colorSensor.calculateColorTemperature(red, green, blue);

	// Display the results in the debug window.
	lcd->clear();
	lcd->setCursor(0, 0);
	lcd->print("Color Temp: ");
	lcd->print(colourTemperature);
	lcd->setCursor(0, 1);
	lcd->print(red);
	lcd->print(",");
	lcd->print(green);
	lcd->print(",");
	lcd->print(blue);
}

void loop() {
	int buttonState = 0;

	delay(1000);
	buttonState = digitalRead(buttonPin);
	if (buttonState == HIGH) {
		lcd->clear();
		lcd->print("Reading sensor.");
		colorSensor.setInterrupt(false);
		delay(500);
		read_colour();
		delay(500);
	}
	else {
		colorSensor.setInterrupt(true);
	}
}
