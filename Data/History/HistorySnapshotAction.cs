using System;
using System.Collections.Generic;
using System.Text;

namespace LaughingLocale.Data.History
{
	public struct HistorySnapshotAction
	{
		public Action Undo { get; set; }
		public Action Redo { get; set; }
	}
}
