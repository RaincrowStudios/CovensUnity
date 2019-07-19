using System.Collections.Generic;

namespace Raincrow.Utils
{
    public class PadlockSet
    {
        /// <summary>
        /// Padlock set
        /// </summary>
        private HashSet<string> _padlockSet = new HashSet<string>();

        /// <summary>
        /// Add a new padlock to our Padlock Set
        /// </summary>
        /// <param name="padlockName">name of our padlock</param>
        /// <returns>a flag that tells if the lock was added</returns>
        public bool AddPadlock(string padlockName)
        {
            _padlockSet.Add(padlockName);
            return _padlockSet.Contains(padlockName);
        }

        /// <summary>
        /// Remove a padlock from our Padlock Set
        /// </summary>
        /// <param name="padlockName">name of our padlock</param>
        /// <returns>a flag that tells if the lock was removed</returns>
        public bool RemovePadlock(string padlockName)
        {
            _padlockSet.Remove(padlockName);
            return _padlockSet.Contains(padlockName);
        }

        public bool HasPadlocks()
        {
            return _padlockSet.Count > 0;
        }        
    }
}
