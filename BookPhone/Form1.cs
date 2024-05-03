using System;
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
    public partial class Form1 : Form
    {
         SqlConnection con = new SqlConnection(@"Data Source = (local)\SQLMUHANNED;Initial Catalog=phone_list;Integrated Security=True");
       //  SqlConnection con = new SqlConnection("Server=10.0.0.62;Database =phone_list;Integrated Security=True");
        


        public void NewFont()
        {
            InitializeComponent();
            Font coustomfont = new Font("Omar",22);
            label1.Font = coustomfont;
            label2.Font = coustomfont;
            label3.Font = coustomfont;
            label4.Font = coustomfont;
            label5.Font = coustomfont;
            label6.Font = coustomfont;
            radioButton1.Font = coustomfont;
            radioButton2.Font = coustomfont;
        }

        //Fields
        private int borderRadius = 20;
        private int borderSize = 2;
        private Color borderColor = Color.FromArgb(128, 128, 255);
        //Constructor
        public Form1()
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
            try
            {
                con.Open();

                SqlCommand cm2;
                
                    cm2 = new SqlCommand("select * from directorylist", con);

                    SqlDataReader dr2 = cm2.ExecuteReader();

                    comboBox1.Items.Clear();

                    while (dr2.Read())
                    {
                        comboBox1.Items.Add(dr2.GetString(4));
                    


                    }
                    con.Close();
                    
                

            }
            catch (Exception e)
            {
                con.Close();
                MessageBox.Show(e.Message);

            }
        }


        public void View2()
        {
            try
            {
                con.Open();

                SqlCommand cm2;

                if (radioButton1.Checked==true)
                {



                    cm2 = new SqlCommand("select * from directorylist", con);

                    SqlDataReader dr2 = cm2.ExecuteReader();

                    comboBox1.Items.Clear();
                        
                    while (dr2.Read())
                    {
                        comboBox1.Items.Add(dr2.GetString(4));
                     
                    }
                    con.Close();
                }


                else 
                {
                    


                    SqlCommand cm3 = new SqlCommand("select distinct mangment from directorylist", con);



                    SqlDataReader dr3 = cm3.ExecuteReader();

                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();

                    comboBox2.ResetText();

                    comboBox1.Items.Clear();

                    while (dr3.Read())
                    {
                        

                        comboBox1.Items.Add(dr3["mangment"].ToString());


                    }
                    
                    
                    
                }
                con.Close();

            }
            catch (Exception e)
            {
                con.Close();
                MessageBox.Show(e.Message);

            }
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

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void meButton1_Click(object sender, EventArgs e)
        {
            comboBox1.ResetText();
            View2();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
          
            try
            {
                con.Open();

                SqlCommand cm;

                if (radioButton1.Checked == true)
                {



                    
                    cm = new SqlCommand("select * from directorylist where name_arabic=@ar", con);
                    cm.Parameters.AddWithValue("@ar", comboBox1.Text.ToString());

                    SqlDataReader dr = cm.ExecuteReader();

                    while (dr.Read())
                    {

                        comboBox2.Text = dr.GetString(4).ToString();
                        textBox2.Text = dr.GetString(0);
                        textBox3.Text = dr.GetString(3);
                        textBox4.Text = dr.GetString(2);
                        textBox5.Text = dr.GetString(1);

                    }
                    con.Close();
                }


                else
                {



                    SqlCommand cm3 = new SqlCommand("select * from directorylist where mangment like N'" + comboBox1.Text.ToString() + "%'", con);



                    SqlDataReader dr3 = cm3.ExecuteReader();

                    comboBox2.Items.Clear();

                    while (dr3.Read())
                    {


                        comboBox2.Items.Add(dr3.GetString(4));


                    }



                }
                con.Close();

            }
            catch (Exception n)
            {
                con.Close();
                MessageBox.Show(n.Message);

            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
           
        }

        private void meButton2_Click(object sender, EventArgs e)
        {

            this.Hide();
            Form3 fm3 = new Form3();
            fm3.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void meButton3_Click(object sender, EventArgs e)
        {
            // RESET ALL FIELDS

           
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            radioButton1.Checked = false;
            radioButton2.Checked = false;

            comboBox1.ResetText();
            comboBox2.ResetText();

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            
            
        }

        private void panelContainer_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {

                con.Open();

                SqlCommand cm;

                cm = new SqlCommand("select * from directorylist where name_arabic=@ar", con);
                cm.Parameters.AddWithValue("@ar", comboBox2.Text.ToString());

                SqlDataReader dr = cm.ExecuteReader();

                while (dr.Read())
                {


                    textBox2.Text = dr.GetString(0);
                    textBox3.Text = dr.GetString(3);
                    textBox4.Text = dr.GetString(2);
                    textBox5.Text = dr.GetString(1);

                }
                con.Close();

            }
            catch(Exception r)
            {
                con.Close();
                MessageBox.Show(r.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}