using System.Linq;
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
	///     Verifies that the <see cref="Assembly" /> has a dependency on the <paramref name="expected" /> assembly.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> DependsOn(
		this IThat<Assembly?> subject, string expected)
	{
		return new StringEqualityTypeResult<Assembly, IThat<Assembly?>>(subject.Get().ExpectationBuilder
				.AddConstraint(expected,
					static (expected, it, grammars)
					=> new DependsOnConstraint(it, grammars, expected, "expected"),
					out DependsOnConstraint constraint),
			subject,
			constraint);
	}

	private sealed class DependsOnConstraint(
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

		public async ValueTask<ConstraintResult> IsMetBy(Assembly? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			StringEqualityOptions evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			_evaluationOptions = evaluationOptions;
			Outcome = actual is not null &&
			          await actual.GetReferencedAssemblyNames()
				          .AnyAsync(dep => evaluationOptions.AreConsideredEqual(dep.Name, expected, parameterName))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a dependency on assembly ").Append(CurrentOptions.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has no dependency on assembly ")
				.Append(CurrentOptions.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Outcome == Outcome.Failure && Actual is not null)
			{
				contexts.AddDependenciesContext("Dependencies",
					Actual.GetReferencedAssemblyNames().Select(dep => dep.Name).ToArray());
			}

			(_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);
		}
	}
}
