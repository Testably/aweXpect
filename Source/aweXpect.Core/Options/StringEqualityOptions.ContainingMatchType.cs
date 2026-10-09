using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly IStringMatchType ContainingMatch = new ContainingMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a substring that must be contained in the actual string.
	/// </summary>
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public StringEqualityOptions Containing()
	{
		SetMatchType(ContainingMatch, nameof(Containing));
		return this;
	}

	private sealed class ContainingMatchType : IStringMatchType
	{
		private static bool Contains(string actual, string expected, IEqualityComparer<string> comparer)
		{
			if (actual.Length < expected.Length)
			{
				return false;
			}

			for (int index = 0; index <= actual.Length - expected.Length; index++)
			{
				string candidate = actual.Substring(index, expected.Length);
				if (AreEqualByComparer(comparer, candidate, expected))
				{
					return true;
				}
			}

			return false;
		}

		#region IMatchType Members

		/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
		public bool InspectsSubject => true;

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		public string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (string.IsNullOrEmpty(actual) || expected == null)
			{
				return $"{it} was {Formatter.Format(actual)}";
			}

			string contains =
				$"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
			if (actual.Length < expected.Length)
			{
				contains +=
					$" with a length of {actual.Length}, which is shorter than the expected length of {expected.Length}";
			}

			return contains;
		}

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
		public ValueTask<bool>
			AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			if (actual is null && expected is null)
			{
				return new ValueTask<bool>(true);
			}

			if (actual is null || expected is null)
			{
				return new ValueTask<bool>(false);
			}

			if (comparer is not null)
			{
				return new ValueTask<bool>(Contains(actual, expected, comparer));
			}

			return new ValueTask<bool>(actual.Contains(expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"{grammars.Verb("contains", "contain")} {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, false) =>
					$"containing {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(true, true) =>
					$"{grammars.Verb("does not contain", "do not contain")} {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, true) =>
					$"not containing {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
			};

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> " containing";

		/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			if (comparer != null)
			{
				return $" using {Formatter.Format(comparer.GetType())}";
			}

			if (ignoreCase)
			{
				return " ignoring case";
			}

			return "";
		}

		/// <inheritdoc cref="IStringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />
		public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			// StringEqualityOptions rejects the options that a built-in match type cannot honour.
		}

		/// <inheritdoc cref="IStringMatchType.ValidateExpected(string?)" />
		public void ValidateExpected(string? expected)
		{
			// StringEqualityOptions validates the expected value of a built-in match type.
		}

		#endregion
	}
}
