using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;
using AdvancedTimers;
using System.IO;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using EventArgsLibrary;
using Utilities;
using Constants;

namespace LogRecorderNs
{
    public class LogRecorder
    {
        private StreamWriter sw;
        private ConcurrentQueue<byte[]> logQueue = new ConcurrentQueue<byte[]>();
        public string logLock = "";

        int subFileIndex = 0;
        DateTime initialDateTime;
        HighFreqTimerV2 timerLogging = new HighFreqTimerV2( ConstVar.ODOMETRY_FREQ_IN_HZ, "LogRecorder");

        bool isLogging = false;
        bool isRecordingFileOpened = false;

        public LogRecorder()
        {
            timerLogging.Tick += TimerLogging_Tick;
            timerLogging.Start();
        }

        private void TimerLogging_Tick(object sender, EventArgs e)
        {
            LogLoop();
        }


        private void StartLogging()
        {
            initialDateTime = DateTime.Now;
            subFileIndex = 0;
            isLogging = true;
            OnLoggingStartEvent?.Invoke(this, new EventArgs());
        }
        private void StopLogging()
        {
            isLogging = false;
            OnLoggingStopEvent?.Invoke(this, new EventArgs());
        }

        
        private void LogLoop()
        {

            if (isLogging)
            {
                /// On est en mode logging

                if (isRecordingFileOpened == false)
                {
                    /// TODO: Implement Security
                    var currentDir = Directory.GetCurrentDirectory();

                    string pattern = @"(.*(?'name'" + Regex.Escape(ConstVar.PROJECT_NAME) + @"))"; // Regex pour la recherche des FTDI 232
                    Match m = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(currentDir);

                    if (m.Success)
                    {
                        string logPath = m.Groups[1].ToString() + "\\" + ConstVar.LOG_FOLDER_NAME + "\\";
                        string currentFileName;

                        if (subFileIndex == 0)
                            currentFileName = logPath + "Log_" + initialDateTime.ToString("yyyy-MM-dd_HH-mm-ss") + "_" + subFileIndex + "_Init.rbt";
                        else
                            currentFileName = logPath + "Log_" + initialDateTime.ToString("yyyy-MM-dd_HH-mm-ss") + "_" + subFileIndex + ".rbt";

                        subFileIndex++;

                        sw = new StreamWriter(currentFileName, true);
                        sw.AutoFlush = true;
                        isRecordingFileOpened = true;

                        OnFileCreation();

                    }
                    else
                    {
                        OnFileErrorName();
                    }

                }

                while (logQueue.Count > 0)
                {
                    byte[] s;
                    while (!logQueue.TryDequeue(out s)) ;

                    sw.WriteLine(Convert.ToBase64String(s));

                    //Vérification de la taille du fichier
                    if (sw.BaseStream.Length > ConstVar.MAX_LOG_FILE_SIZE)
                    {
                        //On ferme le fichier, ce qui a pour conséquence de le splitter
                        sw.Close();
                        isRecordingFileOpened = false;
                        break; //On sort de la boucle de logging puisque le fichier est fermé
                    }
                }
            }
            else
            {
                if (isRecordingFileOpened == true)
                {
                    //On ferme le fichier, ce qui a pour conséquence de le splitter
                    sw.Close();
                    isRecordingFileOpened = false;
                }

                while (logQueue.Count > 0)
                {
                    byte[] s;
                    logQueue.TryDequeue(out s);
                }
            }
        }
        public void Log(byte[] content)
        {
            if (isLogging)
            {
                lock (logLock) // get a lock on the queue
                {
                    logQueue.Enqueue(content);
                }
            }
        }

        

        public void OnEnableDisableLoggingReceived(object sender, BoolEventArgs e)
        {
            if (e.value == true)
            {
                StartLogging();
            }
            else
            {
                StopLogging();
            }
        }

        #region Input Callback
        public void OnRawLidarDataReceived(object sender, RawLidarArgs e)
        {
            RawLidarArgsLog data = new RawLidarArgsLog();
            data.PtList = e.PtList;
            data.RobotId = e.RobotId;
            data.InstantInMs = DateTime.Now.Subtract(initialDateTime).TotalMilliseconds;
            var msg = ZeroFormatterSerializer.Serialize<ZeroFormatterLogging>(data);
            Log(msg);
        }

        public void OnPolarSpeedDataReceived(object sender, PolarSpeedEventArgs e)
        {
            PolarSpeedEventArgsLog data = new PolarSpeedEventArgsLog();
            data.Vx = e.Vx;
            data.Vy = e.Vy;
            data.Vtheta = e.Vtheta;
            data.RobotId = e.RobotId;
            data.timeStampMs = e.timeStampMs;
            data.InstantInMs = DateTime.Now.Subtract(initialDateTime).TotalMilliseconds;
            var msg = ZeroFormatterSerializer.Serialize<ZeroFormatterLogging>(data);
            Log(msg);
        }
        #endregion

        #region Events
        public EventHandler<EventArgs> OnFileErrorNameEvent;
        public EventHandler<EventArgs> OnNewFileCreateEvent;
        public EventHandler<EventArgs> OnLoggingStartEvent;
        public EventHandler<EventArgs> OnLoggingStopEvent;


        public virtual void OnFileErrorName()
        {
            OnFileErrorNameEvent?.Invoke(this, new EventArgs());
        }

        public virtual void OnFileCreation()
        {
            OnNewFileCreateEvent?.Invoke(this, new EventArgs());
        }
        #endregion
    }

    #region Zero Formattable Class
    [ZeroFormattable]
    public class RawLidarArgsLog : ZeroFormatterLogging
    {
        public override ZeroFormatterLoggingType Type
        {
            get
            {
                return ZeroFormatterLoggingType.RawLidarArgs;
            }
        }

        [Index(0)]
        public virtual double InstantInMs { get; set; }
        [Index(1)]
        public virtual int RobotId { get; set; }
        [Index(2)]
        public virtual List<PolarPointRssi> PtList { get; set; }
        [Index(3)]
        public virtual int LidarFrameNumber { get; set; }
    }

    [ZeroFormattable]
    public class PolarSpeedEventArgsLog : ZeroFormatterLogging
    {
        public override ZeroFormatterLoggingType Type
        {
            get
            {
                return ZeroFormatterLoggingType.PolarSpeedEventArgs;
            }
        }
        [Index(0)]
        public virtual double InstantInMs { get; set; }
        [Index(1)]
        public virtual uint timeStampMs { get; set; }
        [Index(2)]
        public virtual int RobotId { get; set; }
        [Index(3)]
        public virtual double Vx { get; set; }
        [Index(4)]
        public virtual double Vy { get; set; }
        [Index(5)]
        public virtual double Vtheta { get; set; }
    }
    #endregion

}

