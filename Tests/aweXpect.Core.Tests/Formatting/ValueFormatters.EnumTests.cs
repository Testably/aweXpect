using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class EnumTests
	{
		[Theory]
		[InlineData(Dummy.Foo, "Foo")]
		[InlineData(Dummy.Bar, "Bar")]
		[InlineData(null, "<null>")]
		public async Task Nullable_ShouldUseStringRepresentation(Dummy? value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[InlineData(Dummy.Foo, "ValueFormatters.EnumTests.Dummy Foo")]
		[InlineData(Dummy.Bar, "ValueFormatters.EnumTests.Dummy Bar")]
		[InlineData(null, "<null>")]
		public async Task Nullable_WithType_ShouldUseStringRepresentation(Dummy? value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task InFailureMessage_WhenCombinedFlagsAreCollectionItems_ShouldKeepThemApartFromTheOtherItems()
		{
			MyFlags[] subject = [MyFlags.A | MyFlags.B, MyFlags.A,];

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [
				                 A | B,
				                 A
				               ]
				             """);
		}

		[Theory]
		[InlineData(MyFlags.A | MyFlags.B, "A | B")]
		[InlineData(MyFlags.A | MyFlags.B | MyFlags.C, "A | B | C")]
		[InlineData(MyFlags.None, "None")]
		[InlineData((MyFlags)8, "8")]
		public async Task ShouldJoinCombinedFlagsLikeInCSharp(MyFlags value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			string withTypeResult = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("a comma reads like the separator of collection items");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(withTypeResult).IsEqualTo($"ValueFormatters.EnumTests.MyFlags {expectedResult}");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[InlineData(Dummy.Foo, "Foo")]
		[InlineData(Dummy.Bar, "Bar")]
		public async Task ShouldUseStringRepresentation(Dummy value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			Dummy? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Theory]
		[InlineData(Dummy.Foo, "ValueFormatters.EnumTests.Dummy Foo")]
		[InlineData(Dummy.Bar, "ValueFormatters.EnumTests.Dummy Bar")]
		public async Task WithType_ShouldUseStringRepresentation(Dummy value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		public enum Dummy
		{
			Foo,
			Bar,
		}

		[Flags]
		public enum MyFlags
		{
			None = 0,
			A = 1,
			B = 2,
			C = 4,
		}
	}
}
