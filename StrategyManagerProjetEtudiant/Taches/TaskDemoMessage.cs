using Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace StrategyManagerProjetEtudiantNS
{
    public enum TaskDemoMessageState
    {
        Etat1,
        Etat1EnCours,
        Etat2,
        Etat2EnCours,
        Etat3,
        Etat3EnCours,
    }

    public class TaskDemoMessage
    {
        /// <summary>
        /// Une tache est un processus permettant de gérer une partie de code de manière autonome et asynchrone.
        /// On la balaie périodiquement (mais souvent) à une vitesse définie par le Thread.Sleep(ms) en sortie du switch case
        /// 
        /// Afin d'assurer qu'une tache ne sera jamais bloquée, il est FORMELLEMENT INTERDIT d'y mettre des while bloquants
        /// 
        /// Pour éviter de flooder à chaque appel (par exemple si on veut lancer un ordre sur une liaison série),
        ///     on passe l'ordre dans une étape ETAPE_NAME et on passe juste après dans une étape ETAPE_NAME_ATTENTE
        ///     
        /// Pour implanter une temporisation, on a donc recours à une stopwatch initialisée (restart) dans ETAPE_NAME
        ///     et lue dans ETAPE_NAME_ENCOURS
        /// 
        /// La tâche a une référence vers le StrategyXXXX parent qui est passée au constructeur de manière à simplifier
        ///     les accès aux propriétés de ce StrategyXXXX parent sans passer par des events 
        ///     C'est une dérogation à la règle en vigueur dans l'ensemble du projet mais on l'accepte ici vu 
        ///     le nombre d'appel potentiels
        /// 
        /// TaskThread.IsBackground = true; signifie que le Thread est détruit quand on ferme l'application de quelque manière
        ///     que ce soit. Il faut le laisser impérativement.
        ///     
        /// </summary>

        StrategyEurobot parent;
        Thread TaskThread;
        public TaskDemoMessageState state = TaskDemoMessageState.Etat1;

        Stopwatch sw = new Stopwatch();
        
        public TaskDemoMessage(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
            sw.Stop();
            sw.Reset();
        }

        public void SetTaskState(TaskDemoMessageState state)
        {
            this.state = state;
        }
             

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskDemoMessageState.Etat1:
                        sw.Restart();
                        parent.OnTextMessage("TaskDemoMessage : Etat 1");
                        state = TaskDemoMessageState.Etat1EnCours;
                        break;
                    case TaskDemoMessageState.Etat1EnCours:
                        if (sw.ElapsedMilliseconds > 3000)
                        {
                            state = TaskDemoMessageState.Etat2;
                        }
                        break;
                    case TaskDemoMessageState.Etat2:
                        sw.Restart();
                        state = TaskDemoMessageState.Etat2EnCours;
                        parent.OnTextMessage("TaskDemoMessage : Etat 2");
                        break;
                    case TaskDemoMessageState.Etat2EnCours:
                        if (sw.ElapsedMilliseconds>2000)
                        {
                            state = TaskDemoMessageState.Etat3;
                        }                            
                        break;
                    case TaskDemoMessageState.Etat3:
                        sw.Restart();
                        state = TaskDemoMessageState.Etat3EnCours;
                        parent.OnTextMessage("TaskDemoMessage : Etat 3");
                        break;
                    case TaskDemoMessageState.Etat3EnCours:
                        if (sw.ElapsedMilliseconds > 500)
                            state = TaskDemoMessageState.Etat1;
                        break;
                    default:
                        break;
                }
                Thread.Sleep(20);
            }
        }
    }
}
