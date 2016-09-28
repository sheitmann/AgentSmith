using System;

using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;

namespace AgentSmith.SpellCheck
{
    public class ReplaceWordWithBulbItem : BulbActionBase
    {
        private readonly string _option;
        private readonly DocumentRange _documentRange;

        public ReplaceWordWithBulbItem(DocumentRange range, string option)
        {
            _option = option;
            _documentRange = range;
        }

        #region IBulbItem Members

        /*public void Execute(ISolution solution, ITextControl textControl)
        {
            PsiManager manager = solution.GetPsiServices().PsiManager;


            using (IProjectModelTransactionCookie cookie = solution.CreateTransactionCookie(DefaultAction.Commit, Text, NullProgressIndicator.Instance))
            {
            }
        }*/

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            return control => _documentRange.Document.ReplaceText(_documentRange.TextRange, _option);
        }

        public override string Text
        {
            get { return String.Format("Replace with '{0}'.", _option); }
        }

        #endregion
    }
}