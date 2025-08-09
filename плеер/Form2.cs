using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Sfx;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TagLib;
using System.Configuration;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Runtime.InteropServices;
using CSAudioVisualization;

namespace плеер
{
    public partial class Form2 : Form
    {
        private System.Drawing.Point location;
        private static Form1 form1;
        private static Form2 form2;
        private static Form3 form3;
        private ToolStrip toolStrip1;
        internal ToolStripItem[] toolStripItems;
        private static int _stream;
        private int effect;
        private long currentPosition;
        private bool isPlaying;
        private bool isPaused;
        private static int stream = 0;

        public string SelectedTrack { get; internal set; }

        BASS_DX8_PARAMEQ par = new BASS_DX8_PARAMEQ();

        private int[] fx = new int[] { 0 };
        private BASS_DX8_ECHO _echo = new BASS_DX8_ECHO(90f, 50f, 500f, 500f, false);

        private string selectedTrack;
        private int _fxEqHandle = 0;
        private int _fxEchoHandle = 0;


        public Form2(long playbackPosition, bool isPlaying, string selectedTrack, int stream)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            if (!isPlaying)
            {
                return;
            }
            this.selectedTrack = selectedTrack;
            _stream = stream;

        }

        private void Form2_Load_1(object sender, EventArgs e)
        {
            _fxEqHandle = Bass.BASS_ChannelSetFX(_stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            if (_fxEqHandle == 0)
            {
                System.Windows.Forms.MessageBox.Show("Error setting EQ FX: " + Bass.BASS_ErrorGetCode().ToString());
                return;
            }

            _fxEchoHandle = Bass.BASS_ChannelSetFX(_stream, BASSFXType.BASS_FX_DX8_ECHO, 0);
            if (_fxEchoHandle == 0)
            {
                System.Windows.Forms.MessageBox.Show("Error setting echo FX: " + Bass.BASS_ErrorGetCode().ToString());
                return;
            }

            BASS_DX8_ECHO echo = new BASS_DX8_ECHO();
            echo.fWetDryMix = 0.0f; // disable echo effect
            Bass.BASS_FXSetParameters(_fxEchoHandle, echo);

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            button1_Click(null, null); //сбрасывает эквалайзер при переходе формы(чтобы не сохр)
            location = this.Location;
            if (this.Owner is Form1 form1)
            {
                form1.Location = location;
                form1.StartPosition = FormStartPosition.Manual;
                this.Hide();
                form1.Show();


                form1.Shown += (s, args) => this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Owner is not a Form1 instance");
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Go to form1, after that go to form3");

            //location = this.Location;
            //form3 = new Form3(stream, form1, form2);
            //form3.Location = location;
            //form3.StartPosition = FormStartPosition.Manual;
            //this.Hide();
            //form3.ShowDialog(this);
        }





        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 161, trackBar3.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 299, trackBar5.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 637, trackBar6.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 1000, trackBar7.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 2000, trackBar8.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 4000, trackBar9.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                if (SetParametersEQ(_fxEqHandle, 9000, trackBar10.Value))
                {
                    Bass.BASS_FXSetParameters(_fxEqHandle, par);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error setting EQ parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }


        private bool SetParametersEQ(int fx, int center, int gain)
        {
            par.fBandwidth = 18.0f;
            par.fCenter = (float)center;
            par.fGain = (float)gain;
            return Bass.BASS_FXSetParameters(fx, par);

        }

        private void buttonApply_Click(object sender, EventArgs e) //close
        {
            location = this.Location;
            Form1 form1 = (Form1)this.Owner;
            form1.Location = location;
            form1.StartPosition = FormStartPosition.Manual;

            this.Hide();
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_fxEqHandle != 0)
            {
                Bass.BASS_FXReset(_fxEqHandle);
            }

            if (_fxEchoHandle != 0)
            {
                Bass.BASS_FXReset(_fxEchoHandle);
            }

            this.Hide();
            Program.ShowForm8(form1);
            Bass.BASS_StreamFree(_stream);

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            if (_fxEchoHandle != 0)
            {
                _echo.fWetDryMix = trackBar4.Value / 100.0f;
                if (Bass.BASS_FXSetParameters(_fxEchoHandle, _echo) == false)
                {
                    System.Windows.Forms.MessageBox.Show("Error setting echo parameters: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }
        }

        private void button6_Click(object sender, EventArgs e) //apply
        {
            //Properties.Settings.Default.TrackBar3Value = trackBar3.Value;
            //Properties.Settings.Default.TrackBar4Value = trackBar4.Value;
            //Properties.Settings.Default.TrackBar5Value = trackBar5.Value;
            //Properties.Settings.Default.TrackBar6Value = trackBar6.Value;
            //Properties.Settings.Default.TrackBar7Value = trackBar7.Value;
            //Properties.Settings.Default.TrackBar8Value = trackBar8.Value;
            //Properties.Settings.Default.TrackBar9Value = trackBar9.Value;
            //Properties.Settings.Default.TrackBar10Value = trackBar10.Value;

            //Properties.Settings.Default.Save();



            // Save the current EQ settings
            //Properties.Settings.Default.EQ161Hz = trackBar3.Value;
            //Properties.Settings.Default.EQ299Hz = trackBar5.Value;
            //Properties.Settings.Default.EQ637Hz = trackBar6.Value;
            //Properties.Settings.Default.EQ1000Hz = trackBar7.Value;
            //Properties.Settings.Default.EQ2000Hz = trackBar8.Value;
            //Properties.Settings.Default.EQ4000Hz = trackBar9.Value;
            //Properties.Settings.Default.EQ9000Hz = trackBar10.Value;
            //Properties.Settings.Default.EchoLevel = trackBar4.Value;

            //// Update the EQ settings
            //SetParametersEQ(_fxEqHandle, 161, trackBar3.Value);
            //SetParametersEQ(_fxEqHandle, 299, trackBar5.Value);
            //SetParametersEQ(_fxEqHandle, 637, trackBar6.Value);
            //SetParametersEQ(_fxEqHandle, 1000, trackBar7.Value);
            //SetParametersEQ(_fxEqHandle, 2000, trackBar8.Value);
            //SetParametersEQ(_fxEqHandle, 4000, trackBar9.Value);
            //SetParametersEQ(_fxEqHandle, 9000, trackBar10.Value);

            //_echo.fWetDryMix = (float)trackBar4.Value / 20.0f;
            //Bass.BASS_FXSetParameters(_fxEchoHandle, _echo);

            //System.Windows.Forms.MessageBox.Show("EQ settings applied!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackBar3.Value = 0;
            trackBar4.Value = 0;
            trackBar5.Value = 0;
            trackBar6.Value = 0;
            trackBar7.Value = 0;
            trackBar8.Value = 0;
            trackBar9.Value = 0;
            trackBar10.Value = 0;

           //upd settings
            if (_fxEqHandle != 0)
            {
                par.fGain = 0.0f;
                Bass.BASS_FXSetParameters(_fxEqHandle, par);
            }

            par.fBandwidth = 18.0f;
            par.fCenter = 0.0f; //может не надо
            par.fGain = 0.0f;

        }

        private void button2_Click_1(object sender, EventArgs e)  //save 
        {


            System.Windows.Forms.MessageBox.Show("Settings saved!(doesn't work)");
        }


    }
    





}