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

namespace LaughingLocale.ViewModel
{
	public interface IHistoryViewModel
	{
		IHistory History { get; set; }

		void Undo();
		void Redo();
	}

	public abstract class HistoryViewModel : IDisposable
	{
		private CompositeDisposable Disposable { get; set; }

		public IHistory History { get; private set; }

		private ICommand UndoCommand { get; set; }
		private ICommand RedoCommand { get; set; }
		private ICommand ClearCommand { get; set; }

		public void CreateSnapshot(Action undo, Action redo)
		{
			History.Snapshot(undo, redo);
		}

		public void ChangeListWithHistory<T>(IList<T> source, IList<T> oldValue, IList<T> newValue)
		{
			void undo() => source = oldValue;
			void redo() => source = newValue;
			History.Snapshot(undo, redo);
			source = newValue;
		}

		public void AddWithHistory<T>(IList<T> source, T item)
		{
			int index = source.Count;
			void redo() => source.Insert(index, item);
			void undo() => source.RemoveAt(index);
			History.Snapshot(undo, redo);
			redo();
		}

		public void RemoveWithHistory<T>(IList<T> source, T item)
		{
			int index = source.IndexOf(item);
			void redo() => source.RemoveAt(index);
			void undo() => source.Insert(index, item);
			History.Snapshot(undo, redo);
			redo();
		}

		public void Undo()
		{
			History.Undo();
		}

		public void Redo()
		{
			History.Redo();
		}

		public void Dispose()
		{
			this.Disposable.Dispose();
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
