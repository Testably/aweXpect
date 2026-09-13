using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Nodes;

#pragma warning disable S1694 // https://rules.sonarsource.com/csharp/RSPEC-1694
internal abstract class Node
{
	/// <summary>
	///     Add a constraint to the current node.
	/// </summary>
	public abstract void AddConstraint(IConstraint constraint);

	/// <summary>
	///     Add a mapping constraint which maps the value according to the <paramref name="memberAccessor" /> to a member
	///     and applies this value to the inner expectations.
	/// </summary>
	public abstract Node AddMapping<TValue, TTarget>(MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null);

	/// <summary>
	///     Add a mapping constraint which maps the value according to the <paramref name="memberAccessor" /> to a member
	///     and applies it to the inner expectations, which are typed at <typeparamref name="TNarrowed" />.
	/// </summary>
	public abstract Node AddNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null);

	/// <summary>
	///     Add a mapping constraint which maps the value according to the <paramref name="memberAccessor" /> asynchronously
	///     to a member and applies this value to the inner expectations.
	/// </summary>
	public abstract Node AddAsyncMapping<TValue, TTarget>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null);

	/// <summary>
	///     Add a mapping constraint which maps the value according to the <paramref name="memberAccessor" /> asynchronously
	///     to a member and applies it to the inner expectations, which are typed at <typeparamref name="TNarrowed" />.
	/// </summary>
	public abstract Node AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null);

	/// <summary>
	///     Add a node as inner node.
	/// </summary>
	public abstract void AddNode(Node node, string? separator = null);

	/// <summary>
	///     Verifies, if the <paramref name="value" /> satisfies the expectations of the node.
	/// </summary>
	public abstract Task<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken);

	/// <summary>
	///     Set the <paramref name="becauseReason" /> on the current node.
	/// </summary>
	public abstract void SetReason(IBecauseReason becauseReason);

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendExpectation(StringBuilder stringBuilder, string? indentation = null);
}
#pragma warning restore S1694
