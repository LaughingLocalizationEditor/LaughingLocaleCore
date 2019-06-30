using LaughingLocale.Data.History;
using LaughingLocale.Data.Locale;
using ReactiveHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaughingLocale.ViewModel
{
	public interface ILocaleContainer
	{
		List<ILocaleData> Data { get; set; }

		bool ChangesUncommitted { get; set; }

		void Add(ILocaleData entry);
		void Add(IList<ILocaleData> entries);
	}

	public class LocaleContainer : HistoryViewModel, ILocaleContainer
	{
		private List<ILocaleData> data = new List<ILocaleData>();

		public List<ILocaleData> Data
		{
			get => data;

			set => data = value;
		}

		public bool ChangesUncommitted { get; set; } = false;

		public virtual void Add(ILocaleData entry)
		{
			AddWithHistory(Data, entry);
		}

		public virtual void Add(IList<ILocaleData> entries)
		{
			var current = Data.ToList();
			var next = Data.Concat(entries).ToList();
			ChangeListWithHistory(data, current, next);
		}
	}
}
