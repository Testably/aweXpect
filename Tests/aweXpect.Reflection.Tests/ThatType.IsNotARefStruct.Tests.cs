using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotARefStruct
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsARefStruct_ShouldFail()
			{
				Type subject = typeof(PublicRefStruct);

				async Task Act()
				{
					await That(subject).IsNotARefStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a ref struct,
					             but it was struct PublicRefStruct
					             """);
			}

			[Test]
			[MethodDataSource(nameof(RefStructTypesForNegated))]
			public async Task WhenTypeIsARefStruct_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARefStruct());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotARefStruct_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARefStruct());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a ref struct,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonRefStructTypes))]
			public async Task WhenTypeIsNotARefStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotARefStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotARefStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a ref struct,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> RefStructTypesForNegated() => new List<Type>
			{
				typeof(PublicRefStruct),
			};

			public static IEnumerable<Type> NonRefStructTypes() => new List<Type>
			{
				typeof(PublicStruct),
				typeof(PublicReadOnlyStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicClass),
				typeof(PublicRecordStruct),
			};
		}
	}
}
