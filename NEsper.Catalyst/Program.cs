using System;

using log4net.Config;

namespace NEsper.Catalyst
{
    class Program
    {
        private const string DEFAULT_INSTANCE_NAME = "default";

        private static EngineManager _engineManager;

        static void Main()
        {
            XmlConfigurator.Configure();

            // create the engine manager
            _engineManager = new EngineManager();
            _engineManager.DefaultInstance = _engineManager.CreateInstance(DEFAULT_INSTANCE_NAME);

            // create the control manager(s)
            var restControlManager = new RestControlManager(_engineManager);
            restControlManager.Open(new Uri("http://localhost/catalyst/engine"));

            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();
        }
    }
}
