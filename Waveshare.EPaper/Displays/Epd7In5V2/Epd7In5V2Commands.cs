namespace Waveshare.EPaper.Displays.Epd7In5V2;

public enum Epd7In5V2Commands
{
    Unknown = -1,
    PanelSetting = 0x00,
    PowerSetting = 0x01,
    PowerOff = 0x02,
    PowerOn = 0x04,
    BoosterSoftStart = 0x06,
    DeepSleep = 0x07,
    DataStartTransmission1 = 0x10,
    DisplayRefresh = 0x12,
    DataStartTransmission2 = 0x13,
    DualSpi = 0x15,
    VcomAndDataIntervalSetting = 0x50,
    Tres = 0x61,
}