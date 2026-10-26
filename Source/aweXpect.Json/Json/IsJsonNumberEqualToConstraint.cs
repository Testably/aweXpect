using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System;
#endif

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON number that fits into a <typeparamref name="TNumber" /> and is equal to the
///     <paramref name="expected" /> value within the <paramref name="options" />.
/// </summary>
internal sealed class IsJsonNumberEqualToConstraint<TNumber>(
	string it,
	ExpectationGrammars grammars,
	JsonValueReader<TNumber> reader,
	TNumber expected,
	OptionsHolder<NumberTolerance<TNumber>> options)
	: IsJsonValueEqualToConstraint<TNumber>(it, grammars, reader)
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>
#else
	where TNumber : struct, IComparable<TNumber>
#endif
{
	protected override ValueTask<bool> IsMatch(TNumber value, IEvaluationContext context,
		CancellationToken cancellationToken)
		=> new(options.Value.IsWithinTolerance(value, expected));

	protected override void AppendComparison(StringBuilder stringBuilder)
	{
		stringBuilder.Append("equal to ");
		Formatter.Format(stringBuilder, expected);
		stringBuilder.Append(options);
	}
}
