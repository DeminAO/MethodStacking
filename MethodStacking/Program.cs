using System.Threading.Tasks;

namespace MethodStacking
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var vm = new SameViewModel();

			await vm.DoSomethingAsync();
		}
	}
}
