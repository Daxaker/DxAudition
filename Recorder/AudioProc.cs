using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.IO;
using Microsoft.Win32;


namespace Recorder
{
    class AudioProc
    {
        public AudioProc(string filename)
        {
            this.FileName = filename;
        }

        private enum RecordingState
        {
            Stoped,
            Recording
        }

        public string FileName;
        private WaveFileWriter writer;
        private RecordingState recordingState;
        private WaveIn waveIn;
        public delegate void pitchDetected();
        public event pitchDetected pitchDetectedEvent;
        private string[] Notes = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "B", "H" };
        public float diff = 0;
        public string Note {get; set; }
        public string Octave { get; set; }
        public float Pitch { get; set; }
        
        public void waveInStart(int device)
        {
            
            waveIn = new WaveIn();
            waveIn.DeviceNumber = device;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(44100, 1);
            writer = new WaveFileWriter(FileName, waveIn.WaveFormat);
            waveIn.StartRecording();
            recordingState = RecordingState.Recording;
                     
        }
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            List<float> samples = new List<float>();
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            WriteToFile(buffer, bytesRecorded);
            
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                float sample32 = sample / 32768f;
                samples.Add(sample32);
            }
            float[] smpl = samples.ToArray();
            Pitch = DetectPitch(60, 1200, smpl);
            int noteIndex = detectNote(Pitch);
            if (noteIndex >= 0 && noteIndex <= 11)
            Note = Notes[noteIndex];
            pitchDetectedEvent();
            samples.Clear();
           
        }
        private void WriteToFile(byte[] buffer, int bytesRecorded)
        {
            if (recordingState == RecordingState.Recording)
            {
                writer.WriteData(buffer, 0, bytesRecorded);
            }
            else { waveInStop(); }
                        
         
        }
        public void waveInStop()
        {
           waveIn.StopRecording();
           recordingState = RecordingState.Stoped;
           writer.Dispose();
        }
        public void SaveWaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WAV file (.wav)|*.wav";
            saveFileDialog.DefaultExt = ".wav";
            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SaveAs(saveFileDialog.FileName);
            }
        }
        private void SaveAs(string fileName)
        {
            if (File.Exists(fileName)) File.Delete(fileName); 
            File.Copy(FileName,fileName);
            File.Delete(FileName);
        }
        private float DetectPitch(int minFreq, int maxFreq, float[] source)
        {
            const int sampleRate = 44100; //Максимальное расстояние в обратном направлении, на которое смещаемся в поиске совпадения
            int maxOffset = sampleRate / minFreq; //Минимальное расстояние в обратном направлении, на которое смещаемся в поиске совпадения
            int minOffset = sampleRate / maxFreq; //Количество фреймов - для фрейма в 4 байта(float)
            int frames = source.Length / 2;
            float[] prevBuffer = new float[frames];
            for (int n = 0; n < frames; n++)
            {
                prevBuffer[n] = source[n];
            }

            float maxCorr = 0;
            int maxLag = 0;
            // начинаем с низких частот и заканчиваем высокими
            //анализируем все возможные целочисленные смещения в нашем диапазоне и вычисляем коррелирующее значение
            for (int lag = maxOffset; lag >= minOffset; lag--)
            {
                float corr = 0; //  сумма квадратов

                //Корреляция вычисляется как сумма квадратов
                for (int i = 0; i < frames; i++)
                {
                    int oldIndex = i - lag;
                    try
                    {
                     float sample = ((oldIndex < 0) ? prevBuffer[frames + oldIndex] : prevBuffer[oldIndex]);
                     corr += (sample * source[i]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        
                        continue;
                    }
                }
                if (corr > maxCorr)
                {
                   maxCorr = corr;
                    maxLag = lag;
                }

            }

            float noiseThreshold = frames / 1000f;

            if (maxCorr < noiseThreshold || maxLag == 0) return 0.0f;
            return (float)sampleRate / maxLag;

        }
        
		private int detectNote(float pitch)
        {
            
            float exp = pitch / 65.41f;
            float freq = (float)Math.Log((double)exp, 2)*12;
            Octave = "3";
            if (pitch < 1046.50f) Octave = "2";
            if (pitch < 523.25f) Octave = "1";
            if (pitch < 261.63f) Octave = "M";
            if (pitch < 130.82f) Octave = "B";
            //if(tmp >=0 && tmp < 192)
            //diff = tmp - (float)Math.Round(tmp);
            return (int)Math.Round(freq % 12);
        }
                
    } 
}
