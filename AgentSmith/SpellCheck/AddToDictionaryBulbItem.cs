using System;

using AgentSmith.Options;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl;

namespace AgentSmith.SpellCheck
{
    public class AddToDictionaryBulbItem : IBulbAction
    {
        private readonly string _word;
        private readonly string _dictName;
        private DocumentRange _documentRange;

        private readonly IContextBoundSettingsStore _settingsStore;

        public AddToDictionaryBulbItem(string word, string dictName, DocumentRange range, IContextBoundSettingsStore settingsStore)
        {
            _word = word;
            _dictName = dictName;
            _documentRange = range;
            _settingsStore = settingsStore;
        }

        #region IBulbItem Members

        public void Execute(ISolution solution, ITextControl textControl)
        {
            ISettingsStore store = solution.GetComponent<ISettingsStore>();

            // Get the dictionary
            CustomDictionary dictionary =
                _settingsStore.GetIndexedValue<CustomDictionarySettings, string, CustomDictionary>(
                    settings => settings.CustomDictionaries, _dictName);

            if (dictionary == null) dictionary = new CustomDictionary() { Name = _dictName };

            string words = dictionary.DecodedUserWords.Trim();
            if (words.Length > 0)
            {
                dictionary.DecodedUserWords = words + "\n";
            }
            dictionary.DecodedUserWords += _word;

            IContextBoundSettingsStore boundStore = store.BindToContextTransient(ContextRange.ApplicationWide);

            boundStore.SetIndexedValue<CustomDictionarySettings, string, CustomDictionary>(x => x.CustomDictionaries, _dictName, dictionary);
            SpellCheckManager.Reset(); // Clear the cache.
            solution.SaveSettings();
			solution.GetComponent<IDaemon>().ForceReHighlight(_documentRange.Document);
        }

        public string Text
        {
            get { return String.Format("Add '{0}' to '{1}' user dictionary", _word, _dictName); }
        }

        #endregion
    }
}