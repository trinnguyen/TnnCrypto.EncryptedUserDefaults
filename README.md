# Encrypted UserDefaults for Xamarin.iOS (AES256)
[![NuGet version](https://badge.fury.io/nu/TnnCrypto.EncryptedUserDefaults.svg)](https://badge.fury.io/nu/TnnCrypto.EncryptedUserDefaults)

- Use Google Tink Crypto library for encrypting/decrypting key and value
- Keyset is generated at runtime if not exist, stored in iOS Keychain

## Install Nuget package
- `<PackageReference Include="TnnCrypto.EncryptedUserDefaults" Version="1.0.0" />`

## Usage
- Checkout tests: [EncryptedUserDefaultsTests.cs](TnnCrypto.Tests/Tests/EncryptedUserDefaultsTests.cs)