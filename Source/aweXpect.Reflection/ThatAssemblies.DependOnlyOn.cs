using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

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
		DependOnlyOnExpectation expectation = new(expected);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …have dependencies only on the <paramref name="expected" /> assemblies.
	/// </summary>
	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Assembly?}}, string[])" path="/remarks" />
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DependOnlyOn(
		this IEnumerableItems<Assembly?> items, params string[] expected)
		=> items.DependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Assembly?}, string[])" />
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DependOnlyOn(
		this IEnumerableItems<Assembly?> items, IEnumerable<string> expected)
	{
		DependOnlyOnExpectation expectation = new(expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Assembly?}, string[])" />
	public static StringEqualityTypeResult<TCollection, IThat<TCollection>> DependOnlyOn<TCollection>(
		this IStructEnumerableItems<TCollection, Assembly?> items, params string[] expected)
		where TCollection : struct, IEnumerable<Assembly?>
		=> items.DependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Assembly?}, string[])" />
	public static StringEqualityTypeResult<TCollection, IThat<TCollection>> DependOnlyOn<TCollection>(
		this IStructEnumerableItems<TCollection, Assembly?> items, IEnumerable<string> expected)
		where TCollection : struct, IEnumerable<Assembly?>
	{
		DependOnlyOnExpectation expectation = new(expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Assembly?}}, string[])" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, params string[] expected)
		=> subject.DependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Assembly?}}, string[])" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, IEnumerable<string> expected)
	{
		DependOnlyOnExpectation expectation = new(expected);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Assembly?}, string[])" />
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IAsyncEnumerableItems<Assembly?> items, params string[] expected)
		=> items.DependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Assembly?}, string[])" />
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IAsyncEnumerableItems<Assembly?> items, IEnumerable<string> expected)
	{
		DependOnlyOnExpectation expectation = new(expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	private sealed class DependOnlyOnExpectation : ItemExpectation<Assembly>
	{
		private readonly string[] _allowed;
		private readonly Dictionary<Assembly, string?[]> _disallowedDependencies = new();
		private StringEqualityOptions? _evaluationOptions;

		public DependOnlyOnExpectation(IEnumerable<string> expected)
		{
			ThrowHelper.ThrowIfNull(expected, nameof(expected));
			_allowed = expected.ToArray();
		}

		public OptionsHolder<StringEqualityOptions> Options { get; } = new(StringEqualityOptions.Default);

		private StringEqualityOptions EvaluationOptions => _evaluationOptions ?? Options.Value;

		public override void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
		{
			_evaluationOptions = Options.Value.ForEvaluation(context, cancellationToken);
			_disallowedDependencies.Clear();
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> EvaluationOptions.AppendContexts(contexts);

		public override async ValueTask<PredicateResult> IsMatch(Assembly item, CancellationToken cancellationToken)
		{
			string?[] violations = await item.GetDisallowedAssemblyDependencies(_allowed, EvaluationOptions);
			if (violations.Length > 0)
			{
				_disallowedDependencies[item] = violations;
			}

			return violations.Length == 0;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			stringBuilder.Append(grammars.IsPlural() ? "have dependencies only on " : "has dependencies only on ");
			if (_allowed.Length == 0)
			{
				stringBuilder.Append("no assemblies");
				return;
			}

			stringBuilder.Append("assemblies ").Append(string.Join(" or ",
				_allowed.Select(expected => Options.Value.GetExpectation(expected, ExpectationGrammars.None))));
		}

		public override string FormatItems(IEnumerable<Assembly?> items, FormattingOptions options)
			=> DependencyViolationRenderer.FormatItemsWithDisallowedDependencies(items.ToList(),
				_disallowedDependencies, options.TotalItemCount);
	}
}
