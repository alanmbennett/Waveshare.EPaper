using System.Diagnostics;
using Waveshare.EPaper;
using Waveshare.EPaper.Displays.Epd7In5V2;

Debug.WriteLine("EPD_7IN5B_V2_test Demo");
using var display = EPaperModule.Create(EPaperDisplay7In5V2.Configuration);

Debug.WriteLine("e-Paper Init and Clear...");
display.Initialize();
display.Clear();
display.Delay(500);

display.Initialize();
display.Clear();
