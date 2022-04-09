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
	/// Lógica de interacción para RankingWindow.xaml
	/// </summary>
	public partial class RankingWindow : Window
	{

        private MainWindow v_principal; //Ventana principal
        private string[] nombres = BrainRanking.getNombres(); //Matriz de nombres del ranking
        private int[] puntuaciones = BrainRanking.getPuntuaciones(); //Matriz de puntuaciones

		public RankingWindow(MainWindow v_principal)
		{
			this.InitializeComponent();
            this.v_principal = v_principal;
            lbl_ayuda_rank.Content = "¿Estás aquí?\n!Eres el mejor¡";
            cargarRanking(); //Cargamos XML
		}

        public void cargarRanking()
        {
            //Introducimos cada nombre y puntuación en el label correspondiente
            lbl_rank1.Content = "1st   " + nombres[0].ToUpper();
            lbl_rank2.Content = "2st   " + nombres[1].ToUpper();
            lbl_rank3.Content = "3st   " + nombres[2].ToUpper();
            lbl_rank4.Content = "4st   " + nombres[3].ToUpper();
            lbl_rank5.Content = "5st   " + nombres[4].ToUpper();
            lbl_rank6.Content = "6st   " + nombres[5].ToUpper();
            lbl_rank7.Content = "7st   " + nombres[6].ToUpper();
            lbl_rank8.Content = "8st   " + nombres[7].ToUpper();
            lbl_rank9.Content = "9st   " + nombres[8].ToUpper();
            lbl_rank10.Content = "10st  " + nombres[9].ToUpper();
            lbl_rankP1.Content = " .........    " + puntuaciones[0];
            lbl_rankP2.Content = " .........    " + puntuaciones[1];
            lbl_rankP3.Content = " .........    " + puntuaciones[2];
            lbl_rankP4.Content = " .........    " + puntuaciones[3];
            lbl_rankP5.Content = " .........    " + puntuaciones[4];
            lbl_rankP6.Content = " .........    " + puntuaciones[5];
            lbl_rankP7.Content = " .........    " + puntuaciones[6];
            lbl_rankP8.Content = " .........    " + puntuaciones[7];
            lbl_rankP9.Content = " .........    " + puntuaciones[8];
            lbl_rankP10.Content = " .........    " + puntuaciones[9];
        }

        //Evento para mostrar mensaje cuando quitas el ratón de las arañas
        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            lbl_ayuda_rank.Content = "¿Estás aquí?\n!Eres el mejor¡";
        }

        //Evento para mostrar mensaje cuando pones el ratón en las arañas
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            lbl_ayuda_rank.Content = "!PUAGGG¡\n!ARAÑAS¡";
        }

        //Evento de cierre de ventana para mostrar la ventana de inicio
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            v_principal.Visibility = Visibility.Visible;
        }
	}
}