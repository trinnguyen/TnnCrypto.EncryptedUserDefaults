using System;
using Foundation;

namespace TnnCrypto
{
    public class EncryptedUserDefaults
    {
        private readonly AesCrypto _crypto;
        private readonly NSUserDefaults _userDefaults;

        public static EncryptedUserDefaults CreateStandardEncryptedUserDefaults()
        {
            return new EncryptedUserDefaults("StandardUserDefaults", NSUserDefaults.StandardUserDefaults);
        }

        public EncryptedUserDefaults(string name, NSUserDefaultsType type) : this(name, new NSUserDefaults(name, type))
        {
        }

        private EncryptedUserDefaults(string name, NSUserDefaults userDefaults)
        {
            _userDefaults = userDefaults;
            _crypto = new AesCrypto($"__encrypted_def_{name}");
        }

        public void SetString(string value, string defaultName)
        {
            var cipher = _crypto.Encrypt(value);
            _userDefaults.SetString(cipher, defaultName);
        }

        public string StringForKey(string defaultName)
        {
            var raw = _userDefaults.StringForKey(defaultName);
            if (!string.IsNullOrWhiteSpace(raw))
                return _crypto.Decrypt(raw);

            return null;
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

        public void SetInt(bool value, string defaultName)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            _userDefaults.SetString(defaultName, _crypto.Encrypt(Convert.ToBase64String(bytes)));
        }

        public void SetBool(bool value, string defaultName)
        {
            SetBytes(BitConverter.GetBytes(value), defaultName);
        }

        public bool BoolForKey(string defaultName)
        {
            byte[] bytes = BytesForKey(defaultName);
            return bytes != null ? BitConverter.ToBoolean(bytes) : false;
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
            return _userDefaults[defaultName] != null;
        }

        private void SetBytes(byte[] bytes, string defaultName)
        {
            byte[] cipher = _crypto.Encrypt(bytes);
            _userDefaults[defaultName] = NSData.FromArray(cipher);
        }

        private byte[] BytesForKey(string defaultName)
        {
            NSObject obj = _userDefaults[defaultName];
            if (obj is NSData)
            {
                using (NSData cipher = (NSData)obj)
                {
                    return _crypto.Decrypt(cipher.ToArray());
                }
            }

            return null;
        }
    }
}
