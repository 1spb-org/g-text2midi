using LunarLabs.Parser;
using MahApps.Metro.Controls;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LC = Melanchall.DryWetMidi.Interaction.LengthConverter;
using MT = Melanchall.DryWetMidi.MusicTheory;
using IA = Melanchall.DryWetMidi.Interaction;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Melanchall.DryWetMidi.Common;
using OP = OxyPlot;

namespace NiceContainer
{
    using InterNote = Melanchall.DryWetMidi.Interaction.Note;

    public static class _EXT_
    {
        public static NoteName[] Maj3 = { NoteName.C, NoteName.E, NoteName.G }; 
        public static NoteName[] Maj6 = { NoteName.C, NoteName.E, NoteName.G, NoteName.A };
        public static NoteName[] Maj69 = { NoteName.C, NoteName.E, NoteName.G, NoteName.A, NoteName.D};

        public static Array AllNotes => Enum.GetValues(typeof(NoteName));

        public static NoteName[] FromTemplate(this NoteName[] t, NoteName root = NoteName.C)
        {
            var tr = Interval.FromHalfSteps((int)root);
            return t.Select(x => x.Transpose(tr)).ToArray();
        }
    }


    class Sound
    {


        public static void Test()
        {

            var midiFile = new MidiFile();
            TempoMap tempoMap = midiFile.GetTempoMap();

            var trackChunk = new TrackChunk();
            using (var notesManager = trackChunk.ManageNotes())
            {
                NotesCollection notes = notesManager.Notes;

                var g = Enum.GetValues(typeof(NoteName));
                NoteName n = (NoteName)g.GetValue(MainWindow._rnd.Next(g.Length));
                notes.Add(new InterNote(n, 4, LC.ConvertFrom(
                                       new MetricTimeSpan(hours: 0, minutes: 0, seconds: 2), 0, tempoMap)));

                n = (NoteName)g.GetValue(MainWindow._rnd.Next(g.Length));
                notes.Add(new InterNote(n, 3, LC.ConvertFrom(
                                       new MetricTimeSpan(hours: 0, minutes: 0, seconds: 1, milliseconds: 500), 0, tempoMap)));
                n = (NoteName)g.GetValue(MainWindow._rnd.Next(g.Length));

                notes.Add(new InterNote(n, 5, LC.ConvertFrom(
                                       new MetricTimeSpan(hours: 0, minutes: 0, seconds: 3), 0, tempoMap)));
            }

            midiFile.Chunks.Add(trackChunk);

            using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
            using (var playback = midiFile.GetPlayback(outputDevice))
            {
                // playback.Speed = 2.0;
                playback.Play();
            }
        }

        static Random R => MainWindow._rnd;
        static NoteName[] ta = { NoteName.C, NoteName.F, NoteName.ASharp };

        public static void FractalMusik()
        {

            var g = Enum.GetValues(typeof(NoteName));
               
            

            var midiFile = new MidiFile();
            var ch =  midiFile.GetChannels();

            StringBuilder sb = new StringBuilder();
             
             

                var trackChunk = new TrackChunk( new ProgramChangeEvent((SevenBitNumber)(0)));


                sb.Append("Begin of Chunk" + Environment.NewLine);

                int G = 36;
                using (var cm = trackChunk.ManageChords())
                {
                    ChordsCollection chords = cm.Chords;

                    long t, tof = 0;
                int oct0 = 3;
                bool bct = false;

                    NoteName tpz = (NoteName) _EXT_.AllNotes.GetValue(0);

                    for (int i = 0; i < 160; i++)
                    {
                        t = (i * (G+1)) + tof;
                        var n0 = ta[R.Next(ta.Length)];

                        var nn = _EXT_.Maj3.FromTemplate(n0);
                        nn = nn.FromTemplate(tpz);

                        int oct = bct ? oct0 : R.Next(3, 7);

                        var m = string.Join(" ", nn.Select(x => x.ToString().Replace("Sharp", "#")));

                        sb.Append(m + " " +  oct ); 
                        sb.Append(Environment.NewLine);

                        InterNote  []ni  = nn.Select(x => new InterNote(x, oct , G, t))
                             .Concat(new[] { new InterNote(nn.First(), oct + 1, G, t) })
                            .ToArray(); 

                        chords.Add(new IA.Chord(ni));

                        if (i % 8 == 7)
                        {
                            sb.Append(Environment.NewLine);
                        
                            oct0 = oct;
                            bct = R.NextDouble() > 0.5; 
                      //      G = R.Next(25, 45);
                            tof += G+5; 
                            tpz = (NoteName)_EXT_.AllNotes.GetValue(R.Next(oct == 3? 4 : 0,_EXT_.AllNotes.Length));
                        }
                    }
                }

                midiFile.Chunks.Add(trackChunk);
                sb.Append(Environment.NewLine);
                sb.Append("End of Chunk");
                sb.Append(Environment.NewLine);
             

            var appDir = Assembly.GetExecutingAssembly().Location;
            appDir = Path.GetDirectoryName(appDir);
            var file = "Im3" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".mid";
            file = Path.Combine(appDir, file);
            var file2 = file + ".txt";
            file2 = Path.Combine(appDir, file2);

            midiFile.Write(file);

            File.WriteAllText(file2, sb.ToString());

            Process.Start(appDir);
            Process.Start(file2);

            using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
            using (var playback = midiFile.GetPlayback(outputDevice))
            {
                // playback.Speed = 2.0;
                playback.Play();
            }
        }

        private static ITimeSpan TimeSpanGet(int v)
        {
          return  (MetricTimeSpan)TimeSpan.FromMilliseconds(v);
        }

        internal static void Test2()
        {

            var g = Enum.GetValues(typeof(NoteName));

            // CancellationTokenSource cts = new CancellationTokenSource();

            using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
            {

                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Colors.DarkGreen);
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                MetroWindow w = new MetroWindow();
                w.TitleCharacterCasing = CharacterCasing.Normal;
                w.Width = 600;
                w.Height = 400;
                w.Content = grid;

                for (int o = 0; o < 6; o++)
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(57, GridUnitType.Star) });
                for (int i = 0; i < g.Length; i++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48, GridUnitType.Star) });


               // List<Task> L = new List<Task>();
                for (int o = 1; o < 7; o++)
                    for (int i = 0; i < g.Length; i++)
                    {
                        NoteName n = (NoteName)g.GetValue(i);
                        var nn = n.ToString().Replace("Sharp", "#") + o;
                        var b = new Button { Content = nn };
                        if (nn.Contains("#"))
                            b.Background = new SolidColorBrush(Colors.LimeGreen);
                        b.BorderThickness = new Thickness(0);
                        b.Margin = new Thickness(1);
                        
                        Grid.SetRow(b, o - 1);
                        Grid.SetColumn(b, i);
                        var tp = new ToPlay { Note = n, Octave = o, Device = outputDevice };
                        tp.InitPlay();
                        b.Tag = tp;

                        b.Click += B_Click; ;
                        grid.Children.Add(b);

                        // MessageBox.Show(n.ToString());
                    }

                w.ShowDialog();

                /*  List<Task> L = new List<Task>();
                  for (int i = 0; i < g.Length; i++)
                  {
                      NoteName n = (NoteName)g.GetValue(i);
                      L.Add(Task.Run(() => PlayNote(outputDevice, n, 2)));//);
                      MessageBox.Show(n.ToString());
                  }
                  // cts.Cancel();
                  */

               // Task.WaitAll(L.ToArray());
            }
            }

        private static void B_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            ToPlay p = (ToPlay)b.Tag;
            p.Play();
        }

        }

        internal class ToPlay
        {
        private MidiFile _midiFile;

        public NoteName Note { get; set; }
            public int Octave { get; set; }
        public OutputDevice Device { get; internal set; }
        public Playback Playback { get; private set; }

        public void InitPlay()
        {
            _midiFile = new MidiFile();
            TempoMap tempoMap = _midiFile.GetTempoMap();

            var trackChunk = new TrackChunk();
            using (var notesManager = trackChunk.ManageNotes())
            {
                NotesCollection notes = notesManager.Notes;
                var len = LC.ConvertFrom(new MetricTimeSpan(hours: 0, minutes: 0, seconds: 1), 0, tempoMap);

                notes.Add(new InterNote(Note, Octave, len));

            }


            _midiFile.Chunks.Add(trackChunk);

             

        }


         ~ToPlay()
         {
            //Playback.Dispose();
         }

        internal void Play()
        {
            Task.Run(() =>
            {
                using (Playback = _midiFile.GetPlayback(Device))
                {
                    // Playback.MoveToStart(); 
                    Playback.Play();
                    // Application.Current.Dispatcher.Invoke(() =>);

                }
            });


           //Dispatcher.CurrentDispatcher.Invoke(()=> );
        }
    }

}

