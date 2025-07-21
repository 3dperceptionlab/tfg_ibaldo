using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Globalization;

public class ServicioDatosTiempo
{

    public IEnumerator ObtenerDatosTiempo(System.Action<Datos> callback)
    {
         string url = "http://www.datos.eltiempoentorrevieja.es/LaMata_Parque/parquenatural.xml";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Datos recibidos: " + request.downloadHandler.text);
                string xmlData = request.downloadHandler.text;
                Debug.Log("Datos recibidos: " + xmlData);
                callback(ProcesarXML(xmlData));
            }
            else
            {
                Debug.LogError("Error en la solicitud: " + request.error);
            }
        }
    }

    private Datos ProcesarXML(string xmlData)
    {
        Datos datos = new Datos();
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ns", "https://www.w3schools.com");

            XmlNode nodoActuales = xmlDoc.SelectSingleNode("//ns:actuales", nsManager);
            if (nodoActuales != null)
            {
                datos.temperatura = ParseDouble(nodoActuales.SelectSingleNode("ns:temperatura", nsManager)?.InnerText);
                datos.velocidadViento = ParseInt(nodoActuales.SelectSingleNode("ns:velocidadviento", nsManager)?.InnerText);
                datos.lluvia = ParseDouble(nodoActuales.SelectSingleNode("ns:lluvia", nsManager)?.InnerText);

            }
            else
            {
                Debug.LogError("No se encontró el nodo <actuales>");
            }
        }
        catch (XmlException ex)
        {
            Debug.LogError("Error al analizar el XML: " + ex.Message);
        }

        return datos;
    }

    private double ParseDouble(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Debug.LogError("Error: La cadena de entrada está vacía o es nula.");
            return 0.0;
        }

        if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
        {
            Debug.Log("Parseado con éxito: " + result);
            return result;
        }
        else
        {
            Debug.LogError("Error al parsear double: " + input);
            return 0.0;
        }
    }

    private int ParseInt(string input)
    {
        if (int.TryParse(input, out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError("Error al parsear int: " + input);
            return 0;
        }
    }

    public IEnumerator ObtenerDatosPuesta(System.Action<DatosPuesta> callback)
    {
        Debug.LogError("Obteniendo datos de puesta de sol");
        DatosPuesta datos = new DatosPuesta();
        string url = "https://api.sunrisesunset.io/json?lat=38.02521461967607&lng=-0.6583201363525588";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            Debug.LogError("Paso......");
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonData = request.downloadHandler.text;
                JObject jObject = JObject.Parse(jsonData);
                datos.sunrise = jObject["results"]["sunrise"].ToString();
                datos.sunset = jObject["results"]["sunset"].ToString();
                datos.dawn = jObject["results"]["dawn"].ToString();
                datos.dusk = jObject["results"]["dusk"].ToString();
                Debug.Log("Datos recibidos: " + jsonData);
                callback(datos);
            }
            else
            {
                Debug.LogError("Error en la solicitud: " + request.error);
            }
        }
    }
}
