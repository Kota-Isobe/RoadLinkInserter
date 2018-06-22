using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace XMLFileReader
{
    class LinkDataGetter
    {
        private SqlConnection sqlConnection;
        private MainForm mainform;

        public LinkDataGetter(SqlConnection connection, MainForm form1)
        {
            this.sqlConnection = connection;
            this.mainform = form1;
        }

        //未

        public void InsertAltitudeData(DataTable dt)
        {
            mainform.Label_state = "ALTITUDE_10M_MESH_testに挿入中";
            Application.DoEvents();

            try
            {
                sqlConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    bulkCopy.DestinationTableName = "ALTITUDE_10M_MESH_test";

                    bulkCopy.WriteToServer(dt);
                }

            }
            finally
            {
                sqlConnection.Close();
            }
            mainform.Label_state = "挿入終了";
        }

        public void AltitudeDataGetter(DataTable dt)
        {
            mainform.Label_state = "ALTITUDE_10M_MESH_testに挿入中";
            Application.DoEvents();

            try
            {
                sqlConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    bulkCopy.DestinationTableName = "ALTITUDE_10M_MESH_test";

                    bulkCopy.WriteToServer(dt);
                }

            }
            finally
            {
                sqlConnection.Close();
            }
            mainform.Label_state = "挿入終了";
        }
    }
}
