using System.Text;
using aweXpect.Formatting;

namespace Snippets
{
	internal sealed class MyValueFormatter : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options) => false;
	}
}

// The page tells to declare this attribute on targets that miss it, and on the others it only causes a warning.
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class ModuleInitializerAttribute : Attribute;
}
