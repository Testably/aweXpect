using System.Diagnostics;
using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class Whose
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AllowsNestedIs()
			{
				Outer subject = new()
				{
					Item = new Derived
					{
						Name = "foo",
					},
				};

				async Task Act()
					=> await That(subject).Whose(o => o.Item, it => it.Is<Derived>()
						.Whose(d => d.Name, it => it.IsEqualTo("foo")));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(true, true, true)]
			[InlineData(true, false, false)]
			[InlineData(false, true, false)]
			[InlineData(false, false, false)]
			public async Task AndCombination_ShouldVerifyAllExpectations(bool a, bool b, bool expectSuccess)
			{
				MyCombinationClass subject = new()
				{
					A = a,
					B = b,
				};

				async Task Act()
					=> await That(subject)
						.Whose(o => o.A, v => v.IsTrue()).And
						.Whose(o => o.B, v => v.IsTrue());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              whose A is True and whose B is True,
					              but {(a ? "" : "A was False")}{(!a && !b ? " and " : "")}{(b ? "" : "B was False")}
					              """);
			}

			[Theory]
			[InlineData(true, true, true)]
			[InlineData(true, false, true)]
			[InlineData(false, true, true)]
			[InlineData(false, false, false)]
			public async Task OrCombination_ShouldVerifyAnyExpectations(bool a, bool b, bool expectSuccess)
			{
				MyCombinationClass subject = new()
				{
					A = a,
					B = b,
				};

				async Task Act()
					=> await That(subject)
						.Whose(o => o.A, v => v.IsTrue()).Or
						.Whose(o => o.B, v => v.IsTrue());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             whose A is True or whose B is True,
					             but A was False and B was False
					             """);
			}

			[Fact]
			public async Task WhenExpectationsAreEmpty_ShouldThrowArgumentException()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.Value, _ => { });

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
					.And.WithParamName("expectations");
			}

			[Theory]
			[AutoData]
			public async Task WhenConditionIsNotSatisfied_ShouldFail(int value)
			{
				int expectedValue = value + 1;
				MyClass subject = new()
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject)
						.Whose(o => o.Value, v => v.IsEqualTo(expectedValue));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              whose Value is equal to {Formatter.Format(expectedValue)},
					              but Value was {Formatter.Format(value)} which differs by -1
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenConditionIsSatisfied_ShouldSucceed(int value)
			{
				MyClass subject = new()
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.Value, v => v.IsEqualTo(value));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAsyncMemberConditionIsNotSatisfied_ShouldFail()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.GetValueAsync(), v => v.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose GetValueAsync() is equal to 2,
					             but GetValueAsync() was 1 which differs by -1
					             """);
			}

			[Fact]
			public async Task WhenAsyncMemberConditionIsSatisfied_ShouldSucceed()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.GetValueAsync(), v => v.IsEqualTo(1));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldNotInvokeAsyncMemberSelector()
			{
				MyClass? subject = null;

				async Task Act()
					=> await That(subject).Whose(o => o.GetValueAsync(), v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose GetValueAsync() is equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenAsyncLambdaIsUsed_ShouldVerifyAwaitedValue()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).Whose(async o => await o.GetValueAsync(), v => v.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose async o => await o.GetValueAsync() is equal to 2,
					             but async o => await o.GetValueAsync() was 1 which differs by -1
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberConditionIsNotSatisfied_ShouldFail()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.GetValueAsValueTaskAsync(), v => v.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose GetValueAsValueTaskAsync() is equal to 2,
					             but GetValueAsValueTaskAsync() was 1 which differs by -1
					             """);
			}

			[Fact]
			public async Task WhenValueTaskMemberConditionIsSatisfied_ShouldSucceed()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.GetValueAsValueTaskAsync(), v => v.IsEqualTo(1));

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task WhenAsyncMemberIsChainedAfterDelegateResult_ShouldVerifyAwaitedValue()
			{
				MyClass subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(() => subject)
						.DoesNotThrow()
						.AndWhoseResult.IsNotNull()
						.And.Whose(o => o.GetValueAsync(), v => v.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that () => subject
					             does not throw any exception and its result is not null and whose GetValueAsync() is equal to 2,
					             but GetValueAsync() was 1 which differs by -1
					             """);
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() is equal to 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndOtherBranchFails_ShouldFailWithBothReasons()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1)).Or
						.Whose(o => o.Value, v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() is equal to 1 or whose Value is equal to 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed and Value was 0 which differs by -1
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndOtherBranchSucceeds_ShouldSucceed()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1)).Or
						.Whose(o => o.Value, v => v.IsEqualTo(0));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndExpectationIsNegated_ShouldStillFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() is not equal to 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndNegatedOrWithOtherBranchFailing_ShouldStillFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it
							.Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1)).Or
							.Whose(o => o.Value, v => v.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() is not equal to 1 and whose Value is not equal to 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndNegatedAnd_ShouldStillFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it
							.Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1)).And
							.Whose(o => o.Value, v => v.IsEqualTo(0)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() is not equal to 1 or whose Value is not equal to 0,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed and Value was 0
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberIsCanceled_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.CanceledAsync(), v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose CanceledAsync() is equal to 1,
					             but CanceledAsync() did throw a TaskCanceledException:
					               *
					             """).AsWildcard()
					.And.WithInner<TaskCanceledException>();
			}

			[Fact]
			public async Task WhenEvaluationIsCanceledWhileAccessingMember_ShouldPropagateCancellation()
			{
				using CancellationTokenSource cts = new();
				CancelingClass subject = new(cts);

				async Task Act()
					=> await That(subject).Whose(o => o.CancelAsync(), v => v.IsEqualTo(1))
						.WithCancellation(cts.Token);

				await That(Act).Throws<OperationCanceledException>();
			}

			[Fact]
			public async Task WhenAsyncMemberThrowsBeforeReturningTask_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.ThrowsBeforeReturningTask(), v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose ThrowsBeforeReturningTask() is equal to 1,
					             but ThrowsBeforeReturningTask() did throw an InvalidOperationException:
					               thrown before returning the task
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("thrown before returning the task"));
			}

			[Fact]
			public async Task WhenMemberThrows_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.Throwing, v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Throwing is equal to 1,
					             but Throwing did throw an InvalidOperationException:
					               member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("member failed"));
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberFaults_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedValueTaskAsync(), v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedValueTaskAsync() is equal to 1,
					             but FaultedValueTaskAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}
#endif

			[Fact]
			public async Task WhenMemberThrows_AndMemberExpectationThrowsOnDefault_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.Throwing, v => v.Satisfies(x => 10 / x > 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Throwing satisfies x => 10 / x > 1,
					             but Throwing did throw an InvalidOperationException:
					               member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("member failed"));
			}

			[Fact]
			public async Task WhenStringMemberThrows_AndMemberExpectationThrowsOnDefault_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.Name, n => n.Satisfies(s => s!.Length > 3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Name satisfies s => s!.Length > 3,
					             but Name did throw a NotSupportedException:
					               name failed
					             """)
					.And.WithInner<NotSupportedException>(inner => inner.HasMessage("name failed"));
			}

			[Fact]
			public async Task WhenMemberThrows_AndMemberExpectationThrowsOnDefault_WhenNegated_ShouldStillFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Whose(o => o.Throwing, v => v.Satisfies(x => 10 / x > 1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Throwing does not satisfy x => 10 / x > 1,
					             but Throwing did throw an InvalidOperationException:
					               member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("member failed"));
			}

			[Fact]
			public async Task WhenMemberThrows_AndMemberExpectationIsNegatedComplyWith_ShouldRenderNegatedExpectation()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.Throwing, v => v.DoesNotComplyWith(x => x.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Throwing is not equal to 1,
					             but Throwing did throw an InvalidOperationException:
					               member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("member failed"));
			}

			[Fact]
			public async Task WhenStringMemberThrows_AndMemberExpectationIsNegatedComplyWith_ShouldRenderNegatedExpectation()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.Name, n => n.DoesNotComplyWith(x => x.StartsWith("a")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Name does not start with "a",
					             but Name did throw a NotSupportedException:
					               name failed
					             """)
					.And.WithInner<NotSupportedException>(inner => inner.HasMessage("name failed"));
			}

			[Fact]
			public async Task WhenSubjectIsNull_AndMemberExpectationIsNegatedComplyWith_ShouldRenderNegatedExpectation()
			{
				ThrowingClass? subject = null;

				async Task Act()
					=> await That(subject).Whose(o => o.Value, v => v.DoesNotComplyWith(x => x.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Value is not equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndMemberExpectationThrowsOnDefault_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedAsync(), v => v.Satisfies(x => 10 / x > 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() satisfies x => 10 / x > 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndMemberExpectationThrowsOnDefault_WhenNegated_ShouldStillFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Whose(o => o.FaultedAsync(), v => v.Satisfies(x => 10 / x > 1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() does not satisfy x => 10 / x > 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberFaults_AndMemberExpectationThrowsOnDefault_ShouldFail()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedValueTaskAsync(), v => v.Satisfies(x => 10 / x > 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedValueTaskAsync() satisfies x => 10 / x > 1,
					             but FaultedValueTaskAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}
#endif

			[Fact]
			public async Task WhenSubjectIsNull_AndMemberExpectationThrowsOnDefault_ShouldFail()
			{
				ThrowingClass? subject = null;

				async Task Act()
					=> await That(subject).Whose(o => o.Value, v => v.Satisfies(x => 10 / x > 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Value satisfies x => 10 / x > 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_AndMemberExpectationThrowsOnDefault_WhenNegated_ShouldStillFail()
			{
				ThrowingClass? subject = null;

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Whose(o => o.Value, v => v.Satisfies(x => 10 / x > 1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Value does not satisfy x => 10 / x > 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_AndAsyncMemberExpectationThrowsOnDefault_ShouldFail()
			{
				ThrowingClass? subject = null;

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedAsync(), v => v.Satisfies(x => 10 / x > 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() satisfies x => 10 / x > 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMemberThrows_AndMemberExpectationIsRepeated_ShouldFailWithoutRetrying()
			{
				ThrowingClass subject = new();
				Stopwatch stopwatch = Stopwatch.StartNew();

				async Task Act()
					=> await That(subject).Whose(o => o.Throwing,
						v => v.Satisfies(x => x == 1).Within(TimeSpan.FromSeconds(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Throwing satisfies x => x == 1 within 0:02,
					             but Throwing did throw an InvalidOperationException:
					               member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("member failed"));
				await That(stopwatch.Elapsed).IsLessThan(TimeSpan.FromSeconds(1));
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_AndMemberExpectationIsRepeated_ShouldFailWithoutRetrying()
			{
				ThrowingClass subject = new();
				Stopwatch stopwatch = Stopwatch.StartNew();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedAsync(),
						v => v.Satisfies(x => x == 1).Within(TimeSpan.FromSeconds(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose FaultedAsync() satisfies x => x == 1 within 0:02,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
				await That(stopwatch.Elapsed).IsLessThan(TimeSpan.FromSeconds(1));
			}

			[Fact]
			public async Task WhenAsyncMemberNeverCompletes_ShouldAbortOnCancellation()
			{
				ThrowingClass subject = new();
				using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(300));

				Task evaluation = Evaluate();

				await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
				await That(evaluation.IsCompleted).IsTrue();
				Func<Task> awaitEvaluation = async () => await evaluation;
				await That(awaitEvaluation).Throws<OperationCanceledException>();

				async Task Evaluate()
					=> await That(subject).Whose(o => o.HangAsync(), v => v.IsEqualTo(1))
						.WithCancellation(cts.Token);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberNeverCompletes_ShouldAbortOnCancellation()
			{
				ThrowingClass subject = new();
				using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(300));

				Task evaluation = Evaluate();

				await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
				await That(evaluation.IsCompleted).IsTrue();
				Func<Task> awaitEvaluation = async () => await evaluation;
				await That(awaitEvaluation).Throws<OperationCanceledException>();

				async Task Evaluate()
					=> await That(subject).Whose(o => o.HangValueTaskAsync(), v => v.IsEqualTo(1))
						.WithCancellation(cts.Token);
			}
#endif

			private sealed class CancelingClass(CancellationTokenSource cts)
			{
				public Task<int> CancelAsync()
				{
					cts.Cancel();
					throw new OperationCanceledException(cts.Token);
				}
			}

			private sealed class ThrowingClass
			{
				public int Value { get; set; }

				public int Throwing => throw new InvalidOperationException("member failed");

				public string Name => throw new NotSupportedException("name failed");

				public Task<int> CanceledAsync()
					=> Task.FromCanceled<int>(new CancellationToken(true));

				public Task<int> HangAsync()
					=> new TaskCompletionSource<int>().Task;

#if NET8_0_OR_GREATER
				public ValueTask<int> HangValueTaskAsync()
					=> new(new TaskCompletionSource<int>().Task);
#endif

				public async Task<int> FaultedAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}

				public Task<int> ThrowsBeforeReturningTask()
					=> throw new InvalidOperationException("thrown before returning the task");

#if NET8_0_OR_GREATER
				public async ValueTask<int> FaultedValueTaskAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}
#endif
			}

			private sealed class MyClass
			{
				public int Value { get; set; }

				public async Task<int> GetValueAsync()
				{
					await Task.Yield();
					return Value;
				}

#if NET8_0_OR_GREATER
				public async ValueTask<int> GetValueAsValueTaskAsync()
				{
					await Task.Yield();
					return Value;
				}
#endif
			}

			private sealed class MyCombinationClass
			{
				public bool A { get; set; }
				public bool B { get; set; }
			}
		}
	}
}
