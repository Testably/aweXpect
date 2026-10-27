using System;
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

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have dependencies only on
	///     the <paramref name="expected" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DependOnlyOn(
		this IThat<IEnumerable<Assembly?>?> subject, params string[] expected)
		=> subject.DependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Assembly?}}, string[])" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DependOnlyOn(
		this IThat<IEnumerable<Assembly?>?> subject, IEnumerable<string> expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint(expected.ToArray(),
					static (allowed, it, grammars)
					=> new DependOnlyOnConstraint(it, grammars, allowed),
					out DependOnlyOnConstraint constraint),
			subject,
			constraint);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have dependencies only on
	///     the <paramref name="expected" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, params string[] expected)
		=> subject.DependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IThat{IAsyncEnumerable{Assembly?}}, string[])" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, IEnumerable<string> expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint(expected.ToArray(),
					static (allowed, it, grammars)
					=> new DependOnlyOnConstraint(it, grammars, allowed),
					out DependOnlyOnConstraint constraint),
			subject,
			constraint);
	}
#endif

	private sealed class DependOnlyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] allowed)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>,
			IOptionsHolderSource<StringEqualityOptions>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
		private OptionsHolder<StringEqualityOptions>? _optionsHolder;

		private StringEqualityOptions CurrentOptions
			=> _optionsHolder is null ? StringEqualityOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default);

		private StringEqualityOptions? _evaluationOptions;

		private StringEqualityOptions EvaluationOptions => _evaluationOptions ?? CurrentOptions;

		private readonly Dictionary<Assembly, string?[]> _disallowedDependencies = new();

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			return await SetAsyncValue(actual, context, DependsOnlyOnAllowed, cancellationToken);
		}
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			return await SetValue(actual, context, DependsOnlyOnAllowed, cancellationToken);
		}

		private async ValueTask<bool> DependsOnlyOnAllowed(Assembly? assembly)
		{
			if (assembly is null)
			{
				return false;
			}

			string?[] violations = await assembly.GetDisallowedAssemblyDependencies(allowed, EvaluationOptions);
			if (violations.Length > 0)
			{
				_disallowedDependencies[assembly] = violations;
			}

			return violations.Length == 0;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			base.AppendContexts(contexts);
			EvaluationOptions.AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained assemblies with disallowed dependencies");

		protected override Func<string?> FormatItems(Assembly?[] items)
			=> () => DependencyViolationRenderer.FormatItemsWithDisallowedDependencies(items, _disallowedDependencies);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained assemblies depending only on the allowed assemblies");

		private string DescribeAllowed()
			=> allowed.Length == 0
				? "no assemblies"
				: $"assemblies {string.Join(" or ", allowed.Select(expected => CurrentOptions.GetExpectation(expected, ExpectationGrammars.None)))}";
	}
}
