using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public class NarrowingMappingNodeTests
{
	[Fact]
	public async Task Async_IsMetBy_WhenMemberIsNarrowedType_ShouldUseInnerConstraint()
	{
		string? receivedValue = null;
		NarrowingAsyncMappingNode<object, object?, string> node = new(
			MemberAccessor<object, Task<object?>>.FromFunc(v => Task.FromResult<object?>(v), " which "));
		node.AddConstraint(new DummyValueConstraint<string>(v =>
		{
			receivedValue = v;
			return new DummyConstraintResult(Outcome.Success);
		}));

		ConstraintResult result = await node.IsMetBy<object>("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(receivedValue).IsEqualTo("foo");
	}

	[Fact]
	public async Task Async_IsMetBy_WhenMemberIsOfAnotherType_ShouldNotUseInnerConstraint()
	{
		NarrowingAsyncMappingNode<object, object?, string> node = new(
			MemberAccessor<object, Task<object?>>.FromFunc(v => Task.FromResult<object?>(v), " which "));
		node.AddConstraint(new NarrowedStringConstraint("is a string"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<object>(42, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Undecided);
		await That(sb.ToString()).IsEqualTo("is a string");
		await That(result.GetResultText()).IsEmpty();
	}

	[Fact]
	public async Task IsMetBy_WhenMemberIsNarrowedType_ShouldUseInnerConstraint()
	{
		string? receivedValue = null;
		NarrowingMappingNode<object, object?, string> node = new(
			MemberAccessor<object, object?>.FromFunc(v => v, " which "));
		node.AddConstraint(new DummyValueConstraint<string>(v =>
		{
			receivedValue = v;
			return new DummyConstraintResult(Outcome.Success);
		}));

		ConstraintResult result = await node.IsMetBy<object>("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(receivedValue).IsEqualTo("foo");
	}

	[Fact]
	public async Task IsMetBy_WhenMemberIsNull_ShouldUseInnerConstraint()
	{
		bool wasCalled = false;
		NarrowingMappingNode<object, object?, string> node = new(
			MemberAccessor<object, object?>.FromFunc(_ => null, " which "));
		node.AddConstraint(new DummyValueConstraint<string?>(_ =>
		{
			wasCalled = true;
			return new DummyConstraintResult(Outcome.Success);
		}));

		ConstraintResult result = await node.IsMetBy<object>("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(wasCalled).IsTrue();
	}

	[Fact]
	public async Task IsMetBy_WhenMemberIsOfAnotherType_ShouldNotUseInnerConstraint()
	{
		NarrowingMappingNode<object, object?, string> node = new(
			MemberAccessor<object, object?>.FromFunc(v => v, " which "));
		node.AddConstraint(new NarrowedStringConstraint("is a string"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<object>(42, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Undecided);
		await That(sb.ToString()).IsEqualTo("is a string");
		await That(result.GetResultText()).IsEmpty();
	}

	[Fact]
	public async Task NotApplicableResult_Negate_ShouldStayUndecided()
	{
		NotApplicableConstraintResult result = new(new DummyNode("foo"));

		ConstraintResult negatedResult = result.Negate();

		await That(negatedResult.Outcome).IsEqualTo(Outcome.Undecided);
	}

	[Fact]
	public async Task NotApplicableResult_TryGetValue_ShouldReturnFalse()
	{
		NotApplicableConstraintResult constraintResult = new(new DummyNode("foo"));

		bool result = constraintResult.TryGetValue(out object? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	private sealed class NarrowedStringConstraint(string expectation) : IValueConstraint<string>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(string actual)
			=> new DummyConstraintResult(Outcome.Failure, failureText: "it was applied");

		/// <inheritdoc />
		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectation);
	}
}
