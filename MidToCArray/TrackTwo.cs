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

        /// <summary>
        /// Converts track to C-code
        /// </summary>
        /// <returns>Converted C-code</returns>
        public string ToCCode(int version)
        {
            if (version == 1 || version == 0)
            {
                StringBuilder cArray = new StringBuilder();
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
                ms.Position = 0;
                BinaryReader br = new BinaryReader(ms);
                cArray.Append("//Music start!\n");
                cArray.Append(string.Format("#define MUSIC_LENGTH {0}\n", notes.Count));
                cArray.Append("const PROGMEM uint8_t music[] = { ");
                cArray.Append(version);
                if (version == 1)
                    for (int i = 0; i < notes.Count * 3; i++)
                    {
                        cArray.Append(string.Format(", {0}", br.ReadByte()));
                    }
                else
                    for (int i = 0; i < notes.Count * 2; i++)
                    {
                        cArray.Append(string.Format(", {0}", br.ReadByte()));
                        cArray.Append(string.Format(", {0}", br.ReadByte()));
                    }
                cArray.Append("};\n\n//Music end!");
                return cArray.ToString();
            }
            return null;
        }
    }
}
