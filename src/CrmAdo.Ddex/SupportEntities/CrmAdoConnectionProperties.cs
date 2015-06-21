using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoConnectionProperties : AdoDotNetConnectionProperties
    {
        public override string ToDisplayString()
        {
            var result = base.ToDisplayString();
            return result;
        }

        public override string ToSafeString()
        {
            var result = base.ToSafeString();
            return result;
        }

        public override void Add(string key, object value)
        {
            base.Add(key, value);
        }

        public override void Add(string key, Type type, object value)
        {
            base.Add(key, type, value);
        }

        public override bool Contains(KeyValuePair<string, object> item)
        {
            return base.Contains(item);
        }

        public override bool ContainsKey(string key)
        {
            return base.ContainsKey(key);
        }

        public override void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public override int Count
        {
            get
            {
                return base.Count;
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string[] GetSynonyms(string key)
        {
            return base.GetSynonyms(key);
        }

        public override bool IsComplete
        {
            get
            {
                return base.IsComplete;
            }
        }

        public override bool IsExtensible
        {
            get
            {
                return base.IsExtensible;
            }
        }

        public override bool IsSensitive(string key)
        {
            return base.IsSensitive(key);
        }

        public override ICollection<string> Keys
        {
            get
            {
                return base.Keys;
            }
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        protected override void OnSiteChanged(EventArgs e)
        {
            base.OnSiteChanged(e);
        }

        public override void Parse(string connectionString)
        {
            base.Parse(connectionString);
        }

        public override bool Remove(string key)
        {
            return base.Remove(key);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override bool Reset(string key)
        {
            return base.Reset(key);
        }

        public override object this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool TryGetValue(string key, out object value)
        {
            return base.TryGetValue(key, out value);
        }

        public override ICollection<object> Values
        {
            get
            {
                return base.Values;
            }
        }

    }
}
