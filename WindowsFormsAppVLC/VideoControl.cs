using LibVLCSharp.Shared;
using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class VideoControl : UserControl
    {
        int volume_value = 0;
        private Queue<ushort> curVols = new Queue<ushort>();
        private readonly MediaPlayer _mp;
        public VideoControl(LibVLC _libVLC, string sUri, string title)
        {

            WaveOutEvent waveOut = new WaveOutEvent();
            BufferedWaveProvider waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2));

            // 设置音频回调
            //HandleAudioPlay(IntPtr.Zero, IntPtr.Zero, 0, 0);

            // 连接BufferedWaveProvider到WaveOutEvent
            waveOut.Init(waveProvider);

            // 开始播放
            waveOut.Play();

            InitializeComponent();
            labelTitle.Text = title;

            Dock = DockStyle.Fill;
            _mp = new MediaPlayer(_libVLC);
            //_mp.SetAudioOutput("directsound");

            //using var outputDevice = new WaveOutEvent();
            //var waveFormat = new WaveFormat(8000, 16, 1);
            //var writer = new WaveFileWriter("sound.wav", waveFormat);
            //var waveProvider = new BufferedWaveProvider(waveFormat);
            //outputDevice.Init(waveProvider);
            //_mp.audio

            byte[] processAudio(byte[] byteArray)
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

            ushort getVol(byte[] byteArray)
            {
                short[] shortArray = new short[byteArray.Length / 2];
                Buffer.BlockCopy(byteArray, 0, shortArray, 0, byteArray.Length);
                short min = 0, max = 0;
                for (short i = 0; i < shortArray.Length; i++)
                {
                    short v = shortArray[i];
                    if (v < min)
                    {
                        min = v;
                    }
                    if (v > max)
                    {
                        max = v;
                    }
                }
                return (ushort)(max - min);
            }

            // 音频回调函数，用于处理音频数据
            void HandleAudioData(IntPtr data, IntPtr samples, uint cc, long pts)
            {
                // 这里是一个示例，你需要替换成实际的音频处理逻辑
                int count = (int)cc * 4;
                byte[] audioData1 = new byte[count];
                Marshal.Copy(samples, audioData1, 0, count);
                //curVol = (double)getVol(audioData1) / 65536;
                curVols.Enqueue(getVol(audioData1));
                //Console.WriteLine(vol);
                byte[] audioData2 = processAudio(audioData1);
                // 将音频数据添加到BufferedWaveProvider中
                waveProvider.AddSamples(audioData2, 0, audioData2.Length);
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
            _mp.Stop();
            _mp.Dispose();
        }

        static double VolumeToDB(double volumeRatio)
        {
            // 计算dB值（假设最大音量对应0 dB）
            return 20 * Math.Log10(volumeRatio);
        }
        static double VolumeToDBRatio(double db1, double minDB = -60)
        {
            // 输入输出都是0至1范围的值，区别是以DB计算的值
            double db2 = VolumeToDB(db1);
            double db3 = db2 < minDB ? minDB : db2;
            return (db3 - minDB) / -minDB;
        }

        double bufsize = 10;
        private ushort GetCurVol()
        {
            int min = 1, max = 100;
            if (curVols.Count == 0)
            {
                //Console.WriteLine("curVols.Count={0},bufsize={1}", curVols.Count, bufsize);
                return 0;
            }
            if (curVols.Count <= 2)
            {
                //Console.WriteLine("curVols.Count={0},bufsize={1}", curVols.Count, bufsize);
                bufsize++;
            }
            if (curVols.Count < bufsize)
            {
                // 太快了，要减速
                timer1.Interval = timer1.Interval >= max ? max : (int)Math.Ceiling((double)timer1.Interval * 1.1);
            }
            else
            {
                // 慢了要加速
                bufsize *= 0.999;
                timer1.Interval = timer1.Interval <= min ? min : (int)((double)timer1.Interval * 0.95);
            }
            return curVols.Dequeue();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            volumeControl1.ValueD = VolumeToDBRatio((double)GetCurVol() / 65536);
        }
    }
}
