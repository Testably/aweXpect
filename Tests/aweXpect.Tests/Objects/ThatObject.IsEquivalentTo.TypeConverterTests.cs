using System.ComponentModel;
using System.Globalization;
using System.Text;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class TypeConverterTests
		{
			[Test]
			public async Task WhenActualIsComparedByContent_ShouldCompareTheContent()
			{
				var subject = new { Title = new StringBuilder("Abbey Road"), };
				var expected = new { Title = new Label("Abbey Road"), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the converted StringBuilder is compared by its text, like any other StringBuilder");
			}

			[Test]
			public async Task WhenActualIsString_ShouldNotConvert()
			{
				var subject = new { Released = "1969-09-26", };
				var expected = new { Released = new ReleaseDate(1969, 9, 26), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Released differed:
					                   Actual: "1969-09-26"
					                 Expected: ThatObject.IsEquivalentTo.TypeConverterTests.ReleaseDate { Value = 1969-09-26T00:00:00.0000000 }

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenComparingPredicateMeetsTheConvertedTypes_ShouldUseThePredicate()
			{
				var subject = new { Released = new DateTime(1969, 9, 26, 0, 0, 1), };
				var expected = new { Released = new ReleaseDate(1969, 9, 26), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<DateTime, DateTime>((a, e) => (a - e).Duration() < TimeSpan.FromSeconds(5)));

				await That(Act).DoesNotThrow()
					.Because("the converted value is compared like any other value of the actual type");
			}

			[Test]
			public async Task WhenComparingPredicateMeetsTheTypes_ShouldPreferThePredicate()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = new ReleaseDate(1970, 1, 1), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<DateTime, ReleaseDate>((actual, _) => actual.Year == 1969));

				await That(Act).DoesNotThrow()
					.Because("an explicit predicate for the pair of types decides before the converter");
			}

			[Test]
			public async Task WhenComparingPredicateOfTheConvertedTypesFails_ShouldShowTheExpectedValueAsWritten()
			{
				var subject = new { Released = new DateTime(1969, 9, 26, 0, 0, 10), };
				var expected = new { Released = new ReleaseDate(1969, 9, 26), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<DateTime, DateTime>((a, e) => (a - e).Duration() < TimeSpan.FromSeconds(5)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Released did not match (a, e) => (a - e).Duration() < TimeSpan.FromSeconds(5):
					                   Actual: 1969-09-26T00:00:10.0000000
					                 Expected: ThatObject.IsEquivalentTo.TypeConverterTests.ReleaseDate { Value = 1969-09-26T00:00:00.0000000 }

					             Equivalency options:
					              - include public fields and properties
					              - compare DateTime with DateTime by (a, e) => (a - e).Duration() < TimeSpan.FromSeconds(5)
					             """);
			}

			[Test]
			public async Task WhenConvertedValueDiffers_ShouldFail()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = new ReleaseDate(1969, 10, 31), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Released differed:
					                   Actual: 1969-09-26T00:00:00.0000000
					                 Expected: ThatObject.IsEquivalentTo.TypeConverterTests.ReleaseDate { Value = 1969-10-31T00:00:00.0000000 }

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenConvertedValueMatches_ShouldSucceed()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = new ReleaseDate(1969, 9, 26), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the expected value is converted to the type of the actual value by its declared converter");
			}

			[Test]
			public async Task WhenConverterCannotConvertToTheActualType_ShouldNotConvert()
			{
				var subject = new { Released = 1969, };
				var expected = new { Released = new ReleaseDate(1969, 9, 26), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Released differed:
					                   Actual: 1969
					                 Expected: ThatObject.IsEquivalentTo.TypeConverterTests.ReleaseDate { Value = 1969-09-26T00:00:00.0000000 }

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenConverterConstructorThrows_ShouldFailWithTheException()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = new UnbuildableReleaseDate(), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but the type converter of ThatObject.IsEquivalentTo.TypeConverterTests.UnbuildableReleaseDate did throw an InvalidOperationException:
					               The converter cannot be built.

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenConverterThrows_ShouldFailWithTheException()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = new FaultyReleaseDate(), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but the type converter of ThatObject.IsEquivalentTo.TypeConverterTests.FaultyReleaseDate did throw an InvalidOperationException:
					               The release date is unknown.

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenReflectedConverterConstructorThrows_ShouldFailWithTheException()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = (object)new UnbuildableReflectedReleaseDate(), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but the type converter of ThatObject.IsEquivalentTo.TypeConverterTests.UnbuildableReflectedReleaseDate did throw an InvalidOperationException:
					               The converter cannot be built.

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenReflectedConverterIsAbstract_ShouldNotConvert()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = (object)new AbstractReleaseDate(), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Released differed:
					                   Actual: 1969-09-26T00:00:00.0000000
					                 Expected: ThatObject.IsEquivalentTo.TypeConverterTests.AbstractReleaseDate { }

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenTypeIsOnlyReachedAsObject_ShouldFindTheConverterByReflection()
			{
				var subject = new { Released = new DateTime(1969, 9, 26), };
				var expected = new { Released = (object)new ReflectedReleaseDate(1969, 9, 26), };

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the source generator cannot see a type behind a member typed as object");
			}

			[TypeConverter(typeof(ReleaseDateConverter))]
			public readonly struct ReleaseDate(int year, int month, int day)
			{
				public DateTime Value { get; } = new(year, month, day);
			}

			public sealed class ReleaseDateConverter : TypeConverter
			{
				public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
					=> destinationType == typeof(DateTime) || base.CanConvertTo(context, destinationType);

				public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture,
					object? value, Type destinationType)
					=> value is ReleaseDate releaseDate && destinationType == typeof(DateTime)
						? releaseDate.Value
						: base.ConvertTo(context, culture, value, destinationType);
			}

			[TypeConverter(typeof(ReflectedReleaseDateConverter))]
			public readonly struct ReflectedReleaseDate(int year, int month, int day)
			{
				public DateTime Value { get; } = new(year, month, day);
			}

			public sealed class ReflectedReleaseDateConverter : TypeConverter
			{
				public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
					=> destinationType == typeof(DateTime) || base.CanConvertTo(context, destinationType);

				public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture,
					object? value, Type destinationType)
					=> value is ReflectedReleaseDate releaseDate && destinationType == typeof(DateTime)
						? releaseDate.Value
						: base.ConvertTo(context, culture, value, destinationType);
			}

			[TypeConverter(typeof(FaultyReleaseDateConverter))]
			public readonly struct FaultyReleaseDate;

			public sealed class FaultyReleaseDateConverter : TypeConverter
			{
				public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
					=> throw new InvalidOperationException("The release date is unknown.");
			}

			[TypeConverter(typeof(UnbuildableConverter))]
			public readonly struct UnbuildableReleaseDate;

			[TypeConverter(typeof(UnbuildableConverter))]
			public readonly struct UnbuildableReflectedReleaseDate;

			public sealed class UnbuildableConverter : TypeConverter
			{
				public UnbuildableConverter()
				{
					throw new InvalidOperationException("The converter cannot be built.");
				}
			}

			[TypeConverter(typeof(AbstractConverter))]
			public readonly struct AbstractReleaseDate;

			public abstract class AbstractConverter : TypeConverter
			{
				public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
					=> destinationType == typeof(DateTime) || base.CanConvertTo(context, destinationType);
			}

			[TypeConverter(typeof(LabelConverter))]
			public sealed class Label(string text)
			{
				public string Text { get; } = text;
			}

			public sealed class LabelConverter : TypeConverter
			{
				public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
					=> destinationType == typeof(StringBuilder) || base.CanConvertTo(context, destinationType);

				public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture,
					object? value, Type destinationType)
					=> value is Label label && destinationType == typeof(StringBuilder)
						? new StringBuilder(label.Text)
						: base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
