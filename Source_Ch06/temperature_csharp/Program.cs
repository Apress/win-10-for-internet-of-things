//
// Example C# console application rewrite
//
// Created by Dr. Charles Bell
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace temperature_csharp
{
    class Program
    {
        static double convert_temp(char scale, double base_temp)
        {
            if ((scale == 'c') || (scale == 'C'))
            {
                return ((9.0/5.0) * base_temp) + 32.0;
            }
            else
            {
                return (5.0 / 9.0) * (base_temp - 32.0);
            }
        }

        static void Main(string[] args)
        {
            double temp_read = 0.0;
            char scale = 'c';

            Console.WriteLine("Welcome to the temperature conversion application.");
            Console.Write("Please choose a starting scale (F) or (C): ");
            scale = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.Write("Please enter a temperature: ");
            temp_read = Convert.ToDouble(Console.ReadLine());

            if ((scale == 'c') || (scale == 'C'))
            {
                Console.WriteLine("Converting value from Celsius to Fahrenheit.");
                Console.Write(temp_read);
                Console.Write(" degrees Celsius = ");
                Console.Write(convert_temp(scale, temp_read));
                Console.WriteLine(" degrees Fahrenheit.");
            }
            else if ((scale == 'f') || (scale == 'F'))
            {
                Console.WriteLine("Converting value from Fahrenheit to Celsius.");
                Console.Write(temp_read);
                Console.Write(" degrees Fahrenheit = ");
                Console.Write(convert_temp(scale, temp_read));
                Console.WriteLine(" degrees Celsius.");
            }
            else
            {
                Console.Write("ERROR: I'm sorry, I don't understand '");
                Console.Write(scale);
                Console.WriteLine("'.");
            }
        }
    }
}
