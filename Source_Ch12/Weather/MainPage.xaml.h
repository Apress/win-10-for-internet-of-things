//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

using namespace WeatherSensor;  // Add for sensor C# project

namespace Weather
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();
	private:
		// Reference to BMP280 C# class
		BMP280 ^bmp280 = ref new BMP280();

		// Variable for the timer
		Windows::UI::Xaml::DispatcherTimer ^timer_;

		// Timer on tick method
		void OnTick(Platform::Object ^sender, Platform::Object ^args);
	};
}
