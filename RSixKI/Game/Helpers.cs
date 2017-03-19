using Microsoft.Xna.Framework;
using System;

namespace RSixKI
{
    public static class Helpers
    {
        public static Random Rand = new Random();

        public enum RotationDirection
        {
            Left,
            Right,
            None,
        }

        public static float? Raycast2D(Rectangle rectangle, Ray ray)
        {
            float num = 0f;
            float maxValue = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Position.X < rectangle.Left) || (ray.Position.X > rectangle.Right))
                {
                    return null;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.X;
                float num8 = (rectangle.Left - ray.Position.X) * num11;
                float num7 = (rectangle.Right - ray.Position.X) * num11;
                if (num8 > num7)
                {
                    float num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = MathHelper.Max(num8, num);
                maxValue = MathHelper.Min(num7, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Position.Y < rectangle.Top) || (ray.Position.Y > rectangle.Bottom))
                {
                    return null;
                }
            }
            else
            {
                float num10 = 1f / ray.Direction.Y;
                float num6 = (rectangle.Top - ray.Position.Y) * num10;
                float num5 = (rectangle.Bottom - ray.Position.Y) * num10;
                if (num6 > num5)
                {
                    float num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = MathHelper.Max(num6, num);
                maxValue = MathHelper.Min(num5, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }

            return new float?(num);
        }

        public static bool IsInView(Vector2 position, float viewAngle, float fov, float viewDistance, Vector2 otherPosition)
        {
            if (fov <= 0)
                throw new ArgumentOutOfRangeException("fov");

            if (Vector2.Distance(position, otherPosition) <= viewDistance || viewDistance < 0)
            {
                Vector2 NormalizedViewDirection = ConvertToDirection(position, viewAngle);
                NormalizedViewDirection.Normalize();

                Vector2 normalizedDirection = otherPosition - position;
                normalizedDirection.Normalize();

                float resultAngle = MathHelper.ToDegrees((float)Math.Acos(Vector2.Dot(NormalizedViewDirection, normalizedDirection)));
                if (resultAngle <= fov)
                    return true;
            }
            return false;
        }

        public static Vector2 ConvertToDirection(Vector2 position, float angle)
        {
            float radAngle = MathHelper.WrapAngle(MathHelper.ToRadians(angle));
            return new Vector2((float)Math.Cos(radAngle), (float)Math.Sin(radAngle));
        }

        public static float ConvertToAngle(Vector2 direction)
        {
            return MathHelper.ToDegrees(MathHelper.WrapAngle((float)Math.Atan2(direction.Y, direction.X))) + 180;
        }

        public static float NormalizeFloat(float value, float min, float max)
        {
            float width = max - min;
            float offsetValue = value - min;

            return (offsetValue - ((float)Math.Floor(offsetValue / width) * width)) + min;
        }

        public static int NormalizeInt(int value, int min, int max)
        {
            int width = max - min;
            int offsetValue = value - min;

            return (offsetValue - ((offsetValue / width) * width)) + min;
        }

        public static RotationDirection GetRotationDirection(float targetAngle, float rotation, float padding)
        {
            float diff = Math.Abs(targetAngle - rotation);
            if (diff >= padding)
            {
                if ((targetAngle < rotation && diff <= 180) || (targetAngle > rotation && diff > 180))
                    return RotationDirection.Left;
                else if ((targetAngle > rotation && diff <= 180) || (targetAngle < rotation && diff > 180))
                    return RotationDirection.Right;
            }

            return RotationDirection.None;
        }
    }
}
