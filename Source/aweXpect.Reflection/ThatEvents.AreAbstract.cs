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
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are abstract.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreAbstract(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreAbstractConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not abstract.
	/// </summary>
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreNotAbstract(
		this IThat<IEnumerable<EventInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotAbstractConstraint(it, grammars)),
			subject);

	private sealed class AreAbstractConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<EventInfo?>>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(@event => @event.IsReallyAbstract()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all abstract");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-abstract events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => !@event.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all abstract");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained abstract events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => @event.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotAbstractConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<EventInfo?>>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(@event => !@event.IsReallyAbstract()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not abstract");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained abstract events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => @event.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an abstract event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-abstract events ");
			Formatter.Format(stringBuilder, Actual?.Where(@event => !@event.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}
	}
}
