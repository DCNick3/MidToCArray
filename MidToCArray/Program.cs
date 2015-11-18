using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;
using System.IO;
using System.Threading;
using NAudio.Wave;

namespace MidiToCArray
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter midi file name: ");
            string file = Console.ReadLine();

            MidiFile midiFile = new MidiFile(file);
            Console.WriteLine("Converting...");
            TimeSpan startTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            MusicTwo music = MusicTwo.FromMidiFile(midiFile);
            Console.Write("Saving... ");
            music.SaveAsCFilesBest(file);
            Console.WriteLine("Done in {0}!", TimeSpan.FromTicks(DateTime.Now.Ticks) - startTime);

            ConsoleKeyInfo enteredKey;
            do
            {
                Console.Write("Play track?(Y/N) ");
                enteredKey = Console.ReadKey();
                Console.WriteLine();
            } while (enteredKey.KeyChar != 'Y' & enteredKey.KeyChar != 'N' & enteredKey.KeyChar != 'y' & enteredKey.KeyChar != 'n');
            if (enteredKey.KeyChar == 'Y' || enteredKey.KeyChar == 'y')
                music.Play();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
