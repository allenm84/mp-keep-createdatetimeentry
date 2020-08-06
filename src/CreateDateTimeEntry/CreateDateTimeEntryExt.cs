using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;

namespace CreateDateTimeEntry
{
	public sealed class CreateDateTimeEntryExt : Plugin
	{
		private IPluginHost _pluginHost = null;
		private CreateDateTimeEntrySettings _settings = null;

		public override bool Initialize(IPluginHost host)
		{
			if (host == null)
			{
				return false;
			}

			_pluginHost = host;
			_settings = CreateDateTimeEntrySettings.Load(_pluginHost);

			return true;
		}

		public override void Terminate()
		{
			_settings.Save(_pluginHost);
		}

		public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
		{
			// Provide a menu item for the main location(s)
			if (t == PluginMenuType.Main)
			{
				var mnuDateTimeEntry = new ToolStripMenuItem();
				mnuDateTimeEntry.Text = "DateTime Entry";

				var mnuNewEntry = new ToolStripMenuItem();
				mnuNewEntry.Text = "New";
				mnuNewEntry.Click += OnNewEntryClicked;
				mnuDateTimeEntry.DropDownItems.Add(mnuNewEntry);

				var mnuConfigurePlugin = new ToolStripMenuItem();
				mnuConfigurePlugin.Text = "Settings...";
				mnuConfigurePlugin.Click += OnSettingsClicked;
				mnuDateTimeEntry.DropDownItems.Add(mnuConfigurePlugin);

				return mnuDateTimeEntry;
			}

			// No menu items in other locations
			return null; 
		}

    private Dictionary<string, PwGroup[]> CreateTable(PwGroup pwGroup)
		{
			return pwGroup
				.GetGroups(false)
				.GroupBy(g => g.Name)
				.ToDictionary(k => k.Key, v => v.ToArray());
		}

		private PwGroup GetOrAddGroup(PwGroup parentGroup, string groupName)
		{
			var childGroups = CreateTable(parentGroup);
			if (!childGroups.TryGetValue(groupName, out var value))
			{
				var pg = new PwGroup(true, true, groupName, PwIcon.FolderOpen);
				parentGroup.AddGroup(pg, true);
				value = new[] { pg };
			}
			return value.First();
		}

		private void OnSettingsClicked(object sender, EventArgs e)
		{
			var mainWindow = _pluginHost.MainWindow;
			using (var dlg = new CreateDateTimeEntrySettingsDialog())
			{
				dlg.Subject = _settings.Clone();
				if (dlg.ShowDialog(mainWindow) == DialogResult.OK)
				{
					_settings = dlg.Subject;
					_settings.Save(_pluginHost);
				}
			}
		}

		private void OnNewEntryClicked(object sender, EventArgs e)
		{
			var pd = _pluginHost.Database;
			if ((pd == null) || !pd.IsOpen) 
			{ 
				return; 
			}

			var date = DateTime.Now;
			var year = $"{date:yyyy}";
			var month = $"{date:MM - MMM}";
			var day = $"{date:dd - dddd}";
			var time = $"{date:HH:mm}";

			var rootGroup = pd.RootGroup;
			
			var yearGroup = GetOrAddGroup(rootGroup, year).SetCustomIcon(pd, _settings.YearIconIndex);
			var monthGroup = GetOrAddGroup(yearGroup, month).SetCustomIcon(pd, _settings.MonthIconIndex);
			var dayGroup = GetOrAddGroup(monthGroup, day).SetCustomIcon(pd, _settings.DayIconIndex);

			var pe = new PwEntry(true, true);
			pe.CustomIconUuid = pd.CustomIcons[_settings.TimeIconIndex].Uuid;
			pe.Strings.Set(PwDefs.TitleField, new ProtectedString(pd.MemoryProtection.ProtectTitle, time));
			dayGroup.AddEntry(pe, true);

      _pluginHost.MainWindow.UpdateUI(false, null, true, null, true, null, true);
		}
  }
}
