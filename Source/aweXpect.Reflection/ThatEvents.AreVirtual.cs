using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are virtual.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreVirtual(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new AreVirtualConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are virtual.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>> AreVirtual(
		this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new AreVirtualConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not virtual.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreNotVirtual(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new AreNotVirtualConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not virtual.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>> AreNotVirtual(
		this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new AreNotVirtualConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreVirtualConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => @event.IsReallyVirtual());
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => @event.IsReallyVirtual());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all virtual");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-virtual events ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all virtual");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained virtual events ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotVirtualConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => !@event.IsReallyVirtual());
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => !@event.IsReallyVirtual());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not virtual");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained virtual events ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a virtual event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-virtual events ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
