using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotImmutable
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(MutableTypes))]
			public async Task WhenTypeIsMutable_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsImmutable_ShouldFail()
			{
				Type subject = typeof(ImmutableClass);

				async Task Act()
				{
					await That(subject).IsNotImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not immutable,
					             but it was immutable ImmutableClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not immutable,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> MutableTypes() => new()
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
			[Fact]
			public async Task WhenTypeIsImmutable_ShouldSucceed()
			{
				Type subject = typeof(ImmutableClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotImmutable());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsMutable_ShouldFail()
			{
				Type subject = typeof(ClassWithMutableField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotImmutable());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithMutableField with mutable members [
					               int ClassWithMutableField.Value
					             ]
					             """);
			}
		}
	}
}
