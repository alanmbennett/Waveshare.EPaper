using System.Device.Gpio;
using Waveshare.EPaper.Hardware;

namespace Waveshare.EPaper.Displays;

public abstract class BaseEPaperDisplay(IEPaperHardware hardware) : IEPaperDisplay
{
    private bool _disposed;
    
    protected readonly IEPaperHardware Hardware = hardware;
    
    public abstract void Initialize();

    public abstract void Reset();

    public abstract void WaitUntilIdle(CancellationToken token = default);

    public abstract void TurnOnDisplay();

    public abstract void Clear();

    public abstract void ClearBlack();

    public abstract void Display(byte[] blackImageBytes);

    public abstract void Sleep();
    
    public void Delay(int milliseconds) => Thread.Sleep(milliseconds);
    
    protected void SendCommand(byte register)
    {
        Hardware.SpiDcPin = PinValue.Low;
        Hardware.SpiCsPin = PinValue.Low;
        Hardware.SpiWrite(register);
        Hardware.SpiCsPin = PinValue.High;
    }
    
    protected void SendData(byte data)
    {
        Hardware.SpiDcPin = PinValue.High;
        Hardware.SpiCsPin = PinValue.Low;
        Hardware.SpiWrite(data);
        Hardware.SpiCsPin = PinValue.High;
    }

    protected void SendData(ReadOnlySpan<byte> data)
    {
        Hardware.SpiDcPin = PinValue.High;
        Hardware.SpiCsPin = PinValue.Low;
        Hardware.SpiWrite(data);
        Hardware.SpiCsPin = PinValue.High;
    }
    
    ~BaseEPaperDisplay() => Dispose(false);
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
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
        
        Hardware.Dispose();
    }
}