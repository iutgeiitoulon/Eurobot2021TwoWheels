using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using System.Threading;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionCoupeOff
    {
        StrategyEurobot parent;

        Thread TaskThread;


        public bool isFinished = false;

        public MissionCoupeOff(StrategyEurobot eurobot)
        {
            parent = eurobot;
                
        }

        public void Init()
        {
            
        }

        public void Start()
        {
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
        }

        private void play1(Note note, int duration = 500, int silence_duration = 500)
        {
            parent.taskTurbine.SetTurbineState(PololuActuators.Turbine2, (TurbineState)note);
            Thread.Sleep(duration);
            if (silence_duration != 0)
                silence(silence_duration);
        }

        private void play2(Note note, int duration = 500, int silence_duration = 500)
        {
            parent.taskTurbine.SetTurbineState(PololuActuators.Turbine1, (TurbineState)note);
            Thread.Sleep(duration);
            if (silence_duration != 0)
                silence(silence_duration);
        }

        private void silence(int duration = 250)
        {
            parent.taskTurbine.TurnAllOff();//(PololuActuators.Turbine2, TurbineState.Off);
            Thread.Sleep(duration);
        }



        public void TaskThreadProcess()
        {
            for (int i = 0; i < 10; i++)
                play1(Note.Note1, 250, 250);
        }
    }

}
