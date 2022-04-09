using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BrainChild
{
    class BrainPreguntas
    {
        private const string PREGUNTAS_FILE = "BrainPreguntas.xml"; // nombre del archivo         
        public const int MAX_PREGUNTAS = 25; // número máximo de preguntas almacenadas

        //Método que devuelve un array de las preguntas que están en el archivo 
        public static string[] getPreguntas()
        {
            bool cargaOk = true;
            string[] preguntas = new string[MAX_PREGUNTAS];
            XmlDocument xml = new XmlDocument();

            try
            {
                xml.Load(PREGUNTAS_FILE); //cargamos el archivo de preguntas
            }
            catch (XmlException)
            {
                cargaOk = false; //error al cargar - devolver null
                preguntas = null;
            }
            if (cargaOk)
            {
                XmlNodeList nodeList = xml.GetElementsByTagName("preg");
                for (int i = 0; i < preguntas.Length; i++) //Guardamos una a una las preguntas en el array
                {
                    preguntas[i] = nodeList.Item(i)["pregunta"].FirstChild.Value;
                }
            }
            return preguntas;
        }

        //Método que devuelve un array de las respuestas de una pregunta determinada
        public static String[] getRespuestas(int pregunta, String dificultad)
        {
            bool cargaOk = true;
            String[] respuestas = null;
            if (dificultad == "Facil") respuestas = new String[2];
            else respuestas = new String[4];
            XmlDocument xml = new XmlDocument();

            try
            {
                xml.Load(PREGUNTAS_FILE); //cargamos el archivo de preguntas
            }
            catch (XmlException)
            {
                cargaOk = false; //error al cargar - devolver null
                respuestas = null;
            }
            if (cargaOk)
            {
                XmlNodeList nodeList = xml.GetElementsByTagName("preg");
                Random resp = new Random();
                int valor1 = 0, valor2 = 0, valor3 = 0, valor4 = 0;
                if (dificultad == "Facil") //Si es fácil solo devolemos dos respuestas
                {
                    //Devolvemos las respuestas en orden aleatorio
                    valor1 = resp.Next(0, 2); 
                    if (valor1 == 0) valor2 = 1;
                    else valor2 = 0;
                    //Guardamos las respuestas
                    respuestas[valor1] = nodeList.Item(pregunta)["respuesta" + resp.Next(1, 4)].FirstChild.Value;
                    respuestas[valor2] = nodeList.Item(pregunta)["solucion"].FirstChild.Value;
                }
                else //Si es dificíl devolvemos las 4 respuestas
                {
                    //Devolvemos las respuestas en orden aleatorio
                    valor1 = resp.Next(0, 4); 
                    if (valor1 == 0) valor2 = 3;
                    else if (valor1 == 1) valor2 = 0;
                    else if (valor1 == 2) valor2 = 1;
                    else if (valor1 == 3) valor2 = 2;
                    if (valor2 == 0)
                    {
                        valor3 = 3;
                        valor4 = 2;
                    }
                    else if (valor2 == 1)
                    {
                        valor3 = 0;
                        valor4 = 3;
                    }
                    else if (valor2 == 2)
                    {
                        valor3 = 1;
                        valor4 = 0;
                    }
                    else if (valor2 == 3)
                    {
                        valor3 = 2;
                        valor4 = 1;
                    }
                    //Guardamos las respuestas
                    respuestas[valor1] = nodeList.Item(pregunta)["respuesta1"].FirstChild.Value;
                    respuestas[valor2] = nodeList.Item(pregunta)["respuesta2"].FirstChild.Value;
                    respuestas[valor3] = nodeList.Item(pregunta)["respuesta3"].FirstChild.Value;
                    respuestas[valor4] = nodeList.Item(pregunta)["solucion"].FirstChild.Value;
                }
            }

            return respuestas;
        }

        //Método para obtener la solución
        public static String getSolucion(int pregunta)
        {
            bool cargaOk = true;
            String solucion = "";
            XmlDocument xml = new XmlDocument();

            try
            {
                xml.Load(PREGUNTAS_FILE); //cargamos el archivo de preguntas
            }
            catch (XmlException)
            {
                cargaOk = false; //error al cargar - devolver null
                solucion = null;
            }
            if (cargaOk)
            {
                XmlNodeList nodeList = xml.GetElementsByTagName("preg");
                solucion = nodeList.Item(pregunta)["solucion"].FirstChild.Value;
            }

            return solucion;
        }
    }
}
