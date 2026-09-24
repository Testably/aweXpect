global using static Snippets.Prelude;
using System.ComponentModel;

namespace Snippets;

internal static class Prelude
{
	public static MyClass subject = new();
}

public class MyViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	public int MyProperty { get; set; }

	public void Execute() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyProperty)));
}
