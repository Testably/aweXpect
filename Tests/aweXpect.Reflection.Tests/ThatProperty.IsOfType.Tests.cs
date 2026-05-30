using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsOfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsOfType<int>();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is of type int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsOfDifferentType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsOfExpectedType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyTypeInheritsFromExpectedType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task WhenPropertyIsNoneOfTheTypes_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfExactType<bool>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string or of exact type bool,
					             but it was of type int
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsOneOfTheTypes_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfExactType<int>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsOfDifferentType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfType<string>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsOfExpectedType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfType<int>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not of type int,
					             but it did
					             """);
			}
		}

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
			public int IntProperty { get; set; }
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
			public async Task WhenPropertyIsOfDifferentType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType(typeof(string));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsOfExpectedType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task WhenPropertyIsNoneOfTheTypes_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfType<bool>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string or of type bool,
					             but it was of type int
					             """);
			}

			[Fact]
			public async Task WithMultipleOrOfType_ShouldSupportChaining()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfType(typeof(bool)).OrOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#pragma warning restore CA2263
	}
}
