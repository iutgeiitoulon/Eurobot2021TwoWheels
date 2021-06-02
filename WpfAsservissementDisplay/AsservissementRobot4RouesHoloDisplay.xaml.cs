using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Utilities;

namespace WpfAsservissementDisplay
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    /// 
    public partial class AsservissementRobot4RouesHoloDisplayControl : UserControl
    {
        int queueSize = 1;
        FixedSizedQueue<double> commandXList;
        FixedSizedQueue<double> commandYList;
        FixedSizedQueue<double> commandThetaList;
        FixedSizedQueue<double> commandM1List;
        FixedSizedQueue<double> commandM2List;
        FixedSizedQueue<double> commandM3List;
        FixedSizedQueue<double> commandM4List;

        FixedSizedQueue<double> consigneXList;
        FixedSizedQueue<double> consigneYList;
        FixedSizedQueue<double> consigneThetaList;
        FixedSizedQueue<double> consigneM1List;
        FixedSizedQueue<double> consigneM2List;
        FixedSizedQueue<double> consigneM3List;
        FixedSizedQueue<double> consigneM4List;

        FixedSizedQueue<double> measuredXList;
        FixedSizedQueue<double> measuredYList;
        FixedSizedQueue<double> measuredThetaList;
        FixedSizedQueue<double> measuredM1List;
        FixedSizedQueue<double> measuredM2List;
        FixedSizedQueue<double> measuredM3List;
        FixedSizedQueue<double> measuredM4List;

        FixedSizedQueue<double> errorXList;
        FixedSizedQueue<double> errorYList;
        FixedSizedQueue<double> errorThetaList;
        FixedSizedQueue<double> errorM4List;
        FixedSizedQueue<double> errorM3List;
        FixedSizedQueue<double> errorM2List;
        FixedSizedQueue<double> errorM1List;

        FixedSizedQueue<double> corrPXList;
        FixedSizedQueue<double> corrPYList;
        FixedSizedQueue<double> corrPThetaList;
        FixedSizedQueue<double> corrPM1List;
        FixedSizedQueue<double> corrPM2List;
        FixedSizedQueue<double> corrPM3List;
        FixedSizedQueue<double> corrPM4List;
        FixedSizedQueue<double> corrIXList;
        FixedSizedQueue<double> corrIYList;
        FixedSizedQueue<double> corrIThetaList;
        FixedSizedQueue<double> corrIM1List;
        FixedSizedQueue<double> corrIM2List;
        FixedSizedQueue<double> corrIM3List;
        FixedSizedQueue<double> corrIM4List;
        FixedSizedQueue<double> corrDXList;
        FixedSizedQueue<double> corrDYList;
        FixedSizedQueue<double> corrDThetaList;
        FixedSizedQueue<double> corrDM1List;
        FixedSizedQueue<double> corrDM2List;
        FixedSizedQueue<double> corrDM3List;
        FixedSizedQueue<double> corrDM4List;

        double corrLimitPX, corrLimitPY, corrLimitPTheta, corrLimitPM1, corrLimitPM2, corrLimitPM3, corrLimitPM4;
        double corrLimitIX, corrLimitIY, corrLimitITheta, corrLimitIM1, corrLimitIM2, corrLimitIM3, corrLimitIM4;
        double corrLimitDX, corrLimitDY, corrLimitDTheta, corrLimitDM1, corrLimitDM2, corrLimitDM3, corrLimitDM4;

        double KpX, KpY, KpTheta, KpM1, KpM2, KpM3, KpM4;
        double KiX, KiY, KiTheta, KiM1, KiM2, KiM3, KiM4;
        double KdX, KdY, KdTheta, KdM1, KdM2, KdM3, KdM4;

        System.Timers.Timer displayTimer;

        AsservissementMode asservissementMode = AsservissementMode.Off2Wheels;

        public AsservissementRobot4RouesHoloDisplayControl()
        {
            InitializeComponent();

            commandXList = new Utilities.FixedSizedQueue<double>(queueSize);
            commandYList = new Utilities.FixedSizedQueue<double>(queueSize);
            commandThetaList = new Utilities.FixedSizedQueue<double>(queueSize);
            commandM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            commandM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            commandM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            commandM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            consigneXList = new Utilities.FixedSizedQueue<double>(queueSize);
            consigneYList = new Utilities.FixedSizedQueue<double>(queueSize);
            consigneThetaList = new Utilities.FixedSizedQueue<double>(queueSize);
            consigneM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            consigneM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            consigneM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            consigneM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            measuredXList = new Utilities.FixedSizedQueue<double>(queueSize);
            measuredYList = new Utilities.FixedSizedQueue<double>(queueSize);
            measuredThetaList = new Utilities.FixedSizedQueue<double>(queueSize);
            measuredM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            measuredM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            measuredM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            measuredM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            errorXList = new Utilities.FixedSizedQueue<double>(queueSize);
            errorYList = new Utilities.FixedSizedQueue<double>(queueSize);
            errorThetaList = new Utilities.FixedSizedQueue<double>(queueSize);
            errorM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            errorM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            errorM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            errorM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            corrPXList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrPYList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrPThetaList = new Utilities.FixedSizedQueue<double>(queueSize); 
            corrPM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrPM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrPM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrPM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            corrIXList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrIYList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrIThetaList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrIM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrIM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrIM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrIM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            corrDXList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrDYList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrDThetaList = new Utilities.FixedSizedQueue<double>(queueSize);
            corrDM1List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrDM2List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrDM3List = new Utilities.FixedSizedQueue<double>(queueSize);
            corrDM4List = new Utilities.FixedSizedQueue<double>(queueSize);

            consigneXList.Enqueue(0);
            consigneYList.Enqueue(0);
            consigneThetaList.Enqueue(0);
            consigneM1List.Enqueue(0);
            consigneM2List.Enqueue(0);
            consigneM3List.Enqueue(0);
            consigneM4List.Enqueue(0);

            commandXList.Enqueue(0);
            commandYList.Enqueue(0);
            commandThetaList.Enqueue(0);
            commandM1List.Enqueue(0);
            commandM2List.Enqueue(0);
            commandM3List.Enqueue(0);
            commandM4List.Enqueue(0);

            measuredXList.Enqueue(0);
            measuredYList.Enqueue(0);
            measuredThetaList.Enqueue(0);
            measuredM1List.Enqueue(0);
            measuredM2List.Enqueue(0);
            measuredM3List.Enqueue(0);
            measuredM4List.Enqueue(0);

            errorXList.Enqueue(0);
            errorYList.Enqueue(0);
            errorThetaList.Enqueue(0);
            errorM1List.Enqueue(0);
            errorM2List.Enqueue(0);
            errorM3List.Enqueue(0);
            errorM4List.Enqueue(0);

            displayTimer = new Timer(100);
            displayTimer.Elapsed += DisplayTimer_Elapsed;
            displayTimer.Start();
        }

        public void SetAsservissementMode(AsservissementMode mode)
        {
            asservissementMode = mode;

            switch(asservissementMode)
            {
                case AsservissementMode.Off2Wheels:
                    LabelConsigneX.Visibility = Visibility.Hidden;
                    LabelConsigneY.Visibility = Visibility.Hidden;
                    LabelConsigneTheta.Visibility = Visibility.Hidden;
                    LabelErreurX.Visibility = Visibility.Hidden;
                    LabelErreurY.Visibility = Visibility.Hidden;
                    LabelErreurTheta.Visibility = Visibility.Hidden;
                    LabelCommandX.Visibility = Visibility.Hidden;
                    LabelCommandY.Visibility = Visibility.Hidden;
                    LabelCommandTheta.Visibility = Visibility.Hidden;

                    LabelConsigneM1.Visibility = Visibility.Hidden;
                    LabelConsigneM2.Visibility = Visibility.Hidden;
                    LabelConsigneM3.Visibility = Visibility.Hidden;
                    LabelConsigneM4.Visibility = Visibility.Hidden;
                    LabelErreurM1.Visibility = Visibility.Hidden;
                    LabelErreurM2.Visibility = Visibility.Hidden;
                    LabelErreurM3.Visibility = Visibility.Hidden;
                    LabelErreurM4.Visibility = Visibility.Hidden;
                    LabelCommandM1.Visibility = Visibility.Hidden;
                    LabelCommandM2.Visibility = Visibility.Hidden;
                    LabelCommandM3.Visibility = Visibility.Hidden;
                    LabelCommandM4.Visibility = Visibility.Hidden;

                    LabelCorrPX.Visibility = Visibility.Hidden;
                    LabelCorrPY.Visibility = Visibility.Hidden;
                    LabelCorrPTheta.Visibility = Visibility.Hidden;
                    LabelCorrIX.Visibility = Visibility.Hidden;
                    LabelCorrIY.Visibility = Visibility.Hidden;
                    LabelCorrITheta.Visibility = Visibility.Hidden;
                    LabelCorrDX.Visibility = Visibility.Hidden;
                    LabelCorrDY.Visibility = Visibility.Hidden;
                    LabelCorrDTheta.Visibility = Visibility.Hidden;

                    LabelCorrPM1.Visibility = Visibility.Hidden;
                    LabelCorrPM2.Visibility = Visibility.Hidden;
                    LabelCorrPM3.Visibility = Visibility.Hidden;
                    LabelCorrPM4.Visibility = Visibility.Hidden;
                    LabelCorrIM1.Visibility = Visibility.Hidden;
                    LabelCorrIM2.Visibility = Visibility.Hidden;
                    LabelCorrIM3.Visibility = Visibility.Hidden;
                    LabelCorrIM4.Visibility = Visibility.Hidden;
                    LabelCorrDM1.Visibility = Visibility.Hidden;
                    LabelCorrDM2.Visibility = Visibility.Hidden;
                    LabelCorrDM3.Visibility = Visibility.Hidden;
                    LabelCorrDM4.Visibility = Visibility.Hidden;
                    break;
                case AsservissementMode.Polar2Wheels:
                    LabelConsigneX.Visibility = Visibility.Visible;
                    LabelConsigneY.Visibility = Visibility.Visible;
                    LabelConsigneTheta.Visibility = Visibility.Visible;
                    LabelErreurX.Visibility = Visibility.Visible;
                    LabelErreurY.Visibility = Visibility.Visible;
                    LabelErreurTheta.Visibility = Visibility.Visible;
                    LabelCommandX.Visibility = Visibility.Visible;
                    LabelCommandY.Visibility = Visibility.Visible;
                    LabelCommandTheta.Visibility = Visibility.Visible;

                    LabelConsigneM1.Visibility = Visibility.Hidden;
                    LabelConsigneM2.Visibility = Visibility.Hidden;
                    LabelConsigneM3.Visibility = Visibility.Hidden;
                    LabelConsigneM4.Visibility = Visibility.Hidden;
                    LabelErreurM1.Visibility = Visibility.Hidden;
                    LabelErreurM2.Visibility = Visibility.Hidden;
                    LabelErreurM3.Visibility = Visibility.Hidden;
                    LabelErreurM4.Visibility = Visibility.Hidden;
                    LabelCommandM1.Visibility = Visibility.Hidden;
                    LabelCommandM2.Visibility = Visibility.Hidden;
                    LabelCommandM3.Visibility = Visibility.Hidden;
                    LabelCommandM4.Visibility = Visibility.Hidden;

                    LabelCorrPX.Visibility = Visibility.Visible;
                    LabelCorrPY.Visibility = Visibility.Visible;
                    LabelCorrPTheta.Visibility = Visibility.Visible;
                    LabelCorrIX.Visibility = Visibility.Visible;
                    LabelCorrIY.Visibility = Visibility.Visible;
                    LabelCorrITheta.Visibility = Visibility.Visible;
                    LabelCorrDX.Visibility = Visibility.Visible;
                    LabelCorrDY.Visibility = Visibility.Visible;
                    LabelCorrDTheta.Visibility = Visibility.Visible;

                    LabelCorrPM1.Visibility = Visibility.Hidden;
                    LabelCorrPM2.Visibility = Visibility.Hidden;
                    LabelCorrPM3.Visibility = Visibility.Hidden;
                    LabelCorrPM4.Visibility = Visibility.Hidden;
                    LabelCorrIM1.Visibility = Visibility.Hidden;
                    LabelCorrIM2.Visibility = Visibility.Hidden;
                    LabelCorrIM3.Visibility = Visibility.Hidden;
                    LabelCorrIM4.Visibility = Visibility.Hidden;
                    LabelCorrDM1.Visibility = Visibility.Hidden;
                    LabelCorrDM2.Visibility = Visibility.Hidden;
                    LabelCorrDM3.Visibility = Visibility.Hidden;
                    LabelCorrDM4.Visibility = Visibility.Hidden;
                    break;
                case AsservissementMode.Independant2Wheels:
                    LabelConsigneX.Visibility = Visibility.Hidden;
                    LabelConsigneY.Visibility = Visibility.Hidden;
                    LabelConsigneTheta.Visibility = Visibility.Hidden;
                    LabelErreurX.Visibility = Visibility.Hidden;
                    LabelErreurY.Visibility = Visibility.Hidden;
                    LabelErreurTheta.Visibility = Visibility.Hidden;
                    LabelCommandX.Visibility = Visibility.Hidden;
                    LabelCommandY.Visibility = Visibility.Hidden;
                    LabelCommandTheta.Visibility = Visibility.Hidden;

                    LabelConsigneM1.Visibility = Visibility.Visible;
                    LabelConsigneM2.Visibility = Visibility.Visible;
                    LabelConsigneM3.Visibility = Visibility.Visible;
                    LabelConsigneM4.Visibility = Visibility.Visible;
                    LabelErreurM1.Visibility = Visibility.Visible;
                    LabelErreurM2.Visibility = Visibility.Visible;
                    LabelErreurM3.Visibility = Visibility.Visible;
                    LabelErreurM4.Visibility = Visibility.Visible;
                    LabelCommandM1.Visibility = Visibility.Visible;
                    LabelCommandM2.Visibility = Visibility.Visible;
                    LabelCommandM3.Visibility = Visibility.Visible;
                    LabelCommandM4.Visibility = Visibility.Visible;

                    LabelCorrPX.Visibility = Visibility.Hidden;
                    LabelCorrPY.Visibility = Visibility.Hidden;
                    LabelCorrPTheta.Visibility = Visibility.Hidden;
                    LabelCorrIX.Visibility = Visibility.Hidden;
                    LabelCorrIY.Visibility = Visibility.Hidden;
                    LabelCorrITheta.Visibility = Visibility.Hidden;
                    LabelCorrDX.Visibility = Visibility.Hidden;
                    LabelCorrDY.Visibility = Visibility.Hidden;
                    LabelCorrDTheta.Visibility = Visibility.Hidden;

                    LabelCorrPM1.Visibility = Visibility.Visible;
                    LabelCorrPM2.Visibility = Visibility.Visible;
                    LabelCorrPM3.Visibility = Visibility.Visible;
                    LabelCorrPM4.Visibility = Visibility.Visible;
                    LabelCorrIM1.Visibility = Visibility.Visible;
                    LabelCorrIM2.Visibility = Visibility.Visible;
                    LabelCorrIM3.Visibility = Visibility.Visible;
                    LabelCorrIM4.Visibility = Visibility.Visible;
                    LabelCorrDM1.Visibility = Visibility.Visible;
                    LabelCorrDM2.Visibility = Visibility.Visible;
                    LabelCorrDM3.Visibility = Visibility.Visible;
                    LabelCorrDM4.Visibility = Visibility.Visible;
                    break;

            }
        }

        public void SetTitle(string titre)
        {
            LabelTitre.Content = titre;
        }

        private void DisplayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate ()
            {
                UpdateDisplay();
            }));
        }

        public void UpdateDisplay()
        {
            LabelConsigneX.Content = consigneXList.Average().ToString("N2");
            LabelConsigneY.Content = consigneYList.Average().ToString("N2");
            LabelConsigneTheta.Content = consigneThetaList.Average().ToString("N2");
            LabelConsigneM1.Content = consigneM1List.Average().ToString("N2");
            LabelConsigneM2.Content = consigneM2List.Average().ToString("N2");
            LabelConsigneM3.Content = consigneM3List.Average().ToString("N2");
            LabelConsigneM4.Content = consigneM4List.Average().ToString("N2");

            LabelMeasureX.Content = measuredXList.Average().ToString("N2");
            LabelMeasureY.Content = measuredYList.Average().ToString("N2");
            LabelMeasureTheta.Content = measuredThetaList.Average().ToString("N2");
            LabelMeasureM1.Content = measuredM1List.Average().ToString("N2");
            LabelMeasureM2.Content = measuredM2List.Average().ToString("N2");
            LabelMeasureM3.Content = measuredM3List.Average().ToString("N2");
            LabelMeasureM4.Content = measuredM4List.Average().ToString("N2");

            LabelErreurX.Content = errorXList.Average().ToString("N2");
            LabelErreurY.Content = errorYList.Average().ToString("N2");
            LabelErreurTheta.Content = errorThetaList.Average().ToString("N2");
            LabelErreurM1.Content = errorM1List.Average().ToString("N2");
            LabelErreurM2.Content = errorM2List.Average().ToString("N2");
            LabelErreurM3.Content = errorM3List.Average().ToString("N2");
            LabelErreurM4.Content = errorM4List.Average().ToString("N2");

            LabelCommandX.Content = commandXList.Average().ToString("N2");
            LabelCommandY.Content = commandYList.Average().ToString("N2");
            LabelCommandTheta.Content = commandThetaList.Average().ToString("N2");
            LabelCommandM1.Content = commandM1List.Average().ToString("N2");
            LabelCommandM2.Content = commandM2List.Average().ToString("N2");
            LabelCommandM3.Content = commandM3List.Average().ToString("N2");
            LabelCommandM4.Content = commandM4List.Average().ToString("N2");

            LabelKpX.Content = KpX.ToString("N2");
            LabelKpY.Content = KpY.ToString("N2");
            LabelKpTheta.Content = KpTheta.ToString("N2");
            LabelKpM1.Content = KpM1.ToString("N2");
            LabelKpM2.Content = KpM2.ToString("N2");
            LabelKpM3.Content = KpM3.ToString("N2");
            LabelKpM4.Content = KpM4.ToString("N2");

            LabelKiX.Content = KiX.ToString("N2");
            LabelKiY.Content = KiY.ToString("N2");
            LabelKiTheta.Content = KiTheta.ToString("N2");
            LabelKiM1.Content = KiM1.ToString("N2");
            LabelKiM2.Content = KiM2.ToString("N2");
            LabelKiM3.Content = KiM3.ToString("N2");
            LabelKiM4.Content = KiM4.ToString("N2");

            LabelKdX.Content = KdX.ToString("N2");
            LabelKdY.Content = KdY.ToString("N2");
            LabelKdTheta.Content = KdTheta.ToString("N2");
            LabelKdM1.Content = KdM1.ToString("N2");
            LabelKdM2.Content = KdM2.ToString("N2");
            LabelKdM3.Content = KdM3.ToString("N2");
            LabelKdM4.Content = KdM4.ToString("N2");

            LabelCorrMaxPX.Content = corrLimitPX.ToString("N2");
            LabelCorrMaxPY.Content = corrLimitPY.ToString("N2");
            LabelCorrMaxPTheta.Content = corrLimitPTheta.ToString("N2");
            LabelCorrMaxPM1.Content = corrLimitPM1.ToString("N2");
            LabelCorrMaxPM2.Content = corrLimitPM2.ToString("N2");
            LabelCorrMaxPM3.Content = corrLimitPM3.ToString("N2");
            LabelCorrMaxPM4.Content = corrLimitPM4.ToString("N2");

            LabelCorrMaxIX.Content = corrLimitIX.ToString("N2");
            LabelCorrMaxIY.Content = corrLimitIY.ToString("N2");
            LabelCorrMaxITheta.Content = corrLimitITheta.ToString("N2");
            LabelCorrMaxIM1.Content = corrLimitIM1.ToString("N2");
            LabelCorrMaxIM2.Content = corrLimitIM2.ToString("N2");
            LabelCorrMaxIM3.Content = corrLimitIM3.ToString("N2");
            LabelCorrMaxIM4.Content = corrLimitIM4.ToString("N2");

            LabelCorrMaxDX.Content = corrLimitDX.ToString("N2");
            LabelCorrMaxDY.Content = corrLimitDY.ToString("N2");
            LabelCorrMaxDTheta.Content = corrLimitDTheta.ToString("N2");
            LabelCorrMaxDM1.Content = corrLimitDM1.ToString("N2");
            LabelCorrMaxDM2.Content = corrLimitDM2.ToString("N2");
            LabelCorrMaxDM3.Content = corrLimitDM3.ToString("N2");
            LabelCorrMaxDM4.Content = corrLimitDM4.ToString("N2");


            if (corrPXList.Count > 0)
            {
                LabelCorrPX.Content = corrPXList.Average().ToString("N2");
                LabelCorrPY.Content = corrPYList.Average().ToString("N2");
                LabelCorrPTheta.Content = corrPThetaList.Average().ToString("N2");

                LabelCorrIX.Content = corrIXList.Average().ToString("N2");
                LabelCorrIY.Content = corrIYList.Average().ToString("N2");
                LabelCorrITheta.Content = corrIThetaList.Average().ToString("N2");

                LabelCorrDX.Content = corrDXList.Average().ToString("N2");
                LabelCorrDY.Content = corrDYList.Average().ToString("N2");
                LabelCorrDTheta.Content = corrDThetaList.Average().ToString("N2");
            }

            if (corrPM1List.Count > 0)
            {
                LabelCorrPM1.Content = corrPM1List.Average().ToString("N2");
                LabelCorrPM2.Content = corrPM2List.Average().ToString("N2");
                LabelCorrPM3.Content = corrPM3List.Average().ToString("N2");
                LabelCorrPM4.Content = corrPM4List.Average().ToString("N2");

                LabelCorrIM1.Content = corrIM1List.Average().ToString("N2");
                LabelCorrIM2.Content = corrIM2List.Average().ToString("N2");
                LabelCorrIM3.Content = corrIM3List.Average().ToString("N2");
                LabelCorrIM4.Content = corrIM4List.Average().ToString("N2");

                LabelCorrDM1.Content = corrDM1List.Average().ToString("N2");
                LabelCorrDM2.Content = corrDM2List.Average().ToString("N2");
                LabelCorrDM3.Content = corrDM3List.Average().ToString("N2");
                LabelCorrDM4.Content = corrDM4List.Average().ToString("N2");
            }
        }

        public void UpdatePolarSpeedConsigneValues(double consigneX, double consigneY, double consigneTheta)
        {
            consigneXList.Enqueue(consigneX);
            consigneYList.Enqueue(consigneY);
            consigneThetaList.Enqueue(consigneTheta);
        }
        public void UpdateIndependantSpeedConsigneValues(double consigneM1, double consigneM2, double consigneM3, double consigneM4)
        {
            consigneM1List.Enqueue(consigneM1);
            consigneM2List.Enqueue(consigneM2);
            consigneM3List.Enqueue(consigneM3);
            consigneM4List.Enqueue(consigneM4);
        }

        public void UpdatePolarSpeedCommandValues(double commandX, double commandY, double commandTheta)
        {
            commandXList.Enqueue(commandX);
            commandYList.Enqueue(commandY);
            commandThetaList.Enqueue(commandTheta);
        }
        public void UpdateIndependantSpeedCommandValues(double commandM1, double commandM2, double commandM3, double commandM4)
        {
            commandM1List.Enqueue(commandM1);
            commandM2List.Enqueue(commandM2);
            commandM3List.Enqueue(commandM3);
            commandM4List.Enqueue(commandM4);
        }

        public void UpdatePolarOdometrySpeed(double valueX, double valueY, double valueTheta)
        {
            measuredXList.Enqueue(valueX);
            measuredYList.Enqueue(valueY);
            measuredThetaList.Enqueue(valueTheta);
        }
        public void UpdateIndependantOdometrySpeed(double valueM1, double valueM2, double valueM3, double valueM4)
        {
            measuredM1List.Enqueue(valueM1);
            measuredM2List.Enqueue(valueM2);
            measuredM3List.Enqueue(valueM3);
            measuredM4List.Enqueue(valueM4);
        }

        public void UpdatePolarSpeedErrorValues(double errorX, double errorY, double errorTheta)
        {
            errorXList.Enqueue(errorX);
            errorYList.Enqueue(errorY);
            errorThetaList.Enqueue(errorTheta);
        }
        public void UpdateIndependantSpeedErrorValues(double errorM1, double errorM2, double errorM3, double errorM4)
        {
            errorM1List.Enqueue(errorM1);
            errorM2List.Enqueue(errorM2);
            errorM3List.Enqueue(errorM3);
            errorM4List.Enqueue(errorM4);
        }

        public void UpdatePolarSpeedCorrectionValues(double corrPX, double corrPY, double corrPTheta,
            double corrIX, double corrIY, double corrITheta,
            double corrDX, double corrDY, double corrDTheta)
        {
            corrPXList.Enqueue(corrPX);
            corrPYList.Enqueue(corrPY);
            corrPThetaList.Enqueue(corrPTheta);
            corrIXList.Enqueue(corrIX);
            corrIYList.Enqueue(corrIY);
            corrIThetaList.Enqueue(corrITheta);
            corrDXList.Enqueue(corrDX);
            corrDYList.Enqueue(corrDY);
            corrDThetaList.Enqueue(corrDTheta);
        }
        public void UpdateIndependantSpeedCorrectionValues(double corrPM1, double corrPM2, double corrPM3, double corrPM4,
            double corrIM1, double corrIM2, double corrIM3, double corrIM4,
            double corrDM1, double corrDM2, double corrDM3, double corrDM4)
        {
            corrPM1List.Enqueue(corrPM1);
            corrPM2List.Enqueue(corrPM2);
            corrPM3List.Enqueue(corrPM3);
            corrPM4List.Enqueue(corrPM4);
            corrIM1List.Enqueue(corrIM1);
            corrIM2List.Enqueue(corrIM2);
            corrIM3List.Enqueue(corrIM3);
            corrIM4List.Enqueue(corrIM4);
            corrDM1List.Enqueue(corrDM1);
            corrDM2List.Enqueue(corrDM2);
            corrDM3List.Enqueue(corrDM3);
            corrDM4List.Enqueue(corrDM4);
        }


        public void UpdatePolarSpeedCorrectionGains(double KpX, double KpY, double KpTheta,
            double KiX, double KiY, double KiTheta,
            double KdX, double KdY, double KdTheta)
        {
            this.KpX = KpX;
            this.KpY = KpY;
            this.KpTheta = KpTheta;
            this.KiX = KiX;
            this.KiY = KiY;
            this.KiTheta = KiTheta;
            this.KdX = KdX;
            this.KdY = KdY;
            this.KdTheta = KdTheta;
        }
        public void UpdateIndependantSpeedCorrectionGains(double KpM1, double KpM2, double KpM3, double KpM4,
            double KiM1, double KiM2, double KiM3, double KiM4,
            double KdM1, double KdM2, double KdM3,double KdM4)
        {
            this.KpM1 = KpM1;
            this.KpM2 = KpM2;
            this.KpM3 = KpM3;
            this.KpM4 = KpM4;
            this.KiM1 = KiM1;
            this.KiM2 = KiM2;
            this.KiM3 = KiM3;
            this.KiM4 = KiM4;
            this.KdM1 = KdM1;
            this.KdM2 = KdM2;
            this.KdM3 = KdM3;
            this.KdM4 = KdM4;
        }

        public void UpdatePolarSpeedCorrectionLimits(double corrLimitPX, double corrLimitPY, double corrLimitPTheta,
            double corrLimitIX, double corrLimitIY, double corrLimitITheta,
            double corrLimitDX, double corrLimitDY, double corrLimitDTheta)
        {
            this.corrLimitPX = corrLimitPX;
            this.corrLimitPY = corrLimitPY;
            this.corrLimitPTheta = corrLimitPTheta;
            this.corrLimitIX = corrLimitIX;
            this.corrLimitIY = corrLimitIY;
            this.corrLimitITheta = corrLimitITheta;
            this.corrLimitDX = corrLimitDX;
            this.corrLimitDY = corrLimitDY;
            this.corrLimitDTheta = corrLimitDTheta;
        }
        public void UpdateIndependantSpeedCorrectionLimits(double corrLimitPM1, double corrLimitPM2, double corrLimitPM3, double corrLimitPM4,
            double corrLimitIM1, double corrLimitIM2, double corrLimitIM3, double corrLimitIM4,
            double corrLimitDM1, double corrLimitDM2, double corrLimitDM3, double corrLimitDM4)
        {
            this.corrLimitPM1 = corrLimitPM1;
            this.corrLimitPM2 = corrLimitPM2;
            this.corrLimitPM3 = corrLimitPM3;
            this.corrLimitPM4 = corrLimitPM4;
            this.corrLimitIM1 = corrLimitIM1;
            this.corrLimitIM2 = corrLimitIM2;
            this.corrLimitIM3 = corrLimitIM3;
            this.corrLimitIM4 = corrLimitIM4;
            this.corrLimitDM1 = corrLimitDM1;
            this.corrLimitDM2 = corrLimitDM2;
            this.corrLimitDM3 = corrLimitDM3;
            this.corrLimitDM4 = corrLimitDM4;
        }
    }
}

