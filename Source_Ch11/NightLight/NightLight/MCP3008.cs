using System;
using Windows.Devices.Spi;         // add this for SPI communication
using System.Diagnostics;          // add this for debugging

namespace NightLight
{
    class MCP3008
    {
        // SPI controller interface
        private SpiDevice mcp_var;
        const int SPI_CHIP_SELECT_LINE = 0;
        const byte MCP3008_SingleEnded = 0x08;
        const byte MCP3008_Differential = 0x00;

        // These are used when we calculate the voltage from the ADC units
        float ReferenceVoltage = 5.0F;
        private const int MAX = 1023;

        public MCP3008()
        {
            Debug.WriteLine("New class instance of MCP3008 created.");
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

                SpiController controller = await SpiController.GetDefaultAsync();     /* Get the default SPI controller */

                mcp_var = controller.GetDevice(SPI_settings);

                if (mcp_var == null)
                {
                    Debug.WriteLine("ERROR! SPI device may be in use.");
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

        // Utility function to get voltage from ADC
        public float ADCToVoltage(int value)
        {
            return (float)value * ReferenceVoltage / (float)MAX;
        }
    }
}
