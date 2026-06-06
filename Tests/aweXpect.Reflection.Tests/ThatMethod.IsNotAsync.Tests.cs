using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAsync
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsAsync_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.AsyncMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAsync();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not async,
					              but it was async {Formatter.Format(subject)}
					              """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAsync();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not async,
					             but it was <null>
					             """);
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenMethodIsNotAsync_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithAsyncMembers).GetMethod(nameof(ClassWithAsyncMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAsync());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is async,
					              but it was non-async {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
