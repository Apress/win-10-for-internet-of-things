//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace BlinkCPPStyle;

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
using namespace Windows::Devices::Enumeration;  // Add this
using namespace Windows::Devices::Gpio;         // Add this
using namespace concurrency;                    // Add this


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

MainPage::MainPage()
{
	InitializeComponent();
	InitGPIO();
	if (pin_ != nullptr) {
		timer_ = ref new DispatcherTimer();
		TimeSpan interval;
		interval.Duration = 500 * 1000 * 10;
		timer_->Interval = interval;
		timer_->Tick += ref new EventHandler<Object ^>(this, &MainPage::OnTick);
	}
}

void MainPage::InitGPIO()
{
	auto gpio = GpioController::GetDefault();

	if (gpio == nullptr) {
		pin_ = nullptr;
		status->Text = "No GPIO Controller!";
		return;
	}
	pin_ = gpio->OpenPin(LED_PIN);
	pin_->Write(pinValue_);
	pin_->SetDriveMode(GpioPinDriveMode::Output);

	status->Text = "We're good to go!";
}


void MainPage::OnTick(Object ^sender, Object ^args)
{
	if (pinValue_ == Windows::Devices::Gpio::GpioPinValue::High) {
		pinValue_ = Windows::Devices::Gpio::GpioPinValue::Low;
		pin_->Write(pinValue_);
		led_indicator->Fill = grayFill_;
	}
	else {
		pinValue_ = Windows::Devices::Gpio::GpioPinValue::High;
		pin_->Write(pinValue_);
		led_indicator->Fill = greenFill_;
	}
}


void BlinkCPPStyle::MainPage::start_stop_button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	blinking = !blinking;
	if (blinking) {
		timer_->Start();
		led_indicator->Fill = greenFill_;
		start_stop_button->Content = "Stop";
	}
	else {
		timer_->Stop();
		led_indicator->Fill = grayFill_;
		start_stop_button->Content = "Start";
	}
}
