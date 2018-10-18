using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Connection
{
    public class csConnection
    {
        public string CadenaConexion()
        {
            try
            {
                return ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Error to connect the database" + ex.Message.ToString());
            }
        }


        /// <summary>
        /// Permite ejecutar un procedimiento almacenado sin parametros de entrada
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento almacenado a ejecutar</param>
        /// <returns>DataSet con el resulado del proceso ejecutado</returns>
        public DataSet ExecutePA(String nombrePA)
        {
            SqlConnection sqlCon = new SqlConnection(CadenaConexion());
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(nombrePA, sqlCon);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlCon.Open();
                adapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception("Error to execute the process." + ex.Message.ToString());
            }
            finally
            {
                sqlCon.Close();
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que recibe paramtros de entrada
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento almacenado</param>
        /// <param name="parametros">Parametros de entrada para el procedimiento almacenado</param>
        /// <returns>DataSet con el resulado del proceso ejecutado</returns>
        public DataSet ExecutePA(String nombrePA, Hashtable parametros)
        {
            SqlConnection sqlCon = new SqlConnection(CadenaConexion());
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(nombrePA, sqlCon);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                //cargo los parametros
                IDictionaryEnumerator paramsPA = parametros.GetEnumerator();
                while (paramsPA.MoveNext())
                {
                    adapter.SelectCommand.Parameters.AddWithValue(paramsPA.Key.ToString(), paramsPA.Value);
                }
                sqlCon.Open();
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error to execute the process." + ex.Message.ToString());
            }
            finally
            {
                sqlCon.Close();
            }
            return ds;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado e indica si se ejecuta o no
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento almacenado</param>
        /// <param name="parametros">Parametros de entrada para el procedimiento almacenado</param>
        /// <returns>Valor boleano que indica la ejecución exitosa o no del procedimiento almacenado</returns>
        public Boolean ExecutePAConfimation(String nombrePA, Hashtable parametros)
        {
            SqlConnection sqlCon = new SqlConnection(CadenaConexion());
            DataSet ds = new DataSet();
            Boolean retorno;
            try
            {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm = sqlCon.CreateCommand();
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.CommandText = nombrePA;
                //cargo los parametros
                IDictionaryEnumerator paramsPA = parametros.GetEnumerator();
                while (paramsPA.MoveNext())
                {
                    sqlComm.Parameters.AddWithValue(paramsPA.Key.ToString(), paramsPA.Value);
                }
                sqlCon.Open();
                sqlComm.ExecuteNonQuery();
                retorno = true;
            }
            catch (Exception ex)
            {
                retorno = false;
                throw new Exception("Error to execute the process." + ex.Message.ToString());
            }
            finally
            {
                sqlCon.Close();
            }
            return retorno;
        }
    }
}
