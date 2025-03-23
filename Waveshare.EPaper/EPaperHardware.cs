using System.Device.Gpio;
using System.Device.Spi;

namespace Waveshare.EPaper;

internal sealed class EPaperHardware : IEPaperHardware
{
    private bool _disposed;
    private readonly GpioController _gpio = new();
    private readonly SpiDevice _spiDevice;
    
    private const int _resetPin = 17;
    private const int _dcPin = 25;
    private const int _csPin = 8;
    private const int _powerPin = 18;
    private const int _busyPin = 24;
    private const int _masterOutSlaveInPin = 10;
    private const int _serialClockPin = 11;

    public PinValue ResetPin
    {
        get => _gpio.Read(_resetPin);
        set => _gpio.Write(_resetPin, value);
    }

    public PinValue DcPin
    {
        get => _gpio.Read(_dcPin);
        set => _gpio.Write(_dcPin, value);
    }

    public PinValue CsPin
    {
        get => _gpio.Read(_csPin);
        set => _gpio.Write(_csPin, value);
    }

    public PinValue PowerPin
    {
        get => _gpio.Read(_powerPin);
        set => _gpio.Write(_powerPin, value);
    }

    public PinValue BusyPin
    {
        get => _gpio.Read(_busyPin);
        set => _gpio.Write(_busyPin, value);
    }

    private PinValue MasterOutSlaveInPin
    {
        get => _gpio.Read(_masterOutSlaveInPin);
        set => _gpio.Write(_masterOutSlaveInPin, value);
    }
    
    private PinValue SerialClockPin
    {
        get => _gpio.Read(_serialClockPin);
        set => _gpio.Write(_serialClockPin, value);
    }
    
    public EPaperHardware()
    {
        GpioInitialize();

        var spiSettings = new SpiConnectionSettings(busId: 0, chipSelectLine: 0);
        _spiDevice = SpiDevice.Create(spiSettings);
    }

    public void Delay(int milliseconds) => Thread.Sleep(milliseconds);
    
    private void GpioInitialize()
    {
        _ = _gpio.OpenPin(_busyPin, PinMode.Input);
        _ = _gpio.OpenPin(_resetPin, PinMode.Output);
        _ = _gpio.OpenPin(_dcPin, PinMode.Output);
        _ = _gpio.OpenPin(_csPin, PinMode.Output);
        _ = _gpio.OpenPin(_powerPin, PinMode.Output);
        
        CsPin = PinValue.High;
        PowerPin = PinValue.High;
    }

    public void SpiWrite(byte value) => _spiDevice.WriteByte(value);
    
    public void SpiWrite(byte[] buffer) => _spiDevice.Write(buffer);

    public void SpiSendData(byte[] register)
    {
        foreach (var registerByte in register)
        {
            SpiSendData(registerByte);
        }
    }
    
    public void SpiSendData(byte register)
    {
        int j = register;
        _gpio.SetPinMode(_masterOutSlaveInPin, PinMode.Output);
        CsPin = PinValue.Low;
        for (var i = 0; i < 8; i++)
        {
            SerialClockPin = PinValue.Low;
            MasterOutSlaveInPin = (j & 0x80) == 1
                ? PinValue.High
                : PinValue.Low;
            SerialClockPin = PinValue.High;
            j <<= 1;
        }

        SerialClockPin = PinValue.Low;
        CsPin = PinValue.High;
    }

    ~EPaperHardware() => Dispose(false);
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        
        _disposed = true;
        if (!disposing)
        {
            return;
        }
        
        CsPin = PinValue.Low;
        PowerPin = PinValue.High;
        DcPin = PinValue.Low;
        ResetPin = PinValue.Low;
        
        _spiDevice.Dispose();
        _gpio.Dispose();
    }
}