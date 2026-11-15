using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForFieldsWithoutAttribute()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().Without<BarAttribute>();

				await That(fields).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetField(nameof(Dummy.PlainField)))
					.And.DoesNotContain(typeof(Dummy).GetField(nameof(Dummy.MyBarField)));
				await That(fields.GetDescription())
					.IsEqualTo("fields without FieldFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WithPredicate_ShouldFilterForFieldsWithoutMatchingAttribute()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().Without<BarAttribute>(bar => bar.Value == 2);

				await That(fields)
					.Contains(typeof(Dummy).GetField(nameof(Dummy.PlainField)))
					.And.Contains(typeof(Dummy).GetField(nameof(Dummy.OtherBarField)))
					.And.DoesNotContain(typeof(Dummy).GetField(nameof(Dummy.MyBarField)));
				await That(fields.GetDescription())
					.IsEqualTo("fields without FieldFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			[Arguments(1)]
			[Arguments(2)]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldExcludeField(int value)
			{
				Filtered.Fields fields = In.Type<Dummy>().Fields()
					.Without<MultiAttribute>(multi => multi.Value == value);
				Filtered.Fields notMatchingFields = In.Type<Dummy>().Fields()
					.Without<MultiAttribute>(multi => multi.Value == 3);

				await That(fields).DoesNotContain(typeof(Dummy).GetField(nameof(Dummy.MultiField)))
					.Because("every attribute of the field counts, not only the first one");
				await That(notMatchingFields).Contains(typeof(Dummy).GetField(nameof(Dummy.MultiField)))
					.Because("a field whose attributes all do not match the predicate is kept");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Fields fields = In.Type<Dummy>().Fields()
					.Which(field => field.Name == nameof(Dummy.MyBarField))
					.Without<BarAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(fields).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that fields matching field => field.Name == nameof(Dummy.MyBarField) without FieldFilters.Without.AttributeTests.BarAttribute matching _ => throw new InvalidOperationException("boom") in type FieldFilters.Without.AttributeTests.Dummy
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class BarAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
			private class MultiAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0649 // Field is never assigned to
			private class Dummy
			{
				[Bar(Value = 2)] public string? MyBarField;

				[Multi(Value = 1)] [Multi(Value = 2)] public string? MultiField;

				[Bar(Value = 3)] public string? OtherBarField;

				public string? PlainField;
			}
#pragma warning restore CS0649
		}
	}
}
