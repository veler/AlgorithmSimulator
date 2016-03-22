using System;
using System.Collections.Generic;

namespace Algo.Runtime.Build.Parser
{
    internal class LanguageParserService
    {
        #region Fields

        private static LanguageParserService _languageParserService;

        private readonly Dictionary<string, LanguageParser> _languageParsers = new Dictionary<string, LanguageParser>();

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

        internal T GetLanguageParser<T>(object[] languageParserArguments) where T : LanguageParser
        {
            var key = typeof(T).FullName;
            if (languageParserArguments != null && languageParserArguments.Length > 0)
            {
                foreach (var languageParserArgument in languageParserArguments)
                {
                    key += languageParserArgument.ToString();
                }
            }

            if (!_languageParsers.ContainsKey(key))
            {
                _languageParsers.Add(key, (LanguageParser)Activator.CreateInstance(typeof(T), languageParserArguments));
            }
            return (T)_languageParsers[key];
        }

        #endregion
    }
}
