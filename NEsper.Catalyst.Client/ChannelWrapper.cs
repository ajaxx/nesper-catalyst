///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ServiceModel;

namespace NEsper.Catalyst.Client
{
    class ChannelWrapper<T> : IDisposable where T : class
    {
        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public T Channel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelWrapper&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public ChannelWrapper(T channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (Channel != null)
            {
                try
                {
                    if (Channel is IClientChannel)
                    {
                        ((IClientChannel)Channel).Close();
                    }
                    if (Channel is IDisposable)
                    {
                        ((IDisposable)Channel).Dispose();
                    }
                }
                catch (CommunicationException)
                {
                }
                finally
                {
                    Channel = null;
                }
            }
        }
    }
}
