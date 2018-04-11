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
                    Log log = new Log();
                    if (row["ID_Usuario"] != null)
                    {
                        log.ID_Usuario = 0;
                    } else
                    {
                        log.ID_Usuario = (int)row["ID_Usuario"];
                    }
                    
                    log.RFID = (long)row["RFID"];
                    log.Fecha = (DateTime)row["Fecha"];
                    log.ID_Lector = (int)row["ID_Lector"];
                    log.Estatus = (int)row["Estatus"];
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
                DataSet ds = Db.executeStoredProcedure("sp_obtenerLogsEntrada");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Log log = new Log();
                    if (row["ID_Usuario"] != null)
                    {
                        log.ID_Usuario = 0;
                    }
                    else
                    {
                        log.ID_Usuario = (int)row["ID_Usuario"];
                    }

                    log.RFID = (long)row["RFID"];
                    log.Fecha = (DateTime)row["Fecha"];
                    log.ID_Lector = (int)row["ID_Lector"];
                    log.Estatus = (int)row["Estatus"];
                    lstLog.Add(log);
                }
                return lstLog;
            }
        }

        // GET: logs/entradas/1
        [Route("logs/entradas/{idUsuario:int}")]
        public IEnumerable<Log> GetLogsEntradas(int idUsuario)
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Log> lstLog = new List<Log>();
                SqlParameter pIdUsuario = new SqlParameter("@idUsuario", idUsuario);
                DataSet ds = Db.executeStoredProcedure("sp_obtenerLogsEntradaPorID", pIdUsuario);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Log log = new Log();
                    if (row["ID_Usuario"] != null)
                    {
                        log.ID_Usuario = idUsuario;
                    }
                    else
                    {
                        log.ID_Usuario = (int)row["ID_Usuario"];
                    }

                    log.RFID = (long)row["RFID"];
                    log.Fecha = (DateTime)row["Fecha"];
                    log.ID_Lector = (int)row["ID_Lector"];
                    log.Estatus = (int)row["Estatus"];
                    lstLog.Add(log);
                }
                return lstLog;
            }
        }

        // GET: logs/salidas
        [Route("logs/salidas")]
        public IEnumerable<Log> GetLogsSalidas()
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Log> lstLog = new List<Log>();
                DataSet ds = Db.executeStoredProcedure("sp_obtenerLogsSalida");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Log log = new Log();
                    if (row["ID_Usuario"] != null)
                    {
                        log.ID_Usuario = 0;
                    }
                    else
                    {
                        log.ID_Usuario = (int)row["ID_Usuario"];
                    }

                    log.RFID = (long)row["RFID"];
                    log.Fecha = (DateTime)row["Fecha"];
                    log.ID_Lector = (int)row["ID_Lector"];
                    log.Estatus = (int)row["Estatus"];
                    lstLog.Add(log);
                }
                return lstLog;
            }
        }

        // GET: logs/salidas/1
        [Route("logs/salidas/{idUsuario:int}")]
        public IEnumerable<Log> GetLogsSalidas(int idUsuario)
        {
            using (DBManualConnection Db = new DBManualConnection())
            {
                List<Log> lstLog = new List<Log>();
                SqlParameter pIdUsuario = new SqlParameter("@idUsuario", idUsuario);
                DataSet ds = Db.executeStoredProcedure("sp_obtenerLogsSalidaPorID", pIdUsuario);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Log log = new Log();
                    if (row["ID_Usuario"] != null)
                    {
                        log.ID_Usuario = idUsuario;
                    }
                    else
                    {
                        log.ID_Usuario = (int)row["ID_Usuario"];
                    }

                    log.RFID = (long)row["RFID"];
                    log.Fecha = (DateTime)row["Fecha"];
                    log.ID_Lector = (int)row["ID_Lector"];
                    log.Estatus = (int)row["Estatus"];
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

        // POST: usuario/crear
        [Route("usuario/crear")]
        public void PostCrearUsuario([FromBody]JObject jsonResult)
        {
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonResult.ToString());
            using (DBManualConnection Db = new DBManualConnection())
            {
                SqlParameter pRFID = new SqlParameter("@RFID", usuario.RFID);
                SqlParameter pNombre = new SqlParameter("@nombre", usuario.Nombre);
                DataSet ds = Db.executeStoredProcedure("sp_insertarUsuario", pRFID, pNombre);
            }
        }

        // POST: usuario/crear
        [Route("permiso/crear")]
        [HttpPost]
        public void PostCrearPermiso([FromBody]JObject jsonResult)
        {
            Permiso permiso = JsonConvert.DeserializeObject<Permiso>(jsonResult.ToString());
            using (DBManualConnection Db = new DBManualConnection())
            {
                SqlParameter pIdUsuario = new SqlParameter("@idUsuario", permiso.ID_Usuario);
                SqlParameter pIdLector = new SqlParameter("@idLector", permiso.ID_Lector);
                SqlParameter pHoraEntrada = new SqlParameter("@horaEntrada", permiso.Hora_Entrada);
                SqlParameter pHoraSalida = new SqlParameter("@horaSalida", permiso.Hora_Salida);
                DataSet ds = Db.executeStoredProcedure("sp_insertarPermiso", pIdUsuario, pIdLector, pHoraEntrada, pHoraSalida);
            }
        }

        // PUT: usuario/actualizar/1
        [Route("usuario/actualizar/{id:int}")]
        public void PutActualizarUsuario(int id, [FromBody]JObject value)
        {
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(value.ToString());
            using (DBManualConnection Db = new DBManualConnection())
            {
                SqlParameter pIdUsuario = new SqlParameter("@idUsuario", id);
                SqlParameter pRFID = new SqlParameter("@RFID", usuario.RFID);
                SqlParameter pNombre = new SqlParameter("@nombre", usuario.Nombre);
                DataSet ds = Db.executeStoredProcedure("sp_editarUsuario", pIdUsuario, pRFID, pNombre);
            }
        }

        // PUT: permiso/actualizar/1
        [Route("permiso/actualizar/{id:int}")]
        public void PutActualizarPermiso(int id, [FromBody]JObject value)
        {
            Permiso permiso = JsonConvert.DeserializeObject<Permiso>(value.ToString());
            using (DBManualConnection Db = new DBManualConnection())
            {
                SqlParameter pIdPermiso = new SqlParameter("@idPermiso", id);
                SqlParameter pIdUsuario = new SqlParameter("@idUsuario", permiso.ID_Usuario);
                SqlParameter pIdLector = new SqlParameter("@idLector", permiso.ID_Lector);
                SqlParameter pHoraEntrada = new SqlParameter("@horaEntrada", permiso.Hora_Entrada);
                SqlParameter pHoraSalida = new SqlParameter("@horaSalida", permiso.Hora_Salida);
                DataSet ds = Db.executeStoredProcedure("sp_editarPermiso", pIdPermiso, pIdUsuario, pIdLector, pHoraEntrada, pHoraSalida);
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
