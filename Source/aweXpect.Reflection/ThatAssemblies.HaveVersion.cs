using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     a <see cref="AssemblyName.Version" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveVersion(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(subject.ExpectAll(new HaveVersionExpectation(predicate, doNotPopulateThisValue)), subject);

	/// <summary>
	///     …have a <see cref="AssemblyName.Version" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveVersion(
		this IEnumerableItems<Assembly?> items,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(items.ExpectItems(new HaveVersionExpectation(predicate, doNotPopulateThisValue)), items.Subject);

	/// <inheritdoc cref="HaveVersion(IEnumerableItems{Assembly?}, Func{Version, bool}, string)" />
	public static AndOrResult<TCollection, IThat<TCollection>> HaveVersion<TCollection>(
		this IStructEnumerableItems<TCollection, Assembly?> items,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<Assembly?>
		=> new(items.ExpectItems(new HaveVersionExpectation(predicate, doNotPopulateThisValue)), items.Subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveVersion(IThat{IEnumerable{Assembly?}}, Func{Version, bool}, string)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(subject.ExpectAll(new HaveVersionExpectation(predicate, doNotPopulateThisValue)), subject);

	/// <inheritdoc cref="HaveVersion(IEnumerableItems{Assembly?}, Func{Version, bool}, string)" />
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveVersion(
		this IAsyncEnumerableItems<Assembly?> items,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(items.ExpectItems(new HaveVersionExpectation(predicate, doNotPopulateThisValue)), items.Subject);
#endif

	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of all items in the filtered
	///     collection of <see cref="Assembly" />.
	/// </summary>
	[GuaranteesNotNull]
	public static VersionResult<IEnumerable<Assembly?>> HaveVersion(this IThat<IEnumerable<Assembly?>?> subject)
		=> new(subject, (_, checks) => subject.ExpectAll(new HaveVersionComponentsExpectation(checks)));

	/// <summary>
	///     …have a <see cref="AssemblyName.Version" />, whose individual components the result verifies.
	/// </summary>
	public static VersionResult<IEnumerable<Assembly?>> HaveVersion(this IEnumerableItems<Assembly?> items)
		=> new(items.Subject, (_, checks) => items.ExpectItems(new HaveVersionComponentsExpectation(checks)));

	/// <inheritdoc cref="HaveVersion(IEnumerableItems{Assembly?})" />
	public static VersionResult<TCollection> HaveVersion<TCollection>(
		this IStructEnumerableItems<TCollection, Assembly?> items)
		where TCollection : struct, IEnumerable<Assembly?>
		=> new(items.Subject, (_, checks) => items.ExpectItems(new HaveVersionComponentsExpectation(checks)));

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveVersion(IThat{IEnumerable{Assembly?}})" />
	[GuaranteesNotNull]
	public static VersionResult<IAsyncEnumerable<Assembly?>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>?> subject)
		=> new(subject, (_, checks) => subject.ExpectAll(new HaveVersionComponentsExpectation(checks)));

	/// <inheritdoc cref="HaveVersion(IEnumerableItems{Assembly?})" />
	public static VersionResult<IAsyncEnumerable<Assembly?>> HaveVersion(this IAsyncEnumerableItems<Assembly?> items)
		=> new(items.Subject, (_, checks) => items.ExpectItems(new HaveVersionComponentsExpectation(checks)));
#endif

	private sealed class HaveVersionComponentsExpectation(IReadOnlyOptionsHolder<VersionComponentCheck[]> checks)
		: ItemExpectation<Assembly>
	{
		/// <remarks>
		///     A comparison that cannot be answered, e.g. an ordering against a <see langword="null" /> bound, fails the
		///     expectation and its negation.
		/// </remarks>
		public override ValueTask<PredicateResult> IsMatch(Assembly item, CancellationToken cancellationToken)
		{
			Version? version = item.GetName().Version;
			if (version is null)
			{
				return new ValueTask<PredicateResult>(false);
			}

			bool isMatching = true;
			foreach (VersionComponentCheck check in checks.Value)
			{
				int component = check.Selector(version);
				Outcome outcome = check.Comparison.Evaluate(component);
				if (outcome == Outcome.FailureBothWays)
				{
					return new ValueTask<PredicateResult>(PredicateResult.FailureBothWays(
						$"had {check.Component} {Formatter.Format(component)}"));
				}

				isMatching &= outcome == Outcome.Success;
			}

			return new ValueTask<PredicateResult>(isMatching);
		}

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			stringBuilder.Append(grammars.IsPlural() ? "have " : "has ");
			if (checks.Value.Length == 0)
			{
				stringBuilder.Append("a version");
				return;
			}

			for (int i = 0; i < checks.Value.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(" and ");
				}

				stringBuilder.Append(checks.Value[i].Component).Append(' ');
				if (checks.Value[i].Comparison.IsNegated)
				{
					stringBuilder.Append("not ");
				}

				checks.Value[i].Comparison.AppendExpectation(stringBuilder);
			}
		}
	}

	private sealed class HaveVersionExpectation : ItemExpectation<Assembly>
	{
		private readonly Func<Version, bool> _predicate;
		private readonly string _predicateExpression;

		public HaveVersionExpectation(Func<Version, bool> predicate, string predicateExpression)
		{
			ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
			_predicate = predicate;
			_predicateExpression = predicateExpression.TrimExpression();
		}

		public override ValueTask<PredicateResult> IsMatch(Assembly item, CancellationToken cancellationToken)
			=> new(item.GetName().Version is { } version &&
			       UserCode.Invoke(_predicate, version, "the predicate"));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "have version matching " : "has version matching ")
				.Append(_predicateExpression);
	}
}
