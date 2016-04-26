using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.MemberMatch
{
    public static class ComplexMatchEvaluator
    {
        public static Match IsMatch(IDeclaration decl, Match[] matches, Match[] notMatches, bool useEffectiveRights)
        {
            if (matches == null)
            {
                return null;
            }

            foreach (Match match in matches)
            {
                if (match.IsMatch(decl, useEffectiveRights))
                {
                    if (notMatches != null)
                    {
                        foreach (Match notMatch in notMatches)
                        {
                            if (notMatch.IsMatch(decl, useEffectiveRights))
                            {
                                return null;
                            }
                        }
                    }
                    return match;
                }
            }

            return null;
        }

		//public static void Prepare(ISolution solution, Match[] matches, Match[] notMatches)
		//{
		//	PsiManager psiManager = PsiManager.GetInstance(solution);
		//	if (matches != null)
		//	{
		//		foreach (Match match in matches)
		//		{
		//			match.Prepare(solution, psiManager);
		//		}
		//	}

		//	if (notMatches != null)
		//	{
		//		foreach (Match match in notMatches)
		//		{
		//			match.Prepare(solution, psiManager);
		//		}
		//	}
		//}
    }
}