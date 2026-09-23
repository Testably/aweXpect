using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class Is
	{
		public sealed class WhoseTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotMatch_ShouldSucceed()
			{
				object subject = new MyClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<MyClass>()
						.Whose(it => it.Value, value => value.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.MyClass whose Value is less than 42,
					             but Value was 42
					             """);
			}

			[Fact]
			public async Task WhenPropertyMatches_ShouldSucceed()
			{
				object subject = new MyClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<MyClass>().Whose(it => it.Value, value => value.IsEqualTo(42));

				await That(Act).DoesNotThrow();
			}

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
					=> await That(subject).Is<Outer>()
						.Whose(o => o.Item, it => it.Is<Derived>()
							.Whose(d => d.Name, it => it.IsEqualTo("foo")));

				await That(Act).DoesNotThrow();
			}
			[Fact]
			public async Task Whose_AllowsNestedIs_FailsWhenInnerTypeMismatches()
			{
				Outer subject = new()
				{
					Item = new OtherDerived(),
				};

				async Task Act()
					=> await That(subject).Is<Outer>()
						.Whose(o => o.Item, it => it.Is<Derived>());

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenAsyncMemberDoesNotMatch_ShouldFail()
			{
				object subject = new AsyncClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.GetValueAsync(), value => value.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose GetValueAsync() is less than 42,
					             but GetValueAsync() was 42
					             """);
			}

			[Fact]
			public async Task WhenAsyncMemberMatches_ShouldSucceed()
			{
				object subject = new AsyncClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.GetValueAsync(), value => value.IsEqualTo(42));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAsyncMemberIsCombinedWithAndWhose_ShouldVerifyBoth()
			{
				object subject = new AsyncClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.Value, value => value.IsEqualTo(42))
						.AndWhose(it => it.GetValueAsync(), value => value.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose Value is equal to 42 and whose GetValueAsync() is less than 42,
					             but GetValueAsync() was 42
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberDoesNotMatch_ShouldFail()
			{
				object subject = new AsyncClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.GetValueAsValueTaskAsync(), value => value.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose GetValueAsValueTaskAsync() is less than 42,
					             but GetValueAsValueTaskAsync() was 42
					             """);
			}

			[Fact]
			public async Task WhenValueTaskMemberIsCombinedWithAndWhose_ShouldVerifyBoth()
			{
				object subject = new AsyncClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.Value, value => value.IsEqualTo(42))
						.AndWhose(it => it.GetValueAsValueTaskAsync(), value => value.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose Value is equal to 42 and whose GetValueAsValueTaskAsync() is less than 42,
					             but GetValueAsValueTaskAsync() was 42
					             """);
			}
#endif

			[Fact]
			public async Task WhenAsyncMemberFaults_ShouldFail()
			{
				object subject = new AsyncClass();

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.FaultedAsync(), value => value.IsEqualTo(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose FaultedAsync() is equal to 42,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			[Fact]
			public async Task WhenAsyncMemberInAndWhoseFaults_ShouldFail()
			{
				object subject = new AsyncClass();

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.Value, value => value.IsEqualTo(0))
						.AndWhose(it => it.FaultedAsync(), value => value.IsEqualTo(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose Value is equal to 0 and whose FaultedAsync() is equal to 42,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenValueTaskMemberFaults_ShouldFail()
			{
				object subject = new AsyncClass();

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.FaultedValueTaskAsync(), value => value.IsEqualTo(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose FaultedValueTaskAsync() is equal to 42,
					             but FaultedValueTaskAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}
#endif

			[Fact]
			public async Task WhenAsyncMemberFaults_AndMemberExpectationThrowsOnDefault_ShouldFail()
			{
				object subject = new AsyncClass();

				async Task Act()
					=> await That(subject).Is<AsyncClass>()
						.Whose(it => it.FaultedAsync(), value => value.Satisfies(x => 10 / x > 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.WhoseTests.AsyncClass whose FaultedAsync() satisfies x => 10 / x > 1,
					             but FaultedAsync() did throw an InvalidOperationException:
					               async member failed
					             """)
					.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
			}

			private sealed class AsyncClass
			{
				public int Value { get; set; }

#pragma warning disable CA1822 // the tests access these members through the subject
				public async Task<int> FaultedAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}

#if NET8_0_OR_GREATER
				public async ValueTask<int> FaultedValueTaskAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}
#endif
#pragma warning restore CA1822

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
		}

		public sealed class AndWhoseTests
		{
			[Fact]
			public async Task AndWhose_AllowsNestedIs()
			{
				Outer subject = new()
				{
					Item = new Derived
					{
						Name = "foo",
					},
					Other = new Derived
					{
						Name = "bar",
					},
				};

				async Task Act()
					=> await That(subject).Is<Outer>()
						.Whose(o => o.Item, it => it.Is<Derived>().Whose(d => d.Name, it => it.IsEqualTo("foo")))
						.AndWhose(o => o.Other, it => it.Is<Derived>().Whose(d => d.Name, it => it.IsEqualTo("bar")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AndWhose_WhenBothMembersAreCollections_ShouldVerifyEachMember()
			{
				TwoCollections subject = new()
				{
					First = [1, 2,],
					Second = [1, 3,],
				};

				async Task Act()
					=> await That(subject).Is<TwoCollections>()
						.Whose(o => o.First, it => it.IsEqualTo([1, 2,]))
						.AndWhose(o => o.Second, it => it.IsEqualTo([1, 2,]));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.Is.AndWhoseTests.TwoCollections whose First is equal to collection [1, 2,] in order and whose Second is equal to collection [1, 2,] in order,
					             but Second
					               contained item 3 at index 1 instead of 2 and
					               lacked 1 of 2 expected items: 2
					             *
					             """).AsWildcard();
			}

			private sealed class TwoCollections
			{
				public List<int> First { get; set; } = [];
				public List<int> Second { get; set; } = [];
			}
		}
	}
}
