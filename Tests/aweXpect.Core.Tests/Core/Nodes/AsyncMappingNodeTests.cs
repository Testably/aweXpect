using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.Nodes;

public class AsyncMappingNodeTests
{
	[Theory]
	[InlineData(" that ", "whose bar", "foo whose bar")]
	[InlineData(" that ", "is bar", "foo that is bar")]
	[InlineData(" whose value ", "whose bar", "foo whose value whose bar")]
	public async Task AppendExpectation_WhenMemberTextEndsWithThat_ShouldOnlyDropItBeforeWhose(
		string memberText, string memberExpectation, string expectedExpectation)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint<string?>(_ => true, "foo"));
		Node mappingNode = node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "),
			(_, expectation) => expectation.Append(memberText));
		mappingNode.AddNode(new DummyNode(memberExpectation,
			() => new DummyConstraintResult(Outcome.Success, memberExpectation)));
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		await That(sb.ToString()).IsEqualTo(expectedExpectation);
		await That(result.GetExpectationText()).IsEqualTo(expectedExpectation)
			.Because("the result path has to apply the same which/whose rule as the node path");
	}

	[Fact]
	public async Task Equals_IfMemberAccessorsAreDifferent_ShouldBeFalse()
	{
		AsyncMappingNode<string, int> node1 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length1 "));
		AsyncMappingNode<string, int> node2 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length2 "));

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfMemberAccessorsAreSame_ShouldBeTrue()
	{
		AsyncMappingNode<string, int> node1 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		AsyncMappingNode<string, int> node2 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_WhenOtherIsDifferentNode_ShouldBeFalse()
	{
		AsyncMappingNode<string, int> node = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		object other = new AsyncMappingNode<int, int>(
			MemberAccessor<int, Task<int>>.FromFunc(s => Task.FromResult(s * 2), " duplicate "));

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		AsyncMappingNode<string, int> node = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsMetBy_ShouldUseInnerConstraintWithOuterValue()
	{
		AsyncMappingNode<string, int> node =
			new(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Success, v, $"yeah: {v}")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("yeah: 6");
		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.TryGetValue(out string? value)).IsTrue();
		await That(value).IsEqualTo("foobar");
	}

	[Fact]
	public async Task IsMetBy_WithInvalidType_ShouldThrowInvalidOperationException()
	{
		AsyncMappingNode<string, int> node =
			new(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		node.AddConstraint(
			new DummyValueConstraint<int?>(v => new DummyConstraintResult<int?>(Outcome.Success, v, "yeah!")));
		async Task Act() => await node.IsMetBy(42, null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("""
			             The member type for the actual value in the which node did not match.
			             Expected: string,
			                Found: int
			             """);
	}

	[Fact]
	public async Task IsMetBy_WhenAbandonedMemberFaultsLater_ShouldNotRaiseUnobservedTaskException()
	{
		NotSupportedException exception = new("foo");
		bool isRaised = false;
		EventHandler<UnobservedTaskExceptionEventArgs> handler = (_, e) =>
		{
			if (e.Exception.InnerExceptions.Contains(exception))
			{
				isRaised = true;
			}
		};

		TaskScheduler.UnobservedTaskException += handler;
		try
		{
			await AbandonMemberThatFaultsLater(exception);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		finally
		{
			TaskScheduler.UnobservedTaskException -= handler;
		}

		await That(isRaised).IsFalse()
			.Because("the exception of an abandoned member task must be observed, as nobody else awaits it");
	}

	[Fact]
	public async Task IsMetBy_WhenMemberFaults_ShouldFailWithoutEvaluatingMemberConstraints()
	{
		NotSupportedException exception = new("foo");
		AsyncMappingNode<string, int> node =
			new(MemberAccessor<string, Task<int>>.FromFunc(_ => Task.FromException<int>(exception), " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.FailureCause).IsSameAs(exception);
		await That(sb.ToString()).IsEqualTo("yeah!");
	}

	[Fact]
	public async Task IsMetBy_WhenMemberFaults_WhenNegated_ShouldNegateExpectationAndStillFail()
	{
		AsyncMappingNode<string, int> node = new(MemberAccessor<string, Task<int>>.FromFunc(
			_ => Task.FromException<int>(new NotSupportedException("foo")), " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("not yeah!");
	}

	[Fact]
	public async Task IsMetBy_WhenMemberNeverCompletes_ShouldAbortOnCancellation()
	{
		AsyncMappingNode<string, int> node = new(MemberAccessor<string, Task<int>>.FromFunc(
			_ => new TaskCompletionSource<int>().Task, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));
		using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(100));

		Task<ConstraintResult> evaluation = node.IsMetBy("foo", null!, cts.Token);

		await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
		await That(evaluation.IsCompleted).IsTrue();
		Func<Task> awaitEvaluation = async () => await evaluation;
		await That(awaitEvaluation).Throws<OperationCanceledException>();
	}

	[Fact]
	public async Task IsMetBy_WithNullDelegate_ShouldReturnNullFailure()
	{
		DelegateValue<string?> value = new("foo", null, 10.Milliseconds(), true);
		AsyncMappingNode<string?, int?> node =
			new(MemberAccessor<string?, Task<int?>>.FromFunc(s => Task.FromResult(s?.Length), " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int?>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(value, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task IsMetBy_WithNullValue_ShouldReturnNullFailure()
	{
		AsyncMappingNode<string?, int?> node =
			new(MemberAccessor<string?, Task<int?>>.FromFunc(s => Task.FromResult(s?.Length), " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int?>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task IsMetBy_WithNullValue_WhenNegated_ShouldNegateExpectationAndStillFail()
	{
		AsyncMappingNode<string?, int?> node =
			new(MemberAccessor<string?, Task<int?>>.FromFunc(s => Task.FromResult(s?.Length), " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int?>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("not yeah!");
		await That(negated.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task WhenMemberCompletesWithinTheTimeout_ShouldSucceed()
	{
		string subject = "foo";

		async Task Act()
			=> await HasAsyncLength(That(subject),
					s => Task.Delay(50.Milliseconds()).ContinueWith(_ => s!.Length),
					length => length.IsEqualTo(3))
				.WithTimeout(5.Seconds());

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenMemberDoesNotFinishWithinTheTimeout_ShouldFailWithTheTimeout()
	{
		string subject = "foo";

		async Task Act()
			=> await HasAsyncLength(That(subject),
					_ => new TaskCompletionSource<int>().Task,
					length => length.IsEqualTo(3))
				.WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             length is equal to 3,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.WithTimeout(30.Seconds())
			.Because("the timeout must abandon a member task that never finishes, and report it like any other timeout");
	}

	[Fact]
	public async Task WhenMemberThrowsWithinTheTimeout_ShouldFail()
	{
		string subject = "foo";

		async Task Act()
			=> await HasAsyncLength(That(subject),
					_ => Task.Delay(50.Milliseconds())
						.ContinueWith<int>(_ => throw new NotSupportedException("member failed")),
					length => length.IsEqualTo(3))
				.WithTimeout(5.Seconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             length is equal to 3,
			             but length did throw a NotSupportedException:
			               member failed
			             """).And
			.WithInner<NotSupportedException>(inner => inner.HasMessage("member failed"));
	}

	[Fact]
	public async Task WhenNegated_WithValidation_AndFailingMemberExpectation_ShouldSucceed()
	{
		string subject = "foo";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasLength(it, length => length.IsEqualTo(4)));

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenNegated_WithValidation_ShouldNegateValidation()
	{
		string subject = "foo";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasLength(it, length => length.IsEqualTo(3)));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             does not have a length which is equal to 3,
			             but it had
			             """);
	}

	/// <remarks>
	///     The member task is only reachable from within this method, so that it can be collected afterwards.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task AbandonMemberThatFaultsLater(Exception exception)
	{
		TaskCompletionSource<int> tcs = new();
		using CancellationTokenSource safetyNet = new(30.Seconds());
		using CancellationTokenRegistration _ = safetyNet.Token.Register(() => tcs.TrySetException(exception));
		AsyncMappingNode<string, int> node =
			new(MemberAccessor<string, Task<int>>.FromFunc(_ => tcs.Task, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));
		using CancellationTokenSource cts = new(50.Milliseconds());

		async Task Act() => await node.IsMetBy("foo", null!, cts.Token);

		await That(Act).Throws<TaskCanceledException>()
			.WithMessage(new TaskCanceledException().Message);
		tcs.TrySetException(exception);
	}

	private static AndOrResult<string?, IThat<string?>> HasAsyncLength(
		IThat<string?> subject,
		Func<string?, Task<int>> length,
		Action<IThat<int>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForAsyncMember(MemberAccessor<string?, Task<int>>.FromFunc(length, "length "))
				.AddExpectations(e => expectations(new ThatSubject<int>(e))),
			subject);

	private static AndOrResult<string?, IThat<string?>> HasLength(
		IThat<string?> subject,
		Action<IThat<int>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForAsyncMember(
					MemberAccessor<string?, Task<int>>.FromFunc(s => Task.FromResult(s!.Length), " which "),
					replaceIt: false)
				.Validate((it, grammars) => new HasLengthConstraint(it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<int>(e))),
			subject);

	private sealed class HasLengthConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual is null ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a length");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was <null>");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a length");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
