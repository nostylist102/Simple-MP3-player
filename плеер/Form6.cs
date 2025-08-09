using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace плеер
{
    public partial class Form6 : Form
    {
        private string _selectedTrack;
        private string _title;
        private string _artist;
        private string _album;
        private int _year;
        private string _trackPath;

        public Form6(string selectedTrack, string title, string artist, string album, int year, string trackPath)
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
            _selectedTrack = selectedTrack;
            _title = title;
            _artist = artist;
            _album = album;
            _year = year;
            _trackPath = trackPath;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            label1.Text = _title;
            label2.Text = _artist;
            label3.Text = _album;
            label4.Text = _year.ToString();

            // Load album art image
            using (TagLib.File file = TagLib.File.Create(_trackPath))
            {
                if (file.Tag.Pictures.Length > 0)
                {
                    byte[] imageData = file.Tag.Pictures[0].Data.Data;
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.no_album_cover_;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 11)) // 11пикселей обводка 
            {
                e.Graphics.DrawRectangle(pen, 0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);
            }
        }

        private void Form6_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 form1 = (Form1)Owner;
            form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
