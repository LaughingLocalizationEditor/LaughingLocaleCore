using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactiveHistory;
using System.Reflection;
using System.Windows.Input;
using System.Collections.Generic;
using LaughingLocale.Data.History;
using LaughingLocale.Data;

namespace LaughingLocale.ViewModel
{
	public interface IHistoryViewModel
	{
		IHistory History { get; }

		void Undo();
		void Redo();
	}

	public static class HistoryViewModelExtensions
	{
		public static void ChangeListWithHistory<T>(this IHistoryViewModel vm, IList<T> source, IList<T> oldValue, IList<T> newValue)
		{
			if (vm.History != null)
			{
				void undo() => source = oldValue;
				void redo() => source = newValue;
				vm.History.Snapshot(undo, redo);
				source = newValue;
			}
		}

		public static void AddWithHistory<T>(this IHistoryViewModel vm, IList<T> source, T item)
		{
			if (vm.History != null)
			{
				int index = source.Count;
				void redo() => source.Insert(index, item);
				void undo() => source.RemoveAt(index);
				vm.History.Snapshot(undo, redo);
				redo();
			}
		}

		public static void RemoveWithHistory<T>(this IHistoryViewModel vm, IList<T> source, T item)
		{
			if(vm.History != null)
			{
				int index = source.IndexOf(item);
				void redo() => source.RemoveAt(index);
				void undo() => source.Insert(index, item);
				vm.History.Snapshot(undo, redo);
				redo();
			}
		}

		public static void CreateSnapshot(this IHistoryViewModel vm, Action undo, Action redo)
		{
			vm.History?.Snapshot(undo, redo);
		}
	}

	public abstract class HistoryViewModel : BaseHistoryData, IDisposable, IHistoryViewModel
	{
		private CompositeDisposable Disposable { get; set; }

		private ICommand UndoCommand { get; set; }
		private ICommand RedoCommand { get; set; }
		private ICommand ClearCommand { get; set; }

		public void Dispose()
		{
			this.Disposable.Dispose();
		}

		public void Undo()
		{
			History.Undo();
		}

		public void Redo()
		{
			History.Redo();
		}

		public HistoryViewModel()
		{
			Disposable = new CompositeDisposable();

			var history = new StackHistory().AddTo(Disposable);
			History = history;

			var undo = new ReactiveCommand(History.CanUndo, false);
			undo.Subscribe(_ => Undo()).AddTo(this.Disposable);
			UndoCommand = undo;

			var redo = new ReactiveCommand(History.CanRedo, false);
			redo.Subscribe(_ => Redo()).AddTo(this.Disposable);
			RedoCommand = redo;

			var clear = new ReactiveCommand(History.CanClear, false);
			clear.Subscribe(_ => History.Clear()).AddTo(this.Disposable);
			ClearCommand = clear;
		}
	}
}
