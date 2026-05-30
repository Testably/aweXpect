using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreOfExactType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFailWhenPropertiesInheritFromType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type *DummyBase,
					             but it contained not matching properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllPropertiesAreOfExactType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task WhenAllPropertiesAreOneOfTheExactTypes_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>().OrOfExactType<Dummy>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
			public DummyBase DummyBaseProperty { get; set; }
			public Dummy DummyProperty { get; set; }
#pragma warning restore CS8618
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
