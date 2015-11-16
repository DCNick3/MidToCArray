using System.Collections.Generic;

namespace WaveToCArray
{
    internal class Track
    {
        public Track()
        {
            notes = new List<Note>();
        }
        public List<Note> notes;
        internal int instrument;
    }
}