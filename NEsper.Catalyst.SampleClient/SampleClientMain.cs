///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NEsper.Catalyst.Client.Consumers;
using NEsper.Catalyst.Client.Publishers;

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class SampleClientMain
    {
        const string DEFAULT_ENGINE_URI = "http://localhost/catalyst/engine";

        public static void Main()
        {
            var configuration = new CatalystConfiguration(
                new Uri(DEFAULT_ENGINE_URI),
                new IEventConsumerFactory[]
                    {
                        new RabbitMqEventConsumerFactory("localhost"),
                        new MsmqEventConsumerFactory()
                    },
                new IDataPublisherFactory[]
                    {
                        new RabbitMqDataPublisherFactory(),
                        new MsmqDataPublisherFactory()
                    });


            // create a catalyst adapter
            var adapter = new Catalyst(configuration);
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
