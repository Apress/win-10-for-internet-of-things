using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;        // Add for Task.Run()
using MySql.Data.MySqlClient;        // Add for MySQL connection
using System.Diagnostics;            // Add for debugging
using Glovebox.IoT.Devices.Sensors;  // Add for BMP280 (or BME280)

namespace WeatherDatabase
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer bmpTimer;    // Timer
        private MySqlConnection mysql_conn;  // Connection to MySQL   
        private BMP280 tempAndPressure;      // Instance of BMP280 class
     
        // String constants for database
        private string connStr = "server=10.0.1.18;user=w_user;password=secret;" +
                                 "port=3306;database=weather;sslMode=None";
        private string INSERT = "INSERT INTO weather.history VALUES (null, null, " + 
                                "{0}, {1}, {2}, {3}, {4})";

        public MainPage()
        {
            this.InitializeComponent();

            // Instantiate a new BMP280 class instance
            tempAndPressure = new BMP280();

            // Connect to MySQL. If successful, setup timer
            if (this.Connect())
            {
                this.bmpTimer = new DispatcherTimer();
                this.bmpTimer.Interval = TimeSpan.FromMilliseconds(5000);
                this.bmpTimer.Tick += BmpTimer_Tick;
                this.bmpTimer.Start();
            }
            else
            {
                Debug.WriteLine("ERROR: Cannot proceed without database connection.");
            }
        }

        private Boolean Connect()
        { 
            mysql_conn = new MySqlConnection(connStr);
            try
            {
                Debug.WriteLine("Connecting to MySQL...");
                mysql_conn.Open();
                Debug.WriteLine("Connected to " + mysql_conn.ServerVersion + ".");
            }
            catch (Exception ex)
            {
                Debug.Write("ERROR: ");
                Debug.WriteLine(ex.ToString());
                return false;
            }
            return true; 
        }

        private void BmpTimer_Tick(object sender, object e)
        {
            var t = Task.Run(() => getData());
        }

        public void getData()
        {
            var degreesCelsius = tempAndPressure.Temperature.DegreesCelsius;
            var degreesFahrenheit = tempAndPressure.Temperature.DegreesFahrenheit;
            var bars = tempAndPressure.Pressure.Bars;
            var hectopascals = tempAndPressure.Pressure.Hectopascals;
            var atmospheres = tempAndPressure.Pressure.Atmospheres;

            Debug.WriteLine(degreesCelsius);
            Debug.WriteLine(degreesFahrenheit);
            Debug.WriteLine(bars);
            Debug.WriteLine(hectopascals);
            Debug.WriteLine(atmospheres);
            try
            {
                // Format the query string with data read
                String insert_str = String.Format(INSERT, degreesCelsius, degreesFahrenheit,
                    bars, hectopascals, atmospheres);
                // Create a new command and setup the query
                MySqlCommand cmd = new MySqlCommand(insert_str, mysql_conn);
                // Execute the query
                cmd.ExecuteNonQuery();
                Debug.WriteLine("Data inserted.");
            }
            catch (Exception ex)
            {
                Debug.Write("ERROR: ");
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
