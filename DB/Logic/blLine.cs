using DB.Connection;
using DB.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Logic
{
    public class blLine : csComponentsConnection
    {
        public csLine GetLastDate()
        {
            csLine data = null;

            try
            {
                dataset = connection.ExecutePA("[dbo].[spGetValueLastDate]");

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow fila in dataset.Tables[0].Rows)
                    {
                        data = new csLine(Convert.ToString(fila[0]), Convert.ToInt32(fila[1]), Convert.ToInt32(fila[2]));
                    }
                }
                else
                {
                    data = null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return data;

        }
    }
}
