using System;
using System.Collections.Generic;

namespace Algo.Runtime.Build.Parser
{
    internal class LanguageParserService
    {
        #region Fields

        private static LanguageParserService _languageParserService;

        private readonly Dictionary<Type, LanguageParser> _languageParsers = new Dictionary<Type, LanguageParser>();

        #endregion

        #region Methods

        internal static LanguageParserService GetService()
        {
            return _languageParserService ?? (_languageParserService = new LanguageParserService());
        }

        internal static void KillService()
        {
            if (_languageParserService != null)
            {
                _languageParserService._languageParsers.Clear();
            }
        }

        internal T GetLanguageParser<T>() where T : LanguageParser
        {
            if (!_languageParsers.ContainsKey(typeof(T)))
            {
                _languageParsers.Add(typeof(T), (LanguageParser)Activator.CreateInstance(typeof(T)));
            }
            return (T)_languageParsers[typeof(T)];
        }

        #endregion
    }
}
