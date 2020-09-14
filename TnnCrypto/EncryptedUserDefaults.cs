using System;
using System.Diagnostics;
using Foundation;

namespace TnnCrypto
{
    public class EncryptedUserDefaults
    {
        private readonly NSUserDefaults _userDefaults;
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
            _userDefaults = defaults;
            _provider = provider;
        }

        public NSUserDefaults UserDefaults => _userDefaults;

        public void SetString(string value, string defaultName)
        {
            SetData(DataFromString(value), defaultName);
        }

        public string StringForKey(string defaultName)
        {
            NSData data = DataForKey(defaultName);
            return StringFromData(data);
        }

        public void SetInt(int value, string defaultName)
        {
            SetBytes(BitConverter.GetBytes(value), defaultName);
        }

        public int IntForKey(string defaultName)
        {
            byte[] bytes = BytesForKey(defaultName);
            return bytes != null ? BitConverter.ToInt32(bytes) : 0;
        }

        public void SetBool(bool value, string defaultName)
        {
            SetBytes(BitConverter.GetBytes(value), defaultName);
        }

        public bool BoolForKey(string defaultName)
        {
            byte[] bytes = BytesForKey(defaultName);
            return bytes != null && BitConverter.ToBoolean(bytes);
        }

        public void SetFloat(float value, string defaultName)
        {
            SetBytes(BitConverter.GetBytes(value), defaultName);
        }

        public float FloatForKey(string defaultName)
        {
            byte[] bytes = BytesForKey(defaultName);
            return bytes != null ? BitConverter.ToSingle(bytes) : 0;
        }

        public void SetDouble(double value, string defaultName)
        {
            SetBytes(BitConverter.GetBytes(value), defaultName);
        }

        public double DoubleForKey(string defaultName)
        {
            byte[] bytes = BytesForKey(defaultName);
            return bytes != null ? BitConverter.ToDouble(bytes) : 0;
        }

        public bool HasKey(string defaultName)
        {
            return _userDefaults[EncryptKey(defaultName)] != null;
        }
        
        public void Remove(string key)
        {
            _userDefaults.RemoveObject(EncryptKey(key));
        }

        private void SetData(NSData plainData, string defaultName)
        {
            string encryptedKey = EncryptKey(defaultName);
            Debug.WriteLine($"SetData: {defaultName} => {encryptedKey}");
            _userDefaults[encryptedKey] = _provider.Encrypt(plainData, DataFromString(defaultName));
        }

        private NSData DataForKey(string defaultName)
        {
            string encryptedKey = EncryptKey(defaultName);
            Debug.WriteLine($"DataForKey: {defaultName} => {encryptedKey}");
            if (!(_userDefaults[encryptedKey] is NSData cipherData)) 
                return null;

            NSData plain = _provider.Decrypt(cipherData, DataFromString(defaultName));
            cipherData.Dispose();
            return plain;
        }

        private string EncryptKey(string defaultName)
        {
            if (string.IsNullOrWhiteSpace(defaultName))
                throw new ArgumentNullException(nameof(defaultName));
            
            NSData cipherKey = _provider.EncryptDeterministically(DataFromString(defaultName), DataFromString(_name));
            return cipherKey.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
        }

        private void SetBytes(byte[] bytes, string defaultName)
        {
            SetData(NSData.FromArray(bytes), defaultName);
        }

        private byte[] BytesForKey(string defaultName)
        {
            NSData data = DataForKey(defaultName);
            return data?.ToArray();
        }

        private static NSData DataFromString(string val)
        {
            if (val == null)
                return null;
            
            return NSData.FromString(val, NSStringEncoding.UTF8);
        }

        private static string StringFromData(NSData data)
        {
            return data?.ToString(NSStringEncoding.UTF8);
        }
    }
}
