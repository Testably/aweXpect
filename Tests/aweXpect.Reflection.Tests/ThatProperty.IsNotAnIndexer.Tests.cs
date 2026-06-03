using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotAnIndexer
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotAnIndexer_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty(nameof(TestClassWithIndexers.RegularProperty))!;

				async Task Act()
				{
					await That(subject).IsNotAnIndexer();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnIndexer();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an indexer,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsAnIndexer_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty("Item")!;

				async Task Act()
				{
					await That(subject).IsNotAnIndexer();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an indexer,
					              but it was an indexer {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsNotAnIndexer_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty(nameof(TestClassWithIndexers.RegularProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnIndexer());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an indexer,
					              but it was not an indexer {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsAnIndexer_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty("Item")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnIndexer());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
