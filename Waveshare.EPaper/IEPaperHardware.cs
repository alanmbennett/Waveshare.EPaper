using System.Device.Gpio;

namespace Waveshare.EPaper;

public interface IEPaperHardware : IDisposable
{ 
    PinValue ResetPin { get; set; }
    
    PinValue DcPin { get; set; }
    
    PinValue CsPin { get; set; }
    
    PinValue PowerPin { get; set; }
    
    PinValue BusyPin { get; set; }

    void Delay(int milliseconds);

    void SpiWrite(byte value);

    void SpiWrite(byte[] buffer);

    void SpiSendData(byte[] register);

    void SpiSendData(byte register);
}