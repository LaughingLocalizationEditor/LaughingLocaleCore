﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LaughingLocale.ViewModel.Menu
{
	public class MenuItemViewModel
	{
		public int ID { get; set; } = -1;
		public string Header { get; set; }
		public ICommand Command { get; set; }
		public object CommandParameter { get; set; }
		public IList<MenuItemViewModel> Items { get; set; }
	}
}
