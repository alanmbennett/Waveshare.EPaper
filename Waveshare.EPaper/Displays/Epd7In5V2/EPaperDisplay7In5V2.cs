using System.Device.Gpio;
using System.Diagnostics;

namespace Waveshare.EPaper.Displays.Epd7In5V2;

public class EPaperDisplay7In5V2 : IEPaperDisplay
{
    private bool _disposed;
    private readonly IEPaperHardware _hardware;
    
    private const short _width = 800;
    private const short _height = 480;
    private const int _pixelPerByte = 8;

    public EPaperDisplay7In5V2()
    {
        _hardware = new EPaperHardware();
        
        Debug.WriteLine("e-Paper Init and Clear...");
        Initialize();
        Clear();
        _hardware.Delay(500);
    }
    public void Initialize()
    {
        Reset();
        
        SendCommand(Epd7In5V2Commands.PowerSetting);
        SendData(0x07);
        SendData(0x07); // VGH=20V,VGL=-20V
        SendData(0x3f); // VDH = 15V
        SendData(0x3f); // VDL=-15V
        
        SendCommand(Epd7In5V2Commands.BoosterSoftStart);
        SendData(0x17);
        SendData(0x17);
        SendData(0x28);
        SendData(0x17);
        
        SendCommand(Epd7In5V2Commands.PowerOn);
        _hardware.Delay(100);
        WaitUntilIdle();
        
        SendCommand(Epd7In5V2Commands.PanelSetting);
        SendData(0x1F); // KW-3f   KWR-2F	BWROTP 0f	BWOTP 1f

        SendCommand(Epd7In5V2Commands.Tres);
        SendData(0x03); // Source 800
        SendData(0x20);
        SendData(0x01); // gate 480
        SendData(0xE0);
        
        SendCommand(Epd7In5V2Commands.DualSpi);
        SendData(0x00);
        
        SendCommand(Epd7In5V2Commands.VcomAndDataIntervalSetting);
        SendData(0x10);
        SendData(0x07);
        
        SendCommand(Epd7In5V2Commands.TconSetting);
        SendData(0x22);
    }
    
    public void Reset()
    {
        _hardware.ResetPin = PinValue.High;
        _hardware.Delay(20);
        _hardware.ResetPin = PinValue.Low;
        _hardware.Delay(2);
        _hardware.ResetPin = PinValue.High;
        _hardware.Delay(20);
    }

    private void SendCommand(byte register)
    {
        _hardware.DcPin = PinValue.Low;
        _hardware.CsPin = PinValue.Low;
        _hardware.SpiWrite(register);
        _hardware.CsPin = PinValue.High;
    }
    
    private void SendCommand(Epd7In5V2Commands command) => SendCommand((byte)command);

    private void SendData(byte data)
    {
        _hardware.DcPin = PinValue.High;
        _hardware.CsPin = PinValue.Low;
        _hardware.SpiWrite(data);
        _hardware.CsPin = PinValue.High;
    }

    private void SendData(byte[] data)
    {
        _hardware.DcPin = PinValue.High;
        _hardware.CsPin = PinValue.Low;
        _hardware.SpiWrite(data);
        _hardware.CsPin = PinValue.High;
    }

    public void WaitUntilIdle(CancellationToken token = default)
    {
        Debug.WriteLine("e-Paper busy");
        do
        {
            token.ThrowIfCancellationRequested();
            _hardware.Delay(5);
        } while (_hardware.BusyPin != PinValue.High);
        
        _hardware.Delay(5);
        Debug.WriteLine("e-Paper busy release");
    }
    
    public void TurnOnDisplay()
    {
        SendCommand(Epd7In5V2Commands.DisplayRefresh);
        _hardware.Delay(100);
        WaitUntilIdle();
    }

    public void Clear()
    {
        var imageBytes = new byte[_width / _pixelPerByte];
        
        SendCommand(Epd7In5V2Commands.DataStartTransmission1);
        Array.Fill<byte>(imageBytes, 0xFF);
        for (var count = 0; count < _height; count++)
        {
            SendData(imageBytes);
        }
        
        SendCommand(Epd7In5V2Commands.DataStartTransmission2);
        Array.Fill<byte>(imageBytes, 0x00);
        for (var count = 0; count < _height; count++)
        {
            SendData(imageBytes);
        }
        
        TurnOnDisplay();
    }
    
    public void ClearBlack()
    {
        var imageBytes = new byte[_width / _pixelPerByte];
        
        SendCommand(Epd7In5V2Commands.DataStartTransmission1);
        Array.Fill<byte>(imageBytes, 0x00);
        for (var count = 0; count < _height; count++)
        {
            SendData(imageBytes);
        }
        
        SendCommand(Epd7In5V2Commands.DataStartTransmission2);
        Array.Fill<byte>(imageBytes, 0xFF);
        for (var count = 0; count < _height; count++)
        {
            SendData(imageBytes);
        }
        
        TurnOnDisplay();
    }

    public void Sleep()
    {
        SendCommand(Epd7In5V2Commands.VcomAndDataIntervalSetting);
        SendData(0xF7);
        
        SendCommand(Epd7In5V2Commands.PowerOff);
        WaitUntilIdle();
        
        SendCommand(Epd7In5V2Commands.DeepSleep);
        SendData(0xA5);
    }
    
    ~EPaperDisplay7In5V2() => Dispose(false);

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

        Sleep();
        _hardware.Delay(2000);
        Debug.WriteLine("close 5V, Module enters 0 power consumption ...");
        _hardware.Dispose();
    }
}