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
///     <paramref name="expected" /> value within the tolerance.
/// </summary>
internal sealed class IsJsonNumberEqualToConstraint<TNumber>(
	string it,
	ExpectationGrammars grammars,
	JsonValueReader<TNumber> reader,
	TNumber expected)
	: IsJsonValueEqualToConstraint<TNumber>(it, grammars, reader),
		IOptionsHolderSource<NumberTolerance<TNumber>>
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>
#else
	where TNumber : struct, IComparable<TNumber>
#endif
{
	private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

	/// <summary>
	///     Compares within the tolerance in the <paramref name="options" /> holder, which the caller shares with its
	///     result.
	/// </summary>
	public IsJsonNumberEqualToConstraint(string it, ExpectationGrammars grammars, JsonValueReader<TNumber> reader,
		TNumber expected, OptionsHolder<NumberTolerance<TNumber>> options)
		: this(it, grammars, reader, expected)
	{
		_optionsHolder = options;
	}

	private NumberTolerance<TNumber> CurrentOptions
		=> _optionsHolder is null ? JsonValueReaders.CreateNumberTolerance<TNumber>() : _optionsHolder.Value;

	/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
	OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
		=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(JsonValueReaders.CreateNumberTolerance<TNumber>());

	protected override ValueTask<bool> IsMatch(TNumber value, IEvaluationContext context,
		CancellationToken cancellationToken)
		=> new(CurrentOptions.IsWithinTolerance(value, expected));

	protected override void AppendComparison(StringBuilder stringBuilder)
	{
		stringBuilder.Append("equal to ");
		Formatter.Format(stringBuilder, expected);
		stringBuilder.Append(CurrentOptions);
	}
}
