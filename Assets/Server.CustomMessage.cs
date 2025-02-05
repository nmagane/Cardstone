using System.Collections.Generic;
using System.Threading;
using Mirror;

public partial class Server
{
    [System.Serializable]
    public struct JsonMessage : NetworkMessage
    {
        public string jsonString;
    }


    [System.Serializable]
    public class CustomMessageIntermediate
    {
        public MessageType type;
        public int clientID;
        public ushort order;
        public List<int> ints = new List<int>();
        public List<ulong> ulongs = new List<ulong>();
        public List<ushort> ushorts = new List<ushort>();
        public List<bool> bools = new List<bool>();
        public List<string> strings = new List<string>();
        public List<int> intArrays = new List<int>();
        public List<ushort> ushortArrays = new List<ushort>();

        public CustomMessageIntermediate(CustomMessage m)
        {
            type = m.type;
            clientID = m.clientID;
            order = m.order;
            ints = new List<int>(m.ints);
            ulongs = new List<ulong>(m.ulongs);
            ushorts = new List<ushort>(m.ushorts);
            bools = new List<bool>(m.bools);
            strings = new List<string>(m.strings);
            intArrays = new List<int>(m.intArrays);
            ushortArrays = new List<ushort>(m.ushortArrays);
        }
    }

    [System.Serializable]
    public class CustomMessage
    {
        public MessageType type=MessageType.Matchmaking;
        public int clientID=0;
        public ushort order=0;
        public Queue<int> ints = new Queue<int>();
        public Queue<ulong> ulongs = new Queue<ulong>();
        public Queue<ushort> ushorts = new Queue<ushort>();
        public Queue<bool> bools=new Queue<bool>();
        public Queue<string> strings = new Queue<string>();
        public List<int> intArrays = new List<int>();
        public List<ushort> ushortArrays = new List<ushort>();

        public int GetInt()
        {
            return ints.Dequeue();
        }
        public ulong GetULong()
        {
            return ulongs.Dequeue();
        }
        public ushort GetUShort()
        {
            return ushorts.Dequeue();
        }
        public bool GetBool()
        {
            return bools.Dequeue();
        }
        public string GetString()
        {
            return strings.Dequeue();
        }
        public List<int> GetInts()
        {
            return intArrays;
        }
        public List<ushort> GetUShorts()
        {
            return ushortArrays;
        }

        public void AddInt(int x)
        {
            ints.Enqueue(x);
        }
        public void AddULong(ulong x)
        {
            ulongs.Enqueue(x);
        }
        public void AddUShort(ushort x)
        {
            ushorts.Enqueue(x);
        }
        public void AddBool(bool x)
        {
            bools.Enqueue(x);
        }
        public void AddString(string x)
        {
            strings.Enqueue(x);
        }
        public void AddInts(List<int> x)
        {
            intArrays = (x);
        }
        public void AddUShorts(List<ushort> x)
        {
            ushortArrays = (x);
        }
        public CustomMessage()
        {

        }
        public CustomMessage(CustomMessageIntermediate m)
        {
            type = m.type;
            clientID = m.clientID;
            order = m.order;
            ints = new Queue<int>(m.ints);
            ulongs = new Queue<ulong>(m.ulongs);
            ushorts = new Queue<ushort>(m.ushorts);
            bools = new Queue<bool>(m.bools);
            strings = new Queue<string>(m.strings);

            intArrays = new List<int>(m.intArrays);//(m.intArrays);
            ushortArrays = new List<ushort>(m.ushortArrays);//(m.intArrays);

        }

        public CustomMessage(CustomMessage m)
        {
            type = m.type;
            clientID = m.clientID;
            order = m.order;
            ints = new Queue<int>(m.ints);
            ulongs = new Queue<ulong>(m.ulongs);
            ushorts = new Queue<ushort>(m.ushorts);
            bools = new Queue<bool>(m.bools);
            strings = new Queue<string>(m.strings);

            intArrays = new List<int>(m.intArrays);//(m.intArrays);
            ushortArrays = new List<ushort>(m.ushortArrays);//(m.intArrays);

        }
    }
}
