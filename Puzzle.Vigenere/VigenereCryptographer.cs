using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Puzzle.Common;

namespace Puzzle.Vigenere
{
	public class VigenereCryptographer : ICryptographer
	{
		private readonly ISimpleLogger _logger;
		private readonly Dictionary<char, int> _alphabetPositions;
		private readonly int _coresCount;


		public VigenereCryptographer(ISimpleLogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			_logger = logger;
			_coresCount = Environment.ProcessorCount;
			_alphabetPositions = new Dictionary<char, int>();
			for (int i = 0; i < N; i++)
			{
				_alphabetPositions[((ICryptographer)this).Alphabet[i]] = i;
			}
		}

		string ICryptographer.Alphabet => "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890\'\".,:;`?! *()[]-_";
		

		public int N => ((ICryptographer)this).Alphabet.Length;


		string ICryptographer.Encrypt(string source, string key)
		{
			string encrypted = Transform(source, key,
				(sourceCharCode, keyCharCode, n) => (sourceCharCode + keyCharCode) % n);
			_logger.Info($"Encrypted : {source} => {encrypted}");
			return encrypted;
		}


		public string Decrypt(string encrypted, string key)
		{
			string decrypted = Transform(encrypted, key,
				(sourceCharCode, keyCharCode, n) => (sourceCharCode + n - keyCharCode) % n);
			_logger.Info($"Decrypted : {encrypted} => {decrypted}");
			return decrypted;
		}


		IEnumerable<string> ICryptographer.Encrypt(IEnumerable<string> source, string key)
		{
			var buff = source.Take(Int32.MaxValue);
			int i = 0;
			foreach (var str in buff)
			{
				_logger.Info($"Encrypting string #{i} '{str}'...");
				string encryptedStr = ((ICryptographer)this).Encrypt(str, key);
				yield return encryptedStr;
				i++;
			}
		}


		IEnumerable<string> ICryptographer.Decrypt(IEnumerable<string> encrypted, string key)
		{
			var buff = encrypted.Take(Int32.MaxValue);
			int i = 0;
			foreach (var str in buff)
			{
				_logger.Info($"Decrypting string #{i} '{str}'...");
				string decryptedStr = ((ICryptographer)this).Decrypt(str, key);
				yield return decryptedStr;
				i++;
			}
		}


		private string Transform(string source, string key, Func<int, int, int, int> transform)
		{
			if (key == null)
			{
				throw new ArgumentException(nameof(key));
			}
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (source == string.Empty)
			{
				return source;
			}

			StringBuilder strBuilder = new StringBuilder();

			string upperSource = source.ToUpper();
			string upperKey = key.ToUpper();

			int keyI = 0;
			foreach (char ch in upperSource)
			{
				char keyChar = upperKey[keyI];

				if (!_alphabetPositions.ContainsKey(ch) ||
				    !_alphabetPositions.ContainsKey(keyChar))
				{
					throw new NotSupportedException("Only english alphabet is supported.");
				}

				int sourceCharCode = _alphabetPositions[ch];
				int keyCharCode = _alphabetPositions[keyChar];

				int encryptedCharCode = transform(sourceCharCode, keyCharCode, N);

				strBuilder.Append(((ICryptographer)this).Alphabet[encryptedCharCode]);

				keyI++;
				if (keyI >= upperKey.Length)
				{
					keyI = 0;
				}
			}

			return strBuilder.ToString();
		}
	}
}
