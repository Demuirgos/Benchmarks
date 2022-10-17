using System.Reflection.Metadata;
using Attributes;
namespace Program
{
    public interface ITest
    {   
        [Default("Description", "Value")]
        public static string TestString { get; }

        [Default("Description", "23")]
        public static int TestInt { get; }
        
    }
    class Source
    {
        public static void Main(string[] args) {
        }
    }
}