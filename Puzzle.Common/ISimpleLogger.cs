using System;

namespace Puzzle.Common
{
    public interface ISimpleLogger
    {
        public void Info(string message);
        public void Error(Exception e, string message);
    }
}
