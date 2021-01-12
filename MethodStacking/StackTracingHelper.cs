using System;
using System.Threading.Tasks;

namespace MethodStacking
{
	public class StackTracingHelper
	{
		private readonly StackTracingHelper firstNode;

		private Func<Task> method;
		private StackTracingHelper next;
		private Action<Exception> onException;
		private bool useCatching;

		public StackTracingHelper()
		{
			firstNode = this;
		}

		private StackTracingHelper(StackTracingHelper first)
		{
			this.firstNode = first;
		}

		public StackTracingHelper Do(Func<Task> method)
		{
			this.method = method;
			return this;
		}

		public StackTracingHelper Catch(Func<Task> method, Action<Exception> onException)
		{
			this.method = method;
			this.onException = onException;
			useCatching = true;

			return this;
		}

		public StackTracingHelper Next
		{
			get
			{
				return next ??= new StackTracingHelper(firstNode);
			}
		}

		public Task Bind()
		{
			next = null;
			return firstNode.BindFirst();
		}

		private async Task BindFirst()
		{
			if (useCatching)
			{
				await BindCatchFirst(onException);
				return;
			}
			await (method?.Invoke() ?? Task.CompletedTask);
			await (next?.BindFirst() ?? Task.FromResult(Task.CompletedTask));
		}

		/// <summary>
		/// принудительная обертка в try-catch для последнего бинда
		/// </summary>
		/// <param name="onException"></param>
		/// <returns></returns>
		public Task<Task> BindCatch(Action<Exception> onException)
		{
			next = null;
			return firstNode.BindCatchFirst(onException);
		}

		private async Task<Task> BindCatchFirst(Action<Exception> onException)
		{
			try
			{
				await (method?.Invoke() ?? Task.CompletedTask);
			}
			catch (Exception e)
			{
				onException?.Invoke(e);
			}

			return next?.BindFirst() ?? Task.CompletedTask;
		}
	}

}
