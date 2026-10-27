using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has dependencies only on the <paramref name="expected" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> DependsOnlyOn(
		this IThat<Assembly?> subject, params string[] expected)
		=> subject.DependsOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependsOnlyOn(IThat{Assembly}, string[])" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> DependsOnlyOn(
		this IThat<Assembly?> subject, IEnumerable<string> expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new StringEqualityTypeResult<Assembly, IThat<Assembly?>>(subject.Get().ExpectationBuilder
				.AddConstraint(expected.ToArray(),
					static (allowed, it, grammars)
					=> new DependsOnlyOnConstraint(it, grammars, allowed),
					out DependsOnlyOnConstraint constraint),
			subject,
			constraint);
	}

	private sealed class DependsOnlyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] allowed)
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
		private string?[] _violations = [];

		public async ValueTask<ConstraintResult> IsMetBy(Assembly? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			_violations = await actual.GetDisallowedAssemblyDependencies(allowed, _evaluationOptions);
			Outcome = _violations.Length == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had disallowed dependencies");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only had dependencies on the allowed assemblies");

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Outcome == Outcome.Failure && !IsNegated)
			{
				contexts.AddDependenciesContext("Disallowed dependencies", _violations);
			}

			(_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);
		}

		private string DescribeAllowed()
			=> allowed.Length == 0
				? "no assemblies"
				: $"assemblies {string.Join(" or ", allowed.Select(expected => CurrentOptions.GetExpectation(expected, ExpectationGrammars.None)))}";
	}
}
