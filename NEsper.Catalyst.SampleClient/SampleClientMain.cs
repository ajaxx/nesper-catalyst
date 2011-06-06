using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class SampleClientMain
    {
        const string DEFAULT_ENGINE_URI = "http://localhost/catalyst/engine";

        private static readonly Catalyst _catalyst;

        public static void Main()
        {
            // create a catalyst adapter
            var adapter = new Catalyst(DEFAULT_ENGINE_URI);
            // attach to the default instance - i.e the default database
            var instance = adapter.GetDefaultInstance();
            // create an injector ... the purpose of the injector is to ensure that
            // events exist and are flowing through the system.
            var injector = new Injector(instance);
            injector.Start();
            injector.WaitOne();
            // create a consumer ... the purpose of the consumer is to demonstrate
            // how to setup statements and consume event flow from the engine.
            var consumer = new Consumer(instance);
            consumer.Start();
        }
    }
}
