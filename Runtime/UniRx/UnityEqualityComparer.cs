namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal static class UnityEqualityComparer
    {
        private static readonly IEqualityComparer<Vector2> _vector2 = new Vector2EqualityComparer();
        private static readonly IEqualityComparer<Vector3> _vector3 = new Vector3EqualityComparer();
        private static readonly IEqualityComparer<Vector4> _vector4 = new Vector4EqualityComparer();
        private static readonly IEqualityComparer<Color> _color = new ColorEqualityComparer();
        private static readonly IEqualityComparer<Color32> _color32 = new Color32EqualityComparer();
        private static readonly IEqualityComparer<Rect> _rect = new RectEqualityComparer();
        private static readonly IEqualityComparer<Bounds> _bounds = new BoundsEqualityComparer();
        private static readonly IEqualityComparer<Quaternion> _quaternion = new QuaternionEqualityComparer();

        private static readonly RuntimeTypeHandle _vector2Type = typeof(Vector2).TypeHandle;
        private static readonly RuntimeTypeHandle _vector3Type = typeof(Vector3).TypeHandle;
        private static readonly RuntimeTypeHandle _vector4Type = typeof(Vector4).TypeHandle;
        private static readonly RuntimeTypeHandle _colorType = typeof(Color).TypeHandle;
        private static readonly RuntimeTypeHandle _color32Type = typeof(Color32).TypeHandle;
        private static readonly RuntimeTypeHandle _rectType = typeof(Rect).TypeHandle;
        private static readonly RuntimeTypeHandle _boundsType = typeof(Bounds).TypeHandle;
        private static readonly RuntimeTypeHandle _quaternionType = typeof(Quaternion).TypeHandle;

        private static readonly IEqualityComparer<Vector2Int> _vector2Int = new Vector2IntEqualityComparer();
        private static readonly IEqualityComparer<Vector3Int> _vector3Int = new Vector3IntEqualityComparer();
        private static readonly IEqualityComparer<RangeInt> _rangeInt = new RangeIntEqualityComparer();
        private static readonly IEqualityComparer<RectInt> _rectInt = new RectIntEqualityComparer();
        private static readonly IEqualityComparer<BoundsInt> _boundsInt = new BoundsIntEqualityComparer();

        private static readonly RuntimeTypeHandle _vector2IntType = typeof(Vector2Int).TypeHandle;
        private static readonly RuntimeTypeHandle _vector3IntType = typeof(Vector3Int).TypeHandle;
        private static readonly RuntimeTypeHandle _rangeIntType = typeof(RangeInt).TypeHandle;
        private static readonly RuntimeTypeHandle _rectIntType = typeof(RectInt).TypeHandle;
        private static readonly RuntimeTypeHandle _boundsIntType = typeof(BoundsInt).TypeHandle;

        public static IEqualityComparer<T> GetDefault<T>()
        {
            return Cache<T>.Comparer;
        }

        private static object GetDefaultHelper(Type type)
        {
            var t = type.TypeHandle;

            if (t.Equals(_vector2Type)) return _vector2;
            if (t.Equals(_vector3Type)) return _vector3;
            if (t.Equals(_vector4Type)) return _vector4;
            if (t.Equals(_colorType)) return _color;
            if (t.Equals(_color32Type)) return _color32;
            if (t.Equals(_rectType)) return _rect;
            if (t.Equals(_boundsType)) return _bounds;
            if (t.Equals(_quaternionType)) return _quaternion;
            if (t.Equals(_vector2IntType)) return _vector2Int;
            if (t.Equals(_vector3IntType)) return _vector3Int;
            if (t.Equals(_rangeIntType)) return _rangeInt;
            if (t.Equals(_rectIntType)) return _rectInt;
            if (t.Equals(_boundsIntType)) return _boundsInt;

            return null;
        }

        private static class Cache<T>
        {
            public static readonly IEqualityComparer<T> Comparer;

            static Cache()
            {
                var comparer = GetDefaultHelper(typeof(T));
                if (comparer == null)
                {
                    Comparer = EqualityComparer<T>.Default;
                }
                else
                {
                    Comparer = (IEqualityComparer<T>)comparer;
                }
            }
        }

        private sealed class Vector2EqualityComparer : IEqualityComparer<Vector2>
        {
            public bool Equals(Vector2 self, Vector2 vector)
            {
                return self.x.Equals(vector.x) && self.y.Equals(vector.y);
            }

            public int GetHashCode(Vector2 obj)
            {
                return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2;
            }
        }

        private sealed class Vector3EqualityComparer : IEqualityComparer<Vector3>
        {
            public bool Equals(Vector3 self, Vector3 vector)
            {
                return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z);
            }

            public int GetHashCode(Vector3 obj)
            {
                return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2;
            }
        }

        private sealed class Vector4EqualityComparer : IEqualityComparer<Vector4>
        {
            public bool Equals(Vector4 self, Vector4 vector)
            {
                return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z) && self.w.Equals(vector.w);
            }

            public int GetHashCode(Vector4 obj)
            {
                return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2 ^ obj.w.GetHashCode() >> 1;
            }
        }

        private sealed class ColorEqualityComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color self, Color other)
            {
                return self.r.Equals(other.r) && self.g.Equals(other.g) && self.b.Equals(other.b) && self.a.Equals(other.a);
            }

            public int GetHashCode(Color obj)
            {
                return obj.r.GetHashCode() ^ obj.g.GetHashCode() << 2 ^ obj.b.GetHashCode() >> 2 ^ obj.a.GetHashCode() >> 1;
            }
        }

        private sealed class RectEqualityComparer : IEqualityComparer<Rect>
        {
            public bool Equals(Rect self, Rect other)
            {
                return self.x.Equals(other.x) && self.width.Equals(other.width) && self.y.Equals(other.y) && self.height.Equals(other.height);
            }

            public int GetHashCode(Rect obj)
            {
                return obj.x.GetHashCode() ^ obj.width.GetHashCode() << 2 ^ obj.y.GetHashCode() >> 2 ^ obj.height.GetHashCode() >> 1;
            }
        }

        private sealed class BoundsEqualityComparer : IEqualityComparer<Bounds>
        {
            public bool Equals(Bounds self, Bounds vector)
            {
                return self.center.Equals(vector.center) && self.extents.Equals(vector.extents);
            }

            public int GetHashCode(Bounds obj)
            {
                return obj.center.GetHashCode() ^ obj.extents.GetHashCode() << 2;
            }
        }

        private sealed class QuaternionEqualityComparer : IEqualityComparer<Quaternion>
        {
            public bool Equals(Quaternion self, Quaternion vector)
            {
                return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z) && self.w.Equals(vector.w);
            }

            public int GetHashCode(Quaternion obj)
            {
                return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2 ^ obj.w.GetHashCode() >> 1;
            }
        }

        private sealed class Color32EqualityComparer : IEqualityComparer<Color32>
        {
            public bool Equals(Color32 self, Color32 vector)
            {
                return self.a.Equals(vector.a) && self.r.Equals(vector.r) && self.g.Equals(vector.g) && self.b.Equals(vector.b);
            }

            public int GetHashCode(Color32 obj)
            {
                return obj.a.GetHashCode() ^ obj.r.GetHashCode() << 2 ^ obj.g.GetHashCode() >> 2 ^ obj.b.GetHashCode() >> 1;
            }
        }

        private sealed class Vector2IntEqualityComparer : IEqualityComparer<Vector2Int>
        {
            public bool Equals(Vector2Int self, Vector2Int vector)
            {
                return self.x.Equals(vector.x) && self.y.Equals(vector.y);
            }

            public int GetHashCode(Vector2Int obj)
            {
                return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2;
            }
        }

        private sealed class Vector3IntEqualityComparer : IEqualityComparer<Vector3Int>
        {
            public static readonly Vector3IntEqualityComparer Default = new Vector3IntEqualityComparer();

            public bool Equals(Vector3Int self, Vector3Int vector)
            {
                return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z);
            }

            public int GetHashCode(Vector3Int obj)
            {
                return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2;
            }
        }

        private sealed class RangeIntEqualityComparer : IEqualityComparer<RangeInt>
        {
            public bool Equals(RangeInt self, RangeInt vector)
            {
                return self.start.Equals(vector.start) && self.length.Equals(vector.length);
            }

            public int GetHashCode(RangeInt obj)
            {
                return obj.start.GetHashCode() ^ obj.length.GetHashCode() << 2;
            }
        }

        private sealed class RectIntEqualityComparer : IEqualityComparer<RectInt>
        {
            public bool Equals(RectInt self, RectInt other)
            {
                return self.x.Equals(other.x) && self.width.Equals(other.width) && self.y.Equals(other.y) && self.height.Equals(other.height);
            }

            public int GetHashCode(RectInt obj)
            {
                return obj.x.GetHashCode() ^ obj.width.GetHashCode() << 2 ^ obj.y.GetHashCode() >> 2 ^ obj.height.GetHashCode() >> 1;
            }
        }

        private sealed class BoundsIntEqualityComparer : IEqualityComparer<BoundsInt>
        {
            public bool Equals(BoundsInt self, BoundsInt vector)
            {
                return Vector3IntEqualityComparer.Default.Equals(self.position, vector.position)
                    && Vector3IntEqualityComparer.Default.Equals(self.size, vector.size);
            }

            public int GetHashCode(BoundsInt obj)
            {
                return Vector3IntEqualityComparer.Default.GetHashCode(obj.position) ^ Vector3IntEqualityComparer.Default.GetHashCode(obj.size) << 2;
            }
        }
    }
}