using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrainChild
{
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

        private static String nombre_perfil = ""; //Nombre del usuario
        private int puntos=0; //Puntos pertenecientes al usuario
        private static String dificultad = ""; //Dificultad seleccionada
        private GamesWindow v_juegos; //Ventana de juegos
        private RankingWindow v_ranking; //Ventana de ranking

		public MainWindow()
		{
			this.InitializeComponent();
            //Mostramos los puntos en la ventana
            BitmapImage bi1 = new BitmapImage();
            bi1.BeginInit();
            bi1.UriSource = new Uri(getNumero(puntos), UriKind.Relative);
            bi1.EndInit();
            this.img_puntos3.Source = bi1;
		}

        //Método que devuelve la ruta de la imagen equivalente a un número determinado
        public String getNumero(int n)
        {
            String ruta = "";
            switch (n)
            {
                case 0:
                    ruta = "Imagenes/0.png";
                    break;
                case 1:
                    ruta = "Imagenes/1.png";
                    break;
                case 2:
                    ruta = "Imagenes/2.png";
                    break;
                case 3:
                    ruta = "Imagenes/3.png";
                    break;
                case 4:
                    ruta = "Imagenes/4.png";
                    break;
                case 5:
                    ruta = "Imagenes/5.png";
                    break;
                case 6:
                    ruta = "Imagenes/6.png";
                    break;
                case 7:
                    ruta = "Imagenes/7.png";
                    break;
                case 8:
                    ruta = "Imagenes/8.png";
                    break;
                case 9:
                    ruta = "Imagenes/9.png";
                    break;
            }
            return ruta;
        }

        //Método que actualiza los puntos en la ventana conforme se van consiguiendo en los juegos
        public void setPuntos(int p)
        {
            puntos = p; //Actualizamos la variable de puntos
            this.img_puntos1.Source = null;
            this.img_puntos2.Source = null;
            //Actualizamos la puntuación segun las centenas, decenas y unidades
            if (puntos > 99) //Si los puntos tiene 3 digitos introducimos 3 numeros
            {
                BitmapImage bi1 = new BitmapImage();
                bi1.BeginInit();
                bi1.UriSource = new Uri(getNumero(puntos / 100), UriKind.Relative); //Centenas
                bi1.EndInit();
                this.img_puntos1.Source = bi1;
                BitmapImage bi2 = new BitmapImage();
                bi2.BeginInit();
                bi2.UriSource = new Uri(getNumero((puntos - (100 * (puntos / 100)))/10), UriKind.Relative);//Decenas
                bi2.EndInit();
                this.img_puntos2.Source = bi2;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(getNumero(puntos - (100*(puntos/100)) - (10 * ((puntos - (100 * (puntos / 100))) / 10))), UriKind.Relative); //Unidades
                bi3.EndInit();
                this.img_puntos3.Source = bi3;
            }
            else if (puntos > 9) //Si los puntos tiene 2 digitos introducimos 2 numeros
            {
                BitmapImage bi1 = new BitmapImage();
                bi1.BeginInit();
                bi1.UriSource = new Uri(getNumero(puntos / 10), UriKind.Relative); //Decenas
                bi1.EndInit();
                this.img_puntos2.Source = bi1;
                BitmapImage bi2 = new BitmapImage();
                bi2.BeginInit();
                bi2.UriSource = new Uri(getNumero(puntos - (10 * (puntos / 10))), UriKind.Relative); //Unidades
                bi2.EndInit();
                this.img_puntos3.Source = bi2;
            }
            else //Si los puntos tiene 1 digitos introducimos 1 numeros
            {
                BitmapImage bi1 = new BitmapImage();
                bi1.BeginInit();
                bi1.UriSource = new Uri(getNumero(puntos), UriKind.Relative); //Unidades
                bi1.EndInit();
                this.img_puntos3.Source = bi1;
            }
        }

        //Evento del boton de calculo para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void btn_calculo_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility= Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento del boton de calculo para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void btn_calculo_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = "Sumar, \nrestar, \nmultiplicar...";
        }

        //Evento de la imagen del Buho para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void image1_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento de la imagen del Buho para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void image1_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = "Soy Buhito \n y te ayudo";
        }

        //Evento del boton de todos los juegos para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void btn_todos_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = "Juegos de \n todo tipo";
        }

        //Evento del boton de ctodos los juegos para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void btn_todos_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento del boton de memoria para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void btn_memoria_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento del boton de memoria para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void btn_memoria_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = "Juegos de \n memorizar";
        }

        //Evento del boton de ciencia para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void btn_ciencia_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = "Preguntas de \n cultura general";
        }

        //Evento del boton de ciencia para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void btn_ciencia_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento de cambio del textBox donde introducimos el nombre del usuario
        private void txtBox_nombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            label1.Content = "Hola " + txtBox_nombre.Text; //Ponemos el nombre en la pizarra
            nombre_perfil = txtBox_nombre.Text; //Guardamos el nombre
            if (txtBox_nombre.Text == "") //Si el textBox se queda vacia iniciamos los puntos
            {
                img_info.Visibility = Visibility.Visible;
                lbl_info.Content = "Vuelves a \n empezar";
                setPuntos(0);
            }
        }

        //Evento del label del nombre para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = label1.Content;
        }

        //Evento del label del nombre para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void label1_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento de la imagen de los puntos (centenas) para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void img_puntos1_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento de la imagen de los puntos (centenas) para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void img_puntos1_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = puntos + " puntos";
        }

        //Evento de la imagen de los puntos (decenas) para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void img_puntos2_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento de la imagen de los puntos (decenas) para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void img_puntos2_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = puntos + " puntos";
        }

        //Evento de la imagen de los puntos (unidades) para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void img_puntos3_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento de la imagen de los puntos (unidades) para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void img_puntos3_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = puntos + " puntos";
        }

        //Evento del boton de ranking para quitar el mensaje de ayuda cuando se quite el ratón de encima
        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Collapsed;
            lbl_info.Content = "";
        }

        //Evento del boton de ranking para mostrar el mensaje de ayuda cuando se pase el ratón por encima
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            img_info.Visibility = Visibility.Visible;
            lbl_info.Content = "Mira tus \nrecords";
        }

        //Evento del boton de calculo para ir a la ventana de juegos
        private void btn_calculo_Click(object sender, RoutedEventArgs e)
        {
            if(nombre_perfil.Equals("")) { //Si no se ha introducido un nombre lo indicamos
                img_info.Visibility = Visibility.Visible;
                lbl_info.Content = "¿Cuál es \n tu nombre?";
            }
            else {
                //Invocamos la ventana juegos indicando la configuración de calculo
                if (rbtn_facil.IsChecked==true) dificultad = "Facil";
                else dificultad = "Dificil";
                v_juegos = new GamesWindow(this, nombre_perfil, puntos, dificultad, "Calculo");
                v_juegos.Show();
                this.Visibility = Visibility.Hidden;
                img_info.Visibility = Visibility.Collapsed;
                lbl_info.Content = "";
            }
        }

        //Evento del boton de todos los juegos para ir a la ventana de juegos
        private void btn_todos_Click(object sender, RoutedEventArgs e)
        {
            if (nombre_perfil.Equals("")) //Si no se ha introducido un nombre lo indicamos
            {
                img_info.Visibility = Visibility.Visible;
                lbl_info.Content = "¿Cuál es \n tu nombre?";
            }
            else
            {
                //Invocamos la ventana juegos indicando la configuración de todos los juegos
                if (rbtn_facil.IsChecked == true) dificultad = "Facil";
                else dificultad = "Dificil";
                v_juegos = new GamesWindow(this, nombre_perfil, puntos, dificultad, "Todos");
                v_juegos.Show();
                this.Visibility = Visibility.Hidden;
                img_info.Visibility = Visibility.Collapsed;
                lbl_info.Content = "";
            }
        }

        //Evento del boton de memoria para ir a la ventana de juegos
        private void btn_memoria_Click(object sender, RoutedEventArgs e)
        {
            if (nombre_perfil.Equals("")) //Si no se ha introducido un nombre lo indicamos
            {
                img_info.Visibility = Visibility.Visible;
                lbl_info.Content = "¿Cuál es \n tu nombre?";
            }
            else
            {
                //Invocamos la ventana juegos indicando la configuración de memoria
                if (rbtn_facil.IsChecked == true) dificultad = "Facil";
                else dificultad = "Dificil";
                v_juegos = new GamesWindow(this, nombre_perfil, puntos, dificultad, "Memoria");
                v_juegos.Show();
                this.Visibility = Visibility.Hidden;
                img_info.Visibility = Visibility.Collapsed;
                lbl_info.Content = "";
            }
        }

        //Evento del boton de ciencia para ir a la ventana de juegos
        private void btn_ciencia_Click(object sender, RoutedEventArgs e)
        {
            if (nombre_perfil.Equals("")) //Si no se ha introducido un nombre lo indicamos
            {
                img_info.Visibility = Visibility.Visible;
                lbl_info.Content = "¿Cuál es \n tu nombre?";
            }
            else
            {
                //Invocamos la ventana juegos indicando la configuración de ciencia
                if (rbtn_facil.IsChecked == true) dificultad = "Facil";
                else dificultad = "Dificil";
                v_juegos = new GamesWindow(this, nombre_perfil, puntos, dificultad, "Ciencia");
                v_juegos.Show();
                this.Visibility = Visibility.Hidden;
                img_info.Visibility = Visibility.Collapsed;
                lbl_info.Content = "";
            }
        }

        //Evento del boton de ranking para ir a la ventana de ranking
        private void btn_ranking_Click(object sender, RoutedEventArgs e)
        {
            v_ranking = new RankingWindow(this);
            v_ranking.Show();
            this.Visibility = Visibility.Hidden;
        }
    }
}