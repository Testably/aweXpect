using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class UsingTests
		{
			[Fact]
			public async Task WhenComparerConsidersDifferent_ShouldSucceed()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
				};
				OuterClass expected = new()
				{
					Value = "Bar",
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(expected).Using(new MyComparer(false));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenComparerConsidersEqual_ShouldFail()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
				};
				OuterClass expected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(expected).Using(new MyComparer(true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               } using ThatObject.IsNotEqualTo.UsingTests.MyComparer,
					             but it was ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               }
					             """);
			}

			[Fact]
			public async Task WhenComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				OuterClass subject = new()
				{
					Value = "Foo",
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(subject).Using(new ThrowingComparer(exception));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               } using ThatObject.IsNotEqualTo.UsingTests.ThrowingComparer,
					             but the comparer did throw an InvalidOperationException:
					               comparer failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a comparer that threw answered nothing, so the negation fails as well");
			}

			private sealed class MyComparer(bool considerEqual) : IEqualityComparer<object>
			{
				#region IEqualityComparer<object> Members

				bool IEqualityComparer<object>.Equals(object? x, object? y)
					=> considerEqual;

				public int GetHashCode(object obj)
					=> obj.GetHashCode();

				#endregion
			}

			private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<object>
			{
				#region IEqualityComparer<object> Members

				bool IEqualityComparer<object>.Equals(object? x, object? y)
					=> throw exception;

				public int GetHashCode(object obj)
					=> obj.GetHashCode();

				#endregion
			}
		}
	}
}
