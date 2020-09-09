using System;
using System.IO;
using System.Security.Cryptography;
using Foundation;

namespace TnnCrypto
{
    public class AesCrypto
    {
        protected readonly byte[] _key;
        protected readonly byte[] _iv;
        protected readonly IKeychainService _keychainService;

        public AesCrypto(string alias) : this(new DefaultKeychainService(), alias)
        {
        }

        public AesCrypto(IKeychainService keychainService, string alias)
        {
            _keychainService = keychainService;

            // load key
            Tuple<byte[], byte[]> keys = CreateOrLoadKeys(alias);
            _key = keys.Item1;
            _iv = keys.Item2;
        }

        private Tuple<byte[], byte[]> CreateOrLoadKeys(string alias)
        {
            // init name
            string nameKey = $"{alias}_aes_key";
            string nameIv = $"{alias}_aes_iv";

            // load from keychain
            var key = _keychainService.GetString(nameKey);
            if (string.IsNullOrWhiteSpace(key))
            {
                return CreateNewKeyIv(nameKey, nameIv);
            }

            var iv = _keychainService.GetString(nameIv);
            if (string.IsNullOrWhiteSpace(iv))
            {
                return CreateNewKeyIv(nameKey, nameIv);
            }

            return ToByteTuple(key, iv);
        }

        private Tuple<byte[], byte[]> CreateNewKeyIv(string nameKey, string nameIv)
        {
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();

                string key = Convert.ToBase64String(aes.Key);
                string iv = Convert.ToBase64String(aes.IV);
                _keychainService.SetString(key, nameKey);
                _keychainService.SetString(iv, nameIv);
                return ToByteTuple(key, iv);
            }
        }

        private Tuple<byte[], byte[]> ToByteTuple(string key, string iv)
        {
            return new Tuple<byte[], byte[]>(Convert.FromBase64String(key), Convert.FromBase64String(iv));
        }

        public string Encrypt(string value)
        {
            if (value == null)
                return null;

            ICryptoTransform encryptor = CreateEncryptor();
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(value);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string value)
        {
            if (value == null)
                return null;

            ICryptoTransform decryptor = CreateDecryptor();
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(value)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        public byte[] Encrypt(byte[] bytes)
        {
            if (bytes == null)
                return null;

            ICryptoTransform encryptor = CreateEncryptor();
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(bytes);
                    csEncrypt.FlushFinalBlock();
                    return msEncrypt.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] cipherData)
        {
            if (cipherData == null)
                return null;

            ICryptoTransform decryptor = CreateDecryptor();
            using (MemoryStream msEncrypt = new MemoryStream(cipherData))
            using (CryptoStream csDecrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    csDecrypt.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        private ICryptoTransform CreateEncryptor()
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                return aes.CreateEncryptor(aes.Key, aes.IV);
            }
        }

        private ICryptoTransform CreateDecryptor()
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                return aes.CreateDecryptor(aes.Key, aes.IV);
            }
        }
    }
}
