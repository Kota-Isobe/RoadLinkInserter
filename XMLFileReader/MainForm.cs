using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace XMLFileReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            insertdb = new LinkDataGetter(this.sqlConnection1, this);
            label_state.Text = "";
            label1.Text = "";
        }

        private LinkDataGetter insertdb;
        private string[] filefullnames;//選択したファイルの絶対パス
        private string[] filenames;

        public string Label_state//現在の状態を表示するlabelのtextを変更
        {
            set { label_state.Text = value; }
        }

        private void ToolStripMenuItem_selectfile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {

                openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//デスクトップを初期表示
                //openFileDialog1.InitialDirectory = @"C:\";//Cドライブを初期表示
                openFileDialog1.Filter = "datファイル|*.xml|すべてのファイル|*.*";
                openFileDialog1.RestoreDirectory = true;//カレントディレクトリをプログラムのあるフォルダに戻す
                openFileDialog1.Multiselect = true;

                //開くボタンを押したとき
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filefullnames = new string[openFileDialog1.FileNames.Length];
                    filenames = new string[openFileDialog1.FileNames.Length];
                    int count = 0;

                    foreach (string fn in openFileDialog1.SafeFileNames)
                    {
                        filenames[count] = fn;
                        count++;
                    }

                    count = 0;

                    foreach (string fn in openFileDialog1.FileNames)
                    {
                        filefullnames[count] = fn;
                        count++;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = 0;
            string mesh = null;
            string lowerCorner = null;
            string upperCorner = null;
            string high = null;
            string tupleList = null;

            double LowerLatitude;
            double UpperLatitude;
            double LowerLongitude;
            double UpperLongitude;

            int maxX = 0;
            int maxY = 0;  

            foreach (string filefullname in filefullnames)
            {
                DataTable dt = new DataTable();
                //dt.Columns.Add(new DataColumn("MESH_ID"));
                dt.Columns.Add(new DataColumn("LOWER_LATITUDE"));
                dt.Columns.Add(new DataColumn("LOWER_LONGITUDE"));
                dt.Columns.Add(new DataColumn("UPPER_LATITUDE"));
                dt.Columns.Add(new DataColumn("UPPER_LONGITUDE"));
                dt.Columns.Add(new DataColumn("ALTITUDE"));

                label1.Text = count.ToString() + "/" + filefullnames.Length + "の挿入が完了しました";
                label_state.Text = filenames[count] + "を読み込み中";
                Application.DoEvents();

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                XmlReader reader = XmlReader.Create(filefullnames[count], settings);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName.Equals("mesh"))
                        {
                            mesh = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("lowerCorner"))
                        {
                            lowerCorner = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("upperCorner"))
                        {
                            upperCorner = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("high"))
                        {
                            high = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("tupleList"))
                        {
                            tupleList = reader.ReadString();
                        }
                    }
                }

                reader.Close();

                string[] split = lowerCorner.Split(new char[] { ' ' });

                LowerLatitude = double.Parse(split[0]);
                LowerLongitude = double.Parse(split[1]);

                split = upperCorner.Split(new char[] { ' ' });

                UpperLatitude = double.Parse(split[0]);
                UpperLongitude = double.Parse(split[1]);

                split = high.Split(new char[] { ' ' });

                maxX = Int32.Parse(split[0]);
                maxY = Int32.Parse(split[1]);

                split = tupleList.Split(new char[] { '\n' });

                string[] altitude;
                double delta = 1.0 / 9000;
                delta = Math.Round(delta, 10, MidpointRounding.AwayFromZero);

                double lowerlati = UpperLatitude;
                double lowerlongi = LowerLongitude;
                double upperlati = UpperLatitude;
                double upperlongi = LowerLongitude;
                int x = 0;
                int num = 0;

                lowerlati = upperlati - delta;

                for (int i = 0; i < split.Length; i++)
                {
                    if (!(split[i].Equals("") || split[i].Equals(" ")))
                    {
                        if (x == maxX + 1)
                        {
                            upperlati = lowerlati;
                            lowerlati = upperlati - delta;
                            lowerlongi = LowerLongitude;
                            x = 0;
                        }

                        upperlongi = lowerlongi + delta;
                        altitude = split[i].Split(new char[] { ',' });

                        if (altitude[1] != "-9999.00")
                        {
                            DataRow dr = dt.NewRow();
                            //dr["MESH_ID"] = mesh;
                            dr["LOWER_LATITUDE"] = Math.Round(lowerlati, 5, MidpointRounding.AwayFromZero);
                            dr["LOWER_LONGITUDE"] = Math.Round(lowerlongi, 5, MidpointRounding.AwayFromZero);
                            dr["UPPER_LATITUDE"] = Math.Round(upperlati, 5, MidpointRounding.AwayFromZero);
                            dr["UPPER_LONGITUDE"] = Math.Round(upperlongi, 5, MidpointRounding.AwayFromZero);
                            dr["ALTITUDE"] = altitude[1];

                            dt.Rows.Add(dr);
                        }
                        lowerlongi = upperlongi;

                        num++;
                        x++;
                    }
                }

                insertdb.InsertAltitudeData(dt);
                //DataTable_to_csv(dt);//チェック用

                count++;
            }

            label1.Text = count.ToString() + "/" + filefullnames.Length + "の挿入が完了しました";
            label_state.Text = "";
        }

        #region DataTableからcsvファイルを作成
        private void DataTable_to_csv(DataTable dt)
        {

            int rows = dt.Rows.Count;
            int cols = dt.Columns.Count;

            try
            {
                using (StreamWriter writer = new StreamWriter(@"testfile.csv"))
                {
                    label_state.Text = "csvファイル書き込み中";
                    Application.DoEvents();

                    //属性名を出力
                    for (int i = 0; i < cols; i++)
                    {
                        if (i != cols - 1)
                        {
                            writer.Write(dt.Columns[i].ColumnName + ",");
                        }
                        else
                        {
                            writer.WriteLine(dt.Columns[i].ColumnName);
                        }
                    }

                    //レコードを出力
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (j != cols - 1)
                            {
                                writer.Write(dt.Rows[i][j].ToString() + ",");
                            }
                            else
                            {
                                writer.WriteLine(dt.Rows[i][j].ToString());
                            }
                        }
                    }

                    writer.Close();

                    //label_state.Text = "終了";
                }
            }
            catch (IOException)
            {
                MessageBox.Show("ファイルが開けません");
            }
        }
        #endregion
    }
}
