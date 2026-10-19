using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAStruct
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAStruct_ShouldFail()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
				{
					await That(subject).IsNotAStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a struct,
					             but it was struct PublicStruct
					             """);
			}

			[Test]
			[MethodDataSource(nameof(StructTypesForNegated))]
			public async Task WhenTypeIsAStruct_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAStruct());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotAStruct_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAStruct());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a struct,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonStructTypes))]
			public async Task WhenTypeIsNotAStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a struct,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> StructTypesForNegated() => new List<Type>
			{
				typeof(PublicStruct),
			};

			public static IEnumerable<Type> NonStructTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
