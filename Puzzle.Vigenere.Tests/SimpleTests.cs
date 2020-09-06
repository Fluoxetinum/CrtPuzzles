using System;
using System.Collections.Generic;
using Puzzle.Common;
using Xunit;

namespace Puzzle.Vigenere.Tests
{
	public class SimpleTests
	{
		private readonly ICryptographer _crypto;


		public SimpleTests()
		{
			_crypto = new VigenereCryptographer(new SimpleConsoleLogger());
		}


		[Fact]
		public void SingleStringEncryption()
		{
			string sourceStr = "testString";
			string key = "key";
			string encryptedStr = _crypto.Encrypt(sourceStr, key);
			string decryptedStr = _crypto.Decrypt(encryptedStr, key);

			Assert.Equal(sourceStr.ToUpper(), decryptedStr);
		}


		[Fact]
		public void NotSupportedAlphabetException()
		{
			string sourceStr = "testСтрока";
			string key = "key";

			Assert.Throws<NotSupportedException>(() =>
			{
				_crypto.Encrypt(sourceStr, key);
			});
			Assert.Throws<NotSupportedException>(() =>
			{
				_crypto.Decrypt(sourceStr, key);
			});
		}


		[Fact]
		public void MultipleStringsEncryption()
		{
			string alphabet = _crypto.Alphabet;
			var sourceStrings = new List<string>();
			for (int i = 0; i < 100; i++)
			{
				string str =
					$"{alphabet[i % alphabet.Length]}" +
					$"{alphabet[(i + 1) % alphabet.Length]}" +
					$"{alphabet[(i + 2) % alphabet.Length]}";

				sourceStrings.Add(str);
			}
			string key = "key";

			IEnumerable<string> encryptedResult = _crypto.Encrypt(sourceStrings, key);
			IEnumerable<string> decrypted = _crypto.Decrypt(encryptedResult, key);

			Assert.Equal(sourceStrings, decrypted);
		}


		[Fact]
		public void EmptyArgumentsTest()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				_crypto.Decrypt("source", null);
			});
			Assert.Throws<ArgumentNullException>(() =>
			{
				_crypto.Encrypt((string)null, "key");
			});
			Assert.Throws<ArgumentNullException>(() =>
			{
				var cr = new VigenereCryptographer(null);
			});
		}
	}

	
}
