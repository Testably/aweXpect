using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsARecordStruct
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsARecordStruct_ShouldSucceed()
			{
				Type subject = typeof(PublicRecordStruct);

				async Task Act()
				{
					await That(subject).IsARecordStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonRecordStructType))]
			public async Task WhenTypeIsNotARecordStruct_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsARecordStruct();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is a record struct,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsARecordStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is a record struct,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonRecordStructType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicStruct), "struct PublicStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsARecordStruct_ShouldFail()
			{
				Type subject = typeof(PublicRecordStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARecordStruct());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a record struct,
					             but it was record struct PublicRecordStruct
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonRecordStructTypeForNegated))]
			public async Task WhenTypeIsNotARecordStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARecordStruct());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonRecordStructTypeForNegated() => new List<Type>
			{
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicRecord),
				typeof(PublicStruct),
			};

			public static IEnumerable<(Type?, string)> NonRecordStructType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicStruct), "struct PublicStruct"),
			};
		}
	}
}
