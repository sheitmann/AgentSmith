using System.Collections.Generic;

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Refactorings.Specific.Rename;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Refactorings.Function2Property;
using JetBrains.ReSharper.Refactorings.Rename;
using JetBrains.TextControl;

using DataConstants = JetBrains.ReSharper.Psi.Services.DataConstants;
using ShellComponentsEx = JetBrains.ActionManagement.ShellComponentsEx;

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
                    DataConstants.DECLARED_ELEMENTS,
                    _declaration.DeclaredElement.ToDeclaredElementsDataConstant()
                ).AddRule(
                    "ManualRenameRefactoringItem",
                    JetBrains.TextControl.DataContext.DataConstants.TEXT_CONTROL,
                    textControl
                ).AddRule(
                    "ManualRenameRefactoringItem",
                    JetBrains.ProjectModel.DataContext.DataConstants.SOLUTION,
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
            provider.AddRule(
                "ManualRenameRefactoringItem",
                RenameRefactoringService.RenameDataProvider, new SimpleRenameDataProvider(_targetName));

	        Lifetimes.Using(
		        (lifetime => {
			        var actionManager = solution.GetComponent<IActionManager>();
			        var context = actionManager.DataContexts.CreateWithDataRules(lifetime, provider);
			        RenameRefactoringService.Instance.ExecuteRename(context);
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