using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CalcularProbabilidad : MonoBehaviour
{
    private TimeSpan salidaSol;
    private TimeSpan puestaSol;
    private DateTime horaActual;
    public TextMeshProUGUI texto;
    
    void Start() {
        horaActual = DateTime.Now;
        if(texto != null) {
            texto.text = "Hora actual: " + horaActual.ToString();
        }
    }
    
    public double Calcular() {
        return 50.0;
    }
    

}