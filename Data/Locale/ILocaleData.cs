using LaughingLocale.Data.History;
using LaughingLocale.ViewModel;
using LaughingLocale.ViewModel.Locale;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaughingLocale.Data.Locale
{
	public interface ILocaleData : ICommittable
	{
		ILocaleContainer Parent { get; set; }

		string Key { get; set; }
		string Content { get; set; }
		string Handle { get; set; }

		bool Locked { get; }
		bool Selected { get; set; }
	}
}
