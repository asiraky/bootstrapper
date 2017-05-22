using System;
using System.Reflection;
using Bootstrapper;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<Type, Object> resolver = t =>
            {
                // replace with your container's getinstance/createinstance api

                try
                {
                    if (t == typeof(SomeModule))
                        return new SomeModule(new DependancyImpl());

                    return Activator.CreateInstance(t);
                }
                catch (Exception)
                {
                    throw new NotImplementedException("Implement a proper container");
                }
            };

            var container = new BootstrapModuleContainer(resolver);

            container.Load<SomeModule>(); //load a specific module by its type

            //load a specific module by its ctor
            container.Load(new SomeOtherModule());

            //loads all modules in this assembly
            container.Load(Assembly.GetExecutingAssembly());

            //load all modules in matching string - make sure the assembly is in the runtime dir of this app via a reference or build script
            container.Load("ConsoleApplication1.*.dll");

            Console.ReadKey(true);
            Console.WriteLine("Shutting down");
        }
    }

    class SomeModule : StandardBootstrapModule
    {
        private readonly Dependancy dependancy;

        public SomeModule(Dependancy dependancy)
        {
            this.dependancy = dependancy;
        }

        public override void Load()
        {
            Console.WriteLine("Loading SomeModule");

            dependancy.DoSomething();
        }
    }

    class SomeOtherModule : StandardBootstrapModule
    {
        public override void Load()
        {
            Console.WriteLine("Loading SomeOtherModule");
        }
    }

    class SomeOtherOtherModule : StandardBootstrapModule
    {
        public override void Load()
        {
            Console.WriteLine("Loading SomeOtherOtherModule");
        }
    }

    interface Dependancy
    {
        void DoSomething();
    }
    class DependancyImpl : Dependancy
    {
        public void DoSomething() { }
    }
}
