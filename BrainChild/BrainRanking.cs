using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BrainChild
{
    class BrainRanking
    {
        private const string RANKING_FILE = "BrainRanking.xml"; // nombre del archivo de ranking        
        public const int MAX_RESULTS = 10; // número máximo de records a almacenar

        //Método que devuelve una array de los nombres que están en el ranking ordenados por puntuación
        public static string[] getNombres()
        {
            bool cargaOk = true;
            string[] nombres = new string[MAX_RESULTS];
            XmlDocument xml = new XmlDocument();

            try
            {
                xml.Load(RANKING_FILE); //cargamos el archivo de rankings
            }
            catch (XmlException)
            {
                cargaOk = false; //error al cargar - devolver null
                nombres = null;
            }
            if (cargaOk)
            {
                XmlNodeList nodeList = xml.GetElementsByTagName("rank");
                for (int i = 0; i < nombres.Length; i++)
                {
                    nombres[i] = nodeList.Item(i)["nombre"].FirstChild.Value;
                }
            }
            return nombres;
        }

        //Método que devuelve un array con las puntuaciones del ranking ordenadas por puntuaciones
        public static int[] getPuntuaciones()
        {
            bool cargaOk = true;
            int[] puntuaciones = new int[MAX_RESULTS];
            XmlDocument xml = new XmlDocument();

            try
            {
                xml.Load(RANKING_FILE); //cargamos el archivo de rankings
            }
            catch (XmlException)
            {
                cargaOk = false; //error al cargar - devolver null
                puntuaciones = null;
            }
            if (cargaOk)
            {
                XmlNodeList nodeList = xml.GetElementsByTagName("rank");
                for (int i = 0; i < puntuaciones.Length; i++)
                {
                    puntuaciones[i] = int.Parse(nodeList.Item(i)["puntos"].FirstChild.Value);
                }
            }

            return puntuaciones;
        }

        //Indica si una puntuación estaría dentro del ranking. Si es mayor que la menor puntuación del ranking
        //devuelve TRUE para que la puntuación entre en el ranking, FALSE en caso contrario
        public static bool estaPuntuacionEnRanking(int puntuacion)
        {
            bool puntuacionEnRanking = true;
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(RANKING_FILE);
            }
            catch (XmlException)
            {
                puntuacionEnRanking = false;
            }

            if (puntuacionEnRanking) //si no hemos pasado por el catch...
            {
                XmlNode nodo = xml.SelectSingleNode("//rank[@pos='" + MAX_RESULTS + "']");
                int puntuacionMenor = int.Parse(nodo["puntos"].FirstChild.Value);

                if (puntuacion < puntuacionMenor)
                {
                    puntuacionEnRanking = false;
                }
            }
            Console.WriteLine(puntuacionEnRanking);
            return puntuacionEnRanking;
        }

        //Indica la posición que debería ocupar una puntuación en el ranking
        //devolviendo la posición en la que debería insertarse la puntuación
        private static int determinarPosicion(int puntuacion)
        {
            int posicion = -1;
            bool tope = false;
            if (estaPuntuacionEnRanking(puntuacion))
            {
                int[] puntuaciones = getPuntuaciones();
                for (posicion = 0; posicion < puntuaciones.Length && !tope; posicion++)
                {
                    if (puntuaciones[posicion] <= puntuacion)
                    {
                        tope = true; //Hemos encontrado la posicion
                    }
                }
            }
            return posicion;
        }

        //Inserta una puntuación en el ranking
        public static void insertarPuntuacion(string nombre, int puntuacion)
        {
            bool cargaOk = true;
            int posicion = determinarPosicion(puntuacion);
            if (posicion != -1)
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(RANKING_FILE);
                }
                catch (XmlException)
                {
                    cargaOk = false;
                }
                if (cargaOk)
                {
                    XmlNodeList nodeList = xml.GetElementsByTagName("rank");

                    //Movemos hacia abajo las posiciones
                    for (int i = MAX_RESULTS - 1; i >= posicion; i--)
                    {
                        nodeList.Item(i)["nombre"].InnerText = nodeList.Item(i - 1)["nombre"].InnerText;
                        nodeList.Item(i)["puntos"].InnerText = nodeList.Item(i - 1)["puntos"].InnerText;
                    }
                    //Insertamos los valores en la posicion
                    nodeList.Item(posicion - 1)["nombre"].InnerText = nombre;
                    nodeList.Item(posicion - 1)["puntos"].InnerText = puntuacion.ToString();
                    xml.Save(RANKING_FILE);
                }
            }
        }

        //Indica si hay fichero de ranking disponible
        public static bool existeRanking()
        {
            bool cargaOk = true;
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(RANKING_FILE);
            }
            catch (Exception)
            {
                cargaOk = false;
            }
            return cargaOk;
        }

    }
}
