using System;
using System.Security.Cryptography;
using System.Text;

namespace White_server
{
    class Keys
    {
        public string myPublicKey { get; set; }
        public string NPublicKey { get; set; }
        private string privateKey;
        public Keys()
        {   
            // Генерация RSA ключей
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    myPublicKey = rsa.ToXmlString(false);
                    privateKey = rsa.ToXmlString(true);
                }
                catch(CryptographicException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }
        public byte[] Encrypt(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(NPublicKey);
                return rsa.Encrypt(data, false);
            }
        }
        public string Decrypt(byte[] data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                byte[] buff=rsa.Decrypt(data, false);
                return Encoding.UTF8.GetString(buff);        
            }
        }
        
    }
}
