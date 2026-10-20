using System;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     Checks the arguments of the snapshot rules.
/// </summary>
internal static class RuleArguments
{
	public static T ThrowIfNull<T>(T value, string parameterName) where T : class
		=> value ?? throw Tracing.WriteException(new ArgumentNullException(parameterName,
			$"The '{parameterName}' cannot be null."));

	/// <summary>
	///     Throws for a <see langword="null" /> or empty <paramref name="text" />.
	/// </summary>
	public static string ThrowIfNullOrEmpty(string text, string parameterName)
	{
		if (text is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(parameterName,
				$"The '{parameterName}' cannot be null."));
		}

		if (text.Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException($"The '{parameterName}' cannot be empty.",
				parameterName));
		}

		return text;
	}
}
