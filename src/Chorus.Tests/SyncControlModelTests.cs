﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Chorus.sync;
using Chorus.UI.Sync;
using Chorus.Utilities;
using Chorus.VcsDrivers;
using LibChorus.Tests;
using NUnit.Framework;

namespace Chorus.Tests
{
	[TestFixture]
	public class SyncControlModelTests
	{
		private string _pathToTestRoot;
		private SyncControlModel _model;
		private StringBuilderProgress _progress;
		private ProjectFolderConfiguration _project;
		private Synchronizer _synchronizer;

		[SetUp]
		public void Setup()
		{
			_progress = new StringBuilderProgress();
			_pathToTestRoot = Path.Combine(Path.GetTempPath(), "ChorusTest");
			if (Directory.Exists(_pathToTestRoot))
				Directory.Delete(_pathToTestRoot, true);
			Directory.CreateDirectory(_pathToTestRoot);


			string pathToText = Path.Combine(_pathToTestRoot, "foo.txt");
			File.WriteAllText(pathToText, "version one of my pretend txt");

			RepositorySetup.MakeRepositoryForTest(_pathToTestRoot, "bob",_progress);

			_project = new ProjectFolderConfiguration(_pathToTestRoot);
			_project.FolderPath = _pathToTestRoot;
			_project.IncludePatterns.Add(pathToText);
			_project.FolderPath = _pathToTestRoot;

			_synchronizer = Synchronizer.FromProjectConfiguration(_project, new NullProgress());
			_model = new SyncControlModel(_project, SyncUIFeatures.Advanced);
			_model.AddProgressDisplay(_progress);
		}

		[Test]
		public void AfterSyncLogNotEmpty()
		{
			_model.Sync(false);
			while(!_model.EnableSendReceive)
				Thread.Sleep(100);
			Assert.IsNotEmpty(_progress.Text);
		}

		[Test]
		public void InitiallyHasUsbTarget()
		{
			Assert.IsNotNull(_model.GetRepositoriesToList()[0].URI == "UsbKey");
			// Assert.IsNotNull(_model.GetRepositoriesToList().Any(r => r.URI == "UsbKey"));
	   }



		[Test]
		public void GetRepositoriesToList_NoRepositoriesKnown_GivesUsb()
		{
			_synchronizer.ExtraRepositorySources.Clear();
			_model = new SyncControlModel(_project, SyncUIFeatures.Advanced);
			_model.AddProgressDisplay(_progress);
			Assert.AreEqual(1, _model.GetRepositoriesToList().Count);
		}

		[Test]
		public void Sync_NonExistantLangDepotProject_ExitsGracefullyWithCorrectErrorResult()
		{
			_model = new SyncControlModel(_project, SyncUIFeatures.Minimal);
			_model.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create("languageforge", "http://hg-public.languagedepot.org/dummy"));
			var progress = new ConsoleProgress();
			_model.AddProgressDisplay(progress);
			SyncResults results = null;
			_model.SynchronizeOver += new EventHandler((sender, e) => results = (sender as SyncResults));
		   _model.Sync(true);
		   while (results==null)
			   Thread.Sleep(100);
			Assert.IsFalse(results.Succeeded);
			Assert.IsNotNull(results.ErrorEncountered);
		}

		[Test]
		public void Sync_Cancelled_ResultsHaveCancelledEqualsTrue()
		{
			_model = new SyncControlModel(_project, SyncUIFeatures.Minimal);
			_model.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create("languageforge", "http://hg-public.languagedepot.org/dummy"));
			var progress = new ConsoleProgress();
			_model.AddProgressDisplay(progress);
			SyncResults results = null;
			_model.SynchronizeOver += new EventHandler((sender, e) => results = (sender as SyncResults));
			_model.Sync(true);
			Thread.Sleep(100);
			_model.Cancel();
			while (results == null)
				Thread.Sleep(100);
			Assert.IsFalse(results.Succeeded);
			Assert.IsTrue(results.Cancelled);
			Assert.IsNull(results.ErrorEncountered);
		}

		private void _model_SynchronizeOver(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}