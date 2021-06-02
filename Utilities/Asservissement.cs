namespace Utilities
{
    public class AsservissementPID
    {
        public double Kp { get; private set; }
        public double Ki { get; private set; }
        public double Kd { get; private set; }

        double error, errorT_1;
        double IntegraleErreur = 0;
        public double ProportionalLimit { get; private set; }
        public double IntegralLimit { get; private set; }
        public double DerivationLimit { get; private set; }

        public double correctionP { get; private set; }
        public double correctionI { get; private set; }
        public double correctionD { get; private set; }

        double SampleFreq;

        public AsservissementPID(/*double fEch, */double kp, double ki, double kd, double proportionalLimit, double integralLimit, double derivationLimit)
        {
            Init(kp, ki, kd, proportionalLimit, integralLimit, derivationLimit);

            //IntegraleErreur = 0;
            //SampleFreq = fEch;
        }

        public void Init(double kp, double ki, double kd, double proportionalLimit, double integralLimit, double derivationLimit)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;

            ProportionalLimit = proportionalLimit;
            IntegralLimit = integralLimit;
            DerivationLimit = derivationLimit;
        }

        public void ResetPID(double error)
        {
            IntegraleErreur = 0;

            errorT_1 = error;
        }

        public double CalculatePIDoutput(double error, double ElapsedTimeBetweenCalculation)
        {
            if (ElapsedTimeBetweenCalculation > 0)
            {
                //Le principe de calcul est le suivant :
                //On veut borner les corrections sur chaque terme à une valeur donnée, par exemple ProportionalLimit pour la contribution de P à la correction
                //Sachant que correctionP = Kp*erreur, il faut donc borner au préalable erreur à ProportionalLimit / Kp

                double erreurBornee = Toolbox.LimitToInterval(error, -ProportionalLimit / Kp, ProportionalLimit / Kp);
                correctionP = Kp * erreurBornee;


                IntegraleErreur += error * ElapsedTimeBetweenCalculation; // / SampleFreq;
                IntegraleErreur = Toolbox.LimitToInterval(IntegraleErreur, -IntegralLimit / Ki, IntegralLimit / Ki); //On touche à Integrale directement car on ne veut pas laisser l'intégrale grandir à l'infini
                correctionI = Ki * IntegraleErreur;

                double derivee = (error - errorT_1) / ElapsedTimeBetweenCalculation; // * SampleFreq;
                double deriveeBornee = Toolbox.LimitToInterval(derivee, -DerivationLimit / Kd, DerivationLimit / Kd);
                errorT_1 = error;
                correctionD = deriveeBornee * Kd;

                return correctionP + correctionI + correctionD;
            }
            else return 0;
        }
    }
}
