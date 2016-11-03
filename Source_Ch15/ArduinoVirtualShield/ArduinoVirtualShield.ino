/*
    Copyright(c) Microsoft Open Technologies, Inc. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

/*
    About the Text Shield:
    
    Text provides a very simple LCD-like fixed font displayed on your device.
    Text also provides a clear() to clear the whole screen, and a clearLine()
    and clearId() to clear specific lines or ids.
    
    Text provides these example methods:
    clear() ... clear the screen to black.
    printAt(1, "Testing") ... displays Testing at line 1
    
*/

// Include the ArduinoJson library, a dependency.
#include <ArduinoJson.h>

// VirtualShield is the core of all shields.
#include <VirtualShield.h>

// Text is to display text onscreen.
#include <Text.h>

// Colors brings in commonly named colors.
#include <Colors.h>

// Instantiate the shields.
VirtualShield shield;
Text screen = Text(shield);

// Read the TMP36 sensor and display the value
float readTemp()
{
  int TMP36 = 5;
  int rawtemp = 0;
  float volts = 0.0;
  float temp = 0.0;

  rawtemp = analogRead(TMP36);  
  volts = (rawtemp * 5.0) / 1024.0; 
  temp = (volts - 0.5) * 100;
  return temp;  
}

// Callback for startup, reconnection, and when the pushing 'Refresh' button
void refresh(ShieldEvent* shieldEvent)
{
  float temperature = 0.0;

  // read the temperature
  temperature = readTemp();
  // display the temperature in Celsius
  screen.clear(ARGB(50,150,255));  // Show a blue screen
  screen.printAt(0, "Welcome to the Arduino Virtual Shield", WHITE);
  screen.printAt(1, "Remote Temperature Sensor!", WHITE);
  screen.printAt(2, "Temperature = " + String(temperature) + " Celsius.", WHITE);
}

void setup()
{
  // Call 'refresh' on startup, on reconnection, and when the pushing 'Refresh' button
  shield.setOnRefresh(refresh);
  
  // Begin the shield communication
  shield.begin();
}

void loop()
{
  // checkSensors() checks for return events and handles them (calling callbacks). This is VirtualShield's single loop() method.
  shield.checkSensors();
}		 
