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
    internal class MicrophoneHandler : IWaveIn
    {
        const int SAMPLE_RATE = 44100;
        private WaveInEvent waveIn=new WaveInEvent();
        byte[] buffer= new byte[SAMPLE_RATE];
        MemoryStream memoryStream=new MemoryStream(SAMPLE_RATE);
        WaveFileWriter writer;
        

        
        public WaveFormat WaveFormat { get; set; }

        public event EventHandler<WaveInEventArgs> DataAvailable;
        public event EventHandler<StoppedEventArgs> RecordingStopped;
        


        public MicrophoneHandler()
        {
            Init();
            

        }

        public async Task getNextSamples()
        {

            StartRecording();

            Thread.Sleep(200);

            StopRecording();

            buffer = memoryStream.GetBuffer();

            await memoryStream.FlushAsync();
        }
        
        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            Dispose();
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            //writer.Write(e.Buffer, 0, e.BytesRecorded);
            memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
        }

        public void Dispose()
        {
            if(waveIn!=null)
                waveIn.Dispose();
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
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
        

        private void Init()
        {
            DataAvailable = WaveIn_DataAvailable;
            RecordingStopped = WaveIn_RecordingStopped;

            this.WaveFormat = new WaveFormat(SAMPLE_RATE, WaveIn.GetCapabilities(0).Channels);
            this.waveIn.WaveFormat = this.WaveFormat;
            this.waveIn.DeviceNumber = 0;
            this.waveIn.DataAvailable += DataAvailable;
            this.waveIn.RecordingStopped += RecordingStopped;


            this.writer = new WaveFileWriter(new IgnoreDisposeStream(memoryStream), WaveFormat);
            
        }
    }
}
