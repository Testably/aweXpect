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
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> override a base class event.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> Override(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new OverrideConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> override a base class event.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>> Override(
		this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new OverrideConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> do not override a base class
	///     event.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> DoNotOverride(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new DoNotOverrideConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> do not override a base class
	///     event.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>> DoNotOverride(
		this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new DoNotOverrideConstraint(it, grammars)),
			subject);
#endif

	private sealed class OverrideConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => @event.IsOverride());
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => @event.IsOverride());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all override a base event");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained events which do not override a base event ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("do not all override a base event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained events which override a base event ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class DoNotOverrideConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => !@event.IsOverride());
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => !@event.IsOverride());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not override a base event");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained events which override a base event ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an event which overrides a base event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained events which do not override a base event ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
