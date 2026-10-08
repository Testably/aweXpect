using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class StringBuilderExtensions
{
	public static void ItWasNull(this StringBuilder stringBuilder, string it)
		=> stringBuilder.Append(it).Append(" was <null>");

	public static void ItWasNull(this StringBuilder stringBuilder, string it, ExpectationGrammars grammars)
		=> stringBuilder.Append(it).Append(grammars.SubjectVerb(it, " was", " were")).Append(" <null>");

	public static StringBuilder AppendItemCount(this StringBuilder stringBuilder, int count)
		=> stringBuilder.Append(count).Append(count == 1 ? " item" : " items");

	/// <summary>
	///     Appends the <paramref name="expected" /> item of an expected collection, which names the kind of a string
	///     pattern, so that it is not mistaken for the value the item had to be equal to.
	/// </summary>
	public static void AppendExpectedItem<TMatch>(this StringBuilder stringBuilder, TMatch expected,
		IOptionsEquality<TMatch> options, string? indentation)
	{
		object itemOptions = options is IOptionsProvider<object> provider ? provider.Options : options;
		if (itemOptions is StringEqualityOptions stringEqualityOptions)
		{
			stringBuilder.Append(stringEqualityOptions.FormatExpectedItem(expected as string));
		}
		else
		{
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
		}
	}
}
