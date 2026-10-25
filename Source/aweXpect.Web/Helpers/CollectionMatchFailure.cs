using System.Text;

namespace aweXpect.Helpers;

/// <summary>
///     The failure of the values of a header or query parameter, which follows their list.
/// </summary>
internal static class CollectionMatchFailure
{
	/// <summary>
	///     The subject of the failure, which refers to the listed values.
	/// </summary>
	public const string Subject = "which";

	/// <summary>
	///     Appends the <paramref name="failure" />, if any, to the listed values.
	/// </summary>
	public static void AppendTo(StringBuilder stringBuilder, string? failure, string? indentation)
	{
		if (failure is not null)
		{
			stringBuilder.Append(", ").Append(failure.Indent(indentation, false));
		}
	}
}
