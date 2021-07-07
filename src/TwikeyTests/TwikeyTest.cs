using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twikey;

namespace TwikeyTests
{
    [TestClass]
    public class TwikeyTest
    {
        private TwikeyClient api;
        
        [TestInitialize]
        public void Before(){
            api =new TwikeyClient("487AC20678D5B2278494B358B4E04E0D0B4873BA",true).WithUserAgent("twikey");
        }
        
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsNotNull(api);
        }
    }
}
