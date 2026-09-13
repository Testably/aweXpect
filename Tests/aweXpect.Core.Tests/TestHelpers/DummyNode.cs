using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;

namespace aweXpect.Core.Tests.TestHelpers;

internal class DummyNode(string name, Func<ConstraintResult>? result = null) : Node
{
	private readonly string _name = name;
	public MemberAccessor? MappingMemberAccessor { get; private set; }
	public string? ReceivedReason { get; private set; }

	public override void AddConstraint(IConstraint constraint)
		=> throw new NotSupportedException();

	public override Node AddMapping<TValue, TTarget>(
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
	{
		MappingMemberAccessor = memberAccessor;
		return this;
	}

	public override Node AddNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
	{
		MappingMemberAccessor = memberAccessor;
		return this;
	}

	public override Node AddAsyncMapping<TValue, TTarget>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
	{
		MappingMemberAccessor = memberAccessor;
		return this;
	}

	public override Node AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
	{
		MappingMemberAccessor = memberAccessor;
		return this;
	}

	public override void AddNode(Node node, string? separator = null)
		=> throw new NotSupportedException();

	public override Task<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
		where TValue : default
		=> result == null ? throw new NotSupportedException() : Task.FromResult(result());

	public override void SetReason(IBecauseReason becauseReason)
		=> ReceivedReason = becauseReason.ToString();

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(_name);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is DummyNode other && Equals(other);

	private bool Equals(DummyNode other) => _name == other._name;

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => _name.GetHashCode();
}
