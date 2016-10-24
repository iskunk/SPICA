﻿using SPICA.Serialization.Attributes;

using System;
using System.Text;

namespace SPICA.Math3D
{
    [Inline]
    public class Matrix3x4
    {
        [FixedLength(4 * 3), Inline]
        private float[] Elems;

        public Matrix3x4()
        {
            Elems = new float[4 * 3];

            //Make identity
            this[0, 0] = 1;
            this[1, 1] = 1;
            this[2, 2] = 1;
        }

        public static Matrix3x4 Empty
        {
            get
            {
                return new Matrix3x4
                {
                    M11 = 0,
                    M22 = 0,
                    M33 = 0
                };
            }
        }

        public float M11 { get { return this[0, 0]; } set { this[0, 0] = value; } }
        public float M12 { get { return this[0, 1]; } set { this[0, 1] = value; } }
        public float M13 { get { return this[0, 2]; } set { this[0, 2] = value; } }
        public float M14 { get { return this[0, 3]; } set { this[0, 3] = value; } }

        public float M21 { get { return this[1, 0]; } set { this[1, 0] = value; } }
        public float M22 { get { return this[1, 1]; } set { this[1, 1] = value; } }
        public float M23 { get { return this[1, 2]; } set { this[1, 2] = value; } }
        public float M24 { get { return this[1, 3]; } set { this[1, 3] = value; } }

        public float M31 { get { return this[2, 0]; } set { this[2, 0] = value; } }
        public float M32 { get { return this[2, 1]; } set { this[2, 1] = value; } }
        public float M33 { get { return this[2, 2]; } set { this[2, 2] = value; } }
        public float M34 { get { return this[2, 3]; } set { this[2, 3] = value; } }

        public float this[int Row, int Col]
        {
            get { return Elems[(Row * 4) + Col]; }
            set { Elems[(Row * 4) + Col] = value; }
        }

        public static Matrix3x4 RotateX(float Angle)
        {
            return new Matrix3x4
            {
                M22 = (float)Math.Cos(Angle),
                M23 = (float)Math.Sin(Angle),
                M32 = -(float)Math.Sin(Angle),
                M33 = (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 RotateY(float Angle)
        {
            return new Matrix3x4
            {
                M11 = (float)Math.Cos(Angle),
                M13 = -(float)Math.Sin(Angle),
                M31 = (float)Math.Sin(Angle),
                M33 = (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 RotateZ(float Angle)
        {
            return new Matrix3x4
            {
                M11 = (float)Math.Cos(Angle),
                M12 = (float)Math.Sin(Angle),
                M21 = -(float)Math.Sin(Angle),
                M22 = (float)Math.Cos(Angle)
            };
        }

        //Vector3D
        public static Matrix3x4 Translate(Vector3D Offset)
        {
            return new Matrix3x4
            {
                M14 = Offset.X,
                M24 = Offset.Y,
                M34 = Offset.Z
            };
        }

        public static Matrix3x4 Scale(Vector3D Scale)
        {
            return new Matrix3x4
            {
                M11 = Scale.X,
                M22 = Scale.Y,
                M33 = Scale.Z
            };
        }

        //Adapted from OpenTK lib
        public static Matrix3x4 operator *(Matrix3x4 LHS, Matrix3x4 RHS)
        {
            Matrix3x4 Output = new Matrix3x4();

            Output.M11 = (LHS.M11 * RHS.M11) + (LHS.M12 * RHS.M21) + (LHS.M13 * RHS.M31);
            Output.M12 = (LHS.M11 * RHS.M12) + (LHS.M12 * RHS.M22) + (LHS.M13 * RHS.M32);
            Output.M13 = (LHS.M11 * RHS.M13) + (LHS.M12 * RHS.M23) + (LHS.M13 * RHS.M33);
            Output.M14 = (LHS.M11 * RHS.M14) + (LHS.M12 * RHS.M24) + (LHS.M13 * RHS.M34) + LHS.M14;
            Output.M21 = (LHS.M21 * RHS.M11) + (LHS.M22 * RHS.M21) + (LHS.M23 * RHS.M31);
            Output.M22 = (LHS.M21 * RHS.M12) + (LHS.M22 * RHS.M22) + (LHS.M23 * RHS.M32);
            Output.M23 = (LHS.M21 * RHS.M13) + (LHS.M22 * RHS.M23) + (LHS.M23 * RHS.M33);
            Output.M24 = (LHS.M21 * RHS.M14) + (LHS.M22 * RHS.M24) + (LHS.M23 * RHS.M34) + LHS.M24;
            Output.M31 = (LHS.M31 * RHS.M11) + (LHS.M32 * RHS.M21) + (LHS.M33 * RHS.M31);
            Output.M32 = (LHS.M31 * RHS.M12) + (LHS.M32 * RHS.M22) + (LHS.M33 * RHS.M32);
            Output.M33 = (LHS.M31 * RHS.M13) + (LHS.M32 * RHS.M23) + (LHS.M33 * RHS.M33);
            Output.M34 = (LHS.M31 * RHS.M14) + (LHS.M32 * RHS.M24) + (LHS.M33 * RHS.M34) + LHS.M34;

            return Output;
        }

        public Matrix3x4 Invert()
        {
            Vector3D InvRot0 = new Vector3D(M11, M21, M31);
            Vector3D InvRot1 = new Vector3D(M12, M22, M32);
            Vector3D InvRot2 = new Vector3D(M13, M23, M33);
            
            InvRot0 *= (1f / InvRot0.Length);
            InvRot1 *= (1f / InvRot1.Length);
            InvRot2 *= (1f / InvRot2.Length);

            Vector3D Translation = new Vector3D(M14, M24, M34);

            float TranslateX = -Vector3D.Dot(InvRot0, Translation);
            float TranslateY = -Vector3D.Dot(InvRot1, Translation);
            float TranslateZ = -Vector3D.Dot(InvRot2, Translation);

            Matrix3x4 Output = new Matrix3x4();

            Output.M11 = InvRot0.X;
            Output.M12 = InvRot0.Y;
            Output.M13 = InvRot0.Z;
            Output.M14 = TranslateX;

            Output.M21 = InvRot1.X;
            Output.M22 = InvRot1.Y;
            Output.M23 = InvRot1.Z;
            Output.M24 = TranslateY;

            Output.M31 = InvRot2.X;
            Output.M32 = InvRot2.Y;
            Output.M33 = InvRot2.Z;
            Output.M34 = TranslateZ;

            return Output;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();

            for (int Row = 0; Row < 3; Row++)
            {
                for (int Col = 0; Col < 4; Col++)
                {
                    SB.Append(string.Format("M{0}{1}: {2,-16}", Row + 1, Col + 1, this[Row, Col]));
                }

                SB.Append(Environment.NewLine);
            }

            return SB.ToString();
        }
    }
}
