using EventArgsLibrary;
using System;
using Constants;
using Protocol;

namespace MessageDecoder
{
    public class MsgDecoder
    {
        private enum State
        {
            Waiting,
            FunctionMSB,
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }

        static State actualState = State.Waiting;

        private static byte functionMSB;
        private static byte functionLSB;
        private static byte payloadLenghtMSB;
        private static byte payloadLenghtLSB;

        private static ushort msgFunction;
        private static ushort msgPayloadLenght;
        private static byte[] msgPayload;
        private static byte msgChecksum;

        private static int msgPayloadIndex = 0; // Maybe edit type
        public void ByteReceived(byte b)
        {
            switch (actualState)
            {
                case State.Waiting:
                    if (b == ConstVar.START_OF_FRAME)
                    {
                        OnSOFReceived(b);
                    }
                    else
                    {
                        OnUnknowReceived(b);
                    }
                    break;

                case State.FunctionMSB:
                    OnFunctionMSBReceived(b);
                    break;

                case State.FunctionLSB:
                    OnFunctionLSBReceived(b);
                    break;

                case State.PayloadLengthMSB:
                    OnPayloadLenghtMSBReceided(b);
                    break;

                case State.PayloadLengthLSB:
                    OnPayloadLenghtLSBReceided(b);
                    break;

                case State.Payload:
                    OnPayloadByteReceived(b);
                    break;

                case State.CheckSum:
                    OnCheckSumReceived(b);
                    break;
            }

        }

        #region Input Callback

        object lockDecode = new object();
        public void BuffReceived(object sender, DataReceivedArgs e)
        {
            lock (lockDecode)
            {
                foreach (var b in e.Data)
                {
                    ByteReceived(b);
                }
            }
        }
        #endregion

        public event EventHandler<EventArgs> OnMessageDecoderCreatedEvent;
        public event EventHandler<byte> OnSOFByteReceivedEvent;
        public event EventHandler<byte> OnUnknowByteEvent;
        public event EventHandler<byte> OnFunctionMSBByteReceivedEvent;
        public event EventHandler<byte> OnFunctionLSBByteReceivedEvent;
        public event EventHandler<byte> OnPayloadLenghtMSBByteReceivedEvent;
        public event EventHandler<byte> OnPayloadLenghtLSBByteReceivedEvent;
        public event EventHandler<byte> OnPayloadByteReceivedEvent;
        public event EventHandler<DecodePayloadArgs> OnPayloadReceivedEvent;
        public event EventHandler<byte> OnChecksumByteReceivedEvent;
        public event EventHandler<MessageByteArgs> OnCorrectMessageReceivedEvent;
        public event EventHandler<MessageByteArgs> OnErrorMessageReceivedEvent;
        public event EventHandler<EventArgs> OnOverLenghtMessageEvent;
        public event EventHandler<EventArgs> OnUnknowFunctionEvent;
        public event EventHandler<EventArgs> OnWrongLenghtFunctionEvent;

        public virtual void OnMessageDecoderCreated()
        {
            OnMessageDecoderCreatedEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnSOFReceived(byte e)
        {
            actualState = State.FunctionMSB;
            OnSOFByteReceivedEvent?.Invoke(this, e);

        }
        public virtual void OnUnknowReceived(byte e)
        {
            OnUnknowByteEvent?.Invoke(this, e);

        }
        public virtual void OnFunctionMSBReceived(byte e)
        {
            functionMSB = e;
            msgFunction = (ushort)(e << 8);
            actualState = State.FunctionLSB;
            OnFunctionMSBByteReceivedEvent?.Invoke(this, e);
        }
        public virtual void OnFunctionLSBReceived(byte e)
        {
            functionLSB = e;
            msgFunction += (ushort)(e << 0);
            OnFunctionLSBByteReceivedEvent?.Invoke(this, e);
            if (Protocol_Security.CheckFunctionLenght(msgFunction) != -2)
            {
                actualState = State.PayloadLengthMSB;
            }
            else
            {
                actualState = State.Waiting;
                OnUnknowFunction();
            }

        }
        public virtual void OnPayloadLenghtMSBReceided(byte e)
        {
            payloadLenghtMSB = e;
            msgPayloadLenght = (ushort)(e << 8);
            actualState = State.PayloadLengthLSB;
            OnPayloadLenghtMSBByteReceivedEvent?.Invoke(this, e);
        }
        public virtual void OnPayloadLenghtLSBReceided(byte e)
        {
            payloadLenghtLSB = e;
            msgPayloadLenght += (ushort)(e << 0);
            actualState = State.Waiting;
            OnPayloadLenghtLSBByteReceivedEvent?.Invoke(this, e);
            if (msgPayloadLenght <= ConstVar.MAX_MSG_LENGHT)
            {
                short allowedLenght = Protocol_Security.CheckFunctionLenght(msgFunction);
                if (allowedLenght != -2)
                {
                    if (allowedLenght == -1 || allowedLenght == msgPayloadLenght)
                    {
                        if (allowedLenght > 0)
                            actualState = State.Payload;
                        else
                            actualState = State.CheckSum;
                        msgPayloadIndex = 0;
                        msgPayload = new byte[msgPayloadLenght];
                    }
                    else
                    {
                        OnWrongLenghtFunction();
                    }
                }
                else
                {
                    OnUnknowFunction();
                }

            }
            else
            {
                OnOverLenghtMessage();
            }

        }



        public virtual void OnOverLenghtMessage()
        {
            OnOverLenghtMessageEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnUnknowFunction()
        {
            OnUnknowFunctionEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnWrongLenghtFunction()
        {
            OnWrongLenghtFunctionEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnPayloadByteReceived(byte e)
        {
            msgPayload[msgPayloadIndex] = e;
            msgPayloadIndex++;
            OnPayloadByteReceivedEvent?.Invoke(this, e);
            if (msgPayloadIndex == msgPayloadLenght)
            {
                OnPayloadReceived(msgPayload);
            }
            
        }
        public virtual void OnPayloadReceived(byte[] e)
        {
            actualState = State.CheckSum;
            OnPayloadReceivedEvent?.Invoke(this, new DecodePayloadArgs(e));
        }
        public virtual void OnCheckSumReceived(byte e)
        {
            msgChecksum = e;
            if (msgChecksum == CalculateChecksum())
            {
                OnCorrectMessageReceived();
            }
            else
            {
                OnErrorMessageReceived();
            }
            actualState = State.Waiting;
            OnChecksumByteReceivedEvent?.Invoke(this, e);
        }
        public virtual void OnCorrectMessageReceived()
        {
            OnCorrectMessageReceivedEvent?.Invoke(this, new MessageByteArgs(msgFunction, msgPayloadLenght, msgPayload, msgChecksum));
        }
        public virtual void OnErrorMessageReceived()
        {
            OnErrorMessageReceivedEvent?.Invoke(this, new MessageByteArgs(msgFunction, msgPayloadLenght, msgPayload, msgChecksum));
        }
        private static byte CalculateChecksum()
        {
            byte checksum = 0;
            checksum ^= functionMSB;
            checksum ^= functionLSB;
            checksum ^= payloadLenghtMSB;
            checksum ^= payloadLenghtLSB;
            foreach (byte x in msgPayload)
            {
                checksum ^= x;
            }
            return checksum;
        }

        
       
    }
}
