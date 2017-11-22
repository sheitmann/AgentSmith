using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

using AgentSmith.Options;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.Util;

namespace AgentSmith.ResX
{
    internal class ResXProcess : IDaemonStageProcess
    {
        private readonly IPsiSourceFile _file;

        private readonly IDaemonProcess _daemonProcess;

        private readonly IContextBoundSettingsStore _settingsStore;

        public ResXProcess(IDaemonProcess process,
            IContextBoundSettingsStore settingsStore, IPsiSourceFile file)
        {
            _file = file;
            _daemonProcess = process;
            _settingsStore = settingsStore;
        }

        #region IDaemonStageProcess Members

        public void Execute(Action<DaemonStageResult> action)
        {
            IPsiModule module = _file.GetPsiModule();

            ResXSettings settings = _settingsStore.GetKey<ResXSettings>(SettingsOptimization.OptimizeDefault);

            IAttributesSet moduleAttributes = _file.GetSolution().GetPsiServices().Symbols.GetModuleAttributes(module);
            string defaultResXDic = "en-US";
            IList<IAttributeInstance> attributes = moduleAttributes
                .GetAttributeInstances(new ClrTypeName(typeof(NeutralResourcesLanguageAttribute).FullName), false);
            if (attributes != null &&
                attributes.Count > 0 &&
                attributes[0].PositionParameter(0).ConstantValue.Value != null)
            {
                defaultResXDic = attributes[0].PositionParameter(0).ConstantValue.Value.ToString();
            }

#if RESHARPER20173
	        var consumer = new DefaultHighlightingConsumer(_daemonProcess.SourceFile);
#else
			var consumer = new DefaultHighlightingConsumer(this, _settingsStore);  
#endif

            ISpellChecker checker = SpellCheckManager.GetSpellChecker(_settingsStore, _file, defaultResXDic);
            if (checker != null)
            {
                foreach (IXmlToken token in getStringsToCheck())
                {
                    WordLexer lexer = new WordLexer(token.GetText());
                    lexer.Start();
                    while (lexer.TokenType != null)
                    {
                        if (SpellCheckUtil.ShouldSpellCheck(lexer.TokenText, settings.CompiledWordsToIgnore) &&
                            !checker.TestWord(lexer.TokenText, false))
                        {
                            DocumentRange docRange = token.GetDocumentRange();
                            TextRange textRange = new TextRange(docRange.TextRange.StartOffset + lexer.TokenStart,
                                                                docRange.TextRange.StartOffset + lexer.TokenEnd);
                            DocumentRange range = new DocumentRange(docRange.Document, textRange);
                            
                            ResXSpellHighlighting highlighting =
                                new ResXSpellHighlighting(lexer.TokenText, _file, checker, range, _settingsStore);
                            
                            consumer.AddHighlighting(highlighting, range);
                        }
                        lexer.Advance();
                    }
                }
            }
			
            action(new DaemonStageResult(consumer.Highlightings));            
        }

        public IDaemonProcess DaemonProcess { get { return _daemonProcess; } }

        #endregion

        private IList<IXmlToken> getStringsToCheck()
        {
            IList<IXmlToken> tokens = new List<IXmlToken>();
            IXmlFile xmlFile = _file.GetTheOnlyPsiFile(XmlLanguage.Instance) as IXmlFile;

            if (xmlFile != null)
            {
                IXmlTag root = xmlFile.GetTag(delegate(IXmlTag tag) { return tag.GetTagName() == "root"; });

                if (root != null)
                {

                    IEnumerable<IXmlTag> datas = root.GetTags<IXmlTag>().Where(tag => tag.GetTagName() == "data");
                    foreach (IXmlTag data in datas)
                    {
                        if (data.GetAttribute("type") == null)
                        {
                            IXmlTag val = data.GetTag(delegate(IXmlTag tag) { return tag.GetTagName() == "value"; });
                            if (val != null)
                            {
                                if (val.FirstChild != null && val.FirstChild.NextSibling != null)
                                {
                                    IXmlToken value = val.FirstChild.NextSibling as IXmlToken;
                                    if (value != null)
                                    {
                                        tokens.Add(value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tokens;
        }
    }
}