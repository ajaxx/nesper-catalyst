///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class SampleClientMain
    {
        const string DEFAULT_ENGINE_URI = "http://localhost/catalyst/engine";

        public static void Main()
        {
            // create a catalyst adapter
            var adapter = new Catalyst(
                DEFAULT_ENGINE_URI,
                new RabbitMqEventConsumerFactory("localhost"),
                new MsmqEventConsumerFactory());
            // attach to the default instance - i.e the default database
            var instance = adapter.GetDefaultInstance();
            // create an injector ... the purpose of the injector is to ensure that
            // events exist and are flowing through the system.
            var injector = new InjectSynthetic(instance);
            injector.Start();
            injector.WaitOne();
            // create a consumer ... the purpose of the consumer is to demonstrate
            // how to setup statements and consume event flow from the engine.
            var consumer = new Consumer(instance, "SyntheticEvent");
            consumer.Start();
        }
    }
}
