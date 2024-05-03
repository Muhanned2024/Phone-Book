﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookPhone
{
    public partial class Form2 : Form
    {
         SqlConnection con = new SqlConnection(@"Data Source = (local)\SQLMUHANNED;Initial Catalog=phone_list;Integrated Security=True");
      //   SqlConnection con = new SqlConnection("Server = 10.0.0.62; Database =phone_list;Integrated Security=True");
        public void NewFont()
        {
            InitializeComponent();
            Font coustomfont = new Font("Omar", 22);
            label1.Font = coustomfont;
            label2.Font = coustomfont;
            label3.Font = coustomfont;
            label4.Font = coustomfont;
            label5.Font = coustomfont;
            label6.Font = coustomfont;
            
        }

        //Fields
        private int borderRadius = 20;
        private int borderSize = 2;
        private Color borderColor = Color.FromArgb(128, 128, 255);
        //Constructor
        public Form2()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderSize);
            this.panelTitleBar.BackColor = borderColor;
            this.BackColor = borderColor;
        }

        //Drag Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        void show()
        {
            
                con.Open();

                SqlCommand cm2 = new SqlCommand("select * from directorylist", con);

                SqlDataReader dr2 = cm2.ExecuteReader();

                

                while (dr2.Read())
                {
                    comboBox2.Items.Add(dr2.GetString(4));



                }
                con.Close();



        }

        void show2()
        {
            con.Open();
            SqlCommand cm = new SqlCommand("select distinct mangment from directorylist", con);

            SqlDataReader dr = cm.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["mangment"].ToString());
            }
            con.Close();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- Minimize borderless form from taskbar
                return cp;
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void FormRegionAndBorder(Form form, float radius, Graphics graph, Color borderColor, float borderSize)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                using (GraphicsPath roundPath = GetRoundedPath(form.ClientRectangle, radius))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                using (Matrix transform = new Matrix())
                {
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    form.Region = new Region(roundPath);
                    if (borderSize >= 1)
                    {
                        Rectangle rect = form.ClientRectangle;
                        float scaleX = 1.0F - ((borderSize + 1) / rect.Width);
                        float scaleY = 1.0F - ((borderSize + 1) / rect.Height);
                        transform.Scale(scaleX, scaleY);
                        transform.Translate(borderSize / 1.6F, borderSize / 1.6F);
                        graph.Transform = transform;
                        graph.DrawPath(penBorder, roundPath);
                    }
                }
            }
        }
        private void ControlRegionAndBorder(Control control, float radius, Graphics graph, Color borderColor)
        {
            using (GraphicsPath roundPath = GetRoundedPath(control.ClientRectangle, radius))
            using (Pen penBorder = new Pen(borderColor, 1))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                control.Region = new Region(roundPath);
                graph.DrawPath(penBorder, roundPath);
            }
        }

        private void DrawPath(Rectangle rect, Graphics graph, Color color)
        {
            using (GraphicsPath roundPath = GetRoundedPath(rect, borderRadius))
            using (Pen penBorder = new Pen(color, 3))
            {
                graph.DrawPath(penBorder, roundPath);
            }
        }

        private struct FormBoundsColors
        {
            public Color TopLeftColor;
            public Color TopRightColor;
            public Color BottomLeftColor;
            public Color BottomRightColor;
        }
        private FormBoundsColors GetFormBoundsColors()
        {
            var fbColor = new FormBoundsColors();
            using (var bmp = new Bitmap(1, 1))
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle rectBmp = new Rectangle(0, 0, 1, 1);

                //Top Left
                rectBmp.X = this.Bounds.X - 1;
                rectBmp.Y = this.Bounds.Y;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.TopLeftColor = bmp.GetPixel(0, 0);

                //Top Right
                rectBmp.X = this.Bounds.Right;
                rectBmp.Y = this.Bounds.Y;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.TopRightColor = bmp.GetPixel(0, 0);

                //Bottom Left
                rectBmp.X = this.Bounds.X;
                rectBmp.Y = this.Bounds.Bottom;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.BottomLeftColor = bmp.GetPixel(0, 0);

                //Bottom Right
                rectBmp.X = this.Bounds.Right;
                rectBmp.Y = this.Bounds.Bottom;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.BottomRightColor = bmp.GetPixel(0, 0);
            }
            return fbColor;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //-> SMOOTH OUTER BORDER
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rectForm = this.ClientRectangle;
            int mWidht = rectForm.Width / 2;
            int mHeight = rectForm.Height / 2;
            var fbColors = GetFormBoundsColors();

            //Top Left
            DrawPath(rectForm, e.Graphics, fbColors.TopLeftColor);

            //Top Right
            Rectangle rectTopRight = new Rectangle(mWidht, rectForm.Y, mWidht, mHeight);
            DrawPath(rectTopRight, e.Graphics, fbColors.TopRightColor);

            //Bottom Left
            Rectangle rectBottomLeft = new Rectangle(rectForm.X, rectForm.X + mHeight, mWidht, mHeight);
            DrawPath(rectBottomLeft, e.Graphics, fbColors.BottomLeftColor);

            //Bottom Right
            Rectangle rectBottomRight = new Rectangle(mWidht, rectForm.Y + mHeight, mWidht, mHeight);
            DrawPath(rectBottomRight, e.Graphics, fbColors.BottomRightColor);

            //-> SET ROUNDED REGION AND BORDER
            FormRegionAndBorder(this, borderRadius, e.Graphics, borderColor, borderSize);
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {
            ControlRegionAndBorder(panelContainer, borderRadius - (borderSize / 2), e.Graphics, borderColor);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Invalidate();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            show();
            
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panelContainer_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void meButton1_Click(object sender, EventArgs e)
        {
            try
            {

                con.Open();

                SqlCommand cm = new SqlCommand("insert into directorylist (name_english,phone,office,mangment,name_arabic) values ( @ne, @phone, @office, @mang, @na)", con);

                cm.Parameters.AddWithValue("@ne", textBox3.Text);
                cm.Parameters.AddWithValue("@phone", textBox6.Text);
                cm.Parameters.AddWithValue("@office", textBox2.Text);
                cm.Parameters.AddWithValue("@mang", comboBox1.Text.ToString());
                cm.Parameters.AddWithValue("@na", comboBox2.Text.ToString());
                
                

                cm.ExecuteNonQuery();

                MessageBox.Show("تمت الإضافة بنجاح");

                con.Close();
            }
            catch (Exception v)
            {
                con.Close();
                MessageBox.Show("فشلت العملية");
                //TimeSpan ts = new TimeSpan(0, 0, 10);
            }
        }

        private void meButton2_Click(object sender, EventArgs e)
        {
            try
            {

                con.Open();

                SqlCommand cm = new SqlCommand("UPDATE [directorylist] SET [name_english]= @ne,[phone]= @phone,[office]= @office,[mangment]= @mang where [name_arabic]= @na", con);

                cm.Parameters.AddWithValue("@na", comboBox2.Text.ToString());
                cm.Parameters.AddWithValue("@ne", textBox3.Text);
                cm.Parameters.AddWithValue("@phone", textBox6.Text);
                cm.Parameters.AddWithValue("@office", textBox2.Text);
                cm.Parameters.AddWithValue("@mang", comboBox1.Text);
                

                cm.ExecuteNonQuery();

                MessageBox.Show("تم التعديل بنجاح");

                con.Close();
            }
            catch (Exception v)
            {
                con.Close();
                MessageBox.Show("فشلت العملية");
                //TimeSpan ts = new TimeSpan(0, 0, 10);
            }
        }

        private void meButton3_Click(object sender, EventArgs e)
        {
            try
            {

                con.Open();

                SqlCommand cm = new SqlCommand("DELETE FROM directorylist WHERE name_arabic = @na", con);

                cm.Parameters.AddWithValue("@na", comboBox2.Text.ToString());

                cm.ExecuteNonQuery();

                MessageBox.Show("تم الحذف بنجاح");

                con.Close();
            }
            catch (Exception v)
            {
                con.Close();
                MessageBox.Show("فشلت العملية");
                //TimeSpan ts = new TimeSpan(0, 0, 10);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cm = new SqlCommand("select * from directorylist where mangment=@mang", con);

                cm.Parameters.AddWithValue("@mang", comboBox1.Text.ToString());

                SqlDataReader dr = cm.ExecuteReader();

                while (dr.Read())
                {
                    textBox2.Text = dr.GetString(2);

                }
                con.Close();
            }
            catch (Exception a)
            {
                con.Close();
                MessageBox.Show(a.Message);

            }
        }

        private void meButton4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 fm1 = new Form1();
            fm1.Show();
        }

        private void panelTitleBar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                con.Open();

                SqlCommand cm;

                cm = new SqlCommand("select * from directorylist where name_arabic=@ar", con);
                cm.Parameters.AddWithValue("@ar", comboBox2.Text);

                SqlDataReader dr = cm.ExecuteReader();

                while (dr.Read())
                {

                    comboBox1.Text = dr.GetString(3).ToString();
                    textBox2.Text = dr.GetString(2);
                    textBox3.Text = dr.GetString(0);
                    textBox6.Text = dr.GetString(1);

                }
                con.Close();

            }
            catch (Exception r)
            {
                con.Close();
                MessageBox.Show(r.Message);
            }
        }
    }
}
