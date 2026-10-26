using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of type <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<EventInfo> IsOfType<THandler>(
		this IThat<EventInfo?> subject)
		=> IsOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of type <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<EventInfo> IsOfType(
		this IThat<EventInfo?> subject, Type handlerType)
	{
		OptionsHolder<TypeFilterOptions> typeFilterOptions = new(TypeFilterOptions.Default.WithType(handlerType, false));
		return new OfTypeResult<EventInfo>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have a handler of type <typeparamref name="THandler" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<EventInfo, IThat<EventInfo?>> IsNotOfType<THandler>(
		this IThat<EventInfo?> subject)
		=> IsNotOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have a handler of type <paramref name="handlerType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<EventInfo, IThat<EventInfo?>> IsNotOfType(
		this IThat<EventInfo?> subject, Type handlerType)
	{
		OptionsHolder<TypeFilterOptions> typeFilterOptions = new(TypeFilterOptions.Default.WithType(handlerType, false));
		return new AndOrResult<EventInfo, IThat<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
	}

	private sealed class IsOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		IReadOnlyOptionsHolder<TypeFilterOptions> typeFilterOptions)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = typeFilterOptions.Value.Matches(actual?.EventHandlerType)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.HasFlag(ExpectationGrammars.Negated) ? "is not " : "is ");
			typeFilterOptions.Value.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was of type ").Append(Formatter.Format(Actual?.EventHandlerType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
