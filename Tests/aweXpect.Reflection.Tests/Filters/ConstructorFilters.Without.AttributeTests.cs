using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
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

			[Test]
			public async Task WithPredicate_ShouldFilterForConstructorsWithoutMatchingAttribute()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().Without<BarAttribute>(bar => bar.Value == 2);

				await That(constructors)
					.Contains(typeof(Dummy).GetConstructor(Type.EmptyTypes))
					.And.Contains(typeof(Dummy).GetConstructor([typeof(int),]))
					.And.DoesNotContain(typeof(Dummy).GetConstructor([typeof(string),]));
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors without ConstructorFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Constructors constructors = In.Type<Dummy>().Constructors()
					.Which(constructor => constructor.GetParameters().Length == 1)
					.Without<BarAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(constructors).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors matching constructor => constructor.GetParameters().Length == 1 without ConstructorFilters.Without.AttributeTests.BarAttribute matching _ => throw new InvalidOperationException("boom") in type ConstructorFilters.Without.AttributeTests.Dummy
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class BarAttribute : Attribute
			{
				public int Value { get; set; }
			}

			// ReSharper disable UnusedMember.Local
			private class Dummy
			{
				public Dummy()
				{
				}

				[Bar(Value = 2)]
				// ReSharper disable once UnusedParameter.Local
				public Dummy(string value)
				{
				}

				[Bar(Value = 3)]
				// ReSharper disable once UnusedParameter.Local
				public Dummy(int value)
				{
				}
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
