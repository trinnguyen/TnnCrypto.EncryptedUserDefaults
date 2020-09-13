using System;
using System.Diagnostics;
using Foundation;
using Tink;

namespace TnnCrypto
{
    public class TinkAead : IAead
    {
        private readonly ITINKAead _aead;
        private readonly ITINKDeterministicAead _deterministicAead;

        public TinkAead(string alias)
        {
            // register
            RegisterAead();

            // load
            string keyAliasName = $"{NSBundle.MainBundle.BundleIdentifier}_encrypted_user_defaults_{alias}_key";
            string valueAliasName = $"{NSBundle.MainBundle.BundleIdentifier}_encrypted_user_defaults_{alias}_value";
            _deterministicAead = InitDeterministicAead(keyAliasName);
            _aead = InitAead(valueAliasName);
        }

        public NSData Encrypt(NSData plainData, NSData associatedData)
        {
            NSData result = _aead.Encrypt(plainData, associatedData, out NSError error);
            if (error != null)
            {
                Debug.WriteLine(error.LocalizedFailureReason);
                return null;
            }

            return result;
        }

        public NSData Decrypt(NSData cipherData, NSData associatedData)
        {
            NSData result = _aead.Decrypt(cipherData, associatedData, out NSError error);
            if (error != null)
            {
                Debug.WriteLine(error.LocalizedFailureReason);
                return null;
            }

            return result;
        }

        public NSData EncryptDeterministically(NSData plainData, NSData associatedData)
        {
            NSData result = _deterministicAead.EncryptDeterministically(plainData, associatedData, out NSError error);
            if (error != null)
            {
                Debug.WriteLine(error.LocalizedFailureReason);
                return null;
            }

            return result;
        }

        public NSData DecryptDeterministically(NSData cipherData, NSData associatedData)
        {
            NSData result = _deterministicAead.DecryptDeterministically(cipherData, associatedData, out NSError error);
            if (error != null)
            {
                Debug.WriteLine(error.LocalizedFailureReason);
                return null;
            }

            return result;
        }

        private static void RegisterAead()
        {
            var config = new TINKAeadConfig(out NSError error);
            if (error != null)
            {
                Debug.WriteLine(error.LocalizedFailureReason);
                throw new Exception("Failed to initialize Tink: " + error.LocalizedFailureReason);
            }
            TINKConfig.RegisterConfig(config, out error);
            if (error != null)
            {
                Debug.WriteLine(error.LocalizedFailureReason);
                throw new Exception("Failed to initialize Tink: " + error.LocalizedFailureReason);
            }
        }

        private static ITINKDeterministicAead InitDeterministicAead(string keyName)
        {
            TINKKeysetHandle handle = LoadOrCreateKey(keyName, CreateDeterministicAeadKey);
            ITINKDeterministicAead aeadDeter = TINKDeterministicAeadFactory.PrimitiveWithKeysetHandle(handle, out NSError error);
            if (error != null)
                throw new Exception("Failed to create DeterministicAead: " + error.LocalizedFailureReason);

            return aeadDeter;
        }

        private static ITINKAead InitAead(string keyName)
        {
            TINKKeysetHandle handle = LoadOrCreateKey(keyName, CreateAeadKey);
            ITINKAead aead = TINKAeadFactory.PrimitiveWithKeysetHandle(handle, out NSError error);
            if (error != null)
                throw new Exception("Failed to create Aead: " + error.LocalizedFailureReason);

            return aead;
        }

        private static TINKKeysetHandle CreateDeterministicAeadKey()
        {
            var tpl = new TINKDeterministicAeadKeyTemplate(TINKDeterministicAeadKeyTemplates.TINKAes256Siv, out NSError error);
            if (error == null)
                return new TINKKeysetHandle(tpl, out _);

            return null;
        }

        private static TINKKeysetHandle CreateAeadKey()
        {
            var tpl = new TINKAeadKeyTemplate(TINKAeadKeyTemplates.Aes256Gcm, out NSError error);
            if (error == null)
                return new TINKKeysetHandle(tpl, out _);

            return null;
        }

        private static TINKKeysetHandle LoadOrCreateKey(string keyName, Func<TINKKeysetHandle> createProvider)
        {
            TINKKeysetHandle handle = LoadFromKeychain(keyName);
            if (handle != null)
            {
                Debug.WriteLine("Found key in iOS keychain, loaded: " + keyName);
                return handle;
            }

            // create new and store
            handle = createProvider();
            if (handle == null)
                throw new Exception("Failed to create key: " + keyName);

            if (!handle.WriteToKeychainWithName(keyName, false, out NSError error))
                throw new Exception("Failed to store key to iOS keychain: " + keyName);

            return handle;
        }

        private static TINKKeysetHandle LoadFromKeychain(string keysetName)
        {
            try
            {
                TINKKeysetHandle handleStore = new TINKKeysetHandle(keysetName, out NSError error);
                if (error != null)
                {
                    Debug.WriteLine(error.LocalizedFailureReason);
                    return null;
                }

                return handleStore;

            }
            catch
            {
                return null;
            }
        }
    }
}
