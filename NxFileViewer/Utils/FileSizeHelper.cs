using System;
using System.Linq;

namespace Emignatik.NxFileViewer.Utils
{
    public static class FileSizeHelper
    {
        private class UnitRange
        {
            public UnitRange(int pow)
            {
                if (pow < 1) throw new ArgumentOutOfRangeException(nameof(pow));
                BinaryThreshold = (ulong)Math.Pow(1024, pow);
                DecimalThreshold = (ulong)Math.Pow(1000, pow);

                BinaryDivider = (ulong)Math.Pow(1024, pow - 1);
                DecimalDivider = (ulong)Math.Pow(1000, pow - 1);

                switch (pow)
                {
                    case 1:
                        DecimalUnit = "B";
                        BinaryUnit = "B";
                        break;
                    case 2:
                        DecimalUnit = "KB";
                        BinaryUnit = "KiB";
                        break;
                    case 3:
                        DecimalUnit = "MB";
                        BinaryUnit = "MiB";
                        break;
                    case 4:
                        DecimalUnit = "GB";
                        BinaryUnit = "GiB";
                        break;
                    case 5:
                        DecimalUnit = "TB";
                        BinaryUnit = "TiB";
                        break;
                    default:
                        DecimalUnit = "PB";
                        BinaryUnit = "PiB";
                        break;
                }
            }

            public ulong GetThreshold(FileSizeUnit unit)
            {
                return unit == FileSizeUnit.BINARY ? BinaryThreshold : DecimalThreshold;
            }

            public string GetUnit(FileSizeUnit unit)
            {
                return unit == FileSizeUnit.BINARY ? BinaryUnit : DecimalUnit;
            }

            public ulong GetDivider(FileSizeUnit unit)
            {
                return unit == FileSizeUnit.BINARY ? BinaryDivider : DecimalDivider;
            }

            public string ToString(ulong nbBytes, FileSizeUnit unit)
            {
                var sizeInUnit = nbBytes / (double)GetDivider(unit);
                return $"{sizeInUnit:0.##} {GetUnit(unit)}";
            }

            public ulong DecimalThreshold { get; }

            public ulong BinaryThreshold { get; }

            public string DecimalUnit { get; }

            public string BinaryUnit { get; }

            public ulong BinaryDivider { get; }

            public ulong DecimalDivider { get; }
        }


        private static readonly UnitRange[] _unitRanges = new UnitRange[6];

        static FileSizeHelper()
        {

            for (var i = 0; i < _unitRanges.Length; i++)
            {
                _unitRanges[i] = new UnitRange(i + 1);
            }
        }

        public static string ToFileSize(this long nbBytes, FileSizeUnit unit = FileSizeUnit.BINARY)
        {
            return ((ulong) nbBytes).ToFileSize(unit);
        }


        public static string ToFileSize(this ulong nbBytes, FileSizeUnit unit = FileSizeUnit.BINARY)
        {
            foreach (var unitRange in _unitRanges)
            {
                if (nbBytes < unitRange.GetThreshold(unit))
                {
                    return unitRange.ToString(nbBytes, unit);
                }
            }

            return _unitRanges.LastOrDefault()?.ToString(nbBytes, unit) ?? nbBytes.ToString();
        }
    }
}
