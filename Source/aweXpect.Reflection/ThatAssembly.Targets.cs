using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> targets the <paramref name="expected" /> framework
	///     (e.g. <c>net8.0</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> Targets(
		this IThat<Assembly?> subject, string expected)
	{
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(expected)));
		return new StringEqualityTypeResult<Assembly, IThat<Assembly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (s, it, grammars)
				=> new TargetsConstraint(it, grammars, s.Expected, s.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Assembly" /> does not target the <paramref name="unexpected" /> framework
	///     (e.g. <c>net8.0</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> DoesNotTarget(
		this IThat<Assembly?> subject, string unexpected)
	{
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(unexpected)));
		return new StringEqualityTypeResult<Assembly, IThat<Assembly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (s, it, grammars)
				=> new TargetsConstraint(it, grammars, s.Unexpected, s.Options).Invert()),
			subject,
			options);
	}

	private sealed class TargetsConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		OptionsSlot<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IAsyncContextConstraint<Assembly?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(Assembly? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(actual?.GetTargetFramework(), expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("targets ").Append(options.Value.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedMemberFailure(It, "target framework",
				Grammars, Actual?.GetTargetFramework(), expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
