using System.Text.Json;
using aweXpect.Customization;

namespace aweXpect.Json.Tests;

public sealed class CustomizeJsonTests
{
	[Fact]
	public async Task SetDefaultJsonDocumentOptions_ShouldApplyOptionsWithinScope()
	{
		string jsonWithTrailingCommas = "[1, 2,]";
		string jsonWithoutTrailingCommas = "[1, 2]";

		async Task Act()
			=> await That(jsonWithTrailingCommas).IsEqualTo(jsonWithoutTrailingCommas).AsJson();

		using (IDisposable __ = Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
		       {
			       // Default options set AllowTrailingCommas to true
			       AllowTrailingCommas = false,
		       }))
		{
			await That(Act).Throws()
				.WithMessage("""
				             Expected that jsonWithTrailingCommas
				             is JSON equivalent to [1, 2],
				             but it could not be parsed as JSON: The JSON array contains a trailing comma at the end which is not supported in this mode. Change the reader options. LineNumber: 0 | BytePositionInLine: 6.

				             Actual:
				             [1, 2,]
				             
				             Expected:
				             [1, 2]
				             """);
		}

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task GlobalValue_ShouldBeVisibleWhenAnotherValueIsSetInTheAsyncFlow()
	{
		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions()))
		{
			using (Customize.aweXpect.Global.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
			       {
				       AllowTrailingCommas = true,
				       MaxDepth = 64,
			       }))
			{
				await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().MaxDepth).IsEqualTo(64)
					.Because("a value set in the async flow must not hide a global value of another setting");
			}
		}
	}

	[Fact]
	public async Task OutOfOrderDisposal_ShouldRestoreEachValueIndependently()
	{
		JsonSerializerOptions serializerOptions = new();
		CustomizationLifetime serializerLifetime =
			Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(serializerOptions);
		CustomizationLifetime documentLifetime =
			Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
			{
				MaxDepth = 3,
			});

		serializerLifetime.Dispose();

		await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().MaxDepth).IsEqualTo(3)
			.Because("disposing the serializer options must keep the document options that were set later");
		await That(Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get()).IsNotSameAs(serializerOptions);

		documentLifetime.Dispose();

		await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().MaxDepth).IsEqualTo(0);
		await That(Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get()).IsNotSameAs(serializerOptions)
			.Because("disposing the document options must not restore the already disposed serializer options");
	}

	[Fact]
	public async Task SetDefaultJsonSerializerOptions_WithNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'value' cannot be null.*").AsWildcard();
	}

	[Fact]
	public async Task ShouldChangeIndividualProperties()
	{
		await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas)
			.IsTrue();
		await That(Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get().AllowTrailingCommas)
			.IsTrue();

		using (Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
		       {
			       // Default options set AllowTrailingCommas to true
			       AllowTrailingCommas = false,
		       }))
		{
			await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas)
				.IsFalse();
			await That(Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get().AllowTrailingCommas)
				.IsTrue();
		}

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
		       {
			       // Default options set AllowTrailingCommas to true
			       AllowTrailingCommas = false,
		       }))
		{
			await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas)
				.IsTrue();
			await That(Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get().AllowTrailingCommas)
				.IsFalse();
		}

		await That(Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas)
			.IsTrue();
		await That(Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get().AllowTrailingCommas)
			.IsTrue();
	}
}
