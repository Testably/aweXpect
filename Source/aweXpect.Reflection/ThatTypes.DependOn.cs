using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IThat<IEnumerable<Type?>?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<NamespaceDependencyOptions, IEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DependOnConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<NamespaceDependencyOptions, IAsyncEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DependOnConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<TypeSetDependencyOptions, IEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DependOnTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<TypeSetDependencyOptions, IAsyncEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DependOnTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> do not depend on (do not reference
	///     in their signature) any type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn(
		this IThat<IEnumerable<Type?>?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<NamespaceDependencyOptions, IEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DoNotDependOnConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> do not depend on (do not reference
	///     in their signature) any type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<NamespaceDependencyOptions, IAsyncEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DoNotDependOnConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> do not depend on (do not
	///     reference in their signature) any type in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<TypeSetDependencyOptions, IEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DoNotDependOnTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> do not depend on (do not
	///     reference in their signature) any type in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(subject.Get().ExpectationBuilder
				.AddConstraint<TypeSetDependencyOptions, IAsyncEnumerable<Type?>>(options,
					static (options, it, grammars)
					=> new DoNotDependOnTypeSetConstraint(it, grammars, options)),
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

	private static bool DependsOnTypeSet(Type? type, ResolvedTypeSet targetSet)
		=> type is not null && targetSet.IsMatchedBy(type);

	// A null item's dependencies cannot be verified, so it fails the negative assertion just like the
	// positive one (and like DependOnlyOn), instead of slipping through as "does not depend".
	private static bool DoesNotDependOnTypeSet(Type? type, ResolvedTypeSet targetSet)
		=> type is not null && !targetSet.IsMatchedBy(type);

	private sealed class DependOnConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => DependsOnNamespace(type, options), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => DependsOnNamespace(type, options));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all depend on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained types without the dependency");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained types with the dependency");
	}

	private sealed class DoNotDependOnConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => DoesNotDependOnNamespace(type, options), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => DoesNotDependOnNamespace(type, options));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not depend on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained types with the dependency");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all do not depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained types without the dependency");
	}

	private sealed class DependOnTypeSetConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeSetDependencyOptions options)
		: CollectionConstraintResult<Type?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			ResolvedTypeSet targetSet = await options.Resolve(cancellationToken);
			return await SetAsyncValue(actual, context, type => DependsOnTypeSet(type, targetSet), cancellationToken);
		}
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			ResolvedTypeSet targetSet = await options.Resolve(cancellationToken);
			return SetValue(actual, context, type => DependsOnTypeSet(type, targetSet));
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all depend on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained types without the dependency");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained types with the dependency");
	}

	private sealed class DoNotDependOnTypeSetConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeSetDependencyOptions options)
		: CollectionConstraintResult<Type?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			ResolvedTypeSet targetSet = await options.Resolve(cancellationToken);
			return await SetAsyncValue(actual, context,
				type => DoesNotDependOnTypeSet(type, targetSet), cancellationToken);
		}
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			ResolvedTypeSet targetSet = await options.Resolve(cancellationToken);
			return SetValue(actual, context, type => DoesNotDependOnTypeSet(type, targetSet));
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not depend on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained types with the dependency");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all do not depend on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained types without the dependency");
	}
}
