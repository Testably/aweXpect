using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreOfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFailWhenSomeFieldsAreNotOfType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type int,
					             but it contained not matching fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllFieldsAreOfType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSucceedWithInheritance()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllFieldsAreOfType_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<int>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all are of type int,
					             but it only contained matching fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSomeFieldsAreNotOfType_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public int IntField;
			public int OtherIntField;
			public string StringField;
			public Dummy DummyField;
#pragma warning restore CS0649
#pragma warning restore CS8618
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldSucceedWhenAllFieldsAreOfType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task WhenAllFieldsAreOneOfTheTypes_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>().OrOfType(typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeFieldsAreNoneOfTheTypes_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<bool>().OrOfType<long>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type bool or of type long,
					             but it contained not matching fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#pragma warning restore CA2263
	}
}
