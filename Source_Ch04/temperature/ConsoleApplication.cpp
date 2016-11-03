//
// Windows 10 for the IOT
//
// Example C++ console application to demonstrate how to build
// Windows 10 IOT Core applications.
//
// Created by Dr. Charles Bell
//
#include "pch.h"

using namespace std;

int main(int argc, char **argv)
{
	double fahrenheit = 0.0;
	double celsius = 0.0;
	double temperature = -.0;
	char scale{ 'c' };

	cout << "Welcome to the temperature conversion application.\n";
	cout << "Please choose a starting scale (F) or (C): ";
	cin >> scale;
	if ((scale == 'c') || (scale == 'C')) {
		cout << "Converting value from Celsius to Fahrenheit.\n";
		cout << "Please enter a temperature: ";
		cin >> celsius;
		fahrenheit = ((9.0 / 5.0) * celsius) + 32.0;
		cout << celsius << " degrees Celsius = " << fahrenheit <<
			" degrees Fahrenheit.\n";
	}
	else if ((scale == 'f') || (scale == 'F')) {
		cout << "Converting value from Fahrenheit to Celsius.\n";
		cout << "Please enter a temperature: ";
		cin >> fahrenheit;
		celsius = (5.0 / 9.0) * (fahrenheit - 32.0);
		cout << fahrenheit << " degrees Fahrenheit = " << celsius <<
			" degrees Celsius.\n";
	}
	else {
		cout << "\nERROR: I'm sorry, I don't understand '" << scale << "'.";
		return -1;
	}
}
