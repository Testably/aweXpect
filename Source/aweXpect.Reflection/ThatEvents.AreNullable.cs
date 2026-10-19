using System.Collections.Generic;
using System.Reflection;
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

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are nullable.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNullable(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>(static (it, grammars)
				=> new AreNullableConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are nullable.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNullable(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>(static (it, grammars)
				=> new AreNullableConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not nullable.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotNullable(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>(static (it, grammars)
				=> new AreNotNullableConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> are not nullable.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNotNullable(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>(static (it, grammars)
				=> new AreNotNullableConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreNullableConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(it, grammars),
			IContextConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, @event => @event.IsNullable(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, @event => @event.IsNullable());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all nullable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained non-nullable events");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all nullable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained nullable events");
	}

	private sealed class AreNotNullableConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<EventInfo?>(it, grammars),
			IContextConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, @event => @event?.IsNullable() == false, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, @event => @event?.IsNullable() == false);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not nullable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained nullable events");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a nullable event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained non-nullable events");
	}
}
