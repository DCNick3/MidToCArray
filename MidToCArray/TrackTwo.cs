using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiToCArray
{
    public class TrackTwo
    {
        /// <summary>
        /// Gets or sets notes list
        /// </summary>
        public List<NoteTwo> Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
            }
        }
        private List<NoteTwo> notes = new List<NoteTwo>();

        /// <summary>
        /// Gets length of track
        /// </summary>
        public TimeSpan Length {
            get
            {
                int l = 0;
                foreach (var note in notes)
                    l += note.Length;
                return TimeSpan.FromMilliseconds(l);
            }
        }

        /// <summary>
        /// Converts track to string with info
        /// Writes Notes Count and Length
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            return string.Format("NotesCount: {0}, Length: {1}", notes.Count, Length);
        }

        class NoteTwoCount : IComparable
        {
            internal int length;
            internal int number;
            internal bool isBass;
            internal int count;
            internal bool isEqual(NoteTwo nt)
            {
                return (nt.IsBass == isBass)&(nt.Length == length)&(nt.Number == number);
            }

            public int CompareTo(object obj)
            {
                return count.CompareTo(((NoteTwoCount)obj).count);
            }
        }

        /// <summary>
        /// Converts track to C-code
        /// selects version automatically
        /// </summary>
        /// <returns>Converted C-code</returns>
        public string ToCCodeBest()
        {
            List<byte[]> converted = new List<byte[]>();
            for (int i = 0; i < 6; i++)
            {
                converted.Add(GetAsByteArray(i));
            }
            int smallestI = -1;
            int smallest = int.MaxValue;
            for (int i = 0; i < converted.Count; i++)
            {
                if (converted[i].Length < smallest)
                {
                    smallest = converted[i].Length;
                    smallestI = i;
                }
            }
            return ToCCode(smallestI);
        }

        /// <summary>
        /// Converts track to C-code
        /// </summary>
        /// <param name="version">Version to use</param>
        /// <returns>Converted C-code</returns>
        public string ToCCode(int version)
        {
            StringBuilder cArray = new StringBuilder();
            MemoryStream ms = new MemoryStream(GetAsByteArray(version));
            BinaryReader br = new BinaryReader(ms);
            cArray.Append("//Music start!\n");
            cArray.Append(string.Format("#define MUSIC_LENGTH {0}\n", notes.Count));
            cArray.Append("const PROGMEM uint8_t music[] = { ");
            cArray.Append(version);
            for (int i = 0; i < ms.Length; i++)
            {
                cArray.Append(string.Format(", {0}", br.ReadByte()));
            }
            cArray.Append("};\n\n//Music end!");
            return cArray.ToString();
        }

        /// <summary>
        /// Converts track to byte array of specified version
        /// Versions:
        /// 0 - Biggest and basic, codes frequency and duration
        /// 1 - Some optimization, codes note number and duration
        /// 2 - With dictionary with size of 120, good in big files
        /// </summary>
        /// <param name="version">Version of file</param>
        /// <returns>Created file without header</returns>
        private byte[] GetAsByteArray(int version)
        {
            if (version == 1 || version == 0)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                for (int i = 0; i < notes.Count; i++)
                {
                    var ntn = notes[i];
                    if (version == 1)
                        bw.Write((byte)ntn.Number);
                    else
                        bw.Write((ushort)ntn.Frequency);
                    bw.Write((ushort)ntn.Length);
                }
                return ms.ToArray();
            }
            if (version == 2 | version == 3 | version == 4 | version == 5)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                List<NoteTwoCount> dictionary = new List<NoteTwoCount>();
                for (int i = 0; i < notes.Count; i++)
                {
                    var ntn = notes[i];
                    for (int ii = 0; ii < dictionary.Count; ii++)
                        if (dictionary[ii].isEqual(ntn))
                        {
                            dictionary[ii].count++;
                            goto ex0;
                        }
                    var ntctmp = new NoteTwoCount();
                    ntctmp.length = ntn.Length;
                    ntctmp.number = ntn.Number;
                    ntctmp.isBass = ntn.IsBass;
                    ntctmp.count = 1;
                    dictionary.Add(ntctmp);
                    ex0: { }
                }
                dictionary.Sort();
                dictionary.Reverse();
                int dictLen = 0;
                if (version == 2)
                    dictLen = 128;
                if (version == 3)
                    dictLen = 64;
                if (version == 4)
                    dictLen = 32;
                if (version == 5)
                    dictLen = 16;
                for (int i = 0; i < dictLen; i++) //Filling dictionary
                {
                    if (i < dictionary.Count)
                    {
                        bw.Write((byte)dictionary[i].number);
                        bw.Write((ushort)dictionary[i].length);
                    }
                    else
                    {
                        bw.Write((byte)0);
                        bw.Write((ushort)0);
                    }
                }
                if (dictionary.Count > dictLen)
                {
                    for (int i = dictLen; i < dictionary.Count; i++)
                    {
                        dictionary.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < notes.Count; i++)
                {
                    var ntn = notes[i];
                    for (int ii = 0; ii < dictionary.Count; ii++)
                        if (dictionary[ii].isEqual(ntn))
                        {
                            bw.Write((byte)((byte)ii + (byte)0x80));
                            goto ex1;
                        }
                    bw.Write((byte)ntn.Number);
                    bw.Write((ushort)ntn.Length);
                    ex1: { }
                }
                return ms.ToArray();
            }
            return null;
        }
    }
}
