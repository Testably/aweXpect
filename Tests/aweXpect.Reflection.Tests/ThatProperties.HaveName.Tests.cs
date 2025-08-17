using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfosContainPropertyInfoWithDifferentName_ShouldFail()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithProperties>().Properties();

				async Task Act()
					=> await That(subject).HaveName("PublicProperty");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that properties in types matching t => t == typeof(T) in assembly containing type ThatProperty.ClassWithProperties
					             all have name equal to "PublicProperty",
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertyInfosHaveName_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithSingleProperty>().Properties();

				async Task Act()
					=> await That(subject).HaveName("MyProperty");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithSingleProperty>().Properties();

				async Task Act()
					=> await That(subject).HaveName("mYpROPERTY").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithProperties>().Properties();

				async Task Act()
					=> await That(subject).HaveName("Property").AsSuffix();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertiesDoNotHaveName_ShouldSucceed()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithProperties>().Properties();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentProperty"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesHaveName_ShouldFail()
			{
				Filtered.Properties subject = GetTypes<ThatProperty.ClassWithSingleProperty>().Properties();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveName("MyProperty"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that properties in types matching t => t == typeof(T) in assembly containing type ThatProperty.ClassWithSingleProperty
					             not all have name equal to "MyProperty",
					             but it only contained matching items *
					             """).AsWildcard();
			}
		}
	}
}
