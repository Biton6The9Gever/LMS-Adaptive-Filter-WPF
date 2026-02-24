using NAudio.Wave;
using ScottPlot;
using System.Windows;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private const int bufferMiliseconds = 100;
        private const int sampleRate = 44100;
        private const int channels = 1;

        //khz * channels * bufferMilisecnods /1000 = number of samples enter in 100 milliseconds
        private float[] inputBuffer = new float[sampleRate * channels * bufferMiliseconds / 1000]; 
        
        //LMS formula: w_n+1 = w_n + 2*mu*e*x
        public MainWindow()
        {
            InitializeComponent();
            StartCapture(); 
        }
        private void StartCapture()
        {
            WaveInEvent waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(sampleRate, channels); // 44.1 kHz, mono
            waveIn.BufferMilliseconds = bufferMiliseconds; // Buffer size

            waveIn.DataAvailable += (s, e) =>
            {
                for(int i = 0; i < e.BytesRecorded; i += 2) // Assuming 16-bit audio
                {
                    short sample = BitConverter.ToInt16(e.Buffer, i);
                    float x = sample / 32768f; // Normalize to [-1, 1]

                    float filteredSample = ProcessLMS(x);

                    UpdateBuffer(x);
                }

                Dispatcher.Invoke(() => {
                    OriginalGraph.Plot.Clear();
                    OriginalGraph.Plot.Add.Signal(inputBuffer);
                    OriginalGraph.Refresh();
                });
                
            };
            waveIn.StartRecording();
        }

        private void UpdateBuffer(float newSample)
        {
            //update the input buffer with the new sample, shifting old samples to the left
            Array.Copy(inputBuffer, 1, inputBuffer, 0, inputBuffer.Length - 1);
            inputBuffer[inputBuffer.Length - 1] = newSample;
        }
        //todo
        private float ProcessLMS(float input) => input;
    }
}