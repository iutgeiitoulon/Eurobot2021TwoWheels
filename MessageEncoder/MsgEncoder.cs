using EventArgsLibrary;
using System;
using Constants;
using Protocol;

namespace MessageEncoder
{
    public class MsgEncoder
    {
        public void EncodeAndSendMessage(object sender, MessageByteArgs e)
        {
            short PayloadLenghtTest = Protocol_Security.CheckFunctionLenght(e.MsgFunction);
            ushort msgPayloadLenght = (ushort)e.MsgPayload.Length;
            if (PayloadLenghtTest != -2)
            {
                if (PayloadLenghtTest != -1)
                {
                    msgPayloadLenght = (ushort)PayloadLenghtTest;
                }
            }
            else
            {
                OnUnknownFunctionSent();
            }

            if (msgPayloadLenght == e.MsgPayload.Length)
            {
                byte[] msg = EncodeWithoutChecksum(e.MsgFunction, msgPayloadLenght, e.MsgPayload);
                byte checksum = CalculateChecksum(e.MsgFunction, msgPayloadLenght, e.MsgPayload);

                msg[msg.Length - 1] = checksum;
                OnSendMessage(msg);
            }
            else
            {
                OnWrongPayloadSent();
            }

        }

        private static byte[] EncodeWithoutChecksum(ushort msgFunction, ushort msgPayloadLength, byte[] msgPayload)
        {
            // Convert Function to byte
            byte LbyteFunction = (byte)(msgFunction >> 0);
            byte HbyteFunction = (byte)(msgFunction >> 8);

            byte LbytePayloadsLength = (byte)(msgPayloadLength >> 0);
            byte HbytePayloadsLength = (byte)(msgPayloadLength >> 8);

            // Append all bytes
            byte[] msg = new byte[6 + msgPayload.Length];
            ushort i;
            msg[0] = ConstVar.START_OF_FRAME;
            msg[1] = HbyteFunction;
            msg[2] = LbyteFunction;
            msg[3] = HbytePayloadsLength;
            msg[4] = LbytePayloadsLength;
            for (i = 0; i < msgPayload.Length; i++)
            {
                msg[5 + i] = msgPayload[i];
            }
            return msg;
        }

        private static byte CalculateChecksum(ushort msgFunction, ushort msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0;
            checksum ^= (byte)(msgFunction >> 8);
            checksum ^= (byte)(msgFunction >> 0);
            checksum ^= (byte)(msgPayloadLength >> 8);
            checksum ^= (byte)(msgPayloadLength >> 0);
            for (int i = 0; i < msgPayloadLength; i++)
            {
                checksum ^= msgPayload[i];
            }
            return checksum;
        }

        public event EventHandler<EventArgs> OnMessageEncoderCreatedEvent;
        public event EventHandler<MessageEncodedArgs> OnSendMessageEvent;
        public event EventHandler<MessageByteArgs> OnSendMessageByteEvent;
        public event EventHandler<EventArgs> OnSetResetPositionEvent;
        public event EventHandler<EventArgs> OnSerialDisconnectedEvent;
        public event EventHandler<EventArgs> OnWrongPayloadSentEvent;
        public event EventHandler<EventArgs> OnUnknownFunctionSentEvent;

        public virtual void OnMessageEncoderCreated()
        {
            OnMessageEncoderCreatedEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnSendMessage(byte[] msg)
        {
            OnSendMessageEvent?.Invoke(this, new MessageEncodedArgs { Msg = msg });
            OnSendMessageByteEvent?.Invoke(this, new MessageByteArgs(msg));
        }

        public virtual void OnSetResetPosition()
        {
            OnSetResetPositionEvent?.Invoke(this, new EventArgs());
        }

        public virtual void OnSerialDisconnected()
        {
            OnSerialDisconnectedEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnWrongPayloadSent()
        {
            OnWrongPayloadSentEvent?.Invoke(this, new EventArgs());
        }
        public virtual void OnUnknownFunctionSent()
        {
            OnUnknownFunctionSentEvent?.Invoke(this, new EventArgs());
        }
    }
}
