using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Dbconnection
    {
        public SqlConnection con = new SqlConnection("Data Source=10.10.10.46;Initial Catalog=DonBest;User ID=luisma;Password=12345678");
        public SqlConnection con3 = new SqlConnection("Data Source=10.10.10.30;Initial Catalog=DGSDATA;User ID=sportbookdba;Password=lumalu");

        public SqlConnection getConn()
        {
            if (con.State == ConnectionState.Closed){ con.Open();}  return con;          
        }

        public SqlConnection getConn3()
        {
            if (con3.State == ConnectionState.Closed){ con3.Open();} return con3;       
        }

        public int ExeNonQuery(SqlCommand cmd)
        {
            cmd.Connection = getConn();
            int rowsaffected = -1;
            try { rowsaffected = cmd.ExecuteNonQuery(); } catch (Exception) { }
            con.Close();
            return rowsaffected;
        }

        public string ExeNonQueryWithResult(string query)
        {
           
            using (con)
            using (SqlCommand cmd = con.CreateCommand())
            {
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("SeqName", "SeqNameValue");
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    var result = returnParameter.Value;

                return (result.ToString());
            }
        }

        public object ExeScalar(string insertString)
        {
            SqlCommand cmd = new SqlCommand(insertString, con);
            cmd.Connection = getConn();
            object obj = -1;
            obj = cmd.ExecuteScalar();
            con.Close();
            return obj;
        }

        public DataTable ExeReader(string query)
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = getConn();
            SqlDataReader sdr;
            DataTable dt = new DataTable();
            sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            return dt;
        }

        public DataTable ExeReaderWithReturnValue(string query)
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = getConn();
            SqlDataReader sdr;
            DataTable dt = new DataTable();
            var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;
            sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            return dt;
        }


        public DataTable ExeSPWithResults(string storedProcedureName, IDictionary<string, string> parametersDictionary)
        {

            using (con)
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, string> entry in parametersDictionary)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }

                    con.Open();

                    SqlDataReader sdr;
                    DataTable dt = new DataTable();

                    sdr = cmd.ExecuteReader();
                    cmd.Connection = con;
                    dt.Load(sdr);
                    con.Close();
                    return dt;
                }
            }
        }


        public DataTable ExeSPWithResultsdb2(string storedProcedureName, IDictionary<string, string> parametersDictionary)
        {
            using (con3)
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, con3))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, string> entry in parametersDictionary)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }

                    con3.Open();
                    SqlDataReader sdr;
                    DataTable dt = new DataTable();
                    sdr = cmd.ExecuteReader();
                    cmd.Connection = con3;
                    dt.Load(sdr);
                    con3.Close();
                    return dt;
                }
            }
        }
    }
}
