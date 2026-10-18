using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnEnum
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnEnum_ShouldSucceed()
			{
				Type subject = typeof(PublicEnum);

				async Task Act()
				{
					await That(subject).IsAnEnum();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonEnumType))]
			public async Task WhenTypeIsNotAnEnum_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAnEnum();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is an enum,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAnEnum();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is an enum,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonEnumType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAnEnum_ShouldFail()
			{
				Type subject = typeof(PublicEnum);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnEnum());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an enum,
					             but it was enum PublicEnum
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonEnumTypeForNegated))]
			public async Task WhenTypeIsNotAnEnum_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnEnum());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonEnumTypeForNegated() => new List<Type>
			{
				typeof(IPublicInterface),
				typeof(PublicStruct),
				typeof(PublicClass),
				typeof(PublicRecord),
				typeof(PublicRecordStruct),
			};

			public static IEnumerable<(Type?, string)> NonEnumType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}
	}
}
