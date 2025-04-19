using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public static Filtered.Events GetEvents(string eventPrefix)
		=> In.AssemblyContaining<ClassWithEvents>().Types().Which(t => t == typeof(ClassWithEvents))
			.Events().Which(eventInfo => eventInfo.Name.StartsWith(eventPrefix));

#pragma warning disable CS8618
#pragma warning disable CS0067
	public class ClassWithEvents
	{
		public event EventHandler PublicEvent1;
		public event EventHandler PublicEvent2;
		internal event EventHandler InternalEvent1;
		internal event EventHandler InternalEvent2;
		protected event EventHandler ProtectedEvent1;
		protected event EventHandler ProtectedEvent2;
		private event EventHandler PrivateEvent1;
		private event EventHandler PrivateEvent2;
	}
#pragma warning restore CS0067
#pragma warning restore CS8618
	
	public static Filtered.Types GetTypes<T>()
		=> In.AssemblyContaining<T>().Types().Which(t => t == typeof(T));
}
