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
using Timer = System.Windows.Threading.DispatcherTimer;
using System.Threading;
using System.Windows.Threading;

namespace BrainChild
{
	/// <summary>
	/// </summary>
	public partial class GamesWindow : Window
	{
        //VARIABLES GENERALES
        MainWindow v_principal; //Ventana principal o de inicio
        private String perfil;  //Nombre del usuario que se ha introducido en la venanta inicio
        private int puntos; //Puntos del usuario
        private String dificultad=""; //Dificultad del juego
        private String juego; //Tipo de juego (calculo, ciencia, memoria o todos)
        private Timer tiempo; //Temporizador para el fin del juego
        private Timer tiempo_memorizar; //Temporizador para memorizar las imagenes y quitar mensajes de ayuda en otros juegos

        //VARIABLES PARA EL JUEGO DE CALCULO
        private int cont_numeros = -1; //Control de errores del usuario al introducir los resultados de las operaciones
        private int resultado = 0; //Resultado de la opración para poder comprobarla

        //VARIABLES PARA EL JUEGO DE CIANCIA
        private int n_pregunta = 0; //Variable para guardar la pregunta elegida en cada ronda
        private int limite_preguntas = 25; //Número límite de preguntas guardadas

        //VARIABLES PARA EL JUEGO DE MEMORIA
        private int[] numeros_memoria = new int[8]; //Matriz para saber donde están situadas las imagenes
        private int tipo_memoria = 0; //Variable para saber si las imagenes son números o dibujos
        private int solucion_memoria = 0; //Variable para guardar la imagen correcta y poder comprobarla
        private int limite_tiempo_mem = 5; //Tiempo limite para memorizar

        //VARIABLES PARA EL JUEGO DE TODOS
        private String juego_actual = ""; //Para saber en que juego estamos

		public GamesWindow(MainWindow v_principal, String perfil, int puntos, String dificultad, String juego)
		{
			this.InitializeComponent();
            //Inicializamos variables de configuración
            this.v_principal = v_principal;
            this.perfil = perfil;
            this.puntos = puntos;
            this.dificultad = dificultad;
            this.juego = juego;
            //Inicializamos temporizador de memoria y otros 
            tiempo_memorizar = new Timer(); 
            tiempo_memorizar.Tick += new EventHandler(TickReloj_memorizar);
            tiempo_memorizar.Interval = TimeSpan.FromSeconds(1);
            if (juego == "Calculo") //Si el juego es de calculo habilitamos los objetos necesarios
            {
                this.Title = "Cálculo";
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Imagenes/igual.png", UriKind.Relative);
                bi.EndInit();
                this.img_igual.Source = bi;
                lbl_ayuda.Content="¿Cuánto es?";
                introducirPuntos(); //Introducimos en la ventana los puntos que tenía el usuario
                lbl_nombre.Content = "Hola " + perfil;
                img_tick.Visibility = Visibility.Visible;
                btn_comprobar.Visibility = Visibility.Visible;
                txtBox_res.Visibility = Visibility.Visible;
                empezarJuegoCalculo(); //Empezamos el juego de calculo
            }
            else if (juego == "Ciencia") //Si el juego es de ciencia habilitamos los objetos necesarios
            {
                this.Title = "Ciencia";
                lbl_ayuda.Content = "¿Cuál es la\n respuesta?";
                introducirPuntos(); //Introducimos en la ventana los puntos que tenía el usuario
                lbl_nombre.Content = "Hola " + perfil;
                img_tick.Visibility = Visibility.Visible;
                btn_comprobar.Visibility = Visibility.Visible;
                rbtn_resp1.Visibility = Visibility.Visible;
                rbtn_resp2.Visibility = Visibility.Visible;
                if (dificultad == "Dificil")
                {
                    rbtn_resp3.Visibility = Visibility.Visible;
                    rbtn_resp4.Visibility = Visibility.Visible;
                }
                empezarJuegoCiencia(); //Empezamos el juego de ciencia
            }
            else if (juego == "Memoria") //Si el juego es de memoria habilitamos los objetos necesarios
            {
                this.Title = "Memoria";
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                progressBar_tiempo_mem.Maximum = limite_tiempo_mem;
                introducirTiempo(limite_tiempo_mem, 1);
                introducirPuntos(); //Introducimos en la ventana los puntos que tenía el usuario
                lbl_nombre.Content = "Hola " + perfil;
                btn_mem1.Visibility = Visibility.Visible;
                img_mem1.Visibility = Visibility.Visible;
                btn_mem2.Visibility = Visibility.Visible;
                img_mem2.Visibility = Visibility.Visible;
                btn_mem3.Visibility = Visibility.Visible;
                img_mem3.Visibility = Visibility.Visible;
                btn_mem4.Visibility = Visibility.Visible;
                img_mem4.Visibility = Visibility.Visible;
                btn_mem5.Visibility = Visibility.Visible;
                img_mem5.Visibility = Visibility.Visible;
                btn_mem6.Visibility = Visibility.Visible;
                img_mem6.Visibility = Visibility.Visible;
                progressBar_tiempo_mem.Visibility = Visibility.Visible;
                if (dificultad == "Dificil")
                {
                    btn_mem7.Visibility = Visibility.Visible;
                    img_mem7.Visibility = Visibility.Visible;
                    btn_mem8.Visibility = Visibility.Visible;
                    img_mem8.Visibility = Visibility.Visible;
                }
                empezarJuegoMemoria(); //Empezamos el juego de memoria
            }
            else if (juego == "Todos") //Si el juego es de todos ...
            {
                this.Title = "Todos";
                lbl_nombre.Content = "Hola " + perfil;
                introducirPuntos(); //Introducimos en la ventana los puntos que tenía el usuario
                empezarJuegoTodos(); //Empezamos el juego de todo
            }
            //Iniciaizamos el temporizador general que indica el fin de la partida
            tiempo = new Timer();
            tiempo.Tick += new EventHandler(TickReloj);
            tiempo.Interval = TimeSpan.FromSeconds(1);
            tiempo.Start();
		}


        //METODOS GENERALES

        //Evento del temporizador general para controlar el final de la partida
        private void TickReloj(object o, EventArgs e) 
        {
            progressBar_tiempo.Value++; //Incremetamos la barra de progreso
            introducirTiempo((int)(60 - progressBar_tiempo.Value), 0);
            if (progressBar_tiempo.Value == 60) //Si llegamos al limite de tiempo
            {
                //Deshabilitamos botones para que el jugador deje de jugar
                btn_comprobar.IsEnabled = false;
                if (juego == "Memoria" || (juego=="Todos" && juego_actual=="Memoria"))
                {
                    btn_mem1.IsEnabled = false;
                    btn_mem2.IsEnabled = false;
                    btn_mem3.IsEnabled = false;
                    btn_mem4.IsEnabled = false;
                    btn_mem5.IsEnabled = false;
                    btn_mem6.IsEnabled = false;
                    btn_mem7.IsEnabled = false;
                    btn_mem8.IsEnabled = false;
                    tiempo_memorizar.Stop();
                }
                tiempo.Stop();
                img_ayuda_profe.Visibility = Visibility.Visible;
                v_principal.setPuntos(puntos); //actualizamos puntos en ventana principal
                lbl_ayuda.Content = puntos + " puntos";
                lbl_profe.Content = "FIN DEL JUEGO";
                actualizarRanking(); //Actualizmos ranking al final de la partida
            }
            else if(progressBar_tiempo.Value == 30) { //Mensajes de ayuda a mitad de partida
                lbl_profe.Content="!Vamos¡ Vas\nmuy bien";
                img_ayuda_profe.Visibility=Visibility.Visible;
            }
            else if (progressBar_tiempo.Value == 35) //Quitamos mensajes de ayuda a mitad de partida
            {
                lbl_profe.Content = "";
                img_ayuda_profe.Visibility = Visibility.Collapsed;
            }
            else if (progressBar_tiempo.Value == 50) //Mensajes de ayuda al final de partida
            {
                lbl_profe.Content = "!Rápido,\n rápido!";
                img_ayuda_profe.Visibility = Visibility.Visible;
            }
            else if (progressBar_tiempo.Value == 55) //Quitamos mensajes de ayuda a final de partida
            {
                lbl_profe.Content = "";
                img_ayuda_profe.Visibility = Visibility.Collapsed;
            }
        }

        //Evento del temporizador de memoria
        private void TickReloj_memorizar(object o, EventArgs e)
        {
            if (juego == "Ciencia" || juego == "Calculo" || (juego=="Todos" && (juego_actual=="Ciencia" || juego_actual=="Calculo")))
            { //Desactivamos mensajes de ayuda en cualquier juego
                img_bocadilloSolu.Visibility = Visibility.Collapsed;
                this.img_memSol.Source = null;
                tiempo_memorizar.Stop();
            }
            else if(juego=="Memoria" || (juego=="Todos" && (juego_actual=="Memoria"))) 
            { //Solo para el juego de memoria, controlamos el tiempo para memorizar
                progressBar_tiempo_mem.Value++; //Incrementamos barra de progreso de memorizar
                introducirTiempo((int)(limite_tiempo_mem - progressBar_tiempo_mem.Value), 1);
                if (progressBar_tiempo_mem.Value == limite_tiempo_mem) //Si hemos llegado al limite de tiempo para memorizar
                {
                    solucion_memoria = 0;
                    tiempo_memorizar.Stop();
                    lbl_ayuda.Content = "¿Donde estaba\nla imagen?";
                    img_bocadilloSolu.Visibility = Visibility.Visible;
                    //Aleatoriamente preguntamos por una imagen y las hacemos desaparecer
                    Random sol = new Random();
                    //Controlamos que preguntamos por una imagen que este en la ventana
                    while (solucion_memoria == 0) solucion_memoria = numeros_memoria[sol.Next(0, 8)];
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    if (dificultad == "Facil") //Si es fácil solo mostramos dibujos
                        bi.UriSource = new Uri("Imagenes/memoria" + solucion_memoria + ".png", UriKind.Relative);
                    else
                    { //Si es difícil mostramos dibujos y números
                        if (tipo_memoria == 0) bi.UriSource = new Uri("Imagenes/memoria" + solucion_memoria + ".png", UriKind.Relative);
                        else bi.UriSource = new Uri("Imagenes/" + solucion_memoria + ".png", UriKind.Relative);
                    }
                    bi.EndInit();
                    this.img_memSol.Source = bi;
                    if (dificultad == "Facil") //Si es fácil solo quitamos un máximo de 3 imagenes
                    {
                        vaciarBotonMemoria(solucion_memoria);
                        vaciarBotonMemoria(sol.Next(0, 7));
                        vaciarBotonMemoria(sol.Next(0, 7));
                    }
                    else //Si es difícil solo quitamos un máximo de 4 imagenes
                    {
                        vaciarBotonMemoria(solucion_memoria);
                        vaciarBotonMemoria(sol.Next(0, 9));
                        vaciarBotonMemoria(sol.Next(0, 9));
                        vaciarBotonMemoria(sol.Next(0, 9));
                    }
                }
                else if (progressBar_tiempo_mem.Value == 2) //Deshabilitamos mensajes de ayuda
                {
                    img_bocadilloSolu.Visibility = Visibility.Collapsed;
                    this.img_memSol.Source = null;
                }
            }
        }

        //Método que actualiza los puntos en la ventana conforme se van consiguiendo
        public void introducirPuntos()
        {
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
                bi2.UriSource = new Uri(getNumero((puntos - (100 * (puntos / 100))) / 10), UriKind.Relative); //Decenas
                bi2.EndInit();
                this.img_puntos2.Source = bi2;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(getNumero(puntos - (100 * (puntos / 100)) - (10 * ((puntos - (100 * (puntos / 100))) / 10))), UriKind.Relative); //Unidades
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

        //Método que actualiza el tiempo en la ventana
        public void introducirTiempo(int tiempo, int tipo)
        {
            if (tiempo > 9) //Si los puntos tiene 2 digitos introducimos 2 numeros
            {
                BitmapImage bi1 = new BitmapImage();
                bi1.BeginInit();
                bi1.UriSource = new Uri(getNumero(tiempo / 10), UriKind.Relative); //Decenas
                bi1.EndInit();
                this.img_tiempo1.Source = bi1;
                BitmapImage bi2 = new BitmapImage();
                bi2.BeginInit();
                bi2.UriSource = new Uri(getNumero(tiempo - (10 * (tiempo / 10))), UriKind.Relative); //Unidades
                bi2.EndInit();
                this.img_tiempo2.Source = bi2;
            }
            else //Si los puntos tiene 1 digitos introducimos 1 numeros
            {
                BitmapImage bi1 = new BitmapImage();
                bi1.BeginInit();
                bi1.UriSource = new Uri(getNumero(tiempo), UriKind.Relative); //Unidades
                bi1.EndInit();
                if (tipo == 0) //Si es el tiempo general
                {
                    this.img_tiempo1.Source = null;
                    this.img_tiempo2.Source = bi1;
                } //Si es el tiempo de memorizar
                else this.img_tiempo_mem.Source = bi1;
            }
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

        //Método para actualizar ranking
        public void actualizarRanking()
        {
            if (BrainRanking.existeRanking()) //Si conectamos con el XML
            {
                if (BrainRanking.estaPuntuacionEnRanking(puntos)) //Si entramos en el ranking
                {
                    lbl_ayuda.Content = "Nuevo record\n" + puntos + " puntos";
                    BrainRanking.insertarPuntuacion(perfil, puntos); //Introducimos puntuación
                }
            }
        }

        //Evento del boton comprobar para los juegos de cáclulo y ciencia
        private void btn_comprobar_Click(object sender, RoutedEventArgs e)
        {
            if (juego == "Calculo") comprobarCalculo();
            else if (juego == "Ciencia") comprobarCiencia();
            else if (juego == "Todos")
            {
                if (juego_actual == "Calculo") comprobarCalculo();
                else if (juego_actual == "Ciencia") comprobarCiencia();
            }
        }

        //Evento de cierre de ventana para mostrar de nuevo la ventana principal
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            v_principal.Visibility = Visibility.Visible;
        }



        //MÉTODOS PARA EL JUEGO DE CÁLCULO

        //Método que devuelve la ruta de la imagen equivalente a una operacion (+, -, *)
        public String getOp(int op)
        {
            String ruta = "";
            switch (op)
            {
                case 0:
                    ruta = "Imagenes/suma.png";
                    break;
                case 1:
                    ruta = "Imagenes/resta.png";
                    break;
                case 2:
                    ruta = "Imagenes/multiplicar.png";
                    break;
            }
            return ruta;
        }

        //Método muestra en pantalla los operandos de la operación 
        public void cargarNumeros(int n1, int n2)
        {
            int n11, n12, n21, n22;
            if (n1 < 10) //Si el primer operando tiene un dígito
            {
                n11 = 0; //Primer dígito un 0
                n12 = n1; //Segundo dígito el valor
            }
            else //Si el primer operando tiene dos dígito
            {
                n11 = n1 / 10; //Primer dígito las decenas
                n12 = n1 - (10 * n11); //Segundo dígito las unidades
            }
            if (n2 < 10) //Si el segundo operando tiene un dígito
            {
                n21 = 0;
                n22 = n2;
            }
            else //Si el segundo operando tiene dos dígito
            {
                n21 = n2 / 10;
                n22 = n2 - (10 * n21);
            }
            //Cargamos las imagenes correspondientes a los números 
            if (n11 != 0) 
            {
                BitmapImage bi1 = new BitmapImage();
                bi1.BeginInit();
                bi1.UriSource = new Uri(getNumero(n11), UriKind.Relative);
                bi1.EndInit();
                this.img_n11.Source = bi1;
            }
            BitmapImage bi2 = new BitmapImage();
            bi2.BeginInit();
            bi2.UriSource = new Uri(getNumero(n12), UriKind.Relative);
            bi2.EndInit();
            this.img_n12.Source = bi2;
            if (n21 != 0)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(getNumero(n21), UriKind.Relative);
                bi3.EndInit();
                this.img_n21.Source = bi3;
            }
            BitmapImage bi4 = new BitmapImage();
            bi4.BeginInit();
            bi4.UriSource = new Uri(getNumero(n22), UriKind.Relative);
            bi4.EndInit();
            this.img_n22.Source = bi4;
        }

        //Evento de cambio del textBox donde introducimos el resultado de las operaciones
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((txtBox_res.Text.Length == 1 || txtBox_res.Text.Length == 2) && Char.IsNumber(Convert.ToChar(txtBox_res.Text.ToCharArray()[txtBox_res.Text.ToCharArray().Length - 1])))
            { //Controlamos que no se introduzcan letras
                lbl_ayuda.Content = "¿Cuanto es?";
                if (txtBox_res.GetLineLength(0) == 1) //Cogemos uno a uno los digitos para mostrar las imagenes
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(getNumero(Convert.ToInt32(txtBox_res.Text)), UriKind.Relative);
                    bi.EndInit();
                    this.img_r1.Source = bi;
                    this.img_r2.Source = null;
                    cont_numeros = Convert.ToInt32(txtBox_res.Text);
                }
                else if (txtBox_res.GetLineLength(0) == 2) //Cogemos uno a uno los digitos para mostrar las imagenes
                {
                    if (cont_numeros >= 0)
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(getNumero(Convert.ToInt32(txtBox_res.Text) - (10 * cont_numeros)), UriKind.Relative);
                        bi.EndInit();
                        this.img_r2.Source = bi;
                        cont_numeros = -1;
                    }
                    else
                    {
                        this.img_r2.Source = null;
                    }
                }
                else if (txtBox_res.GetLineLength(0) == 0) //Si está vacio quitamos imagenes de los números
                {
                    this.img_r2.Source = null;
                    this.img_r1.Source = null;
                }
            }
            else
            { //Si hemos introducido algo distinto a un número lo indicamos
                txtBox_res.Text = "";
                this.img_r2.Source = null;
                this.img_r1.Source = null;
                lbl_ayuda.Content = "Solo números";
            }
        }

        //Método para comprobar la solución introducida por el usuario
        public void comprobarCalculo()
        {
            if (txtBox_res.Text != "")
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (Convert.ToInt16(txtBox_res.Text) == resultado) //Comprobamos resultados
                {
                    txtBox_res.Text = "";
                    this.img_r1.Source = null;
                    this.img_r2.Source = null;
                    cont_numeros = -1;
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 3;
                    else puntos += 6;
                }
                else
                {
                    txtBox_res.Text = "";
                    this.img_r1.Source = null;
                    this.img_r2.Source = null;
                    cont_numeros = -1;
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 2;
                    else puntos -= 3;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                tiempo_memorizar.Start();
                img_bocadilloSolu.Visibility = Visibility.Visible;
                introducirPuntos();
                this.img_n11.Source = null;
                this.img_n12.Source = null;
                this.img_n21.Source = null;
                this.img_n22.Source = null;
                lbl_ayuda.Content = "¿Cuanto es?";
                if (juego == "Calculo") empezarJuegoCalculo(); //Reiniciamos ronda
                else if (juego == "Todos") //Si es el juego de todos reiniciamos ronda
                {
                    reiniciarTablero("Calculo"); //Reiniciamos los objetos usados en el juego de cálculo
                    empezarJuegoTodos();
                }
            }
            else lbl_ayuda.Content = "No hay \n números";
        }

        //Método para configurar el juego de cálculo
        public void empezarJuegoCalculo()
        {
            Random op_random = new Random(); //Generamos de forma aleatoria la operación
            int op = 0;
            if (dificultad.Equals("Facil")) //Si es fácil solo usamos sumas y restas
                op = op_random.Next(0, 2);
            else //Si es difícil utilizamos también multiplicaciones
                op = op_random.Next(0, 3);
            Random n_random = new Random();
            int n1 = 0, n2 = 0;
            if (dificultad.Equals("Facil")) //Si es facil no generamos valores mayores de 10
            {

                n1 = op_random.Next(0, 10);
                n2 = op_random.Next(0, 10);
                if (op == 1 && n1 < n2)
                {
                    int aux = n1;
                    n1 = n2;
                    n2 = aux;
                }
            }
            else if (dificultad.Equals("Dificil"))
            {
                if (op == 2) //Si es multiplicación solo valores menores que 10
                {
                    n1 = op_random.Next(0, 9);
                    n2 = op_random.Next(0, 9);
                }
                else //Sino, valores hasta 20
                {
                    n1 = op_random.Next(0, 20);
                    n2 = op_random.Next(0, 20);
                }
                if (op == 1 && n1 < n2) //Para que no salga valores negativos en las restas
                {
                    int aux = n1;
                    n1 = n2;
                    n2 = aux;
                }
            }
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(getOp(op), UriKind.Relative); //Cargamos los operandos
            bi.EndInit();
            this.img_op.Source = bi;
            cargarNumeros(n1, n2); //Cargamos los numeros de la operacion
            //Guardamos los resultados de la operación para poder comprobarlo
            if (op == 0) resultado = n1 + n2; 
            else if (op == 1) resultado = n1 - n2;
            else resultado = n1 * n2;
        }

        
        
        //MÉTODOS PARA EL JUEGO DE CIENCIA

        //Método para comprobar las solución introducida por el usuario
        public void comprobarCiencia()
        {
            String respuesta = "";
            if (rbtn_resp1.IsChecked == true) respuesta = rbtn_resp1.Content.ToString();
            else if (rbtn_resp2.IsChecked == true) respuesta = rbtn_resp2.Content.ToString();
            else if (rbtn_resp3.IsChecked == true && dificultad == "Dificil") respuesta = rbtn_resp3.Content.ToString();
            else if (rbtn_resp4.IsChecked == true && dificultad == "Dificil") respuesta = rbtn_resp4.Content.ToString();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            if (respuesta == BrainPreguntas.getSolucion(n_pregunta)) //Comprobamos la solucón
            {
                bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                //lbl_ayuda.Content = "BIEN HECHO";
                if (dificultad == "Facil") puntos += 4; 
                else puntos += 5;
            }
            else
            {
                bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                //lbl_ayuda.Content = "!OOH¡ No es \n correcto";
                if (dificultad == "Facil") puntos -= 2;
                else puntos -= 3;
                if (puntos < 0) puntos = 0;
            }
            bi.EndInit();
            tiempo_memorizar.Start();
            this.img_memSol.Source = bi;
            img_bocadilloSolu.Visibility = Visibility.Visible;
            introducirPuntos();
            if (juego == "Ciencia") empezarJuegoCiencia(); //Iniciamos la siguiente ronda
            else if (juego == "Todos") //Si el es todos iniciamos la siguiente ronda
            {
                reiniciarTablero("Ciencia"); //Reiniciamos el tablero indicando a que hemos jugado
                empezarJuegoTodos();
            }
        }

        //Método para configurar el juego de ciencia
        public void empezarJuegoCiencia()
        {
            String[] preguntas = new String[2];
            String[] respuestas = null;
            if (dificultad == "Facil") //Si es fácil solo recogeremos 2 posibles respuestas
                respuestas = new String[2];
            else //Si es difícil recogemos las 4 posibles respuestas 
                respuestas = new String[4];
            Random preg = new Random();
            preguntas = BrainPreguntas.getPreguntas(); //Cogemos todas las preguntas disponibles
            n_pregunta = preg.Next(0, limite_preguntas); //Elegimos una pregunta
            respuestas = BrainPreguntas.getRespuestas(n_pregunta, dificultad); //Recogemos las respuestas
            String pregunta = preguntas[n_pregunta];
            //Mostramos preguntas y respuestas
            lbl_pregunta.Content = BrainPreguntas.getPreguntas()[n_pregunta];
            rbtn_resp1.Content = respuestas[0];
            rbtn_resp2.Content = respuestas[1];
            if (dificultad == "Dificil")
            {
                rbtn_resp3.Content = respuestas[2];
                rbtn_resp4.Content = respuestas[3];
            }
        }


        //MÉTODOS PARA EL JUEGO DE MEMORIA

        //Método para comprobar si la imagen ya esta colocada para memorizar
        public Boolean estaPuesta(int n, int[] numeros)
        {
            for (int i = 0; i < 8; i++)
            {
                if (numeros[i] == n) return false;
            }
            return true;
        }

        //Método para hacer desaparecer las imagenes
        public void vaciarBotonMemoria(int n)
        {
            int valor = 0;
            for (int i = 0; i < 8; i++) //Buscamos la imagen solución
            {
                if (numeros_memoria[i] == n) valor = i;
            }
            switch (valor) //Deshabilitamos la imagen solución
            {
                case 0:
                    this.img_mem1.Source = null;
                    break;
                case 1:
                    this.img_mem2.Source = null;
                    break;
                case 2:
                    this.img_mem3.Source = null;
                    break;
                case 3:
                    this.img_mem4.Source = null;
                    break;
                case 4:
                    this.img_mem5.Source = null;
                    break;
                case 5:
                    this.img_mem6.Source = null;
                    break;
                case 6:
                    this.img_mem7.Source = null;
                    break;
                case 7:
                    this.img_mem8.Source = null;
                    break;
            }
        }

        //Evento del boton1 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem1_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem1.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[0] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Evento del boton2 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem2_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem2.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[1] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Evento del boton3 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem3_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem3.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[2] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Evento del boton4 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem4_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem4.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[3] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Evento del boton5 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem5_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem5.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[4] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Evento del boton6 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem6_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem6.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[5] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Evento del boton7 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem7_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem7.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[6] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }
        
        //Evento del boton8 del juego de memoria para comprobar la solucíon introducida por el usuario
        private void btn_mem8_Click(object sender, RoutedEventArgs e)
        {
            if (img_mem8.Source == null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (numeros_memoria[7] == solucion_memoria)
                {
                    bi.UriSource = new Uri("Imagenes/tick_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos += 5;
                    else puntos += 8;
                }
                else
                {
                    bi.UriSource = new Uri("Imagenes/error_reg.png", UriKind.Relative);
                    if (dificultad == "Facil") puntos -= 3;
                    else puntos += 4;
                    if (puntos < 0) puntos = 0;
                }
                bi.EndInit();
                this.img_memSol.Source = bi;
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                introducirPuntos();
                if (juego == "Memoria") empezarJuegoMemoria();
                else if (juego == "Todos")
                {
                    tiempo_memorizar.Start();
                    reiniciarTablero("Memoria");
                    empezarJuegoTodos();
                }
            }
        }

        //Método para configurar el juego de memoria
        public void empezarJuegoMemoria()
        {
            for (int i = 0; i < 8; i++) numeros_memoria[i] = 0; //Inicializamos la matriz de imagenes
            Random n_imagen = new Random();
            int imagen = n_imagen.Next(1, 9); //Escogemos aleatoriamente una imagen como respuesta
            tipo_memoria = n_imagen.Next(0, 2);
            numeros_memoria[0] = imagen;
            //Mostramos las imagenes aleatoriamente
            BitmapImage bi1 = new BitmapImage(); //Ponemos una imagen en el boton 1
            bi1.BeginInit();
            if (dificultad == "Facil") //Solo dibujos
                bi1.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
            else //Dibujos y números para memorizar
            {
                if (tipo_memoria == 0) bi1.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else bi1.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
            }
            bi1.EndInit();
            this.img_mem1.Source = bi1;
            imagen = n_imagen.Next(1, 9);
            while (!estaPuesta(imagen, numeros_memoria)) //Controlamos que no se repitan imagenes
                imagen = n_imagen.Next(1, 9);
            numeros_memoria[1] = imagen;
            BitmapImage bi2 = new BitmapImage(); //Ponemos una imagen en el boton 2
            bi2.BeginInit();
            if (dificultad == "Facil")
                bi2.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
            else
            {
                if (tipo_memoria == 0) bi2.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else bi2.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
            }
            bi2.EndInit();
            this.img_mem2.Source = bi2;
            imagen = n_imagen.Next(1, 9);
            while (!estaPuesta(imagen, numeros_memoria)) imagen = n_imagen.Next(1, 9);
            numeros_memoria[2] = imagen;
            BitmapImage bi3 = new BitmapImage(); //Ponemos una imagen en el boton 3
            bi3.BeginInit();
            if (dificultad == "Facil")
                bi3.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
            else
            {
                if (tipo_memoria == 0) bi3.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else bi3.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
            }
            bi3.EndInit();
            this.img_mem3.Source = bi3;
            imagen = n_imagen.Next(1, 9);
            while (!estaPuesta(imagen, numeros_memoria)) imagen = n_imagen.Next(1, 9);
            numeros_memoria[3] = imagen;
            BitmapImage bi4 = new BitmapImage(); //Ponemos una imagen en el boton 4
            bi4.BeginInit();
            if (dificultad == "Facil")
                bi4.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
            else
            {
                if (tipo_memoria == 0) bi4.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else bi4.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
            }
            bi4.EndInit();
            this.img_mem4.Source = bi4;
            imagen = n_imagen.Next(1, 9);
            while (!estaPuesta(imagen, numeros_memoria)) imagen = n_imagen.Next(1, 9);
            numeros_memoria[4] = imagen;
            BitmapImage bi5 = new BitmapImage(); //Ponemos una imagen en el boton 5
            bi5.BeginInit();
            if (dificultad == "Facil")
                bi5.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
            else
            {
                if (tipo_memoria == 0) bi5.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else bi5.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
            }
            bi5.EndInit();
            this.img_mem5.Source = bi5;
            imagen = n_imagen.Next(1, 9);
            while (!estaPuesta(imagen, numeros_memoria)) imagen = n_imagen.Next(1, 9);
            numeros_memoria[5] = imagen;
            BitmapImage bi6 = new BitmapImage(); //Ponemos una imagen en el boton 6
            bi6.BeginInit();
            if (dificultad == "Facil")
                bi6.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
            else
            {
                if (tipo_memoria == 0) bi6.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else bi6.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
            }
            bi6.EndInit();
            this.img_mem6.Source = bi6;
            if (dificultad == "Dificil") //Si es dificil mostramos 8 imagenes sino 6
            {
                imagen = n_imagen.Next(1, 9);
                while (!estaPuesta(imagen, numeros_memoria)) imagen = n_imagen.Next(1, 9);
                numeros_memoria[6] = imagen;
                BitmapImage bi7 = new BitmapImage(); //Ponemos una imagen en el boton 7
                bi7.BeginInit();
                if (dificultad == "Facil")
                    bi7.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else
                {
                    if (tipo_memoria == 0) bi7.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                    else bi7.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
                }
                bi7.EndInit();
                this.img_mem7.Source = bi7;
                imagen = n_imagen.Next(1, 9);
                while (!estaPuesta(imagen, numeros_memoria)) imagen = n_imagen.Next(1, 9);
                numeros_memoria[7] = imagen;
                BitmapImage bi8 = new BitmapImage(); //Ponemos una imagen en el boton 8
                bi8.BeginInit();
                if (dificultad == "Facil")
                    bi8.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                else
                {
                    if (tipo_memoria == 0) bi8.UriSource = new Uri("Imagenes/memoria" + imagen + ".png", UriKind.Relative);
                    else bi8.UriSource = new Uri("Imagenes/" + imagen + ".png", UriKind.Relative);
                }
                bi8.EndInit();
                this.img_mem8.Source = bi8;
            }
            progressBar_tiempo_mem.Value = 0; //Inicializamos barra de progreso para memorizar
            introducirTiempo(limite_tiempo_mem, 1);
            tiempo_memorizar.Start(); //Iniciamos temporizador para memorizar
        }



        //MÉTODOS PARA EL JUEGO DE TODOS

        //Método que deshabilita objetos para cambiar de juego de todos
        public void reiniciarTablero(String tablero)
        {
            if (tablero == "Calculo")
            {
                img_n11.Source = null;
                img_n12.Source = null;
                img_n21.Source = null;
                img_n22.Source = null;
                img_op.Source = null;
                img_r1.Source = null;
                img_r2.Source = null;
                img_igual.Source = null;
                txtBox_res.Visibility = Visibility.Collapsed;
                img_tick.Visibility=Visibility.Collapsed;
                btn_comprobar.Visibility = Visibility.Collapsed;
            }
            else if (tablero == "Ciencia")
            {
                lbl_pregunta.Content = "";
                rbtn_resp1.Visibility = Visibility.Collapsed;
                rbtn_resp2.Visibility = Visibility.Collapsed;
                rbtn_resp3.Visibility = Visibility.Collapsed;
                rbtn_resp4.Visibility = Visibility.Collapsed;
                txtBox_res.Visibility = Visibility.Collapsed;
                img_tick.Visibility = Visibility.Collapsed;
                btn_comprobar.Visibility = Visibility.Collapsed;
            }
            else if (tablero == "Memoria")
            {
                btn_mem1.Visibility = Visibility.Collapsed;
                btn_mem2.Visibility = Visibility.Collapsed;
                btn_mem3.Visibility = Visibility.Collapsed;
                btn_mem4.Visibility = Visibility.Collapsed;
                btn_mem5.Visibility = Visibility.Collapsed;
                btn_mem6.Visibility = Visibility.Collapsed;
                btn_mem7.Visibility = Visibility.Collapsed;
                btn_mem8.Visibility = Visibility.Collapsed;
                img_mem1.Source = null;
                img_mem2.Source = null;
                img_mem3.Source = null;
                img_mem4.Source = null;
                img_mem5.Source = null;
                img_mem6.Source = null;
                img_mem7.Source = null;
                img_mem8.Source = null;
                img_tiempo_mem.Source = null;
                progressBar_tiempo_mem.Visibility = Visibility.Collapsed;
            }
        }

        //Método para configurar el juego de todos
        public void empezarJuegoTodos() {
            Random rand_juego = new Random();
            int random_juego = rand_juego.Next(0, 3); //Aleatoriamente elegimos un juego
            if (random_juego == 0) //Juego de calculo
            {
                juego_actual = "Calculo";
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Imagenes/igual.png", UriKind.Relative);
                bi.EndInit();
                this.img_igual.Source = bi;
                lbl_ayuda.Content = "¿Cuanto es?";
                img_tick.Visibility = Visibility.Visible;
                btn_comprobar.Visibility = Visibility.Visible;
                txtBox_res.Visibility = Visibility.Visible;
                empezarJuegoCalculo();
            }
            else if (random_juego == 1) //Juego de ciencia
            {
                juego_actual = "Ciencia";
                lbl_ayuda.Content = "¿Cuál es la\n respuesta?";
                img_tick.Visibility = Visibility.Visible;
                btn_comprobar.Visibility = Visibility.Visible;
                rbtn_resp1.Visibility = Visibility.Visible;
                rbtn_resp2.Visibility = Visibility.Visible;
                if (dificultad == "Dificil")
                {
                    rbtn_resp3.Visibility = Visibility.Visible;
                    rbtn_resp4.Visibility = Visibility.Visible;
                }
                empezarJuegoCiencia();
            }
            else if (random_juego == 2) //Juego de memoria
            {
                juego_actual = "Memoria";
                lbl_ayuda.Content = "!Rápido¡\nMEMORIZA";
                progressBar_tiempo_mem.Maximum = limite_tiempo_mem;
                btn_mem1.Visibility = Visibility.Visible;
                img_mem1.Visibility = Visibility.Visible;
                btn_mem2.Visibility = Visibility.Visible;
                img_mem2.Visibility = Visibility.Visible;
                btn_mem3.Visibility = Visibility.Visible;
                img_mem3.Visibility = Visibility.Visible;
                btn_mem4.Visibility = Visibility.Visible;
                img_mem4.Visibility = Visibility.Visible;
                btn_mem5.Visibility = Visibility.Visible;
                img_mem5.Visibility = Visibility.Visible;
                btn_mem6.Visibility = Visibility.Visible;
                img_mem6.Visibility = Visibility.Visible;
                progressBar_tiempo_mem.Visibility = Visibility.Visible;
                if (dificultad == "Dificil")
                {
                    btn_mem7.Visibility = Visibility.Visible;
                    img_mem7.Visibility = Visibility.Visible;
                    btn_mem8.Visibility = Visibility.Visible;
                    img_mem8.Visibility = Visibility.Visible;
                }
                empezarJuegoMemoria();
            }
        }
    }
}