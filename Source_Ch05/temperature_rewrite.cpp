//
// Windows 10 for the IOT
//
// Example C++ console application rewrite.
//
// Created by Dr. Charles Bell
//
#include "pch.h"

using namespace std;

double convert_temp(char scale, double base_temp) {
	if ((scale == 'c') || (scale == 'C')) {
		return ((9.0 / 5.0) * base_temp) + 32.0;
	} else if ((scale == 'f') || (scale == 'F')) {
		return (5.0 / 9.0) * (base_temp - 32.0);
	}
	return 0.0;
}

int main(int argc, char **argv)
{
	double temp_read = 0.0;
	char scale{'c'};

	cout << "Welcome to the temperature conversion application.\n";
	cout << "Please choose a starting scale (F) or (C): ";
	cin >> scale;
    cout << "Please enter a temperature: ";
	cin >> temp_read;
	if ((scale == 'c') || (scale == 'C')) {
		cout << "Converting value from Celsius to Fahrenheit.\n";
		cout << temp_read << " degrees Celsius = " <<
		    convert_temp(scale, temp_read) << " degrees Fahrenheit.\n";
	} else if ((scale == 'f') || (scale == 'F')) {
		cout << "Converting value from Fahrenheit to Celsius.\n";
		cout << temp_read << " degrees Fahrenheit = " <<
		    convert_temp(scale, temp_read) << " degrees Celsius.\n";
	} else {
		cout << "\nERROR: I'm sorry, I don't understand '" << scale << "'.";
		return -1;
	}
}
