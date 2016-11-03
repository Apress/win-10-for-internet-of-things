//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace Weather;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace concurrency; // Add for timer

MainPage::MainPage()
{

	InitializeComponent();

	// Initialize the sensor
	bmp280->Initialize();

	// Update screen initially

	// Setup timer
	timer_ = ref new DispatcherTimer();
	TimeSpan interval;
	interval.Duration = 1000 * 1000 * 10;
	timer_->Interval = interval;
	timer_->Tick += ref new EventHandler<Object ^>(this, &MainPage::OnTick);
	timer_->Start();
}

void MainPage::OnTick(Object ^sender, Object ^args)
{
	// Initialize the variables
	float temp = 0;
	float pressure = 0;
	float altitude = 0;

	// Create a constant for pressure at sea level. 
	// This is based on your local sea level pressure (Unit: Hectopascal)
	// To find the sea level pressure for your area, go to:
	// weather.gov and enter your city then read the pressure from the 
	// history. 
	const float seaLevelPressure = 1013.25f;

	// Read samples of the data every 10 seconds
	temp = bmp280->ReadTemperature();
	pressure = bmp280->ReadPreasure();
	altitude = bmp280->ReadAltitude(seaLevelPressure);

	Temp->Text = "Temperature: " + temp + " C";
	Press->Text = "Pressure: " + pressure + " Pa";
	Alt->Text = "Altitude: " + altitude + " m";
}
