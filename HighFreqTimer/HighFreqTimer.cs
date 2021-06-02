using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AdvancedTimers
{
    /// <summary>
    /// Fonctions utiles pour traiter avec le temps.
    /// </summary>
    /// 
    //public static class Time
    //{
    //    /// <summary>Renvoie le temps actuel en millisecondes (selon le système).</summary>
    //    public static long GetSystemTime_MS() => NativeMethods.timeGetTime();
    //    /// <summary>Renvoie le temps actuel en secondes (selon le système).</summary>
    //    public static double GetSystemTime_Sec() => NativeMethods.timeGetTime() / 1000.0;
    //}

    /// <summary>
    /// Timer très haute fréquence géré par le système et fonctionant similairement à <see cref="System.Timers.Timer"/>.
    /// </summary>
    //public class HighFreqTimer
    //{        
    //    private const int TimePeriodic = 1;
    //    private const int EventType = TimePeriodic;

    //    private TimerEventHandler _handler;

    //    /// <summary>
    //    /// Résolu à chaque fin de période <see cref="Interval"/>.
    //    /// </summary>
    //    public event EventHandler Tick;

    //    private int _timerId;

    //    /// <summary>
    //    /// Crée une instance de <see cref="HighFreqTimer"/> avec la fréquence donnée, non démarré.
    //    /// </summary>
    //    /// <param name="freqInHz">Fréquence du timer.</param>
    //    public HighFreqTimer(double freqInHz)
    //        => Interval = (int)(1000 / freqInHz);

    //    private int _interval = 1;
    //    /// <summary>Période du timer (en ms).</summary>
    //    public int Interval
    //    {
    //        get => _interval;
    //        set
    //        {
    //            if (value <= 0) return;
    //            _interval = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Indique l'état de fonctionnement du Timer.
    //    /// </summary>
    //    public bool IsEnabled => _timerId != 0;

    //    /// <summary>
    //    /// Démarre le Timer.
    //    /// </summary>
    //    public void Start()
    //    {
    //        if (_timerId != 0) return;

    //        NativeMethods.timeBeginPeriod(1);
    //        _handler = TimerHandler;
    //        _timerId = NativeMethods.timeSetEvent(_interval, 0, _handler, IntPtr.Zero, EventType);
    //    }

    //    private long _previousTickTime = Time.GetSystemTime_MS();

    //    private void TimerHandler(int id, int msg, IntPtr user, int dw1, int dw2)
    //    {
    //        const double TOLERANCE = 0.95;
    //        long currentTime = Time.GetSystemTime_MS();

    //        if (currentTime - _previousTickTime >= Interval * TOLERANCE)
    //        {
    //            _previousTickTime = currentTime;
    //            Tick?.Invoke(this, new EventArgs());
    //        }
    //    }

    //    /// <summary>
    //    /// Arrête le Timer.
    //    /// </summary>
    //    public void Stop()
    //    {
    //        if (_timerId == 0) return;

    //        var err = NativeMethods.timeKillEvent(_timerId);
    //        NativeMethods.timeEndPeriod(1);
    //        _timerId = 0;
    //    }

    //    /// <summary>Arrête le Timer lors de sa destruction.</summary>
    //    ~HighFreqTimer() => Stop();
    //}

    //internal delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
    //internal static class NativeMethods
    //{
    //    [DllImport("winmm.dll")]
    //    internal static extern int timeGetTime();

    //    [DllImport("winmm.dll")]
    //    internal static extern int timeSetEvent(int delay, int resolution, TimerEventHandler handler, IntPtr user, int eventType);

    //    [DllImport("winmm.dll")]
    //    internal static extern int timeKillEvent(int id);

    //    [DllImport("winmm.dll")]
    //    internal static extern int timeBeginPeriod(int msec);

    //    [DllImport("winmm.dll")]
    //    internal static extern int timeEndPeriod(int msec);
    //}


    public class HighFreqTimerV2
    {
        Thread mainThread;
        Thread outputThread;
        int interval;
        //AutoResetEvent triggerTimer = new AutoResetEvent(false);
        public event EventHandler Tick;
        bool isStarted = false;
        AutoResetEvent timerTrigger = new AutoResetEvent(false);
        bool isTimerProcessing = false;

        public HighFreqTimerV2(double freqInHz, string timerName)
        {
            interval = (int)(1000 / freqInHz);
            mainThread = new Thread(mainThreadProcessing);
            mainThread.IsBackground = true;
            mainThread.Name = "TimerHFmain_" + timerName;
            mainThread.Start();
            outputThread = new Thread(outputThreadProcessing);
            outputThread.IsBackground = true;
            outputThread.Name = "TimerHFoutput_" + timerName;
            outputThread.Start();
        }

        void mainThreadProcessing()
        {
            while(true)
            {
                //triggerTimer.Set();
                if (isStarted)
                {
                    if (!isTimerProcessing)
                        timerTrigger.Set();
                    //else
                    //    Console.WriteLine(mainThread.Name+" : Task in progress");
                    //Tick?.Invoke(this, new EventArgs());
                }
                Thread.Sleep(interval);
            }
        }



        void outputThreadProcessing()
        {
            while(true)
            {
                timerTrigger.WaitOne();
                isTimerProcessing = true;
                Tick?.Invoke(this, new EventArgs());
                isTimerProcessing = false;
            }
        }

        public void Start()
        {
            isStarted = true;
        }

        public void Stop()
        {
            isStarted = false;
        }
    }
}

