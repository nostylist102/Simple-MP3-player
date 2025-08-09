using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSAudioVisualization;
using CSCore;
using TagLib;
using System.Configuration;
using System.Windows.Media.Imaging;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Wma;
using System.Runtime.InteropServices;
using Un4seen.Bass.AddOn.Tags;
using System.Security.Policy;


namespace плеер
{

    public partial class Form3 : Form
    {
        private int stream;
        private System.Drawing.Point location;
        private static Form1 form1;
        private static Form2 form2;
        private bool isPaused;
        private bool selectedTrack;
        private bool isPlaying;
        private long currentPosition;
        private bool selectedTrack1;
        private Bass _bass;
        private int _stream;
        private Image gifImage;
        private int currentStationIndex = 0;
        private bool isFirstLoad = true;
        private int[] _streams = new int[4];
        private int _currentStreamIndex;

        public Form3(int stream, Form1 form1, Form2 form2)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.stream = stream;
            Form3.form1 = form1;
            Form3.form2 = form2;

            gifImage = Properties.Resources.DLLs; // bg gif
            pictureBox1.Image = gifImage;

            Bass.BASS_Free();

            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                throw new Exception("Error initializing Bass: " + Bass.BASS_ErrorGetCode());
            }
        }

        
        private string[] radioStationNames = new string[]
        {
            "Record",
            "Lofi",
            "Chillhouse",
            "Synthwave"
        };
        
        private string[] radioStations = new string[]
        {
            "https://radiorecord.hostingradio.ru/rr_main96.aacp",
            "https://radiorecord.hostingradio.ru/lofi96.aacp",
            "https://radiorecord.hostingradio.ru/chillhouse96.aacp",
            "https://radiorecord.hostingradio.ru/synth96.aacp"
        };

        private void Form3_Load(object sender, EventArgs e)
        {
            label4.Text = string.Empty;
            label5.Text = string.Empty;
            colorSlider1.Value = 20;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(radioStationNames);
            comboBox1.SelectedIndex = -1;

        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            location = this.Location;
            if (this.Owner is Form1)
            {
                Form1 form1 = (Form1)this.Owner;
                form1.IsFormActivated = false;
                form1.Location = location;
                form1.StartPosition = FormStartPosition.Manual;
                this.Hide();
                form1.Show();
            }
            else
            {
                MessageBox.Show("Owner is not a Form1 object");
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }




        private void PlayRadio(string url, int streamIndex)
        {
            try
            {
                if (_streams[streamIndex] != 0)
                {
                    StopRadio(_streams[streamIndex]);
                    Bass.BASS_StreamFree(_streams[streamIndex]);
                }
                _streams[streamIndex] = Bass.BASS_StreamCreateURL(url, 0, BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);

                if (_streams[streamIndex] == 0)
                {
                    throw new Exception("Error creating stream: " + Bass.BASS_ErrorGetCode());
                }

                float volumeLevel = (float)colorSlider1.Value / 100.0f;
                Bass.BASS_ChannelSetAttribute(_streams[streamIndex], BASSAttribute.BASS_ATTRIB_VOL, volumeLevel);

                if (!Bass.BASS_ChannelPlay(_streams[streamIndex], false))
                {
                    throw new Exception("Error playing stream: " + Bass.BASS_ErrorGetCode());
                }

                Console.WriteLine("Stream created and playing");

                TAG_INFO tagInfo = new TAG_INFO(url);
                if (BassTags.BASS_TAG_GetFromURL(_streams[streamIndex], tagInfo))
                {

                    timer1.Interval = 3600;
                    timer1.Tick += (sender, e) =>
                    {
                        label4.Text = tagInfo.title;
                        label5.Text = tagInfo.artist;
                    };
                    timer1.Start();
                }

                _currentStreamIndex = streamIndex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing radio: " + ex.Message);
            }
        }


        private void StopRadio(int stream)
        {
            try
            {
                if (stream != 0)
                {
                    if (!Bass.BASS_ChannelStop(stream))
                    {
                        throw new Exception("Error stopping stream: " + Bass.BASS_ErrorGetCode());
                    }

                    stream = 0;

                    if (!Bass.BASS_StreamFree(stream))
                    {
                        throw new Exception("Error freeing stream: " + Bass.BASS_ErrorGetCode());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping radio: " + ex.Message);
            }
        }



        private void button3_Click(object sender, EventArgs e) // Stop
        {
            if (_streams != null && _streams.Length > 0)
            {
                for (int i = 0; i < _streams.Length; i++)
                {
                    if (_streams[i] != 0 && Bass.BASS_ChannelIsActive(_streams[i]) == BASSActive.BASS_ACTIVE_PLAYING)
                    {
                        StopRadio(_streams[i]);
                        Bass.BASS_StreamFree(_streams[i]);
                        _streams[i] = 0;
                    }
                }
            }
            currentStationIndex = 0;
            isPlaying = false;
            label4.Visible = false; // hide label4
            label5.Visible = false; // hide label5
        }


        private void button1_Click(object sender, EventArgs e)
        {
            location = this.Location;
            Form1 form1 = (Form1)this.Owner;
            form1.Location = location;
            form1.StartPosition = FormStartPosition.Manual;

            this.Hide();
            form1.Show();
        }


        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < _streams.Length; i++)
            {
                StopRadio(_streams[i]);
            }
            Bass.BASS_Free();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.ShowForm8(Form3.form1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e) // radiostation photo
        {

        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            int volume = colorSlider1.Value;
            float volumeLevel = (float)volume / 100.0f;

            if (_currentStreamIndex >= 0 && _currentStreamIndex < _streams.Length)
            {
                int currentStream = _streams[_currentStreamIndex];
                if (currentStream != 0)
                {
                    Bass.BASS_ChannelSetAttribute(currentStream, BASSAttribute.BASS_ATTRIB_VOL, volumeLevel);
                }
            }
        }



        private void button4_Click(object sender, EventArgs e) //prev
        {
            currentStationIndex = (currentStationIndex - 1 + radioStations.Length) % radioStations.Length;
            PlayRadio(radioStations[currentStationIndex], currentStationIndex);
        }

        private void button5_Click(object sender, EventArgs e) //next
        {
            currentStationIndex = (currentStationIndex + 1) % radioStations.Length;
            PlayRadio(radioStations[currentStationIndex], currentStationIndex); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < radioStations.Length)
            {
                StopRadio(_streams[currentStationIndex]);
                currentStationIndex = selectedIndex;
                PlayRadio(radioStations[selectedIndex], selectedIndex);
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {

            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e) //name
        {

        }

        private void label5_Click(object sender, EventArgs e) //artist
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e) // Play
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedIndex < radioStations.Length)
            {
                if (_streams[comboBox1.SelectedIndex] == 0)
                {
                    string url = radioStations[comboBox1.SelectedIndex];
                    PlayRadio(url, comboBox1.SelectedIndex);
                    isPlaying = true;

                    TAG_INFO tagInfo = new TAG_INFO(url);
                    if (BassTags.BASS_TAG_GetFromURL(_streams[comboBox1.SelectedIndex], tagInfo))
                    {
                        timer1.Interval = 4000; // 4 seconds
                        timer1.Tick += (timerSender, timerEventArgs) =>
                        {
                            label4.Text = tagInfo.title;
                            label5.Text = tagInfo.artist;
                            label4.Visible = true; 
                            label5.Visible = true; 
                        };
                        timer1.Start();
                    }
                }
                else
                {
                    Bass.BASS_ChannelPlay(_streams[comboBox1.SelectedIndex], false);
                    isPlaying = true;

                }
            }
        }
    }
}
