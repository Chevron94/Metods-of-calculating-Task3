using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Task3
{
    class Solver
    {
        double[,] matrix;// = { { 5, 1, 2 }, { 1, 4, 1 }, { 2, 1, 3 } };
        double[] lambda;
        double[] lambda_vec;
        double max_lambda;
        double[,] HouseHolder;
        double eps;
        int iteration;
        int N;
        int range;
        public Solver(int N, int range,int iteration,double eps)
        {
            this.eps = Math.Pow(10,eps);
            this.iteration = iteration;
            this.N = N;
            this.range = range;
            matrix = new double[N, N];
            lambda = new double[N];
            lambda_vec = new double[N];
            HouseHolder = new double[N, N];
        }
        private static double LengthVec(double[] x)
        {
            double sum = 0;
            foreach (double val in x)
                sum += val * val;
            return Math.Sqrt(sum);
        }

        private static double[] RandVect(int N)
        {
            double[] x = new double[N];
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < N; i++)
                x[i] = 2 * (rand.NextDouble() - 0.5) * 50;
            return x;
        }

        private static double[] Normalize(double[] x)
        {
            int N = x.Length;
            double[] res = new double[N];
            double Norma = LengthVec(x);
            Norma = 1 / Norma;
            for (int i = 0; i < N; i++)
                res[i] = x[i] * Norma;
            return res;
        }

        private int OneMatr(int i, int j)
        {
            if (i == j)
                return 1;
            else
                return 0;
        }

        private void RandLyambda()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < N; i++)
                lambda[i] = 2 * (rand.NextDouble() - 0.5) * range;
            Array.Sort(lambda);
        }

        private bool CheckLambda()
        {
            for (int i = 0; i < N - 1; i++)
                if (lambda[i] == lambda[i+1])
                    return false;
            return true;
        }

        private void RandomInit()
        {
            double[] w = Normalize(RandVect(N));
            w = Normalize(w);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    HouseHolder[i, j] = OneMatr(i, j) - 2 * w[i] * w[j];
            do
            {
                RandLyambda();
            }
            while (!CheckLambda());
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    matrix[i, j] = HouseHolder[i, j] * lambda[j];
            double[,] tempM = new double[N, N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < N; k++)
                        temp += matrix[i, k] * HouseHolder[k, j];
                    tempM[i, j] = temp;
                }
            matrix = tempM;
        }

        private double[] MultiplyMatrAndVec(double[] vec)
        {
            double[] res = new double[N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    res[i] += matrix[i, j] * vec[j];
            return res;
        }

        public static double CosAngleBeetwen(double[] x1, double[] x2)
        {
            double len1 = LengthVec(x1);
            double len2 = LengthVec(x2);
            double cos = Scaler(x1, x2) / (len1 * len2);
            if (cos > 1)
                cos = 1;
            else if (cos < -1)
                cos = -1;
            return cos;
        }

        public static double Scaler(double[] x1, double[] x2)
        {
            int len2 = x2.Length;
            int len1 = x1.Length;
            if (len1 != len2)
                return Double.NaN;
            double res = 0;
            for (int i = 0; i < len1; i++)
            {
                res += x1[i] * x2[i];
            }
            return res;
        }
        public double MultiplyRowByCol(double[] row, double[] col)
        {
            double res = 0;
            for (int i = 0; i < N; i++)
            {
                res += row[i] * col[i];
            }
            return res;
        }

        public void FormAnswer(int countTest,ref double LambdaAvg, ref double VecAvg, ref double r, ref int iter)
        {
            LambdaAvg = 0;
            VecAvg = 0;
            r = 0;
            iter = 0;
            for (int i = 0; i < countTest; i++)
            {
                iter+=Solve();
                LambdaAvg += (max_lambda - lambda[N - 1]);
                r += getR();
            }
            iter /= countTest;
            LambdaAvg /= countTest;
            VecAvg /= countTest;
            r /= countTest;
        }

        public double getR()
        {
            double[] vectorTmp = new double[N];
            vectorTmp = MultiplyMatrAndVec(lambda_vec);
            for (int i = 0; i < N; i++)
                vectorTmp[i] -= max_lambda * lambda_vec[i];
            return vectorTmp.Max();
        }

        public int Solve()
        {
            RandomInit();
            double[] x_0 = new double[N];
            double[] predVec = new double[N];
            double curEpsLambda=1;
            double curEpsVec=0;
            int countIter = 0;
            double[] tempVec = new double[N];
            double predLambda = Double.MinValue;
            for (int i = 0; i < N; i++)
            {
                x_0[i] = 1;
                predVec[i] = 1;
            }
            x_0 = Normalize(x_0);
            tempVec = MultiplyMatrAndVec(x_0);
            double max_pred = x_0[0];
            double max_cur = tempVec[0];
            predLambda = max_cur / max_pred;
            Array.Copy(tempVec, x_0, N);
            while ((curEpsLambda > eps || curEpsVec > eps ) && countIter <= iteration)
            {
                x_0 = Normalize(x_0);
                x_0 = MultiplyMatrAndVec(tempVec);
                max_cur = x_0[0];
                max_lambda = max_cur / tempVec[0];
                curEpsLambda = Math.Abs(max_lambda - predLambda);
                curEpsVec = Math.Abs(Math.Acos(1-CosAngleBeetwen(x_0, tempVec)));
                predLambda = max_lambda;
                Array.Copy(x_0, tempVec, N);
                countIter++;
                lambda_vec = x_0;
            }
            return countIter;
        }
    }
}
