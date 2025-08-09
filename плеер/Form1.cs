using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Fx;
using TagLib;
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NAudio.Wave;
using NAudio.Dsp;
using NAudio.Wave.SampleProviders;
using System.Windows.Media.Media3D;
using System.Drawing.Drawing2D;
using System.Windows;
using MaterialSkin.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using System.Timers;
using System.Windows.Media;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using MB.Controls;
using System.Security.Cryptography.X509Certificates;
using static плеер.Form6;
using CSCore.Win32;
using System.Collections.ObjectModel;
using System.Reflection.Emit;



namespace плеер
{


    public partial class Form1 : Form
    {
        private static int stream;
        private System.Drawing.Point location;
        private static Form1 form1;
        private static Form6 form6;
        private ToolStripMenuItem плеерToolStripMenuItem;
        private ToolStrip toolStrip1;
        private static Form2 form2;
        private SoundPlayer soundPlayer;
        private bool isUserScrolling = false; //трое нижних фикс положения трека
        private bool isMouseDown = false;
        private int lastPosition = 0;
        private bool isPlaying = false;
        private bool isPaused = false;
        private long pausedPosition = 0;
        private bool isRestarting = false;
        private static Form3 form3;
        private bool albumNotFound = false;
        private int _fxChorusHandle = 0;
        private BASS_DX8_ECHO _echo = new BASS_DX8_ECHO(90f, 50f, 500f, 500f, false);
        private int intValue;
        private Form7 form7;
        private Form8 form8;
        private BASS_DX8_PARAMEQ _eq = new BASS_DX8_PARAMEQ();
        private List<string> playlist = new List<string>(); //3н
        private int currentTrackIndex = 0;
        private List<string> tracks = new List<string>();
        private Dictionary<string, List<string>> playlistTracks = new Dictionary<string, List<string>>();
        private string trackPath;
        List<string> filePaths = new List<string>();
        private bool isMinimized = false;
        private bool isFormActivated;


        private string _selectedTrack;
        private string _title;
        private string _artist;
        private string _album;
        private int _year;
        private string _trackPath;



        private string currentPlaylistName;

        private static long _playbackPosition;
        private static bool _isPlaying;

        private string selectedTrack;
        private List<string> playlistNames = new List<string>();
        private Dictionary<string, List<string>> playlists = new Dictionary<string, List<string>>();

        private int currentPlaylistIndex = 0;
        public Form1()
        {
            InitializeComponent();



            Bass.BASS_SetVolume(1.0f);
            this.MaximizeBox = false;
            this.FormClosed += Form1_FormClosed;
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            pictureBox1.Paint += PictureBox1_Paint;
            playlist.AddRange(listBox1.Items.Cast<string>()); //эт
        }

        public Form1(string selectedTrack, string title, string artist, string album, string trackPath) : this()
        {
            _selectedTrack = selectedTrack;
            _title = title;
            _artist = artist;
            _album = album;
            _trackPath = trackPath;

            // загружаем мета-данные и заполняем лейблы
        }



        public bool IsFormActivated
        {
            get { return isFormActivated; }
            set { isFormActivated = value; }
        }



        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                _playbackPosition = Bass.BASS_ChannelGetPosition(stream);
                _isPlaying = this.isPlaying;

                long currentPosition = Bass.BASS_ChannelGetPosition(stream);
                bool isPlaying = this.isPlaying;
                bool isPaused = this.isPaused;
                Form2 form2 = new Form2(_playbackPosition, _isPlaying, selectedTrack, stream);
                location = this.Location;
                form2.SelectedTrack = listBox1.SelectedItem.ToString();
                form2.Location = location;
                form2.StartPosition = FormStartPosition.Manual;
                this.Hide();
                form2.Owner = this;
                form2.ShowDialog();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please start playing a track first.");
            }


        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            isFormActivated = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            location = this.Location;
            Form3 form3 = new Form3(stream, this, form2);
            form3.Location = location;
            form3.StartPosition = FormStartPosition.Manual;
            this.Hide();
            form3.ShowDialog(this);
        }


        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 11)) // 11пикселей обводка 
            {
                e.Graphics.DrawRectangle(pen, 0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);
            }
        }




        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Audio files|*.wav;*.mp3;*.flac;*.aac;*.m4a;*.ogg",
                    Multiselect = true
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFileDialog.FileNames)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        tracks.Add(file);
                        listBox1.Items.Add(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void PlayButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio files|*.wav;*.mp3;*.flac;*.aac;*.m4a;*.ogg",
                Multiselect = true
            };


            if (listBox1.SelectedIndex >= 0)
            {

                string selectedTrack = listBox1.SelectedItem.ToString();
                string trackPath = tracks[listBox1.SelectedIndex];

                if (System.IO.File.Exists(trackPath))
                {
                    try
                    {
                        if (isPlaying)
                        {
                            if (isPaused)
                            {
                                if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".flac") //мп3 без задержки, флак 4-5сек назад.
                                {
                                    double currentPositionSeconds = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream));
                                    currentPositionSeconds -= 4;
                                    Bass.BASS_ChannelSetPosition(stream, Bass.BASS_ChannelSeconds2Bytes(stream, currentPositionSeconds));
                                }
                                Bass.BASS_ChannelPlay(stream, false);
                                isPaused = false;
                            }
                            else
                            {
                                Bass.BASS_ChannelPause(stream);
                                isPaused = true;
                            }
                        }
                        else
                        {
                            if (stream != 0)
                            {
                                Bass.BASS_ChannelStop(stream);
                                stream = 0;
                            }

                            isPlaying = false;
                            isPaused = false;
                            pausedPosition = 0;

                            TagLib.File file = TagLib.File.Create(trackPath);

                            IPicture picture = file.Tag.Pictures.FirstOrDefault();
                            if (picture != null)
                            {
                                byte[] imageData = picture.Data.Data;
                                using (MemoryStream stream = new MemoryStream(imageData))
                                {
                                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                                    pictureBox1.Image = image;
                                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                }
                            }
                            else
                            {
                                if (!albumNotFound)
                                {
                                    System.Windows.MessageBox.Show("No album cover found.");
                                    albumNotFound = true;
                                }
                                Bitmap imagePath = Properties.Resources.no_album_cover_;
                                pictureBox1.Image = imagePath;
                            }

                            LoadTrackMetadata(trackPath);

                            stream = Bass.BASS_StreamCreateFile(trackPath, 0, 0, BASSFlag.BASS_DEFAULT);
                            if (stream != 0)
                            {
                                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_FREQ, 44100);

                                float volumeFloat = 2.0f;
                                colorSlider1.Value = (int)(volumeFloat * 1000);
                                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, volumeFloat);

                                if (Bass.BASS_ChannelPlay(stream, false))
                                {
                                    checkBox1_CheckedChanged(sender, e);
                                    isPlaying = true;
                                    isPaused = false;
                                    pausedPosition = 0;
                                }
                                else
                                {
                                    Console.WriteLine("Error={0}", Bass.BASS_ErrorGetCode());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }



            label2.Text = "Album photo";

            //int fxHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
            //_eq.fCenter = 1000f;
            //_eq.fBandwidth = 100f;
            //_eq.fGain = 10f;
            //Bass.BASS_FXSetParameters(fxHandle, _eq);

            isPlaying = true;
            timer1.Interval = 100;
            timer1.Start();

            audioVis.Mode = (CSAudioVisualization.Mode)Enum.Parse(typeof(CSAudioVisualization.Mode), comboBox1.Text);
            audioVis.DeviceIndex = comboBox2.SelectedIndex;
            audioVis.HighQuality = true;
            audioVis.BackColor = System.Drawing.Color.Black;
            audioVis.ColorBase = System.Drawing.Color.RoyalBlue;
            audioVis.ColorMax = System.Drawing.Color.White;
            audioVis.BarCount = 100;
            audioVis.BarSpacing = 5;
            audioVis.Interval = 30;
            audioVis.Start();








        }


        private void button4_Click(object sender, EventArgs e)
        {
            int currentIndex = listBox1.SelectedIndex;
            int newIndex = (currentIndex + 1) % listBox1.Items.Count;
            listBox1.SelectedIndex = newIndex;

            string selectedTrack = listBox1.Items[newIndex].ToString();
            string trackPath = tracks[newIndex];

            if (System.IO.File.Exists(trackPath))
            {
                if (stream != 0)
                {
                    Bass.BASS_ChannelStop(stream);
                    Bass.BASS_StreamFree(stream);
                    stream = 0;
                }

                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                stream = Bass.BASS_StreamCreateFile(trackPath, 0, 0, BASSFlag.BASS_DEFAULT);
                if (stream != 0 && Bass.BASS_ChannelPlay(stream, false))
                {
                    checkBox1_CheckedChanged(sender, e);
                    isPlaying = true;
                    isPaused = false;

                    Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, 0.02f);

                    colorSlider1.Value = 20;
                }
                else
                {
                    Console.WriteLine("Error={0}", Bass.BASS_ErrorGetCode());
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid track path: " + trackPath);
            }
            
            LoadTrackMetadata(trackPath);
        
        
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int currentIndex = listBox1.SelectedIndex;
            int newIndex = (currentIndex - 1 + listBox1.Items.Count) % listBox1.Items.Count;
            listBox1.SelectedIndex = newIndex;

            string selectedTrack = listBox1.Items[newIndex].ToString();
            string trackPath = tracks[newIndex];

            if (System.IO.File.Exists(trackPath))
            {
                if (stream != 0)
                {
                    Bass.BASS_ChannelStop(stream);
                    Bass.BASS_StreamFree(stream);
                    stream = 0;
                }

                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                stream = Bass.BASS_StreamCreateFile(trackPath, 0, 0, BASSFlag.BASS_DEFAULT);
                if (stream != 0 && Bass.BASS_ChannelPlay(stream, false))
                {
                    checkBox1_CheckedChanged(sender, e);
                    isPlaying = true;
                    isPaused = false;

                    Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, 0.02f);
                    colorSlider1.Value = 20;
                }
                else
                {
                    Console.WriteLine("Error={0}", Bass.BASS_ErrorGetCode());
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid track path: " + trackPath);
            }
            LoadTrackMetadata(trackPath);
        }



        private void LoadPlaylistsAndTracks()
        {
            string lastPlaylistFile = Path.Combine(playlistFolder, "last_playlist.txt");
            if (System.IO.File.Exists(lastPlaylistFile))
            {
                using (StreamReader reader = new StreamReader(lastPlaylistFile))
                {
                    string line;
                    listBox2.Items.Clear();
                    listBox1.Items.Clear();
                    while ((line = reader.ReadLine()) != null)
                    {
                        listBox2.Items.Add(line);
                        listBox1.Items.Add(line);
                    }
                    listBox2.SelectedItem = listBox2.Items.Count > 0 ? listBox2.Items[0] : null;
                }
            }

            string playlistsFile = Path.Combine(playlistFolder, "playlists.txt");
            if (System.IO.File.Exists(playlistsFile))
            {
                using (StreamReader reader = new StreamReader(playlistsFile))
                {
                    string line;
                    bool isPlaylist = true;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == "---") // Separator
                        {
                            isPlaylist = false;
                        }
                        else if (isPlaylist)
                        {
                            listBox2.Items.Add(line);
                        }
                        else
                        {
                            tracks.Add(line);
                        }
                    }
                }
                foreach (string track in tracks)
                {
                    listBox1.Items.Add(Path.GetFileNameWithoutExtension(track));
                }
            }
        }

        private void SavePlaylistsAndTracks()
        {
            string playlistFolder = Path.Combine(System.Windows.Forms.Application.StartupPath, "Playlists");
            string lastPlaylistFile = Path.Combine(playlistFolder, "last_playlist.txt");
            string playlistsFile = Path.Combine(playlistFolder, "playlists.txt");

            try
            {
                if (!Directory.Exists(playlistFolder))
                {
                    Directory.CreateDirectory(playlistFolder);
                }

                using (StreamWriter writer = new StreamWriter(lastPlaylistFile, false))
                {
                    if (listBox2.SelectedItem != null)
                    {
                        writer.WriteLine(listBox2.SelectedItem.ToString());
                    }
                    if (listBox1.Items.Count > 0)
                    {
                        foreach (string track in listBox1.Items)
                        {
                            writer.WriteLine(track);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving last playlist: " + ex.Message);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(playlistsFile, false))
                {
                    if (listBox2.Items.Count > 0)
                    {
                        foreach (string playlist in listBox2.Items)
                        {
                            writer.WriteLine(playlist);
                        }
                    }
                    writer.WriteLine("---");
                    if (tracks.Count > 0)
                    {
                        foreach (string track in tracks)
                        {
                            writer.WriteLine(track);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving playlists: " + ex.Message);
            }
        }

        private void LoadPlaylistsAndTracks2()
        {
            string playlistFolder = Path.Combine(System.Windows.Forms.Application.StartupPath, "Playlists");
            string lastPlaylistFile = Path.Combine(playlistFolder, "last_playlist.txt");
            string playlistsFile = Path.Combine(playlistFolder, "playlists.txt");

            try
            {
                if (Directory.Exists(playlistFolder) && System.IO.File.Exists(lastPlaylistFile))
                {
                    string[] lines = System.IO.File.ReadAllLines(lastPlaylistFile);
                    if (lines.Length > 0)
                    {
                        listBox2.SelectedItem = lines[0];
                        listBox1.Items.Clear();
                        listBox1.Items.AddRange(lines.Skip(1).ToArray());
                    }
                    else
                    {
                        listBox2.SelectedItem = null;
                        listBox1.Items.Clear();
                    }
                }
                else
                {
                    listBox2.SelectedItem = null;
                    listBox1.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading last playlist: " + ex.Message);
            }

            try
            {
                if (Directory.Exists(playlistFolder) && System.IO.File.Exists(playlistsFile))
                {
                    string[] lines = System.IO.File.ReadAllLines(playlistsFile);
                    listBox2.Items.Clear();
                    List<string> playlists = lines.TakeWhile(line => line != "---").ToList();
                    foreach (string playlist in playlists)
                    {
                        if (!listBox2.Items.Contains(playlist))
                        {
                            listBox2.Items.Add(playlist);
                        }

                        if (!playlistTracks.TryGetValue(playlist, out List<string> tracksList))
                        {
                            playlistTracks.Add(playlist, new List<string>());
                        }
                    }
                    tracks.Clear();
                    foreach (string track in lines.SkipWhile(line => line != "---").Skip(1))
                    {
                        tracks.Add(track);

                        foreach (string playlist in playlists)
                        {
                            if (playlistTracks.TryGetValue(playlist, out List<string> tracksList))
                            {
                                tracksList.Add(track);
                            }
                        }
                    }
                    foreach (string playlist in playlists)
                    {
                        if (!playlistTracks.TryGetValue(playlist, out List<string> tracksList))
                        {
                            playlistTracks.Add(playlist, new List<string>());
                        }
                        tracksList.AddRange(tracks);
                    }
                }
                else
                {
                    listBox2.Items.Clear();
                    tracks.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading playlists: " + ex.Message);
            }
        }

        private void LoadTrackMetadata(string filePath)
        {
            
            TagLib.File file = TagLib.File.Create(filePath);
           
            if (file.Tag.Pictures.Length > 0)
            {
                pictureBox1.Image = System.Drawing.Image.FromStream(new MemoryStream(file.Tag.Pictures[0].Data.Data));
            }
            else
            {
                pictureBox1.Image = Properties.Resources._1be269c5c5c608a1f41af5cebe5af718;
            }
            label21.Text = file.Tag.Title;
            label22.Text = file.Tag.FirstPerformer;

        }






        private void progressBar1_Click(object sender, EventArgs e)
        {

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e) //может и не paint
        {


        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPlaylistsAndTracks2();

            label21.Text = _title;
            label22.Text = _artist;

            colorSlider1.Value = 2;
            pictureBox2.Image = Properties.Resources.WyTK; //bg gif
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            this.BackgroundImageLayout = ImageLayout.Stretch;


            colorSlider1.Value = 100;
            label7.Text = "100%";



            foreach (CSAudioVisualization.Mode mode in Enum.GetValues(typeof(CSAudioVisualization.Mode)))
            {
                comboBox1.Items.Add(audioVis.GetEnumValue(mode));
            }
            comboBox1.SelectedItem = audioVis.GetEnumValue(audioVis.Mode);
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isUserScrolling && !isRestarting)
            {
                long byteOffset = Bass.BASS_ChannelGetPosition(stream, BASSMode.BASS_POS_BYTE);
                int position = (int)((double)byteOffset / Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTE) * colorSlider2.Maximum);
                colorSlider2.Value = position;
            }

            if (!isUserScrolling && !isRestarting)
            {
                long byteOffset = Bass.BASS_ChannelGetPosition(stream, BASSMode.BASS_POS_BYTE);
                double seconds = Bass.BASS_ChannelBytes2Seconds(stream, byteOffset);
                int position = (int)(seconds / Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTE)) * colorSlider2.Maximum);
                colorSlider2.Value = position;

                // обнов позиции лбл  4 
                long currentPosition = Bass.BASS_ChannelGetPosition(stream);
                if (currentPosition == -1) // трек ост
                {
                    label4.Text = "00:00";
                }
                else
                {
                    seconds = Bass.BASS_ChannelBytes2Seconds(stream, currentPosition);
                    int hours = (int)(seconds / 3600);
                    int minutes = (int)((seconds % 3600) / 60);
                    int secondsRemaining = (int)(seconds % 60);
                    label4.Text = $"{minutes:D2}:{secondsRemaining:D2}";
                }

                // обн лбл   6
                long songDuration = Bass.BASS_ChannelGetLength(stream);
                if (songDuration == -1) // трек не продолж
                {
                    label6.Text = "00:00";
                }
                else
                {
                    double songDurationSeconds = Bass.BASS_ChannelBytes2Seconds(stream, songDuration);
                    int hours = (int)(songDurationSeconds / 3600);
                    int minutes = (int)((songDurationSeconds % 3600) / 60);
                    int secondsRemaining = (int)(songDurationSeconds % 60);
                    label6.Text = $"{minutes:D2}:{secondsRemaining:D2}";
                }

                // обнов 7   лбл
                float volume = 2.0f;
                Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, ref volume);
                label7.Text = $"{(int)(volume * 1000)}%";
            }

            //это листбокс след трек должен быть
            if (stream != 0)
            {
                long currentPosition = Bass.BASS_ChannelGetPosition(stream);
                long trackLength = Bass.BASS_ChannelGetLength(stream);

                if (currentPosition >= trackLength)
                {
                    int selectedIndex = listBox1.SelectedIndex;
                    if (selectedIndex < listBox1.Items.Count - 1)
                    {
                        listBox1.SelectedIndex = selectedIndex + 1;
                        string nextTrack = listBox1.SelectedItem.ToString();

                        Bass.BASS_ChannelStop(stream);
                        stream = Bass.BASS_StreamCreateFile(nextTrack, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);
                        Bass.BASS_ChannelPlay(stream, true);
                    }
                }
            }
        }




        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer2.Interval = 100;
                timer2.Start();
            }
            else
            {
                timer2.Stop();
            }
        }




        private void colorSlider1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) //звук
        {
            float volumeFloat = (float)colorSlider1.Value / 1000f;
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, volumeFloat);
        }




        private void colorSlider2_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            Bass.BASS_ChannelPause(stream);          // ставим на паузу
        }

        private void colorSlider2_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;

            
            long pos = (long)(colorSlider2.Value /
                              (double)colorSlider2.Maximum *
                              Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTE));

            Bass.BASS_ChannelSetPosition(stream, pos);

            
            if (isPlaying && !isPaused)
                Bass.BASS_ChannelPlay(stream, false);
        }

        //
        //крутка
        private void colorSlider2_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            if (isMouseDown)
            {
                long pos = (long)(colorSlider2.Value /
                                  (double)colorSlider2.Maximum *
                                  Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTE));

                Bass.BASS_ChannelSetPosition(stream, pos);  
            }
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Bass.BASS_ChannelIsActive(stream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelSetPosition(stream, 0);
                Bass.BASS_ChannelPlay(stream, false);
            }
        }

        private void audioVis_Load(object sender, EventArgs e)
        {

        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CSAudioVisualization.Mode mode = (CSAudioVisualization.Mode)Enum.Parse(typeof(CSAudioVisualization.Mode), comboBox1.Text);
            comboBox2.DataSource = audioVis.GetDevices(mode);
            int default_index = audioVis.GetDeviceDefaultIndex(mode);

            if (default_index != -1)
            {
                comboBox2.SelectedIndex = default_index;
            }
            else
            {
                comboBox2.SelectedIndex = 0;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e) // начало таймер
        {
            label4.Text = "00:00";
        }

        private void label6_Click(object sender, EventArgs e) // конец таймер
        {
            label6.Text = "00:00";
        }

        private void label7_Click(object sender, EventArgs e) // громкость
        {
            label7.Text = "100%";
        }

        private void button7_Click(object sender, EventArgs e) // сохр треки в плане
        {

        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            SavePlaylistsAndTracks();

            audioVis.Stop();
            audioVis.Dispose();
            audioVis = null;

            this.Hide();
            Form1 form1 = new Form1();
            Form8 form8 = new Form8(this);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Bass.BASS_Stop();
            Bass.BASS_Free();


        }

        private void button9_Click(object sender, EventArgs e) // настройки
        {
            Form4 form4 = new Form4(this);
            form4.StartPosition = FormStartPosition.CenterScreen;
            form4.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                _playbackPosition = Bass.BASS_ChannelGetPosition(stream);
                _isPlaying = this.isPlaying;

                long currentPosition = Bass.BASS_ChannelGetPosition(stream);
                bool isPlaying = this.isPlaying;
                bool isPaused = this.isPaused;
                string selectedTrack = listBox1.SelectedItem.ToString();
                string trackPath = tracks[listBox1.SelectedIndex];

                TagLib.File file = TagLib.File.Create(trackPath);
                string title = file.Tag.Title;
                string artist = file.Tag.FirstPerformer;
                string album = file.Tag.Album;
                int year = (int)file.Tag.Year;

                Form6 form6 = new Form6(selectedTrack, title, artist, album, year, trackPath);
                form6.Owner = this;
                form6.TopMost = true;
                //System.Drawing.Point point = Cursor.Position;
                //int x = Screen.PrimaryScreen.Bounds.Width / 2 + 40; //40
                //int y = (Screen.PrimaryScreen.Bounds.Height - form6.Height) / 2 + (form6.Height / 2) - 220;
                //form6.Location = new System.Drawing.Point(x, y);
                form6.Show();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please start playing a track first.");
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string playlistName = listBox2.SelectedItem.ToString();
                playlists.Remove(playlistName);
                listBox2.Items.Remove(listBox2.SelectedItem);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string newPlaylistName = textBox1.Text;
            if (!string.IsNullOrEmpty(newPlaylistName) && !playlistNames.Contains(newPlaylistName))
            {
                playlistNames.Add(newPlaylistName);
                listBox2.Items.Add(newPlaylistName);
                playlists.Add(newPlaylistName, new List<string>());
                textBox1.Text = "";
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Playlist with this name already exists.");
            }
        }



        private string previousPlaylistName = "";

        public string SelectedTrackName { get; private set; }
        public string SelectedTrack { get; private set; }








        private bool isTrackPlaying = false;
        private void listBox1_DoubleClick(object sender, EventArgs e) //
        {

            if (isFormActivated && listBox1.SelectedIndex >= 0)
            {
                if (stream != 0)
                {
                    Bass.BASS_ChannelStop(stream);
                    stream = 0;
                }


                string selectedTrack = listBox1.SelectedItem.ToString();
                string trackPath = tracks[listBox1.SelectedIndex];

                if (System.IO.File.Exists(trackPath))
                {

                    TagLib.File file = TagLib.File.Create(trackPath);

                    IPicture picture = file.Tag.Pictures.FirstOrDefault();
                    if (picture != null)
                    {
                        byte[] imageData = picture.Data.Data;
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                            pictureBox1.Image = image;
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                    }
                    else
                    {
                        if (!albumNotFound)
                        {
                            System.Windows.MessageBox.Show("No album cover found.");
                            albumNotFound = true;
                        }
                        Bitmap imagePath = Properties.Resources.no_album_cover_;
                        pictureBox1.Image = imagePath;
                    }

                    LoadTrackMetadata(trackPath);

                    if (stream != 0)
                    {
                        Bass.BASS_ChannelStop(stream);
                        Bass.BASS_StreamFree(stream);
                        stream = 0;
                    }

                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    stream = Bass.BASS_StreamCreateFile(trackPath, 0, 0, BASSFlag.BASS_DEFAULT);
                    if (stream != 0 && Bass.BASS_ChannelPlay(stream, false))
                    {
                        checkBox1_CheckedChanged(sender, e);
                        isPlaying = true;
                        isPaused = false;

                        Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, 0.02f);
                        colorSlider1.Value = 20;
                    }
                    else
                    {
                        Console.WriteLine("Error={0}", Bass.BASS_ErrorGetCode());
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Invalid track path: " + trackPath);
                }

                label2.Text = "Album photo";

                int fxHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
                _eq.fCenter = 1000f;
                _eq.fBandwidth = 100f;
                _eq.fGain = 10f;
                Bass.BASS_FXSetParameters(fxHandle, _eq);

                isPlaying = true;
                timer1.Interval = 100;
                timer1.Start();

                audioVis.Mode = (CSAudioVisualization.Mode)Enum.Parse(typeof(CSAudioVisualization.Mode), comboBox1.Text);
                audioVis.DeviceIndex = comboBox2.SelectedIndex;
                audioVis.HighQuality = true;
                audioVis.BackColor = System.Drawing.Color.Black;
                audioVis.ColorBase = System.Drawing.Color.RoyalBlue;
                audioVis.ColorMax = System.Drawing.Color.White;
                audioVis.BarCount = 100;
                audioVis.BarSpacing = 5;
                audioVis.Interval = 30;
                audioVis.Start();
            }


        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }



        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {

        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                string selectedSong = listBox1.SelectedItem.ToString();
                listBox1.Items.Remove(selectedSong);
                listBox1.Refresh();
                playlist.Remove(selectedSong);
            }
        }

        private string selectedPlaylistName;
        private List<string> playlist1Tracks = new List<string>();
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string playlistName = listBox2.SelectedItem.ToString();
                string selectedPlaylist = listBox2.SelectedItem.ToString();
                UpdateTrackList(playlistName);
                LoadPlaylistTracks(selectedPlaylist);
                selectedPlaylistName = listBox2.SelectedItem.ToString();
            }
        }

        private void LoadPlaylistTracks(string playlistName)
        {
            string playlistFile = playlistName + ".txt";
            if (System.IO.File.Exists(playlistFile))
            {
                tracks.Clear();
                listBox1.Items.Clear();

                using (StreamReader reader = new StreamReader(playlistFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (System.IO.File.Exists(line))
                        {
                            string trackName = Path.GetFileNameWithoutExtension(line);
                            tracks.Add(line);
                            listBox1.Items.Add(trackName);
                        }
                        else
                        {
                            // skip
                            continue;
                        }
                    }
                }
            }
        }

        private void button13_Move(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Press, before add a new song ");
        }

        private string _playlistName;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _playlistName = textBox1.Text;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            label21.Text = "";
            label22.Text = "";
            try
            {
                listBox1.Items.Clear();
                selectedTrack = string.Empty;
                currentTrackIndex = -1;
                tracks.Clear();
                Bass.BASS_Stop();
                Bass.BASS_Free();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting track: " + ex.Message);
            }



        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.Clear();
                playlistNames.Clear();
                playlists.Clear();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error deleting track: " + ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                currentTrackIndex = listBox1.SelectedIndex;
                selectedTrack = listBox1.SelectedItem.ToString();
                UpdateTrackMetadata2();
            }
        }
        private void UpdateTrackMetadata2()
        {
            if (tracks.Count > 0 && currentTrackIndex >= 0 && currentTrackIndex < tracks.Count)
            {
                string trackPath = tracks[currentTrackIndex];
                if (System.IO.File.Exists(trackPath))
                {
                    string trackTitle = GetTrackTitleFromPath(trackPath);
                    // ...
                }
                else
                {
                    Console.WriteLine($"Invalid track path: {trackPath}");
                }
            }
        }

        private string previousTrackTitle = "";
        private void UpdateTrackList(string playlistName)
        {
            listBox1.Items.Clear();

            Dictionary<string, List<string>> playlistTracks = new Dictionary<string, List<string>>();


            if (playlistName != null)
            {
                if (playlists.TryGetValue(playlistName, out List<string> tracksList))
                {
                    playlistTracks[playlistName] = tracksList;
                    tracks.Clear();
                    tracks.AddRange(tracksList);
                }
                else
                {
                    Console.WriteLine($"Playlist '{playlistName}' not found.");
                }
            }
            else
            {
                tracks.Clear();
                tracks.AddRange(playlists.Values.SelectMany(x => x));
            }

            foreach (string trackPath in tracks)
            {
                if (System.IO.File.Exists(trackPath))
                {
                    string trackTitle = GetTrackTitleFromPath(trackPath);
                    Console.WriteLine($"Track Path: {trackPath}, Track Title: {trackTitle}");
                    if (!string.IsNullOrEmpty(trackTitle))
                    {
                        listBox1.Items.Add(trackTitle);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid track path: {trackPath}");
                }
            }

            Dictionary<string, int> playlistTrackIndices = new Dictionary<string, int>();

            if (listBox1.Items.Count > 0)
            {
                if (listBox1.SelectedItem != null)
                {
                    currentTrackIndex = listBox1.SelectedIndex;
                    Console.WriteLine($"Current track index: {currentTrackIndex}");
                    playlistTrackIndices[playlistName] = currentTrackIndex;
                }
                else
                {
                    int newIndex = -1;
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        if (listBox1.Items[i].ToString() == previousTrackTitle)
                        {
                            newIndex = i;
                            break;
                        }
                    }
                    if (newIndex != -1)
                    {
                        currentTrackIndex = newIndex;
                        Console.WriteLine($"New current track index: {currentTrackIndex}");
                        playlistTrackIndices[playlistName] = currentTrackIndex;
                    }
                    else
                    {
                        currentTrackIndex = 0;
                        Console.WriteLine($"Current track index reset to 0");
                        playlistTrackIndices[playlistName] = 0;
                    }
                }
            }
            else
            {
                currentTrackIndex = -1;
                Console.WriteLine($"Current track index set to -1");
                playlistTrackIndices[playlistName] = -1;
            }

            currentPlaylistIndex = 0;
            currentPlaylistName = playlistName;
            previousPlaylistName = playlistName;
        }

        private string GetTrackTitleFromPath(string trackPath)
        {
            return Path.GetFileNameWithoutExtension(trackPath);
        }

        //private string GetTrackTitleFromPath(string trackPath)
        //{
        //    try
        //    {
        //        using (TagLib.File file = TagLib.File.Create(trackPath))
        //        {
        //            return file.Tag.Title;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error getting track title: {ex.Message}");
        //        return string.Empty;
        //    }
        //}

        private void button16_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (!isMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                isMinimized = true;
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                isMinimized = false;
            }
        }

        private void audioVis_Load_1(object sender, EventArgs e)
        {

        }


        private string playlistFolder = Path.Combine(System.Windows.Forms.Application.StartupPath, "Playlists");
        private void button7_Click_2(object sender, EventArgs e) // Save
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = playlistFolder;
            saveFileDialog.Title = "Save Playlist";

            string defaultFileName = listBox2.SelectedItem + ".txt";
            saveFileDialog.FileName = defaultFileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    foreach (string track in tracks)
                    {
                        writer.WriteLine(track);
                    }
                }
                System.Windows.Forms.MessageBox.Show("Playlist saved successfully!", "Success");
            }
        }

        private void button17_Click(object sender, EventArgs e) // Load
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = playlistFolder;
            openFileDialog.Title = "Load Playlist";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string playlistName = Path.GetFileNameWithoutExtension(filePath);

                try
                {
                    using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                    {
                        string line;
                        List<string> newTracks = new List<string>();
                        while ((line = reader.ReadLine()) != null)
                        {
                            string trackFileName = line;
                            string trackPath = Path.Combine(Path.GetDirectoryName(filePath), trackFileName);
                            if (System.IO.File.Exists(trackPath))
                            {
                                newTracks.Add(trackPath);
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show($"File not found: {trackFileName}");
                            }
                        }
                        listBox1.Items.Clear();
                        listBox1.Items.AddRange(newTracks.ToArray());
                        listBox1.SelectedIndex = 0;


                        tracks.Clear();
                        tracks.AddRange(newTracks);

                        playlists[playlistName] = newTracks;
                        listBox2.Items.Add(playlistName);
                        listBox2.SelectedItem = playlistName;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error loading playlist: " + ex.Message, "Error");
                }
            }
        }

        private void button18_Click(object sender, EventArgs e) //delete all
        {
            string playlistFolder = Path.Combine(System.Windows.Forms.Application.StartupPath, "Playlists");

            if (Directory.Exists(playlistFolder))
            {
                foreach (string file in Directory.EnumerateFiles(playlistFolder, "*.txt"))
                {
                    System.IO.File.Delete(file);
                }

                listBox2.Items.Clear();
                listBox2.SelectedIndex = -1;

                System.Windows.Forms.MessageBox.Show("All playlists deleted successfully.");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Playlists folder not found.");
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    

        public void UpdateTrackInfo(string title, string artist, string album, string year)
        {
            label21.Text = title;
            label22.Text = artist;
        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }
    }
}