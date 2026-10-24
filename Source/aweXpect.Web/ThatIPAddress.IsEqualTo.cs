using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IPAddress.Equals(object)" />, so an IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>,
	///     is not equal to the IPv4 address <c>10.0.0.1</c>, unless
	///     <see cref="IgnoringIPv4MappedToIPv6{TSelf}" /> is specified.
	/// </remarks>
	public static ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?> IsEqualTo(
		this IThat<IPAddress?> subject,
		IPAddress? expected)
	{
		ObjectEqualityOptions<IPAddress?> options = new();
		return new ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IPAddress.Equals(object)" />, so an IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>,
	///     is not equal to the IPv4 address <c>10.0.0.1</c>, unless
	///     <see cref="IgnoringIPv4MappedToIPv6{TSelf}" /> is specified.
	/// </remarks>
	public static ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?> IsNotEqualTo(
		this IThat<IPAddress?> subject,
		IPAddress? unexpected)
	{
		ObjectEqualityOptions<IPAddress?> options = new();
		return new ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint(it, grammars, state.Unexpected, state.Options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Maps IPv4-mapped IPv6 addresses, e.g. <c>::ffff:10.0.0.1</c>, to their IPv4 address before comparing,
	///     so that they are equal to the IPv4 address, e.g. <c>10.0.0.1</c>.
	/// </summary>
	/// <remarks>
	///     Like the other options that specify how two values are compared, e.g. <c>Using(comparer)</c>, it cannot be
	///     combined with another one.
	///     <seealso cref="IPAddress.IsIPv4MappedToIPv6" />
	/// </remarks>
	public static TSelf IgnoringIPv4MappedToIPv6<TSelf>(this IObjectEqualityResult<TSelf, IPAddress?> result)
		where TSelf : IObjectEqualityResult<TSelf, IPAddress?>
	{
		result.Options.SetMatchType(IgnoringIPv4MappedToIPv6MatchType.Instance, nameof(IgnoringIPv4MappedToIPv6));
		return (TSelf)result;
	}

	/// <remarks>
	///     Mirrors the equality constraint of aweXpect, which is not public, so that all options of
	///     <see cref="ObjectEqualityOptions{TSubject}" /> keep working.
	/// </remarks>
	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		IPAddress? expected,
		ObjectEqualityOptions<IPAddress?> options)
		: ConstraintResult.WithEqualToValue<IPAddress?>(it, grammars, expected is null),
			IAsyncContextConstraint<IPAddress?>
	{
		private ObjectEqualityOptions<IPAddress?>? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(IPAddress? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(actual, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult, indentation));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IgnoringIPv4MappedToIPv6MatchType : ObjectMatchType
	{
		public static readonly IgnoringIPv4MappedToIPv6MatchType Instance = new();
		private static readonly Matcher SharedMatcher = new();

		public override ObjectMatcher ForEvaluation(MatchEnvironment environment)
			=> SharedMatcher;

		public override string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}{this}";

		public override string PrependItemAndComparison(string expected, string? itemNoun = null,
			string? comparison = null)
			=> (itemNoun is null ? "" : itemNoun + " ") + (comparison is null ? "" : comparison + " ") + expected + this;

		public override string ToString() => " ignoring IPv4-to-IPv6 mapping";

		private static IPAddress? Normalize(IPAddress? address)
			=> address is { IsIPv4MappedToIPv6: true, } ? address.MapToIPv4() : address;

		private sealed class Matcher : ObjectMatcher
		{
			public override ValueTask<MatchResult> Match<TActual, TExpected>(TActual actual, TExpected expected)
				=> new(MatchResult.From(Equals(Normalize(actual as IPAddress), Normalize(expected as IPAddress))));
		}
	}
}
