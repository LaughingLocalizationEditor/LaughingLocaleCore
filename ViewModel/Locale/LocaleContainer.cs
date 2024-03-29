﻿using LaughingLocale.Data.History;
using LaughingLocale.Data.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaughingLocale.ViewModel.Locale
{
	public interface ILocaleContainer : ICommittable
	{
		List<ILocaleData> Entries { get; set; }

		string Source { get; set; }
		string Name { get; set; }
		string DisplayName { get; }

		bool Active { get; set; }

		bool AllSelected { get; set; }

		bool Locked { get; set; }

		void SelectAll();
		void SelectNone();

		void Add(ILocaleData entry);
		void Add(IList<ILocaleData> entries);
	}

	public class LocaleContainer : HistoryViewModel, ILocaleContainer
	{
		public List<ILocaleData> Entries { get; set; } = new List<ILocaleData>();

		private string source;

		public string Source
		{
			get => source;
			set
			{
				Update(ref source, value);
			}
		}

		private string name;

		public string Name
		{
			get { return name; }
			set
			{
				Update(ref name, value);
				UpdateDisplayName();
			}
		}

		private string displayName;

		public string DisplayName
		{
			get { return displayName; }
			private set
			{
				Update(ref displayName, value);
			}
		}

		private void UpdateDisplayName()
		{
			DisplayName = !ChangesUncommitted ? Name : "*" + Name;
		}

		private bool active = false;

		public bool Active
		{
			get { return active; }
			set
			{
				Update(ref active, value);
			}
		}

		private bool locked = false;

		public bool Locked
		{
			get { return locked; }
			set
			{
				Update(ref locked, value);
			}
		}

		private bool changesUncommitted = false;

		public bool ChangesUncommitted
		{
			get { return changesUncommitted; }
			set
			{
				Update(ref changesUncommitted, value);
				UpdateDisplayName();
			}
		}

		public virtual void CommitChanges()
		{
			ChangesUncommitted = false;
		}

		public void SelectAll()
		{
			foreach (var entry in Entries) { entry.Selected = true; }
		}

		public void SelectNone()
		{
			foreach (var entry in Entries) { entry.Selected = false; }
		}

		private bool allSelected;

		public bool AllSelected
		{
			get { return allSelected; }
			set
			{
				Update(ref allSelected, value);
				if (allSelected)
					SelectAll();
				else
					SelectNone();
			}
		}

		public virtual void Add(ILocaleData entry)
		{
			entry.Parent = this;
			this.AddWithHistory(Entries, entry);
		}

		public virtual void Add(IList<ILocaleData> entries)
		{
			var current = Entries.ToList();
			foreach(var entry in entries)
			{
				entry.Parent = this;
			}
			var next = Entries.Concat(entries).ToList();
			this.ChangeListWithHistory(entries, current, next);
		}
	}
}
