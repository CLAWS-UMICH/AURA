using UnityEngine.Animations;

namespace UnityEngine.XR.Content.Utils
{
    public static class VectorUtilities
    {
        public static Vector3 SetAxis(this Vector3 vector3, float value, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return new Vector3(value, vector3.y, vector3.z);
                case Axis.Y:
                    return new Vector3(vector3.x, value, vector3.z);
                case Axis.Z:
                    return new Vector3(vector3.x, vector3.y, value);
                case Axis.None:
                default:
                    return vector3;
            }
        }
    }
}
