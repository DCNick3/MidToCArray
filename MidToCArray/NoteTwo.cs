using System;

namespace MidiToCArray
{
    public class NoteTwo
    {
        /// <summary>
        /// Gets or sets lngth of the note
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }
        private int length;

        /// <summary>
        /// Gets or sets note frequency
        /// </summary>
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }
        private int number;

        /// <summary>
        /// Gets or sets isBass value
        /// </summary>
        public bool IsBass
        {
            get
            {
                return isBass;
            }
            set
            {
                isBass = value;
            }
        }

        public ushort Frequency
        {
            get
            {
                if (number != 0)
                    return (ushort)(27.5 * Math.Pow(2.0, (number - 21.0) / 12.0));
                else
                    return 0;
            }
        }

        private bool isBass = false;

        /// <summary>
        /// Converts note to string with info
        /// Contains Frequency and length
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", number, length);
        }
    }
}