using SMS_Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMS_BL;
using SMS_DL;
using System.IO;
using System.Diagnostics;

namespace SMS.Inventory
{
    public partial class frm0310I :  SMS_Base
    {
        Login_Info loginInfo;
        PETC0310I_DL pdl = new PETC0310I_DL();
        public frm0310I(Login_Info loginInfo)
            : base(loginInfo)
        {
            InitializeComponent();
            this.loginInfo = loginInfo;
        }
        private void PETC0310I_Load(object sender, EventArgs e)
        {
            lblMode.Visible = false;
            FormName = "在庫情報反映";
            F12_Text = "作成(F12)";
            FunctionButtonDisabled(2);
            FunctionButtonDisabled(3);
            FunctionButtonDisabled(4);
            FunctionButtonDisabled(5);
            FunctionButtonDisabled(6);
            FunctionButtonDisabled(7);
            FunctionButtonDisabled(8);
            FunctionButtonDisabled(9);
            FunctionButtonDisabled(10);
            FunctionButtonDisabled(11);
        }

        public override void functionClick(int btnIndex)
        {
            switch (btnIndex)
            {
                case 1:
                    break;

                case 11:
                 //   F11();
                    break;

                case 12:
                    F12();
                    break;
            }
        }
        protected void F12()
        {
            DialogResult dialogResult = MessageBox.Show("在庫データCSVを作成しますか？", ""  , MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor; ;
                var ds = pdl.PETC310I_Select();
                if (ds.Tables[0].Rows.Count == 0 && ds.Tables[1].Rows.Count == 0)
                {
                    MessageBox.Show("対象データはありません");
                   // return;
                }
                else
                {
                    var path = @"C:\SMS\PETC0310I\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    try
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (File.Exists(path + "\\" + "select.CSV"))
                            {
                                File.Delete(path + "\\" + "select.CSV");
                            }
                            ToCSV(ds.Tables[0], path + "\\" + "select.CSV");// For Rakuten
                        }
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            if (File.Exists(path + "\\" + "quantity.CSV"))
                            {
                                File.Delete(path + "\\" + "quantity.CSV");
                            }
                            ToCSV(ds.Tables[1], path + "\\" + "quantity.CSV");// For Yahoo
                        }
                        MessageBox.Show("在庫データCSVを作成しました");
                        Process.Start(Path.GetDirectoryName(path));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                }
                Cursor = Cursors.Arrow;
                //do something
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
                //do something else
            }
           
           

        }
        public static void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.GetEncoding(932));
            //headers  
            //for (int i = 0; i < dtDataTable.Columns.Count; i++)
            //{
            //    sw.Write(dtDataTable.Columns[i]);
            //    if (i < dtDataTable.Columns.Count - 1)
            //    {
            //        sw.Write(",");
            //    }
            //}
            //sw.Write(sw.NewLine);
            foreach (DataColumn column in dtDataTable.Columns)
            {
                var value = String.Format("\"{0}\"", column.ColumnName);
                sw.Write(value);
                if (column.Ordinal < dtDataTable.Columns.Count)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        //if (value.Contains(','))
                        //{

                        value = String.Format("\"{0}\"", value);
                        sw.Write(value);

                        //}
                        //else
                        //{
                        //   sw.Write(dr[i].ToString());
                        //}
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
