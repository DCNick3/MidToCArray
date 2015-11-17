using System;

namespace MidiToCArray
{
    internal class Note
    {
        public int time;
        public int number;
        public int frequency { get { if (number != 0) return (int)(27.5 * Math.Pow(2.0, (number - 21.0) / 12.0)); else return 0; } }
        public int length;
        public int instrument;

        public override string ToString()
        {
            return string.Format("Note: {0}, Frequency: {1}, Length: {2}, Time: {3}, Instrument:{4}", number, frequency, length, time, instrument);
        }
    }
}