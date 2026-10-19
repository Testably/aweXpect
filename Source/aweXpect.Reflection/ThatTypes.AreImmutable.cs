using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreImmutable(
		this IThat<IEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>(static (it, grammars)
				=> new AreImmutableConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreImmutable(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>(static (it, grammars)
				=> new AreImmutableConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreNotImmutable(
		this IThat<IEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>(static (it, grammars)
				=> new AreNotImmutableConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreNotImmutable(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>(static (it, grammars)
				=> new AreNotImmutableConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreImmutableConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => type.IsImmutable(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type.IsImmutable());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all immutable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained mutable types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all immutable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained immutable types");
	}

	private sealed class AreNotImmutableConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => !type.IsImmutable(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => !type.IsImmutable());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not immutable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained immutable types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an immutable type");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained mutable types");
	}
}
