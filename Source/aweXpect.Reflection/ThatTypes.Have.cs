using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> Have<TAttribute>(
		this IThat<IEnumerable<Type?>> subject, bool inherit = true)
		where TAttribute : Attribute
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveAttributeConstraint<TAttribute>(it, grammars, inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> Have<TAttribute>(
		this IThat<IEnumerable<Type?>> subject,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveAttributeConstraint<TAttribute>(it, grammars, inherit, predicate, doNotPopulateThisValue)),
			subject);

	private sealed class HaveAttributeConstraint<TAttribute>(
		string it,
		ExpectationGrammars grammars,
		bool inherit,
		Func<TAttribute, bool>? predicate = null,
		string predicateExpression = "")
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
		where TAttribute : Attribute
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type.HasAttribute(predicate, inherit)) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all have ");
			if (inherit)
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
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.HasAttribute(predicate, inherit)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all have ");
			if (inherit)
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
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.HasAttribute(predicate, inherit)),
				FormattingOptions.Indented(indentation));
		}
	}
}
