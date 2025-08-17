using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> InheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.InheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit
	///     from <paramref name="baseType" />.
	/// </summary>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> InheritFrom(
		this IThat<IEnumerable<Type?>> subject,
		Type baseType,
		bool forceDirect = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, baseType, forceDirect)),
			subject);

	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> inherit from
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotInheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.DoNotInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> inherit from
	///     <paramref name="baseType" />.
	/// </summary>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotInheritFrom(
		this IThat<IEnumerable<Type?>> subject,
		Type baseType,
		bool forceDirect = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, baseType, forceDirect)),
			subject);

	private sealed class InheritFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		bool forceDirect)
		: ConstraintResult.WithNotNullValue<IEnumerable<Type?>>(it, grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.InheritsFrom(baseType, forceDirect) == true)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
			
			stringBuilder.Append(' ');
			IEnumerable<Type?>? nonMatchingTypes =
				Actual?.Where(type => type?.InheritsFrom(baseType, forceDirect) != true);
			Formatter.Format(stringBuilder, nonMatchingTypes, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
			stringBuilder.Append(' ');
			IEnumerable<Type?>? matchingTypes =
				Actual?.Where(type => type?.InheritsFrom(baseType, forceDirect) == true);
			Formatter.Format(stringBuilder, matchingTypes, FormattingOptions.Indented(indentation));
		}

		private static void AppendDirectlyFrom(StringBuilder stringBuilder, bool forceDirect)
		{
			if (forceDirect)
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append("from ");
		}
	}

	private sealed class DoNotInheritFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		bool forceDirect)
		: ConstraintResult.WithNotNullValue<IEnumerable<Type?>>(it, grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type =>
			{
				return type?.InheritsFrom(baseType, forceDirect) != true;
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all do not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
			
			stringBuilder.Append(' ');
			IEnumerable<Type?>? nonMatchingTypes =
				Actual?.Where(type => type?.InheritsFrom(baseType, forceDirect) == true);
			Formatter.Format(stringBuilder, nonMatchingTypes, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("at least one inherits ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
			
			stringBuilder.Append(' ');
			IEnumerable<Type?>? matchingTypes =
				Actual?.Where(type => type?.InheritsFrom(baseType, forceDirect) != true);
			Formatter.Format(stringBuilder, matchingTypes, FormattingOptions.Indented(indentation));
		}

		private static void AppendDirectlyFrom(StringBuilder stringBuilder, bool forceDirect)
		{
			if (forceDirect)
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append("from ");
		}
	}
}
