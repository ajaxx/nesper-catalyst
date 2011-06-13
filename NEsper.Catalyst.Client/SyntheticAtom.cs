using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEsper.Catalyst.Client
{
    public class SyntheticAtom
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public object Type { get; protected set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="NEsper.Catalyst.Client.SyntheticAtom"/> to <see cref="System.Collections.Generic.KeyValuePair"/>.
        /// </summary>
        /// <param name="atom">The atom.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator KeyValuePair<string, object>(SyntheticAtom atom)
        {
            return new KeyValuePair<string, object>(atom.Name, atom.Type);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntheticAtom"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public SyntheticAtom(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntheticAtom"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public SyntheticAtom(string name, SyntheticType type)
        {
            Name = name;
            Type = type;
        }
    }

    /// <summary>
    /// Typed synthetic atom
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyntheticAtom<T> : SyntheticAtom
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntheticAtom&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SyntheticAtom(string name)
            : base(name, typeof(T))
        {
        }
    }
}
