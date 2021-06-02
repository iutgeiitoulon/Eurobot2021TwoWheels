using System;
using EventArgsLibrary;

namespace ConsoleFormatNS
{
    public class ConsoleFormat
    {
        private static long hex_received_index = 0;
        private static long hex_sender_index = 0;

        #region Setup in Main
        public static void InitMainConsole()
        {
            ConsoleInformationFormat("MAIN", "Begin Booting Sequence", true);
        }

        public static void SetupScichartLicenceKey()
        {
            ConsoleInformationFormat("SCICHART", "Setup Scichart RunTime Key", true);
        }

        public static void StartRobotInterface()
        {
            ConsoleInformationFormat("GUI", "Start Robot Interface", true);
        }

        public static void SetupAllCommunication()
        {
            ConsoleInformationFormat("USB", "USB Vendor is launched", true);
            ConsoleInformationFormat("DECODER", "Message Decoder is launched", true);
            ConsoleInformationFormat("ENCODER", "Message Encoder is launched", true);
            ConsoleInformationFormat("GENERATOR", "Message Generator is launched", true);
            ConsoleInformationFormat("PROCESSOR", "Message PROCESSOR is launched", true);
        }

        public static void SetupXboxController()
        {
            ConsoleInformationFormat("XBOX", "Setup XBOX Controller", true);
        }

        public static void PrintStrategyBoot()
        {
            ConsoleInformationFormat("STRATEGY", "Strategy is launched", true);
        }

        public static void EndMainBootSequence()
        {
            ConsoleInformationFormat("MAIN", "End Booting Sequence", true);
        }
        #endregion

        #region General Method
        static public void ConsoleTitleFormat(string title, bool isCorrect)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.Write("[");
            if (isCorrect)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(title);
            Console.ResetColor();
            Console.Write("] ");
        }
        static public void ConsoleInformationFormat(string title, string content, bool isCorrect = true)
        {
            ConsoleTitleFormat(title, isCorrect);
            Console.WriteLine(content);
        }

        static public void ConsoleListFormat(string content, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("    - " + content);
        }
        #endregion
        #region Usb Vendor
        static public void PrintNewDeviceAdded(object sender, EventArgs e)
        {
            ConsoleInformationFormat("USB", "New deviced has been added", true);
        }

        static public void PrintDeviceRemoved(object sender, EventArgs e)
        {
            ConsoleInformationFormat("USB", "A device had been removed", false);
        }

        static public void PrintUsbErrorExeption(object sender, string msg)
        {
            ConsoleInformationFormat("USB", msg, false);
        }
        #endregion

        #region Hex Decoder
        static public void PrintUnknowByte(object sender, byte e)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("0x" + e.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintSOF(object sender, byte e)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.ResetColor();
            Console.Write(hex_received_index++ + ": ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("0x" + e.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintFunctionMSB(object sender, byte e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("0x" + e.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintFunctionLSB(object sender, byte e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("0x" + e.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintLenghtMSB(object sender, byte e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("0x" + e.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintLenghtLSB(object sender, byte e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("0x" + e.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintPayloadByte(object sender, byte e)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("0x" + e.ToString("X2") + " ");
            // Console.ResetColor();

        }
        static public void PrintCorrectChecksum(object sender, MessageByteArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("0x" + e.checksum.ToString("X2") + " ");
            Console.ResetColor();
        }
        static public void PrintWrongChecksum(object sender, MessageByteArgs e)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0x" + e.checksum.ToString("X2") + " ");
            Console.ResetColor();
        }
        #endregion
        #region Hex Decoder Error
        static public void PrintOverLenghtWarning(object sender, EventArgs e)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n /!\\ WARNING A MESSAGE HAS EXCEED THE MAX LENGHT /!\\");
            Console.ResetColor();
        }

        static public void PrintWrongFonctionLenghtReceived(object sender, EventArgs e)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n /!\\ WARNING A FUNCTION HAS WRONG LENGHT /!\\");
            Console.ResetColor();
        }

        static public void PrintUnknowFunctionReceived(object sender, EventArgs e)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n /!\\ WARNING AN UNKNOW FUNCTION HAD BEEN RECEIVED /!\\");
            Console.ResetColor();
        }

        static public void PrintWrongMessage(object sender, MessageByteArgs e)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n /!\\ WARNING AN MESSAGED HAD BEEN CORRUPTED /!\\");
            Console.ResetColor();
        }
        #endregion
        #region Hex Encoder
        static public void PrintSendMsg(object sender, MessageByteArgs e)
        {
            Console.ResetColor();
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.Write(hex_sender_index++ + ": ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.Write("0x" + e.SOF.ToString("X2") + " ");
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write("0x" + e.functionMsb.ToString("X2") + " 0x" + e.functionLsb.ToString("X2") + " ");
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.Write("0x" + e.lenghtMsb.ToString("X2") + " 0x" + e.lenghtLsb.ToString("X2") + " ");
            Console.BackgroundColor = ConsoleColor.White;
            foreach (byte x in e.MsgPayload)
            {
                Console.Write("0x" + x.ToString("X2") + " ");
            }
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("0x" + e.checksum.ToString("X2") + " ");
            Console.ResetColor();
        }
        #endregion

        #region Hex Encoder Error
        static public void PrintOnSerialDisconnectedError(object sender, EventArgs e)
        {
            Console.ResetColor();
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("/!\\ WARNING MESSAGE CAN'T BE SENT BECAUSE SERIAL IS CLOSED /!\\");
            Console.ResetColor();
        }

        static public void PrintUnknowFunctionSent(object sender, EventArgs e)
        {
            Console.ResetColor();
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("/!\\ WARNING AN UNKNOW FUNCTION HAD BEEN SENT /!\\");
            Console.ResetColor();
        }

        static public void PrintWrongFunctionLenghtSent(object sender, EventArgs e)
        {
            Console.ResetColor();
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("/!\\ WARNING A FUNCTION WITH WRONG LENGHT HAS TRIED TO BE SENT /!\\");
            Console.ResetColor();
        }
        #endregion

        #region Hex Processor

        #endregion

        #region Lidar
        static public void NewLidarDeviceConnected(object sender, Lidar.LidarDevice e)
        {
            ConsoleInformationFormat("LIDAR", "A New Lidar Device is paired");
        }
        #endregion

    }
}
