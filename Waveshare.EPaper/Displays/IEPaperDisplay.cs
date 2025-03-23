namespace Waveshare.EPaper.Displays;

public interface IEPaperDisplay : IDisposable
{
    void Initialize();
    
    void Reset();

    void WaitUntilIdle(CancellationToken token = default);

    void TurnOnDisplay();

    void Clear();

    void ClearBlack();

    void Sleep();
}