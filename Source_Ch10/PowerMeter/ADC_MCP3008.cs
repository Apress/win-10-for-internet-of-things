using System;
using Windows.Devices.Spi;         // add this for SPI communication
using System.Diagnostics;          // add this for debugging
using Windows.Devices.Enumeration; // add this for DeviceInformation

namespace PowerMeter
{
    class ADC_MCP3008
    {
        // SPI controller interface
        private SpiDevice mcp_var;
        const int SPI_CHIP_SELECT_LINE = 0;  
        const byte MCP3008_SingleEnded = 0x08;
        const byte MCP3008_Differential = 0x00;

        public ADC_MCP3008()
        {
            Debug.WriteLine("New class instance of ADC_MCP3008 created.");
        }

        // Setup the MCP3008 chip
        public async void Initialize()
        {
            Debug.WriteLine("Setting up the MCP3008.");
            try
            {
                // Settings for the SPI bus 
                var SPI_settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                SPI_settings.ClockFrequency = 3600000;
                SPI_settings.Mode = SpiMode.Mode0;

                // Get the list of devices on the SPI bus and get a device instance
                string strDev = SpiDevice.GetDeviceSelector();
                var spidev = await DeviceInformation.FindAllAsync(strDev);

                // Create an SpiDevice with our bus controller and SPI settings           
                mcp_var = await SpiDevice.FromIdAsync(spidev[0].Id, SPI_settings);

                if (mcp_var == null)
                {
                    Debug.WriteLine("ERROR! SPI device {0} may be in used.", spidev[0].Id);
                    return;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("EXEPTION CAUGHT: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
        }

        // Communicate with the chip via the SPI bus. We must encode the command
        // as follows so that we can read the value returned (on byte boundaries).
        //
        // Example:
        // 
        //   WRITE 0000 000S GDDD .... .... ....
        //   READ  .... .... .... .N98 7654 3210
        //
        // Where:
        //
        //   S = start bit
        //   G = Single / Differential
        //   D = Chanel data 
        //   . = undefined, ignore
        //   N = 0 
        //   9-0 = 10 data bits
        //
        public int getValue(byte whichChannel)
        {
            byte command = whichChannel;
            command |= MCP3008_SingleEnded;
            command <<= 4;

            byte[] commandBuf = new byte[] { 0x01, command, 0x00 };
            byte[] readBuf = new byte[] { 0x00, 0x00, 0x00 };

            mcp_var.TransferFullDuplex(commandBuf, readBuf);
            Debug.Write("commandBuf = ");
            Debug.Write(commandBuf[0]);
            Debug.Write(commandBuf[1]);
            Debug.WriteLine(commandBuf[2]);

            int sample = readBuf[2] + ((readBuf[1] & 0x03) << 8);
            Debug.Write("readBuf = ");
            Debug.Write(readBuf[0]);
            Debug.Write(readBuf[1]);
            Debug.WriteLine(readBuf[2]);

            int s2 = sample & 0x3FF;
            return sample;
        }
    }
}
