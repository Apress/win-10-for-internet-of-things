//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

namespace BlinkCPPStyle
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

	private:
		// Add references for color brushes to paint the led_indicator control
		Windows::UI::Xaml::Media::SolidColorBrush ^greenFill_ = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
		Windows::UI::Xaml::Media::SolidColorBrush ^grayFill_ = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::LightGray);

		// Add the start and stop button click event header
		void start_stop_button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		// Add a constructor for the InitGPIO class
		void InitGPIO();
		// Add an event for the timer object
		void OnTick(Platform::Object ^sender, Platform::Object ^args);

		// Add references for the timer and a variable to store the GPIO pin value
		Windows::UI::Xaml::DispatcherTimer ^timer_;
		Windows::Devices::Gpio::GpioPinValue pinValue_ = Windows::Devices::Gpio::GpioPinValue::High;

		// Variables for blinking
		bool blinking{ false };
		const int LED_PIN = 5;
		Windows::Devices::Gpio::GpioPin^ pin_;
	};
}
