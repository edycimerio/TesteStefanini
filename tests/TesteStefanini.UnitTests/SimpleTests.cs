using System;
using Xunit;

namespace TesteStefanini.UnitTests
{
    public class SimpleTests
    {
        [Fact]
        public void Test_Soma_DeveRetornarSomaCorreta()
        {
            // Arrange
            int a = 2;
            int b = 3;
            
            // Act
            int resultado = a + b;
            
            // Assert
            Assert.Equal(5, resultado);
        }
        
        [Fact]
        public void Test_String_DeveRetornarStringCorreta()
        {
            // Arrange
            string nome = "TesteStefanini";
            
            // Act
            string resultado = nome.ToUpper();
            
            // Assert
            Assert.Equal("TESTESTEFANINI", resultado);
        }
        
        [Fact]
        public void Test_Guid_DeveSerUnico()
        {
            // Arrange & Act
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            
            // Assert
            Assert.NotEqual(guid1, guid2);
        }
    }
}
