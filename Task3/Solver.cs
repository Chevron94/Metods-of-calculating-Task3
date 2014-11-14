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
        int countIter;
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
            countIter = 0;
        }
        private double LengthVec(double[] x)
        {
            double sum = 0;
            foreach (double val in x)
                sum += val * val;
            return Math.Sqrt(sum);
        }

        private double[] RandVect(int N)
        {
            double[] x = new double[N];
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < N; i++)
                x[i] = 200 * rand.NextDouble() - 100;
            return x;
        }

        private double[] Normalize(double[] x)
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

        private int CompareByAbs(double x, double y)
        {
            if (Math.Abs(x) == Math.Abs(y))
                return 0;
            if (Math.Abs(x) < Math.Abs(y))
                return -1;
            return 1;
        }

        private void RandLyambda()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < N; i++)
                lambda[i] = 2 * (rand.NextDouble() - 0.5) * range;
            Array.Sort(lambda, CompareByAbs);
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
                    HouseHolder[i, j] = OneMatr(i, j) - 2 * w[i] * w[i];
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

        public double CosAngleBeetwen(double[] x1, double[] x2)
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
                while(Solve() == -1);
                iter += countIter;
                LambdaAvg += Math.Abs((max_lambda - lambda[N - 1]) / lambda[N - 1]);
                r += getR();
                VecAvg += getAvgVector();
            }
            iter /= countTest;
            LambdaAvg /= countTest;
            VecAvg /= countTest;
            r /= countTest;
        }

        public double[] getHcol()
        {
            double[] res = new double[N];
            for (int i = 0; i < N; i++)
                res[i] = HouseHolder[i, N - 1];
            return res;
        }

        public double getAvgVector()
        {
            return (1 - Math.Abs(CosAngleBeetwen(getHcol(), lambda_vec)));
            //double res = double.MinValue;
            //for (int i = 0; i < N; i++)
            //{
            //    res = Math.Max(res, Math.Abs(HouseHolder[i, 0] - lambda_vec[i]));
            //}
            //return res;
        }

        public double getR()
        {
            double[] vectorTmp = new double[N];
            vectorTmp = MultiplyMatrAndVec(lambda_vec);
            for (int i = 0; i < N; i++)
                vectorTmp[i] =Math.Abs(vectorTmp[i] - max_lambda * lambda_vec[i]);
            return vectorTmp.Max();
        }

        public int Solve()
        {
            RandomInit();
            double[] pred_vector = new double[N];
            double[] cur_vector = new double[N];
            double[] x_temp = new double[N];
            for (int i = 0; i < N; i++)
                x_temp[i] = 1;
            double current_lambda = double.MaxValue;
            double pred_lambda;

            pred_vector = Normalize(x_temp);
            x_temp = MultiplyMatrAndVec(pred_vector);
            pred_lambda = MultiplyRowByCol(pred_vector, x_temp);

            if (double.IsNaN(pred_lambda) || double.IsInfinity(pred_lambda))
                return -1;

            cur_vector = Normalize(x_temp);
            x_temp = MultiplyMatrAndVec(cur_vector);
            current_lambda = MultiplyRowByCol(cur_vector, x_temp);

            if (double.IsNaN(current_lambda) || double.IsInfinity(current_lambda))
                return -1;

            countIter = 2;

            for (int i = 0; (i < iteration) && (Math.Abs(current_lambda - pred_lambda) >= eps || Math.Abs(1-CosAngleBeetwen(pred_vector, cur_vector)) >= eps); i++)
            {
                pred_vector = cur_vector;
                pred_lambda = current_lambda;
             
                cur_vector = Normalize(x_temp);
                x_temp = MultiplyMatrAndVec(cur_vector);
                current_lambda = MultiplyRowByCol(cur_vector, x_temp);
                countIter++;
                if (double.IsNaN(current_lambda) || double.IsInfinity(current_lambda))
                    return -1;
            }
            lambda_vec = cur_vector;
            max_lambda = current_lambda;
            return countIter;
        }
    }
}
