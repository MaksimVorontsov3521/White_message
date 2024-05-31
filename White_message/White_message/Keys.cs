using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace White_message
{
    class Keys
    {
        ulong openkey = 0;
        ulong Open_e = 0;
        private double d;
        public Keys()
        {
            createkeys(ref openkey, ref Open_e);
        }
        public ulong giveOpenkey()
        {
            return openkey;
        }
        public ulong giveOpen_e()
        {
            return Open_e;
        }
        private void createkeys(ref ulong openkey, ref ulong Open_e)
        {
            ulong A = 0, B = 0;
            ulong[] simplenum = { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131 };
            while (A == B)
            {
                Random random = new Random();
                A = simplenum[random.Next(0, 28)]; B = simplenum[random.Next(0, 28)];
            }
            Console.WriteLine("a=" + A + "b=" + B);
            openkey = A * B;
            ulong fi = (A - 1) * (B - 1);
            int i = -1;
            do
            {
                i++;
                Open_e = fi % simplenum[i];
            } while (Open_e == 0);
            Open_e = simplenum[i];

            d = 1.5; ulong ii = 0; bool Integer = false;
            while (Integer == false)
            {
                ii++;
                d = (fi * ii + 1) / Open_e;
                if ((fi * ii + 1) % Open_e == 0)
                {
                    Integer = true;
                }
            }
        }
        public string decoder(string intmessage, int bytesReceived)
        {
            string[] letters = intmessage.Split(',');
            byte[] buffer = new byte[letters.Length];
            ulong[] message = new ulong[letters.Length - 1];
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = Convert.ToUInt32(letters[i]);
                BigInteger buff = BigInteger.Pow(message[i], Convert.ToInt32(d));
                buff = buff % openkey;
                message[i] = (ulong)buff;
                buffer[i] = Convert.ToByte(message[i]);
            }
            string replyMessage = Encoding.UTF8.GetString(buffer);
            return replyMessage;
        }
        public byte[] coder(byte[] Bmessage, int e, uint OpenKey)
        {
            string intmessage = null;
            ulong[] message = new ulong[Bmessage.Length];
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = (ulong)Bmessage[i];
                BigInteger buff = BigInteger.Pow(message[i], e);
                buff = buff % OpenKey;
                message[i] = (ulong)buff;
                intmessage += Convert.ToString(message[i]) + ",";
            }
            byte[] buffer = Encoding.UTF8.GetBytes(intmessage);
            return buffer;
        }
    }

}

