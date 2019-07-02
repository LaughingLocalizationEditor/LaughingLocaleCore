using System;
using System.Collections.Generic;
using System.Text;

namespace LaughingLocale.Data
{
	public class AppSettings : BaseData
	{
		private string lastDirectory;

		public string LastDirectory
		{
			get => lastDirectory;
			set
			{
				Update(ref lastDirectory, value);
			}
		}

		private string lastFile;

		public string LastFile
		{
			get => lastFile;
			set
			{
				Update(ref lastFile, value);
			}
		}
	}
}
