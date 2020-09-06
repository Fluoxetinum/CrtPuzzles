using System;

namespace Puzzle.Common
{
    public class SimpleConsoleLogger : ISimpleLogger
    {
        void ISimpleLogger.Info(string message)
        {
            Console.WriteLine(message);
        }


        void ISimpleLogger.Error(Exception e, string message)
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
