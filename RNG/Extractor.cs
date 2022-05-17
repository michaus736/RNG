using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Utils;
using NAudio.Wave;


namespace RNG
{
    internal class Extractor : IWaveIn
    {

        const int SAMPLE_RATE = 44100;
        public const int BUFFER_SIZE = 900000;
        const int SAMPLE_ARRAY_SIZE = BUFFER_SIZE * 3;
        const int MILISTOWAIT = 2000;
        const int OFFSET = 80000;
        const int CHUNK = 80000;





        private WaveInEvent waveIn=new WaveInEvent();
        byte[] buffer= new byte[BUFFER_SIZE];
        MemoryStream memoryStream=new MemoryStream(BUFFER_SIZE);
        

        
        public WaveFormat WaveFormat { get; set; }

        public event EventHandler<WaveInEventArgs> DataAvailable;
        public event EventHandler<StoppedEventArgs> RecordingStopped;
        


        public Extractor()
        {
            
            DataAvailable = WaveIn_DataAvailable;
            RecordingStopped = WaveIn_RecordingStopped;

            this.WaveFormat = new WaveFormat(SAMPLE_RATE, WaveIn.GetCapabilities(0).Channels);
            this.waveIn.WaveFormat = this.WaveFormat;
            this.waveIn.DeviceNumber = 0;
            this.waveIn.DataAvailable += DataAvailable;
            this.waveIn.BufferMilliseconds = 1000;
            this.waveIn.RecordingStopped += RecordingStopped;


        }

        public async Task GetSamples()
        {

            StartRecording();

            Thread.Sleep(MILISTOWAIT);

            StopRecording();
            

            buffer = memoryStream.GetBuffer()[OFFSET..((int)memoryStream.Length)];
            
            await memoryStream.FlushAsync();
        }

        public void Parse()
        {
            buffer=buffer.Where((x, idx) => (long)idx%2!=1).ToArray();//getting buffer without extra 0 and 255
        }
        
        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            //TODO
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
        }

        public void Dispose()
        {
            if(waveIn!=null)
                waveIn.Dispose();
            if (memoryStream != null)
            {
                memoryStream.Dispose();
            }
        }

        public void StartRecording()
        {
            this.waveIn.StartRecording();
        }

        public void StopRecording()
        {
            this.waveIn.StopRecording();
        }
        
        public byte[] getBuffer()
        {
            return this.buffer;
        }

        public static void printAudioInputDevices()
        {
            ;
            for(int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var item = WaveIn.GetCapabilities(i);
                Console.WriteLine("{0}: {1}{2}"
                    ,i,
                    item.ProductGuid.ToString(),
                    item.ProductName
                    );
            }

        }


        
    }
}
