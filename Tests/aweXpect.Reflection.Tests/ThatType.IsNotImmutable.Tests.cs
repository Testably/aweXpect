using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotImmutable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsImmutable_ShouldFail()
			{
				Type subject = typeof(ImmutableClass);

				async Task Act()
				{
					await That(subject).IsNotImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not immutable,
					             but it was immutable ImmutableClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(MutableTypes))]
			public async Task WhenTypeIsMutable_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not immutable,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> MutableTypes() => new List<Type>
			{
				typeof(ClassWithMutableField),
				typeof(ClassWithSettableProperty),
				typeof(ClassWithMutableFieldAndSettableProperty),
				typeof(MutableBaseClass),
				typeof(ClassInheritingMutableField),
				typeof(MutableBaseClassWithProtectedField),
				typeof(ClassInheritingProtectedMutableField),
				typeof(ClassWithSettableIndexer),
				typeof(ClassWithPrivateSettableProperty),
				typeof(MutableStruct),
				typeof(MutableRecordStruct),
				typeof(IMutableInterface),
				typeof(GenericMutableClass<>),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsImmutable_ShouldSucceed()
			{
				Type subject = typeof(ImmutableClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotImmutable());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsMutable_ShouldFail()
			{
				Type subject = typeof(ClassWithMutableField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotImmutable());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithMutableField

					             Mutable members:
					             [
					               int ClassWithMutableField.Value
					             ]
					             """);
			}
		}
	}
}
