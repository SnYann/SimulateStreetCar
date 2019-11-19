using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulate
{
    public static class MathHelper
    {
        public const float Pi = 3.14159274F;

        public static Random random = new System.Random(Guid.NewGuid().GetHashCode());//得到不会重复的数字



        public static float RandomUniform(float min, float max)//是精确地均匀分布
        {
            float delta = (max - min) * 100;
            int r = random.Next((int)delta);
            return min + (float)r / 100;
        }



        public static float RandomNormal(float min, float max)
        {
            return RandomNormal((max - min) / 4, (min + max) / 2, min, max);
        }
        /// <summary>
        /// 产生截断的正态分布随机数
        /// </summary>
        /// <param name="multi">multi值越大，曲线越平，相当于截取其中部分</param>
        /// <param name="center">中心</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomNormal(float multi, float center, float min, float max)
        {
            Random r = new System.Random(Guid.NewGuid().GetHashCode());//得到不会重复的数字
            double u1, u2, v1 = 0, v2 = 0, s = 0, z1 = 0;
            while (s > 1 || s == 0)
            {
                u1 = r.NextDouble();
                u2 = r.NextDouble();
                v1 = 2 * u1 - 1;
                v2 = 2 * u2 - 1;
                s = v1 * v1 + v2 * v2;
            }
            z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
            z1 *= multi;
            z1 += +center;
            if (z1 < min || z1 > max) z1 = RandomNormal(multi, center, min, max);
            //Debug.Log(z1);
            return (float)z1; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
        }

        /// <summary>
        /// 从随机数0到1产生指数分布随机数,有最大值最小值
        /// </summary>
        /// <param name="multi">multi值越大，曲线越平，相当于截取其中部分</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomEXP(int multi, int min, int max)
        {
            double u1, S1 = 0;
            u1 = random.NextDouble();
            S1 = -1 * Math.Log(1 - u1);

            int k = random.Next(0, 2) == 1 ? -1 : +1;
            S1 *= multi * k;
            if (S1 > max) S1 = RandomEXP(multi, min, max);
            if (S1 < min) S1 = RandomEXP(multi, min, max);
            //Debug.Log(S1);
            return (float)S1;
        }


        /**
         * <summary>Computes the length of a specified two-dimensional vector.
         * </summary>
         *
         * <param name="vector">The two-dimensional vector whose length is to be
         * computed.</param>
         * <returns>The length of the two-dimensional vector.</returns>
         */
        public static float abs(Vector2 vector)
        {
            return sqrt(absSq(vector));
        }

        /**
         * <summary>Computes the squared length of a specified two-dimensional
         * vector.</summary>
         *
         * <returns>The squared length of the two-dimensional vector.</returns>
         *
         * <param name="vector">The two-dimensional vector whose squared length
         * is to be computed.</param>
         */
        public static float absSq(Vector2 vector)
        {
            return vector * vector;
        }

        internal static float sqrt(float scalar)
        {
            return (float)Math.Sqrt(scalar);
        }

        /// <summary>
        /// sin角超过180则返回true，不超过则返回false
        /// </summary>
        /// <param name="px1"></param>
        /// <param name="py1"></param>
        /// <param name="px2"></param>
        /// <param name="py2"></param>
        /// <returns></returns>
        public static bool Over180(float px1,float py1,float px2,float py2,float px3, float py3, float px4, float py4)
        {

            return false;

        }



        ///------------alg 3------------  
        public static double determinant(float v1, float v2, float v3, float v4)  // 行列式  
        {
            return (v1 * v3 - v2 * v4);
        }
       
        public static bool intersect3(float px1, float py1, float px2, float py2, float px3, float py3, float px4, float py4)

        {
            double delta = determinant(px2 - px1, px3 - px4, py2 - py1, py3 - py4);
            if (delta <= (1e-6) && delta >= -(1e-6))  // delta=0，表示两线段重合或平行  
            {
                return false;
            }
            double namenda = determinant(px3 - px1, px3 - px4, py3 - py1, py3 - py4) / delta;
            if (namenda > 1 || namenda < 0)
            {
                return false;
            }
            double miu = determinant(px2 - px1, px3 - px1, py2 - py1, py3 - py1) / delta;
            if (miu > 1 || miu < 0)
            {
                return false;
            }
            return true;
        }
    }

}

