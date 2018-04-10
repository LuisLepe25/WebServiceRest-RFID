using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebServiceRest_RFID.Controllers
{

    public class DBManualConnection : IDisposable
    {
        private SqlConnection sqlConnection = null;

        public DBManualConnection()
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
            sqlConnection.Open();
        }

        ~DBManualConnection() 
        {
            Dispose();
        }

        public void Dispose()
        {
            if (sqlConnection != null)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();
                sqlConnection = null;
            }
        }

        public DataSet executeStoredProcedure(string spName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            using (SqlCommand sqlCommand = new SqlCommand(spName, sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                int i = 0;
                foreach(SqlParameter sqlParam in parameters)
                {
                    sqlCommand.Parameters.Add(sqlParam);
                }
                
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds);
                }
            }
            return ds;
        }

        public DataSet getConceptosSAT(int idConceptoSAT = -1)
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT idConceptoSAT, Codigo";
            if (idConceptoSAT > 0)
                strQuery += ", Descripcion ";
            else
                strQuery += ", codigo + ' ' + case naturaleza WHEN 1 THEN '(Per) ' WHEN 2 THEN '(Ded) ' ELSE '(Otr) ' END + Descripcion as Descripcion";
            strQuery += ", Naturaleza, case naturaleza WHEN 1 THEN 'Percepcion' ELSE 'Deduccion' END as NaturalezaDescripcion FROM [SESSipConceptosSAT] as c ";
            if (idConceptoSAT > 0)
            {
                strQuery += " WHERE idConceptoSAT = @idConceptoSAT ";
            }
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idConceptoSAT",
                    Value = idConceptoSAT
                };
                sqlCommand.Parameters.Add(sqlParameter);
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds, "listaConceptosSAT");
                }
            }
            return ds;
        }

        public DataSet getListaNominas(int PortalId, string strTipoNomina, string strAnio)
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT d.idNomina, d.idEmpresa, substring(d.idNomina, 1, 1) as Tipo, substring(d.idNomina, 2, 2) as Numero, substring(d.idNomina, 4, 4) as Anio  , sum(Salario) as Salario, isnull(Sum(e.GRAVABLE), 0) as Credito, sum(OtrasP) - isnull(sum(e.GRAVABLE), 0) as OtrasP, sum(ISPT) as ISPT, sum(IMMS) as IMMS, sum(IMMSVariable) as IMMSVariable, sum(OtrasD) as OtrasD , sum(Salario) + sum(OtrasP) as TotalPercepciones, sum(ISPT) + sum(IMMS) + sum(OtrasD) as TotalDeducciones, FechaDeAplicacion FROM(SELECT substring(idnomina, 1, 7) as idNomina, idEmpresa, Nomina, Nombre, Depto, Puesto, Salario, Credito, OtrasP, ISPT, IMMS, IMMSVariable, OtrasD, FechaDeAplicacion, FechaTimbradoReal FROM SESSipNominaArchivoD WHERE idEmpresa = @idPortal and idNomina like @strNomina) as d left JOIN(SELECT substring(idnomina, 1, 7) as idNomina, idEmpresa, Nomina, folio, Concepto, Naturaleza, Valor, GRAVABLE FROM SESSipNominaArchivoE WHERE (CONCEPTO= 'PP21') and idEmpresa = @idPortal and idNomina like @strNomina) as e on d.idNomina = e.idNomina and d.idEmpresa = e.idEmpresa and d.Nomina = e.Nomina GROUP BY d.idNomina, d.idEmpresa, FechaDeAplicacion ORDER BY Tipo, Anio, Numero ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idPortal",
                    Value = PortalId
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@strNomina",
                    Value = strTipoNomina + "%" + strAnio + "%"
                };
                sqlCommand.Parameters.Add(sqlParameter);
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds, "listaNominas");
                }
            }
            return ds;
        }

        public DataSet getListaNominasDetalle(int PortalId, string strNomina)
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT d.idNomina, d.idEmpresa, substring(d.idNomina, 1, 1) as Tipo, substring(d.idNomina, 2,2) as Numero, substring(d.idNomina, 4, 5) as Anio  , d.Nomina, Nombre, Salario, isnull(e.GRAVABLE,0) as Credito, (isnull(OtrasP, 0)-isnull(e.GRAVABLE,0)) + isnull(e.EXCENTO, 0) as OtrasP,ISPT, IMMS, IMMSVariable, OtrasD  , Salario+OtrasP as TotalPercepciones,  ISPT+IMMS+OtrasD as TotalDeducciones, EstaTimbrado, Error, [FechaDeAplicacion], [FechaDeTimbrado], d.[idFormaDePago], [FormaDePagoDatoExtra], tp.Descripcion " +
                "FROM (SELECT * FROM(SELECT substring(idNomina, 1, 7) as idNomina, idEmpresa, Nomina, Nombre, Depto, Puesto, Salario, Credito, OtrasP, ISPT, IMMS, IMMSVariable, OtrasD, iif(ArchivoXMLTimbrado is null or ArchivoXMLTimbrado = '', 0, 1) as EstaTimbrado, Error, [FechaDeAplicacion], [FechaDeTimbrado], [idFormaDePago], [FormaDePagoDatoExtra]   FROM SESSipNominaArchivoD WHERE idEmpresa = @idPortal) as Tmp WHERE idNomina = @idNomina) as d " +
                "left JOIN (SELECT substring(idNomina, 1, 7) as idNomina, idEmpresa, Nomina, folio, Concepto, Naturaleza, Valor, GRAVABLE, EXCENTO FROM SESSipNominaArchivoE WHERE(CONCEPTO= 'PP21') and idEmpresa = @idPortal) as e on d.idNomina = e.idNomina and d.idEmpresa = e.idEmpresa and d.Nomina = e.Nomina " +
                "left join [SESSipTiposDePago] as tp on d.idFormaDePago = tp.idFormaDePago " +
                "ORDER BY Tipo, Anio, Numero, Nomina ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idPortal",
                    Value = PortalId
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idNomina",
                    Value = strNomina
                };
                sqlCommand.Parameters.Add(sqlParameter);
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds, "listaNominas");
                }
            }
            return ds;
        }

        public DataSet getListaErroresTimbrado(int PortalId)
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT idEmpresa, Nombre, Error, Status, count(*) as Cantidad " +
                " FROM(" +
                " SELECT idEmpresa, Nombre, isnull(nullif(ErrorMensajePublico, ''), Error) as Error, 1 as Status " +
                " FROM[dbo].[SESSipNominaArchivoD] " +
                " WHERE(ArchivoXMLTimbrado is null or ArchivoXMLTimbrado = '') " +
                "     and Error <> '' " +
                "     and idEmpresa = @idPortal " +
                " ) as Tmp " +
                " GROUP BY idEmpresa, Nombre, Error, Status " +
                " ORDER BY idEmpresa, Nombre, Error, Cantidad ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@idPortal", PortalId);
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds, "listaNominas");
                }
            }
            return ds;
        }

        public DataSet getListaAniosDeNominas(int PortalId)
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT distinct SUBSTRING(idNomina, 4, 4) as Anios " +
                " FROM [dbo].[SESSipNominaArchivoD] " +
                " WHERE idEmpresa = @idPortal " +
                " ORDER BY Anios ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@idPortal", PortalId);
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds, "listaAnios");
                }
            }
            return ds;
        }

        public DataSet getListaNominasTimbradas(int PortalId, string anio = "")
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT SUBSTRING(idNomina, 1, 7) as idNomina, Nomina, Nombre, Salario, Credito, OtrasP, ISPT, IMMS, IMMSVariable, OtrasD, FechaDeAplicacion as FechaDePago, FechaTimbradoReal " +
                " FROM [dbo].[SESSipNominaArchivoD] " +
                " WHERE ArchivoXMLTimbrado<> '' " +
                " and idEmpresa = @idPortal ";  
            if (anio != "")
            {
                strQuery += " and substring(idNomina, 4, 4) = @anio ";
            }
            strQuery += " ORDER BY idNomina, Nombre, Nomina ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@idPortal", PortalId);
                if (anio != "")
                {
                    sqlCommand.Parameters.AddWithValue("@anio", anio);
                }
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    da.Fill(ds, "listaNominasTimbradas");
                }
            }
            return ds;
        }

        //Quitar Errores (para poder volver a timbrar)
        public void limpiarErrores(int idPortal, string Nombre, string Error)
        {
            string strQuery = "UPDATE [dbo].[SESSipNominaArchivoD] " +
                " SET error = '', idError = '', ErrorMensajePublico = '' " +
                " WHERE idEmpresa = @idPortal " +
                "     and Nombre = @Nombre " +
                "     and (Error = @Error or ErrorMensajePublico = @Error) ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@idPortal", idPortal);
                sqlCommand.Parameters.AddWithValue("@Nombre", Nombre);
                sqlCommand.Parameters.AddWithValue("@Error", Error);
                sqlCommand.ExecuteNonQuery();
            }

        }

        //Crear una nueva nomina
        public int createNewNomina(string idNomina, int pIdEmpresa, string numeroDeNomina, DateTime dtFechaBaja, double pGravable, double pExento, string strConcepto)
        {
            int id = -1;
            string strQuery = "INSERT INTO [dbo].[SESSipNominaArchivoD] " +
                " ([idNomina] " +
                " ,[idEmpresa] " +
                " ,[Nomina] " +
                " ,[Nombre] " +
                " ,[Depto] " +
                " ,[Puesto] " +
                " ,[Salario] " +
                " ,[Credito] " +
                " ,[OtrasP] " +
                " ,[ISPT] " +
                " ,[IMMS] " +
                " ,[IMMSVariable] " +
                " ,[OtrasD] " +
                " ,[PGGravable] " +
                " ,[PEXCENTA] " +
                " ,[CONFIDENCIAL] " +
                " ,[CALCULOISPT] " +
                " ,[CDACT] " +
                " ,[idFormaDePago] " +
                " ,[FormaDePagoDatoExtra] " +
                " ,[FechaDeAplicacion] " +
                " ,[FechaDeTimbrado] " +
                " ,[FechaGeneracionDeArchivos] " +
                " ,[ArchivoXMLOriginal] " +
                " ,[ArchivoXMLSellado] " +
                " ,[ArchivoXMLTimbrado] " +
                " ,[ArchivoXMLTimbradoOptimizado] " +
                " ,[ArchivoPDF] " +
                " ,[FechaTimbradoReal] " +
                " ,[Error] " +
                " ,[idError] " +
                " ,[ErrorMensajePublico] " +
                " ,[CreatedOnDate] " +
                " ,[CreatedByUserId]) " +
          " VALUES " +
                " (@idNomina " +
                " ,@idEmpresa " +
                " ,@Nomina " +
                " ,@Nombre " +
                " ,@Depto " +
                " ,@Puesto " +
                " ,@Salario " +
                " ,@Credito " +
                " ,@OtrasP " +
                " ,@ISPT " +
                " ,@IMMS " +
                " ,@IMMSVariable " +
                " ,@OtrasD " +
                " ,@PGGravable " +
                " ,@PEXCENTA " +
                " ,@CONFIDENCIAL " +
                " ,@CALCULOISPT " +
                " ,@CDACT " +
                " ,@idFormaDePago " +
                " ,@FormaDePagoDatoExtra " +
                " ,@FechaDeAplicacion " +
                " ,@FechaDeTimbrado " +
                " ,@FechaGeneracionDeArchivos " +
                " ,@ArchivoXMLOriginal " +
                " ,@ArchivoXMLSellado " +
                " ,@ArchivoXMLTimbrado " +
                " ,@ArchivoXMLTimbradoOptimizado " +
                " ,@ArchivoPDF " +
                " ,@FechaTimbradoReal " +
                " ,@Error " +
                " ,@idError " +
                " ,@ErrorMensajePublico " +
                " ,@CreatedOnDate " +
                " ,@CreatedByUserId) " +
                "; SELECT cast(@@IDENTITY as int) as id; ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@idNomina", idNomina);
                sqlCommand.Parameters.AddWithValue("@idEmpresa", idNomina);
                sqlCommand.Parameters.AddWithValue("@Nomina", numeroDeNomina);
                sqlCommand.Parameters.AddWithValue("@Nombre", "");
                sqlCommand.Parameters.AddWithValue("@Depto", "");
                sqlCommand.Parameters.AddWithValue("@Puesto", idNomina);
                sqlCommand.Parameters.AddWithValue("@Salario", idNomina);
                sqlCommand.Parameters.AddWithValue("@Credito", idNomina);
                sqlCommand.Parameters.AddWithValue("@OtrasP", idNomina);
                sqlCommand.Parameters.AddWithValue("@ISPT", idNomina);
                sqlCommand.Parameters.AddWithValue("@IMMS", idNomina);
                sqlCommand.Parameters.AddWithValue("@IMMSVariable", idNomina);
                sqlCommand.Parameters.AddWithValue("@OtrasD", idNomina);
                sqlCommand.Parameters.AddWithValue("@PGGravable", idNomina);
                sqlCommand.Parameters.AddWithValue("@PEXCENTA", idNomina);
                sqlCommand.Parameters.AddWithValue("@CONFIDENCIAL", idNomina);
                sqlCommand.Parameters.AddWithValue("@CALCULOISPT", idNomina);
                sqlCommand.Parameters.AddWithValue("@CDACT", idNomina);
                sqlCommand.Parameters.AddWithValue("@idFormaDePago", idNomina);
                sqlCommand.Parameters.AddWithValue("@FormaDePagoDatoExtra", idNomina);
                sqlCommand.Parameters.AddWithValue("@FechaDeAplicacion", idNomina);
                sqlCommand.Parameters.AddWithValue("@FechaDeTimbrado", idNomina);
                sqlCommand.Parameters.AddWithValue("@FechaGeneracionDeArchivos", idNomina);
                sqlCommand.Parameters.AddWithValue("@ArchivoXMLOriginal", idNomina);
                sqlCommand.Parameters.AddWithValue("@ArchivoXMLSellado", idNomina);
                sqlCommand.Parameters.AddWithValue("@ArchivoXMLTimbrado", idNomina);
                sqlCommand.Parameters.AddWithValue("@ArchivoXMLTimbradoOptimizado", idNomina);
                sqlCommand.Parameters.AddWithValue("@ArchivoPDF", idNomina);
                sqlCommand.Parameters.AddWithValue("@FechaTimbradoReal", idNomina);
                sqlCommand.Parameters.AddWithValue("@Error", idNomina);
                sqlCommand.Parameters.AddWithValue("@idError", idNomina);
                sqlCommand.Parameters.AddWithValue("@ErrorMensajePublico", idNomina);
                sqlCommand.Parameters.AddWithValue("@CreatedOnDate", idNomina);
                sqlCommand.Parameters.AddWithValue("@CreatedByUserId", idNomina);

                id = (int)sqlCommand.ExecuteScalar();
            }
            return id;
        }

        //Crear un nuevo concepto de SAT
        public int createNewConceptoSAT(string codigo, string descripcion, int naturaleza)
        {
            int id = -1;
            string strQuery = "INSERT INTO [SESSipConceptosSAT] (codigo, descripcion, naturaleza) VALUES (@codigo, @descripcion, @naturaleza); SELECT cast(@@IDENTITY as int) as id;";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter;
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@codigo",
                    Value = codigo
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@descripcion",
                    Value = descripcion
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@naturaleza",
                    Value = naturaleza
                };
                sqlCommand.Parameters.Add(sqlParameter);

                id = (int)sqlCommand.ExecuteScalar();
            }
            return id;
        }

        //Actualizar Concepto del SAT
        public void updateConceptoSIP(string codigo, string descripcion, int idConceptoSAT, int tipo, int prioridad, int integraAlSeguroSocial, int impuestoEstatal, int isr, int integrarComoPrevisionSocial, string acumularConElConcepto)
        {
            string strQuery = "UPDATE SESSipConceptos SET  " +
                " descripcion = @descripcion " +
                ", idConceptoSAT = @idConceptoSAT, tipo = @tipo " +
                ", prioridad = @prioridad, integraAlSeguroSocial = @integraAlSeguroSocial " +
                ", impuestoEstatal = @impuestoEstatal, isr = @isr " +
                ", integrarComoPrevisionSocial = @integrarComoPrevisionSocial " +
                ", acumularConElConcepto = @acumularConElConcepto " +
                "WHERE codigo = @codigo ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@codigo",
                    Value = codigo
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@descripcion",
                    Value = descripcion
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idConceptoSAT",
                    Value = idConceptoSAT
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@tipo",
                    Value = tipo
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@prioridad",
                    Value = prioridad
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@integraAlSeguroSocial",
                    Value = integraAlSeguroSocial
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@impuestoEstatal",
                    Value = impuestoEstatal
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@isr",
                    Value = isr
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@integrarComoPrevisionSocial",
                    Value = integrarComoPrevisionSocial
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@acumularConElConcepto",
                    Value = acumularConElConcepto
                };
                sqlCommand.Parameters.Add(sqlParameter);

                sqlCommand.ExecuteNonQuery();
            }

        }

        //Actualizar Concepto del SAT
        public void updateConceptoSAT(int idConceptoSAT, string codigo, string descripcion, int naturaleza)
        {
            string strQuery = "UPDATE [SESSipConceptosSAT] SET codigo = @codigo " +
                ", descripcion = @descripcion, naturaleza = @naturaleza " +
                "WHERE idConceptoSAT = @idConceptoSAT ";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idConceptoSAT",
                    Value = idConceptoSAT
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@codigo",
                    Value = codigo
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@descripcion",
                    Value = descripcion
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@naturaleza",
                    Value = naturaleza
                };
                sqlCommand.Parameters.Add(sqlParameter);

                sqlCommand.ExecuteNonQuery();
            }

        }

        //Actualizar la fecha de aplicacion (fecha de pago) siempre y cuando no esté ya timbrado.
        public void updateDateFromNomina(int PortalId, string strIdNomina, DateTime dtFecha)
        {
            {
                string strQuery = "UPDATE SESSipNominaArchivoD SET FechaDeAplicacion = @fecha WHERE substring(idNomina, 1, 7) = @idNomina and idEmpresa = @idEmpresa " +
                    "and idnomina not in " +
                    "(SELECT distinct idNomina FROM SESSipNominaArchivoD " +
                    "WHERE substring(idNomina, 1, 7) = @idNomina and idEmpresa = @idEmpresa " +
                    "and (ArchivoXMLTimbrado <> '' or ArchivoXMLTimbrado is not null))";

                using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
                {
                    SqlParameter sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@idEmpresa",
                        Value = PortalId
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@idNomina",
                        Value = strIdNomina
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@fecha",
                        Value = dtFecha
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            {
                DataSet ds = new DataSet();
                string strQuery = "SESSipActualizaDatosDePagoConNominaAnterior";

                using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@idEmpresa",
                        Value = PortalId
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@tipo",
                        Value = strIdNomina.Substring(0, 1)
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@numero",
                        Value = strIdNomina.Substring(1, 2)
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@anio",
                        Value = strIdNomina.Substring(3, 4)
                    };
                    sqlCommand.Parameters.Add(sqlParameter);

                    sqlCommand.ExecuteNonQuery();
                }
            }

        }

        public void updateDateFromNomina(int PortalId, string strIdNomina, string strNomina = "", DateTime? dtFecha = null)
        {
            string strQuery = "UPDATE SESSipNominaArchivoD " +
                "SET FechaDeAplicacion = @fecha " +
                "WHERE substring(idNomina, 1, 7) = @idNomina and idEmpresa = @idEmpresa ";
            if (strNomina != "")
            {
                strQuery += " and Nomina=@Nomina ";
            }
            strQuery += "and idnomina not in " +
                "(SELECT distinct idNomina FROM SESSipNominaArchivoD " +
                "WHERE substring(idNomina, 1, 7) = @idNomina and idEmpresa = @idEmpresa " +
                "and(ArchivoXMLTimbrado <> '' or ArchivoXMLTimbrado is not null))";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idEmpresa",
                    Value = PortalId
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idNomina",
                    Value = strIdNomina
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@Nomina",
                    Value = strNomina
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@fecha"
                };
                if (dtFecha == null)
                    sqlParameter.Value = DBNull.Value;
                else
                    sqlParameter.Value = dtFecha;
                sqlCommand.Parameters.Add(sqlParameter);
                sqlCommand.ExecuteNonQuery();
            }

        }

        public void updateTipoDePagoFromNomina(int PortalId, string strIdNomina, string strNomina = "", int intIdFormaDePago = -1, string strFormaDePagoExtra = "")
        {
            string strQuery = "UPDATE SESSipNominaArchivoD " +
                "SET idFormaDePago = @idFormaDePago, FormaDePagoDatoExtra = @FormaDePagoDatoExtra " +
                "WHERE substring(idNomina, 1, 7) = @idNomina and idEmpresa = @idEmpresa ";
            if (strNomina != "")
            {
                strQuery += " and Nomina=@Nomina ";
            }
            strQuery += "and idnomina not in " +
                "(SELECT distinct idNomina FROM SESSipNominaArchivoD " +
                "WHERE substring(idNomina, 1, 7) = @idNomina and idEmpresa = @idEmpresa " +
                "and(ArchivoXMLTimbrado <> '' or ArchivoXMLTimbrado is not null))";
            using (SqlCommand sqlCommand = new SqlCommand(strQuery, sqlConnection))
            {
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idEmpresa",
                    Value = PortalId
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idNomina",
                    Value = strIdNomina
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@Nomina",
                    Value = strNomina
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@idFormaDePago"
                };
                if (intIdFormaDePago < 0)
                    sqlParameter.Value = DBNull.Value;
                else
                    sqlParameter.Value = intIdFormaDePago;
                sqlCommand.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter()
                {
                    ParameterName = "@FormaDePagoDatoExtra",
                    Value = strFormaDePagoExtra
                };
                sqlCommand.Parameters.Add(sqlParameter);
                sqlCommand.ExecuteNonQuery();
            }

        }

    }
}