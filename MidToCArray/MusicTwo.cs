﻿using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveToCArray
{
    public class MusicTwo
    {
        /// <summary>
        /// Gets or sets tracks of music
        /// </summary>
        public List<TrackTwo> Tracks
        {
            get
            {
                return tracks;
            }
            set
            {
                tracks = value;
            }
        }

        private List<TrackTwo> tracks = new List<TrackTwo>();

        /// <summary>
        /// Creates MusicTwo instance using midi file
        /// </summary>
        /// <param name="midiFile">Midi file to use</param>
        /// <returns>Created MusicTwo instance</returns>
        public static MusicTwo FromMidiFile(string midiFile)
        {
            return FromMidiFile(new MidiFile(midiFile));
        }

        /// <summary>
        /// Creates MusicTwo instance using midi file
        /// </summary>
        /// <param name="midiFile">Midi file to use</param>
        /// <param name="firstNoteMode">Use first note mode. If some notes appears at same time, use the first one, else the last</param>
        /// <returns>Created MusicTwo instance</returns>
        public static MusicTwo FromMidiFile(string midiFile, bool firstNoteMode)
        {
            return FromMidiFile(new MidiFile(midiFile), firstNoteMode);
        }

        /// <summary>
        /// Creates MusicTwo instance using midi file
        /// </summary>
        /// <param name="midiFile">Midi file to use</param>
        /// <returns>Created MusicTwo instance</returns>
        public static MusicTwo FromMidiFile(MidiFile midiFile)
        {
            return FromMidiFile(midiFile, false);
        }

        /// <summary>
        /// Creates MusicTwo instance using midi file
        /// </summary>
        /// <param name="midiFile">Midi file to use</param>
        /// <param name="firstNoteMode">Use first note mode. If some notes appears at same time, use the first one, else the last</param>
        /// <returns>Created MusicTwo instance</returns>
        public static MusicTwo FromMidiFile(MidiFile midiFile, bool firstNoteMode)
        {
            double tempo = 0;
            List<Track> tracks = new List<Track>();

            for (int track_ = 1; track_ <= midiFile.Tracks; track_++)
            {
                Track track__ = new Track();
                foreach (var e in midiFile.Events)
                {
                    foreach (var ee in e)
                    {
                        if (ee is NoteEvent)
                        {
                            NoteEvent eee = (NoteEvent)ee;
                            if (eee is NoteOnEvent)
                            {
                                var eeee = (NoteOnEvent)eee;
                                Note nt = new Note();
                                nt.time = (int)(eeee.AbsoluteTime / (double)midiFile.DeltaTicksPerQuarterNote * tempo);
                                nt.length = 0;
                                if (eeee.OffEvent != null)
                                    nt.length = (int)(eeee.NoteLength / (double)midiFile.DeltaTicksPerQuarterNote * tempo);
                                nt.number = eeee.NoteNumber;
                                nt.instrument = track__.instrument;
                                if (eeee.Channel == track_)
                                    track__.notes.Add(nt);
                            }
                        }
                        if (ee is MetaEvent)
                        {
                            MetaEvent eee = (MetaEvent)ee;
                            if (eee.MetaEventType == MetaEventType.SetTempo)
                            {
                                TempoEvent eeee = (TempoEvent)eee;
                                tempo = eeee.MicrosecondsPerQuarterNote / 1000;
                            }
                        }

                        if (ee is PatchChangeEvent)
                        {
                            PatchChangeEvent eee = (PatchChangeEvent)ee;
                            if (eee.Channel == track_)
                                track__.instrument = eee.Patch;
                        }
                    }
                }
                tracks.Add(track__);
            }
            for (int i = 0; i < tracks.Count; i++)
                if (tracks[i].notes.Count == 0)
                {
                    tracks.RemoveAt(i);
                    i--;
                }



            MusicTwo music = new MusicTwo();
            for (int tracko = 0; tracko < tracks.Count; tracko++)
            {
                Track track = tracks[tracko];
                TrackTwo nts = new TrackTwo();
                NoteTwo ntt = new NoteTwo();
                ntt.Frequency = 0;
                ntt.Length = 0;
                nts.Notes.Add(ntt);
                Note lastNote = (new Note());
                Note en = new Note();
                lastNote.time = 0;
                lastNote.number = 0;
                lastNote.length = 0;
                en.time = 0;
                en.number = 0;
                en.length = 0;
                for (int i = 0; i < track.notes.Last().time + track.notes.Last().length; i++)
                {
                    Note nowNote = en;
                    for (int ii = 0; ii < track.notes.Count; ii++)
                    {
                        if (i >= track.notes[ii].time && i <= track.notes[ii].time + track.notes[ii].length)
                        {
                            nowNote = track.notes[ii];
                            if (firstNoteMode)
                                goto c1;
                        }
                    }
                    c1:
                    if (nts.Notes.Last().Frequency == nowNote.frequency)
                    {
                        nts.Notes.Last().Length++;
                        continue;
                    }
                    else
                    {
                        NoteTwo nttt = new NoteTwo();
                        nttt.Frequency = nowNote.frequency;
                        nttt.Length = 1;
                        nttt.IsBass = isInstrumentBass(nowNote.instrument);
                        nts.Notes.Add(nttt);
                    }
                }
                music.tracks.Add(nts);
            }
            return music;
        }
        
        private static bool isInstrumentBass(int i)
        {
            if ((i >= 32 && i <= 49) || (i >= 112 && i <= 119))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Saves music as multiplue C files
        /// Makes one file for each track
        /// Names it by mask:
        /// {name}.{trackNumber}.c
        /// </summary>
        /// <param name="name">Name without a extension</param>
        public void SaveAsCFiles(string name)
        {
            for (int i = 0; i < tracks.Count; i++)
                File.WriteAllText(name + "." + i + ".c", tracks[i].ToCCode());
        }
    }
}
