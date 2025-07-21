using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class DatosPuesta
{
    public string date { get; set; }
    public string sunrise { get; set; }
    public string sunset { get; set; }
    public string firstLight { get; set; }
    public string lastLight { get; set; }
    public string dawn { get; set; }
    public string dusk { get; set; }
    public string solarNoon { get; set; }
    public string goldenHour { get; set; }
    public string dayLength { get; set; }
    public string timezone { get; set; }
    public int utcOffset { get; set; }
    
}

