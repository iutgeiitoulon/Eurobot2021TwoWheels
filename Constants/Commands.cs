using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{

    public enum Commands
    {
        #region Commandes générales
#pragma warning disable CS1591

        /// <summary>
        /// Le principe d'attribution des commandes est le suivant :
        /// On distingue :
        /// Les commandes R2PC : Robot to Computer - elles permettent d'envoyer des infos ou des ordres au PC
        /// Elles sont dans le range 0x0100 - 0x1FF
        /// Les acknowledgment ont un format particulier : on rajoute 0x1000 à la commande initiale 
        /// Les commandes PC2R : Computer to Robot - elles permettent d'envoyer des infos ou des ordres au robot
        /// Elles sont dans le range 0x0200 - 0x2FF
        /// </summary>

        R2PC_WelcomeMessage = 0x0100,                                   //Pas de payload
        R2PC_ErrorMessage = 0x0101,                                     //Payload de taille variable

        R2PC_IMUData = 0x0110,                                          //Timestamp(4L) - AccX(4F) - AccY(4F) - AccZ(4F) - GyroX(4F) - GyroY(4F) - GyroZ(4F)
        R2PC_IOMonitoring = 0x0120,                                     //Timestamp(4L) - IO1-IO8 (1)
        R2PC_PowerMonitoring = 0x0130,                                  //Timestamp(4L) - BattCmdVoltage(4F) - BattCmdCurrent(4F) - BattPwrVoltage(4F) - BattPwrCurrent(4F)
        R2PC_EncoderRawData = 0x0140,                                   //Timestamp(4L) - Enc Motor 1 Value(4L) - ... - Enc Motor 7 Value(4L))

        R2PC_SpeedPolarAndIndependantOdometry = 0x0150,                 //Timestamp(4L) - Vx(4F) - Vy(4F) - VTheta(4F) - VM1(4F) - VM2(4F) - VM3(4F) - VM4(4F)
        R2PC_SpeedAuxiliaryOdometry = 0x0151,                           //Timestamp(4L) - VM5(4F) - VM6(4F) - VM7(4F) 
        R2PC_SpeedPolarPidDebugErrorCorrectionConsigne = 0x152,         //Timestamp(4L) - ErrX(4F) - ErrY(4F) - ErrTh(4F) - CorrX(4F) - CorrY(4F) - CorrTh(4F) - ConsX(4F) - ConsY(4F) - ConsTh(4F)
        R2PC_SpeedIndependantPidDebugErrorCorrectionConsigne = 0x153,   //Timestamp(4L) - ErrM1(4F) - ErrM2(4F) - ErrM3(4F) - ErrM4(4F) - CorrM1(4F) - CorrM2(4F) - CorrM3(4F) - CorrM4(4F) - ConsM(4F) - ConsM2(4F) - ConsM3(4F) - ConsM4(4F)
        R2PC_SpeedPolarPidDebugInternal = 0x0154,                       //Timestamp(4L) - CorrPx(4F) - CorrIx(4F) - CorrDx(4F) - CorrPy(4F) - CorrIy(4F) - CorrDy(4F) - CorrPTh(4F) - CorrITh(4F) - CorrDTh(4F)  
        R2PC_SpeedIndependantPidDebugInternal = 0x0155,                 //Timestamp(4L) - CorrPM1(4F) - CorrIM1(4F) - CorrDM1(4F) - CorrPM2(4F) - CorrIM2(4F) - CorrDM2(4F) - CorrPM3(4F) - CorrIM3(4F) - CorrDM3(4F) - CorrPM4(4F) - CorrIM4(4F) - CorrDM4(4F)
        R2PC_SpeedAuxiliaryMotorsConsignes = 0x0156,                    //Timestamp(4L) - Consigne Motor 5(4F) - Consigne Motor 6(4F) - Consigne Motor 7(4F) )
        
        R2PC_MotorCurrentsMonitoring = 0x0160,                          //Timestamp(4L) - Motor Current 1 (4F) - ... - Motor Current 7 (4F)

        //Retour des commandes d'enable du PC
        R2PC_IOPollingEnableStatus = 0x0180,                               //Enable-Disable (1 Byte)
        R2PC_PowerMonitoringEnableStatus = 0x0181,                         //Enable-Disable (1 Byte)
        R2PC_EncoderRawMonitoringEnableStatus = 0x0182,                    //Enable-Disable (1 Byte)
        R2PC_AsservissementModeStatus = 0x0183,                            //Enable-Disable (1 Byte)
        R2PC_SpeedPIDEnableDebugErrorCorrectionConsigneStatus = 0x0184,    //Enable-Disable (1 Byte)
        R2PC_SpeedPIDEnableDebugInternalStatus = 0x0185,                       //Enable-Disable (1 Byte)
        R2PC_SpeedConsigneMonitoringEnableStatus = 0x0186,                 //Enable-Disable (1 Byte)
        R2PC_MotorsEnableDisableStatus = 0x0187,                           //Enable-Disable (1 Byte)
        R2PC_MotorCurrentMonitoringEnableStatus = 0x0188,                  //Enable-Disable (1 Byte)
        R2PC_TirEnableDisableStatus = 0x0189,                              //Enable-Disable (1 Byte)

        /// <summary>
        /// PC to Robot commands
        /// </summary>
        PC2R_EmergencyStop = 0x0200,

        PC2R_IOPollingEnable = 0x0220,                                  //Enable-Disable (1 Byte)
        PC2R_IOPollingSetFrequency = 0x0221,                            //Frequency (1 Byte)

        PC2R_PowerMonitoringEnable = 0x0230,                            //Enable-Disable (1 Byte)        

        PC2R_EncoderRawMonitoringEnable = 0x0240,                       //Enable-Disable (1 Byte)
        PC2R_OdometryPointToMeter = 0x0241,                             //PointToMeter (4F)
        PC2R_4WheelsAngleSet = 0x0242,                                  //AngleMotor1 (4F) - AngleMotor2 (4F) - AngleMotor3 (4F) - AngleMotor4 (4F)
        PC2R_4WheelsToPolarMatrixSet = 0x0243,                          //Mx1 (4F) - Mx2 (4F) - Mx3 (4F) - Mx4 (4F) - My1 (4F) - My2 (4F) - My3 (4F) - My4 (4F) - Mtheta1 (4F) - Mtheta2 (4F) - Mtheta3 (4F) - Mtheta4 (4F)
        PC2R_2WheelsAngleSet = 0x0244,                                  //AngleMotor1 (4F) - AngleMotor2 (4F)
        PC2R_2WheelsToPolarMatrixSet = 0x0245,                          //Mx1 (4F) - Mx2 (4F) - Mtheta1 (4F) - Mtheta2 (4F)

        PC2R_SetAsservissementMode = 0x250,                             //Mode (1 Byte : Disabled=0 - Polarie = 1 - Independant = 2)
        PC2R_SpeedPIDEnableDebugErrorCorrectionConsigne = 0x251,        //Enable-Disable (1 Byte)
        PC2R_SpeedPIDEnableDebugInternal = 0x0252,                          //Enable-Disable (1 Byte)
        PC2R_SpeedConsigneMonitoringEnable = 0x0253,                    //Enable-Disable (1 Byte)
        PC2R_SpeedPolarPIDSetGains = 0x0254,                            //KpX(4F) - KiX(4F) - KdX(4F) - idem en Y, en Theta, puis en LimitX, LimitY et Limit Theta : total 72 octets
        PC2R_SpeedIndependantPIDSetGains = 0x0255,                      //KpM1(4F) - KiM1(4F) - KdM1(4F) - idem en M2, M3 et M4, puis en LimitM1, LimitM2, LimitM3 et Limit M4 : total 96 octets
        PC2R_SpeedPolarSetConsigne = 0x0256,                            //Vx(4F) - Vy(4F) - VTh(4F)
        PC2R_SpeedIndividualMotorSetConsigne = 0x0257,                  //Numero Moteur (1 byte) - VMoteur(4F)
        PC2R_SpeedPIDReset = 0x0258,                                    //Pas de payload

        PC2R_MotorsEnableDisable = 0x0260,                              //Enable-Disable (1 Byte)
        PC2R_MotorCurrentMonitoringEnable = 0x0261,                     //Enable-Disable (1 Byte)

        PC2R_2WheelsPolarSpeedPIDSetGains = 0x0264,                     //KpX(4F) - KiX(4F) - KdX(4F) - idem en Theta, puis en LimitX et Limit Theta : total 48 octets
        PC2R_2WheelsIndependantSpeedPIDSetGains = 0x0265,               //KpM1(4F) - KiM1(4F) - KdM1(4F) - idem en M2, puis en LimitM1, LimitM2 : total 48 octets

        PC2R_TirEnableDisable = 0x0270,                                 //Enable-Disable (1 Byte)
        PC2R_TirCommand = 0x0271,                                       //Duree Pulse Coil 1 (2) - .. - Duree Pulse Coil 4 (2) - Offset Pulse Coil 2 (2) - .. - Offset Pulse Coil 4 (2) : total 14 bytes
        PC2R_TirMoveUp = 0x0272,                                        //Pas de payload
        PC2R_TirMoveDown = 0x0273,                                      //Pas de payload

        PC2R_HerkulexForward = 0x0280,                                  //Payload variable
        PC2R_PololuServoSetPosition = 0x0290,                           //A définir


#pragma warning restore CS1591
        #endregion
        #region Commandes de la RoboCup

        /// <summary>Arrêt de jeu</summary>
        STOP = 'S',
        /// <summary>Prise ou reprise de jeu</summary>
        START = 's',
        /// <summary>Envoyé pour signaler une connection établie</summary>
        WELCOME = 'W',
        /// <summary>Commande inconnue.</summary> TODO
        WORLD_STATE = 'w',
        /// <summary>Remise à zéro du match</summary>
        RESET = 'Z',
        /// <summary>Reserved for RefBox debugging</summary>
        TESTMODE_ON = 'U',
        /// <summary>Reserved for RefBox debugging</summary>
        TESTMODE_OFF = 'u',
        /// <summary>Carton jaune Magenta</summary>
        YELLOW_CARD_MAGENTA = 'y',
        /// <summary>Carton jaune Cyan</summary>
        YELLOW_CARD_CYAN = 'Y',
        /// <summary>Carton rouge Magenta</summary>
        RED_CARD_MAGENTA = 'r',
        /// <summary>Carton rouge Cyan</summary>
        RED_CARD_CYAN = 'R',
        /// <summary>Commande inconnue.</summary> TODO
        DOUBLE_YELLOW_IN_MAGENTA = 'j',
        /// <summary>Commande inconnue.</summary> TODO
        DOUBLE_YELLOW_IN_CYAN = 'J',
        /// <summary>Début de la première mi-temps</summary>
        FIRST_HALF = '1',
        /// <summary>Début de la seconde mi-temps</summary>
        SECOND_HALF = '2',
        /// <summary>Début de la première mi-temps du temps additionnel</summary>
        FIRST_HALF_OVERTIME = '3',
        /// <summary>Début de la seconde mi-temps du temps additionnel</summary>
        SECOND_HALF_OVERTIME = '4',
        /// <summary>Fin de la première mi-temps (normal ou additionnel)</summary>
        HALF_TIME = 'h',
        /// <summary>Fin de la seconde mi-temps (normal ou additionnel)</summary>
        END_GAME = 'e',
        /// <summary>Commande inconnue.</summary> TODO
        GAMEOVER = 'z',
        /// <summary>Commande inconnue.</summary> TODO
        PARKING = 'L',
        /// <summary>But+ Magenta</summary>
        GOAL_MAGENTA = 'a',
        /// <summary>But+ Cyan</summary>
        GOAL_CYAN = 'A',
        /// <summary>But- Magenta</summary>
        SUBGOAL_MAGENTA = 'd',
        /// <summary>But- Cyan</summary>
        SUBGOAL_CYAN = 'D',
        /// <summary>Coup d'envoi Magenta</summary>
        KICKOFF_MAGENTA = 'k',
        /// <summary>Coup d'envoi Cyan</summary>
        KICKOFF_CYAN = 'K',
        /// <summary>Coup franc Magenta</summary>
        FREEKICK_MAGENTA = 'f',
        /// <summary>Coup franc Cyan</summary>
        FREEKICK_CYAN = 'F',
        /// <summary>Coup franc depuis le goal Magenta</summary>
        GOALKICK_MAGENTA = 'g',
        /// <summary>Coup franc depuis le goal Cyan</summary>
        GOALKICK_CYAN = 'G',
        /// <summary>Touche Magenta</summary>
        THROWIN_MAGENTA = 't',
        /// <summary>Touche Cyan</summary>
        THROWIN_CYAN = 'T',
        /// <summary>Corner Magenta</summary>
        CORNER_MAGENTA = 'c',
        /// <summary>Corner Cyan</summary>
        CORNER_CYAN = 'C',
        /// <summary>Penalty Magenta</summary>
        PENALTY_MAGENTA = 'p',
        /// <summary>Penalty Cyan</summary>
        PENALTY_CYAN = 'P',
        /// <summary>Balle lachée</summary>
        DROPPED_BALL = 'N',
        /// <summary>Robot parti en réparation Magenta</summary>
        REPAIR_OUT_MAGENTA = 'o',
        /// <summary>Robot parti en réparation Cyan</summary>
        REPAIR_OUT_CYAN = 'O',
        /// <summary>Commande inconnue.</summary>
        REPAIR_IN_MAGENTA = 'i',
        /// <summary>Commande inconnue.</summary>
        REPAIR_IN_CYAN = 'I'

        #endregion
    }

    public enum AsservissementMode
    {
        Off4Wheels = 0,
        Off2Wheels = 1,
        Polar4Wheels = 2,
        Polar2Wheels = 3,
        Independant4Wheels = 4,
        Independant2Wheels = 5,
    }
    public enum ActiveMode
    {
        Disabled = 0,
        Enabled = 1
    }
}
