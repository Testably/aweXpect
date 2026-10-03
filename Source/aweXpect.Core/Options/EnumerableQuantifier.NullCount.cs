using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{

	/// <summary>
	///     Matches no count, because nothing can be ordered against <see langword="null" />, e.g.
	///     <c>more than &lt;null&gt;</c>.
	/// </summary>
	/// <remarks>
	///     The negation fails as well, as for a property that is compared for order against <see langword="null" />.
	/// </remarks>
	internal static EnumerableQuantifier OrderedAgainstNull(string text)
		=> new NullCountQuantifier(text, true);

	private sealed class NullCountQuantifier(string text, bool isOrderedAgainstNull) : EnumerableQuantifier
	{
		public override string ToString() => text;

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount) => true;

		/// <inheritdoc />
		public override bool IsSingle() => false;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
			=> isOrderedAgainstNull ? Outcome.FailureBothWays : Outcome.Failure;

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
