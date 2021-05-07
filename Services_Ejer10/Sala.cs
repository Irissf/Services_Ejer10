using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Services_Ejer10
{
    /*
     Realización del servidor de una sala de chat. Los clientes serán consolas telnet. Se pide:
    a) Crea una clase denominada Sala. En dicha clase habrá al menos la propiedad clientes que será una 
    colección de Sockets y los siguientes métodos:

     */
    class Sala
    {

        private List<Socket> clientes = new List<Socket>();
        public Object llave = new object();

        /// <summary>
        /// Método que trata de leer el archivo C:\temp\puerto.txt y lee el valor que haya en su primera linea
        /// Si es un valor de puerto válido lo devuelve. Si no lo es o no puede abrir el archivo devuelve 10000.
        /// </summary>
        public int LeePuerto()
        {
            Console.WriteLine("entro");
            //si ponemos la \ en vez de / poner delante de la cadena de texto @ => @"C:\temp\puerto.txt"
            try
            {
                using (StreamReader sr = new StreamReader("C:/temp/puerto.txt"))
                {
                    //intentamos leer una linea de texto
                    string line = sr.ReadLine();
                    //si la linea contiene algo
                    if (line != null)
                    {
                        //Un número de puerto es válido si pertenece al rango de 0 a 65535.
                        try
                        {
                            int num = Convert.ToInt32(line);
                            if (num > 10000 && num <= IPEndPoint.MaxPort) //puerto valor máximo y valor mínimo
                            {
                                Console.WriteLine("Estoy entre 10000 y 65535");
                                return num;
                            }
                          
                        }
                        catch (Exception)
                        {
                          
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No tengo archivo");
                
            }
            return 10000;

        }

        /// <summary>
        /// envioMensaje(string m, IPEndPoint ie): Este método recorre la colección clientes y les envía el mensaje
        /// parámetro m a cada uno de ellos. Antepone al mensaje la IP y el puerto de quién lo envía. Además el 
        /// mensaje no debe repetirse en el cliente que lo ha enviado (identificado por el segundo parámetro).
        /// </summary>
        /// <param name="m">mensaje</param>
        /// <param name="ie">para la ip y el puerto de cada uno de los clientes</param>
        public void EnvioMensaje(string m, IPEndPoint ie)
        {
            IPEndPoint info;
            lock (llave)
            {
                for (int i = clientes.Count; i > 0; i--)
                {
                    info = (IPEndPoint)clientes[i-1].RemoteEndPoint;
                    using (NetworkStream ns = new NetworkStream(clientes[i-1]))
                    using (StreamWriter sw = new StreamWriter(ns))
                    {
                        try
                        {
                            if (ie.Port != info.Port)
                            {
                                sw.WriteLine("tamaño:{0}",clientes.Count );
                                sw.WriteLine("Ip:{0}, Port:{1}: {2}", ie.Address, ie.Port, m);
                                sw.Flush();
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Entro aqui");
                            clientes.RemoveAt(i);
                        }

                    }
                }
            }
        }

        /// <summary>
        ///  Método principal donde se inicia el servicio. Realiza la programación necesaria para la
        ///  comunicación por red. Se obtiene el puerto llamando a leePuerto, si el puerto está ocupado lo incrementará
        ///  en una unidad de forma sucesiva hasta que encuentre uno libre. Si llega a máximo puerto permitido (si no
        ///  sabes su valor o no sabes averiguarlo usa 60000) se vuelve a poner en 10000. Informa por pantalla del 
        ///  puerto de conexión y se queda a la escucha. ¿?
        ///  
        /// Finalmente entra en un bucle en el cual se realiza la conexión con el cliente iniciando un hilo que ejecuta la 
        /// función hiloCliente.Además se añadirá el socket de cliente a la colección clientes
        /// </summary>
        public void IniciaServicioChat()

        {
            bool puertoCorrecto = false;
            int puerto = LeePuerto();
            while (!puertoCorrecto)
                try
                {
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), puerto);
                    puertoCorrecto = true;

                    Socket socketConexion = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketConexion.Bind(endpoint);
                    socketConexion.Listen(2);

                    while (true)
                    {
                        Socket socketCliente = socketConexion.Accept();
                        Thread hilos = new Thread(HiloCliente);
                        hilos.Start(socketCliente);
                    }
                }
                catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                {
                    if (puerto < IPEndPoint.MaxPort)
                    {
                        puerto++;
                    }
                    else
                    {
                        puerto = 10000;
                    }
                }
                catch (Exception)
                {

                }

        }



        /// <summary>
        ///  Función que se ejecuta como hilo según se ha comentado. El parámetro es el 
        /// socket de cliente. Indicará por pantalla la IP y puerto del cliente. Luego mandará un mensaje de bienvenida 
        /// al cliente y le informa de la cantidad de clientes conectados a la sala de chat.
        /// </summary>
        /// <param name="Socket"></param>
        public void HiloCliente(object Socket)
        {
            Socket socketCliente = (Socket)Socket;
            string mensaje;
            bool cerrarChat = false;

            lock (llave)
            {
                clientes.Add(socketCliente);
            }

            IPEndPoint info = (IPEndPoint)socketCliente.RemoteEndPoint;

            using (NetworkStream ns = new NetworkStream(socketCliente))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                sw.WriteLine("Wellcome Puerto:{0} || IP:{1}", info.Port, info.Address);
                sw.WriteLine("Personas conectadas:{0} ", clientes.Count);
                sw.Flush();

                while (!cerrarChat)
                {
                    try
                    {
                        mensaje = sr.ReadLine();
                        if (mensaje != null)
                        {
                            if (mensaje == "MELARGO")
                            {
                                EnvioMensaje("Se ha desconectado", info);
                            }
                            else
                            {
                                EnvioMensaje(mensaje, info);
                            }
                        }

                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Entro aqui, se marchó"+ info.Port);
                        
                        //CREO QUE TENGO QUE ELIMINAR AQUI AL SUJETO. creo

                        //al desconectarse el cliente devuelve null, como la variable sigue a false del while vuelve a intentar leer
                        //y da error, lo solucionamos con el try catch, o buscariamos la forma de poner la variable a true 
                        
                    }


                }

            }
        }

        /*
         A continuación queda a la espera de el cliente le envíe algún mensaje. Si sucede esto, recoge el mensaje y lo
        reenvía al resto de los clientes llamando a la función envioMensaje.

        Si el mensaje que envía el cliente es la palabra MELARGO se cerrará la conexión con dicho cliente y se 
        eliminará de la colección
        */
    }
}
