using EventArgsLibrary;
using SharpDX.XInput;
using System;
using System.Timers;

namespace XBoxControllerNS
{
    public class XBoxController
    {
        int robotId = 0;
        Controller controller;
        Gamepad gamepad;
        public bool connected = false;
        public int deadband = 7000;
        public float leftTrigger, rightTrigger;
        double Vtheta;
        double VxRampe = 0;
        double VyRampe = 0;
        double VthetaRampe = 0;
        bool stopped = false;

        Timer timerGamepad = new Timer(100);

        public XBoxController(int id)
        {
            robotId = id;
            controller = new Controller(UserIndex.One);
            connected = controller.IsConnected;

            timerGamepad.Elapsed += TimerGamepad_Elapsed;
            timerGamepad.Start();
        }

        bool useRampe = false;
        private void TimerGamepad_Elapsed(object sender, ElapsedEventArgs e)
        {
            double VLinMax = 3;   //1.2 ~= 0.3m/s
            double VThetaMax = 1.5* Math.PI;
            double valeurRampe = 0.6;
            double Vx;
            double Vy;

            double vitessePriseBalle;
            if (controller.IsConnected)
            {
                gamepad = controller.GetState().Gamepad;

                if (gamepad.LeftThumbY > deadband)
                    Vx = gamepad.LeftThumbY - deadband;
                else if (gamepad.LeftThumbY < -deadband)
                    Vx = gamepad.LeftThumbY + deadband;
                else
                    Vx = 0;
                Vx = Vx / short.MaxValue * VLinMax;

                //Inversion sur Vy pour avoir Vy positif quand on va vers la gauche.
                double gamePadVy = -gamepad.LeftThumbX;
                if (gamePadVy > deadband)
                    Vy = gamePadVy - deadband;
                else if (gamePadVy < -deadband)
                    Vy = gamePadVy + deadband;
                else
                    Vy = 0;
                Vy = Vy / short.MaxValue * VLinMax;


                //Inversion sur VTHeta pour avoir VTheta positif quand on va vers la gauche.
                double gamePadVTheta = -gamepad.RightThumbX;
                if (gamePadVTheta > deadband)
                    Vtheta = gamePadVTheta - deadband;
                else if (gamePadVTheta < -deadband)
                    Vtheta = gamePadVTheta + deadband;
                else
                    Vtheta = 0;
                Vtheta = Vtheta / short.MaxValue * VThetaMax;

                //Console.WriteLine("Gamepad Vx : " + Vx + " Vy : "+Vy +" VTheta : "+Vtheta);
                vitessePriseBalle = (float)(gamepad.RightTrigger) / 2.55;
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.X))
                {
                    OnTirToRobot(robotId, 50);
                }

                if(gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
                {
                    OnMoveTirUpToRobot();
                }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
                {
                    OnMoveTirDownToRobot();
                }
                if(gamepad.Buttons.HasFlag(GamepadButtonFlags.Start))
                {
                    if(stopped)
                    {
                        OnStopToRobot(false);
                        stopped = false;
                    }
                    else
                    {
                        OnStopToRobot(true);
                        stopped = true;
                    }
                    
                }


                if (useRampe)
                {
                    if (Vx >= VxRampe)
                    {
                        VxRampe += valeurRampe;
                        VxRampe = Math.Min(VxRampe, Vx);
                    }
                    else
                    {
                        VxRampe -= valeurRampe;
                        VxRampe = Math.Max(VxRampe, Vx);
                    }

                    if (Vy >= VyRampe)
                    {
                        VyRampe += valeurRampe;
                        VyRampe = Math.Min(VyRampe, Vy);
                    }
                    else
                    {
                        VyRampe -= valeurRampe;
                        VyRampe = Math.Max(VyRampe, Vy);
                    }

                    if (Vtheta >= VthetaRampe)
                    {
                        VthetaRampe += valeurRampe;
                        VthetaRampe = Math.Min(VthetaRampe, Vtheta);
                    }
                    else
                    {
                        VthetaRampe -= valeurRampe;
                        VthetaRampe = Math.Max(VthetaRampe, Vtheta);
                    }
                }
                else
                {
                    VxRampe = Vx;
                    VyRampe = Vy;
                    VthetaRampe = Vtheta;
                }

                OnSpeedConsigneToRobot(robotId, VxRampe, VyRampe, VthetaRampe);
                //OnPriseBalleToRobot(2, (float)(Vx*33.3));
                OnPriseBalleToRobot(5, vitessePriseBalle);
                OnPriseBalleToRobot(6, -vitessePriseBalle);
            }
        }

        //Events générés en sortie
        public delegate void SpeedConsigneEventHandler(object sender, PolarSpeedArgs e);
        public event EventHandler<PolarSpeedArgs> OnSpeedConsigneEvent;
        public virtual void OnSpeedConsigneToRobot(int id, double vx, double vy, double vtheta)
        {
            OnSpeedConsigneEvent?.Invoke(this, new PolarSpeedArgs { RobotId = id, Vx = vx, Vy = vy, Vtheta = vtheta });
        }

        public delegate void OnTirEventHandler(object sender, TirEventArgs e);
        public event EventHandler<TirEventArgs> OnTirEvent;
        public virtual void OnTirToRobot(int id, float puissance)
        {
            var handler = OnTirEvent;
            if (handler != null)
            {
                handler(this, new TirEventArgs { RobotId = id, Puissance = puissance });
            }
        }
        public delegate void OnStopEventHandler(object sender, BoolEventArgs e);
        public event EventHandler<BoolEventArgs> OnStopEvent;
        public virtual void OnStopToRobot(bool stop)
        {
            var handler = OnStopEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs());
            }
        }

        public delegate void OnMoveTirUpEventHandler(object sender, EventArgs e);
        public event EventHandler<EventArgs> OnMoveTirUpEvent;
        public virtual void OnMoveTirUpToRobot()
        {
            var handler = OnMoveTirUpEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public delegate void OnMoveTirDownEventHandler(object sender, EventArgs e);
        public event EventHandler<EventArgs> OnMoveTirDownEvent;
        public virtual void OnMoveTirDownToRobot()
        {
            var handler = OnMoveTirDownEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public event EventHandler<SpeedConsigneToMotorArgs> OnPriseBalleEvent;
        public virtual void OnPriseBalleToRobot(byte motorNumber, double vitesse)
        {
            OnPriseBalleEvent?.Invoke(this, new SpeedConsigneToMotorArgs { MotorNumber = motorNumber, V = vitesse });
        }
    }
}
