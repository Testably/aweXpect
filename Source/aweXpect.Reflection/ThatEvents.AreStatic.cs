using System.Collections.Generic;
using System.Reflection;
using System.Text;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are static.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreStatic(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are static.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>> AreStatic(
		this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not static.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreNotStatic(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not static.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>> AreNotStatic(
		this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => @event.IsReallyStatic());
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => @event.IsReallyStatic());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-static events ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained static events ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => !@event.IsReallyStatic());
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => !@event.IsReallyStatic());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained static events ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a static event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-static events ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
