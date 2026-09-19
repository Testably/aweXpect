using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     Allows comparing two instances for equivalency.
/// </summary>
public static partial class EquivalencyComparison
{
	/// <summary>
	///     Checks if <paramref name="actual" /> is considered equivalent to <paramref name="expected" /> using the
	///     <paramref name="equivalencyOptions" />.
	/// </summary>
	/// <remarks>
	///     In case of a difference, the <paramref name="failureBuilder" /> contains a human readable explanation.
	/// </remarks>
#if NET8_0_OR_GREATER
	public static ValueTask<bool>
#else
	public static Task<bool>
#endif
		Compare<TActual, TExpected>(
			[RequiresMemberMetadata] TActual actual,
			[RequiresMemberMetadata] TExpected expected,
			EquivalencyOptions equivalencyOptions,
			StringBuilder failureBuilder)
		=> Compare(
			actual,
			expected,
			equivalencyOptions,
			equivalencyOptions.GetTypeOptions(actual?.GetType(), equivalencyOptions),
			failureBuilder,
			"",
			MemberType.Value,
			new EquivalencyContext());

	private sealed class EquivalencyContext
	{
		/// <summary>
		///     Tracks the pairs that are compared on the current path to catch recursions.
		/// </summary>
		/// <remarks>
		///     Only the ancestors of the current pair are tracked, so that an instance which is reached again via a
		///     second, independent path is still compared against its own expected counterpart.
		/// </remarks>
		public HashSet<ComparedPair> ComparedPairs { get; } = [];
	}

	private readonly struct ComparedPair : IEquatable<ComparedPair>
	{
		private readonly object _actual;
		private readonly object _expected;

		public ComparedPair(object actual, object expected)
		{
			_actual = actual;
			_expected = expected;
		}

		public bool Equals(ComparedPair other)
			=> ReferenceEquals(_actual, other._actual) && ReferenceEquals(_expected, other._expected);

		public override bool Equals(object? obj) => obj is ComparedPair other && Equals(other);

		public override int GetHashCode()
			=> (RuntimeHelpers.GetHashCode(_actual) * 397) ^ RuntimeHelpers.GetHashCode(_expected);
	}
}
