using System;

using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AgentSmith.SpellCheck
{
    public abstract class SpellCheckHighlightBase : IHighlighting
    {
        private readonly string _word;
        private readonly ISolution _solution;
        private readonly ISpellChecker _spellChecker;

        private readonly DocumentRange _range;

        private readonly IContextBoundSettingsStore _settingsStore;

        protected SpellCheckHighlightBase(DocumentRange range, string misspelledWord,
            ISolution solution, ISpellChecker spellChecker, IContextBoundSettingsStore settingsStore)
        {
            _range = range;
            _word = misspelledWord;
            _solution = solution;
            _spellChecker = spellChecker;
            _settingsStore = settingsStore;
        }

        /// <summary>
        /// The word that is misspelled
        /// </summary>
        public string MisspelledWord
        {
            get { return _word; }
        }

        /// <summary>
        /// The solution that the misspelled word is part of
        /// </summary>
        public ISolution Solution
        {
            get { return _solution; }
        }

        /// <summary>
        /// The spell checker instance that was used to detect the issue.
        /// </summary>
        public ISpellChecker SpellChecker
        {
            get { return _spellChecker; }
        }

        /// <summary>
        /// The document range where the error occurs.
        /// </summary>
        public DocumentRange DocumentRange
        {
            get { return _range; }
        }

        /// <summary>
        /// Returns true if data (PSI, text ranges) associated with highlighting is valid
        /// </summary>
        public virtual bool IsValid()
        {
            return true;
        }

	    /// <summary>
	    /// Calculates range of a highlighting.
	    /// </summary>
	    public DocumentRange CalculateRange() {
		    return _range;
	    }

	    /// <summary>
        /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
        ///             To override the default mechanism of tooltip, mark the implementation class with 
        ///             <see cref="T:JetBrains.ReSharper.Daemon.DaemonTooltipProviderAttribute"/> attribute, and then this property will not be called
        /// </summary>
        public virtual string ToolTip {
            get { return String.Format("Word '{0}' is not in dictionary.", MisspelledWord); }
        }

        /// <summary>
        /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
        /// </summary>
        public virtual string ErrorStripeToolTip { get { return ToolTip; } }

        /// <summary>
        /// Specifies the offset from the Range.StartOffset to set the cursor to when navigating 
        ///             to this highlighting. Usually returns <c>0</c>
        /// </summary>
        public virtual int NavigationOffsetPatch { get { return 0; } }


        public IContextBoundSettingsStore SettingsStore
        {
            get { return _settingsStore; }
        }
    }
}