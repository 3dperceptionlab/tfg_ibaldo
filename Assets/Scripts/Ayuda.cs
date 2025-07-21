using UnityEngine;
using Vuforia;
public class Ayuda : MonoBehaviour
{
    public GameObject ayudaPanel; 
    public GameObject ayudaButton; 
    public GameObject capturaImage;
    
    void Start()
    {
        
    }

    public void ShowAyuda()
    {
        ayudaPanel.SetActive(true); 
        ayudaButton.SetActive(false);
        capturaImage.SetActive(false); 
        
    }
    
    public void HideAyuda()
    {
        ayudaPanel.SetActive(false); 
        ayudaButton.SetActive(true); 
        capturaImage.SetActive(true); 
    }
}
