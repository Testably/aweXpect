using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches exactly <paramref name="expected" /> items, or no count at all when it is <see langword="null" />,
	///     because a count is never <see langword="null" />.
	/// </summary>
	internal static EnumerableQuantifier Exactly(int? expected)
		=> expected is null
			? new NullCountQuantifier("exactly <null>", false)
			: Exactly(expected.Value);

	/// <summary>
	///     Matches no count, because nothing can be ordered against <see langword="null" />, e.g.
	///     <c>more than &lt;null&gt;</c>.
	/// </summary>
	/// <remarks>
	///     The negation fails as well, as for a property that is compared for order against <see langword="null" />.
	/// </remarks>
	internal static EnumerableQuantifier OrderedAgainstNull(string text)
		=> new NullCountQuantifier(text, true);

	internal sealed class NullCountQuantifier(string text, bool isOrderedAgainstNull) : EnumerableQuantifier
	{
		public bool IsOrderedAgainstNull { get; } = isOrderedAgainstNull;

		public override string ToString() => text;

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount) => true;

		/// <inheritdoc />
		public override bool IsSingle() => false;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
			=> Outcome.Failure;

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			string it,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
			=> AppendCounts(stringBuilder, it, matchingCount, notMatchingCount, totalCount, verb, false);
	}
}
