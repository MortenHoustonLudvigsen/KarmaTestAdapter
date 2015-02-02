using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public class FilesSpec : IEnumerable<string>
    {
        public void Include(params FilePattern[] patterns)
        {
            _included.AddRange(patterns);
        }

        public void Include(string cwd, params string[] patterns)
        {
            Include(cwd, patterns.AsEnumerable());
        }

        public void Include(string cwd, IEnumerable<string> patterns)
        {
            Include(patterns.Select(p => new FilePattern(p, cwd)));
        }

        public void Include(IEnumerable<FilePattern> patterns)
        {
            Include(patterns.ToArray());
        }

        public void Exclude(params FilePattern[] patterns)
        {
            _excluded.AddRange(patterns);
        }

        public void Exclude(string cwd, params string[] patterns)
        {
            Exclude(cwd, patterns.AsEnumerable());
        }

        public void Exclude(string cwd, IEnumerable<string> patterns)
        {
            Exclude(patterns.Select(p => new FilePattern(p, cwd)));
        }

        public void Exclude(IEnumerable<FilePattern> patterns)
        {
            Exclude(patterns.ToArray());
        }

        private List<FilePattern> _included = new List<FilePattern>();
        public IEnumerable<FilePattern> Included { get { return _included; } }

        private List<FilePattern> _excluded = new List<FilePattern>();
        public IEnumerable<FilePattern> Excluded { get { return _excluded; } }

        public IEnumerable<string> GetFiles()
        {
            return Included
                .GetFiles()
                .Except(Excluded.GetFiles(), StringComparer.OrdinalIgnoreCase);
        }

        public bool Contains(string file)
        {
            return Included.IsMatch(file) && !Excluded.IsMatch(file);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return GetFiles().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
