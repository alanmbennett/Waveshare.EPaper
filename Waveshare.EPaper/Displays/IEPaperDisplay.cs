using Waveshare.EPaper.Hardware;

namespace Waveshare.EPaper.Displays;

public interface IEPaperDisplay : IDisposable
{
    void Initialize();
    
    void Reset();

    void WaitUntilIdle(CancellationToken token = default);

    void TurnOnDisplay();

    void Clear();

    void ClearBlack();

    void Display(byte[] blackImageBytes);

    void Delay(int milliseconds);

    void Sleep();
}