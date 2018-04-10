using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

public class SQLManager
{
    public static DataSet RunStoredProcedure(string spName, params SqlParameter[] parameters)
    {
        DataSet dataSet = null;
        SqlCommand sqlCommand;
        SqlDataAdapter sqlDataAdapter;

        //Epsilon:
        //string connectionString = "Data Source=148.234.37.144;Initial Catalog=CAAF;User ID=dlepe;Password=fsw-123";
        
        //Lambda:
        string connectionString = "Data Source=148.234.37.143;Initial Catalog=CAAF;User ID=fsw-cajero;Password=cajeroAutomatico2017";
        //string connectionString = "Data Source=148.234.37.144;Initial Catalog=CashierServer;User ID=ihernandez;Password=fsw-123";
        
        SqlConnection conn = new SqlConnection(connectionString);

        try
        {
            conn.Open();
            sqlCommand = new SqlCommand(spName, conn);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter sqlParam in parameters)
            {
                sqlCommand.Parameters.Add(sqlParam);
            }

            dataSet = new DataSet();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);            
            sqlDataAdapter.Fill(dataSet);
            conn.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dataSet = null;
        }
        
        return dataSet;
    }
}