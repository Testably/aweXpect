using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAStruct
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAStruct_ShouldSucceed()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
				{
					await That(subject).IsAStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonStructType))]
			public async Task WhenTypeIsNotAStruct_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAStruct();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is a struct,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is a struct,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonStructType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAStruct_ShouldFail()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAStruct());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a struct,
					             but it was struct PublicStruct
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonStructTypeForNegated))]
			public async Task WhenTypeIsNotAStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAStruct());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonStructTypeForNegated() => new List<Type>
			{
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicRecord),
			};

			public static IEnumerable<(Type?, string)> NonStructType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
			};
		}
	}
}
