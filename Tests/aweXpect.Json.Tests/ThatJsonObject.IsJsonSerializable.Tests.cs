using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonObject
{
	public sealed class IsJsonSerializable
	{
		public sealed class ObjectTests
		{
			[Test]
			public async Task WhenDefaultSerializerOptionsAreCustomized_ShouldUseThem()
			{
				PocoWithoutDefaultFieldConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
				       {
					       IncludeFields = true,
				       }))
				{
					await That(Act).DoesNotThrow()
						.Because("the customized default options include the field that the constructor binds to");
				}
			}

			[Test]
			public async Task WhenIgnoredMemberMatchesNoMember_ShouldHintAtTheClosestMember()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable(o => o with
					{
						MembersToIgnore = [new MemberToIgnore.ByName("Nme"),],
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was not:
					               Property Name differed:
					                   Actual: <null>
					                 Expected: "foo"

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Nme"]
					                "Nme" matched no member (did you mean "Name"?)
					             """)
					.Because("a misspelled member to ignore explains why the difference was not ignored");
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				object?[] items = [new PocoWithIgnoredProperty { Id = 1, Name = "foo", },];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsJsonSerializable()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are serializable as JSON for all items,
					             but none of 1 were

					             Not matching items (Items):
					             [
					               ThatJsonObject.IsJsonSerializable.PocoWithIgnoredProperty {
					                 Id = 1,
					                 Name = "foo"
					               }
					             ]

					             Collection (Items):
					             [
					               ThatJsonObject.IsJsonSerializable.PocoWithIgnoredProperty {
					                 Id = 1,
					                 Name = "foo"
					               }
					             ]

					             Equivalency options (Items[0]):
					              - include public fields and properties
					             """)
					.Because("the items are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenNestedInThatAll_ShouldIndentTheDifferences()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await ThatAll(That(subject).IsJsonSerializable());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that subject is serializable as JSON
					             but
					              [01] it was not:
					                     Property Name differed:
					                         Actual: <null>
					                       Expected: "foo"

					             [01] Equivalency options:
					              - include public fields and properties
					             """)
					.Because("every line of the differences belongs to the nested result");
			}

			[Test]
			public async Task WhenSerializerOptionsAreGiven_ShouldPreferThemOverTheCustomizedDefault()
			{
				PocoWithoutDefaultFieldConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable(new JsonSerializerOptions());

				using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
				       {
					       IncludeFields = true,
				       }))
				{
					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is serializable as JSON,
						             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultFieldConstructor' must bind to an object property or field on deserialization. Each parameter name*must match with a property or field on the object. Fields are only considered when 'JsonSerializerOptions.IncludeFields' is enabled*
						             """).AsWildcard()
						.Because("the given serializer options replace the customized default options");
				}
			}

			[Test]
			public async Task WhenSubjectHasAnIgnoredProperty_ShouldFail()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was not:
					               Property Name differed:
					                   Actual: <null>
					                 Expected: "foo"

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
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
						MembersToIgnore = [new MemberToIgnore.ByName("Name"),],
					});

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAPrivateConstructor_ShouldFail()
			{
				PocoWithPrivateConstructor subject = PocoWithPrivateConstructor.Create(42);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported. Type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithPrivateConstructor'*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSubjectHasAPrivateConstructorWithJsonConstructorAttribute_ShouldSucceed()
			{
				PocoWithPrivateConstructorWithJsonConstructorAttribute subject =
					PocoWithPrivateConstructorWithJsonConstructorAttribute.Create(42);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasNoDefaultConstructor_ShouldFail()
			{
				PocoWithoutDefaultConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultConstructor' must bind to an object property or field on deserialization. Each parameter name*must match with a property or field on the object*
					             """).AsWildcard();
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenSubjectHasNoDefaultFieldConstructor_ShouldFailUnlessIncludeFieldsIsSet(
				bool includeFields)
			{
				PocoWithoutDefaultFieldConstructor subject = new(12);

				async Task Act()
					=> await That(subject).IsJsonSerializable(new JsonSerializerOptions
					{
						IncludeFields = includeFields,
					});

				await That(Act).Throws<FailException>().OnlyIf(!includeFields)
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: Each parameter in the deserialization constructor on type 'aweXpect.Json.Tests.ThatJsonObject+IsJsonSerializable+PocoWithoutDefaultFieldConstructor' must bind to an object property or field on deserialization. Each parameter name*must match with a property or field on the object. Fields are only considered when 'JsonSerializerOptions.IncludeFields' is enabled*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSubjectIsDeclaredAsBaseType_ShouldRoundTripTheRuntimeType()
			{
				BasePoco subject = new DerivedPoco
				{
					Id = 1,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).DoesNotThrow()
					.Because("the subject is serialized as its runtime type, not as the declared type of the subject");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was <null>
					             """);
			}

			[Test]
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

		public sealed class TypeInfoTests
		{
			[Test]
			public async Task WhenDefaultSerializerOptionsAreCustomized_ShouldUseTheOptionsOfTheTypeInfo()
			{
				PocoWithField subject = new()
				{
					Value = 12,
				};
				JsonTypeInfo<PocoWithField> typeInfo = GetTypeInfo<PocoWithField>(new JsonSerializerOptions());

				async Task Act()
					=> await That(subject).IsJsonSerializable(typeInfo);

				using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
				       {
					       IncludeFields = true,
				       }))
				{
					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is serializable as JSON,
						             but it was not:
						               Field Value differed:
						                   Actual: 0
						                 Expected: 12

						             Equivalency options:
						              - include public fields and properties
						             """)
						.Because("the type info carries its own options, which do not include fields");
				}
			}

			[Test]
			public async Task WhenSerializationThrows_ShouldFail()
			{
				Node subject = new();
				subject.Next = subject;

				async Task Act()
					=> await That(subject).IsJsonSerializable(GetTypeInfo<Node>(new JsonSerializerOptions()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it could not be deserialized: A possible object cycle was detected.*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSubjectHasAnIgnoredProperty_ShouldFail()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable(
						GetTypeInfo<PocoWithIgnoredProperty>(new JsonSerializerOptions()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was not:
					               Property Name differed:
					                   Actual: <null>
					                 Expected: "foo"

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenSubjectHasAnIgnoredProperty_WhenPropertyIsIgnored_ShouldSucceed()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).IsJsonSerializable(
						GetTypeInfo<PocoWithIgnoredProperty>(new JsonSerializerOptions()), o => o with
						{
							MembersToIgnore = [new MemberToIgnore.ByName("Name"),],
						});

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				SimplePocoWithPrimitiveTypes? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializable(
						GetTypeInfo<SimplePocoWithPrimitiveTypes>(new JsonSerializerOptions()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serializable as JSON,
					             but it was <null>
					             """);
			}

			[Test]
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
					=> await That(subject).IsJsonSerializable(
						GetTypeInfo<SimplePocoWithPrimitiveTypes>(new JsonSerializerOptions()));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectHasAnIgnoredProperty_ShouldSucceed()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAnIgnoredProperty_WhenPropertyIsIgnored_ShouldFail()
			{
				PocoWithIgnoredProperty subject = new()
				{
					Id = 2,
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable(o => o with
					{
						MembersToIgnore = [new MemberToIgnore.ByName("Name"),],
					}));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serializable as JSON,
					             but it was

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Name"]
					             """);
			}

			[Test]
			public async Task WhenSubjectHasAPrivateConstructor_ShouldSucceed()
			{
				PocoWithPrivateConstructor subject = PocoWithPrivateConstructor.Create(42);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAPrivateConstructorWithJsonConstructorAttribute_ShouldFail()
			{
				PocoWithPrivateConstructorWithJsonConstructorAttribute subject =
					PocoWithPrivateConstructorWithJsonConstructorAttribute.Create(42);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serializable as JSON,
					             but it was

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenSubjectHasNoDefaultConstructor_ShouldSucceed()
			{
				PocoWithoutDefaultConstructor subject = new(12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serializable as JSON,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithTypeInfo_ShouldFail()
			{
				SimplePocoWithPrimitiveTypes? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable(
						GetTypeInfo<SimplePocoWithPrimitiveTypes>(new JsonSerializerOptions())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serializable as JSON,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsPoco_ShouldFail()
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
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serializable as JSON,
					             but it was

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenSubjectIsPoco_WithTypeInfo_ShouldFail()
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
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializable(
						GetTypeInfo<SimplePocoWithPrimitiveTypes>(new JsonSerializerOptions())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serializable as JSON,
					             but it was

					             Equivalency options:
					              - include public fields and properties
					             """);
			}
		}

		private static JsonTypeInfo<T> GetTypeInfo<T>(JsonSerializerOptions options)
		{
			options.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
			return (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
		}

		private class BasePoco
		{
			public int Id { get; set; }
		}

		private sealed class DerivedPoco : BasePoco
		{
			public string Name { get; set; } = "";
		}

		private sealed class Node
		{
			public Node? Next { get; set; }
		}

		private class PocoWithoutDefaultConstructor(int value)
		{
			public int Id { get; } = value;
		}

		private class PocoWithIgnoredProperty
		{
			public int Id { get; set; }

			[JsonIgnore] public string? Name { get; set; }
		}

		private class PocoWithoutDefaultFieldConstructor(int value)
		{
			public int Value = value;
		}

		private sealed class PocoWithField
		{
			public int Value;
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

		private class SimplePocoWithPrimitiveTypes
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
