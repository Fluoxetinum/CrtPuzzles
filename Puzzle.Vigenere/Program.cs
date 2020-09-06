using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Puzzle.Common;

namespace Puzzle.Vigenere
{
	class Program
	{
		private static ISimpleLogger _logger;
		private const string UserInputTemplate = "Puzzle.Vigenere.exe [encrypt|decrypt] [key] [in.txt] [out.txt]";
		private const string DefaultOutputFilePath = "out.txt";


		static void Main(string[] args)
		{
			_logger = new SimpleConsoleLogger();
			try
			{
				if (args.Length < 3 ||
					(args[0] != "encrypt" && args[0] != "decrypt"))
				{
					Console.WriteLine($"Program input format : '{UserInputTemplate}'");
					return;
				}

				string action = args[0];
				string key = args[1];
				string inputFilePath = args[2];
				string outputFilePath = DefaultOutputFilePath;
				if (args.Length > 3)
				{
					outputFilePath = args[3];
				}

				ICryptographer crypto = new VigenereCryptographer(_logger);
				string[] lines = File.ReadAllLines(inputFilePath);
				IEnumerable<string> result;

				if (action == "encrypt")
				{
					result = crypto.Encrypt(lines, key);
				}
				else
				{
					result = crypto.Decrypt(lines, key);
				}

				File.WriteAllLines(outputFilePath, result);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Unexpected error");
			}
		}
	}
}
