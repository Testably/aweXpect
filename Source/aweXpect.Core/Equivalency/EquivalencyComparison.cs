using System.Collections.Generic;
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
		///     Tracks already compared objects to catch recursions.
		/// </summary>
		public HashSet<object> ComparedObjects { get; } = new(ReferenceEqualityComparer.Instance);
	}
}
