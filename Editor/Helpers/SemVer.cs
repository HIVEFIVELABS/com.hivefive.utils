// Copyright 2021 by Hivefive Labs. https://www.hivefivelabs.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hivefive.Utils;
using UnityEngine;

namespace Hivefive.Editor.Utils
{
    [Serializable]
    public class SemVer : IComparable<SemVer>, IComparable
    {
        // Regex to match valid semVer strings (http://semver.org/); Expected format: Major.Minor.Patch[-PreRelease][+BuildMetadata]
        private static readonly Regex semVer =
            new(
                @"(?<major>\d+)\.{1}(?<minor>\d+)\.{1}(?<patch>\d+)(?:-{1}(?:\.?(?<preRelease>[1-9A-Za-z-][0-9A-Za-z-]*))+)?(?:\+{1}(?:\.?(?<buildMetadata>[1-9A-Za-z-][0-9A-Za-z-]*))+)?",
                RegexOptions.Compiled);

        #region Serialized Fields

        [SerializeField] private int _major;
        [SerializeField] private int _minor;
        [SerializeField] private int _patch;
        [SerializeField] private string[] _buildMetadata;
        [SerializeField] private string[] _preRelease;

        #endregion

        public string[] buildMetadata { get => _buildMetadata; private set => _buildMetadata = value; }

        public int major { get => _major; private set => _major = value; }

        public int minor { get => _minor; private set => _minor = value; }

        public int patch { get => _patch; private set => _patch = value; }

        public string[] preRelease { get => _preRelease; private set => _preRelease = value; }

        public static bool operator >(SemVer left, SemVer right)
        {
            return Comparer<SemVer>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(SemVer left, SemVer right)
        {
            return Comparer<SemVer>.Default.Compare(left, right) >= 0;
        }

        public static bool operator <(SemVer left, SemVer right)
        {
            return Comparer<SemVer>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(SemVer left, SemVer right)
        {
            return Comparer<SemVer>.Default.Compare(left, right) <= 0;
        }

        public static SemVer FromString(string version)
        {
            if (version.IsNullOrWhiteSpace()) {
                return null;
            }

            if (!semVer.IsMatch(version)) {
                Debug.LogWarning($"Parameter {nameof(version)} has wrong format.");
                return null;
            }

            var match = semVer.Match(version);
            int.TryParse(match.Groups["major"].Value, out var major);
            int.TryParse(match.Groups["minor"].Value, out var minor);
            int.TryParse(match.Groups["patch"].Value, out var patch);

            var preRelease = new List<string>();
            foreach (Capture capture in match.Groups["preRelease"].Captures) {
                preRelease.Add(capture.Value);
            }

            var buildMetadata = new List<string>();
            foreach (Capture capture in match.Groups["buildMetadata"].Captures) {
                buildMetadata.Add(capture.Value);
            }

            return new SemVer
            {
                major = major,
                minor = minor,
                patch = patch,
                preRelease = preRelease.ToArray(),
                buildMetadata = buildMetadata.ToArray()
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

            if (_patch >= 0) {
                nums.Add(_patch);
            }

            var s = string.Join('.', nums);

            if (_preRelease is { Length: > 0 }) {
                s += $"-{string.Join('.', _preRelease)}";
            }

            if (_buildMetadata is { Length: > 0 }) {
                s += $"-{string.Join('.', _buildMetadata)}";
            }

            return s;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return 1;
            }

            if (ReferenceEquals(this, obj)) {
                return 0;
            }

            return obj is SemVer other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(SemVer)}");
        }

        #endregion

        #region IComparable<SemVer> Members

        public int CompareTo(SemVer other)
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

            return _patch.CompareTo(other._patch);
        }

        #endregion
    }
}