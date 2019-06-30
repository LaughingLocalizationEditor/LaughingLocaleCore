using LaughingLocale.Data.History;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LaughingLocale.Data.Locale
{
	public interface ILocaleGroup : ICommittable
	{
		List<ILocaleEntryContainer> Entries { get; set; }

		string Source { get; set; }
		string Name { get; set; }
		string DisplayName { get; }
	}

	public class LocaleGroup : BaseData, ILocaleGroup
	{
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

		private string source;

		public string Source
		{
			get => source;
			set
			{
				Update(ref source, value);
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

		List<ILocaleEntryContainer> Entries { get; set; } = new List<ILocaleEntryContainer>();

		private ILocaleEntryContainer all;

		public ILocaleEntryContainer All
		{
			get { return all; }
			private set
			{
				Update(ref all, value);
			}
		}

		List<ILocaleEntryContainer> DisplayedEntries { get; set; } = new List<ILocaleEntryContainer>();

		public void UpdateCombinedData()
		{
			DisplayedEntries = new List<ILocaleEntryContainer>();

			if (All != null)
			{
				All.Entries = new List<ILocaleData>();
				DisplayedEntries.Add(All);
			}

			Entries = Entries.OrderBy(c => c.Name).ToList();

			foreach (var container in Entries)
			{
				if (All != null) All.Entries.AddRange(container.Entries);
				DisplayedEntries.Add(container);
			}

			if (All != null)
			{
				All.Entries.OrderBy(e => e.Key);
				Notify("All");
			}
			
			Notify("DisplayedEntries");
		}

		public Action<ILocaleGroup, ILocaleEntryContainer> SelectedFileChanged { get; set; }

		private int selectedfileIndex = 0;

		public int SelectedFileIndex
		{
			get { return selectedfileIndex; }
			set
			{
				Update(ref selectedfileIndex, value);
				Notify("SelectedFile");
				SelectedFileChanged?.Invoke(this, SelectedFile);
			}
		}

		public ILocaleEntryContainer SelectedFile
		{
			get
			{
				return SelectedFileIndex > -1 && DisplayedEntries.Count > 0 ? DisplayedEntries[SelectedFileIndex] : null;
			}
		}

		public ICommand UpdateAllCommand { get; set; }

		private bool visibility = true;

		public bool Visibility
		{
			get { return visibility; }
			set
			{
				Update(ref visibility, value);
			}
		}

		public void SelectFirst()
		{
			SelectedFileIndex = 0;
		}

		public void SelectLast()
		{
			SelectedFileIndex = DisplayedEntries.Count - 1;
		}

		public LocaleTabGroup(string name = "")
		{
			Name = name;

			UpdateAllCommand = new Comman(UpdateCombinedData);
		}
	}
}
