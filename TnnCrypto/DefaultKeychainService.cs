using System;
using Foundation;
using Security;

namespace TnnCrypto
{
    public class DefaultKeychainService : IKeychainService
    {
        static DefaultKeychainService()
        {
            string id = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleIdentifier")?.ToString() ?? "";
            _serviceId = $"__secured_userdefaults_{id}";
        }

        private static readonly string _serviceId;

        private static SecAccessible DefaultAccessible => SecAccessible.AfterFirstUnlock;

        public void SetString(string value, string key)
        {
            SecRecord newAttr = CreateRecordFromKey(key);
            newAttr.ValueData = NSData.FromString(value, NSStringEncoding.UTF8);

            // find existing
            SecRecord existing = GetRecord(key);
            if (existing != null)
            {
                SecKeyChain.Update(existing, newAttr);
            }
            else
            {
                SecKeyChain.Add(newAttr);
            }
        }

        public string GetString(string key)
        {
            SecRecord existing = GetRecord(key);
            if (existing != null)
            {
                var data = existing.ValueData;
                if (data != null)
                {
                    return new NSString(data, NSStringEncoding.UTF8);
                }
            }

            return null;
        }

        public void Remove(string key)
        {
            SecKeyChain.Remove(CreateRecordFromKey(key));
        }

        private static SecRecord GetRecord(string key)
        {
            SecRecord result = SecKeyChain.QueryAsRecord(CreateRecordFromKey(key), out SecStatusCode status);
            if (status == SecStatusCode.Success && result != null)
                return result;

            return null;
        }

        private static SecRecord CreateRecordFromKey(string key)
        {
            return new SecRecord(SecKind.GenericPassword)
            {
                Account = key,
                Service = _serviceId,
                Label = key,
                Accessible = DefaultAccessible
            };
        }
    }
}
