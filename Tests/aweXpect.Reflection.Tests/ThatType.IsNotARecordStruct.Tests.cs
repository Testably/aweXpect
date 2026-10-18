using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotARecordStruct
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsARecordStruct_ShouldFail()
			{
				Type subject = typeof(PublicRecordStruct);

				async Task Act()
				{
					await That(subject).IsNotARecordStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a record struct,
					             but it was record struct PublicRecordStruct
					             """);
			}

			[Test]
			[MethodDataSource(nameof(RecordStructTypesForNegated))]
			public async Task WhenTypeIsARecordStruct_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARecordStruct());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotARecordStruct_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARecordStruct());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a record struct,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonRecordStructTypes))]
			public async Task WhenTypeIsNotARecordStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotARecordStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotARecordStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a record struct,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> RecordStructTypesForNegated() => new List<Type>
			{
				typeof(PublicRecordStruct),
			};

			public static IEnumerable<Type> NonRecordStructTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicRecord),
					typeof(PublicStruct),
				];
		}
	}
}
