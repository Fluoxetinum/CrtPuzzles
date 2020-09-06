using System;

namespace Puzzle.Common
{
    public class SimpleConsoleLogger : ISimpleLogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }


        public void Error(Exception e, string message)
        {
	        if (e == null)
	        {
		        throw new ArgumentNullException(nameof(e));
	        }

            string errorMessage = $"{message}.\n" +
                                  $"{e.Message}\n" +
                                  $"{e.StackTrace}\n";

            Console.WriteLine(errorMessage);
        }
    }
}
