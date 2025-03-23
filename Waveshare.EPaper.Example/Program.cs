using System.Diagnostics;
using Waveshare.EPaper.Displays.Epd7In5V2;

Debug.WriteLine("EPD_7IN5B_V2_test Demo");
using var display = new EPaperDisplay7In5V2();

display.Initialize();
display.Clear();
