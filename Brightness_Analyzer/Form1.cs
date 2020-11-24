using Brightness_Analyzer.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brightness_Analyzer
{
    public partial class Form1 : Form
    {
        int countColumns = 15;
        int countRows = 25;
        double mediumBrightless = 0;
        Rectangle selRect;
        Point orig;
        Pen pen = new Pen(Brushes.Blue, 0.8f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };

        public Form1()
        {
            InitializeComponent();
            Init();
            
        }
        public void Init()
        {
            Image temp = Image.FromFile("C:\\Users\\Andriy\\Documents\\3ds Max 2020\\renderoutput\\auditorium_november_lamps2.jpg");

            //Image temp = pictureBox1.Image;// берем картинку или Image.FromFile("D:\\123.png");
            Bitmap src = new Bitmap(temp, groupBox1.Width, groupBox1.Height);
            pictureBox1.Image = src;
            pictureBox3.Image = src;
            List<Rectangle> rectList = new List<Rectangle>() { };
            int partIndex = 0;
            for (int i = 0; i < groupBox1.Width; i += groupBox1.Width / (countColumns))
            {
                for (int j = 0; j < groupBox1.Height; j += groupBox1.Height / (countRows))
                {
                    rectList.Add(new Rectangle(new Point(i, j), new Size((groupBox1.Width / (countColumns)), groupBox1.Height / (countRows))));
                    //partIndex++;
                }
            }

            Bitmap bmp;
            Graphics g;

            for (int i = 0; i < groupBox1.Width; i += groupBox1.Width / (countColumns))
            {
                for (int j = 0; j < groupBox1.Height; j += groupBox1.Height / (countRows))
                {
                    bmp = new Bitmap(src.Width, src.Height);
                    g = Graphics.FromImage(bmp);
                    g.DrawImage(src, 0, 0, rectList[partIndex], GraphicsUnit.Pixel);
                    var picture = new PictureBox
                    {
                        Name = "pictureBox" + i.ToString(),
                        Size = new Size(groupBox1.Width / (countColumns), groupBox1.Height / (countRows)),
                        Location = new Point(i, j),
                        Image = bmp,
                        AllowDrop = true
                    };
                    picture.AllowDrop = true;
                    picture.Click += PictureClick;
                    groupBox1.Controls.Add(picture);
                    partIndex++;
                }
            }
        }

        private void PictureClick(object sender, EventArgs e)
        {
            label1.Text = "Освітлення на вибраній ділянці:";
            PictureBox pic = (PictureBox)sender;
            pictureBox2.Image = pic.Image;
            Bitmap bmp = new Bitmap(pic.Width, pic.Height);
            Graphics g = Graphics.FromImage(bmp);
            Rectangle rec = new Rectangle(new Point(0, 0),new Size(pic.Width, pic.Height));
            g.DrawImage(pic.Image, 0, 0, rec, GraphicsUnit.Pixel);

            label1.Text += '\n' +(Math.Round(BrigtlessActions.AreaBrightless(bmp),3)*100).ToString()+"%";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Init();double brightless = (double)numericUpDown1.Value/100;
            Bitmap src;
            //countRows = Convert.ToInt32(numericUpDown2);
            //countColumns = Convert.ToInt32(numericUpDown3);
            
            if (tabControl1.SelectedTab.Text == "Сітка")
            {
                List<Rectangle> rectList = new List<Rectangle>() { };
                for (int i = 0; i < groupBox1.Width; i += groupBox1.Width / (countColumns))
                {
                    for (int j = 0; j < groupBox1.Height; j += groupBox1.Height / (countRows))
                    {
                        rectList.Add(new Rectangle(new Point(i, j), new Size((groupBox1.Width / (countColumns)), groupBox1.Height / (countRows))));
                       }
                }

                
                for (int i = 0; i < groupBox1.Controls.Count; i++)
                {

                    PictureBox pic = (PictureBox)groupBox1.Controls[0];
                    Bitmap bmp = new Bitmap(pic.Width, pic.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    Rectangle rec = new Rectangle(new Point(0, 0), new Size(pic.Width, pic.Height));
                    g.DrawImage(pic.Image, 0, 0, rec, GraphicsUnit.Pixel);

                    mediumBrightless = BrigtlessActions.AreaBrightless(bmp);

                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            if (x == bmp.Width-1 || x == 1 || y == 1 || y == bmp.Height-1)
                            {
                                if (mediumBrightless < brightless)
                                {
                                    bmp.SetPixel(x, y, Color.FromName("Red"));
                                }
                                else
                                {
                                    bmp.SetPixel(x, y, Color.FromName("Green"));
                                }
                            }
                        }
                    }
                    pic.Image = bmp;
                    groupBox1.Controls.RemoveAt(0);
                    groupBox1.Controls.Add(pic);

                }
                
            }
            else
            {

                src = new Bitmap(pictureBox1.Image);
                for (int i = 0; i < groupBox1.Width; i++)
                {
                    for (int j = 0; j < groupBox1.Height; j++)
                    {
                        if (src.GetPixel(i, j).GetBrightness() < brightless)
                        {
                            src.SetPixel(i, j, Color.FromName("Red"));
                        }
                    }
                }
                pictureBox1.Image = src;

            }
        }

        void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox3.Paint -= Selection_Paint;
            pictureBox3.Paint += pictureBox3_Paint;
            pictureBox3.Invalidate();
        }

        void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox3.Paint -= pictureBox3_Paint;
            pictureBox3.Paint += Selection_Paint;
            orig = e.Location;
            
        }


        private void Selection_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(pen, selRect);
        }

        void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            selRect = GetSelRectangle(orig, e.Location);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                (sender as PictureBox).Refresh();
            
            
        }
        

        void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, selRect);
        }

        Rectangle GetSelRectangle(Point orig, Point location)
        {
            int deltaX = location.X - orig.X, deltaY = location.Y - orig.Y;
            Size s = new Size(Math.Abs(deltaX), Math.Abs(deltaY));
            Rectangle rect = new Rectangle();
            if (deltaX >= 0 & deltaY >= 0)
                rect = new Rectangle(orig, s);
            if (deltaX < 0 & deltaY > 0)
                rect = new Rectangle(location.X, orig.Y, s.Width, s.Height);
            if (deltaX < 0 & deltaY < 0)
                rect = new Rectangle(location, s);
            if (deltaX > 0 & deltaY < 0)
                rect = new Rectangle(orig.X, location.Y, s.Width, s.Height);
            return rect;
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            label6.Text = "Освітлення на виділеній ділянці:";
            Bitmap bmp = new Bitmap(GetSelRectangle(orig, e.Location).Width, GetSelRectangle(orig, e.Location).Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(pictureBox3.Image, 0, 0, GetSelRectangle(orig, e.Location), GraphicsUnit.Pixel);
            pictureBox4.Image = bmp;
            label6.Text += '\n' + (Math.Round(BrigtlessActions.AreaBrightless(bmp), 3) * 100).ToString() + "%";
        }
    }
}
