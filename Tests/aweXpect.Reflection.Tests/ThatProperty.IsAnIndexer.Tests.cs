using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsAnIndexer
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsAnIndexer_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty("Item")!;

				async Task Act()
				{
					await That(subject).IsAnIndexer();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNotAnIndexer_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty(nameof(TestClassWithIndexers.RegularProperty))!;

				async Task Act()
				{
					await That(subject).IsAnIndexer();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is an indexer,
					              but it was not an indexer public string TestClassWithIndexers.RegularProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsAnIndexer();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is an indexer,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsAnIndexer_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty("Item")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnIndexer());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not an indexer,
					              but it was an indexer public string TestClassWithIndexers.Item { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNotAnIndexer_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithIndexers)
					.GetProperty(nameof(TestClassWithIndexers.RegularProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnIndexer());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
