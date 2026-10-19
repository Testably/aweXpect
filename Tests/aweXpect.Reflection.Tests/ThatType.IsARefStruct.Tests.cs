using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsARefStruct
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsARefStruct_ShouldSucceed()
			{
				Type subject = typeof(PublicRefStruct);

				async Task Act()
				{
					await That(subject).IsARefStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonRefStructType))]
			public async Task WhenTypeIsNotARefStruct_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsARefStruct();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is a ref struct,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsARefStruct();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is a ref struct,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonRefStructType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(PublicReadOnlyStruct), "struct PublicReadOnlyStruct"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsARefStruct_ShouldFail()
			{
				Type subject = typeof(PublicRefStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARefStruct());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a ref struct,
					             but it was struct PublicRefStruct
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonRefStructTypeForNegated))]
			public async Task WhenTypeIsNotARefStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARefStruct());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonRefStructTypeForNegated() => new List<Type>
			{
				typeof(PublicStruct),
				typeof(PublicReadOnlyStruct),
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
