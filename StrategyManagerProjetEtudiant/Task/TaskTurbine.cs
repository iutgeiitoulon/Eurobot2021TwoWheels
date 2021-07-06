using Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using HerkulexManagerNs;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskTurbine : TaskBase
    {

        public enum TaskState
        {
            Waiting,
            EditState
        }

        private int _CompTrb1, _CompTrb2, _CompTrb3, _CompTrb4, _CompTrb5 = new int();



        Dictionary<PololuActuators, TurbineState> Turbines = new Dictionary<PololuActuators, TurbineState>();
        TaskState taskState = TaskState.Waiting;

        public TaskTurbine(StrategyEurobot parent) : base(parent)
        {
            Init();
        }

        public override void Init()
        {
            //definition des compensations pour trim indépendemment la vitesse des turbines
            _CompTrb1 = 0;
            _CompTrb2 = 0;
            _CompTrb3 = 0;
            _CompTrb4 = 0;
            _CompTrb5 = 0;
            lock (Turbines)
            {
                //init du dico
                Turbines.Add(PololuActuators.Turbine1, TurbineState.Off);
                Turbines.Add(PololuActuators.Turbine2, TurbineState.Off);
                Turbines.Add(PololuActuators.Turbine3, TurbineState.Off);
                Turbines.Add(PololuActuators.Turbine4, TurbineState.Off);
                Turbines.Add(PololuActuators.Turbine5, TurbineState.Off);
            }

            //init des turbines
            parent.OnPololuSetUs(PololuActuators.Turbine1, (ushort)TurbineState.Off);
            parent.OnPololuSetUs(PololuActuators.Turbine2, (ushort)TurbineState.Off);
            parent.OnPololuSetUs(PololuActuators.Turbine3, (ushort)TurbineState.Off);
            parent.OnPololuSetUs(PololuActuators.Turbine4, (ushort)TurbineState.Off);
            parent.OnPololuSetUs(PololuActuators.Turbine5, (ushort)TurbineState.Off);
        }

        //TaskMethods

        public void SetTurbineState(PololuActuators id, TurbineState state)
        {
            lock(Turbines)
                Turbines.AddOrUpdate(id, state);

            ResetSubState();
            taskState = TaskState.EditState;
        }

        public void TurnAllOff()
        {
            lock (Turbines)
            {
                Turbines.AddOrUpdate(PololuActuators.Turbine1, TurbineState.Off);
                Turbines.AddOrUpdate(PololuActuators.Turbine2, TurbineState.Off);
                Turbines.AddOrUpdate(PololuActuators.Turbine3, TurbineState.Off);
                Turbines.AddOrUpdate(PololuActuators.Turbine4, TurbineState.Off);
                Turbines.AddOrUpdate(PololuActuators.Turbine5, TurbineState.Off);
            }
            ResetSubState();
            taskState = TaskState.EditState;
        }

        public void SetAllStatesTo(TurbineState state)
        {
            lock (Turbines)
            {
                Turbines.AddOrUpdate(PololuActuators.Turbine1, state);
                Turbines.AddOrUpdate(PololuActuators.Turbine2, state);
                Turbines.AddOrUpdate(PololuActuators.Turbine3, state);
                Turbines.AddOrUpdate(PololuActuators.Turbine4, state);
                Turbines.AddOrUpdate(PololuActuators.Turbine5, state);
            }
            ResetSubState();
            taskState = TaskState.EditState;
        }
        //---

        public override void TaskStateMachine()
        {
            switch (taskState)
            {
                case TaskState.Waiting:
                    
                    break;

                #region TurnOn
                case TaskState.EditState:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            lock (Turbines)
                            {
                                foreach (KeyValuePair<PololuActuators, TurbineState> kvPair in Turbines)
                                {
                                    PololuActuators actuator = kvPair.Key;
                                    int comp = 0;
                                    if (actuator == PololuActuators.Turbine1)
                                        comp = _CompTrb1;
                                    if (actuator == PololuActuators.Turbine2)
                                        comp = _CompTrb2;
                                    if (actuator == PololuActuators.Turbine3)
                                        comp = _CompTrb3;
                                    if (actuator == PololuActuators.Turbine4)
                                        comp = _CompTrb4;
                                    if (actuator == PololuActuators.Turbine5)
                                        comp = _CompTrb5;

                                    parent.OnPololuSetUs(kvPair.Key, (ushort)(kvPair.Value + comp));
                                }
                            }
                            break;

                        case SubTaskState.EnCours:
                            ExitState();
                            break;

                        case SubTaskState.Exit:
                            taskState = TaskState.Waiting;
                            isFinished = true;
                            break;
                    }
                    break;
                    #endregion
            }

        }




    }
}

