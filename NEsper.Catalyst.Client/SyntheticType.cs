using System.Collections.Generic;

namespace NEsper.Catalyst.Client
{
    public class SyntheticType
        : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntheticType"/> class.
        /// </summary>
        /// <param name="atoms">The atoms.</param>
        public SyntheticType(params SyntheticAtom[] atoms)
        {
            foreach (var atom in atoms)
            {
                this[atom.Name] = atom.Type;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntheticType"/> class.
        /// </summary>
        /// <param name="atoms">The atoms.</param>
        public SyntheticType(IEnumerable<SyntheticAtom> atoms)
        {
            foreach (var atom in atoms)
            {
                this[atom.Name] = atom.Type;
            }
        }

        /// <summary>
        /// Defines the an atom of this type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public SyntheticAtom Declare(string name)
        {
            return new SyntheticAtom(name, this);
        }

        /// <summary>
        /// Creates a synthetic type.
        /// </summary>
        /// <returns></returns>
        public static SyntheticType Define()
        {
            return new SyntheticType();
        }

        /// <summary>
        /// Withes the atom.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public SyntheticType Declare<T>(string name)
        {
            this[name] = typeof (T);
            return this;
        }

        /// <summary>
        /// Withes the atom.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public SyntheticType Declare(string name, SyntheticType type)
        {
            this[name] = type;
            return this;
        }
    }
}
