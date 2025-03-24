using Waveshare.EPaper.Displays;
using Waveshare.EPaper.Hardware;

namespace Waveshare.EPaper;

public static class EPaperModule
{
    private static readonly Lazy<EPaperHardware> _hardware = new(() => new EPaperHardware());
    
    public delegate TDisplay ConfigureDisplay<out TDisplay>(IEPaperHardware hardware) where TDisplay : IEPaperDisplay;
    
    public static IEPaperDisplay Create<TDisplay>(ConfigureDisplay<TDisplay> configureDisplay) where TDisplay : IEPaperDisplay
        => configureDisplay(_hardware.Value);
}