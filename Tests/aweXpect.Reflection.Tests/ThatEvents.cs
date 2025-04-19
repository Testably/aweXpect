using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public static Filtered.Types GetTypes<T>()
		=> In.AssemblyContaining<T>().Types().Which(t => t == typeof(T));
}
