using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoStore
{
    public static class KeyTypeValueStoreExtensions
    {
        [Flags]
        public enum SerializedFlags : byte
        {
            None = 0x00,
            Null = 0x01,
            Array = 0x02,
            List = 0x04 | Array,
            Dictionary = 0x08 | Array,

        }

        public static SerializedFlags GetFlagsForType(object value)
        {
            if (value == null)
            {
                return SerializedFlags.Null;
            }
            else if (value.GetType().IsArray)
            {
                return SerializedFlags.Array;
            }
            else if(false /*Something to check if this is a list*/)
            {
                return SerializedFlags.List;
            }
            else if (false /*Something to check if this is a dict*/)
            {
                return SerializedFlags.Dictionary;
            }
            else
            {
                // Just a default object
                return SerializedFlags.None;
            }
        }

        public static void Serialize<Key, Value>(this Dictionary<Key, Value> dict, BinaryWriter writer)
        {
            Dictionary<Key, int> offsetDict = new Dictionary<Key, int>();
            MemoryStream memStream = new MemoryStream();
            int currentIndex = 0;
            using (BinaryWriter memWriter = new BinaryWriter(memStream))
            {
                foreach (KeyValuePair<Key, Value> valuePair in dict)
                {
                    currentIndex = (int)memWriter.Seek(0, SeekOrigin.Current);
                    offsetDict.Add(valuePair.Key, currentIndex);
                    Value val = valuePair.Value;
                    SerializedFlags flags = GetFlagsForType(val);
                    memWriter.Write((byte)flags);
                    if ((flags & SerializedFlags.Null) != 0)
                    {
                        Type valType = val.GetType();
                    }
                }
            }
            
        }
    }
}
