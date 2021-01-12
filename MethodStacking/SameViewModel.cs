using System;
using System.Threading.Tasks;

namespace MethodStacking
{
	/// <summary>
	/// пример вю-модели
	/// </summary>
	public class SameViewModel
	{
		/// <summary>
		/// Индикатор загрузки
		/// </summary>
		public bool indicator;

		/// <summary>
		/// Пример метода 
		/// </summary>
		public Task DoSomethingAsync() =>
			new StackTracingHelper()
			// установка индикатора в true
			.Do(() => Task.Run(() =>
				{
					indicator = true;
					Console.WriteLine($"indicator = {indicator}");
				}))
			// основное действо
			.Next.Catch(() => Task.Run(async ()
				=>
			{
				Console.WriteLine("delay 500");
				await Task.Delay(500);
				Console.WriteLine("DoSomethingAsync body");

				return Task.CompletedTask;
			}),
				e => { /* обработчик ошибок, если надо */ }
			)
			// установка индикатора в false
			.Next.Do(() => Task.Run(() =>
			{
				indicator = false;
				Console.WriteLine($"indicator = {indicator}");
			}))
			// запуск цепочки
			.Bind();
	}

}
