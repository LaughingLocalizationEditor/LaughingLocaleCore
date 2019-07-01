using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LaughingLocale.Data;
using LaughingLocale.Data.History;
using LaughingLocale.Data.Locale;
using LaughingLocale.ViewModel.Locale;
//using Reactive.Bindings;
using ReactiveUI;

namespace LaughingLocale.ViewModel.Locale
{
	public interface ILocaleGroup : ICommittable
	{
		List<ILocaleContainer> Entries { get; set; }

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

		public List<ILocaleContainer> Entries { get; set; } = new List<ILocaleContainer>();

		private ILocaleContainer all;

		public ILocaleContainer All
		{
			get { return all; }
			private set
			{
				Update(ref all, value);
			}
		}

		public List<ILocaleContainer> DisplayedEntries { get; private set; } = new List<ILocaleContainer>();

		public void UpdateCombinedData()
		{
			DisplayedEntries = new List<ILocaleContainer>();

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

		public Action<ILocaleGroup, ILocaleContainer> SelectedFileChanged { get; set; }

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

		public ILocaleContainer SelectedFile
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

		public LocaleGroup(string name = "")
		{
			Name = name;

			UpdateAllCommand = ReactiveCommand.Create(UpdateCombinedData);
		}
	}
}
