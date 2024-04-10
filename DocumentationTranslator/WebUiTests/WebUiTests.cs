using WebApp.Components.Pages;

namespace WebUiTests
{
	[TestClass]
	public class WebUiTests
	{
		[TestMethod]
		public void TestOnInit()
		{
			Home sut = new Home();
			sut.CallOnInit();

			Assert.IsNotNull(sut, "Home page did not initialise correctly");

			Assert.AreEqual("Provide valid input", sut.messageInfo);

			Assert.IsTrue(sut.languages.Count > 0);
			Assert.IsTrue(sut.documentNames.Count >= 0);

			Assert.AreEqual(sut.inputLanguage, sut.languages.FirstOrDefault());
			Assert.AreEqual(sut.outputLanguage, sut.languages.FirstOrDefault());
			Assert.AreEqual(sut.documentName, sut.documentNames.FirstOrDefault());
		}
	}
}