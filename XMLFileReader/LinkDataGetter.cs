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

        public void InsertLinkData(DataTable dt)
        {
            mainform.Label_state = "LINKS_GSI20_RAWに挿入中";
            Application.DoEvents();

            try
            {
                sqlConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    bulkCopy.DestinationTableName = "LINKS_GSI20_RAW";

                    bulkCopy.WriteToServer(dt);
                }

            }
            finally
            {
                sqlConnection.Close();
            }
            mainform.Label_state = "挿入終了";
        }

        public void LinksDataGetter(DataTable dt)
        {
            mainform.Label_state = "LINKS_GSI20_RAWに挿入中";
            Application.DoEvents();

            try
            {
                sqlConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    bulkCopy.DestinationTableName = "LINKS_GSI20_RAW";

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
