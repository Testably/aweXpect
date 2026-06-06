using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsImmutable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeHasMultipleMutableMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithMutableFieldAndSettableProperty);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithMutableFieldAndSettableProperty with mutable members [
					               int ClassWithMutableFieldAndSettableProperty.Field,
					               public int ClassWithMutableFieldAndSettableProperty.Property { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasMutableField_ShouldFail()
			{
				Type subject = typeof(ClassWithMutableField);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithMutableField with mutable members [
					               int ClassWithMutableField.Value
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasPropertyWithPrivateSetter_ShouldFail()
			{
				Type subject = typeof(ClassWithPrivateSettableProperty);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithPrivateSettableProperty with mutable members [
					               public int ClassWithPrivateSettableProperty.Value { get; private set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasSettableIndexer_ShouldFail()
			{
				Type subject = typeof(ClassWithSettableIndexer);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithSettableIndexer with mutable members [
					               public int ClassWithSettableIndexer.Item { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasSettableProperty_ShouldFail()
			{
				Type subject = typeof(ClassWithSettableProperty);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithSettableProperty with mutable members [
					               public int ClassWithSettableProperty.Value { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsMutableField_ShouldFail()
			{
				Type subject = typeof(ClassInheritingMutableField);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassInheritingMutableField with mutable members [
					               int MutableBaseClass._value
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsProtectedMutableField_ShouldFail()
			{
				Type subject = typeof(ClassInheritingProtectedMutableField);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassInheritingProtectedMutableField with mutable members [
					               int MutableBaseClassWithProtectedField.ProtectedValue
					             ]
					             """);
			}

			[Theory]
			[MemberData(nameof(ImmutableTypes))]
			public async Task WhenTypeIsImmutable_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPositionalRecordStruct_ShouldFail()
			{
				Type subject = typeof(MutableRecordStruct);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable MutableRecordStruct with mutable members [
					               public int MutableRecordStruct.Value { get; set; }
					             ]
					             """);
			}

			public static TheoryData<Type> ImmutableTypes() => new()
			{
				typeof(ImmutableClass),
				typeof(ImmutableClassWithInitProperty),
				typeof(ImmutableDerivedClass),
				typeof(PublicRecord),
				typeof(PublicSealedClass),
				typeof(PositionalRecord),
				typeof(ImmutableReadOnlyStruct),
				typeof(ImmutableRecordStruct),
				typeof(IImmutableInterface),
				typeof(PublicEnum),
				typeof(PublicStaticClass),
				typeof(GenericImmutableClass<>),
			};
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsImmutable_ShouldFail()
			{
				Type subject = typeof(ImmutableClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsImmutable());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not immutable,
					             but it was immutable ImmutableClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsMutable_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMutableField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsImmutable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
