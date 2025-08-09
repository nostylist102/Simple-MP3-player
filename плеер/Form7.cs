using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace плеер
{
    public partial class Form7 : Form
    {
        private Image gifImage;
        private int frameIndex = 0;
        private int _dotIndex = 0;
        private int _dotIndex2 = 0;
        private string _text = "Gain Access";
        private bool hasPlayedAnimation = false;
        private bool isTimerStarted = false;
        private bool labelClicked = false;
        private bool updateLabelText = true;

        private string _text2 = "Synchronized";
        double label2Opacity = 0.0;
        public Form7()
        {
            InitializeComponent();
            LoadGif();
            label2.Visible = false;

            timer2 = new Timer { Interval = 500 };
            timer2.Tick += timer2_Tick;
            timer2.Start();

            StartAnimation(); // Start the animation GIF

            Timer updateLabelTimer = new Timer { Interval = 2000 }; // 2 seconds
            updateLabelTimer.Tick += (sender, e) =>
            {
                label1.Visible = false; // Hide label1
                label2.Visible = true; // Show label2
                label2.Text = "Synchronized"; // Set label2 text to "Synchronized"

                for (int i = 0; i <= 255; i++)
                {
                    label2.BackColor = Color.FromArgb(i, label2.BackColor.R, label2.BackColor.G, label2.BackColor.B);
                }
            };
            updateLabelTimer.Start();

            Timer closeTimer = new Timer { Interval = 2777 };
            closeTimer.Tick += (sender, e) =>
            {
                this.Close();
            };
            closeTimer.Start();
        }

        private void LoadGif()
        {
            gifImage = Properties.Resources.e00c47431702e4b6e423d5742a0bec1a;
        }

        private void StartAnimation()
        {
            Timer animationTimer = new Timer();
            animationTimer.Interval = 12; // 8ms = 120fps (f)
            animationTimer.Tick += (sender, e) =>
            {
                GetFrame(gifImage, frameIndex);
                pictureBox1.Image = gifImage;
                frameIndex = (frameIndex + 1) % gifImage.GetFrameCount(FrameDimension.Time);
            };
            animationTimer.Start();
        }

        private void GetFrame(Image image, int frameIndex)
        {
            image.SelectActiveFrame(FrameDimension.Time, frameIndex);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            label1.Visible = true;
            label2.Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (updateLabelText)
            {
                label1.Text = _text + new string('.', _dotIndex + 1);
                _dotIndex = (_dotIndex + 1) % 3;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            //labelClicked = true;
            //if (!hasPlayedAnimation)
            //{
            //    StartAnimation();
            //    hasPlayedAnimation = true;
            //}
            //timer2.Stop(); // Stop timer2
            //label1.Visible = true;

            //Timer delayTimer = new Timer { Interval = 1950 }; // 2 seconds
            //delayTimer.Tick += (timerSender, timerEventArgs) =>
            //{
            //    updateLabelText = false;
            //    label1.Text = _text2;
            //    delayTimer.Stop();
            //};
            //delayTimer.Start();

            //StartCloseTimer();
            //Program.StartTimer();
            //label2.Hide();
            //label3.Hide();
            //label4.Hide();
            //textBox2.Hide();
            //textBox1.Hide();




        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    
        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }
    }
}