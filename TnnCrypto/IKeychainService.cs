using System;
namespace TnnCrypto
{
    public interface IKeychainService
    {
        /// <summary>
        /// Create new record in Keychain if not exist
        /// Otherwise update the existing record
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        void SetString(string value, string key);

        /// <summary>
        /// Get value stored in existing record
        /// </summary>
        /// <param name="key"></param>
        /// <returns>value if found, otherwise null</returns>
        string GetString(string key);

        /// <summary>
        /// Remove record
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
