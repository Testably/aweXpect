using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAsync
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsAsync_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.AsyncMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAsync();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not async,
					              but it was async Task ClassWithAsyncMembers.AsyncMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsNotAsync_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAsync();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAsync();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not async,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodReturnsTaskButIsNotAsync_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.NonAsyncTaskMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAsync();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsAsync_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.AsyncMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAsync());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNotAsync_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAsync());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is async,
					              but it was non-async void ClassWithAsyncMembers.RegularMethod()
					              """);
			}
		}
	}
}
