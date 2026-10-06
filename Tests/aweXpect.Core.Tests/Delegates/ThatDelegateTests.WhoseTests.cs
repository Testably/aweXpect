using System.Collections.ObjectModel;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WhoseTests
	{
		[Test]
		public async Task Throws_Whose_WhenAsyncMemberAccessorIsNull_ShouldThrowArgumentNullException()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose((Func<AsyncException, Task<int>>)null!, v => v.IsEqualTo(1));

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("memberAccessor").And
				.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Throws_Whose_WhenAsyncMemberFaults_ShouldFail()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.FaultedAsync(), v => v.IsEqualTo(1));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose FaultedAsync() is equal to 1,
				             but FaultedAsync() did throw an InvalidOperationException:
				               async member failed for 1
				             """)
				.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed for 1"));
		}

		[Test]
		public async Task Throws_Whose_WhenAsyncMemberReturnsNullTask_ShouldFail()
		{
			void Delegate() => throw new AsyncException(1, task: null);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetTask(), v => v.IsEqualTo(1));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetTask() is equal to 1,
				             but GetTask() returned <null> instead of a task
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull())
				.Because("a null task is not an exception thrown by the member");
		}

		[Test]
		public async Task Throws_Whose_WhenExpectationsIsNull_ShouldThrowArgumentNullException()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.Message, null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expectations").And
				.WithMessage("The 'expectations' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Throws_Whose_WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose((Func<AsyncException, string>)null!, m => m.IsNull());

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("memberAccessor").And
				.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Throws_Whose_WhenTheAsyncMemberIsACollection_ShouldUseThePluralForm()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetItemsAsync(), v => v.Get().ExpectationBuilder.AddConstraint((_, g)
						=> new DummyConstraint<int[]?>(_ => false, g.Verb("has one item", "have one item"))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetItemsAsync() have one item,
				             *
				             """).AsWildcard();
		}

		[Test]
		public async Task Throws_Whose_WhenTheMemberIsACollection_ShouldUseThePluralForm()
		{
			void Delegate() => throw new AggregateException(new InvalidOperationException());

			async Task Act()
				=> await That(Delegate).Throws<AggregateException>()
					.Whose(e => e.InnerExceptions, v => v.Get().ExpectationBuilder.AddConstraint((_, g)
						=> new DummyConstraint<ReadOnlyCollection<Exception>?>(_ => false,
							g.Verb("has two items", "have two items"))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws an AggregateException whose InnerExceptions have two items,
				             *
				             """).AsWildcard()
				.Because("the number of the member follows its type, like in the other Whose overloads");
		}

		[Test]
		public async Task Throws_Whose_WhenValueTaskMemberAccessorIsNull_ShouldThrowArgumentNullException()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose((Func<AsyncException, ValueTask<int>>)null!, v => v.IsEqualTo(1));

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("memberAccessor").And
				.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Throws_Whose_WithAsyncLambda_ShouldRenderMemberPath()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(async e => await e.GetValueAsync(), v => v.IsEqualTo(2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetValueAsync() is equal to 2,
				             but GetValueAsync() was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task Throws_Whose_WithAsyncLambda_WithConfigureAwait_ShouldRenderMemberPath()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(async e => await e.GetValueAsync().ConfigureAwait(false), v => v.IsEqualTo(2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetValueAsync() is equal to 2,
				             but GetValueAsync() was 1, which differs by -1
				             """)
				.Because("ConfigureAwait does not select a member, so it is not part of the member path");
		}

		[Test]
		public async Task Throws_Whose_WithAsyncLambda_WithoutMemberPath_ShouldRenderExpression()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(async e => await e.GetValueAsync() + 1, v => v.IsEqualTo(3));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose async e => await e.GetValueAsync() + 1 is equal to 3,
				             but async e => await e.GetValueAsync() + 1 was 2, which differs by -1
				             """)
				.Because("an expression that is not a member path must not be shortened");
		}

		[Test]
		public async Task Throws_Whose_WithAsyncMember_ShouldVerifyAwaitedValue()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetValueAsync() is equal to 2,
				             but GetValueAsync() was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task Throws_Whose_WithAsyncMember_WhenExpectationsIsNull_ShouldThrowArgumentNullException()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetValueAsync(), null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expectations").And
				.WithMessage("The 'expectations' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Throws_Whose_WithAsyncMember_WhenMatching_ShouldSucceed()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(1));

			await That(Act).DoesNotThrow();
		}

		private sealed class AsyncException(int value, Task<int>? task = null) : Exception
		{
			public async Task<int> FaultedAsync()
			{
				await Task.Yield();
				throw new InvalidOperationException($"async member failed for {value}");
			}

			public Task<int[]> GetItemsAsync() => Task.FromResult(new[] { value, });

			public Task<int> GetTask() => task!;

			public Task<int> GetValueAsync() => Task.FromResult(value);
		}
	}
}
