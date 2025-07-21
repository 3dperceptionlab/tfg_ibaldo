using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class Datos
{
    
    public double temperatura { get; set; }
    public int velocidadViento { get; set; }
    public double lluvia { get; set; }

    [XmlIgnore]
    public List<string> errores { get; set; } = new List<string>();
}

