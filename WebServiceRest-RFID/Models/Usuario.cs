using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceRest_RFID.Models
{
    public class Usuario
    {
        public int ID { get; set; }
        public long RFID { get; set; }
        public String Nombre { get; set; }

        public Usuario(int ID, long RFID, String Nombre)
        {
            this.ID = ID;
            this.RFID = RFID;
            this.Nombre = Nombre;
        }

        public Usuario()
        {

        }
    }
}