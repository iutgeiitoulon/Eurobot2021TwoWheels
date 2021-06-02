using AdvancedTimers;
using EventArgsLibrary;
using LogRecorderNs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using ZeroFormatter;
using Constants;

namespace LogReplayNs
{
    public class LogReplay
    {
        //private Thread replayThread;
        //ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        //ManualResetEvent _pauseEvent = new ManualResetEvent(true);        //The true parameter tells the event to start out in the signaled state.
        private StreamReader sr;
        public string logLock = "";        
        string filePath = "";
        
        DateTime initialDateTime;
        double? LogDateTimeOffsetInMs = null;
        double speedFactor = 1;

        bool replayModeActivated = false;

        HighFreqTimerV2 timerReplayV2 = new HighFreqTimerV2(200, "LogReplay");
        Stopwatch sw;

        public LogReplay()
        {
            timerReplayV2.Tick += TimerReplay_Tick;
            timerReplayV2.Start();
            initialDateTime = DateTime.Now;
            sw = new Stopwatch();
            sw.Start();
        }

        private void TimerReplay_Tick(object sender, EventArgs e)
        {
            ReplayLoop();
        }

        public void OnEnableDisableLogReplayEvent(object sender, BoolEventArgs e)
        {
            replayModeActivated = e.value;

            if(replayModeActivated)
            {
                if (isReplayingFileSerieDefined == false)
                {
                    /// Défini le path des fichiers de logReplay
                    var currentDir = Directory.GetCurrentDirectory();

                    string pattern = @"(.*(?'name'" + Regex.Escape(ConstVar.PROJECT_NAME) + @"))";
                    Match m = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(currentDir);

                    if (m.Success)
                    {
                        //On a trouvé le path
                        string path = m.Groups[1].ToString();
                        var logPath = path + "\\" + ConstVar.LOG_FOLDER_NAME + "\\";
                        /// Ouvre une boite de dialog pour demander le fichier à ouvrir
                        /// 
                        /// ATTENTION : le OpenFileDialog ne doit surtout pas être placé dans la routine du Timer, sinon, ça ne fonctionne pas : FUITE MEMOIRE !!!
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.InitialDirectory = logPath;
                        openFileDialog.Filter = "Log files |*_0_Init.rbt";


                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            filePath = openFileDialog.FileName;
                            replayFileSerieName = filePath.Substring(0, filePath.Length - 10);
                            isReplayingFileSerieDefined = true;
                            subFileIndex = 0; //On réinit le compteur de sous-fichier pour les multi files chainés
                        }
                    }
                }
            }
        }

        bool isReplayingFileSerieDefined = false;
        string replayFileSerieName = "";
        bool isReplayingFileOpened = false;

        double lastDataTimestamp = 0;
        int subFileIndex = 0;
        bool replayLoopInProgress = false;

        double? currentTimestamp = 0;
        private void ReplayLoop()
        {
            if(!replayLoopInProgress)
            {
                replayLoopInProgress = true;
                if (replayModeActivated)
                {        
                    if (isReplayingFileSerieDefined)
                    {
                        //Si le nom de la série de fichier à traiter est défini
                        if (!isReplayingFileOpened)
                        {
                            //Si aucun fichier n'est ouvert, on ouvre le courant
                            if (subFileIndex == 0)
                                filePath = replayFileSerieName + "0_Init.rbt";
                            else
                                filePath = replayFileSerieName + subFileIndex + ".rbt";
                            if (File.Exists(filePath))
                            {
                                sr = new StreamReader(filePath);
                                isReplayingFileOpened = true;
                                OnFileNameChange(filePath);
                                if (subFileIndex == 0)
                                {
                                    initialDateTime = DateTime.Now;
                                    lastDataTimestamp = 0;
                                }
                            }
                            else
                            {
                                //On n'a plus de fichier à traiter
                                isReplayingFileOpened = false;
                                isReplayingFileSerieDefined = false;
                                replayModeActivated = false;
                            }
                        }

                        if (isReplayingFileOpened)
                        {
                            int successiveDataCounter = 0;

                            //Récupère l'instant courant de la dernière data du replay
                            currentTimestamp = DateTime.Now.Subtract(initialDateTime).TotalMilliseconds + LogDateTimeOffsetInMs;
                            if (currentTimestamp == null)
                                currentTimestamp = 0;

                            //On le fait tant que l'instant courant des data de Replay est inférieur à l'instant théorique de simulation, on traite les datas
                            while (lastDataTimestamp <= currentTimestamp * speedFactor && isReplayingFileOpened && successiveDataCounter < 10)                           
                            {
                                var s = sr.ReadLine();

                                if (s != null)
                                {
                                    byte[] bytes = Convert.FromBase64String(s);
                                    var deserialization = ZeroFormatterSerializer.Deserialize<ZeroFormatterLogging>(bytes);

                                    switch (deserialization.Type)
                                    {
                                        case ZeroFormatterLoggingType.PolarSpeedEventArgs:
                                            PolarSpeedEventArgsLog robotSpeedData = (PolarSpeedEventArgsLog)deserialization;
                                            lastDataTimestamp = robotSpeedData.InstantInMs;
                                            var eSpeed = new PolarSpeedEventArgs();
                                            eSpeed.RobotId = robotSpeedData.RobotId;
                                            eSpeed.Vtheta = robotSpeedData.Vtheta;
                                            eSpeed.Vx = robotSpeedData.Vx;
                                            eSpeed.Vy = robotSpeedData.Vy;
                                            eSpeed.timeStampMs = robotSpeedData.timeStampMs;
                                            OnSpeedData(eSpeed);
                                            break;
                                        case ZeroFormatterLoggingType.RawLidarArgs:
                                            RawLidarArgsLog currentLidarLog = (RawLidarArgsLog)deserialization;
                                            lastDataTimestamp = currentLidarLog.InstantInMs;
                                            OnLidar(currentLidarLog.RobotId, currentLidarLog.PtList);
                                            break;
                                        default:
                                            break;
                                    }

                                    if (LogDateTimeOffsetInMs == null)
                                        LogDateTimeOffsetInMs = lastDataTimestamp;

                                    successiveDataCounter++;
                                }
                                else
                                {
                                    //Lecture échouée, on a une fin de fichier
                                    isReplayingFileOpened = false;
                                    sr.Close();
                                    subFileIndex++;
                                }

                                //Récupère l'instant courant de la dernière data du replay
                                currentTimestamp = (double) (DateTime.Now.Subtract(initialDateTime).TotalMilliseconds + LogDateTimeOffsetInMs);
                            }
                        }
                    }
                }
                replayLoopInProgress = false;
            }
        }

        #region Events
        public event EventHandler<RawLidarArgs> OnLidarEvent;
        public event EventHandler<StringEventArgs> OnUpdateFileNameEvent;
        public event EventHandler<PolarSpeedEventArgs> OnSpeedPolarOdometryFromReplayEvent;

        public virtual void OnLidar(int id, List<PolarPointRssi> ptList)
        {
            OnLidarEvent?.Invoke(this, new RawLidarArgs { RobotId = id, PtList = ptList });
        }
 
        public virtual void OnSpeedData( PolarSpeedEventArgs dat)
        {
            OnSpeedPolarOdometryFromReplayEvent?.Invoke(this, new PolarSpeedEventArgs { Vx = dat.Vx, Vy = dat.Vy, Vtheta = dat.Vtheta, RobotId = dat.RobotId, timeStampMs = dat.timeStampMs });
        }

        
        public virtual void OnFileNameChange(string name)
        {
            OnUpdateFileNameEvent?.Invoke(this, new StringEventArgs { value = name });
        }
        #endregion
    }
}
