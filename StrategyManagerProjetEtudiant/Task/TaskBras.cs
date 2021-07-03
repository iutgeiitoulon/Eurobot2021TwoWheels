//using EventArgsLibrary;
//using HerkulexManagerNS;
//using StrategyManagerEurobotNS;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using static HerkulexManagerNS.HerkulexEventArgs;

//namespace StrategyManagerNS
//{
//    public class TaskBras : TaskBase
//    {
//        DateTime timestamp;
//        TaskBrasState state = TaskBrasState.Idle;
//        ServoId _servoID;
//        PilotageTurbineID _turbineID;

//        public TaskBras() : base()
//        {

//        }

//        public TaskBras(StrategyGenerique sg, ServoId servoID, PilotageTurbineID turbineID) : base(sg)
//        {
//            parent = sg;
//            _turbineID = turbineID;
//            _servoID = servoID;
//        }

//        enum TaskBrasState
//        {
//            Init,
//            Idle,
//            PrehensionGobelet,
//            PrehensionGobeletCouche,
//            PrehensionGobeletDistributeur,
//            StockageEnHauteur,
//            Depose
//        }

//        enum TaskBrasServoPositions
//        {
//            Init = 512,
//            Gobelet = 750,
//            GobeletCouche = 785,
//            GobeletDistributeur = 730
//        }



//        Dictionary<ServoId, int> servoPositionsRequested = new Dictionary<ServoId, int>();
//        public bool isSupportCentreFull { get; private set; } = false;
//        public override void Init()
//        {
//            state = TaskBrasState.Init;
//            parent.OnPilotageTurbine((byte)_turbineID, 1000);   //On eteint la turbine
//            isSupportCentreFull = false;
//            isFinished = false;
//        }

//        public void StartPrehensionGobelet()
//        {
//            state = TaskBrasState.PrehensionGobelet;
//            ResetSubState();
//            isFinished = false;
//        }

//        public void StartPrehensionGobeletCouche()
//        {
//            state = TaskBrasState.PrehensionGobeletCouche;
//            ResetSubState();
//            isFinished = false;
//        }

//        public void StartPrehensionGobeletDistributeur()
//        {
//            state = TaskBrasState.PrehensionGobeletDistributeur;
//            ResetSubState();
//            isFinished = false;
//        }

//        public void StartRemonteeBras()
//        {
//            state = TaskBrasState.StockageEnHauteur;
//            ResetSubState();
//            isFinished = false;
//        }

//        public void StartDepose()
//        {
//            state = TaskBrasState.Depose;
//            ResetSubState();
//            isFinished = false;
//        }

//        public override void TaskStateMachine()
//        {
//            switch (state)
//            {
//                case TaskBrasState.Init:
//                    switch (subState)
//                    {
//                        case SubTaskState.Entry:
//                            timestamp = DateTime.Now;
//                            servoPositionsRequested = new Dictionary<ServoId, int>();
//                            servoPositionsRequested.Add((ServoId)_servoID, (int)TaskBrasServoPositions.Init);
//                            parent.OnHerkulexSetPosition(servoPositionsRequested);
//                            break;
//                        case SubTaskState.EnCours:
//                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds > 1000.0)
//                            {
//                                ExitState();/// A appeler quand on souhaite passer à Exit       
//                            }
//                            break;
//                        case SubTaskState.Exit:                             /// L'état suivant ne doit être défini que dans le substate Exit
//                            //On a terminé l'action en cours, et le bras est vide et en position de depart
//                            isRunning = false;
//                            state = TaskBrasState.Idle;
//                            break;
//                    }
//                    break;
//                case TaskBrasState.Idle:
//                    {
//                        /// On ne sort pas de cet état sans un forçage extérieur vers un autre état
//                        /// On maintient l'état du bras
//                        isFinished = true; /// Pas d'action en cours
//                    }
//                    break;
//                case TaskBrasState.PrehensionGobelet:
//                    switch (subState)
//                    {
//                        case SubTaskState.Entry:
//                            isRunning = true;           //On a une action de task en cours
//                            timestamp = DateTime.Now;
//                            servoPositionsRequested = new Dictionary<ServoId, int>();
//                            /// On allume la turbine a 40%
//                            parent.OnPilotageTurbine((byte)_turbineID, 1400);
//                            /// On descend le servo
//                            servoPositionsRequested.Add((ServoId)_servoID, (int)TaskBrasServoPositions.Gobelet);
//                            parent.OnHerkulexSetPosition(servoPositionsRequested);
//                            break;
//                        case SubTaskState.EnCours:
//                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds > 1000.0)
//                            {
//                                ExitState();/// A appeler quand on souhaite passer à Exit       
//                            }
//                            break;
//                        case SubTaskState.Exit:                             /// L'état suivant ne doit être défini que dans le substate Exit
//                            /// On part en Idle après avoir demandé l'attrapage d'un gobelet
//                            /// La turbine est active à ce moment là à puissance de prise normale
//                            state = TaskBrasState.Idle;
//                            break;
//                    }

//                    break;
//                case TaskBrasState.PrehensionGobeletCouche:
//                    switch (subState)
//                    {
//                        case SubTaskState.Entry:
//                            isRunning = true;           //On a une action de task en cours
//                            timestamp = DateTime.Now;
//                            servoPositionsRequested = new Dictionary<ServoId, int>();
//                            /// On allume la turbine a 40%
//                            parent.OnPilotageTurbine((byte)_turbineID, 1400); 
//                            servoPositionsRequested.Add((ServoId)_servoID, (int)TaskBrasServoPositions.GobeletCouche);
//                            parent.OnHerkulexSetPosition(servoPositionsRequested);
//                            break;
//                        case SubTaskState.EnCours:
//                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds > 1000.0)
//                            {
//                                ExitState();/// A appeler quand on souhaite passer à Exit       
//                            }
//                            break;
//                        case SubTaskState.Exit:                             /// L'état suivant ne doit être défini que dans le substate Exit
//                            state = TaskBrasState.StockageEnHauteur;
//                            break;
//                    }

//                    break;
//                case TaskBrasState.PrehensionGobeletDistributeur:
//                    switch (subState)
//                    {
//                        case SubTaskState.Entry:
//                            isRunning = true;           //On a une action de task en cours
//                            timestamp = DateTime.Now;
//                            servoPositionsRequested = new Dictionary<ServoId, int>();
//                            parent.OnPilotageTurbine((byte)_turbineID, 1400); //On allume la turbine a 40%
//                            servoPositionsRequested.Add((ServoId)_servoID, (int)TaskBrasServoPositions.GobeletDistributeur);
//                            parent.OnHerkulexSetPosition(servoPositionsRequested);
//                            break;
//                        case SubTaskState.EnCours:
//                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds > 1000.0)
//                            {
//                                ExitState();/// A appeler quand on souhaite passer à Exit       
//                            }
//                            break;
//                        case SubTaskState.Exit:                             /// L'état suivant ne doit être défini que dans le substate Exit
//                            state = TaskBrasState.StockageEnHauteur;
//                            break;
//                    }
//                    break;
//                case TaskBrasState.StockageEnHauteur:
//                    switch (subState)
//                    {
//                        case SubTaskState.Entry:
//                            timestamp = DateTime.Now;
//                            servoPositionsRequested = new Dictionary<ServoId, int>();
//                            servoPositionsRequested.Add((ServoId)_servoID, (int)TaskBrasServoPositions.Init);
//                            parent.OnHerkulexSetPosition(servoPositionsRequested);
//                            break;
//                        case SubTaskState.EnCours:
//                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds > 1000.0)
//                            {
//                                ExitState();                                /// A appeler quand on souhaite passer à Exit         
//                            }
//                            break;
//                        case SubTaskState.Exit:                             /// L'état suivant ne doit être défini que dans le substate Exit
//                            parent.OnPilotageTurbine((byte)_turbineID, 1350); //On baisse la turbine a 35%
//                            state = TaskBrasState.Idle;
//                            //On a terminé l'action en cours, mais la task est toujours running tant que l'on a pas deposé le gobelet
//                            break;
//                    }
//                    break;

//                case TaskBrasState.Depose:
//                    switch (subState)
//                    {
//                        case SubTaskState.Entry:
//                            timestamp = DateTime.Now;
//                            servoPositionsRequested = new Dictionary<ServoId, int>();
//                            servoPositionsRequested.Add((ServoId)_servoID, (int)TaskBrasServoPositions.Gobelet);
//                            parent.OnHerkulexSetPosition(servoPositionsRequested);
//                            break;
//                        case SubTaskState.EnCours:
//                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds > 500.0)
//                            {
//                                parent.OnPilotageTurbine((byte)_turbineID, 1000);   //On eteint la turbine
//                                ExitState();                                /// A appeler quand on souhaite passer à Exit         
//                            }
//                            break;
//                        case SubTaskState.Exit:                             /// L'état suivant ne doit être défini que dans le substate Exit
//                            isFinished = true;//On a terminé l'action en cours,
//                                              //On vient d'effectuer une depose, on repasse donc a l'init afin de remettre le bras en position initiale
//                            state = TaskBrasState.Idle;
//                            break;
//                    }
//                    break;
//                default:
//                    break;
//            }

//        }

//    }
//}
