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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are attributes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreAttributes(
		this IThat<IEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>(static (it, grammars)
				=> new AreAttributesConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are attributes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreAttributes(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>(static (it, grammars)
				=> new AreAttributesConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not attributes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreNotAttributes(
		this IThat<IEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>(static (it, grammars)
				=> new AreNotAttributesConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not attributes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreNotAttributes(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>(static (it, grammars)
				=> new AreNotAttributesConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreAttributesConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => type.IsAttribute(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type.IsAttribute());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all attributes");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained other types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all attributes");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained attributes");
	}

	private sealed class AreNotAttributesConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => !type.IsAttribute(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => !type.IsAttribute());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not attributes");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained attributes");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an attribute");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained not attributes");
	}
}
