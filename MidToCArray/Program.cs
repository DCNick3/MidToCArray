using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;
using System.IO;
using System.Threading;

namespace WaveToCArray
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter midi file name: ");
            string file = Console.ReadLine();
            MidiFile rd = new MidiFile(file);
            Console.WriteLine("{0} tracks!", rd.Events.Tracks);
            if (rd.Events.Tracks != 0)
            {
                Console.WriteLine("This file is ok!");
                int st;
                var converted = DoFileConvert(rd, out st);
                Console.WriteLine(converted);
                System.IO.File.WriteAllText(file + "." + st + ".c", converted);
            }
            else
            {
                Console.WriteLine("Bad File!");
            }
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
        
        private static string DoFileConvert(MidiFile rd, out int selectedTrack)
        {
            StringBuilder cArray = new StringBuilder();
            double tempo = 0;
            List<Track> tracks = new List<Track>();

            for (int track_ = 1; track_ <= rd.Tracks; track_++)
            {
                Track track__ = new Track();
                foreach (var e in rd.Events)
                {
                    foreach (var ee in e)
                    {
                        if (ee is NoteEvent)
                        {
                            NoteEvent eee = (NoteEvent)ee;
                            if (eee is NoteOnEvent)
                            {
                                var eeee = (NoteOnEvent)eee;
                                Note nt = new Note();
                                nt.time = (int)(eeee.AbsoluteTime / (double)rd.DeltaTicksPerQuarterNote * tempo);
                                nt.length = 0;
                                if (eeee.OffEvent != null)
                                    nt.length = (int)(eeee.NoteLength / (double)rd.DeltaTicksPerQuarterNote * tempo);
                                else
                                    Console.WriteLine("Warning: bad note {0}", eeee);
                                nt.number = eeee.NoteNumber;
                                nt.instrument = track__.instrument;
                                if (eeee.Channel == track_)
                                    track__.notes.Add(nt);
                            }
                        }
                        if (ee is MetaEvent)
                        {
                            MetaEvent eee = (MetaEvent)ee;
                            if (eee.MetaEventType == MetaEventType.SetTempo)
                            {
                                TempoEvent eeee = (TempoEvent)eee;
                                tempo = eeee.MicrosecondsPerQuarterNote / 1000;
                            }
                        }
                        
                        if (ee is PatchChangeEvent)
                        {
                            PatchChangeEvent eee = (PatchChangeEvent)ee;
                            if (eee.Channel == track_)
                                track__.instrument = eee.Patch;
                        }
                    }
                }
                foreach (var e in track__.notes)
                {
                    foreach (var ee in track__.notes)
                    {
                        if ((ee.time > e.time && ee.time < e.time + e.length) ||
                            (ee.time + ee.length > e.time && ee.time + ee.length < e.time + e.length))
                        {
                            Console.WriteLine("Can't convert polyphone melody!");
                            Console.WriteLine("Conflicting notes: {0} and {1}", e, ee);
                            //return "";
                        }
                    }
                }
                tracks.Add(track__);
            }
            for (int i = 0; i < tracks.Count; i++)
                if (tracks[i].notes.Count == 0)
                {
                    tracks.RemoveAt(i);
                    i--;
                }
            Console.WriteLine("Real track count: {0}", tracks.Count);
            foreach (var t in tracks)
                Console.WriteLine("Track. NotesCount: {0}", t.notes.Count);
            Console.Write("Select track: ");
            int tracko = int.Parse(Console.ReadLine());
            selectedTrack = tracko;
            Track track = tracks[tracko - 1];
            List<NoteTwo> nts = new List<NoteTwo>();
            NoteTwo ntt = new NoteTwo();
            ntt.frequency = 0;
            ntt.length = 0;
            nts.Add(ntt);
            Note lastNote = (new Note());
            Note en = new Note();
            lastNote.time = 0;
            lastNote.number = 0;
            lastNote.length = 0;
            en.time = 0;
            en.number = 0;
            en.length = 0;
            for (int i = 0; i < track.notes.Last().time + track.notes.Last().length; i++)
            {
                Note nowNote = en;
                for (int ii = 0; ii < track.notes.Count; ii++)
                    if (i >= track.notes[ii].time && i <= track.notes[ii].time + track.notes[ii].length)
                    {
                        nowNote = track.notes[ii];
                    }
                if (nts.Last().frequency == nowNote.frequency)
                {
                    nts.Last().length++;
                    continue;
                }
                else
                {
                    NoteTwo nttt = new NoteTwo();
                    nttt.frequency = nowNote.frequency;
                    nttt.length = 1;
                    nttt.isBass = isInstrumentBass(nowNote.instrument);
                    nts.Add(nttt);
                }
            }

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            for (int i = 0; i < nts.Count; i++)
            {
                var ntn = nts[i];
                bw.Write((ushort)ntn.frequency);
                bw.Write((ushort)ntn.length);
            }
            ms.Position = 0;
            BinaryReader br = new BinaryReader(ms);
            cArray.Append("//Music start!\n");
            cArray.Append(string.Format("#define MUSIC_LENGTH {0}\n", nts.Count));
            cArray.Append("const PROGMEM uint16_t music[] = { ");
            cArray.Append(br.ReadInt16());
            cArray.Append(string.Format(", {0}", br.ReadInt16()));
            for (int i = 2; i < nts.Count * 2; i++)
            {
                cArray.Append(string.Format(", {0}", br.ReadInt16()));
            }
            cArray.Append("};\n\n//Music end!");
            return cArray.ToString();
        }

        private static bool isInstrumentBass(int i)
        {
            if ((i >= 32 && i <= 49) || (i >= 112 && i <= 119))
                return true;
            else
                return false;
        }
    }
}
