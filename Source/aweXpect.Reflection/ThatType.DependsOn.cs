using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> depends on (references in its signature) at least one type in one of
	///     the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<Type?> DependsOn(
		this IThat<Type?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnNamespaceConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> depends on (references in its signature) the type <typeparamref name="T" />.
	/// </summary>
	public static TypeDependencyResult<Type?> DependsOn<T>(
		this IThat<Type?> subject)
		=> subject.DependsOn(typeof(T));

	/// <summary>
	///     Verifies that the <see cref="Type" /> depends on (references in its signature) the <paramref name="type" />.
	/// </summary>
	public static TypeDependencyResult<Type?> DependsOn(
		this IThat<Type?> subject, Type type)
	{
		TypeDependencyOptions options = new(type);
		return new TypeDependencyResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnTypeConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> depends on (references in its signature) at least one type in the
	///     filtered collections of types <paramref name="target" /> or <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	public static TypeSetDependencyResult<Type?> DependsOn(
		this IThat<Type?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not depend on (does not reference in its signature) any type in
	///     one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<Type?> DoesNotDependOn(
		this IThat<Type?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnNamespaceConstraint(it, grammars, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not depend on (does not reference in its signature) the type
	///     <typeparamref name="T" />.
	/// </summary>
	public static TypeDependencyResult<Type?> DoesNotDependOn<T>(
		this IThat<Type?> subject)
		=> subject.DoesNotDependOn(typeof(T));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not depend on (does not reference in its signature) the
	///     <paramref name="type" />.
	/// </summary>
	public static TypeDependencyResult<Type?> DoesNotDependOn(
		this IThat<Type?> subject, Type type)
	{
		TypeDependencyOptions options = new(type);
		return new TypeDependencyResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnTypeConstraint(it, grammars, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not depend on (does not reference in its signature) any type in
	///     the filtered collections of types <paramref name="target" /> or <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	public static TypeSetDependencyResult<Type?> DoesNotDependOn(
		this IThat<Type?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnTypeSetConstraint(it, grammars, options).Invert()),
			subject,
			options);
	}

	private sealed class DependsOnNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		private Type[] _dependencies = [];

		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_dependencies = actual.ResolveDependencies();
			Outcome = _dependencies.Any(dependency => options.Matches(dependency.Namespace))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		// The sorted namespace lists are only needed for failure messages, so they are built lazily here
		// instead of on every (typically succeeding) evaluation.
		private static string[] ToSortedNamespaces(IEnumerable<Type> dependencies)
			=> dependencies
				.Select(dependency => dependency.Namespace ?? TypeHelpers.GlobalNamespaceDisplay)
				.Distinct(StringComparer.Ordinal)
				.OrderBy(@namespace => @namespace, StringComparer.Ordinal)
				.ToArray();

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("depends on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" depended on ");
			Formatter.Format(stringBuilder, ToSortedNamespaces(_dependencies));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" depended on ");
			Formatter.Format(stringBuilder,
				ToSortedNamespaces(_dependencies.Where(dependency => options.Matches(dependency.Namespace))));
		}
	}

	private sealed class DependsOnTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeDependencyOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		private Type[] _dependencies = [];

		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_dependencies = actual.ResolveDependencies();
			Outcome = _dependencies.Any(options.Matches) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("depends on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// The sorted matching types are only needed for this failure message, so they are built lazily.
			stringBuilder.Append(It).Append(" depended on ");
			Formatter.Format(stringBuilder, _dependencies
				.Where(options.Matches)
				.Distinct()
				.OrderBy(type => type.FullName ?? type.Name, StringComparer.Ordinal)
				.ToArray());
		}
	}

	private sealed class DependsOnTypeSetConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeSetDependencyOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncConstraint<Type?>
	{
		private Type[] _dependencies = [];
		private ResolvedTypeSet? _targetSet;

		public async Task<ConstraintResult> IsMetBy(Type? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ResolvedTypeSet targetSet = await options.Resolve(cancellationToken);
			_targetSet = targetSet;
			_dependencies = actual.ResolveDependencies();
			Outcome = _dependencies.Any(targetSet.Matches) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("depends on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// The sorted matching types are only needed for this failure message, so they are built lazily.
			// _targetSet can only be null when the subject was null, in which case _dependencies is empty.
			stringBuilder.Append(It).Append(" depended on ");
			Formatter.Format(stringBuilder, _dependencies
				.Where(dependency => _targetSet?.Matches(dependency) == true)
				.Distinct()
				.OrderBy(type => type.FullName ?? type.Name, StringComparer.Ordinal)
				.ToArray());
		}
	}
}
