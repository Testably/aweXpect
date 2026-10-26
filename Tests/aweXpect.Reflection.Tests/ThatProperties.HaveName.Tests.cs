using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyInfosContainPropertyInfoWithDifferentName_ShouldFail()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithProperties>().Properties();

				async Task Act()
				{
					await That(subject).HaveName("PublicProperty");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that properties in types matching t => t == typeof(T) in assembly containing type ThatProperty.ClassWithProperties
					             all have name equal to "PublicProperty",
					             but only 1 of 6 did

					             Not matching items:
					             [
					               internal int ThatProperty.ClassWithProperties.InternalProperty { get; set; },
					               protected int ThatProperty.ClassWithProperties.ProtectedProperty { get; set; },
					               private int ThatProperty.ClassWithProperties.PrivateProperty { get; set; },
					               protected internal int ThatProperty.ClassWithProperties.ProtectedInternalProperty { get; set; },
					               private protected int ThatProperty.ClassWithProperties.PrivateProtectedProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperty.ClassWithProperties.PublicProperty { get; set; },
					               internal int ThatProperty.ClassWithProperties.InternalProperty { get; set; },
					               protected int ThatProperty.ClassWithProperties.ProtectedProperty { get; set; },
					               private int ThatProperty.ClassWithProperties.PrivateProperty { get; set; },
					               protected internal int ThatProperty.ClassWithProperties.ProtectedInternalProperty { get; set; },
					               private protected int ThatProperty.ClassWithProperties.PrivateProtectedProperty { get; set; }
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfosHaveName_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithSingleProperty>().Properties();

				async Task Act()
				{
					await That(subject).HaveName("MyProperty");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithSingleProperty>().Properties();

				async Task Act()
				{
					await That(subject).HaveName("mYpROPERTY").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithProperties>().Properties();

				async Task Act()
				{
					await That(subject).HaveName("Property").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertiesDoNotHaveName_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithProperties>().Properties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentProperty"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesHaveName_ShouldFail()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithSingleProperty>().Properties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("MyProperty"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that properties in types matching t => t == typeof(T) in assembly containing type ThatProperty.ClassWithSingleProperty
					             not all have name equal to "MyProperty",
					             but all 1 did

					             Collection:
					             [
					               public int ThatProperty.ClassWithSingleProperty.MyProperty { get; set; }
					             ]
					             """).AsWildcard();
			}
		}
	}
}
