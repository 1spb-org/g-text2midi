using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using IA = Melanchall.DryWetMidi.Interaction;

namespace GT2Midi
{

    using InterNote = IA.Note;


    public class Magician
    {
        static Random R = new Random();
        static NoteName[] ta = { NoteName.C, NoteName.F, NoteName.ASharp };

        public static string [] AllDevices()
        {
            return OutputDevice.GetAll().Select(x => x.Name).ToArray();
        }

        public static void ParseAndPlay(string text,
             string nameSynth = null)
        {
            if (nameSynth == null)
            {
             //    nameSynth = "Microsoft GS Wavetable Synth";
                var devices = AllDevices();
                nameSynth = devices[R.Next(0, devices.Length)];
            }

            var g = Enum.GetValues(typeof(NoteName));

            T2MJsonObject obj = null; 
            
            obj  = JsonConvert.DeserializeObject<T2MJsonObject>(text);


            var midiFile = new MidiFile(); 
                var cmdsplit = "-".ToCharArray();

#if DEBUG
            StringBuilder sb = new StringBuilder();
#endif

            foreach (MidiTrack track in obj.Tracks)
            {
                FourBitNumber channel = (FourBitNumber)(track.Channel & 15);
                int velocity = track.Velocity;
                var instro = new ProgramChangeEvent((SevenBitNumber)track.Instrument);
                instro.Channel = channel;
                var trackChunk = new TrackChunk(instro);                
#if DEBUG
                sb.Append("Begin of Chunk" + Environment.NewLine);
#endif

                int length = 40;
                using (var cm = trackChunk.ManageChords())
                {
                    ChordsCollection chords = cm.Chords;
                   
                    var commands = string.Join(" ", track.Commands).Split(' ', '\t', '\r', '\n');

                    //   NoteName tpz = (NoteName)_EXT_.AllNotes.GetValue(0); // TODO Transpose as cmd


                    int t = track.Start;
                    int i = track.Interval;
                    int oct = track.Octave;
                    string cmd;

                    foreach (string v in commands)
                    {

                        if (v.StartsWith("/"))
                        {
                            cmd = v.TrimStart('/');
                            if (cmd == "_")
                            {  t += i; continue;  }
                            var n0 = cmd.Split(cmdsplit, StringSplitOptions.RemoveEmptyEntries).Select(x => GetNoteName(x));

                            var nn = n0; //.FromTemplate(n0);
                                         //   nn = nn.FromTemplate(tpz);

#if DEBUG
                            var m = string.Join(" ",
                                nn.Select(x => x.Name.ToString().Replace("Sharp", "#") +  
                                " " + (x.Octave == -1 ? oct : x.Octave ) ));

                            sb.Append(m + " /");
                            sb.Append(Environment.NewLine);
#endif

                            InterNote[] ni = nn.Select(x => new InterNote(x.Name, x.Octave == -1 ? oct : x.Octave, length, t)
                             {
                                Channel = channel, 
                                Velocity = (SevenBitNumber)( velocity & 127) } )
                              //  .Concat(new[] { new InterNote(nn.First(), oct + 1, G, t) })
                                .ToArray();

                            var chord = new IA.Chord(ni);
                            chords.Add(chord);
                            t += i;
                        }
                        else
                        if (v.StartsWith("*"))
                        {
                            cmd = v.TrimStart('*');
                            if (int.TryParse(cmd, out int ia))
                                oct = ia;
                        }
                        else
                        if (v.StartsWith("+"))
                        {
                            cmd = v.TrimStart('+');
                            if (int.TryParse(cmd, out int ia))
                                i += ia;
                        }
                        else
                        if (v.StartsWith("-"))
                        {
                            if (int.TryParse(v, out int ia))
                                i += ia;
                        }
                        else
                        if (v.StartsWith("L"))
                        {
                            cmd = v.TrimStart('L');
                            if (int.TryParse(cmd, out int ia))
                                length = ia;
                        }
                        else
                        if (v == "_")
                        { t += i; continue; }


                    }

                    cm.SaveChanges();
                }

                midiFile.Chunks.Add(trackChunk);
#if DEBUG
                sb.Append(Environment.NewLine);
                sb.Append("End of Chunk");
                sb.Append(Environment.NewLine);
#endif
            }

            var appDir = Assembly.GetExecutingAssembly().Location;
            appDir = Path.GetDirectoryName(appDir);
            var file = "Im3" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".mid";
            file = Path.Combine(appDir, file);
            midiFile.Write(file);
            Process.Start(appDir);

#if DEBUG

            var file2 = file + ".txt";
            file2 = Path.Combine(appDir, file2);
            File.WriteAllText(file2, sb.ToString());
            Process.Start(file2);
#endif

            using (var outputDevice = OutputDevice.GetByName(nameSynth))
            using (var playback = midiFile.GetPlayback(outputDevice))
            {
                // playback.Speed = 2.0;
                playback.Play();
            }
        }

        private static NoteParsed GetNoteName(string x)
        {
            int octave = -1;
            if (char.IsDigit(x[0]))
            {
                octave = int.Parse(x.Substring(0, 1));
                x = x.Remove(0, 1);
            }
            switch(x)
            {
                case "C": return  new NoteParsed { Octave = octave, Name = NoteName.C };
                case "C#": return new NoteParsed { Octave = octave, Name = NoteName.CSharp};
                case "D": return new NoteParsed { Octave = octave, Name = NoteName.D};
                case "D#": return new NoteParsed { Octave = octave, Name = NoteName.DSharp};
                case "E": return new NoteParsed { Octave = octave, Name = NoteName.E};
                case "F": return new NoteParsed { Octave = octave, Name = NoteName.F};
                case "F#": return new NoteParsed { Octave = octave, Name = NoteName.FSharp};
                case "G": return new NoteParsed { Octave = octave, Name = NoteName.G};
                case "G#": return new NoteParsed { Octave = octave, Name = NoteName.GSharp};
                case "A": return new NoteParsed { Octave = octave, Name = NoteName.A};
                case "A#": return new NoteParsed { Octave = octave, Name = NoteName.ASharp};
                case "B": return new NoteParsed { Octave = octave, Name = NoteName.B };
            }
            return null;
        }
    }

    internal class NoteParsed
    {
        public int Octave { get; set; }
        public NoteName Name { get; set; }
    }

    public class MidiTrack
    {
        public int Start { get; set; } = 0;
        public int Interval { get; set; } = 40;
        public int Instrument { get; set; } = 0;
        public List<string> Commands { get; set; }
        public int Octave { get; set; } = 3;
        public int Channel { get; set; } = 0;
        public int Velocity { get; set; } = 64;
    }

    public class T2MJsonObject
    {
        public List<MidiTrack> Tracks { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public static class _EXT_
    {
        public static NoteName[] Maj3 = { NoteName.C, NoteName.E, NoteName.G };
        public static NoteName[] Maj6 = { NoteName.C, NoteName.E, NoteName.G, NoteName.A };
        public static NoteName[] Maj69 = { NoteName.C, NoteName.E, NoteName.G, NoteName.A, NoteName.D };

        public static Array AllNotes => Enum.GetValues(typeof(NoteName));

        public static IEnumerable<NoteName> FromTemplate(this IEnumerable<NoteName> t, NoteName root = NoteName.C)
        {
            var tr = Interval.FromHalfSteps((int)root);
            return t.Select(x => x.Transpose(tr));
        }
    }


}
