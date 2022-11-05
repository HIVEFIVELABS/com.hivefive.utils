using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hivefive.Utils;
using UnityEngine;

namespace Hivefive.Editor.Utils
{
    [Serializable]
    public class Version : IComparable<Version>, IComparable
    {
        private static readonly Regex versionRegex =
            new("(\\d+)\\.{1}(\\d+)\\.{1}(\\d+)(?:-{1}(\\d+))?", RegexOptions.Compiled);

        [SerializeField] private int _major;
        [SerializeField] private int _minor;
        [SerializeField] private int _release;
        [SerializeField] private int _build;
        public int build { get => _build; private set => _build = value; }

        public int major { get => _major; private set => _major = value; }

        public int minor { get => _minor; private set => _minor = value; }

        public int release { get => _release; private set => _release = value; }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return 1;
            }

            if (ReferenceEquals(this, obj)) {
                return 0;
            }

            return obj is Version other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(Version)}");
        }

        public int CompareTo(Version other)
        {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (ReferenceEquals(null, other)) {
                return 1;
            }

            var majorComparison = _major.CompareTo(other._major);
            if (majorComparison != 0) {
                return majorComparison;
            }

            var minorComparison = _minor.CompareTo(other._minor);
            if (minorComparison != 0) {
                return minorComparison;
            }

            var releaseComparison = _release.CompareTo(other._release);
            if (releaseComparison != 0) {
                return releaseComparison;
            }

            return _build.CompareTo(other._build);
        }

        public static bool operator >(Version left, Version right)
        {
            return Comparer<Version>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(Version left, Version right)
        {
            return Comparer<Version>.Default.Compare(left, right) >= 0;
        }

        public static bool operator <(Version left, Version right)
        {
            return Comparer<Version>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(Version left, Version right)
        {
            return Comparer<Version>.Default.Compare(left, right) <= 0;
        }

        public static Version FromString(string version)
        {
            if (version.IsNullOrWhiteSpace()) {
                return null;
            }

            if (!versionRegex.IsMatch(version)) {
                Debug.LogWarning($"Parameter {nameof(version)} has wrong format.");
                return null;
            }

            int major = -1, minor = -1, release = -1, build = -1;
            foreach (Match m in versionRegex.Matches(version)) {
                for (var i = 0; i < m.Groups.Count; i++) {
                    switch (i) {
                        case 1:
                            major = int.Parse(m.Groups[i].Value);
                            break;
                        case 2:
                            minor = int.Parse(m.Groups[i].Value);
                            break;
                        case 3:
                            release = int.Parse(m.Groups[i].Value);
                            break;
                        case 4:
                            build = int.Parse(m.Groups[i].Value);
                            break;
                    }
                }
            }

            return new Version
            {
                major = major,
                minor = minor,
                release = release,
                build = build
            };
        }

        public override string ToString()
        {
            var nums = new List<int>();
            if (_major >= 0) {
                nums.Add(_major);
            }

            if (_minor >= 0) {
                nums.Add(_minor);
            }

            if (_release >= 0) {
                nums.Add(_release);
            }

            var s = string.Join('.', nums);
            if (_build >= 0) {
                s += $"-{_build}";
            }

            return s;
        }
    }
}