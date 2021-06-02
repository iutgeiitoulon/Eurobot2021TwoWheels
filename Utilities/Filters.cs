namespace Utilities
{

    /// Reference intéressante pour la génération de filtres : http://www-users.cs.york.ac.uk/~fisher/mkfilter/
    /// Sinon, faire les calculs soit même, ça fait pas de mal non plus...
    public enum TrigState { Waiting, TriggeredHigh, TriggeredLow, TriggeredHighAndLow };

    public class FiltreOrdre1
    {
        /// <summary>
        /// On a filtre d'ordre 1 générique :
        /// S_n = E_n * a_n + E_n_1 * a_n + S_n_1 * b_n_1
        /// </summary>
        double a_n;
        double a_n_1;
        double b_n_1;

        double E_n_1;
        double S_n_1;

        bool internalVariablesInitRequired = false;

        public void LowPassFilterInit(double freqEch, double freqCoupure)
        {
            /// On utilise les formules de la transformée bilinéaire
            /// p=2/Te*(1-z-1)/(1+z-1)
            /// Elle introduit un délai plus réduit qu'avec la transformée d'Euler
            double Te = 1 / freqEch;
            double Tau = 1 / freqCoupure;
            a_n = Te / (Te + 2 * Tau);
            a_n_1 = Te / (Te + 2 * Tau);
            b_n_1 = (2 * Tau - Te) / (2 * Tau + Te);            
            internalVariablesInitRequired = true;
        }

        public double Filter(double input)
        {
            if(internalVariablesInitRequired)
            {
                internalVariablesInitRequired = false;
                E_n_1 = input;
                S_n_1 = 0;
            }

            double output = input * a_n + E_n_1 * a_n + S_n_1 * b_n_1;
            S_n_1 = output;
            E_n_1 = input;
            return output;
        }

        public int initOutputMode = 0;
    }

    ////public class FiltreOrdre1
    ////{
    ////    public double w0;
    ////    public double ts;
    ////    public double c;
    ////    public double Q;

    ////    public double n_0;
    ////    public double n_1;
    ////    public double d_0;
    ////    public double _d_0;                      //Correspond a 1/d0
    ////    public double d_1;

    ////    public double e_1;
    ////    public double s_0;
    ////    public double s_1;

    ////    public int initOutputMode = 0;

    ////    public FiltreOrdre1()
    ////    {
    ////    }

    ////}

    //public class FiltreOrdre2
    //{
    //    public double w0;
    //    public double ts;
    //    public double c;
    //    public double Q;

    //    public double n_0;
    //    public double n_1;
    //    public double n_2;
    //    public double d_0;
    //    public double _d_0;                      //Correspond a 1/d0
    //    public double d_1;
    //    public double d_2;

    //    public double e_1;
    //    public double e_2;
    //    public double s_0;
    //    public double s_1;
    //    public double s_2;

    //    public FiltreOrdre2()
    //    {
    //    }

    //}

    //public class FiltreOrdre4
    //{
    //    public double n_0;
    //    public double n_1;
    //    public double n_2;
    //    public double n_3;
    //    public double n_4;
    //    public double d_0;
    //    public double _d_0;                      //Correspond a 1/d0
    //    public double d_1;
    //    public double d_2;

    //    public double e_1;
    //    public double e_2;
    //    public double e_3;
    //    public double e_4;
    //    public double s_0;
    //    public double s_1;
    //    public double s_2;
    //    public double s_3;
    //    public double s_4;

    //    public FiltreOrdre4()
    //    {
    //    }

    //}


    //public class FiltreOrdre8
    //{
    //    public double e_1;
    //    public double e_2;
    //    public double e_3;
    //    public double e_4;
    //    public double e_5;
    //    public double e_6;
    //    public double e_7;
    //    public double e_8;
    //    public double s_0;
    //    public double s_1;
    //    public double s_2;
    //    public double s_3;
    //    public double s_4;
    //    public double s_5;
    //    public double s_6;
    //    public double s_7;
    //    public double s_8;

    //    public FiltreOrdre8()
    //    {
    //    }

    //}

    //public class Integrateur
    //{
    //    public double s_0;
    //    public double e_1;
    //    public double _d_0;

    //    public Integrateur()
    //    {
    //    }
    //}

    //public class Derivateur
    //{
    //    public double e_1;
    //    public double fech;

    //    public Derivateur()
    //    {
    //    }
    //}

    //public class FiltreMinMaxOrdre1
    //{
    //    public FiltreOrdre1 filtreMin;
    //    public FiltreOrdre1 filtreMax;
    //    public long timeStampLastTriggerUp;
    //    public long timeStampLastTriggerDown;
    //    public double aMax;
    //    public double aMin;
    //    public bool flagCroissant;
    //    public bool flagDecroissant;
    //    public TrigState trigState;
    //    public TrigState trigStateStart;
    //    public long deadZoneDelay;
    //    public double amplitudeMin;
    //}

    //public class FiltreNbRep
    //{
    //    public FiltreMinMaxOrdre1 extremumLocal;
    //    public long timeStampMinMax;
    //    public long timeStampMinMaxT_1;
    //    public int nbDeRepCumulees;
    //    public double amplitude;
    //    public double topValueUp;
    //    public double topValueDown;
    //    public double topValueUpT_1;
    //    public double topValueDownT_1;
    //    public int periodeRep;
    //    public char flag_axe;
    //}

    //public class Threshold
    //{
    //    public double Value;
    //    public bool threshold_t_1;
    //}

    //public class IdleDetector
    //{
    //    public double idleAccelRefValue = 0;
    //    public bool flagAccelIdle = false;
    //    public bool flagAccelIdleChanged = false;
    //    public long idleStartTime;
    //    public double accelIdleSensitivity;
    //    public int accelIdleMinTime;
    //}


    //public static class Filtrage
    //{
    //    /*******************************************************************************
    //    * @fn      FiltreOrdre1TpsReel
    //    *
    //    * @brief   Fonction executant le filtrage d'un signal. (a executer a chaque
    //    *          echantillon). Filtre d'ordre 1.
    //    *
    //    * @param   filtre - struct FiltreOrdre1 - variable/objet filtre qui contiendra
    //    *          les infos/parametres du filtre.
    //    *          e_0 - _Q16 - Echantillon a filtrer
    //    *
    //    * @return  _Q16 - echantillon filtré
    //    *
    //    ******************************************************************************/
    //    static public double FiltreOrdre1TpsReel(FiltreOrdre1 filtre, double e_0)
    //    {
    //        switch (filtre.initOutputMode)
    //        {
    //            case 0: //Filtre initialisé
    //                break;
    //            case 1: //Initialisation Filtre passe-bas
    //                filtre.initOutputMode = 0;
    //                filtre.s_1 = e_0;
    //                filtre.e_1 = e_0;
    //                break;
    //            case 2: //Initialisation Filtre passe-haut
    //                filtre.initOutputMode = 0;
    //                filtre.s_1 = 0;
    //                filtre.e_1 = e_0;
    //                break;
    //            default:
    //                filtre.initOutputMode = 0;
    //                break;
    //        }

    //        //On calcule la sortie du filtre
    //        //filtre.s_0 = _Q16div(_Q16mac(e_0 ,filtre.n_0,_Q16mac(filtre.e_1 , filtre.n_1,_Q16neg(_Q16mac(filtre.s_1 , filtre.d_1,NULLACCU)))),filtre.d_0);
    //        filtre.s_0 = 1.0 / filtre.d_0 * (e_0 * filtre.n_0 + filtre.e_1 * filtre.n_1 - filtre.s_1 * filtre.d_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_1 = e_0;
    //        return filtre.s_0;
    //    }

    //    static public double FiltreOrdre1TpsReelScaled(FiltreOrdre1 filtre, double e_0, double scale)
    //    {
    //        //Dans le cas où on vient saturer les termes de la somme n0*e0 + n1*e1 + d1* s1, il faut scaler les entrées
    //        //C'est en particulier valable si n0, n1 ou d1 sont très grands !

    //        e_0 = e_0 / scale;
    //        //On calcule la sortie du filtre
    //        filtre.s_0 = 1.0 / filtre.d_0 * (e_0 * filtre.n_0 + filtre.e_1 * filtre.n_1 - filtre.s_1 * filtre.d_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0 * scale;
    //    }

    //    /*******************************************************************************
    //     * @fn      FiltreOrdre2TpsReel
    //     *
    //     * @brief   Fonction executant le filtrage d'un signal. (a executer a chaque
    //     *          echantillon). Filtre d'ordre 2.
    //     *
    //     * @param   filtre - struct FiltreOrdre2 - variable/objet filtre qui contiendra
    //     *          les infos/parametres du filtre.
    //     *          e_0 - double - Echantillon a filtrer
    //     *
    //     * @return  double - echantillon filtré
    //     *
    //     ******************************************************************************/
    //    static public double FiltreOrdre2TpsReel(FiltreOrdre2 filtre, double e_0)
    //    {

    //        //On calcule la sortie du filtre
    //        //filtre.s_0 = _Q16mac(filtre._d_0 ,_Q16mac(e_0 ,filtre.n_0,_Q16mac(filtre.e_1 ,filtre.n_1,_Q16mac(filtre.e_2 ,filtre.n_2,_Q16neg(_Q16mac(filtre.s_1 ,filtre.d_1,_Q16neg(_Q16mac(filtre.s_2 ,filtre.d_2,NULLACCU))))))),NULLACCU);
    //        filtre.s_0 = 1.0 / filtre.d_0 * (e_0 * filtre.n_0 + filtre.e_1 * filtre.n_1 + filtre.e_2 * filtre.n_2 - filtre.s_1 * filtre.d_1 - filtre.s_2 * filtre.d_2);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }


    //    /*******************************************************************************
    //     * @fn      Filtres spécifiques
    //     *
    //     * @brief   
    //     *          
    //     *
    //     * @param   
    //     *          
    //     *          
    //     *
    //     * @return  
    //     *
    //     ******************************************************************************/
    //    static public double Filtre_Specific_HP4_Butterworth_8Hz_Ech125Hz_TpsReel(FiltreOrdre4 filtre, double e_0)
    //    {

    //        //On calcule la sortie du filtre
    //        e_0 /= 1.698276129;
    //        filtre.s_0 = (filtre.e_4 + e_0) - 4 * (filtre.e_3 + filtre.e_1) + 6 * filtre.e_2
    //                 + (-0.3467246215 * filtre.s_4) + (1.7501003051 * filtre.s_3)
    //                 + (-3.3720634665 * filtre.s_2) + (2.9524299218 * filtre.s_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }

    //    static public double Filtre_Specific_LP4_Butterworth_20Hz_Ech125Hz_TpsReel(FiltreOrdre4 filtre, double e_0)
    //    {

    //        //On calcule la sortie du filtre
    //        e_0 /= 4.372500733e+01;
    //        filtre.s_0 = (filtre.e_4 + e_0) + 4 * (filtre.e_3 + filtre.e_1) + 6 * filtre.e_2
    //                 + (-0.0632116957 * filtre.s_4) + (0.4080709519 * filtre.s_3)
    //                 + (-1.1227660808 * filtre.s_2) + (1.4119835012 * filtre.s_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }

    //    static public double Filtre_Specific_BP4_Tchebytchev_8Hz_20Hz_Ech125Hz_TpsReel(FiltreOrdre4 filtre, double e_0)
    //    {
    //        //On calcule la sortie du filtre
    //        e_0 /= 8.518480096e+00;
    //        filtre.s_0 = (filtre.e_4 + e_0) + -2 * filtre.e_2
    //                 + (-0.3941329430 * filtre.s_4) + (1.3273184073 * filtre.s_3)
    //                 + (-2.3170692186 * filtre.s_2) + (2.2682838861 * filtre.s_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }
    //    static public double Filtre_Specific_BP4_Tchebytchev_8Hz_20Hz_Ech250Hz_TpsReel(FiltreOrdre4 filtre, double e_0)
    //    {
    //        //On calcule la sortie du filtre
    //        e_0 /= 3.607295967e+01;
    //        filtre.s_0 = (filtre.e_4 + e_0) + -2 * filtre.e_2
    //                 + (-0.6538538826 * filtre.s_4) + (2.7058012676 * filtre.s_3)
    //                 + (-4.4232336697 * filtre.s_2) + (3.3631390648 * filtre.s_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }

    //    static public double Filtre_Specific_BP8_Tchebytchev_8Hz_20Hz_Ech125Hz_TpsReel(FiltreOrdre8 filtre, double e_0)
    //    {

    //        //On calcule la sortie du filtre
    //        e_0 /= 4.540077773e+02;
    //        filtre.s_0 = (filtre.e_8 + e_0) - 4 * (filtre.e_6 + filtre.e_2) + 6 * filtre.e_4
    //                 + (-0.4871671369 * filtre.s_8) + (3.2323659678 * filtre.s_7)
    //                 + (-10.2846142690 * filtre.s_6) + (20.1965354020 * filtre.s_5)
    //                 + (-26.6673752670 * filtre.s_4) + (24.2061664630 * filtre.s_3)
    //                 + (-14.7766198110 * filtre.s_2) + (5.5639349329 * filtre.s_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_8 = filtre.s_7;
    //        filtre.s_7 = filtre.s_6;
    //        filtre.s_6 = filtre.s_5;
    //        filtre.s_5 = filtre.s_4;
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_8 = filtre.e_7;
    //        filtre.e_7 = filtre.e_6;
    //        filtre.e_6 = filtre.e_5;
    //        filtre.e_5 = filtre.e_4;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }

    //    static public double Filtre_Specific_BP8_Tchebytchev_8Hz_20Hz_Ech250Hz_TpsReel(FiltreOrdre8 filtre, double e_0)
    //    {
    //        //On calcule la sortie du filtre
    //        e_0 /= 6.251751635e+03;
    //        filtre.s_0 = (filtre.e_8 + e_0) - 4 * (filtre.e_6 + filtre.e_2) + 6 * filtre.e_4
    //                 + (-0.6971716137 * filtre.s_8) + (5.4703389048 * filtre.s_7)
    //                 + (-19.1406916990 * filtre.s_6) + (38.9830055390 * filtre.s_5)
    //                 + (-50.5282555430 * filtre.s_4) + (42.6749997110 * filtre.s_3)
    //                 + (-22.9355773800 * filtre.s_2) + (7.1732672200 * filtre.s_1);

    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_8 = filtre.s_7;
    //        filtre.s_7 = filtre.s_6;
    //        filtre.s_6 = filtre.s_5;
    //        filtre.s_5 = filtre.s_4;
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_8 = filtre.e_7;
    //        filtre.e_7 = filtre.e_6;
    //        filtre.e_6 = filtre.e_5;
    //        filtre.e_5 = filtre.e_4;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }

    //    static public double Filtre_Specific_BP8_Tchebytchev_13Hz_20Hz_Ech125Hz_TpsReel(FiltreOrdre8 filtre, double e_0)
    //    {

    //        //On calcule la sortie du filtre
    //        e_0 /= 3.382278893e+03;
    //        filtre.s_0 = (filtre.e_8 + e_0) - 4 * (filtre.e_6 + filtre.e_2) + 6 * filtre.e_4
    //                 + (-0.6566010570 * filtre.s_8) + (3.7324231386 * filtre.s_7)
    //                 + (-10.8198223600 * filtre.s_6) + (19.8219732230 * filtre.s_5)
    //                 + (-24.9869659190 * filtre.s_4) + (22.0239618710 * filtre.s_3)
    //                 + (-13.3581141030 * filtre.s_2) + (5.1208624974 * filtre.s_1);


    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.s_8 = filtre.s_7;
    //        filtre.s_7 = filtre.s_6;
    //        filtre.s_6 = filtre.s_5;
    //        filtre.s_5 = filtre.s_4;
    //        filtre.s_4 = filtre.s_3;
    //        filtre.s_3 = filtre.s_2;
    //        filtre.s_2 = filtre.s_1;
    //        filtre.s_1 = filtre.s_0;
    //        filtre.e_8 = filtre.e_7;
    //        filtre.e_7 = filtre.e_6;
    //        filtre.e_6 = filtre.e_5;
    //        filtre.e_5 = filtre.e_4;
    //        filtre.e_4 = filtre.e_3;
    //        filtre.e_3 = filtre.e_2;
    //        filtre.e_2 = filtre.e_1;
    //        filtre.e_1 = e_0;

    //        return filtre.s_0;
    //    }

    //    /*******************************************************************************
    //     * @fn      FiltreOrdre1SetOutput
    //     *
    //     * @brief   Fonction actualisant la sortie du filtre d'ordre 1.
    //     *
    //     * @param   filtre - struct FiltreOrdre1 - variable/objet filtre qui contiendra
    //     *          les infos/parametres du filtre.
    //     *          value - double - valeure a mettre en sortie.
    //     *
    //     * @return  None.
    //     *
    //     ******************************************************************************/
    //    static public void FiltreOrdre1SetOutput(FiltreOrdre1 filtre, double value)
    //    {
    //        //Init des sorties
    //        filtre.s_0 = value;
    //        filtre.s_1 = value;

    //        //On init aussi les entrée
    //        filtre.e_1 = value;
    //    }

    //    static public void FiltreOrdre2SetOutput(FiltreOrdre2 filtre, double value)
    //    {
    //        //Init des sorties
    //        filtre.s_0 = value;
    //        filtre.s_1 = value;
    //        filtre.s_2 = value;
    //    }

    //    static public void FiltrePasseHautButterworthOrdre1SetFrequence(FiltreOrdre1 filtre, double freqEch, double freqCoupure)
    //    {
    //        filtre.w0 = freqCoupure * 2 * Math.PI;
    //        filtre.ts = 1 / freqEch;
    //        filtre.c = 1 / Math.Tan(filtre.w0 * filtre.ts / 2);

    //        filtre.n_0 = filtre.c;
    //        filtre.n_1 = -filtre.c;

    //        filtre.d_0 = filtre.c + 1;
    //        filtre._d_0 = 1 / (filtre.c + 1);
    //        filtre.d_1 = -filtre.c + 1;
    //    }

    //    //static public void FiltrePasseBasOrdre1Init(FiltreOrdre1 filtre, double freqEch, double freqCoupure)
    //    //{
    //    //    filtre.w0 = freqCoupure * 2 * Math.PI;
    //    //    filtre.ts = 1 / freqEch;
    //    //    filtre.c = 1 / Math.Tan(filtre.w0 * filtre.ts / 2);

    //    //    filtre.n_0 = 1;
    //    //    filtre.n_1 = 1;

    //    //    filtre.d_0 = filtre.c + 1;
    //    //    filtre.d_1 = -filtre.c + 1;
    //    //}

    //    static public void FiltrePasseBasOrdre1Init(FiltreOrdre1 filtre, double freqEch, double freqCoupure)
    //    {
    //        filtre.d_0 = 1 / freqCoupure + 1 / freqEch;
    //        filtre.d_1 = 1 / freqCoupure;
    //        filtre.n_0 = 1 / (2 * freqEch);
    //        filtre.n_1 = 1 / (2 * freqEch);
    //        FiltrePasseBasOrdre1OutputInitRequest(filtre);
    //    }

    //    static public void FiltrePasseBasButterworthOrdre1SetFrequence(FiltreOrdre1 filtre, double freqEch, double freqCoupure)
    //    {
    //        filtre.w0 = freqCoupure * 2 * Math.PI;
    //        filtre.ts = 1 / freqEch;
    //        filtre.c = 1 / Math.Tan(filtre.w0 * filtre.ts / 2);

    //        filtre.n_0 = 1;
    //        filtre.n_1 = 1;

    //        filtre.d_0 = filtre.c + 1;
    //        filtre.d_1 = -filtre.c + 1;
    //    }

    //    static public void FiltrePasseHautButterworthOrdre2SetFrequence(FiltreOrdre2 filtre, double freqEch, double freqCoupure)
    //    {
    //        filtre.w0 = freqCoupure * 2 * Math.PI;
    //        filtre.ts = 1 / freqEch;                           //Sample time
    //        filtre.c = 1 / Math.Tan(filtre.w0 * filtre.ts / 2.0);     //Warping equation
    //        filtre.Q = 5.0;                                  //??

    //        filtre.n_0 = filtre.c * filtre.c;
    //        filtre.n_1 = -2.0 * filtre.c * filtre.c;
    //        filtre.n_2 = filtre.c * filtre.c;

    //        filtre.d_0 = filtre.c * filtre.c + 1.41421 * filtre.c + 1;
    //        filtre._d_0 = 1.0 / filtre.d_0;
    //        filtre.d_1 = -2.0 * (filtre.c * filtre.c - 1);
    //        filtre.d_2 = filtre.c * filtre.c - 1.41421 * filtre.c + 1;
    //    }

    //    static public void FiltrePasseBasButterworthOrdre2SetFrequence(FiltreOrdre2 filtre, double freqEch, double freqCoupure)
    //    {
    //        filtre.w0 = freqCoupure * 2 * Math.PI;
    //        filtre.ts = 1 / freqEch;                           //Sample time
    //        filtre.c = 1 / Math.Tan(filtre.w0 * filtre.ts / 2.0);     //Warping equation
    //        filtre.Q = 5.0;                                  //??

    //        filtre.n_0 = 1.0;
    //        filtre.n_1 = 2.0;
    //        filtre.n_2 = 1.0;

    //        filtre.d_0 = filtre.c * filtre.c + 1.41421 * filtre.c + 1;
    //        filtre._d_0 = 1.0 / filtre.d_0;
    //        filtre.d_1 = -2.0 * (filtre.c * filtre.c - 1);
    //        filtre.d_2 = filtre.c * filtre.c - 1.41421 * filtre.c + 1;
    //    }

    //    static public void FiltreNotchOrdre2SetFrequence(FiltreOrdre2 filtre, double freqEch, double freqNotch)
    //    {
    //        filtre.w0 = freqNotch * 2 * Math.PI;
    //        filtre.ts = 1 / freqEch;                           //Sample time
    //        filtre.c = 1 / Math.Tan(filtre.w0 * filtre.ts / 2.0);     //Warping equation
    //        filtre.Q = 1.0;                                  //??

    //        filtre.n_0 = filtre.Q * (filtre.c * filtre.c + 1);
    //        filtre.n_1 = -2 * filtre.Q * (filtre.c * filtre.c - 1);
    //        filtre.n_2 = filtre.Q * (filtre.c * filtre.c + 1);

    //        filtre.d_0 = filtre.c * filtre.c * filtre.Q + filtre.c + filtre.Q;
    //        filtre.d_1 = -2.0 * filtre.Q * (filtre.c * filtre.c - 1);
    //        filtre.d_2 = filtre.c * filtre.c * filtre.Q - filtre.c + filtre.Q;
    //    }

    //    static public void FiltrePasseHautButterworthOrdre1Init(FiltreOrdre1 filtre, double freqEch, double freqCoupure)
    //    {
    //        FiltrePasseHautButterworthOrdre1SetFrequence(filtre, freqEch, freqCoupure);
    //        FiltrePasseHautButterworthOrdre1OutputInit(filtre);
    //    }

    //    static public void FiltrePasseHautButterworthOrdre1OutputInit(FiltreOrdre1 filtre)
    //    {
    //        filtre.initOutputMode = 2; //passe-haut
    //    }

    //    static public void FiltrePasseHautButterworthOrdre2Init(FiltreOrdre2 filtre, double freqEch, double freqCoupure)
    //    {
    //        FiltrePasseHautButterworthOrdre2SetFrequence(filtre, freqEch, freqCoupure);
    //        FiltreOrdre2SetOutput(filtre, 0);
    //    }
    //    static public void FiltrePasseBasButterworthOrdre1Init(FiltreOrdre1 filtre, double freqEch, double freqCoupure)
    //    {
    //        FiltrePasseBasButterworthOrdre1SetFrequence(filtre, freqEch, freqCoupure);
    //        FiltrePasseBasOrdre1OutputInitRequest(filtre);
    //    }

    //    static public void FiltrePasseBasOrdre1OutputInitRequest(FiltreOrdre1 filtre)
    //    {
    //        filtre.initOutputMode = 1; //passe-bas
    //    }

    //    static public void FiltrePasseBasButterworthOrdre2Init(FiltreOrdre2 filtre, double freqEch, double freqCoupure)
    //    {
    //        FiltrePasseBasButterworthOrdre2SetFrequence(filtre, freqEch, freqCoupure);
    //        FiltreOrdre2SetOutput(filtre, 0);
    //    }

    //    static public void FiltreNotchOrdre2Init(FiltreOrdre2 filtre, double freqEch, double freqNotch)
    //    {
    //        FiltreNotchOrdre2SetFrequence(filtre, freqEch, freqNotch);
    //        FiltreOrdre2SetOutput(filtre, 0);
    //    }

    //    static public void IntegrateurInit(Integrateur filtre, double freqEch)
    //    {
    //        filtre._d_0 = 1 / (2.0 * freqEch);
    //        //Init des sorties
    //        filtre.s_0 = 0;
    //        //Init des entrees
    //        filtre.e_1 = 0;
    //    }

    //    static public double Integrate(Integrateur filtre, double e_0)
    //    {
    //        //On calcule la sortie du filtre
    //        //filtre.s_0 = _Q16mac(_Q16mac(filtre.e_1, _Q16_CONST_1,  e_0), filtre._d_0, filtre.s_0);
    //        filtre.s_0 = (filtre.e_1 + e_0) * filtre._d_0 + filtre.s_0;
    //        //On réactualise les variables internes pour le cycle suivant
    //        filtre.e_1 = e_0;
    //        //On renvoie la valeur
    //        return filtre.s_0;
    //    }

    //    static public void IntegrateurReset(Integrateur filtre)
    //    {
    //        //Init des sorties
    //        filtre.s_0 = 0;
    //        //Init des entrees
    //        filtre.e_1 = 0;
    //    }

    //    static public void DerivateurInit(Derivateur filtre, double freqech)
    //    {
    //        filtre.fech = freqech;
    //        //init des entrees
    //        filtre.e_1 = 0;
    //    }

    //    static public double Derivate(Derivateur filtre, double e_0)
    //    {
    //        double output = (e_0 - filtre.e_1) * filtre.fech;
    //        //on réactualise les variables internes pour le cycle suivant
    //        filtre.e_1 = e_0;
    //        //on renvoie la valeur
    //        return output;
    //    }


    //    static public void FiltreMinMaxOrdre1Init(FiltreMinMaxOrdre1 filtre, double freqEch, double freqCoupure, long deadZoneDelay, double minTriggerValue)
    //    {
    //        //FiltrePasseBasButterworthOrdre1Init(filtre.filtreMin, freqEch, freqCoupure);
    //        //FiltrePasseBasButterworthOrdre1Init(filtre.filtreMax, freqEch, freqCoupure);
    //        filtre.aMax = filtre.aMin = 0;
    //        filtre.flagCroissant = false;
    //        filtre.flagDecroissant = false;
    //        filtre.trigState = TrigState.Waiting;
    //        filtre.deadZoneDelay = deadZoneDelay;
    //        filtre.amplitudeMin = minTriggerValue;
    //    }


    //    static public bool FiltreMinMaxOrdre1CalculCroissant(FiltreMinMaxOrdre1 filtre, double input, long longTimeStamp)
    //    {
    //        bool updateOutput = false;
    //        if (longTimeStamp - filtre.timeStampLastTriggerUp > filtre.deadZoneDelay)
    //            updateOutput = true;

    //        if (updateOutput)
    //        {
    //            filtre.filtreMax.s_0 = FiltreOrdre1TpsReel(filtre.filtreMax, filtre.filtreMin.s_0);
    //        }
    //        //calcul de Vmax sur l'axe Z
    //        if (input >= filtre.filtreMax.s_0)          //ATTENTION COMPARAISON EN Q16
    //        {
    //            //VmaxMoyFiltree = Vmax = vit;
    //            FiltreOrdre1SetOutput(filtre.filtreMax, input);      //Attention, la fonction attend un double..
    //            filtre.aMax = input;
    //            filtre.flagCroissant = true;
    //        }
    //        else
    //        {
    //            if (updateOutput)
    //            {
    //                if (filtre.flagCroissant == true)
    //                {
    //                    //On vient de passer le max
    //                    filtre.flagCroissant = false;
    //                    filtre.aMax = filtre.filtreMax.s_0;
    //                    //On teste si l'amplitude minimale a été respectée
    //                    //if (_Q16mac(filtre.aMax, _Q16_CONST_1, _Q16neg(filtre.filtreMin.s_0)) > amplitudeMin)
    //                    if ((filtre.aMax - filtre.filtreMin.s_0) > filtre.amplitudeMin)
    //                    {
    //                        if (filtre.trigStateStart == TrigState.Waiting)
    //                        {
    //                            //On mémorise aussi la première transition
    //                            filtre.trigStateStart = TrigState.TriggeredHigh;
    //                            filtre.trigState = TrigState.TriggeredHigh;
    //                            filtre.timeStampLastTriggerUp = longTimeStamp;
    //                        }
    //                        else if (filtre.trigState == TrigState.TriggeredLow)
    //                        {
    //                            filtre.trigState = TrigState.TriggeredHighAndLow;
    //                            filtre.timeStampLastTriggerUp = longTimeStamp;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return updateOutput;
    //    }

    //    static public bool FiltreMinMaxOrdre1CalculDecroissant(FiltreMinMaxOrdre1 filtre, double input, long longTimeStamp)
    //    {

    //        bool updateOutput = false;
    //        if (longTimeStamp - filtre.timeStampLastTriggerDown > filtre.deadZoneDelay)
    //            updateOutput = true;

    //        //calcul de Vmin sur l'axe Z    
    //        if (updateOutput)
    //        {
    //            filtre.filtreMin.s_0 = FiltreOrdre1TpsReel(filtre.filtreMin, filtre.filtreMax.s_0);
    //        }

    //        if (input <= filtre.filtreMin.s_0)                                                  //ATTENTION COMPARAISON EN Q16
    //        {
    //            FiltreOrdre1SetOutput(filtre.filtreMin, input);                          //Attention, la fonction attend un double..
    //            filtre.aMin = input;
    //            filtre.flagDecroissant = true;
    //        }
    //        else
    //        {
    //            if (updateOutput)
    //            {
    //                if (filtre.flagDecroissant == true)
    //                {
    //                    //On vient de passer le min
    //                    filtre.flagDecroissant = false;
    //                    filtre.aMin = filtre.filtreMin.s_0;
    //                    //On teste si l'amplitude minimale a été respectée
    //                    //if(_Q16mac(filtre.filtreMax.s_0, _Q16_CONST_1, _Q16neg(filtre.aMin))>amplitudeMin)
    //                    if ((filtre.filtreMax.s_0 - filtre.aMin) > filtre.amplitudeMin)
    //                    {
    //                        if (filtre.trigStateStart == TrigState.Waiting)
    //                        {
    //                            //On mémorise aussi la première transition
    //                            filtre.trigStateStart = TrigState.TriggeredLow;
    //                            filtre.trigState = TrigState.TriggeredLow;
    //                            filtre.timeStampLastTriggerDown = longTimeStamp;
    //                        }
    //                        else if (filtre.trigState == TrigState.TriggeredHigh)
    //                        {
    //                            filtre.trigState = TrigState.TriggeredHighAndLow;
    //                            filtre.timeStampLastTriggerDown = longTimeStamp;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return updateOutput;
    //    }

    //    static public void FiltreMinMaxOrdre1ResetTrig(FiltreMinMaxOrdre1 filtre)
    //    {
    //        filtre.trigState = TrigState.Waiting;
    //        filtre.trigStateStart = TrigState.Waiting;
    //    }

    //    static public void FiltreMinMaxOrdre1Reset(FiltreMinMaxOrdre1 filtre)
    //    {
    //        filtre.trigState = TrigState.Waiting;
    //        filtre.trigStateStart = TrigState.Waiting;
    //        filtre.aMax = 0;
    //        filtre.aMin = 0;
    //        filtre.flagCroissant = false;
    //        filtre.flagDecroissant = false;
    //        FiltreOrdre1SetOutput(filtre.filtreMax, 0);
    //        FiltreOrdre1SetOutput(filtre.filtreMin, 0);
    //    }


    //    ///*******************************************************************************
    //    // * Filtre Nombre de Repetitions
    //    // ******************************************************************************/

    //    static public void FiltreNbRepInit(FiltreNbRep filtre, FiltreMinMaxOrdre1 filtreMinMax, FiltreOrdre1 filtreLP1Min, FiltreOrdre1 filtreLP1Max, double freqCoupure, double freqEch, long deadZoneDelay, double minTriggerValue)
    //    {
    //        //Filtre 1
    //        FiltrePasseBasButterworthOrdre1Init(filtreLP1Min, freqEch, freqCoupure);//filtre min
    //        FiltrePasseBasButterworthOrdre1Init(filtreLP1Max, freqEch, freqCoupure);//filtre max

    //        FiltreMinMaxOrdre1Init(filtreMinMax, freqEch, freqCoupure, deadZoneDelay, minTriggerValue);

    //        filtre.extremumLocal = filtreMinMax;
    //        filtre.extremumLocal.filtreMax = filtreLP1Max;
    //        filtre.extremumLocal.filtreMin = filtreLP1Min;
    //        filtre.timeStampMinMax = 0;
    //        filtre.timeStampMinMaxT_1 = 0;
    //    }
    //    static public void FiltreNbRepReset(FiltreNbRep filtre)
    //    {
    //        filtre.flag_axe = 'N';
    //        filtre.periodeRep = 0;
    //        filtre.timeStampMinMax = 0;
    //        filtre.timeStampMinMaxT_1 = 0;
    //        filtre.nbDeRepCumulees = 0;
    //        filtre.topValueDown = 0;
    //        filtre.topValueUp = 0;
    //        FiltreMinMaxOrdre1Reset(filtre.extremumLocal); // reset du trig de bas niveau
    //    }

    //    static public bool FiltreNbRepProcess(FiltreNbRep filtre, double value, double valueTopMeasure, long timeStamp)
    //    {
    //        bool repEventDetected = false;

    //        ////On récupère le max des vitesse en montée et descente
    //        //if (valueTopMeasure > filtre.topValueUp)
    //        //    filtre.topValueUp = valueTopMeasure;
    //        //if (valueTopMeasure < filtre.topValueDown)
    //        //    filtre.topValueDown = valueTopMeasure;

    //        // calcul des extremums min et max pour les axes X, Y et Z
    //        if (FiltreMinMaxOrdre1CalculCroissant(filtre.extremumLocal, value, timeStamp))
    //        {
    //            //On récupère le max des vitesse en montée et descente
    //            //en dehors de la dead Zone
    //            if (valueTopMeasure > filtre.topValueUp)
    //                filtre.topValueUp = valueTopMeasure;
    //        }

    //        if (FiltreMinMaxOrdre1CalculDecroissant(filtre.extremumLocal, value, timeStamp))
    //        {
    //            //On récupère le max des vitesse en montée et descente
    //            //en dehors de la dead Zone
    //            if (valueTopMeasure < filtre.topValueDown)
    //                filtre.topValueDown = valueTopMeasure;
    //        }
    //        //******************************************************************//
    //        // detecteur des rep et freq de rep
    //        //******************************************************************//
    //        if (filtre.extremumLocal.trigState == TrigState.TriggeredHighAndLow)
    //        {
    //            //On réinitialise le trigger
    //            FiltreMinMaxOrdre1ResetTrig(filtre.extremumLocal);

    //            filtre.nbDeRepCumulees++;
    //            repEventDetected = true;

    //            //On mémorise l'instant de déclenchement
    //            filtre.timeStampMinMaxT_1 = filtre.timeStampMinMax;
    //            filtre.timeStampMinMax = timeStamp;

    //            //On calcule l'amplitude du mouvement
    //            filtre.amplitude = filtre.extremumLocal.aMax - filtre.extremumLocal.aMin;

    //            // on a alors la periode entre deux repetitions
    //            if (filtre.timeStampMinMaxT_1 != 0)
    //            {
    //                filtre.periodeRep = (int)((filtre.timeStampMinMax - filtre.timeStampMinMaxT_1));
    //                if (filtre.periodeRep > 2000)
    //                    filtre.periodeRep = 0;

    //            }

    //            filtre.topValueDownT_1 = filtre.topValueDown;
    //            filtre.topValueUpT_1 = filtre.topValueUp;

    //            filtre.topValueUp = 0;
    //            filtre.topValueDown = 0;
    //        }
    //        return repEventDetected;
    //    }




    //    static public void IdleDetectorProcess(IdleDetector idleDetector, double accel, long instant)
    //    {
    //        //Si on est superieur a 0.1G ou inferieur a -0.1G et en mode Idle
    //        if (accel > (idleDetector.idleAccelRefValue + idleDetector.accelIdleSensitivity) || accel < (idleDetector.idleAccelRefValue - idleDetector.accelIdleSensitivity))
    //        {
    //            idleDetector.idleStartTime = instant;             //On reset le compteur
    //            idleDetector.idleAccelRefValue = accel;
    //            if (idleDetector.flagAccelIdle == true)
    //                idleDetector.flagAccelIdleChanged = true;
    //            else
    //                idleDetector.flagAccelIdleChanged = false;
    //            idleDetector.flagAccelIdle = false;
    //        }

    //        //Si on atteint X ms en idle
    //        else if ((instant >= idleDetector.idleStartTime + idleDetector.accelIdleMinTime) && idleDetector.flagAccelIdle == false)
    //        {
    //            idleDetector.flagAccelIdleChanged = true;
    //            idleDetector.flagAccelIdle = true;
    //            //On reset les filtres et les integrateurs
    //        }

    //        else
    //        {
    //            idleDetector.flagAccelIdleChanged = false;
    //        }
    //    }

    //    static public bool IdleDetectorIsIdle(IdleDetector idleDetector)
    //    {
    //        return idleDetector.flagAccelIdle;
    //    }

    //    static public bool IdleDetectorStateHasChanged(IdleDetector idleDetector)
    //    {
    //        return idleDetector.flagAccelIdleChanged;
    //    }

    //    static public void IdleDetectorInit(IdleDetector idleDetector, double sensitivity, int idleTime)
    //    {
    //        idleDetector.accelIdleSensitivity = sensitivity;
    //        idleDetector.accelIdleMinTime = idleTime;
    //    }

    //    static char sign;
    //    static double oldEch;

    //    static public int CrossDetector(double signal1)
    //    {
    //        if (signal1 > 0 && oldEch >= 0)
    //        {
    //            oldEch = signal1;
    //            return 1;
    //        }
    //        else if (signal1 < 0 && oldEch <= 0)
    //        {
    //            oldEch = signal1;
    //            return 1;
    //        }
    //        else
    //        {
    //            oldEch = signal1;
    //            return 0;
    //        }
    //    }

    //    static public double Threshold(double signal, Threshold threshold)
    //    {
    //        if (signal >= threshold.Value)
    //        {
    //            if (threshold.threshold_t_1 == false)
    //            {
    //                threshold.threshold_t_1 = true;
    //                return 1000000;
    //            }
    //            else
    //            {
    //                return 0;
    //            }
    //        }
    //        else
    //        {
    //            threshold.threshold_t_1 = false;
    //            return 0;
    //        }
    //    }
    //}
}

