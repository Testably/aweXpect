using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreOfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFailWhenSomePropertiesAreNotOfType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type int,
					             but it contained not matching properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllPropertiesAreOfType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
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
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
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
			public async Task WhenAllPropertiesAreOfType_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<int>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all are of type int,
					             but it only contained matching properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSomePropertiesAreNotOfType_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
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
			public int IntProperty { get; set; }
			public int OtherIntProperty { get; set; }
			public string StringProperty { get; set; }
			public Dummy DummyProperty { get; set; }
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
			public async Task ShouldSucceedWhenAllPropertiesAreOfType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
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
			public async Task WhenAllPropertiesAreOneOfTheTypes_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>().OrOfType(typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomePropertiesAreNoneOfTheTypes_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<bool>().OrOfType<long>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type bool or of type long,
					             but it contained not matching properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#pragma warning restore CA2263
	}
}
