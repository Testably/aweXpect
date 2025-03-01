using System.Text.Json;
using System.Text.Json.Serialization;

namespace aweXpect.Json.Tests;

public sealed class ThatJsonObject
{
	public sealed class IsJsonSerializable
	{
		public sealed class ObjectTests
		{
			[Fact]
			public async Task WhenSubjectHasAnIgnoredProperty_ShouldFail()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was not:
					               Property Name was <null> instead of "foo"
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAnIgnoredProperty_WhenPropertyIsIgnored_ShouldSucceed()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable(o => o with
					{
						MembersToIgnore = ["Name",],
					});

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectHasAPrivateConstructor_ShouldFail()
			{
				PocoWithPrivateConstructor subject = PocoWithPrivateConstructor.Create(42);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported. Type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithPrivateConstructor'*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectHasAPrivateConstructorWithJsonConstructorAttribute_ShouldSucceed()
			{
				PocoWithPrivateConstructorWithJsonConstructorAttribute subject =
					PocoWithPrivateConstructorWithJsonConstructorAttribute.Create(42);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectHasNoDefaultConstructor_ShouldFail()
			{
				PocoWithoutDefaultConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultConstructor' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object*
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectHasNoDefaultFieldConstructor_ShouldFailUnlessIncludeFieldsIsSet(
				bool includeFields)
			{
				PocoWithoutDefaultFieldConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable(new JsonSerializerOptions
					{
						IncludeFields = includeFields,
					});

				await That(Act).Throws<XunitException>().OnlyIf(!includeFields)
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultFieldConstructor' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object. Fields are only considered when 'JsonSerializerOptions.IncludeFields' is enabled*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsPoco_ShouldSucceed()
			{
				SimplePocoWithPrimitiveTypes subject = new()
				{
					Id = 1,
					GlobalId = Guid.NewGuid(),
					Name = "foo",
					DateOfBirth = DateTime.Today,
					Height = new decimal(4.3),
					Weight = 5.6,
					ShoeSize = 7.8f,
					IsActive = true,
					Image = [8, 9, 10, 11,],
					Category = 'a',
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenSubjectHasAnIgnoredProperty_ShouldFail()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithIgnoredProperty>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as PocoWithIgnoredProperty JSON,
					             but it was not:
					               Property Name was <null> instead of "foo"
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAnIgnoredProperty_WhenPropertyIsIgnored_ShouldSucceed()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithIgnoredProperty>(o => o with
					{
						MembersToIgnore = ["Name",],
					});

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectHasAPrivateConstructor_ShouldFail()
			{
				PocoWithPrivateConstructor subject = PocoWithPrivateConstructor.Create(42);

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithPrivateConstructor>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as PocoWithPrivateConstructor JSON,
					             but it could not be deserialized: Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported. Type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithPrivateConstructor'*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectHasAPrivateConstructorWithJsonConstructorAttribute_ShouldSucceed()
			{
				PocoWithPrivateConstructorWithJsonConstructorAttribute subject =
					PocoWithPrivateConstructorWithJsonConstructorAttribute.Create(42);

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithPrivateConstructorWithJsonConstructorAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectHasNoDefaultConstructor_ShouldFail()
			{
				PocoWithoutDefaultConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithoutDefaultConstructor>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as PocoWithoutDefaultConstructor JSON,
					             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultConstructor' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object*
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectHasNoDefaultFieldConstructor_ShouldFailUnlessIncludeFieldsIsSet(
				bool includeFields)
			{
				PocoWithoutDefaultFieldConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithoutDefaultFieldConstructor>(
						new JsonSerializerOptions
						{
							IncludeFields = includeFields,
						});

				await That(Act).Throws<XunitException>().OnlyIf(!includeFields)
					.WithMessage("""
					             Expected that subject
					             is serializable as PocoWithoutDefaultFieldConstructor JSON,
					             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultFieldConstructor' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object. Fields are only considered when 'JsonSerializerOptions.IncludeFields' is enabled*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializable<SimplePocoWithPrimitiveTypes>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as SimplePocoWithPrimitiveTypes JSON,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeDoesNotMatch_ShouldFail()
			{
				SimplePocoWithPrimitiveTypes subject = new()
				{
					Id = 1,
					GlobalId = Guid.NewGuid(),
					Name = "foo",
					DateOfBirth = DateTime.Today,
					Height = new decimal(4.3),
					Weight = 5.6,
					ShoeSize = 7.8f,
					IsActive = true,
					Image = [8, 9, 10, 11,],
					Category = 'a',
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable<PocoWithIgnoredProperty>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as PocoWithIgnoredProperty JSON,
					             but it was not assignable to PocoWithIgnoredProperty
					             """);
			}

			[Fact]
			public async Task WhenTypeMatches_ShouldSucceed()
			{
				SimplePocoWithPrimitiveTypes subject = new()
				{
					Id = 1,
					GlobalId = Guid.NewGuid(),
					Name = "foo",
					DateOfBirth = DateTime.Today,
					Height = new decimal(4.3),
					Weight = 5.6,
					ShoeSize = 7.8f,
					IsActive = true,
					Image = [8, 9, 10, 11,],
					Category = 'a',
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable<SimplePocoWithPrimitiveTypes>();

				await That(Act).DoesNotThrow();
			}
		}

		public class PocoWithoutDefaultConstructor(int value)
		{
			public int Id { get; } = value;
		}

		public class PocoWithIgnoredProperty
		{
			public int Id { get; set; }

			[JsonIgnore] public string? Name { get; set; }
		}

		public class PocoWithoutDefaultFieldConstructor(int value)
		{
			public int Value = value;
		}

		public class PocoWithPrivateConstructor
		{
			private PocoWithPrivateConstructor() { }

			public int Id { get; set; }

			public static PocoWithPrivateConstructor Create(int id) => new()
			{
				Id = id,
			};
		}

		public class PocoWithPrivateConstructorWithJsonConstructorAttribute
		{
			[JsonConstructor]
			private PocoWithPrivateConstructorWithJsonConstructorAttribute() { }

			public int Id { get; init; }

			public static PocoWithPrivateConstructorWithJsonConstructorAttribute Create(int id) => new()
			{
				Id = id,
			};
		}

		public class SimplePocoWithPrimitiveTypes
		{
			public int Id { get; set; }

			public Guid GlobalId { get; set; }

			public string Name { get; set; } = "";

			public DateTime DateOfBirth { get; set; }

			public decimal Height { get; set; }

			public double Weight { get; set; }

			public float ShoeSize { get; set; }

			public bool IsActive { get; set; }

			public byte[] Image { get; set; } = [];

			public char Category { get; set; }
		}
	}
}
