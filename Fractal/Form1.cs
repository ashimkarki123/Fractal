using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Form1 : Form
    {
        private int MAX = 256;      // max iterations
        private double SX = -2.025; // start value real
        private double SY = -1.125; // start value imaginary
        private double EX = 0.6;    // end value real
        private double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Image pics;
        private Pen pen;
        private Graphics g1;
        private HSB HSBcol = new HSB();
        private bool mouseClicked;


        public Form1()
        {
            InitializeComponent();
            init();
            start();
            pictureBox1.Cursor = Cursors.Cross;

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateSave(-2.025, -1.125, 0.6, 1.126);
            Application.Restart();

        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 copy = new Form1();
            copy.Show();
            
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Dispose();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "JPG(*.JPG) | *.JPG";
            if (f.ShowDialog() == DialogResult.OK)
            {
                pics.Save(f.FileName);
            }
        }

        public void init() // all instances will be prepared
        {

            finished = false;
            x1 = pictureBox1.Width;
            y1 = pictureBox1.Height;
            xy = (float)x1 / (float)y1;
            pics = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(pics);
            finished = true;


            SX = stateRead()[0];
            SY = stateRead()[1];
            EX = stateRead()[2];
            EY = stateRead()[3];

        }

        public void destroy() // delete all instances 
        {
            if (finished)
            {

                pics = null;
                g1 = null;

                //System.gc();  garbage collection
            }
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        public void stop()
        {
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics obj = e.Graphics;
            obj.DrawImage(pics, new Point(0, 0));
        }


        public void update()
        {
            stateSave(xstart, ystart, xende, yende);
            Graphics g = pictureBox1.CreateGraphics();
            g.DrawImage(pics, 0, 0);
            if (rectangle)
            {
                Pen mypen = new Pen(Color.White, 1);
                if (xs < xe)
                {
                    if (ys < ye) g.DrawRectangle(mypen, xs, ys, (xe - xs), (ye - ys));
                    else g.DrawRectangle(mypen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g.DrawRectangle(mypen, xe, ys, (xs - xe), (ye - ys));
                    else g.DrawRectangle(mypen, xe, ye, (xs - xe), (ys - ye));
                }
            }
        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            action = false;


            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightness
                        HSBcol.fromHSB(h * 255, 0.8f * 255, b * 255); //convert hsb to rgb then make a Java Color
                        Color col = Color.FromArgb((int)HSBcol.rChan, (int)HSBcol.gChan, (int)HSBcol.bChan);

                        pen = new Pen(col);

                        alt = h;

                    }
                    g1.DrawLine(pen, x, y, x + 1, y);
                }
                action = true;
            }
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseClicked = true;

            if (action)
                {
                    xs = e.X;
                    ys = e.Y;
                }
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseClicked)
            {


                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    rectangle = true;

                    update();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            
            
            int z, w;

            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;

                mouseClicked = false;

                update();

                pictureBox1.Refresh();   
                
            }
        }

        private void stateSave(double xs, double ys, double xe, double ye)
        {
            using (StreamWriter sw = File.CreateText("state.txt"))
            {
                sw.WriteLine(xs);
                sw.WriteLine(ys);
                sw.WriteLine(xe);
                sw.WriteLine(ye);
            }

        }

        private List<double> stateRead()
        {
         
           List<double> l = new List<double>();

            using (StreamReader sr = File.OpenText("state.txt"))
            {
                double s = 0;
                while ((s = Convert.ToDouble(sr.ReadLine())) != 0)
                {
                    l.Add(s);
                }
            }

            return l;
        }
    }
}