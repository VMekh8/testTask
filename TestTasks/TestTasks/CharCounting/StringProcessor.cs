using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTasks.CharCounting
{
    public class StringProcessor
    {
        private ConcurrentDictionary<char, int> _countChars;

        public (char symbol, int count)[] GetCharCount(string veryLongString, char[] countedChars)
        {
            if (string.IsNullOrEmpty(veryLongString))
                throw new ArgumentNullException($"{nameof(veryLongString)} was null");

            if (countedChars == null)
                throw new ArgumentNullException($"{nameof(countedChars)} was null");

            _countChars = new ConcurrentDictionary<char, int>(countedChars.ToDictionary(c => c, _ => 0));

            Parallel.ForEach(veryLongString, letter =>
            {
                if (_countChars.ContainsKey(letter))
                    _countChars.AddOrUpdate(letter, 1, (key, value) => value + 1);
            });

            return countedChars.Select(c => (c, _countChars[c])).ToArray();
        }
    }
}
