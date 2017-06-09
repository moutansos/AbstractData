using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AbstractData
{
    public abstract class ObjectEntry
    {
        private string fieldName;

        public ObjectEntry(string fieldName)
        {
            this.fieldName = fieldName;
        }

        public string name
        {
            get => fieldName;
            set => this.fieldName = value;
        }

        public abstract BsonValue toBsonValue();
    }

    public sealed class DataObject<T> : ObjectEntry
    {
        private T value;
        private string fieldName;
        private Type destType;

        public DataObject(T value, string fieldName, Type destType) : base(fieldName)
        {
            this.value = value;
            this.fieldName = fieldName;
            this.destType = destType;
        }

        public T data
        {
            get => value;
            set => this.value = value;
        }


        public override BsonValue toBsonValue()
        {
            return value.ToBson(destType);
        }
    }

    public sealed class ContainerObject : ObjectEntry
    {
        private Dictionary<string, ObjectEntry> subObjects;

        public ContainerObject(string fieldName) : base(fieldName)
        {
            subObjects = new Dictionary<string, ObjectEntry>();
        }

        public ContainerObject() : base(null)
        {
            subObjects = new Dictionary<string, ObjectEntry>();
        }

        public void setData<T>(string[] address, T value, Type destType)
        {
            List<string> addr = address.ToList();
            if (addr.Count == 0)
            {
                throw new ArgumentException("You must have at least a base to the address!");
            }
            else if (addr.Count == 1)
            {
                //TODO: Figure out how to handle generics
                subObjects[addr[0]] = new DataObject<T>(value, addr[0], destType);
            }
            else
            {
                if (!subObjects.ContainsKey(addr[0]) || subObjects[addr[0]].GetType() != typeof(ContainerObject))
                {
                    subObjects[addr[0]] = new ContainerObject(addr[0]);
                }
                string adrTmp = addr[0];
                addr.RemoveAt(0);
                ((ContainerObject)subObjects[adrTmp]).setData<T>(addr.ToArray(), value, typeof(T));
            }
        }

        public T getData<T>(string[] address)
        {
            List<string> addr = address.ToList();
            if (addr.Count == 0)
            {
                throw new ArgumentException("You must have at least a base to the address!");
            }
            else if (addr.Count == 1)
            {
                ObjectEntry data = subObjects[addr[0]];
                if (data.GetType() == typeof(ContainerObject))
                {
                    return default(T);
                }
                else if (data.GetType() == typeof(DataObject<T>))
                {
                    return ((DataObject<T>)subObjects[addr[0]]).data;
                }
                else
                {
                    throw new Exception("Internal Error. Invalid ObjectEntry subclass");
                }
            }
            else
            {
                ObjectEntry data = subObjects[addr[0]];
                if (data.GetType() == typeof(ContainerObject))
                {
                    string adrTmp = addr[0];
                    addr.RemoveAt(0);
                    return ((ContainerObject)subObjects[adrTmp]).getData<T>(address);
                }
                return default(T); //If it isn't a container we can't go down any farther
            }
        }

        public override BsonValue toBsonValue()
        {
            BsonDocument doc = new BsonDocument();

            foreach (KeyValuePair<string, ObjectEntry> pair in subObjects)
            {
                BsonValue newVal = pair.Value.toBsonValue();
                BsonElement newElement = new BsonElement(pair.Key, newVal);
                var obj = pair.Value;
                doc.Add(newElement);
            }
            return doc;
        }
    }
}
