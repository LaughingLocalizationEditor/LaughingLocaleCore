using System;
using System.Collections.Generic;
using System.Text;

namespace LaughingLocale.Data.History
{
	public interface ICommittable
	{
		bool ChangesUncommitted { get; set; }

		void CommitChanges();
	}
}
