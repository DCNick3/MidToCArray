namespace WaveToCArray
{
    internal class NoteTwo
    {
        public int length;
        public int frequency;
        public bool isBass = false;

        public override string ToString()
        {
            return string.Format("{0} {1}", frequency, length);
        }
    }
}