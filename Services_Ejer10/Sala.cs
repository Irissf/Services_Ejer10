using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Services_Ejer10
{
    class Sala
    {
       
        private List<Socket> sockets;

        /// <summary>
        /// Método que trata de leer el archivo C:\temp\puerto.txt y lee el valor que haya en su primera linea
        /// Si es un valor de puerto válido lo devuelve. Si no lo es o no puede abrir el archivo devuelve 10000.
        /// </summary>
        public void LeePuerto()
        {

        }

        /// <summary>
        /// envioMensaje(string m, IPEndPoint ie): Este método recorre la colección clientes y les envía el mensaje
        /// parámetro m a cada uno de ellos. Antepone al mensaje la IP y el puerto de quién lo envía. Además el 
        /// mensaje no debe repetirse en el cliente que lo ha enviado (identificado por el segundo parámetro).
        /// </summary>
        /// <param name="m"></param>
        /// <param name="ie"></param>
        public void EnvioMensaje(string m, IPEndPoint ie)
        {

        }

    }
}
