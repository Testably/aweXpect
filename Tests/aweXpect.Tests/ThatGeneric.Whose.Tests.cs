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
			public async Task WhenAsyncMemberFaults_ShouldPropagateException()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1));

				await That(Act).ThrowsExactly<InvalidOperationException>()
					.WithMessage("async member failed");
			}

			[Fact]
			public async Task WhenAsyncMemberFaults_ShouldPropagateExceptionEvenIfOtherBranchSucceeds()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject)
						.Whose(o => o.FaultedAsync(), v => v.IsEqualTo(1)).Or
						.Whose(o => o.Value, v => v.IsEqualTo(0));

				await That(Act).ThrowsExactly<InvalidOperationException>()
					.WithMessage("async member failed");
			}

			[Fact]
			public async Task WhenAsyncMemberIsCanceled_ShouldPropagateCancellation()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.CanceledAsync(), v => v.IsEqualTo(1));

				await That(Act).Throws<OperationCanceledException>();
			}

			[Fact]
			public async Task WhenAsyncMemberThrowsBeforeReturningTask_ShouldPropagateException()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.ThrowsBeforeReturningTask(), v => v.IsEqualTo(1));

				await That(Act).ThrowsExactly<InvalidOperationException>()
					.WithMessage("thrown before returning the task");
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberFaults_ShouldPropagateException()
			{
				ThrowingClass subject = new();

				async Task Act()
					=> await That(subject).Whose(o => o.FaultedValueTaskAsync(), v => v.IsEqualTo(1));

				await That(Act).ThrowsExactly<InvalidOperationException>()
					.WithMessage("async member failed");
			}
#endif

			private sealed class ThrowingClass
			{
				public int Value { get; set; }

				public Task<int> CanceledAsync()
					=> Task.FromCanceled<int>(new CancellationToken(true));

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
