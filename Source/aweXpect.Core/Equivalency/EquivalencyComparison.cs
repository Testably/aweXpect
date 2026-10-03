using System;
using System.Collections.Generic;
using System.Linq;
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
	public static async ValueTask<bool>
		Compare<TActual, TExpected>(
			[RequiresMemberMetadata] TActual actual,
			[RequiresMemberMetadata] TExpected expected,
			EquivalencyOptions equivalencyOptions,
			StringBuilder failureBuilder)
	{
		int start = failureBuilder.Length;
		bool result = await Compare(
			actual,
			expected,
			equivalencyOptions,
			equivalencyOptions,
			failureBuilder,
			"",
			MemberType.Value,
			new EquivalencyContext());
		JoinSingleLineEntries(failureBuilder, start);
		return result;
	}

	/// <remarks>
	///     The entries are written with the "and" on its own line, because whether any of them spans several lines is
	///     only known once all of them are written.
	/// </remarks>
	private static void JoinSingleLineEntries(StringBuilder failureBuilder, int start)
	{
		if (failureBuilder.Length == start)
		{
			return;
		}

		string separator = $"{Environment.NewLine}and{Environment.NewLine}";
		string failures = failureBuilder.ToString(start, failureBuilder.Length - start);
		if (failures.Split([separator], StringSplitOptions.None).Any(entry => entry.TrimStart().Contains('\n', StringComparison.Ordinal)))
		{
			return;
		}

		failureBuilder.Replace(separator, $" and{Environment.NewLine}", start, failureBuilder.Length - start);
	}

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

		/// <summary>
		///     The number of nested comparisons on the current path.
		/// </summary>
		/// <remarks>
		///     Counted per path and not globally, so that two members on the same level are both at the same depth.
		/// </remarks>
		public int Depth { get; set; }

		/// <summary>
		///     The number of differences that were appended to a failure message so far.
		/// </summary>
		/// <remarks>
		///     Counted globally, so that a caller which is only interested in one comparison reads it before and after
		///     that comparison and restores it afterwards when the differences were written into a throwaway builder.
		/// </remarks>
		public int DifferenceCount { get; set; }

		/// <summary>
		///     Whether the comparison only decides whether the objects are equivalent, without writing or counting their
		///     differences.
		/// </summary>
		/// <remarks>
		///     Set while elements whose order is ignored are paired, as most of the pairs that are tried are not
		///     equivalent and never reported, so formatting their values would be wasted.
		/// </remarks>
		public bool IsDecidingOnly { get; set; }

		/// <summary>
		///     Whether the comparison only counts the differences of the objects, without writing them.
		/// </summary>
		/// <remarks>
		///     Set while the leftovers of elements whose order is ignored are ranked by their number of differences, as
		///     only the pairs that are reported in the end are written.
		/// </remarks>
		public bool IsCountingOnly { get; set; }

		/// <summary>
		///     The options registered for a type, or <see langword="null" /> when it has no registration.
		/// </summary>
		/// <remarks>
		///     Cached, because resolving a registration invokes its callback, and every compared pair looks up both
		///     of its types.
		/// </remarks>
		public Dictionary<Type, EquivalencyTypeOptions?> RegisteredOptions { get; } = [];

		private EquivalencyTypeOptions? _lastParentOptions;
		private EquivalencyTypeOptions? _lastInheritedOptions;

		/// <summary>
		///     Returns the options that a type without a registration inherits from the
		///     <paramref name="parentOptions" />.
		/// </summary>
		/// <remarks>
		///     The members of one object share their parent options, so the inherited copy is kept for the last parent
		///     instead of being created again for every member.
		/// </remarks>
		public EquivalencyTypeOptions GetInheritedOptions(EquivalencyOptions equivalencyOptions,
			EquivalencyTypeOptions parentOptions)
		{
			if (!ReferenceEquals(parentOptions, _lastParentOptions))
			{
				_lastParentOptions = parentOptions;
				_lastInheritedOptions = equivalencyOptions.GetInheritedOptions(parentOptions);
			}

			return _lastInheritedOptions!;
		}
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
