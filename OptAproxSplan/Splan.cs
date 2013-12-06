using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptAproxSplan
{
    class Splan
    {
        public int n = 0;
        
        public double omega = 0;
        public double alpha = 0;
        public double beta = 0;
        
        public double R = 0;
        
        public double teta = 0;
        public double tau = 0;
        public double x = 0;
        public double h = 0;
        
        public double ro0 = 0;
        public double ro1 = 0;

        public double ksi0 = 0;

        double Uminus1 = 0;
        double U0 = 1;
        double J = 0;
        public string error = "";
        

        public void GetAllParam(double L, int N, double G, double x) // L - лямбда, N - N, G - гамма
        {
            n = N - 1;
            double mu = 1 - L;
            omega = L - mu; //омега надо будет записывать в массив (Наверное, лучше вместе со значением функционала), чтобы определять при каком значении функционал минимален. 
            h = 1 / N;
            teta = G * tau / h;
            R = -(1 / 4) * ((1 + teta * teta) * omega * omega - 4 * teta * omega + 3 * teta - 1);
            alpha = (1 - R - teta) / R;
            beta = (1 - R + teta) / R;
            double[,] Aobr = new double[n,n]; // обратная матрица (по идее в инициализации компонентов формы можно прописать присвоение n значения, а после процедуры инициализации объявление самого массива)
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= n; j++)
                    if (i <= j)
                    {
                        Aobr[i, j] = R * (alpha * beta - 1) / (U(N, x) - beta * U(n, x)) * MinusOne(i+j) * ((U(i, x) - beta*U(i - 1, x)) * U(n - j, x));
                    }
                    else
                    {
                        Aobr[i, j] = R * (alpha * beta - 1) / (U(N, x) - beta * U(n, x)) * MinusOne(i + j) * ((U(j, x) - beta * U(j - 1, x)) * U(n - i, x));
                    }
            ksi0 = tau * ((ro1 - ro0) / tau - G * (fi(1) - fi(0)) / h);        
            double[] ksi = new double[n]; // вектор кси
            for (int i = 1; i <= n; i++)
            {
                ksi[i] = ksiI(i, G);
            }
            J = scalarProd(matrixVectorProd(Aobr, ksi, n), ksi);

        }
        //произведение матрицы на вектор
        double[] matrixVectorProd(double[,] matrix, double[] vect, int rasmernostVect)
        {
            double[] glas = new double[rasmernostVect];
            for (int i = 0; i <= matrix.Rank; i++)
                for (int j = 0; j <= matrix.Rank; j++)
                {
                    glas[i] += matrix[i, j] * vect[j];
                    if (j >= 2)
                        j = 0;
                }
            return glas;
        }

        //скалярное произведение векторов
        double scalarProd(double[] vect1, double[] vect2)
        {
            double glas = 0;
            if (vect1.Length == vect2.Length)
                for (int i = 0; i <= vect1.Length; i++)
                {
                    glas += vect1[i] * vect2[i];
                }
            else
                error = "Невозможно выполнить: скалярное произведение (вектора разных размерностей)";
            return glas;
        }

        double fi(int j)
        {
            return j * h;
        }

        double ksiI(int i, double gamma) // i = 1..n
        {
            return gamma * tau * ((fi(i) - fi(i - 1)) / h - (fi(i + 1) - fi(i)) / h);
        }

        int MinusOne(int stepen) // вычисление знака минус единицы при возведении в степень
        {
            if (stepen % 2 == 0)
                return 1;
            else if (stepen % 2 == 1)
                return -1;
            return 0;
        }

        double U(int k,double x) // многочлен Чебышева
        {
            return (2 * x * U(k - 1, x) - U(k - 2, x));
        }
    }
}
