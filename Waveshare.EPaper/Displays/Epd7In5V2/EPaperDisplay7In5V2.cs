using System.Device.Gpio;
using System.Diagnostics;
using Waveshare.EPaper.Hardware;

namespace Waveshare.EPaper.Displays.Epd7In5V2;

public class EPaperDisplay7In5V2(IEPaperHardware hardware) : BaseEPaperDisplay(hardware)
{
    private bool _disposed;
    private bool _isSleeping;

    private const short _width = 800;
    private const short _height = 480;
    private const int _pixelPerByte = 8;
    private const int _widthByteArrayLength = _width / _pixelPerByte;
    
    public static EPaperModule.ConfigureDisplay<EPaperDisplay7In5V2> Configuration { get; } 
        = hardware => new EPaperDisplay7In5V2(hardware);

    public override void Initialize()
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
        Delay(100);
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

        _isSleeping = false;
    }
    
    public override void Reset()
    {
        Hardware.ResetPin = PinValue.High;
        Delay(20);
        Hardware.ResetPin = PinValue.Low;
        Delay(2);
        Hardware.ResetPin = PinValue.High;
        Delay(20);
    }
    
    private void SendCommand(Epd7In5V2Commands command) => SendCommand((byte)command);

    public override void WaitUntilIdle(CancellationToken token = default)
    {
        Debug.WriteLine("e-Paper busy");
        do
        {
            token.ThrowIfCancellationRequested();
            Delay(5);
        } while (Hardware.BusyPin != PinValue.High);
        
        Delay(5);
        Debug.WriteLine("e-Paper busy release");
    }
    
    public override void TurnOnDisplay()
    {
        SendCommand(Epd7In5V2Commands.DisplayRefresh);
        Delay(100);
        WaitUntilIdle();
    }

    public override void Clear()
    {
        var imageBytes = new byte[_widthByteArrayLength];
        
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
    
    public override void ClearBlack()
    {
        var imageBytes = new byte[_widthByteArrayLength];
        
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

    public override void Display(byte[] blackImageBytes)
    {
        ReadOnlySpan<byte> span = blackImageBytes;
        
        SendCommand(Epd7In5V2Commands.DataStartTransmission1);
        for (var index = 0; index < _height; index++)
        {
            SendData(span.Slice(index * _widthByteArrayLength, _widthByteArrayLength));
        }
        
        SendCommand(Epd7In5V2Commands.DataStartTransmission2);
        for (var heightIndex = 0; heightIndex < _height; heightIndex++)
        {
            for (var widthIndex = 0; widthIndex < _widthByteArrayLength; widthIndex++)
            {
                var index = widthIndex + heightIndex * _widthByteArrayLength;
                blackImageBytes[index] = (byte)~blackImageBytes[index];
            }
        }
        for (var index = 0; index < _height; index++)
        {
            SendData(span.Slice(index * _widthByteArrayLength, _widthByteArrayLength));
        }
        
        TurnOnDisplay();
    }

    public override void Sleep()
    {
        if (_isSleeping)
        {
            return;
        }
        
        SendCommand(Epd7In5V2Commands.VcomAndDataIntervalSetting);
        SendData(0xF7);
        
        SendCommand(Epd7In5V2Commands.PowerOff);
        WaitUntilIdle();
        
        SendCommand(Epd7In5V2Commands.DeepSleep);
        SendData(0xA5);
        
        _isSleeping = true;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            if (disposing)
            {
                Sleep();
                Delay(2000);
            }
        }

        base.Dispose(disposing);
    }
}