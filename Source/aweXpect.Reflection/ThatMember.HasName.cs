using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> has the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TMember, IThat<TMember?>> HasName<TMember>(
		this IThat<TMember?> subject, string expected)
		where TMember : MemberInfo
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<TMember, IThat<TMember?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (s, it, grammars)
				=> new HasNameConstraint<TMember?>(it, grammars, s.Expected, s.Options)),
			subject,
			options);
	}

	private sealed class HasNameConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<TMember>(it, grammars),
			IAsyncContextConstraint<TMember>
		where TMember : MemberInfo?
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(TMember actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(actual?.Name, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options).GetExtendedMemberFailure(It, "name", Grammars,
				Actual?.Name, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
