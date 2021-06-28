using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerkulexManagerNS
{
    public static class CommonMethods
    {
        public static byte CheckSum1(byte pSIZE, byte pID, byte CMD, byte[] data)
        {
            byte checksum = (byte)(pSIZE ^ (byte)pID ^ CMD);
            for (int i = 0; i < data.Length; i++)
                checksum ^= data[i];
            checksum &= 0xFE;
            return checksum;
        }

        public static byte CheckSum1(byte pSIZE, byte pID, byte CMD)
        {
            byte checksum = (byte)(pSIZE ^ (byte)pID ^ CMD);
            checksum &= 0xFE;
            return checksum;
        }

        public static byte CheckSum2(byte checkSum1)
        {
            byte checkSum2 = (byte)((~checkSum1) & 0xFE);
            return checkSum2;
        }

        public static List<HerkulexDescription.ErrorStatus> GetErrorStatusFromByte(byte b)
        {
            List<HerkulexDescription.ErrorStatus> errorList = new List<HerkulexDescription.ErrorStatus>();
            for (int i = 0; i < 8; i++)
            {
                if ((byte)((b >> i) & 0x01) == 1)
                {
                    if (i == 0)
                        errorList.Add(HerkulexDescription.ErrorStatus.Exceed_input_voltage_limit);
                    if (i == 1)
                        errorList.Add(HerkulexDescription.ErrorStatus.Exceed_allowed_pot_limit);
                    if (i == 2)
                        errorList.Add(HerkulexDescription.ErrorStatus.Exceed_Temperature_limit);
                    if (i == 3)
                        errorList.Add(HerkulexDescription.ErrorStatus.Invalid_packet);
                    if (i == 4)
                        errorList.Add(HerkulexDescription.ErrorStatus.Overload_detected);
                    if (i == 5)
                        errorList.Add(HerkulexDescription.ErrorStatus.Driver_fault_detected);
                    if (i == 6)
                        errorList.Add(HerkulexDescription.ErrorStatus.EEP_REG_distorted);
                }
            }
            return errorList;
        }

        public static List<HerkulexDescription.ErrorStatusDetail> GetErrorStatusDetailFromByte(byte b)
        {
            List<HerkulexDescription.ErrorStatusDetail> errorList = new List<HerkulexDescription.ErrorStatusDetail>();
            for (int i = 0; i < 8; i++)
            {
                if ((byte)((b >> i) & 0x01) == 1)
                {
                    if (i == 0)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.Moving_flag);
                    if (i == 1)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.Inposition_flag);
                    if (i == 2)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.CheckSumError);
                    if (i == 3)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.Unknown_Command);
                    if (i == 4)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.Exceed_REG_RANGE);
                    if (i == 5)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.Garbage_detected);
                    if (i == 6)
                        errorList.Add(HerkulexDescription.ErrorStatusDetail.MOTOR_ON_flag);
                }
            }
            return errorList;
        }
    }
}
