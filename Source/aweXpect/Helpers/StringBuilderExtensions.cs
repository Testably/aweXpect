using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class StringBuilderExtensions
{
	public static void ItWasNull(this StringBuilder stringBuilder, string it)
		=> stringBuilder.Append(it).Append(" was <null>");

	public static void ItWasNull(this StringBuilder stringBuilder, string it, ExpectationGrammars grammars)
		=> stringBuilder.Append(it).Append(grammars.SubjectVerb(it, " was", " were")).Append(" <null>");
}
