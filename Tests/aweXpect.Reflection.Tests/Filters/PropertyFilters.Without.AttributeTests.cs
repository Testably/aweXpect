using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForPropertiesWithoutAttribute()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Without<BarAttribute>();

				await That(properties).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetProperty(nameof(Dummy.PlainProperty)))
					.And.DoesNotContain(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)))
					.And.DoesNotContain(typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo("properties without PropertyFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldFilterForPropertiesWithoutAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Without<BarAttribute>().Directly();

				await That(properties)
					.Contains(typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty)))
					.And.DoesNotContain(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo("properties without direct PropertyFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WithPredicate_ShouldFilterForPropertiesWithoutMatchingAttribute()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Without<BarAttribute>(bar => bar.Value == 2);

				await That(properties)
					.Contains(typeof(Dummy).GetProperty(nameof(Dummy.PlainProperty)))
					.And.Contains(typeof(Dummy).GetProperty(nameof(Dummy.OtherBarProperty)))
					.And.DoesNotContain(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)))
					.And.DoesNotContain(typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo("properties without PropertyFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenDirectly_ShouldFilterForPropertiesWithoutMatchingAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Without<BarAttribute>(bar => bar.Value == 2).Directly();

				await That(properties)
					.Contains(typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty)))
					.And.DoesNotContain(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties without direct PropertyFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Properties properties = In.Type<Dummy>().Properties()
					.Which(property => property.Name == nameof(Dummy.MyBarProperty))
					.Without<BarAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(properties).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that properties matching property => property.Name == nameof(Dummy.MyBarProperty) without PropertyFilters.Without.AttributeTests.BarAttribute matching _ => throw new InvalidOperationException("boom") in type PropertyFilters.Without.AttributeTests.Dummy
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class BarAttribute : Attribute
			{
				public int Value { get; set; }
			}

			private class Dummy
			{
				[Bar(Value = 2)] public virtual int MyBarProperty { get; set; }

				[Bar(Value = 3)] public int OtherBarProperty { get; set; }

				public int PlainProperty { get; set; }
			}

			private class DummyChild : Dummy
			{
				public override int MyBarProperty { get; set; }
			}
		}
	}
}
