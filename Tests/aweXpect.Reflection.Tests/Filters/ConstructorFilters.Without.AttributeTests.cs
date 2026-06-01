using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithoutAttribute()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().Without<BarAttribute>();

				await That(constructors).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetConstructor(Type.EmptyTypes))
					.And.DoesNotContain(typeof(Dummy).GetConstructor([typeof(string),]));
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without ConstructorFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class BarAttribute : Attribute
			{
			}

			// ReSharper disable UnusedMember.Local
			private class Dummy
			{
				public Dummy()
				{
				}

				[Bar]
				// ReSharper disable once UnusedParameter.Local
				public Dummy(string value)
				{
				}
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
