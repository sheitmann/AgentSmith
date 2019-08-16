using System.Collections.Generic;


using JetBrains.Application.DataContext;

using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Refactorings.Specific.Rename;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;

#if RESHARPER20172
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.Actions.ActionManager;
using JetBrains.ReSharper.Refactorings.Convert.Function2Property;

#else
using JetBrains.ReSharper.Refactorings.Function2Property;
using JetBrains.ActionManagement;
#endif

#if RESHARPER20191
using JetBrains.Lifetimes;
#else
using JetBrains.DataFlow;
#endif

namespace AgentSmith.Identifiers
{
	internal class RenameBulbItem : IBulbAction
    {
        private readonly IDeclaration _declaration;

        private readonly string _targetName;

        public RenameBulbItem(IDeclaration declaration, string targetName = null)
        {
            _declaration = declaration;
            _targetName = targetName;
        }

        #region IBulbItem Members

        public void Execute(ISolution solution, ITextControl textControl)
        {

            IList<IDataRule> provider =
                DataRules.AddRule(
                    "ManualRenameRefactoringItem",
                    PsiDataConstants.DECLARED_ELEMENTS,
                    _declaration.DeclaredElement.ToDeclaredElementsDataConstant()
                ).AddRule(
                    "ManualRenameRefactoringItem",
                    TextControlDataConstants.TEXT_CONTROL,
                    textControl
                ).AddRule(
                    "ManualRenameRefactoringItem",
                    ProjectModelDataConstants.SOLUTION,
                    solution);
            
            /*
            if (Shell.Instance.IsTestShell)
                provider.AddRule(
                    "ManualRenameRefactoringItem",
                    RenameWorkflow.RenameDataProvider,
                    new RenameTestDataProvider("TestName", false, false)
                );
            */
            if (_targetName != null)
#if RESHARPER20172
	            provider.AddRule(
		            "ManualRenameRefactoringItem",
		            RenameRefactoringService.RenameDataProvider, new RenameDataProvider(_targetName));
#else
				provider.AddRule(
                "ManualRenameRefactoringItem",
                RenameRefactoringService.RenameDataProvider, new SimpleRenameDataProvider(_targetName));
#endif
#if RESHARPER20191
            Lifetime.Using(
#else
            Lifetimes.Using(
#endif
                (lifetime => {
			        var actionManager = solution.GetComponent<IActionManager>();
			        var context = actionManager.DataContexts.CreateWithDataRules(lifetime, provider);
#if RESHARPER20161
					RenameRefactoringService.Instance.ExecuteRename(context);
#else
					RenameRefactoringService.RenameFromContext(context);
#endif
				}));
        }

        public string Text
        {
            get
            {
                if (_targetName != null)
                {
                    return string.Format("Rename to {0}", _targetName);
                }
                return "Rename...";
            }
        }

#endregion
    }
}