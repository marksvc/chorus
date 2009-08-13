﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Chorus.sync;
using Chorus.UI.Sync;
using Chorus.VcsDrivers;
using NUnit.Framework;

namespace LibChorus.Tests
{
	[TestFixture]
	public class SyncDialogTests
	{
		[Test, Ignore("Run by hand only")]
		public void LaunchDialog_LazyWithAdvancedUI()
		{
			var setup = new RepositorySetup("pedro");
			{
				Application.EnableVisualStyles();

				using (var dlg = new SyncDialog(setup.ProjectFolderConfig,
					SyncUIDialogBehaviors.Lazy,
					SyncUIFeatures.Everything))
				{
				//    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create("bogus", @"z:/"));
					dlg.ShowDialog();
				}

			}
		}


		[Test, Ignore("Run by hand only")]
		public void LaunchDialog_MinimalUI()
		{
			var setup = new RepositorySetup("pedro");
			{
				Application.EnableVisualStyles();
				var dlg = new SyncDialog(setup.ProjectFolderConfig,
				   SyncUIDialogBehaviors.StartImmediately,
				   SyncUIFeatures.Minimal);

				dlg.ShowDialog();
			}
		}

		[Test]
		public void LaunchDialog_AutoWithMinimalUI()
		{
			var setup = new RepositorySetup("pedro");
			{
				Application.EnableVisualStyles();
				var dlg = new SyncDialog(setup.ProjectFolderConfig,
				   SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
				   SyncUIFeatures.Minimal);

				dlg.ShowDialog();
			}
		}

		[Test]
		public void LaunchDialog_BogusTarget_AdmitsError()
		{
			var setup = new RepositorySetup("pedro");
			{
				Application.EnableVisualStyles();
				using (var dlg = new SyncDialog(setup.ProjectFolderConfig,
				   SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
				   SyncUIFeatures.Minimal))
				{
					dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create("bogus", @"z:/"));
					dlg.ShowDialog();
					Assert.IsTrue(dlg.FinalStatus.WarningEncountered);
				}
 }
		}
	}
}
