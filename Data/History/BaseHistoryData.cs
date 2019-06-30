using ReactiveHistory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LaughingLocale.Data.History
{
	public interface IHistoryData
	{
		IHistory History { get; set; }
		void SetHistory(ref IHistory history);
		void Snapshot(Action undo, Action redo);
	}

	public abstract class BaseHistoryData : BaseData, IHistoryData
	{
		public IHistory History { get; set; }

		public void SetHistory(ref IHistory history)
		{
			History = history;
		}

		public virtual void Snapshot(Action undo, Action redo)
		{
			History.Snapshot(undo, redo);
		}

		public bool UpdateWithHistory<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (!Equals(field, value))
			{
				if (History != null)
				{
					var undoValue = field;
					var redoValue = value;

					History.Snapshot(() =>
					{
						this.SetProperty(this, propertyName, undoValue, true);
					}, () =>
					{
						this.SetProperty(this, propertyName, redoValue, true);
					});
				}

				field = value;
				Notify(propertyName);
				return true;
			}
			return false;
		}

		public bool UpdateWithHistory<T>(ref T field, T value, Action undo, Action redo, [CallerMemberName] string propertyName = null)
		{
			if (!Equals(field, value))
			{
				if (History != null)
				{
					History.Snapshot(undo, redo);
				}

				field = value;
				if (propertyName != null) Notify(propertyName);
				return true;
			}
			return false;
		}

		/*
		public bool UpdateWithHistoryForProperty<T>(T value, object targetObject, string targetPropertyName, string updatePropertyName = "")
		{
			var propVal = GetPropertyValue<T>(targetObject, targetPropertyName, targetObject.GetType());
			if (propVal != null && !Equals(propVal, value))
			{
				if (History != null)
				{
					var undoValue = propVal;
					var redoValue = value;

					History.Snapshot(() =>
					{
						this.SetProperty(targetObject, propertyName, undoValue, notify);
					}, () =>
					{
						this.SetProperty(targetObject, propertyName, redoValue, notify);
					});
				}
				return true;
			}
			return false;
		}

		public bool UpdateWithHistoryForField<T>(T value, object targetObject, string fieldName)
		{
			var fieldVal = GetFieldValue<T>(targetObject, fieldName, targetObject.GetType());
			if (fieldVal != null && !Equals(fieldVal, value))
			{
				if (History != null)
				{
					var undoValue = fieldVal;
					var redoValue = value;

					History.Snapshot(() =>
					{
						this.SetField(fieldName, undoValue, propertyName, true);
					}, () =>
					{
						this.SetField(fieldName, redoValue, propertyName, true);
					});
				}

				field = value;
				Notify(propertyName);
				return true;
			}
			return false;
		}

		private T GetFieldValue<T>(object targetObject, string memberName, Type objectType)
		{
			var fieldInfo = objectType.GetField(memberName);
			if (fieldInfo != null)
			{
				return (T)fieldInfo.GetValue(targetObject);
			}
			return default(T);
		}

		private T GetPropertyValue<T>(object targetObject, string memberName, Type objectType)
		{
			var propInfo = objectType.GetProperty(memberName);
			if (propInfo != null)
			{
				return (T)propInfo.GetValue(targetObject);
			}
			return default(T);
		}
		*/

		private bool SetProperty<T>(object targetObject, string propertyName, T value, bool notify = true)
		{
			var prop = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
			if (prop != null && prop.CanWrite)
			{
				prop.SetValue(this, value);
				if (notify) Notify(propertyName);
				return true;
			}
			return false;
		}

		private bool SetField<T>(string fieldName, T value, string propertyName = null, bool notify = true)
		{
			var field = this.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (field != null)
			{
				field.SetValue(this, value);
				if (notify && propertyName != null) Notify(propertyName);
				return true;
			}
			return false;
		}
		
	}
}
