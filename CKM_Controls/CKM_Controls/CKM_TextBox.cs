﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;


namespace CKM_Controls
{
    public class CKM_TextBox : TextBox
    {
        [Browsable(true)]
        [Category("Extended Properties")]
        [Description("Textbox Type")]
        [DisplayName("CKM_Type")]
        public Type CKM_Type { get; set; }
        public enum Type
        {
            Normal = 0,
            Date = 1,
            Integer = 2,
            Price = 3,
            Hiragana = 4
            //IntegerOnly = 1,
            //Decimal = 3,
            //Date = 4
        }


        [Browsable(true)]
        [Category("Extended Properties")]
        [Description("To Check Required Field On Enter KeyDown")]
        [DisplayName("CKM_IsRequired")]
        public bool CKM_Reqired { get; set; }

        //bool CKM_Reqired = true;
        //[Browsable(true)]
        //[Category("Extended Properties")]
        //[Description("To Check Required Field On Enter KeyDown")]
        //[DisplayName("CKM_IsRequired")]
        //public bool CKM_Reqired 
        //{
        //    get { return CKM_Reqired; }
        //    set { CKM_Reqired = value; }
        //}

        bool useThousandSeparator = true;
        [Browsable(true)]
        [Category("Extended Properties")]
        [Description("UseThousandSeparator")]
        [DisplayName("UseThousandSeparator")]
        public bool UseThousandSeparator
        {
            get { return useThousandSeparator; }
            set { useThousandSeparator = value; }
        }

        bool useMinus = true;
        [Browsable(true)]
        [Category("Extended Properties")]
        [Description("UseMinus")]
        [DisplayName("UseMinus")]
        public bool UseMinus
        {
            get { return useMinus; }
            set { useMinus = value; }
        }

        //int decimalPlace = 0;
        [Browsable(true)]
        [Category("Extended Properties")]
        [Description("DecimalPlace")]
        [DisplayName("DecimalPlace")]
        public int DecimalPlace { get; set; }

        public CKM_TextBox()
        {
            this.Enter += CKM_TextBox_Enter;
            this.Leave += CKM_TextBox_Leave;
            this.KeyDown += CKM_TextBox_KeyDown;
            this.KeyPress += CKM_TextBox_KeyPress;
            this.TextChanged += CKM_TextBox_TextChanged;
            this.Validating += CKM_TextBox_Validating;
        }

        private void CKM_TextBox_Validating(object sender, CancelEventArgs e)
        {
            //throw new NotImplementedException();
            if (CKM_Type == Type.Price)
            {
                if (!string.IsNullOrWhiteSpace(this.Text))
                {
                    if (IsInteger(this.Text.Replace(",", "")) || IsDouble(this.Text.Replace(",", "")))
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                        MessageBox.Show("Plz,Enter Integer Numbers!", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (this.useMinus == true && this.Text.Contains("-"))
                    {
                        this.ForeColor = Color.Red;
                    }
                    else
                    {
                        this.ForeColor = DefaultForeColor;
                    }

                }
            }

            else if (CKM_Type == Type.Date)
            {
                if (!string.IsNullOrWhiteSpace(this.Text))
                {
                    if (IsInteger(this.Text.Replace("/", "")))
                    {
                        string day = string.Empty, month = string.Empty, year = string.Empty;
                        if (this.Text.Contains("/"))
                        {
                            string[] date = this.Text.Split('/');
                            day = date[date.Length - 1].PadLeft(2, '0');
                            month = date[date.Length - 2].PadLeft(2, '0');

                            if (date.Length > 2)
                                year = date[date.Length - 3];

                            this.Text = this.Text.Replace("/", "");
                        }

                        string text = this.Text;
                        text = text.PadLeft(8, '0');
                        day = text.Substring(text.Length - 2);
                        month = text.Substring(text.Length - 4).Substring(0, 2);
                        year = Convert.ToInt32(text.Substring(0, text.Length - 4)).ToString();

                        if (month == "00")
                        {
                            month = string.Empty;
                        }
                        if (year == "0")
                        {
                            year = string.Empty;
                        }

                        if (string.IsNullOrWhiteSpace(month))
                            month = DateTime.Now.Month.ToString().PadLeft(2, '0');//if user doesn't input for month,set current month

                        if (string.IsNullOrWhiteSpace(year))
                        {
                            year = DateTime.Now.Year.ToString();//if user doesn't input for year,set current year
                        }
                        else
                        {
                            if (year.Length == 1)
                                year = "200" + year;
                            else if (year.Length == 2)
                                year = "20" + year;
                        }

                        string strdate = year + "/" + month + "/" + day;
                        if (CheckDate(strdate))
                        {
                            this.Text = strdate;
                        }
                        else
                        {
                            MessageBox.Show("Invalid Date", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Plz,Enter Integer Numbers!", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Focus();
                    }
                }
            }
        }

        public void CKM_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CKM_Type == Type.Date)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '/';

                if (e.KeyChar == '/' && this.Text.Count(f => f == '/') >= 2)
                    e.Handled = true;

                if (char.IsDigit(e.KeyChar))
                {
                    if (this.SelectionLength == 0)
                    {
                        if (this.Text.Replace("/", "").Count() == 8)
                            e.Handled = true;
                    }
                }
            }
            else if (CKM_Type == Type.Integer)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
            else if (CKM_Type == Type.Price)
            {

                if ((sender as TextBox).TextLength <= 8)
                {
                    if (this.useMinus == true)
                    {
                        if (!Char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '.')
                        {
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        if (!Char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
                        {
                            e.Handled = true;
                        }
                    }
                }

                else if ((sender as TextBox).TextLength >= 8)
                {
                    if ((sender as TextBox).Text.Contains(","))
                    {
                        var f = (sender as TextBox).Text.Replace(",", string.Empty);
                        if (f.Length < 8)
                        {
                            e.Handled = false;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        public void CKM_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (CKM_Type == Type.Date)
                {
                    if (!string.IsNullOrWhiteSpace(this.Text))
                    {
                        if (IsInteger(this.Text.Replace("/", "")))
                        {
                            string day = string.Empty, month = string.Empty, year = string.Empty;
                            if (this.Text.Contains("/"))
                            {
                                string[] date = this.Text.Split('/');
                                day = date[date.Length - 1].PadLeft(2, '0');
                                month = date[date.Length - 2].PadLeft(2, '0');

                                if (date.Length > 2)
                                    year = date[date.Length - 3];

                                this.Text = this.Text.Replace("/", "");
                            }

                            string text = this.Text;
                            text = text.PadLeft(8, '0');
                            day = text.Substring(text.Length - 2);
                            month = text.Substring(text.Length - 4).Substring(0, 2);
                            year = Convert.ToInt32(text.Substring(0, text.Length - 4)).ToString();

                            if (month == "00")
                            {
                                month = string.Empty;
                            }
                            if (year == "0")
                            {
                                year = string.Empty;
                            }


                            if (string.IsNullOrWhiteSpace(month))
                                month = DateTime.Now.Month.ToString().PadLeft(2, '0');//if user doesn't input for month,set current month

                            if (string.IsNullOrWhiteSpace(year))
                            {
                                year = DateTime.Now.Year.ToString();//if user doesn't input for year,set current year
                            }
                            else
                            {
                                if (year.Length == 1)
                                    year = "200" + year;
                                else if (year.Length == 2)
                                    year = "20" + year;
                            }

                            string strdate = year + "/" + month + "/" + day;
                            if (CheckDate(strdate))
                            {
                                this.Text = strdate;

                            }
                            else
                            {
                                MessageBox.Show("日付の入力が正しくありません。", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Plz,Enter Integer Numbers!", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Focus();
                        }

                    }
                    else if (string.IsNullOrWhiteSpace(this.Text) && CKM_Reqired == true)
                    {
                        MessageBox.Show("該当の値を入力してください。", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Focus();
                    }
                }
                else if (CKM_Type == Type.Normal)
                {
                    if (string.IsNullOrWhiteSpace(this.Text) && CKM_Reqired == true)
                    {
                        MessageBox.Show("該当の値を入力してください。", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Focus();
                    }
                }
                else if (CKM_Type == Type.Price)
                {
                    CultureInfo cultureInfo = new CultureInfo("en-US");
                    //localFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                    //CultureInfo cultureInfo = new CultureInfo("en-US");
                    // price = string.Format(cultureInfo, "{0:C}", myRate)
                    this.Text = string.Format(cultureInfo, "{0:C0}", Convert.ToInt32(this.Text.Replace(",", string.Empty))).Replace("$", string.Empty);

                }
                //else if (CKM_Type == Type.Price)
                //{
                //    if (!string.IsNullOrWhiteSpace(this.Text))
                //    {
                //        int distance;
                //        if (int.TryParse(this.Text, out distance))
                //        {
                //            this.BackColor = Color.Red;
                //        }

                //    }
                //}
            }
        }

        private void CKM_TextBox_Enter(object sender, EventArgs e)
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                if (lang.LayoutName.Equals("Japanese"))
                {
                    InputLanguage.CurrentInputLanguage = lang;
                    break;
                }
            }
            if (this.CKM_Type == Type.Hiragana)
                this.ImeMode = System.Windows.Forms.ImeMode.Hiragana;

            this.BackColor = Color.LightYellow;
            if (CKM_Type == Type.Date)
                this.TextAlign = HorizontalAlignment.Center;
            if (CKM_Type == Type.Integer)
                this.TextAlign = HorizontalAlignment.Right;
            if (CKM_Type == Type.Price)
                this.TextAlign = HorizontalAlignment.Right;
        }

        private void CKM_TextBox_Leave(object sender, EventArgs e)
        {
            if (this.CKM_Type == Type.Price && useThousandSeparator)
            {
                TextBox tb = sender as TextBox;
                string value = tb.Text.Replace(",", "");
                int num;
                double dou;
                this.DecimalPlace = 2;
                int a = Convert.ToInt32(this.DecimalPlace);

                if (Int32.TryParse(value, out num))
                {
                    tb.TextChanged -= CKM_TextBox_TextChanged;
                    if (!tb.Text.Equals("0"))
                        tb.Text = string.Format("{0:#,#}", num);
                    tb.SelectionStart = tb.Text.Length;
                    tb.TextChanged += CKM_TextBox_TextChanged;
                }
                else
                {
                    tb.Text = string.Format("{0:#,#}", value);

                    string[] p = tb.Text.Split('.');
                    if (a != p[1].Length)
                    {
                        MessageBox.Show("Decimal place must be " + a, "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Focus();
                    }
                    else
                    {
                        tb.Text = p[0].ToString();
                        if (Int32.TryParse(tb.Text, out num))
                        {
                            tb.TextChanged -= CKM_TextBox_TextChanged;
                            if (!tb.Text.Equals("0"))
                                tb.Text = string.Format("{0:#,#}", num);
                            this.Text = tb.Text + "." + p[1].ToString();
                            tb.SelectionStart = tb.Text.Length;
                            tb.TextChanged += CKM_TextBox_TextChanged;
                        }
                    }
                }               
            }
            this.BackColor = Color.White;
        }

        private bool CheckDate(string value)
        {
            DateTime d;
            return DateTime.TryParseExact(value,
                       "yyyy/MM/dd",
                       System.Globalization.CultureInfo.InvariantCulture,
                       DateTimeStyles.None,
                       out d);
        }

        private void CKM_TextBox_TextChanged(object sender, EventArgs e)
        {
            //if (this.CKM_Type == Type.Price && useThousandSeparator)
            //{
            //    TextBox tb = sender as TextBox;
            //    string value = tb.Text.Replace(",", "");
            //    //ulong ul;
            //    int Num;

            //    //if (ulong.TryParse(value, out ul))
            //    if (Int32.TryParse(value, out Num))
            //    {
            //        tb.TextChanged -= CKM_TextBox_TextChanged;
            //        if (!tb.Text.Equals("0"))
            //            tb.Text = string.Format("{0:#,#}", Num);
            //        tb.SelectionStart = tb.Text.Length;
            //        tb.TextChanged += CKM_TextBox_TextChanged;
            //    }
            //}
        }

        private bool IsInteger(string value)
        {
            int Num;
            if (Int32.TryParse(value, out Num))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsDouble(string value)
        {
            double dou;
            if (Double.TryParse(value, out dou))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}