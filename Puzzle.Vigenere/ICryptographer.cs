using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Vigenere
{
	public interface ICryptographer
	{
		public string Alphabet { get; }

		public string Encrypt(string source, string key);

		public string Decrypt(string encrypted, string key);

		public IEnumerable<string> Encrypt(IEnumerable<string> source, string key);

		public IEnumerable<string> Decrypt(IEnumerable<string> encrypted, string key);
	}
}
