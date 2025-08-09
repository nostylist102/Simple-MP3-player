using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace плеер
{
    public partial class Form4 : Form
    {
        private Form1 _form1;
        private int stream;

        public Form4(Form1 form1)
        {
            InitializeComponent();
            _form1 = form1;
        }


        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // black
        {
            Bitmap image = Properties.Resources.wave2;
            _form1.pictureBox2.Image = image;
            Form1 form1 = (Form1)_form1; //
            form1.toolStrip2.BackgroundImage = Properties.Resources.wave2;
        }

        private void button2_Click(object sender, EventArgs e) //defolt
        {
            Bitmap image = Properties.Resources.WyTK;
            _form1.pictureBox2.Image = image;
            Form1 form1 = (Form1)_form1; //
            form1.toolStrip2.BackgroundImage = Properties.Resources.WyTK;
        }

        private void button3_Click(object sender, EventArgs e) //orange
        {
            Bitmap image = Properties.Resources._2221;
            _form1.pictureBox2.Image = image;
            Form1 form1 = (Form1)_form1; //
            form1.toolStrip2.BackgroundImage = Properties.Resources._2221;
            form1.toolStrip2.BackgroundImageLayout = ImageLayout.Tile;
        }

        
    }
}
