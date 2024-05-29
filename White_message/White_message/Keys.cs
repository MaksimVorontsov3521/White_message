using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace White_message
{
    class Keys
    {
        uint openkey = 0;
        uint Open_e = 0;
        private double d;
        public Keys()
        {
            createkeys(ref openkey, ref Open_e);
            Console.WriteLine("Done");
        }
        private void createkeys(ref uint openkey, ref uint Open_e)
        {
            uint[] simplenum = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131 };
            Random random = new Random();
            uint A, B; A = simplenum[random.Next(0, 23)]; B = simplenum[random.Next(0, 23)];
            openkey = A * B;
            uint fi = (A - 1) * (B - 1);
            int i = -1;
            while (Open_e == 0)
            {
                i++;
                Open_e = fi % simplenum[i];
            }
            Open_e = simplenum[i];
            d = 1.5; i = 1;
            while (IsInteger(d) == false)
            {
                d = (fi * i + 1) / Open_e;
            }

        }
        public string decoder(string intmessage, int bytesReceived)
        {
            byte[] buffer = new byte[1024];
            string[] letters = intmessage.Split(',');
            uint[] message = new uint[letters.Length];
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = Convert.ToUInt32(letters);
                message[i] = message[i] ^ Convert.ToUInt32(d);
                message[i] = message[i] % openkey;
                buffer[i] = Convert.ToByte(message[i]);
            }
            string replyMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            return replyMessage;
        }
        public byte[] coder(byte[] Bmessage, uint e, uint OpenKey)
        {
            string intmessage = null;
            uint[] message = new uint[Bmessage.Length];
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = Convert.ToUInt32(Bmessage[i]);
            }
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = (message[i] ^ e) % OpenKey;
            }
            for (int i = 0; i < message.Length; i++)
            {
                intmessage += Convert.ToString(message[i]) + ",";
            }
            byte[] buffer = Encoding.UTF8.GetBytes(intmessage);
            return buffer;
        }

        public static bool IsInteger(double number)
        {
            return number == Math.Truncate(number);
        }
    }
}
