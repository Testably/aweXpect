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

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> HasName(
		this IThat<Assembly?> subject, string expected)
	{
		return new StringEqualityTypeResult<Assembly, IThat<Assembly?>>(subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars)
				=> new HasNameConstraint(it, grammars, expected, "expected"),
			out HasNameConstraint constraint),
			subject,
			constraint);
	}

	private sealed class HasNameConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		string parameterName)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IAsyncContextConstraint<Assembly?>,
			IOptionsHolderSource<StringEqualityOptions>
	{
		private OptionsHolder<StringEqualityOptions>? _optionsHolder;

		private StringEqualityOptions CurrentOptions
			=> _optionsHolder is null ? StringEqualityOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default);

		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(Assembly? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(actual?.GetName().Name, expected, parameterName);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has name ").Append(CurrentOptions.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? CurrentOptions).GetExtendedMemberFailure(It, "name", Grammars,
				Actual?.GetName().Name, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);
	}
}
