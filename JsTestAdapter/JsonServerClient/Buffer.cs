using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.JsonServerClient
{
    public class Buffer
    {
        public Buffer(string str)
            : this()
        {
            Add(str);
        }

        public Buffer(byte[] bytes)
            : this()
        {
            Bytes = bytes;
        }

        public Buffer()
        {
            Encoding = Encoding.UTF8;
        }

        private static readonly byte[] _emptyBytes = new byte[0];
        private readonly List<byte> _bytes = new List<byte>();

        public Encoding Encoding { get; private set; }

        public byte[] Bytes
        {
            get { return _bytes.ToArray(); }
            set { Clear(); Add(value); }
        }

        public int Count
        {
            get { return _bytes.Count; }
        }

        public void Clear()
        {
            _bytes.Clear();
        }

        public void Add(params byte[] bytes)
        {
            _bytes.AddRange(bytes);
        }

        public void Add(string str)
        {
            Add(Encoding.GetBytes(str));
        }

        public override string ToString()
        {
            return Encoding.GetString(Bytes);
        }
    }
}
