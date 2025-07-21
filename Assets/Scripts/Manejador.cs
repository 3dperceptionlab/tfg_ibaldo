using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using TMPro;

public class Manejador : MonoBehaviour
{
    public GameObject bienvenida;
    public GameObject capturaArea;
    public GameObject ayudaPanel;
    public Button botonEco;
    private ObserverBehaviour[] imageTargets;

    void Start()
    {
        capturaArea.SetActive(false);

        // Buscar todos los Image Targets en la escena sin ordenarlos (más eficiente)
        imageTargets = FindObjectsByType<ObserverBehaviour>(FindObjectsSortMode.None);
        botonEco.gameObject.SetActive(false);
        ayudaPanel.SetActive(false);
        if (PlayerPrefs.GetInt("PRIMERAVEZ", 1) == 1)
        {
            
            DesactivarDeteccion();
            bienvenida.SetActive(true);
        }
        else
        {
            
            bienvenida.SetActive(false);
            ActivarDeteccion();
        }

        // Suscribirse a los eventos de cada Image Target
        foreach (var target in imageTargets)
        {
            target.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    public void onClickBienvenida()
    {
        PlayerPrefs.SetInt("PRIMERAVEZ", 0);
        PlayerPrefs.Save();
       

        if (bienvenida != null)
            bienvenida.SetActive(false);

        ActivarDeteccion();
    }

    private void DesactivarDeteccion()
    {
        foreach (var target in imageTargets)
        {
            target.enabled = false;
        }
        botonEco.gameObject.SetActive(false);
    }

    private void ActivarDeteccion()
    {
        capturaArea.SetActive(true);
        foreach (var target in imageTargets)
        {
            target.enabled = true;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED || status.Status == Status.LIMITED)
        {
            //Comprobar que el observer no es el deviceobserver
            if (behaviour.TargetName == "DeviceObserver")
            {
                return;
            }
            capturaArea.SetActive(false);
        }
        else if (status.Status == Status.NO_POSE)
        {
            capturaArea.SetActive(true);
        }
    }

    void OnDestroy()
    {
        // Desuscribirse de los eventos de cada Image Target
        foreach (var target in imageTargets)
        {
            target.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}
