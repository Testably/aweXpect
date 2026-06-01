using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWithoutAttribute()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().Without<BarAttribute>();

				await That(methods).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetMethod(nameof(Dummy.PlainMethod)))
					.And.DoesNotContain(typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)))
					.And.DoesNotContain(typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod)));
				await That(methods.GetDescription())
					.IsEqualTo("methods without MethodFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForMethodsWithoutAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().Without<BarAttribute>(false);

				await That(methods)
					.Contains(typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod)))
					.And.DoesNotContain(typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)));
				await That(methods.GetDescription())
					.IsEqualTo("methods without direct MethodFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class BarAttribute : Attribute
			{
			}

			private class Dummy
			{
				[Bar]
				public virtual void MyBarMethod()
				{
				}

				public void PlainMethod()
				{
				}
			}

			private class DummyChild : Dummy
			{
				public override void MyBarMethod()
				{
				}
			}
		}
	}
}
