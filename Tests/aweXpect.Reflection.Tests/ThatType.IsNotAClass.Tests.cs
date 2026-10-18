using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAClass
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAClass_ShouldFail()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).IsNotAClass();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a class,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonClassTypes))]
			public async Task WhenTypeIsNotAClass_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAClass();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAClass();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a class,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> NonClassTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAClass_ShouldSucceed()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAClass());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonClassTypes))]
			public async Task WhenTypeIsNotAClass_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAClass());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is a class*but it was*").AsWildcard();
			}

			public static IEnumerable<Type> NonClassTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
