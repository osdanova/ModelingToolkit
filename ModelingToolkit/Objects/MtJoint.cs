using System;
using System.Numerics;

namespace ModelingToolkit.Objects
{
    public class MtJoint
    {
        public string? Name { get; set; }
        public Vector3? AbsoluteScale { get; set; }
        public Vector3? RelativeScale { get; set; }
        public Vector3? AbsoluteRotation { get; set; } // Radians
        public Vector3? RelativeRotation { get; set; } // Radians
        public Quaternion? AbsoluteRotationQ { get; set; }
        public Quaternion? RelativeRotationQ { get; set; }
        public Vector3? AbsoluteTranslation { get; set; }
        public Vector3? RelativeTranslation { get; set; }
        public Matrix4x4? AbsoluteTransformationMatrix { get; set; }
        public Matrix4x4? RelativeTransformationMatrix { get; set; }
        public int? ParentId { get; set; }
        public bool IsVisible { get; set; } // For the viewport

        public MtJoint()
        {
            Name = null;
            AbsoluteScale = null;
            RelativeScale = null;
            AbsoluteRotation = null;
            RelativeRotation = null;
            AbsoluteRotationQ = null;
            RelativeRotationQ = null;
            AbsoluteTranslation = null;
            RelativeTranslation = null;
            AbsoluteTransformationMatrix = null;
            RelativeTransformationMatrix = null;
            ParentId = null;
            IsVisible = true;
        }

        public override string ToString()
        {
            return "TRS: " + AbsoluteTranslation + " " + AbsoluteRotation + " " + AbsoluteScale;
        }

        public void Decompose()
        {
            if (AbsoluteTransformationMatrix != null)
            {
                Matrix4x4.Decompose((Matrix4x4)AbsoluteTransformationMatrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation);
                AbsoluteScale = scale;
                AbsoluteRotationQ = rotation;
                AbsoluteTranslation = translation;
                AbsoluteRotation = ToEulerAngles(AbsoluteRotationQ.Value);
            }

            if (RelativeTransformationMatrix != null)
            {
                Matrix4x4.Decompose((Matrix4x4)RelativeTransformationMatrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation);
                RelativeScale = scale;
                RelativeRotationQ = rotation;
                RelativeTranslation = translation;
                RelativeRotation = ToEulerAngles(RelativeRotationQ.Value);
            }
        }

        public void Compose(bool preferEuler = false)
        {
            if (AbsoluteScale != null && (AbsoluteRotation != null || AbsoluteRotationQ != null) && AbsoluteTranslation != null)
            {
                Matrix4x4 scaleMatrix = Matrix4x4.CreateScale((Vector3)AbsoluteScale);
                Matrix4x4 rotationMatrix;
                if (!preferEuler && AbsoluteRotationQ != null)
                {
                    rotationMatrix = Matrix4x4.CreateFromQuaternion((Quaternion)AbsoluteRotationQ);
                }
                else
                {
                    Matrix4x4 rotationMatrixX = Matrix4x4.CreateRotationX(((Vector3)AbsoluteRotation).X);
                    Matrix4x4 rotationMatrixY = Matrix4x4.CreateRotationY(((Vector3)AbsoluteRotation).Y);
                    Matrix4x4 rotationMatrixZ = Matrix4x4.CreateRotationZ(((Vector3)AbsoluteRotation).Z);
                    rotationMatrix = rotationMatrixX * rotationMatrixY * rotationMatrixZ;
                    //rotationMatrix = rotationMatrixZ * rotationMatrixY * rotationMatrixX;
                }
                Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation((Vector3)AbsoluteTranslation);

                AbsoluteTransformationMatrix = scaleMatrix * rotationMatrix * translationMatrix;
            }

            if (RelativeScale != null && (RelativeRotation != null || RelativeRotationQ != null) && RelativeTranslation != null)
            {
                Matrix4x4 scaleMatrix = Matrix4x4.CreateScale((Vector3)RelativeScale);
                Matrix4x4 rotationMatrix;
                if (!preferEuler && RelativeRotationQ != null)
                {
                    rotationMatrix = Matrix4x4.CreateFromQuaternion((Quaternion)RelativeRotationQ);
                }
                else
                {
                    Matrix4x4 rotationMatrixX = Matrix4x4.CreateRotationX(((Vector3)RelativeRotation).X);
                    Matrix4x4 rotationMatrixY = Matrix4x4.CreateRotationY(((Vector3)RelativeRotation).Y);
                    Matrix4x4 rotationMatrixZ = Matrix4x4.CreateRotationZ(((Vector3)RelativeRotation).Z);
                    rotationMatrix = rotationMatrixX * rotationMatrixY * rotationMatrixZ;
                    //rotationMatrix = rotationMatrixZ * rotationMatrixY * rotationMatrixX;
                }
                Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation((Vector3)RelativeTranslation);

                RelativeTransformationMatrix = scaleMatrix * rotationMatrix * translationMatrix;
            }
        }

        // VVV Attempts to get euler angles (in radians) from the quaternion rotations VVV

        // From ChatGPT
        public static Vector3 ToEulerAnglesXYZ(Quaternion quaternion)
        {
            // Extract the individual components of the quaternion
            float qx = quaternion.X;
            float qy = quaternion.Y;
            float qz = quaternion.Z;
            float qw = quaternion.W;

            // Calculate the roll (X-axis rotation)
            float roll = (float)Math.Atan2(2 * (qx * qw + qy * qz), 1 - 2 * (qx * qx + qy * qy));

            // Calculate the pitch (Y-axis rotation)
            float pitch = (float)Math.Asin(2 * (qw * qy - qx * qz));

            // Calculate the yaw (Z-axis rotation)
            float yaw = (float)Math.Atan2(2 * (qx * qy + qz * qw), 1 - 2 * (qy * qy + qz * qz));

            // Return the Euler angles as a Vector3 (roll, pitch, yaw)
            return new Vector3(roll, pitch, yaw);
        }

        /// From https://stackoverflow.com/a/70462919
        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        // From ChatGPT based on http://eecs.qmul.ac.uk/~gslabaugh/publications/euler.pdf
        public static Vector3 ExtractEulerAnglesXYZ(Quaternion q)
        {
            Matrix4x4 matrix = Matrix4x4.CreateFromQuaternion(q);

            // Extract Euler angles (roll, pitch, yaw) from a rotation matrix
            float roll, pitch, yaw;

            // Extract pitch (around X-axis)
            pitch = (float)Math.Asin(-matrix.M23);

            // Check for special cases to avoid gimbal lock
            if (Math.Abs(matrix.M23) < 0.99999)
            {
                // Extract yaw (around Y-axis)
                yaw = (float)Math.Atan2(matrix.M13, matrix.M33);

                // Extract roll (around Z-axis)
                roll = (float)Math.Atan2(matrix.M21, matrix.M22);
            }
            else
            {
                // Gimbal lock: pitch is near +/-90 degrees
                // Extract yaw (around Y-axis)
                yaw = (float)Math.Atan2(-matrix.M31, matrix.M11);

                // Roll is not well-defined in gimbal lock, so set it to zero
                roll = 0;
            }

            return new Vector3(roll, pitch, yaw);
        }
    }
}
