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
			[Arguments(1)]
			[Arguments(2)]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldExcludeMethod(int value)
			{
				Filtered.Methods methods = In.Type<Dummy>().Methods()
					.Without<MultiAttribute>(multi => multi.Value == value);
				Filtered.Methods notMatchingMethods = In.Type<Dummy>().Methods()
					.Without<MultiAttribute>(multi => multi.Value == 3);

				await That(methods).DoesNotContain(typeof(Dummy).GetMethod(nameof(Dummy.MultiMethod)))
					.Because("every attribute of the method counts, not only the first one");
				await That(notMatchingMethods).Contains(typeof(Dummy).GetMethod(nameof(Dummy.MultiMethod)))
					.Because("a method whose attributes all do not match the predicate is kept");
			}

			[Test]
			public async Task WithPredicate_WhenOnlyOverriddenMethodHasMatchingAttribute_ShouldExcludeOverrideUnlessDirectly()
			{
				Filtered.Methods methods = In.Type<DummyChild>().Methods()
					.Without<MultiAttribute>(multi => multi.Value == 1);
				Filtered.Methods directMethods = In.Type<DummyChild>().Methods()
					.Without<MultiAttribute>(multi => multi.Value == 1).Directly();

				await That(methods).DoesNotContain(typeof(DummyChild).GetMethod(nameof(DummyChild.MyMultiMethod)))
					.Because("the attribute of the overridden base method is inherited");
				await That(directMethods).Contains(typeof(DummyChild).GetMethod(nameof(DummyChild.MyMultiMethod)))
					.Because("with Directly only the attribute declared on the overriding method counts");
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

			[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
			private class MultiAttribute : Attribute
			{
				public int Value { get; set; }
			}

			private class Dummy
			{
				[Bar(Value = 2)]
				public virtual void MyBarMethod()
				{
				}

				[Multi(Value = 1)]
				public virtual void MyMultiMethod()
				{
				}

				[Multi(Value = 1)]
				[Multi(Value = 2)]
				public static void MultiMethod()
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

				[Multi(Value = 2)]
				public override void MyMultiMethod()
				{
				}
			}
		}
	}
}
