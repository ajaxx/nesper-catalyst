using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NEsper.Catalyst.Common
{
    public static class IOExtensions
    {
        /// <summary>
        /// Reads the stream to its logical end or until a max length is achieved.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string ReadToEnd(this Stream stream, int maxLength)
        {
            return ReadToEnd(new StreamReader(stream), maxLength);
        }

        /// <summary>
        /// Reads the reader to its logical end or until a max length is achieved.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string ReadToEnd(this TextReader reader, int maxLength)
        {
            var buffer = new char[4096];
            var stringWriter = new StringWriter();

            while (true)
            {
                int avail = Math.Min(buffer.Length, maxLength);
                if (avail <= 0)
                {
                    break;
                }

                int read = reader.ReadBlock(buffer, 0, avail);
                if (read <= 0)
                {
                    break;
                }

                maxLength -= read;
                stringWriter.Write(buffer, 0, read);
            }

            return stringWriter.ToString();
        }
    }
}
