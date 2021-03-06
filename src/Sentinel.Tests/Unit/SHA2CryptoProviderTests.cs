﻿namespace Sentinel.Tests.Unit
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Sentinel.OAuth.Core.Constants.Identity;
    using Sentinel.OAuth.Core.Interfaces.Providers;
    using Sentinel.OAuth.Implementation.Providers;
    using Sentinel.OAuth.Models.Identity;
    using System;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    using HashAlgorithm = Sentinel.OAuth.Core.Constants.HashAlgorithm;

    [TestFixture]
    [Category("Unit")]
    public class SHA2CryptoProviderTests
    {
        private ICryptoProvider provider;

        [SetUp]
        public void SetUp()
        {
            this.provider = new SHA2CryptoProvider(OAuth.Core.Constants.HashAlgorithm.SHA256);
        }

        [TestCase(256)]
        [TestCase(384)]
        [TestCase(512)]
        public void Create_WhenGivenValidLength_ReturnsValidHash(int size)
        {
            var p = new SHA2CryptoProvider((HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), size.ToString()));
            var hash = p.CreateHash(size);
            var raw = Convert.FromBase64String(hash);

            Console.WriteLine($"Hash: {hash}");
            Console.WriteLine($"Hash Size: {size / 8} bits");
            Console.WriteLine($"Raw Size: {raw.Length} bits");

            Assert.AreEqual(size / 8, raw.Length);
        }

        [TestCase("aabbccddee")]
        public void Create_WhenGivenValidString_ReturnsHash(string text)
        {
            var hash = this.provider.CreateHash(text);

            Console.WriteLine("Hash: {0}", hash);

            Assert.IsNotNullOrEmpty(hash);
        }

        [Test]
        public void Create_WhenGeneratingString_ReturnsValidHash()
        {
            string text;
            var hash = this.provider.CreateHash(out text, 8);

            Console.WriteLine("Hash: {0}", hash);

            Assert.IsNotNullOrEmpty(hash);

            var valid = this.provider.ValidateHash(text, hash);

            Assert.IsTrue(valid);
        }

        [TestCase(8)]
        [TestCase(48)]
        [TestCase(64)]
        [TestCase(128)]
        public void Create_WhenGeneratingStringWithSpecificLength_ReturnsValidHash(int size)
        {
            string text;
            var hash = this.provider.CreateHash(out text, size);

            Console.WriteLine("Hash: {0}", hash);

            var textSize = Encoding.UTF8.GetBytes(text);

            Console.WriteLine("Text: {0}", text);
            Console.WriteLine("Text Size: {0} bits", textSize.Length * 8);

            Assert.AreEqual(size, textSize.Length * 8);

            Assert.IsNotNullOrEmpty(hash);

            var valid = this.provider.ValidateHash(text, hash);

            Assert.IsTrue(valid);
        }

        [TestCase(64, "aabbccddee")]
        [TestCase(48, "123")]
        [TestCase(64, "aabbccddee")]
        [TestCase(128, "aabbccddee")]
        public void Create_WhenGivenValidString_ReturnsHash(int saltByteSize, string text)
        {
            var p = new SHA2CryptoProvider(Sentinel.OAuth.Core.Constants.HashAlgorithm.SHA512, saltByteSize);

            var hash = p.CreateHash(text);

            Console.WriteLine("Hash: {0}", hash);

            Assert.IsNotNullOrEmpty(hash);
        }

        [TestCase("aabbccddee")]
        [TestCase("bfdsghbjflbnfkdkhgureinbfdlg")]
        public void Validate_WhenGivenValidTextAndHashCombination_ReturnsTrue(string text)
        {
            var hash = this.provider.CreateHash(text);
            var valid = this.provider.ValidateHash(text, hash);

            Assert.IsTrue(valid);
        }

        [TestCase("aabbccddee", "JsSAQQiINCmefez0E1vt9VLEwd6kMPD/M3/qwDOpuIw2WupF77UkJIYwzabp2G0CVhYPgDs+craqGzYX9anabcTePNfni5gJ79it0LnjiIng9A5guQ4wEwn+OCPgWZI+a7n7Uy2rNLd+0wTkYX/JX+dU+rgh0xhefIs1Gq5Vxuc=")]
        [TestCase("aabbccddee", "bdsbgdsnkjlnkls")]
        public void Validate_WhenGivenIncorrectTextAndHashCombination_ReturnsFalse(string text, string correctHash)
        {
            var valid = this.provider.ValidateHash(text, correctHash);

            Assert.IsFalse(valid);
        }

        [TestCase(64)]
        [TestCase(24)]
        public void Validate_WhenGivenAutoGeneratedString_ReturnsValid(int saltByteSize)
        {
            var p = new SHA2CryptoProvider(Sentinel.OAuth.Core.Constants.HashAlgorithm.SHA512, saltByteSize);

            var csprng = new RNGCryptoServiceProvider();
            var arr = new byte[64];

            csprng.GetBytes(arr);

            var text = Encoding.UTF8.GetString(arr);

            Console.WriteLine("Text: {0}", text);

            var hash = p.CreateHash(text);

            Console.WriteLine("Hash: {0}", hash);

            var valid = p.ValidateHash(text, hash);

            Assert.IsTrue(valid);
        }

        [TestCase("Lorem ipsum", "myspecialkey")]
        [TestCase("b dnsnfgrsnfgnfghnfgnfg", "some otherky")]
        public void Encrypt_WhenGivenString_ReturnsEncryptedString(string text, string key)
        {
            var r = this.provider.Encrypt(text, key);

            Console.WriteLine("Original: {0}", text);
            Console.WriteLine("Encrypted: {0}", r);

            Assert.IsNotNullOrEmpty(r);
        }

        [TestCase("v s bvzølnbdskøcmbøsdmvdøsbvjkdsb mvbdsvndjkls")]
        [TestCase("myspecialkey")]
        public void Decrypt_WhenGivenEncryptedString_ReturnsDecryptedString(string key)
        {
            var c1 = new SentinelPrincipal(new SentinelIdentity(AuthenticationType.OAuth, new SentinelClaim(ClaimTypes.Name, "azzlack")));
            var s = JsonConvert.SerializeObject(c1);

            var e = this.provider.Encrypt(s, key);

            Console.WriteLine("Original: {0}", s);
            Console.WriteLine();
            Console.WriteLine("Encrypted: {0}", e);
            Console.WriteLine();

            var d = this.provider.Decrypt(e, key);

            Console.WriteLine("Decrypted: {0}", d);

            var c2 = JsonConvert.DeserializeObject<SentinelPrincipal>(d);

            Assert.AreEqual(c1.Identity.Name, c2.Identity.Name);
        }
    }
}