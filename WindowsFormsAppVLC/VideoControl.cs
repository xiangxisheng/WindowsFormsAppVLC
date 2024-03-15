using LibVLCSharp.Shared;
using NAudio.Wave;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class VideoControl : UserControl
    {
        int aaa = 0;
        int volume_value = 0;
        private readonly MediaPlayer _mp;
        public VideoControl(LibVLC _libVLC, string sUri)
        {
            WaveOutEvent waveOut = new WaveOutEvent();
            BufferedWaveProvider waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2));

            // 设置音频回调
            //HandleAudioPlay(IntPtr.Zero, IntPtr.Zero, 0, 0);

            // 连接BufferedWaveProvider到WaveOutEvent
            waveOut.Init(waveProvider);

            // 开始播放
            waveOut.Play();


            aaa = 1;
            InitializeComponent();
            Dock = DockStyle.Fill;
            _mp = new MediaPlayer(_libVLC);
            //_mp.SetAudioOutput("directsound");

            //using var outputDevice = new WaveOutEvent();
            //var waveFormat = new WaveFormat(8000, 16, 1);
            //var writer = new WaveFileWriter("sound.wav", waveFormat);
            //var waveProvider = new BufferedWaveProvider(waveFormat);
            //outputDevice.Init(waveProvider);
            //_mp.audio

            byte[] processAudio(byte[]byteArray)
            {
                short[] shortArray = new short[byteArray.Length / 2];
                Buffer.BlockCopy(byteArray, 0, shortArray, 0, byteArray.Length);
                for (int i = 0; i < shortArray.Length; i++)
                {
                    shortArray[i] = (short)((double)shortArray[i] * (double)volume_value / 100);

                }
                byte[] byteArray2 = new byte[byteArray.Length];
                Buffer.BlockCopy(shortArray, 0, byteArray2, 0, byteArray.Length);
                return byteArray2;
            }

            // 音频回调函数，用于处理音频数据
            void HandleAudioData(IntPtr data, IntPtr samples, uint cc, long pts)
            {
                // 这里是一个示例，你需要替换成实际的音频处理逻辑
                int count = (int)cc * 4;
                byte[] audioData1 = new byte[count];
                Marshal.Copy(samples, audioData1, 0, count);
                byte[] audioData2 = processAudio(audioData1);
                // 将音频数据添加到BufferedWaveProvider中
                waveProvider.AddSamples(audioData2, 0, audioData2.Length);
                return;
                if (aaa==0)
                {
                    return;
                }
                // 在这里添加处理音频数据的逻辑
                //Console.WriteLine($"Received audio data: count={count}, pts={pts}");


                int bytes = (int)count * 2; // (16 bit, 1 channel)
                var buffer = new byte[bytes];
                Marshal.Copy(samples, buffer, 0, bytes);

                //waveProvider.AddSamples(buffer, 0, bytes);
                //writer.Write(buffer, 0, bytes);

                // 将 IntPtr 转换为 byte 数组
                byte[] audioData = new byte[count];
                System.Runtime.InteropServices.Marshal.Copy(samples, audioData, 0, (int)count);
                //Console.Write(audioData);
                if (aaa == 0)
                {
                    return;
                }
                if (!IsDisposed)
                {
                    Invoke(new Action(() =>
                    {
                        if (aaa == 0)
                        {
                            return;
                        }
                        volumeControl1.Value = audioData[0];
                    }));
                }
                // 在这里添加显示电平信号的逻辑
            }
            
            _mp.SetAudioCallbacks(HandleAudioData, null, null, null, null);
            /*
            Disposed += delegate (object sender, EventArgs e)
            {
                _mp.Stop();
                _mp.Dispose();
            };*/
            _mp.Hwnd = panel1.Handle;
            Media media = new Media(_libVLC, new Uri(sUri));
            _mp.Play(media);

            media.Dispose();
            trackBar1.ValueChanged += delegate (object sender, EventArgs e)
            {
                //_mp.Volume = ((TrackBar)sender).Value;
                volume_value = ((TrackBar)sender).Value;
            };
            //_mp.Playing += delegate (object sender, EventArgs e)
            //{
            //    _mp.Mute = false;
            //    _mp.Volume = 0;
            //};


        }

        private void _mp_VolumeChanged(object sender, MediaPlayerVolumeChangedEventArgs e)
        {
            if (_mp.Volume == -1)
            {
                _mp.Volume = 0;
            }
            //Console.WriteLine("_mp_VolumeChanged={0}", _mp.Volume);
        }

        public void Stop()
        {
            aaa = 0;
            _mp.Stop();
            _mp.Dispose();
        }

    }
}
