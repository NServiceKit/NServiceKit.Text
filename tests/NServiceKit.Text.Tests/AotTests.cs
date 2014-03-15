using NUnit.Framework;
using NServiceKit.Text.Tests.Support;

namespace NServiceKit.Text.Tests
{
	[TestFixture]
	public class AotTests
	{
#if SILVERLIGHT || MONOTOUCH
		[Test]
		public void Can_Register_AOT()
		{
			JsConfig.RegisterForAot();
		}
#endif
	}
}