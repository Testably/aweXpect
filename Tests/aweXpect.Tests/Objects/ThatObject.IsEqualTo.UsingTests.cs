using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEqualTo
	{
		public sealed class UsingTests
		{
			[Theory]
			[InlineData(false, false)]
			[InlineData(false, true)]
			[InlineData(true, false)]
			[InlineData(true, true)]
			public async Task WhenCombinedWithEquivalent_ShouldThrowInvalidOperationException(bool comparerFirst,
				bool negated)
			{
				OuterClass subject = new()
				{
					Value = "Foo",
				};

				async Task Act()
				{
					switch (comparerFirst, negated)
					{
						case (true, false):
							await That(subject).IsEqualTo(subject).Using(new MyComparer(true)).Equivalent();
							break;
						case (true, true):
							await That(subject).IsNotEqualTo(subject).Using(new MyComparer(true)).Equivalent();
							break;
						case (false, false):
							await That(subject).IsEqualTo(subject).Equivalent().Using(new MyComparer(true));
							break;
						case (false, true):
							await That(subject).IsNotEqualTo(subject).Equivalent().Using(new MyComparer(true));
							break;
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage(comparerFirst
						? "Equivalent cannot be combined with Using."
						: "Using cannot be combined with Equivalent.")
					.Because("the second option would silently replace the comparison of the first one");
			}

			[Fact]
			public async Task WhenComparerConsidersDifferent_ShouldFail()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
				};
				OuterClass expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Using(new MyComparer(false));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               } using ThatObject.IsEqualTo.UsingTests.MyComparer,
					             but it was ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               }
					             """);
			}

			[Fact]
			public async Task WhenComparerConsidersEqual_ShouldSucceed()
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
					=> await That(subject).IsEqualTo(expected).Using(new MyComparer(true));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task WhenComparerIsNull_ShouldThrowArgumentNullException(bool negated)
			{
				OuterClass subject = new();

				async Task Act()
				{
					if (negated)
					{
						await That(subject).IsNotEqualTo(subject).Using(null!);
					}
					else
					{
						await That(subject).IsEqualTo(subject).Using(null!);
					}
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer").And
					.WithMessage("The 'comparer' cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task WhenComparerIsSpecifiedTwice_ShouldThrowInvalidOperationException(bool negated)
			{
				OuterClass subject = new();

				async Task Act()
				{
					if (negated)
					{
						await That(subject).IsNotEqualTo(subject)
							.Using(new MyComparer(true)).Using(new MyComparer(false));
					}
					else
					{
						await That(subject).IsEqualTo(subject)
							.Using(new MyComparer(true)).Using(new MyComparer(false));
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Using cannot be specified more than once.")
					.Because("the second comparer would silently replace the first one");
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
					=> await That(subject).IsEqualTo(subject).Using(new ThrowingComparer(exception));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               } using ThatObject.IsEqualTo.UsingTests.ThrowingComparer,
					             but the comparer did throw an InvalidOperationException:
					               comparer failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
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
