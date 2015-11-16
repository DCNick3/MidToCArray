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
            DoFileConvert(file);
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
        
        private static void DoFileConvert(string file)
        {
            MidiFile midiFile = new MidiFile(file);
            Console.WriteLine("Converting...");
            TimeSpan startTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            MusicTwo music = MusicTwo.FromMidiFile(midiFile);
            Console.Write("Saving... ");
            music.SaveAsCFiles(file);
            Console.WriteLine("Done in {0}!", TimeSpan.FromTicks(DateTime.Now.Ticks) - startTime);
        }
    }
}
