using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services_Ejer10
{
    class Program
    {
        static void Main(string[] args)
        {
            /*En el programa principal que estará en la clase Program simplemente se crea un objeto del tipo Sala y se 
            llama a la función iniciaServicioChat.

            b) Lanza la clase anterior como servicio de Windows. Al iniciarse lanzará como hilo iniciaServicioChat(). Se 
            finalizará dicho hilo al detener el servicio. No dispone de Pausa ni Continuación. El nombre del servicio será 
            ServicioChat2015.*/

            Sala sala = new Sala();
            sala.IniciaServicioChat();
        }
    }
}
