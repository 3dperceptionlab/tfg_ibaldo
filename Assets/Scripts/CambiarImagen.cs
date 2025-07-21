using UnityEngine;
using TMPro;
public class CambiarImagen : MonoBehaviour
{
    public TextMeshProUGUI texto;
    private int contador = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void changeImagen() {
        contador++;
        if (texto != null)
        {
            texto.text = "Pulsado: " + contador;
        }
    }
}
