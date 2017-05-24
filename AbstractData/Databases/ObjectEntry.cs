using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public sealed class DataObject : ObjectEntry
    {
        private string value;
        private string fieldName;

        public DataObject(string value, string fieldName) : base(fieldName)
        {
            this.value = value;
        }

        public string data
        {
            get => value;
            set => this.value = value;
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

        public string this[List<string> addr]
        {
            set
            {
                if (addr.Count == 0)
                {
                    throw new ArgumentException("You must have at least a base to the address!");
                }
                else if (addr.Count == 1)
                {
                    subObjects[addr[0]] = new DataObject(value, addr[0]);
                }
                else
                {
                    if (!subObjects.ContainsKey(addr[0]) || subObjects[addr[0]].GetType() != typeof(ContainerObject))
                    {
                        subObjects[addr[0]] = new ContainerObject(addr[0]);
                    }
                    string adrTmp = addr[0];
                    addr.RemoveAt(0);
                    ((ContainerObject)subObjects[adrTmp])[addr] = value;
                }
            }
            get
            {
                if (addr.Count == 0)
                {
                    throw new ArgumentException("You must have at least a base to the address!");
                }
                else if (addr.Count == 1)
                {
                    ObjectEntry data = subObjects[addr[0]];
                    if (data.GetType() == typeof(ContainerObject))
                    {
                        return null;
                    }
                    else if (data.GetType() == typeof(DataObject))
                    {
                        return ((DataObject)subObjects[addr[0]]).data;
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
                        return ((ContainerObject)subObjects[adrTmp])[addr];
                    }
                    return null; //If it isn't a container we can't go down any farther
                }
            }
        }


    }
}
