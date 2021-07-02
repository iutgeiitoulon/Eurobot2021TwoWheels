using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using EventArgsLibrary;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Input;
using System.Linq;
using System.IO;
using Constants;
using WpfWorldMapDisplay;
using WpfOscilloscopeControl;
using Utilities;
using WorldMap;

namespace RobotInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class WpfRobot2RouesInterface : Window
    {
        GameMode gameMode;
        System.Timers.Timer timerAffichage = new System.Timers.Timer(50);

        int nbMsgSent = 0;

        int nbMsgReceived = 0;

        double zoomFactor = 5;
        bool isZoomed = false;
        int lastZoomedRow = 0;
        int lastZoomedCol = 0;

        public WpfRobot2RouesInterface(GameMode gamemode)
        {
            gameMode = gamemode;
            InitializeComponent();

            #region Needed
            //Among other settings, this code may be used
            CultureInfo ci = CultureInfo.CurrentUICulture;

            try
            {
                //Override the default culture with something from app settings
                ci = new CultureInfo("Fr");
            }
            catch { }

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            //Here is the important part for databinding default converters
            FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(ci.IetfLanguageTag)
                    )
            );

            //Among other code
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                //Handler attach - will not be done if not needed
                PreviewKeyDown += new KeyEventHandler(MainWindow_PreviewKeyDown);
            }

            timerAffichage.Elapsed += TimerAffichage_Tick;
            timerAffichage.Start();
            #endregion

            #region Oscillo Configs
            oscilloX.SetTitle("Vx");
            oscilloX.AddOrUpdateLine(0, 100, "Vitesse X Consigne");
            oscilloX.AddOrUpdateLine(1, 100, "Vitesse X");
            oscilloX.AddOrUpdateLine(2, 100, "Accel X");
            oscilloX.ChangeLineColor("Vitesse X", Colors.Red);
            oscilloX.ChangeLineColor("Vitesse X Consigne", Colors.Blue);
            
            oscilloTheta.SetTitle("Vtheta");
            oscilloTheta.AddOrUpdateLine(0, 100, "Vitesse Theta Consigne");
            oscilloTheta.AddOrUpdateLine(1, 100, "Vitesse Theta");
            oscilloTheta.AddOrUpdateLine(2, 100, "Gyr Z");
            oscilloTheta.ChangeLineColor(1, Colors.Red);
            oscilloTheta.ChangeLineColor(0, Colors.Blue);
            
            oscilloLidar.AddOrUpdateLine(0, 20000, "RSSI", false);
            oscilloLidar.AddOrUpdateLine(1, 20000, "Dist");
            oscilloLidar.ChangeLineColor(0, Colors.SeaGreen);
            oscilloLidar.ChangeLineColor(1, Colors.Blue);
            #endregion

            #region Asserv Config
            asservPositionDisplay.SetTitle("Asservissement Position");
            asservSpeedDisplay.SetTitle("Asservissement Vitesse");
            #endregion

            #region Map Config
            worldMapDisplayStrategy.InitTeamMate((int)TeamId.Team1 + (int)RobotId.Robot1, GameMode.Eurobot, "Wally");
            worldMapDisplayWaypoint.InitTeamMate((int)TeamId.Team1 + (int)RobotId.Robot1, GameMode.Eurobot, "Wally");

            worldMapDisplayStrategy.OnCtrlClickOnHeatMapEvent += WorldMapDisplay_OnCtrlClickOnHeatMapEvent;
            worldMapDisplayWaypoint.OnCtrlClickOnHeatMapEvent += WorldMapDisplay_OnCtrlClickOnHeatMapEvent;

            var currentDir = Directory.GetCurrentDirectory();
            var racineProjets = Directory.GetParent(currentDir);
            var imagePath = racineProjets.Parent.Parent.FullName.ToString() + "\\Images\\";

            if (gameMode == GameMode.Eurobot)
            {
                worldMapDisplayStrategy.Init(gameMode, LocalWorldMapDisplayType.StrategyMap);
                worldMapDisplayStrategy.SetFieldImageBackGround(imagePath + "Eurobot_Background_Min.png");
                worldMapDisplayWaypoint.Init(gameMode, LocalWorldMapDisplayType.WayPointMap);
                worldMapDisplayWaypoint.SetFieldImageBackGround(imagePath + "Eurobot_Background_Min.png");
            }
            #endregion

            #region Log Config

            logDisplay.OnLogReplayEvent += OnEnableDisableLogReplay;
            logDisplay.OnLogRecorderSwitchEvent += OnEnableDisableLogging;
            logDisplay.OnPausePlaySwitchEvent += On_Log_Pause_Play_Switch;
            logDisplay.OnBackBtnEvent += On_Log_Back_Click;
            logDisplay.OnSkipBtnEvent += On_Log_Skip_Click;

            #endregion
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;

                if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length > 0)
                {
                    Keyboard.FocusedElement.RaiseEvent(
                        new TextCompositionEventArgs(
                            InputManager.Current.PrimaryKeyboardDevice,
                            new TextComposition(InputManager.Current,
                                Keyboard.FocusedElement,
                                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                            )
                        { RoutedEvent = TextCompositionManager.TextInputEvent });
                }
            }
        }

        

        public void ResetInterfaceState()
        {
            oscilloX.ResetGraph();
            oscilloTheta.ResetGraph();
        }

        #region Inputs Callback
        public void DisplayMessageDecoded(object sender, MessageByteArgs e)
        {
            nbMsgReceived += 1;
        }
        
        int nbMsgReceivedErrors = 0;
        public void DisplayMessageDecodedError(object sender, MessageByteArgs e)
        {
            nbMsgReceivedErrors += 1;
        }

        double currentTime = 0;
        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            worldMapDisplayStrategy.UpdateWorldMapDisplay();
            worldMapDisplayWaypoint.UpdateWorldMapDisplay();
        }

        public void OnLocalWorldMapRobotUpdate(object sender, LocationArgs location)
        {
            worldMapDisplayWaypoint.UpdateRobotLocation(location.RobotId, location.Location);
            worldMapDisplayStrategy.UpdateRobotLocation(location.RobotId, location.Location);
        }

        public void OnLocalWorldMapStrategyEvent(object sender, LocalWorldMap e)
        {
            worldMapDisplayStrategy.UpdateLocalWorldMap(e);
        }
        public void OnLocalWorldMapWayPointEvent(object sender, LocalWorldMap e)
        {
            worldMapDisplayWaypoint.UpdateLocalWorldMap(e);
        }
        

        public void OnRawLidarDataReceived(object sender, RawLidarArgs e)
        {
            List<Point> ptList = new List<Point>();
            ptList = e.PtList.Select(p => new Point(p.Angle, p.Rssi)).ToList();
            oscilloLidar.UpdatePointListOfLine(0, ptList);
            List<Point> ptList2 = new List<Point>();
            ptList2 = e.PtList.Select(p => new Point(p.Angle, p.Distance)).ToList();
            oscilloLidar.UpdatePointListOfLine(1, ptList2);
        }

        public void OnProcessLidarDataReceived(object sender, RawLidarArgs e)
        {
            //List<Point> ptListProcess = new List<Point>();
            //ptListProcess = e.PtList.Select(p => new Point(p.Angle, p.Distance)).ToList();
            //oscilloLidar.UpdatePointListOfLine(2, ptListProcess);
        }

        public void OnMessageToDisplayPolarSpeedPidSetupReceived(object sender, PolarPIDSetupArgs e)
        {
            asservSpeedDisplay.UpdatePolarSpeedCorrectionGains(e.P_x, e.P_theta, e.I_x, e.I_theta, e.D_x, e.D_theta);
            asservSpeedDisplay.UpdatePolarSpeedCorrectionLimits(e.P_x_Limit, e.P_theta_Limit, e.I_x_Limit, e.I_theta_Limit, e.D_x_Limit, e.D_theta_Limit);
        }

        public void OnMessageToDisplayIndependantSpeedPidSetupReceived(object sender, IndependantPIDSetupArgs e)
        {
            asservSpeedDisplay.UpdateIndependantSpeedCorrectionGains(e.P_M1, e.P_M2, e.I_M1, e.I_M2, e.D_M1, e.D_M2);
            asservSpeedDisplay.UpdateIndependantSpeedCorrectionLimits(e.P_M1_Limit, e.P_M2_Limit, e.I_M1_Limit, e.I_M2_Limit, e.D_M1_Limit, e.D_M2_Limit);
        }

        public void OnMessageToDisplayPositionPidSetupReceived(object sender, PolarPIDSetupArgs e)
        {
            asservPositionDisplay.UpdatePolarSpeedCorrectionGains(e.P_x, e.P_theta, e.I_x, e.I_theta, e.D_x, e.D_theta);
            asservPositionDisplay.UpdatePolarSpeedCorrectionLimits(e.P_x_Limit, e.P_theta_Limit, e.I_x_Limit, e.I_theta_Limit, e.D_x_Limit, e.D_theta_Limit);
        }

        public void OnMessageToDisplayPositionPidCorrectionReceived(object sender, PolarPidCorrectionArgs e)
        {
            asservPositionDisplay.UpdatePolarSpeedCorrectionValues(e.CorrPx, e.CorrPTheta, e.CorrIx, e.CorrITheta, e.CorrDx, e.CorrDTheta);
        }

        public void UpdateSpeedPolarOdometryOnInterface(object sender, PolarSpeedEventArgs e)
        {
            oscilloX.AddPointToLine(1, e.timeStampMs / 1000.0, e.Vx);
            oscilloTheta.AddPointToLine(1, e.timeStampMs / 1000.0, e.Vtheta);
            currentTime = e.timeStampMs / 1000.0;

            asservSpeedDisplay.UpdatePolarOdometrySpeed(e.Vx, e.Vtheta);
        }
        public void UpdateSpeedIndependantOdometryOnInterface(object sender, IndependantSpeedEventArgs e)
        {
            asservSpeedDisplay.UpdateIndependantOdometrySpeed(e.VitesseMoteur1, e.VitesseMoteur2);
        }
        public void ActualizeAccelDataOnGraph(object sender, AccelEventArgs e)
        {
            oscilloX.AddPointToLine(2, e.timeStampMS, e.accelX);
        }

        public void UpdateImuDataOnGraph(object sender, IMUDataEventArgs e)
        {
            oscilloX.AddPointToLine(2, e.EmbeddedTimeStampInMs / 1000.0, e.accelX);
            oscilloTheta.AddPointToLine(2, e.EmbeddedTimeStampInMs / 1000.0, e.gyroZ);
            currentTime = e.EmbeddedTimeStampInMs / 1000.0;
        }

        public void UpdatePolarSpeedConsigneOnGraph(object sender, PolarSpeedArgs e)
        {
            oscilloX.AddPointToLine(0, currentTime, e.Vx);
            oscilloTheta.AddPointToLine(0, currentTime, e.Vtheta);

            //asservSpeedDisplay.UpdateConsigneValues(e.Vx, e.Vy, e.Vtheta);
        }

        

        


        #region Not Coded
        public void UpdateIndependantSpeedConsigneOnGraph(object sender, IndependantSpeedEventArgs e)
        {
            //oscilloM1.AddPointToLine(4, e.timeStampMs / 1000.0, e.VitesseMoteur1);
            //oscilloM2.AddPointToLine(4, e.timeStampMs / 1000.0, e.VitesseMoteur2);
            //oscilloM3.AddPointToLine(4, e.timeStampMs / 1000.0, e.VitesseMoteur3);
            //oscilloM4.AddPointToLine(4, e.timeStampMs / 1000.0, e.VitesseMoteur4);
        }


        public void UpdateMotorsCurrentsOnGraph(object sender, MotorsCurrentsEventArgs e)
        {
            //oscilloM1.AddPointToLine(1, e.timeStampMS / 1000.0, e.motor1);
            //oscilloM2.AddPointToLine(1, e.timeStampMS / 1000.0, e.motor2);
            //oscilloM3.AddPointToLine(1, e.timeStampMS / 1000.0, e.motor3);
            //oscilloM4.AddPointToLine(1, e.timeStampMS / 1000.0, e.motor4);
        }

        public void UpdateMotorsPositionOnGraph(object sender, MotorsPositionDataEventArgs e)
        {
            //oscilloM1.AddPointToLine(2, e.timeStampMS / 1000.0, e.motor1);
            //oscilloM2.AddPointToLine(2, e.timeStampMS / 1000.0, e.motor2);
            //oscilloM3.AddPointToLine(2, e.timeStampMS / 1000.0, e.motor3);
            //oscilloM4.AddPointToLine(2, e.timeStampMS / 1000.0, e.motor4);
        }

        public void UpdateMotorsEncRawDataOnGraph(object sender, EncodersRawDataEventArgs e)
        {
            //oscilloM1.AddPointToLine(3, e.timeStampMS / 1000.0, e.motor1);
            //oscilloM2.AddPointToLine(3, e.timeStampMS / 1000.0, e.motor2);
            //oscilloM3.AddPointToLine(3, e.timeStampMS / 1000.0, e.motor3);
            //oscilloM4.AddPointToLine(3, e.timeStampMS / 1000.0, e.motor4);
        }
        #endregion

        public void UpdateSpeedPolarPidErrorCorrectionConsigneDataOnGraph(object sender, PolarPidErrorCorrectionConsigneDataArgs e)
        {
            asservSpeedDisplay.UpdatePolarSpeedErrorValues(e.xErreur, e.thetaErreur);
            asservSpeedDisplay.UpdatePolarSpeedCommandValues(e.xCorrection, e.thetaCorrection);
            asservSpeedDisplay.UpdatePolarSpeedConsigneValues(e.xConsigneFromRobot, e.thetaConsigneFromRobot);

            oscilloX.AddPointToLine(3, e.timeStampMS / 1000.0, e.xErreur);
            oscilloX.AddPointToLine(4, e.timeStampMS / 1000.0, e.xCorrection);

            oscilloTheta.AddPointToLine(3, e.timeStampMS / 1000.0, e.thetaErreur);
            oscilloTheta.AddPointToLine(4, e.timeStampMS / 1000.0, e.thetaCorrection);

            oscilloX.AddPointToLine(5, e.timeStampMS / 1000.0, e.xConsigneFromRobot);
            oscilloTheta.AddPointToLine(5, e.timeStampMS / 1000.0, e.thetaConsigneFromRobot);
        }
        public void UpdateSpeedIndependantPidErrorCorrectionConsigneDataOnGraph(object sender, IndependantPidErrorCorrectionConsigneDataArgs e)
        {
            asservSpeedDisplay.UpdateIndependantSpeedErrorValues(e.M1Erreur, e.M2Erreur);
            asservSpeedDisplay.UpdateIndependantSpeedCommandValues(e.M1Correction, e.M2Correction);
            asservSpeedDisplay.UpdateIndependantSpeedConsigneValues(e.M1ConsigneFromRobot, e.M2ConsigneFromRobot);
        }

        public void UpdateSpeedPolarPidCorrectionData(object sender, PolarPidCorrectionArgs e)
        {
            asservSpeedDisplay.UpdatePolarSpeedCorrectionValues(e.CorrPx, e.CorrPTheta,
                e.CorrIx, e.CorrITheta,
                e.CorrDx, e.CorrDTheta);
        }

        public void UpdateSpeedIndependantPidCorrectionData(object sender, IndependantPidCorrectionArgs e)
        {
            asservSpeedDisplay.UpdateIndependantSpeedCorrectionValues(e.CorrPM1, e.CorrPM2,
                e.CorrIM1, e.CorrIM2,
                e.CorrDM1, e.CorrDM2);
        }
        public void UpdatePowerMonitoringValues(object sender, PowerMonitoringValuesEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            LabelBattCommandVoltage.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                LabelBattCommandVoltage.Content = "BATT COMMAND Voltage : " + e.battCMDVoltage.ToString("F2") + "V" + "  Current : " + e.battCMDCurrent.ToString("F2") + "A" ;
            }));


            LabelBattPowerVoltage.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                LabelBattPowerVoltage.Content = "BATT POWER Voltage : " + e.battPWRVoltage.ToString("F2") + "V" + "  Current : " + e.battPWRCurrent.ToString("F2") + "A";
            }));

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Maximized)
                {
                    // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                    Properties.Settings.Default.Top = RestoreBounds.Top;
                    Properties.Settings.Default.Left = RestoreBounds.Left;
                    Properties.Settings.Default.Height = RestoreBounds.Height;
                    Properties.Settings.Default.Width = RestoreBounds.Width;
                    Properties.Settings.Default.Maximized = true;
                }
                else
                {
                    Properties.Settings.Default.Top = this.Top;
                    Properties.Settings.Default.Left = this.Left;
                    Properties.Settings.Default.Height = this.Height;
                    Properties.Settings.Default.Width = this.Width;
                    Properties.Settings.Default.Maximized = false;
                }

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            catch { }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            try
            {
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
                this.Height = Properties.Settings.Default.Height;
                this.Width = Properties.Settings.Default.Width;
                if (Properties.Settings.Default.Maximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }
            catch {; }
        }

        private void ButtonDisableMotors_Click(object sender, RoutedEventArgs e)
        {
            if (currentMotorActivation == true)
            {
                OnEnableDisableMotorsFromInterface(false);
            }
            else
            {                
                OnEnableDisableMotorsFromInterface(true);
            }
            ResetInterfaceState();
        }


        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        bool currentMotorActivation = false;
        public void ActualizeEnableDisableMotorsButton(object sender, BoolEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            //Utilisation ici d'une methode anonyme

            currentMotorActivation = e.value;
            ButtonDisableMotors.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if (currentMotorActivation)
                {
                    LabelMotorState.Content = "Motor State : Enabled";
                }
                else
                {
                    LabelMotorState.Content = "Motor State : Off4Wheels";
                }
            }));
        }                       

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void UpdateAsservissementMode(object sender, AsservissementModeEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.

            currentAsservissementMode = e.mode;
            ButtonChangeAsservissementMode.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                switch(currentAsservissementMode)
                {
                    case AsservissementMode.Off2Wheels:
                        LabelAsservMode.Content = "Asserv Mode : Off2Wheels";
                        asservSpeedDisplay.SetAsservissementMode(currentAsservissementMode);
                        break;
                    case AsservissementMode.Off4Wheels:
                        LabelAsservMode.Content = "Asserv Mode :  Off4Wheels";
                        asservSpeedDisplay.SetAsservissementMode(currentAsservissementMode);
                        break;
                    case AsservissementMode.Independant2Wheels:
                        LabelAsservMode.Content = "Asserv Mode : Independant2Wheels";
                        asservSpeedDisplay.SetAsservissementMode(currentAsservissementMode);
                        break;
                    case AsservissementMode.Independant4Wheels:
                        LabelAsservMode.Content = "Asserv Mode : Independant4Wheels";
                        asservSpeedDisplay.SetAsservissementMode(currentAsservissementMode);
                        break;
                    case AsservissementMode.Polar2Wheels:
                        LabelAsservMode.Content = "Asserv Mode : Polar2Wheels";
                        asservSpeedDisplay.SetAsservissementMode(currentAsservissementMode);
                        break;
                    case AsservissementMode.Polar4Wheels:
                        LabelAsservMode.Content = "Asserv Mode : Polar4Wheels";
                        asservSpeedDisplay.SetAsservissementMode(currentAsservissementMode);
                        break;
                    
                    


                }
            }));
        }

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeEnableMotorCurrentCheckBox(object sender, BoolEventArgs e)
        {
        }


        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeEnableAsservissementDebugDataCheckBox(object sender, BoolEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            //CheckBoxEnableAsservissementDebugData.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //{
            //}));
        }


        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizEnablePowerMonitoringCheckBox(object sender, BoolEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.

        }

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void AppendConsole(object sender, StringEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            textBoxConsole.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                textBoxConsole.Text += e.value + '\n';
                if (textBoxConsole.Text.Length >= 2000)
                {
                    textBoxConsole.Text = textBoxConsole.Text.Remove(0, 2000);
                }
                //scrollViewerTextBoxConsole.ScrollToEnd();
            }));
        }

        public void MessageCounterReceived(object sender, MsgCounterArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate ()
            {
                LabelNbSpeedOdometryDataPerSec.Content = "Nb odometry data / sec : " + e.nbMessageOdometry;
                LabelNbIMUDataPerSec.Content = "Nb IMU data / sec : " + e.nbMessageIMU;
            }));
        }
        

       
        private void ZoomOnGraph_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int row = 0, column = 0;
            if (sender.GetType() == typeof(WpfOscilloscope))
            {
                WpfOscilloscope s = (WpfOscilloscope)sender;
                if (s != null)
                {
                    row = Grid.GetRow(s);
                    column = Grid.GetColumn(s);
                }
            }
            else if(sender.GetType()== typeof(GroupBox))
            {
                GroupBox s = (GroupBox)sender;
                if (s != null)
                {
                    row = Grid.GetRow(s);
                    column = Grid.GetColumn(s);
                }
            }

            if (!isZoomed)
            {
                GridApplication.ColumnDefinitions[column].Width = new GridLength(GridApplication.ColumnDefinitions[column].Width.Value * zoomFactor, GridUnitType.Star);
                GridApplication.RowDefinitions[row].Height = new GridLength(GridApplication.RowDefinitions[row].Height.Value * zoomFactor, GridUnitType.Star);
                lastZoomedCol = column;
                lastZoomedRow = row;
                isZoomed = true;
            }
            else
            {
                GridApplication.ColumnDefinitions[lastZoomedCol].Width = new GridLength(GridApplication.ColumnDefinitions[lastZoomedCol].Width.Value / zoomFactor, GridUnitType.Star);
                GridApplication.RowDefinitions[lastZoomedRow].Height = new GridLength(GridApplication.RowDefinitions[lastZoomedRow].Height.Value / zoomFactor, GridUnitType.Star);
                isZoomed = false;
                if (lastZoomedRow != row || lastZoomedCol != column)
                {
                    GridApplication.ColumnDefinitions[column].Width = new GridLength(GridApplication.ColumnDefinitions[column].Width.Value * zoomFactor, GridUnitType.Star);
                    GridApplication.RowDefinitions[row].Height = new GridLength(GridApplication.RowDefinitions[row].Height.Value * zoomFactor, GridUnitType.Star);
                    lastZoomedCol = column;
                    lastZoomedRow = row;
                    isZoomed = true;
                }
            }
        }
        private void WorldMapDisplay_OnCtrlClickOnHeatMapEvent(object sender, PositionArgs e)
        {

        }

       

        AsservissementMode currentAsservissementMode = AsservissementMode.Off2Wheels;
        private void ButtonEnableAsservissement_Click(object sender, RoutedEventArgs e)
        {
            switch(currentAsservissementMode)
            {
                case AsservissementMode.Off2Wheels:
                    OnSetAsservissementModeFromInterface((byte)AsservissementMode.Independant2Wheels);
                    break;
                case AsservissementMode.Independant2Wheels:
                    OnSetAsservissementModeFromInterface((byte)AsservissementMode.Polar2Wheels);
                    break;
                case AsservissementMode.Polar2Wheels:
                    OnSetAsservissementModeFromInterface((byte)AsservissementMode.Off2Wheels);
                    break;
            }
        }

        private void CheckBoxEnableAsservissementDebugData_Checked(object sender, RoutedEventArgs e)
        {

        }               
        
        bool currentXBoxActivation = false;
        private void ButtonXBoxController_Click(object sender, RoutedEventArgs e)
        {

            currentXBoxActivation = !currentXBoxActivation;
            if (currentXBoxActivation)
            {
                OnEnableDisableControlManetteFromInterface(true);
                LabelXBoxControllerMode.Content = "XBox Pad : Enabled";

                
            }
            else
            {
                OnEnableDisableControlManetteFromInterface(false);
                LabelXBoxControllerMode.Content = "XBox Pad : Off4Wheels";


               
            }
        }              

        private void worldMapDisplayStrategy_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnWaypointMapDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Point point = worldMapDisplayWaypoint.GetRelativeCoords(e.GetPosition(worldMapDisplayWaypoint));
                OnWaypointLeftDoubleClick?.Invoke(this, new PointD(point.X, point.Y));
            }
        }

        private void OnStrategyMapDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Point point = worldMapDisplayStrategy.GetRelativeCoords(e.GetPosition(worldMapDisplayStrategy));
                OnStrategyLeftDoubleClick?.Invoke(this, new PointD(point.X, point.Y));
            } 
        }

        private void OnWaypointRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = worldMapDisplayWaypoint.GetRelativeCoords(e.GetPosition(worldMapDisplayWaypoint));
            OnWaypointRightClick?.Invoke(this, new PointD(point.X, point.Y));
        }

        private void OnStrategyRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = worldMapDisplayStrategy.GetRelativeCoords(e.GetPosition(worldMapDisplayStrategy));
            OnStrategyRightClick?.Invoke(this, new PointD(point.X, point.Y));
        }

        private void OnWaypointMouseWheelClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                Point point = worldMapDisplayWaypoint.GetRelativeCoords(e.GetPosition(worldMapDisplayWaypoint));
                OnWaypointWheelClick?.Invoke(this, new PointD(point.X, point.Y));
            }
        }

        private void OnStrategyMouseWheelClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                Point point = worldMapDisplayStrategy.GetRelativeCoords(e.GetPosition(worldMapDisplayStrategy));
                OnStrategyWheelClick?.Invoke(this, new PointD(point.X, point.Y));
            }
        }
        #endregion

        #region OUTPUT EVENT
        //OUTPUT EVENT
        public delegate void EnableDisableMotorsEventHandler(object sender, BoolEventArgs e);

        public event EnableDisableMotorsEventHandler OnEnableDisableMotorsFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableDisableTirFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableDisableServosFromInterfaceGeneratedEvent;
        public event EventHandler<ByteEventArgs> OnSetAsservissementModeFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableDisableControlManetteFromInterfaceGeneratedEvent;
       
        public event EventHandler<BoolEventArgs> OnEnableMotorCurrentDataFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableEncodersDataFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableEncodersRawDataFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableMotorsSpeedConsigneDataFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableSpeedPIDEnableDebugInternalFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnablePowerMonitoringDataFromInterfaceGeneratedEvent;
        public event EventHandler<BoolEventArgs> OnEnableSpeedPIDEnableDebugErrorCorrectionConsigneFromInterfaceEvent;
        public event EventHandler<PolarPIDSetupArgs> OnSetRobotPIDFromInterfaceGeneratedEvent;
        public event EventHandler<EventArgs> OnCalibrateGyroFromInterfaceGeneratedEvent;

        #region Map
        public event EventHandler<GameState> OnGameStateEditionEvent;
        public event EventHandler<PointD> OnWaypointLeftDoubleClick;
        public event EventHandler<PointD> OnStrategyLeftDoubleClick;
        public event EventHandler<PointD> OnWaypointRightClick;
        public event EventHandler<PointD> OnStrategyRightClick;
        public event EventHandler<PointD> OnWaypointWheelClick;
        public event EventHandler<PointD> OnStrategyWheelClick;
        #endregion

        #region Log
        public event EventHandler<BoolEventArgs> OnEnableDisableLoggingEvent;
        public event EventHandler<BoolEventArgs> OnEnableDisableLogReplayEvent;


        public event EventHandler<BoolEventArgs> OnPausePlaySwitchEvent;

        public event EventHandler<EventArgs> OnSkipBtnEvent;
        public event EventHandler<EventArgs> OnBackBtnEvent;
        #endregion

        public virtual void OnEnableDisableMotorsFromInterface(bool val)
        {
            OnEnableDisableMotorsFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableDisableTirFromInterface(bool val)
        {
            OnEnableDisableTirFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableDisableServosFromInterface(bool val)
        {
            OnEnableDisableServosFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnSetAsservissementModeFromInterface(byte val)
        {
            OnSetAsservissementModeFromInterfaceGeneratedEvent?.Invoke(this, new ByteEventArgs { Value = val });
        }


        public virtual void OnEnableDisableControlManetteFromInterface(bool val)
        {
            OnEnableDisableControlManetteFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }


       
        public virtual void OnEnableMotorCurrentDataFromInterface(bool val)
        {
            OnEnableMotorCurrentDataFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableEncodersDataFromInterface(bool val)
        {
            OnEnableEncodersDataFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableEncodersRawDataFromInterface(bool val)
        {
            OnEnableEncodersRawDataFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }


        public virtual void OnEnableMotorSpeedConsigneDataFromInterface(bool val)
        {
            OnEnableMotorsSpeedConsigneDataFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableSpeedPIDEnableDebugInternalFromInterface(bool val)
        {
            OnEnableSpeedPIDEnableDebugInternalFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnablePowerMonitoringDataFromInterface(bool val)
        {
            OnEnablePowerMonitoringDataFromInterfaceGeneratedEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableSpeedPIDEnableDebugErrorCorrectionConsigneFromInterface(bool val)
        {
            OnEnableSpeedPIDEnableDebugErrorCorrectionConsigneFromInterfaceEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnSetRobotPIDFromInterface(double px, double ix, double dx, double py, double iy, double dy, double ptheta, double itheta, double dtheta)
        {
            OnSetRobotPIDFromInterfaceGeneratedEvent?.Invoke(this, new PolarPIDSetupArgs { P_x = px, I_x = ix, D_x = dx, P_y = py, I_y = iy, D_y = dy, P_theta = ptheta, I_theta = itheta, D_theta = dtheta });
        }

        public virtual void OnCalibrateGyroFromInterface()
        {
            OnCalibrateGyroFromInterfaceGeneratedEvent?.Invoke(this, new EventArgs());
        }


        public virtual void OnEnableDisableLogging(object sender, bool val)
        {
            OnEnableDisableLoggingEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        public virtual void OnEnableDisableLogReplay(object sender, bool val)
        {
            OnEnableDisableLogReplayEvent?.Invoke(this, new BoolEventArgs { value = val });
        }


        private void On_Log_Pause_Play_Switch(object sender, bool e)
        {
            OnPausePlaySwitchEvent?.Invoke(this, new BoolEventArgs { value = e });
        }

        private void On_Log_Skip_Click(object sender, EventArgs e)
        {
            OnSkipBtnEvent?.Invoke(this, e);
        }

        private void On_Log_Back_Click(object sender, EventArgs e)
        {
            OnBackBtnEvent?.Invoke(this, e);
        }

        #endregion
    }
}
