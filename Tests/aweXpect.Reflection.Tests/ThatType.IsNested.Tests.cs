using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNested_ShouldSucceed()
			{
				Type subject = typeof(Container.PublicNestedClass);

				async Task Act()
					=> await That(subject).IsNested();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotNested_ShouldFail()
			{
				Type subject = typeof(Container);

				async Task Act()
					=> await That(subject).IsNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is nested,
					             but it was non-nested Container
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is nested,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsNested_ShouldFail()
			{
				Type subject = typeof(Container.PublicNestedClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNested());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not nested,
					             but it was nested Container.PublicNestedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotNested_ShouldSucceed()
			{
				Type subject = typeof(Container);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNested());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
