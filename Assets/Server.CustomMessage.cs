using System.Collections.Generic;


public partial class Server
{
    [System.Serializable]
    public class CustomMessage
    {
        public MessageType type;
        public ushort clientID;
        Queue<int> ints = new Queue<int>();
        Queue<ulong> ulongs = new Queue<ulong>();
        Queue<ushort> ushorts = new Queue<ushort>();
        Queue<bool> bools=new Queue<bool>();
        Queue<string> strings = new Queue<string>();
        Queue<int[]> intArrays = new Queue<int[]>();
        Queue<ushort[]> ushortArrays = new Queue<ushort[]>();

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
        public int[] GetInts()
        {
            return intArrays.Dequeue();
        }
        public ushort[] GetUShorts()
        {
            return ushortArrays.Dequeue();
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
        public void AddInts(int[] x)
        {
            intArrays.Enqueue(x);
        }
        public void AddUShorts(ushort[] x)
        {
            ushortArrays.Enqueue(x);
        }
    }
}
