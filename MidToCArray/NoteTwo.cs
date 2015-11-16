namespace WaveToCArray
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
        public int Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
            }
        }
        private int frequency;

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
        private bool isBass = false;

        /// <summary>
        /// Converts note to string with info
        /// Contains Frequency and length
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", frequency, length);
        }
    }
}