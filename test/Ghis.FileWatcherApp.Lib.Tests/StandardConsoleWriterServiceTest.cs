

namespace Ghis.FileWatcherApp.Lib.Tests
{
    using Ghis.FileWatcherApp.Lib;
    using Xunit;
    public class StandardConsoleWriterServiceTest
    {
        private readonly StandardConsoleWriterService standardConsoleWriterService;
        public StandardConsoleWriterServiceTest()
        {
            standardConsoleWriterService = new StandardConsoleWriterService();
        }

       
        [Fact]
        
            public void IsPrime_NonPrimesLessThan10_ReturnFalseTest1() { 
                var result = standardConsoleWriterService.IsPrime(value);

                Assert.False(result, $"{value} should not be prime");
            }
        
    }
}