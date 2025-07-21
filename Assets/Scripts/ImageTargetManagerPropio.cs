using UnityEngine;
using TMPro;
using Vuforia;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Xml;
using System.Globalization;
using UnityEngine.UI;
public class ImageTargetManagerPropio : MonoBehaviour
{
    private bool isDetected = false;
    private Datos datos;
    private DatosPuesta datosPuesta;
    public GameObject contenedor;
    private ObserverBehaviour observerBehaviour;
    public Button miBoton;
    TargetStatus status;
    private float originalScaleY;
    void Start()
    {
        status = new TargetStatus();
        originalScaleY = contenedor.transform.localScale.y;
        if (miBoton != null)
        {
            Debug.Log("El botón no es nulo");
            miBoton.onClick.AddListener(onClickButton);
        }else{
            Debug.Log("El botón es nulo");
        }

        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour)
        {
            // Suscribirse al evento OnTargetStatusChanged
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED || status.Status == Status.LIMITED)
        {
            this.status = status;
            if(!isDetected){
                miBoton.gameObject.SetActive(true);
                isDetected = true;
                if (this.datos == null)
                {
                    Debug.Log("No tengo los datos");
                    ServicioDatosTiempo servicioDatosTiempo = new ServicioDatosTiempo();
                    StartCoroutine(servicioDatosTiempo.ObtenerDatosTiempo((datos) =>
                    {
                        this.datos = datos;

                        if (this.datosPuesta == null)
                        {
                            Debug.Log("No tengo los datos de puesta");
                            StartCoroutine(servicioDatosTiempo.ObtenerDatosPuesta((datosPuesta) =>
                            {
                                this.datosPuesta = datosPuesta;

                                // Mueve aquí los Debug.Log y el cálculo
                                Debug.Log("Datos de la API: " + this.datos.temperatura + " " + this.datos.velocidadViento + " " + this.datos.lluvia);
                                Debug.Log("Datos de la API puesta: " + this.datosPuesta.sunset + " " + this.datosPuesta.sunrise + " " + this.datosPuesta.dusk + " " + this.datosPuesta.dawn);
                                Calcular(this.datos, this.datosPuesta);
                            }));
                        }
                        else
                        {
                            Debug.Log("Ya tengo datos de puesta");
                            Debug.Log("Datos de la API: " + this.datos.temperatura + " " + this.datos.velocidadViento + " " + this.datos.lluvia);
                            Debug.Log("Datos de la API puesta: " + this.datosPuesta.sunset + " " + this.datosPuesta.sunrise + " " + this.datosPuesta.dusk + " " + this.datosPuesta.dawn);
                            Calcular(this.datos, this.datosPuesta);
                        }
                        }));
                    }
                else
                {
                    Debug.Log("Ya tengo los datos");
                    Debug.Log("Datos de la API: " + this.datos.temperatura + " " + this.datos.velocidadViento + " " + this.datos.lluvia);
                    Debug.Log("Datos de la API puesta: " + this.datosPuesta.sunset + " " + this.datosPuesta.sunrise + " " + this.datosPuesta.dusk + " " + this.datosPuesta.dawn);
                    Calcular(this.datos, this.datosPuesta);
                }
            }
            
            
        }
        else if (status.Status == Status.NO_POSE)
        {
            this.status = status;
            miBoton.gameObject.SetActive(false);
            isDetected = false;
        }
    }

    public void OnTargetFound()
    {
        
    }

    public void OnTargetLost()
    {   
        
    }

    private void Calcular(Datos datos, DatosPuesta datosPuesta){
        
        DateTime dusk = DateTime.Parse(datosPuesta.dusk);
        DateTime sunset = DateTime.Parse(datosPuesta.sunset);
        DateTime sunrise = DateTime.Parse(datosPuesta.sunrise);
        DateTime dawn = DateTime.Parse(datosPuesta.dawn);
        DateTime ahora = DateTime.Now;
        // Hora 9:23:22
        //DateTime ahora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 01, 48, 22);
        double probabilityTime = 0.0;
        double k = 0.02;

        int ahoraMinutos = ahora.Hour * 60 + ahora.Minute;
        int sunsetMinutos = sunset.Hour * 60 + sunset.Minute;
        int sunriseMinutos = sunrise.Hour * 60 + sunrise.Minute;
        int dawnMinutos = dawn.Hour * 60 + dawn.Minute;
        int duskMinutos = dusk.Hour * 60 + dusk.Minute;
        
        if (ahoraMinutos < dawnMinutos || ahoraMinutos > duskMinutos)
        {
            probabilityTime = 1.0;
        }
        else if (ahoraMinutos >= dawnMinutos && ahoraMinutos <= sunriseMinutos)
        {
            probabilityTime = 1.0 - ((double)(ahoraMinutos - dawnMinutos) / (sunriseMinutos - dawnMinutos)) * 0.9;
        }
        else if (ahoraMinutos > sunriseMinutos && ahoraMinutos < sunsetMinutos)
        {
            probabilityTime = 0.1;
        }
        else if (ahoraMinutos >= sunsetMinutos && ahoraMinutos <= duskMinutos)
        {
            probabilityTime = 0.1 + ((double)(ahoraMinutos - sunsetMinutos) / (duskMinutos - sunsetMinutos)) * 0.9;
        }
        else
        {
            probabilityTime = 1.0;
        }

        double probabilityTemp = GaussianProbability(datos.temperatura);
        double probabilityViento = VientoProbabilidad(datos.velocidadViento);
        double epocaProbabilidad = EpocaProbabilidad();
        double finalProbability = probabilityTime * probabilityTemp * probabilityViento * epocaProbabilidad;

        Debug.Log("Procentaje hora: " + probabilityTime);
        Debug.Log("Procentaje tempe: " + probabilityTemp);
        Debug.Log("Procentaje viento: " + probabilityViento);
        Debug.Log("Procentaje toal: " + finalProbability);
        Debug.Log("Probabilidad de época: " + epocaProbabilidad);
        //cambiar el tamaño del panel en el eje y dependiendo de la probabilidad final
        contenedor.transform.localScale = new Vector3(contenedor.transform.localScale.x,originalScaleY * (1.0f - (float)finalProbability), contenedor.transform.localScale.z);
        
    }

    // static double Sigmoid(int t, int tStart, int tEnd, double k, bool invert = false)
    // {
    //     double t0 = (tStart + tEnd) / 2.0; // Punto medio
    //     double value = 1.0 / (1.0 + Math.Exp(-k * (t - t0)));
    //     return invert ? 1.0 - value : value;
    // }


    static double TemperatureAdjustment(double T )
    {
        double k = 0.02;
        double minOptimal = 10.0;
        double maxOptimal = 22.0;
        if (T >= minOptimal && T <= maxOptimal)
            return 1.0; 

        if (T < minOptimal)
            return 1.0 - (1.0 / (1.0 + Math.Exp(-k * (T - minOptimal)))); 

        return 1.0 - (1.0 / (1.0 + Math.Exp(-k * (maxOptimal - T)))); 
    }

    static double GaussianProbability(double temperature, double mean = 20, double sigma = 8)
    {
        Debug.Log("Temperatura: " + temperature);
        double exponent = -Math.Pow((temperature - mean) / sigma, 2) / 2;
        return Math.Exp(exponent);
    }

    
    static double VientoProbabilidad(double windSpeed, double center = 30.0, double k = 0.2)
    {
        return 1.0 - (1.0 / (1.0 + Math.Exp(-k * (windSpeed - center))));
    }

    static double EpocaProbabilidad()
{
    DateTime fecha = DateTime.Now;
    int diaDelAño = fecha.DayOfYear;

    int diaPico = 182; 
    double minProb = 0.1;
    double maxProb = 1.0;

    double fase = 2 * Math.PI * (diaDelAño - diaPico) / 365.0;

    double normalizado = (1 + Math.Cos(fase)) / 2;
    double probabilidad = minProb + (maxProb - minProb) * normalizado;

    return probabilidad;
}


    private void Calcular2() {
        //transformar el contenedor en el eje y en un de su tamaño original 0.5f
        float finalProbability = 1.0f - 0.7f;
         contenedor.transform.localScale = new Vector3(contenedor.transform.localScale.x,contenedor.transform.localScale.y * (float)finalProbability, contenedor.transform.localScale.z);
    }

    public void onClickButton()
    {
        
        string sonido = "";
        bool reproducir = false;
        Debug.Log("onClickButton: " + observerBehaviour.TargetName + " " + this.status.Status);
        if(this.status.Status == Status.NO_POSE)
        {
            return;
        }

        if(observerBehaviour.TargetName == "enano")
        {
            sonido = "enano";
            reproducir = true;
        }
        else if(observerBehaviour.TargetName == "borde_claro")
        {
            sonido = "borde-claro";
            reproducir = true;
        }
        else if(observerBehaviour.TargetName == "cabrera")
        {
            sonido = "cabrera";
            reproducir = true;
        }
        else if(observerBehaviour.TargetName == "rabudo")
        {
            sonido = "rabudo";
            reproducir = true;
        }
        else if(observerBehaviour.TargetName == "hortelano")
        {
            sonido = "hortelano";
            reproducir = true;
        }
        
        if(reproducir == true)
        {
            this.reproducirSonido(sonido);
        }
        
        
    }

    // Reproducir un sonido que está en la carpeta Assets/Sonidos
    private void reproducirSonido( string nombreSonido) {
        Debug.Log("Reproduciendo sonido: " + nombreSonido);
        string path = "Sonidos/" + nombreSonido; // Ruta del archivo de sonido
        AudioClip clip = Resources.Load<AudioClip>(path); // Cargar el clip de sonido desde la ruta especificada
        if (clip != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
            Destroy(audioSource, clip.length); // Destruir el AudioSource después de que termine de reproducirse
        }
    }

    void OnDestroy()
    {
        isDetected = false;
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}

