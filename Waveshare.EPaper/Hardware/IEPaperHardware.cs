using System.Device.Gpio;

namespace Waveshare.EPaper.Hardware;

public interface IEPaperHardware : IDisposable
{ 
    PinValue ResetPin { get; set; }
    
    PinValue SpiDcPin { get; set; }
    
    PinValue SpiCsPin { get; set; }
    
    PinValue BusyPin { get; set; }

    void SpiWrite(byte value);

    void SpiWrite(ReadOnlySpan<byte> buffer);

    void SpiSendData(byte[] register);

    void SpiSendData(byte register);
}