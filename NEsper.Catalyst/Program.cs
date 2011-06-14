///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

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
            restControlManager.Open();

            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();

            _engineManager.Dispose();
        }
    }
}
