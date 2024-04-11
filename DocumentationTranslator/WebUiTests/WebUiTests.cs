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
			sut.OnInit();

			Assert.IsNotNull(sut, "Home page did not initialise correctly");

			Assert.AreEqual("Provide valid input", sut.messageInfo);
		}
	}
}