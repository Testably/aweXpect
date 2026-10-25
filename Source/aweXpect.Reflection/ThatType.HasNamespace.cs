using System;
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

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> has the <paramref name="expected" /> namespace.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Type, IThat<Type?>> HasNamespace(
		this IThat<Type?> subject, string expected)
	{
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (s, it, grammars)
				=> new HasNamespaceConstraint(it, grammars, s.Expected, "expected", s.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have the <paramref name="unexpected" /> namespace.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="IsNotWithinNamespace(IThat{Type}, string)" />, this compares the whole namespace as a string
	///     with the string options (e.g. <c>AsPrefix()</c> or <c>IgnoringCase()</c>), instead of excluding a namespace
	///     and its sub-namespaces.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Type, IThat<Type?>> DoesNotHaveNamespace(
		this IThat<Type?> subject, string unexpected)
	{
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (s, it, grammars)
				=> new HasNamespaceConstraint(it, grammars, s.Unexpected, "unexpected", s.Options).Invert()),
			subject,
			options);
	}

	private sealed class HasNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		string parameterName,
		OptionsSlot<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncContextConstraint<Type?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(Type? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(actual?.Namespace, expected, parameterName);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has namespace ").Append(options.Value.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedMemberFailure(It, "namespace", Grammars,
				Actual?.Namespace, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
