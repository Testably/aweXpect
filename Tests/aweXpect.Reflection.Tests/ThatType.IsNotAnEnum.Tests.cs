using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnEnum
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnEnum_ShouldFail()
			{
				Type subject = typeof(PublicEnum);

				async Task Act()
				{
					await That(subject).IsNotAnEnum();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an enum,
					             but it was enum PublicEnum
					             """);
			}

			[Test]
			[MethodDataSource(nameof(EnumTypesForNegated))]
			public async Task WhenTypeIsAnEnum_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnEnum());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotAnEnum_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnEnum());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an enum,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonEnumTypes))]
			public async Task WhenTypeIsNotAnEnum_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAnEnum();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnEnum();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an enum,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> EnumTypesForNegated() => new List<Type>
			{
				typeof(PublicEnum),
			};

			public static IEnumerable<Type> NonEnumTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicClass),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
