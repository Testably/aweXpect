using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<IEnumerable<Type?>> DependOn(
		this IThat<IEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new DependOnConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>> DependOn(
		this IThat<IAsyncEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new DependOnConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> do not depend on (do not reference
	///     in their signature) any type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<IEnumerable<Type?>> DoNotDependOn(
		this IThat<IEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new DoNotDependOnConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> do not depend on (do not reference
	///     in their signature) any type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new DoNotDependOnConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	private static bool DependsOnNamespace(Type? type, NamespaceDependencyOptions options)
		=> type is not null && options.IsMatchedBy(type);

	// A null item's dependencies cannot be verified, so it fails the negative assertion just like the
	// positive one (and like DependOnlyOn), instead of slipping through as "does not depend".
	private static bool DoesNotDependOnNamespace(Type? type, NamespaceDependencyOptions options)
		=> type is not null && !options.IsMatchedBy(type);

	private sealed class DependOnConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => DependsOnNamespace(type, options));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => DependsOnNamespace(type, options));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all depend on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types without the dependency ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types with the dependency ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class DoNotDependOnConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => DoesNotDependOnNamespace(type, options));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => DoesNotDependOnNamespace(type, options));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not depend on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types with the dependency ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all do not depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types without the dependency ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
