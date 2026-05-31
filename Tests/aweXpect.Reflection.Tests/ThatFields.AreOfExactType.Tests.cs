using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreOfExactType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFailWhenFieldsInheritFromType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type *DummyBase,
					             but it contained not matching fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllFieldsAreOfExactType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
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
			public async Task WhenAllFieldsAreOneOfTheExactTypes_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>().OrOfExactType<Dummy>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldStrictlyInheritsFromOrOfExactType_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<int>().OrOfExactType<DummyBase>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type int or of exact type *DummyBase,
					             but it contained not matching fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task ShouldSucceedWhenAllFieldsAreOfExactType()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldFailWhenFieldsInheritFromType()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type *DummyBase,
					             but it contained not matching fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public DummyBase DummyBaseField;
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
	}
}
