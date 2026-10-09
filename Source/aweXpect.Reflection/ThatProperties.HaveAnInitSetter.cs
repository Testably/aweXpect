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

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have an init-only setter.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> HaveAnInitSetter(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new HaveAnInitSetterConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have an init-only setter.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>> HaveAnInitSetter(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new HaveAnInitSetterConstraint(it, grammars)),
			subject);
#endif

	private sealed class HaveAnInitSetterConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(it, grammars),
			IContextConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, property => property.HasInitSetter(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, property => property.HasInitSetter());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have an init setter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained properties without an init setter");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have an init setter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained properties with an init setter");
	}
}
