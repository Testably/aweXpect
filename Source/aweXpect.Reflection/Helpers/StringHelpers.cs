namespace aweXpect.Reflection.Helpers;

internal static class StringHelpers
{
	public static string PrefixIn(this string description)
	{
		if (description.StartsWith("in "))
		{
			return description;
		}

		return "in " + description;
	}
}
