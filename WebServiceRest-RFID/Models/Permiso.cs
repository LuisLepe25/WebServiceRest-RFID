using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceRest_RFID.Models
{
    public class Permiso
    {
        public int ID { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Lector { get; set; }
        public TimeSpan Hora_Entrada { get; set; }
        public TimeSpan Hora_Salida { get; set; }

        public Permiso(int ID, int ID_Usuario, int ID_Lector, TimeSpan Hora_Entrada, TimeSpan Hora_Salida)
        {
            this.ID = ID;
            this.ID_Usuario = ID_Usuario;
            this.ID_Lector = ID_Lector;
            this.Hora_Entrada = Hora_Entrada;
            this.Hora_Salida = Hora_Salida;
        }

        public Permiso()
        {

        }
    }
}