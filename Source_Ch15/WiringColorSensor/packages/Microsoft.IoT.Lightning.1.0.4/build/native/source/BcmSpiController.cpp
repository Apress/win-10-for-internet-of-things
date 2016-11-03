// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  
// Licensed under the BSD 2-Clause License.  
// See License.txt in the project root for license information.

#include "BcmSpiController.h"
#include "BoardPins.h"

/**
\param[in] misoPin The pin number of the MISO signal.
\param[in] mosiPin The pin number of the MOSI signal.
\param[in] sckPin The pin number of the SCK signal
\return HRESULT success or error code.
*/
HRESULT BcmSpiControllerClass::configurePins(ULONG misoPin, ULONG mosiPin, ULONG sckPin)
{
    HRESULT hr = S_OK;

    // Save the pin numbers.
    m_sckPin = sckPin;
    m_mosiPin = mosiPin;
    m_misoPin = misoPin;

    //
    // Set SCK and MOSI as SPI pins.
    //
    hr = g_pins.verifyPinFunction(m_sckPin, FUNC_SPI, BoardPinsClass::LOCK_FUNCTION);

    if (SUCCEEDED(hr))
    {
        hr = g_pins.verifyPinFunction(m_mosiPin, FUNC_SPI, BoardPinsClass::LOCK_FUNCTION);
    }

    //
    // Set MISO as an SPI pin.
    //
    if (SUCCEEDED(hr))
    {
        hr = g_pins.verifyPinFunction(m_misoPin, FUNC_SPI, BoardPinsClass::LOCK_FUNCTION);
    }

    //
    // If this method failed, set all the pins back to digital I/O (ignoring any errors
    // that occur in the process.)
    //
    if (FAILED(hr))
    {
        revertPinsToGpio();
    }

    return hr;
}

/**
Initialize member variables, including the SPI Clock rate variables.
*/
BcmSpiControllerClass::BcmSpiControllerClass()
{
    m_hController = INVALID_HANDLE_VALUE;
    m_registers = nullptr;

    // Load values for the SPI clock generator divisors.
    // SPI clock is 250mhz / Divisor.  Divisors must be even, and < 65536.
    spiSpeed10mhz = 26;      // Fastest supported SPI clock on BCM2836 is 10mhz
    spiSpeed8mhz = 32;
    spiSpeed4mhz = 64;
    spiSpeed2mhz = 126;
    spiSpeed1mhz = 250;
    spiSpeed500khz = 500;
    spiSpeed250khz = 1000;
    spiSpeed125khz = 2000;
    spiSpeed50khz = 5000;
    spiSpeed31k25hz = 8000;  // 31.25 khz for MIDI
    spiSpeed25khz = 10000;
    spiSpeed10khz = 25000;
    spiSpeed5khz = 50000;
    spiSpeed4khz = 62500;
}

/**
\param[in] busNumber The number of the SPI bus to open (0 or 1)
\param[in] mode The SPI mode (clock polarity and phase: 0, 1, 2 or 3)
\param[in] clockKhz The clock speed to use for the SPI bus
\param[in] dataBits The size of an SPI transfer in bits
\return HRESULT success or error code.
*/
HRESULT BcmSpiControllerClass::begin(ULONG busNumber, ULONG mode, ULONG clockKhz, ULONG dataBits)
{
    HRESULT hr = S_OK;
    
    PWCHAR deviceName = nullptr;
    PVOID baseAddress = nullptr;
    _CS cs;


    // If this object does not yet have the SPI bus open:
    if (m_hController == INVALID_HANDLE_VALUE)
    {
        // Get the name of the PCI device that describes the SPI controller.
        switch (busNumber)
        {
        case EXTERNAL_SPI_BUS:
            deviceName = pi2Spi0DeviceName;
            break;
        case SECOND_EXTERNAL_SPI_BUS:
            deviceName = pi2Spi1DeviceName;
            break;
        default:    // Only support two SPI buses
            hr = DMAP_E_SPI_BUS_REQUESTED_DOES_NOT_EXIST;
        }

        if (SUCCEEDED(hr))
        {
            // Open the Dmap device for the SPI controller for exclusive access.
            hr = GetControllerBaseAddress(deviceName, m_hController, baseAddress);
            if (SUCCEEDED(hr))
            {
                m_registers = (PSPI_CONTROLLER)baseAddress;
            }
        }

        //
        // We now "own" the SPI controller, intialize it.
        //

        if (SUCCEEDED(hr))
        {
            hr = setClock(clockKhz);
        }

        if (SUCCEEDED(hr))
        {
            hr = setMode(mode);
        }

        if (SUCCEEDED(hr))
        {
            cs.ALL_BITS = 0;
            cs.CPHA = m_clockPhase;
            cs.CPOL = m_clockPolarity;
            cs.CLEAR = 3;                       // Clear both FIFOs,
            cs.TA = 1;                          //  then start transfers.
            m_registers->CS.ALL_BITS = cs.ALL_BITS;
        }
    }

    return hr;
}

/**
Unmap and close the SPI controller associated with this object.
*/
void BcmSpiControllerClass::end()
{
    _CS cs;

    if (m_registers != nullptr)
    {
        cs.ALL_BITS = m_registers->CS.ALL_BITS;
        cs.TA = 0;                              // No tranfer is active
        m_registers->CS.ALL_BITS = cs.ALL_BITS;

        m_registers = nullptr;
    }

    if (m_hController != INVALID_HANDLE_VALUE)
    {
        // Unmap the SPI controller.
        DmapCloseController(m_hController);
    }
}

/**
This method follows the Arduino conventions for SPI mode settings.
The SPI mode specifies the clock polarity and phase.
\param[in] mode The mode to set for the SPI bus (0, 1, 2 or 3)
\return HRESULT success or error code.
*/
HRESULT BcmSpiControllerClass::setMode(ULONG mode)
{
    HRESULT hr = S_OK;
    
    // Determine the clock phase and polarity settings for the requested mode.
    switch (mode)
    {
    case 0:
        m_clockPolarity = 0;   // Clock inactive state is low
        m_clockPhase = 0;      // Sample data on active going clock edge
        break;
    case 1:
        m_clockPolarity = 0;   // Clock inactive state is low
        m_clockPhase = 1;      // Sample data on inactive going clock edge
        break;
    case 2:
        m_clockPolarity = 1;   // Clock inactive state is high
        m_clockPhase = 0;      // Sample data on active going clock edge
        break;
    case 3:
        m_clockPolarity = 1;   // Click inactive state is high
        m_clockPhase = 1;      // Sample data on inactive going clock edge
        break;
    default:
        hr = DMAP_E_SPI_MODE_SPECIFIED_IS_INVALID;
    }

    return hr;
}

/**
This method sets one of the SPI clock rates we support: 1 khz - 15 mhz.
\param[in] clockKhz Desired clock rate in Khz.
\return HRESULT success or error code.
*/
HRESULT BcmSpiControllerClass::setClock(ULONG clockKhz)
{
    HRESULT hr = S_OK;
    ULONG speed = spiSpeed4mhz;
    _CLK clk;

    // If we don't have the controller registers mapped, fail.
    if (m_registers == nullptr)
    {
        hr = DMAP_E_DMAP_INTERNAL_ERROR;
    }

    if (SUCCEEDED(hr))
    {
        // Round down to the closest clock rate we support.
        if (clockKhz >= 10000)
        {
            speed = spiSpeed10mhz;
        }
        else if (clockKhz >= 8000)
        {
            speed = spiSpeed8mhz;
        }
        else if (clockKhz >= 4000)
        {
            speed = spiSpeed4mhz;
        }
        else if (clockKhz >= 2000)
        {
            speed = spiSpeed2mhz;
        }
        else if (clockKhz >= 1000)
        {
            speed = spiSpeed1mhz;
        }
        else if (clockKhz >= 500)
        {
            speed = spiSpeed500khz;
        }
        else if (clockKhz >= 250)
        {
            speed = spiSpeed250khz;
        }
        else if (clockKhz >= 125)
        {
            speed = spiSpeed125khz;
        }
        else if (clockKhz >= 50)
        {
            speed = spiSpeed50khz;
        }
        else if (clockKhz >= 31)
        {
            speed = spiSpeed31k25hz;
        }
        else if (clockKhz >= 25)
        {
            speed = spiSpeed25khz;
        }
        else if (clockKhz >= 10)
        {
            speed = spiSpeed10khz;
        }
        else if (clockKhz >= 5)
        {
            speed = spiSpeed5khz;
        }
        else if (clockKhz >= 4)
        {
            speed = spiSpeed4khz;
        }
        else
        {
            hr = DMAP_E_SPI_SPEED_SPECIFIED_IS_INVALID;
        }
    }

    if (SUCCEEDED(hr))
    {
        // Set the clock rate.
        clk.ALL_BITS = 0;
        clk.CDIV = speed & 0xFFFF;
        m_registers->CLK.ALL_BITS = clk.ALL_BITS;
    }
    
    return hr;
}
