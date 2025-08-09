using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace плеер
{
    public partial class Form8 : Form
    {

        
        
        
        private Form1 form1;
        private int _dotIndex = 0;
        private string _text = "Exiting";
        
        
        public Form8(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            timer1 = new Timer();
            timer1.Interval = 900; // 3 seconds
            timer1.Tick += timer1_Tick;
            timer1.Start();
            timer2 = new Timer { Interval = 5000 }; // timer2
            timer2.Tick += timer2_Tick;
            timer2.Start(); // start timer2
            timer3 = new Timer { Interval = 10 }; // 10ms interval
            timer3.Tick += timer3_Tick;

        }




        private void Form8_Load(object sender, EventArgs e)
        {

            pictureBox1.Image = Properties.Resources._07f55516c49808f89f7fd3935b0af04f;
            //@"C:\Users\PC\source\repos\плеер\background image\a3f3abe546f87d7bceb19ff1f3eea602.gif"
            //@"C:\Users\PC\source\repos\плеер\background image\07f55516c49808f89f7fd3935b0af04f.gif"
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer3.Start(); // fade out timer

            // Update dot positions
            string dots = label1.Text;
            dots = dots.Replace("...", ".");
            label1.Text = dots;

            // Close the form when the timer expires
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            label1.Text = _text + new string('.', _dotIndex + 1); // update label1 text
            _dotIndex = (_dotIndex + 1) % 24; // cycle through 0, 1, 2
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            // Exponential fadeout
            this.Opacity *= 0.9;
            if (this.Opacity <= 0.01)
            {
                this.Close();
            }
        }

        private void Form8_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer1 != null)
            {
                timer1.Stop();
                timer1.Dispose();
                timer1 = null;
            }
            if (timer2 != null)
            {
                timer2.Stop();
                timer2.Dispose();
                timer2 = null;
            }
            if (timer3 != null)
            {
                timer3.Stop();
                timer3.Dispose();
                timer3 = null;
            }
            this.Dispose();
        }

        private void Form8_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
    

    
}
