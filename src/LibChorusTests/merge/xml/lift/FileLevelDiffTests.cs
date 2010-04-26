using Chorus.FileTypeHanders;
using Chorus.FileTypeHanders.xml;
using LibChorus.Tests.merge.xml.generic;
using NUnit.Framework;

namespace LibChorus.Tests.merge.xml.lift
{
	/// <summary>
	/// NB: this uses dummy strategies because the tests are not testing if the internals of the entries are merged
	/// </summary>
	[TestFixture]
	public class FileLevelDiffTests
	{
		[Test]
		public void NewEntryFromUs_Reported()
		{
			var child = @"<?xml version='1.0' encoding='utf-8'?>
					<lift version='0.10' producer='WeSay 1.0.0.0'>
						<entry id='old1'/>
						<entry id='newGuy'/>
						<entry id='old2'/>
					</lift>";
			var parent = @"<?xml version='1.0' encoding='utf-8'?>
					<lift version='0.10' producer='WeSay 1.0.0.0'>
						<entry id='old1'/>
						<entry id='old2'/>
					</lift>";
			var listener = new ListenerForUnitTests();
			var differ = Xml2WayDiffer.CreateFromStrings(parent, child, listener,
				"<entry ", "</lift>", "id");
			differ.ReportDifferencesToListener();
			listener.AssertExpectedChangesCount(1);
			listener.AssertFirstChangeType<XmlAdditionChangeReport>();
		}

		[Test]
		public void WeRemovedEntry_Reported()
		{
			var child = @"<?xml version='1.0' encoding='utf-8'?>
					<lift version='0.10' producer='WeSay 1.0.0.0'>
						<entry id='old1'/>
					</lift>";
			var parent = @"<?xml version='1.0' encoding='utf-8'?>
					<lift version='0.10' producer='WeSay 1.0.0.0'>
						<entry id='old1'/>
						<entry id='old2'/>
					</lift>";
			var listener = new ListenerForUnitTests();
			var differ = Xml2WayDiffer.CreateFromStrings(parent, child, listener,
				"<entry ", "</lift>", "id");
			differ.ReportDifferencesToListener();
			listener.AssertExpectedChangesCount(1);
			listener.AssertFirstChangeType<XmlDeletionChangeReport>();
		}

		[Test]
		public void WeMarkedEntryAsDeleted_ReportedAsDeletion()
		{
			var parent = @"<?xml version='1.0' encoding='utf-8'?>
					<lift version='0.10' producer='WeSay 1.0.0.0'>
						<entry id='old1'/>
						<entry id='old2'/>
					</lift>";
			var child = @"<?xml version='1.0' encoding='utf-8'?>
					<lift version='0.10' producer='WeSay 1.0.0.0'>
						<entry id='old1' dateDeleted='2009-06-16T06:14:20Z'/>
						<entry id='old2'/>
					</lift>";
			var listener = new ListenerForUnitTests();
			var differ = Xml2WayDiffer.CreateFromStrings(parent, child, listener,
				"<entry ", "</lift>", "id");
			differ.ReportDifferencesToListener();
			listener.AssertExpectedChangesCount(1);
			listener.AssertFirstChangeType<XmlDeletionChangeReport>();
		}
	}
}