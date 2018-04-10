using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebServiceRest_RFID.Models;

namespace WebServiceRest_RFID.Controllers
{
    public class rfidClientController : ApiController
    {
        // GET: prueba
        [Route("prueba")]
        public String GetPrueba()
        {
            return "Prueba";
        }

        // GET: usuario/all
        [Route("usuario/all")]
        public IEnumerable<Usuario> GetUsuarios()
        {
            List<Usuario> lstUsuario = new List<Usuario>();
            using (DBManualConnection Db = new DBManualConnection())
            {
                DataSet ds = Db.executeStoredProcedure("sp_obtenerUsuarios");
                
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Usuario user = new Usuario();
                    user.ID = (int)row["ID"];
                    user.RFID = (long)row["RFID"];
                    user.Nombre = (String)row["Nombre"];
                    lstUsuario.Add(user);
                }
            }
            return lstUsuario;
        }

        // GET: usuario/id
        [Route("usuario/{id:int}")]
        public Usuario GetUsuarioPorID(int id)
        {
            using(DBManualConnection Db = new DBManualConnection())
            {
                SqlParameter pUsuario = new SqlParameter("@idUsuario", id);
                DataSet ds = Db.executeStoredProcedure("sp_obtenerUsuariosPorID", pUsuario);
                Usuario user = new Usuario();
                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    user.ID = (int) row["ID"];
                    user.RFID = (long)row["RFID"];
                    user.Nombre = (String) row["Nombre"];
                }
                return user;
            }
        }

        // GET: usuario/Lector/5
        [Route("usuario/Lector/{id:int}")]
        public IEnumerable<Usuario> GetUsuarioPorLector(int id)
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Usuario> lstUsuario = new List<Usuario>();
                SqlParameter pLector = new SqlParameter("@idLector", id);
                DataSet ds = Db.executeStoredProcedure("sp_obtenerUsuariosPorLector", pLector);
                
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Usuario user = new Usuario();
                    user.ID = (int)row["ID"];
                    user.RFID = (long)row["RFID"];
                    user.Nombre = (String)row["Nombre"];
                    lstUsuario.Add(user);
                }
                return lstUsuario;
            }
        }

        // GET: permiso/
        [Route("permiso")]
        public IEnumerable<Permiso> GetPermisos()
        {
            List<Permiso> lstPermiso = new List<Permiso>();
            using (DBManualConnection Db = new DBManualConnection())
            {
                DataSet ds = Db.executeStoredProcedure("sp_obtenerPermisos");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Permiso permiso = new Permiso();
                    permiso.ID = (int)row["ID"];
                    permiso.ID_Usuario = (int)row["ID_Usuario"];
                    permiso.ID_Lector = (int)row["ID_Lector"];
                    permiso.Hora_Entrada = (TimeSpan)row["Hora_Entrada"];
                    permiso.Hora_Salida = (TimeSpan)row["Hora_Salida"];
                    lstPermiso.Add(permiso);
                }
            }
            return lstPermiso;
        }

        // GET: permiso/id
        [Route("permiso/{id:int}")]
        public IEnumerable<Permiso> GetPermisoPorUsuario(int id)
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Permiso> lstPermiso = new List<Permiso>();
                SqlParameter pUsuario = new SqlParameter("@idUsuario", id);
                DataSet ds = Db.executeStoredProcedure("sp_obtenerPermisosPorIDUsuario", pUsuario);
                
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Permiso permiso = new Permiso();
                    permiso.ID = (int)row["ID"];
                    permiso.ID_Usuario = (int)row["ID_Usuario"];
                    permiso.ID_Lector = (int)row["ID_Lector"];
                    permiso.Hora_Entrada = (TimeSpan)row["Hora_Entrada"];
                    permiso.Hora_Salida = (TimeSpan)row["Hora_Salida"];
                    lstPermiso.Add(permiso);
                }
                return lstPermiso;
            }
        }

        // GET: permiso/Lector/id
        [Route("permiso/Lector/{id:int}")]
        public IEnumerable<Permiso> GetPermisoPorLector(int id)
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Permiso> lstPermiso = new List<Permiso>();
                SqlParameter pLector = new SqlParameter("@idLector", id);
                DataSet ds = Db.executeStoredProcedure("sp_obtenerPermisosPorLector", pLector);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Permiso permiso = new Permiso();
                    permiso.ID = (int)row["ID"];
                    permiso.ID_Usuario = (int)row["ID_Usuario"];
                    permiso.ID_Lector = (int)row["ID_Lector"];
                    permiso.Hora_Entrada = (TimeSpan)row["Hora_Entrada"];
                    permiso.Hora_Salida = (TimeSpan)row["Hora_Salida"];
                    lstPermiso.Add(permiso);
                }
                return lstPermiso;
            }
        }

        // GET: intentos
        [Route("intentos")]
        public IEnumerable<Log> GetIntentosAccesoNoAutorizados()
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Log> lstLog = new List<Log>();
                DataSet ds = Db.executeStoredProcedure("sp_obtenerIntentosDeAccesosNoAutorizados");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Log log = new Log
                    {
                        ID_Usuario = (int)row["ID_Usuario"],
                        RFID = (int)row["RFID"],
                        Fecha = (DateTime)row["Fecha"],
                        ID_Lector = (int)row["ID_Lector"],
                        Estatus = (int)row["Estatus"]
                    };
                    lstLog.Add(log);
                }
                return lstLog;
            }
        }

        // GET: logs/entradas
        [Route("logs/entradas")]
        public IEnumerable<Log> GetLogsEntradas()
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Log> lstLog = new List<Log>();
                DataSet ds = Db.executeStoredProcedure("sp_obtenerIntentosDeAccesosNoAutorizados");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Log log = new Log
                    {
                        ID_Usuario = (int)row["ID_Usuario"],
                        RFID = (int)row["RFID"],
                        Fecha = (DateTime)row["Fecha"],
                        ID_Lector = (int)row["ID_Lector"],
                        Estatus = (int)row["Estatus"]
                    };
                    lstLog.Add(log);
                }
                return lstLog;
            }
        }

        // POST: logs/bonitos
        [Route("logs/bonitos")]
        public void Post([FromBody]JObject jsonResult)
        {
            Logs listaLogs = JsonConvert.DeserializeObject<Logs>(jsonResult.ToString());
            using (DBManualConnection Db = new DBManualConnection())
            {
                foreach (Log log in listaLogs.logs)
                {
                    SqlParameter pIdUsuario = new SqlParameter("@idUsuario", log.ID_Usuario);
                    SqlParameter pRFID = new SqlParameter("@rfid", log.RFID);
                    SqlParameter pFecha = new SqlParameter("@fecha", log.Fecha);
                    SqlParameter pIdLector = new SqlParameter("@idLector", log.ID_Lector);
                    SqlParameter pEstatus = new SqlParameter("@estatus", log.Estatus);
                    DataSet ds = Db.executeStoredProcedure("sp_insertarLog", pIdUsuario, pRFID, pFecha, pIdLector, pEstatus);
                }
            }
        }

        // PUT: api/rfidClient/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/rfidClient/5
        public void Delete(int id)
        {
        }
    }
}
