using Foundation;

namespace TnnCrypto
{
    public interface IAead
    {
        NSData Encrypt(NSData plainData, NSData associatedData);
        NSData Decrypt(NSData cipherData, NSData associatedData);
        NSData EncryptDeterministically(NSData plainData, NSData associatedData);
        NSData DecryptDeterministically(NSData cipherData, NSData associatedData);
    }
}