using System.Threading;
using StrategyManagerProjetEtudiantNS;

namespace StrategyManagerProjetEtudiantNS
{
    public abstract class TaskBase
    {
        Thread TaskThread;
        private SubTaskState _subState;
        public SubTaskState subState
        {
            get { return _subState; }
            private set { _subState = value; }
        }

        public void ResetSubState()
        {
            subState = SubTaskState.Entry;
        }

        public bool isRunning = false;
        int taskPeriod = 20;
        bool exitRequested = false;
        public StrategyGenerique parent;

        public TaskBase()
        {
            InitTaskThread();
        }
        public TaskBase(StrategyGenerique p)
        {
            parent = p;
            InitTaskThread();
        }

        public abstract void Init();

        public void ExitState()
        {
            exitRequested = true;
        }
        public bool isFinished = false;

        //private enum TaskTestState
        //{
        //    Init,
        //    UN,
        //    DEUX,
        //    TROIS
        //}

        private void InitTaskThread()
        {
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
        }

        public void TaskThreadProcess()
        {
            while (true)
            {
                TaskStateMachine();
                TaskSubStateManager();
                Thread.Sleep(taskPeriod);
            }
        }

        public abstract void TaskStateMachine();

        private void TaskSubStateManager()
        {
            /***************** NE PAS MODIFIER *********************/

            if (subState == SubTaskState.Entry) //Assure qu'on ne reste qu'une itération en Entry
                subState = SubTaskState.EnCours;
            else if (subState == SubTaskState.Exit) //Assure qu'on ne reste qu'une itération en Exit
                subState = SubTaskState.Entry;
            if (exitRequested) //A faire absolument après les lignes précédentes
            {
                exitRequested = false;
                subState = SubTaskState.Exit;
            }
            /***************** FIN DU NE PAS MODIFIER ***************/
        }
    }


    public enum SubTaskState
    {
        Entry,
        EnCours,
        Exit,
    }
}
