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
            string id = null;
            string posList = null;
            string type = null;
            string rdCtg = null;
            string state = null;
            string rnkWidth = null;
            string tollSect = null;

            foreach (string filefullname in filefullnames)
            {
                label1.Text = count.ToString() + "/" + filefullnames.Length + "の挿入が完了しました";
                label_state.Text = filenames[count] + "を読み込み中";
                Application.DoEvents();

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("LINK_ID"));
                dt.Columns.Add(new DataColumn("LATITUDE"));
                dt.Columns.Add(new DataColumn("LONGITUDE"));
                dt.Columns.Add(new DataColumn("ROAD_TYPE"));
                dt.Columns.Add(new DataColumn("ROAD_CATEGORY"));
                dt.Columns.Add(new DataColumn("ROAD_STATE"));
                dt.Columns.Add(new DataColumn("ROAD_WIDTH"));
                dt.Columns.Add(new DataColumn("TOLL"));

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                XmlReader reader = XmlReader.Create(filefullnames[count], settings);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName.Equals("RdCL"))
                        {
                            id = null;
                            posList = null;
                            type = null;
                            rdCtg = null;
                            state = null;
                            rnkWidth = null;
                            tollSect = null;
                        }

                        if (reader.LocalName.Equals("rID")) //タイプ名を指定
                        {
                            id = reader.ReadString();     //一致したタイプ名の中の値を取得
                        }

                        if (reader.LocalName.Equals("posList"))
                        {
                            posList = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("type"))
                        {
                            type = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("rdCtg"))
                        {
                            rdCtg = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("state"))
                        {
                            state = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("rnkWidth"))
                        {
                            rnkWidth = reader.ReadString();
                        }

                        if (reader.LocalName.Equals("tollSect"))
                        {
                            tollSect = reader.ReadString();
                            reader.Close();
                        }
                    }
                }


                if (posList != null)
                {
                    String[] split = posList.Split(new char[] { '\n' });    //座標リストは複数行に渡るので、改行の度にsplitして配列に保存
                    for (int i = 0; i < split.Length; i++)                  //座標リストの行数だけインサートを繰り返す
                    {
                        if (!(split[i].Equals("") || split[i].Equals(" ")))
                        {
                            String[] split2 = split[i].Split(new char[] { ' ' });   //座標リストは"緯度 経度"の形になっているので、空白でsplit
                            float latitude = float.Parse(split2[0]);
                            float longitude = float.Parse(split2[1]);

                            DataRow dr = dt.NewRow();

                            dr["LINK_ID"] = id;
                            dr["LATITUDE"] = latitude;
                            dr["LONGITUDE"] = longitude;
                            dr["ROAD_TYPE"] = type;
                            dr["ROAD_CATEGORY"] = rdCtg;
                            dr["ROAD_STATE"] = state;
                            dr["ROAD_WIDTH"] = rnkWidth;
                            dr["TOLL"] = tollSect;
                            dt.Rows.Add(dr);
                            insertdb.InsertLinkData(dt);
                        }
                    }

                    count++;
                }
                label1.Text = count.ToString() + "/" + filefullnames.Length + "の挿入が完了しました";
                label_state.Text = "";
            }
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