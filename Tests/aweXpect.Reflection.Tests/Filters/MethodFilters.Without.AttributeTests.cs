using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
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

			[Test]
			public async Task WhenDirectly_ShouldFilterForMethodsWithoutAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().Without<BarAttribute>().Directly();

				await That(methods)
					.Contains(typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod)))
					.And.DoesNotContain(typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)));
				await That(methods.GetDescription())
					.IsEqualTo("methods without direct MethodFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WithPredicate_ShouldFilterForMethodsWithoutMatchingAttribute()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().Without<BarAttribute>(bar => bar.Value == 2);

				await That(methods)
					.Contains(typeof(Dummy).GetMethod(nameof(Dummy.PlainMethod)))
					.And.Contains(typeof(Dummy).GetMethod(nameof(Dummy.OtherBarMethod)))
					.And.DoesNotContain(typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)))
					.And.DoesNotContain(typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod)));
				await That(methods.GetDescription())
					.IsEqualTo("methods without MethodFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenDirectly_ShouldFilterForMethodsWithoutMatchingAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().Without<BarAttribute>(bar => bar.Value == 2).Directly();

				await That(methods)
					.Contains(typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod)))
					.And.DoesNotContain(typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)));
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods without direct MethodFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Methods methods = In.Type<Dummy>().Methods()
					.Which(method => method.Name == nameof(Dummy.MyBarMethod))
					.Without<BarAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(methods).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching method => method.Name == nameof(Dummy.MyBarMethod) without MethodFilters.Without.AttributeTests.BarAttribute matching _ => throw new InvalidOperationException("boom") in type MethodFilters.Without.AttributeTests.Dummy
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class BarAttribute : Attribute
			{
				public int Value { get; set; }
			}

			private class Dummy
			{
				[Bar(Value = 2)]
				public virtual void MyBarMethod()
				{
				}

				[Bar(Value = 3)]
				public static void OtherBarMethod()
				{
				}

				public static void PlainMethod()
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
