using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GT2Midi;
using Newtonsoft.Json;

namespace GT2MidiWin
{
    public partial class FMain : Form
    {

        static string trnd  = "- Random -";
    //  private Dictionary<string, string> _cultures = new Dictionary<string, string>();


        public FMain()
        {
            InitializeComponent();

            textBox1.Text = Example();

            if (Program.Args.Length > 1)
            {
                try { comboBox1.SelectedItem = Program.Args[1].Trim(); } catch { }
            }

            var r = GT2Midi.Magician.AllDevices();
            comboBox1.Items.Add(trnd);
            comboBox1.Items.AddRange(r); 

            if (Program.Args.Length > 0)
            {
                try {
                    textBox1.Text = File.ReadAllText ( Program.Args[0].Trim() );
                    Speak();
                }
                catch
                {

                    textBox1.Text = Example();
                    Speak();
                }
            }
        }

        private string Example()
        {
            var track = new List<string>{
        "L50 *4 /C--3E--3G *3 /2G-A _ ",
        "_ /2G--3A /3G--4A L500 /4G /5G  L122  /A",
        "_ _ _ _ L77 *4 /C--3E--3G *3 /2G-A _",
        " _ /2G--3A /3G--4A L500 /4G /5G  L122  /A" 
        };

            Random r = new Random((int) DateTime.Now.Ticks);
            

            T2MJsonObject obj = new T2MJsonObject
            {
                Name = "Example",
                Description = "An example of GM2T. See https://github.com/1spb-org/g-text2midi for help",
                Tracks = new List<MidiTrack>  {
                
                new MidiTrack
                        { Commands = track, Instrument = r.Next(0,127), Interval = 60, Channel = 0 },

                new MidiTrack
                        { Commands = track, Instrument = r.Next(0,127), Interval = 60, Start = 700, Channel = 1}
                }
            };
            return JsonConvert.SerializeObject(obj, Formatting.Indented);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Speak();
        }

        void Speak()
        {
            var voice = comboBox1.SelectedItem?.ToString();
             voice = voice == trnd ? null : voice;
 
 
            if (textBox1.Text.Length == 0)
            {
                 textBox1.Text = Example();                 
            }

            Task.Run(() => { 

            try
            {
                Magician.ParseAndPlay(textBox1.Text, voice);
            }
            catch(Exception ex)
             {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);   
             }
        });

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {
              // o.InitialDirectory = "c:\\";
                o.Filter = "GText2Midi JSON files (*.gt2m.json)|*.gt2m.json| JSON files (*.json)|*.json|All files (*.*)|*.*";
                o.FilterIndex = 1;
                o.RestoreDirectory = true;

                if (o.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        textBox1.Text = File.ReadAllText(o.FileName);
                        Speak();
                    }
                    catch
                    {
                        textBox1.Text = "Error! Can't open the file!";
                        Speak();
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var a = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
            var v = a.Version;
            var dr = MessageBox.Show(this, $"George's Text2Midi ver. {v} ){Environment.NewLine}" +
                $"Navigate to 1spb-org GitHub repo?", Text, MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
                System.Diagnostics.Process.Start("https://github.com/1spb-org/g-text2midi");
        }
    }
}
