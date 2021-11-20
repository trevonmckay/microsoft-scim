using Microsoft.SCIM;
using Microsoft.SCIM.Service;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace AirCo.AzureAD.Licensing
{
    [JsonConverter(typeof(RoleCollectionJsonConverter))]
    public class RoleCollection : ICollection<Role>
    {
        private readonly List<Role> _values = new();

        public int Count => _values.Count;

        public bool IsReadOnly => false;

        public void Add(Role item)
        {
            _values.Add(item);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public bool Contains(Role item)
        {
            return _values.Contains(item);
        }

        public void CopyTo(Role[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Role> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public bool Remove(Role item)
        {
            return _values.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}
