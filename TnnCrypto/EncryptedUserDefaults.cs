using System;
using System.Diagnostics;
using Foundation;

namespace TnnCrypto
{
    public class EncryptedUserDefaults
    {
        private readonly IAead _provider;
        private readonly string _name;

        public static EncryptedUserDefaults CreateStandardEncryptedUserDefaults()
        {
            return new EncryptedUserDefaults("Standard", NSUserDefaults.StandardUserDefaults);
        }

        public EncryptedUserDefaults(string name, NSUserDefaults defaults) : this(name, defaults, new TinkAead(name))
        {
        }

        public EncryptedUserDefaults(string name, NSUserDefaults defaults, IAead provider)
        {
            _name = name;
            UserDefaults = defaults;
            _provider = provider;
        }

        public NSUserDefaults UserDefaults { get; }

        public void SetString(string value, string key)
        {
            SetData(DataFromString(value), key);
        }

        public string StringForKey(string key, string defValue)
        {
            if (!Contains(key))
                return defValue;

            NSData data = DataForKey(key);
            if (data == null)
                return defValue;

            return data.ToString(NSStringEncoding.UTF8);
        }

        public void SetInt(int value, string key)
        {
            SetBytes(BitConverter.GetBytes(value), key);
        }

        public int IntForKey(string key, int defValue)
        {
            return InternalGetForKey(key, defValue, BitConverter.ToInt32);
        }

        public void SetBool(bool value, string key)
        {
            SetBytes(BitConverter.GetBytes(value), key);
        }

        public bool BoolForKey(string key, bool defValue)
        {
            return InternalGetForKey(key, defValue, BitConverter.ToBoolean);
        }

        public void SetFloat(float value, string key)
        {
            SetBytes(BitConverter.GetBytes(value), key);
        }

        public float FloatForKey(string key, float defValue)
        {
            return InternalGetForKey(key, defValue, BitConverter.ToSingle);
        }

        public void SetDouble(double value, string key)
        {
            SetBytes(BitConverter.GetBytes(value), key);
        }

        public double DoubleForKey(string key, double defValue)
        {
            return InternalGetForKey(key, defValue, BitConverter.ToDouble);
        }

        public void SetBytes(byte[] bytes, string key)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            SetData(NSData.FromArray(bytes), key);
        }

        public byte[] BytesForKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            NSData data = DataForKey(key);
            return data?.ToArray();
        }

        public bool Contains(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return UserDefaults[EncryptKey(key)] != null;
        }
        
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            UserDefaults.RemoveObject(EncryptKey(key));
        }

        private T InternalGetForKey<T>(string key, T defValue, Func<byte[], int, T> converter)
        {
            if (!Contains(key))
                return defValue;

            byte[] bytes = BytesForKey(key);
            if (bytes == null)
                return defValue;

            try
            {
                return converter(bytes, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return defValue;
            }
        }

        private void SetData(NSData plainData, string key)
        {
            string encryptedKey = EncryptKey(key);
            Debug.WriteLine($"SetData: {key} => {encryptedKey}");
            UserDefaults[encryptedKey] = _provider.Encrypt(plainData, DataFromString(key));
        }

        private NSData DataForKey(string key)
        {
            string encryptedKey = EncryptKey(key);
            Debug.WriteLine($"DataForKey: {key} => {encryptedKey}");
            if (!(UserDefaults[encryptedKey] is NSData cipherData)) 
                return null;

            NSData plain = _provider.Decrypt(cipherData, DataFromString(key));
            cipherData.Dispose();
            return plain;
        }

        private string EncryptKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            
            NSData cipherKey = _provider.EncryptDeterministically(DataFromString(key), DataFromString(_name));
            return cipherKey.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
        }

        private static NSData DataFromString(string val)
        {
            if (val == null)
                return null;
            
            return NSData.FromString(val, NSStringEncoding.UTF8);
        }
    }
}
