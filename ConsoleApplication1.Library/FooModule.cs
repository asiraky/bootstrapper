using System;
using Bootstrapper;

namespace ConsoleApplication1.Library
{
    public class FooModule : StandardBootstrapModule
    {
        public override void Load()
        {
            Console.WriteLine("Loading FooModule");
        }
    }
}
