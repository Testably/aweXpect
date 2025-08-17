using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are sealed.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreSealed(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreSealedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not sealed.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreNotSealed(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotSealedConstraint(it, grammars)),
			subject);

	private sealed class AreSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<EventInfo?>>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(@event => @event.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-sealed events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => !@event.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all sealed");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained sealed events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => @event.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<EventInfo?>>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(@event => !@event.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained sealed events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => @event.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a sealed event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-sealed events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => !@event.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}
}
