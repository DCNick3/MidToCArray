using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace MidiToCArray
{
    public class SquareWaveProvider : WaveProvider32
    {
        /// <summary>
        /// Creates new square wave generator with frequency = 1000 Hz
        /// </summary>
        public SquareWaveProvider() : this(1000)
        {}
        /// <summary>
        /// Creates new square wave generator with specified frequency
        /// </summary>
        /// <param name="frequency"></param>
        public SquareWaveProvider(int frequency)
        {
            Frequency = frequency;
        }

        /// <summary>
        /// Gets or sets generator frequency
        /// </summary>
        public int Frequency
        {
            set
            {
                if (value != 0)
                {
                    halfPeriod = (WaveFormat.SampleRate / value);
                    enabled = true;
                }
                else
                    enabled = false;
            }
        }

        bool enabled = true;
        int halfPeriod = 0;

        long pos = 0;
        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            if (enabled)
                for (int i = 0; i < sampleCount; i++)
                {
                    pos++;
                    buffer[offset + i] = ((pos / halfPeriod) % 2) * 2.0F - 1.0F;
                } 
            return sampleCount;
        }
    }
    /*
    public class SquareWaveProvider : WaveProvider32
    {
        public SquareWaveProvider()
        {
            SetWaveFormat(44100, 1);
            frequency = 1000; // <--  This is your frequency
            Amplitude = 0.25f; // let's not hurt our ears            
        }

        float frequency;
        public float Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
                chng = (WaveFormat.SampleRate / (int)frequency);
            }
        }
        public float Amplitude { get; set; }

        int chng;
        //float attenuation = 1.0000000F;

        public void Reset()
        {
            //attenuation = 1.0000000F;
        }

        long pos = 0;

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            for (int n = 0; n < sampleCount; n++)
            {
                //buffer[n + offset] = (float)((Amplitude) * Math.Sin((2 * Math.PI * sample * frequency) / sampleRate));
                if ((pos / chng) % 2 == 0)
                {
                    buffer[n + offset] = Amplitude * 1;
                }
                else
                {
                    buffer[n + offset] = Amplitude * -1;
                }
                pos++;
                //if (attenuation > 0)
                // attenuation -= 0.0000625F;
            }
            return sampleCount;
        }
    }*/
}
