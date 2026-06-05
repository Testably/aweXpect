using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutConstructorsThatSatisfyThePredicate()
			{
				Filtered.Constructors constructors = In.Type<ClassWithConstructors>()
					.Constructors().Except(constructor => constructor.GetParameters().Length == 0);

				await That(constructors).All().Satisfy(c => c!.GetParameters().Length != 0).And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors except constructor => constructor.GetParameters().Length == 0 in")
					.AsPrefix();
			}

			private class ClassWithConstructors
			{
				public ClassWithConstructors()
				{
				}

				// ReSharper disable once UnusedParameter.Local
				public ClassWithConstructors(int value)
				{
				}
			}
		}
	}
}
