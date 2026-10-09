using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are static.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> AreStatic(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<MethodInfo?>>((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are static.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> AreStatic(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<MethodInfo?>>((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are not static.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> AreNotStatic(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<MethodInfo?>>((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are not static.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> AreNotStatic(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<MethodInfo?>>((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<MethodInfo?>(it, grammars),
			IContextConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, method => method?.IsStatic == true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, method => method?.IsStatic == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained non-static methods");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained static methods");
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<MethodInfo?>(it, grammars),
			IContextConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, method => method?.IsStatic == false, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, method => method?.IsStatic == false);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained static methods");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a static method");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained non-static methods");
	}
}
