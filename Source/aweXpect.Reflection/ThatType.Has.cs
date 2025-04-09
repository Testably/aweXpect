using System;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> Has<TAttribute>(this IThat<Type?> subject, bool inherit = true)
		where TAttribute : Attribute
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint<TAttribute>(it, grammars, inherit)),
			subject);


	/// <summary>
	///     Verifies that the <see cref="Type" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> Has<TAttribute>(
		this IThat<Type?> subject,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint<TAttribute>(it, grammars, inherit, predicate, doNotPopulateThisValue)),
			subject);

	private sealed class HasAttributeConstraint<TAttribute>(
		string it,
		ExpectationGrammars grammars,
		bool inherit,
		Func<TAttribute, bool>? predicate = null,
		string predicateExpression = "")
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
		where TAttribute : Attribute
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.HasAttribute(predicate, inherit) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has ");
			if (!inherit)
			{
				stringBuilder.Append("direct ");
			}

			Formatter.Format(stringBuilder, typeof(TAttribute));
			if (predicate != null)
			{
				stringBuilder.Append(" matching ");
				stringBuilder.Append(predicateExpression);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not in ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has no ");
			if (!inherit)
			{
				stringBuilder.Append("direct ");
			}

			Formatter.Format(stringBuilder, typeof(TAttribute));
			if (predicate != null)
			{
				stringBuilder.Append(" matching ");
				stringBuilder.Append(predicateExpression);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did in ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
