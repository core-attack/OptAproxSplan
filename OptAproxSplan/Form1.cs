using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO; // для работы с текстовыми файлами
using ZedGraph; // для построения с графиками


namespace OptAproxSplan
{
    public partial class Form1 : Form
    {
        System.Globalization.NumberFormatInfo format;
        Parser p = new Parser();
        public int n = 0;

        double omega = 0;
        double alpha = 0;
        double beta = 0;

        double R = 0;

        double teta = 0;
        double x = 0;
        double h = 0;

        double ro0 = 0;
        double ro1 = 0;

        double ksi0 = 0;

        double Uminus1 = 0;
        double U0 = 1;
        double Uglas = 0;
        double J = 0;
        
        string error = "";
        string temp = "";
        string filekolExp = Application.StartupPath + "/txt/kolExperiments.txt";
        string fileallExp = Application.StartupPath + "/txt/allExperiments.txt";
        List<string> tempList = new List<string>();
        string[,] ArrayRoFi = {{"Sin(x) + 1", "Cos(x)"},
                              {"Cos(x+pi/2)", "Sin(x)"},
                              {"Cos(x+pi/2)", "(x-3)*(x+2)*(x-1)*x*(x+1)*(x-2)*(x+3)"},
                              {"x^2", "(x-3)*(x+2)*(x-1)*x*(x+1)*(x-2)*(x+3)"}};

        // для выплывания лога
        string[] btnlog = { ">>>>>>>", "<<<<<<<" };
        Size defaultSize;
        int logX = 0;
        int logY = 0;


        List<double> ALLJ = new List<double>();
        List<double> ALLOMEGA = new List<double>();
        public Form1()
        {
            format = new System.Globalization.NumberFormatInfo();
            format.NumberDecimalSeparator = ",";

            InitializeComponent();
            radioButtonsValues();
            textBoxG.Text = "1";
            textBoxL.Text = "0,2";
            textBoxN.Text = "12";
            textBoxTau.Text = "0,1";
            textBoxKol.Text = "12";
            textBoxFi.Text = "Cos(x)";
            textBoxRo.Text = "Sin(x) + 1";
            textBoxEps.Text = "0,1";
            string[] a = {"Круги", "Многоугольники" , "Плюсы", "Квадраты", "Звезды", "Треугольники вверх", "Треугольники вниз",
                             "V", "X", "По умолчанию", "Ничего"};
            for (int i = 0; i < a.Length - 1; i++ )
            {
                listBoxTypeOfPoint.Items.Add(a[i]);
            }
            defaultSize = Size;
            logX = groupBoxLog.Size.Width;
            logY = groupBoxLog.Size.Height;
            // убираем лог
            hideLog();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            WriteNewNumExp();
            listBoxLog.Items.Clear();
            //Thread t = new Thread(Go); // добавление нового потока, обрабатывающего метод Go 
            //t.Start();
            //Go();
            pb.Value = 0;
            pb.Style = ProgressBarStyle.Continuous;
            pb.Increment(1);
            GetAllParam(lambda, N, gamma, tau);
            pb.Value = 100;
            //MyBiuld();
        }

        //void Go()
        //{
        //    //progressBar1.Enabled = true;
        //}

        private void textBoxN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                //запрет на ввод более одной десятичной точки и тире
                if (
                    (e.KeyChar != ',' || textBoxN.Text.IndexOf(",") != -1) && (e.KeyChar != '-' || textBoxN.Text.IndexOf("-") != -1)
                   )
                {
                    e.Handled = true;
                }
                
            }
        }

        private void textBoxL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                //запрет на ввод более одной десятичной точки и тире
                if (
                    (e.KeyChar != ',' || textBoxL.Text.IndexOf(",") != -1) && (e.KeyChar != '-' || textBoxL.Text.IndexOf("-") != -1)
                   )
                {
                    e.Handled = true;
                }
                
            }
        }

        private void textBoxG_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                //запрет на ввод более одной десятичной точки и тире
                if (
                    (e.KeyChar != ',' || textBoxG.Text.IndexOf(",") != -1) && (e.KeyChar != '-' || textBoxG.Text.IndexOf("-") != -1)
                   )
                {
                    e.Handled = true;
                }
                
            }
        }

        private void textBoxTau_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                //запрет на ввод более одной десятичной точки и тире
                if (
                    (e.KeyChar != ',' || textBoxTau.Text.IndexOf(",") != -1) && (e.KeyChar != '-' || textBoxTau.Text.IndexOf("-") != -1)
                   )
                {
                    e.Handled = true;
                }

            }
        }

        private void textBoxEps_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                //запрет на ввод более одной десятичной точки и тире
                if (
                    (e.KeyChar != ',' || textBoxTau.Text.IndexOf(",") != -1)
                   )
                {
                    e.Handled = true;
                }

            }
        }

        private void textBoxKol_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                {
                    e.Handled = true;
                }

            }
        }

        private void textBoxN_Click(object sender, EventArgs e)
        {
            textBoxN.SelectAll();
        }

        private void textBoxL_Click(object sender, EventArgs e)
        {
            textBoxL.SelectAll();
        }

        private void textBoxG_Click(object sender, EventArgs e)
        {
            textBoxG.SelectAll();
        }

        private void textBoxTau_Click(object sender, EventArgs e)
        {
            textBoxTau.SelectAll();
        }

        private void textBoxKol_Click(object sender, EventArgs e)
        {
            textBoxTau.SelectAll();
        }

        private void textBoxEps_Click(object sender, EventArgs e)
        {
            textBoxEps.SelectAll();
        }
        
        public int N
        {
            get
            {
                if (textBoxN.Text != "")
                    return Convert.ToInt16(textBoxN.Text, format);
                else
                    return 0;
            }
        }

        public double lambda
        {
            get
            {
                if (textBoxL.Text != "")
                    return Convert.ToDouble(textBoxL.Text, format);
                else
                    return 0.0;
            }
        }

        public double gamma
        {
            get
            {
                if (textBoxG.Text != "")
                    return Convert.ToDouble(textBoxG.Text, format);
                else
                    return 0.0;
            }
        }

        public double tau
        {
            get
            {
                if (textBoxTau.Text != "")
                    return Convert.ToDouble(textBoxTau.Text, format);
                else
                    return 0.0;
            }
        }

        public int kol
        {
            get
            {
                if (textBoxKol.Text != "")
                    return Convert.ToInt16(textBoxKol.Text, format);
                else
                    return 0;
            }
        }

        public string f_Ro
        {
            get
            {
                return textBoxRo.Text;
            }
        }

        public string f_Fi
        {
            get
            {
                return textBoxFi.Text;
            }
        }

        public double eps
        {
            get
            {
                if (textBoxEps.Text != "")
                    return Convert.ToDouble(textBoxEps.Text, format);
                else
                    return 0.0;
            }
        }

        private void listBoxTable_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Омега = " + Convert.ToString(omega) + "\n" + "Значение функционала = " + Convert.ToString(J));
        }

        private void WriteTxtFile(string filename)
        {
            //создание нового файла или перезапись существующего
            StreamWriter outStream =
              new StreamWriter(filename+"_"+"ro="+textBoxRo.Text+"_fi="+textBoxFi.Text+"_N="+Convert.ToString(N)+".txt");
            //outStream - созданный нами объект

            //открытие существующего файла на чтение
            //StreamReader inStream =
              //new StreamReader(@"C:\Users\Николай\Desktop\newtxt.txt");
            //inStream - созданный нами объект

            for (int i = 0; i < listBoxLog.Items.Count; i++)
                outStream.WriteLine(listBoxLog.Items[i]);
            outStream.Close();
            //inStream.Close();
        }
        // Application.StartupPath; //путь к папке с программой (где находится exe'шник)
        // возвращает номер последнего эксперимента
        int GetNextExpNum() 
        {
            int i = 0;
            string str = "";
            StreamReader inStream =
              new StreamReader(filekolExp);
            
            while (!inStream.EndOfStream)
            {
                str = inStream.ReadLine();
                if (str != "")
                    i = Convert.ToInt16(str);
            }
            inStream.Close();
            return i;
        }
        //печатает в файл с результатами всех экспериментов все необходимые данные
        private void WriteToAllExperimentsFile()
        {
            int i = GetNextExpNum();
            StreamWriter outStream;
            FileInfo fi = new FileInfo(fileallExp);//для дописывания в конец файла
            outStream = fi.AppendText();
            outStream.WriteLine("Эксперимент №" + Convert.ToString(i + 1));
            for (int j = 0; j < tempList.Count; j++)
                outStream.WriteLine(tempList[j]);
            outStream.WriteLine("");
            outStream.Close();

        }
        //печатает в файл новый порядковый номер эксперимента
        private void WriteNewNumExp()
        {
            int i = GetNextExpNum();
            StreamWriter outStream =
              new StreamWriter(filekolExp);
            outStream.WriteLine(Convert.ToString(i + 1));//заменяет предыдущую цифру на новую (во избежание длинных строк в файле)
            outStream.Close();
        }
        // для каждого эксперимента создает новый файл (слишком неудобно)
        private void WriteTempTxtFile(string filename)
        {
            StreamWriter outStream =
              new StreamWriter(filename);
            for (int i = 0; i < tempList.Count; i++)
                outStream.WriteLine(tempList[i]);
            outStream.Close();
        }
        

        void GetAllParam(double L, int N, double G, double T) // L - лямбда, N - N, G - гамма
        {
            n = N - 1;
            listBoxLog.Items.Add("n = N - 1 ; n = " + n.ToString() + ";");
            double mu = 1 - L;
            listBoxLog.Items.Add("mu = 1 - L ; mu = " + mu.ToString() + ";");
            omega = L - mu; 
            listBoxLog.Items.Add("omega = L - mu ; omega = " + omega.ToString() + ";");
            h = (double) 1 / N;
            listBoxLog.Items.Add("h = 1 / N ; h = " + h.ToString() + ";");
            teta = (double)(G * T / h); // по условию должна быть по модулю больше 1
            listBoxLog.Items.Add("teta = G * tau / h ; teta = " + teta.ToString() + ";");
            R = (double)-0.25 * ((1 + teta * teta) * omega * omega - 4 * teta * omega + 3 * teta * teta - 1);// д.б. меньше 0
            listBoxLog.Items.Add("R = -(1 / 4) * ((1 + teta * teta) * omega * omega - 4 * teta * omega + 3 * teta - 1); ");
            listBoxLog.Items.Add("R = " + R.ToString() + ";");
            x = (double)(1 - R) / R;
            listBoxLog.Items.Add("x = (1 - R) / R; x = " + x.ToString() + ";");
            alpha = (double) (1 - R - teta) / R;
            listBoxLog.Items.Add("alpha = (1 - R - teta) / R;");
            listBoxLog.Items.Add("alpha = " + alpha.ToString() + ";");
            beta = (double) (1 - R + teta) / R;
            listBoxLog.Items.Add("Произведение alpha и beta, и проверка равенства alpha + beta = х");
            listBoxLog.Items.Add(Convert.ToString(alpha * beta) + "\n" + Convert.ToString(alpha + beta) + "?" + Convert.ToString(2 * x));
            listBoxLog.Items.Add("beta = (1 - R + teta) / R;");
            listBoxLog.Items.Add("beta = " + beta.ToString() + ";");
            double abmo = alpha*beta - 1;
            listBoxLog.Items.Add("alpha*beta - 1 > 0: "+abmo.ToString());
            double[,] Aobr = new double[n+1, n+1]; // обратная матрица 
            listBoxLog.Items.Add("Считаем элементы матрицы...");
            pb.Increment(1);
            double u1, u2, u3, u4, u5;
            u1 = u2 = u3 = u4 = u5 = 0;
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= n; j++)
                {
                    if (i <= j)
                    {
                        u1 = U(N, x);
                        u2 = U(n, x);
                        u3 = U(i, x);
                        u4 = U(i - 1, x);
                        u5 = U(n - j, x);
                        Aobr[i, j] = (double) R * (alpha * beta - 1) / (u1 - beta * u2) * MinusOne(i + j) * ((u3 - beta * u4) * u5);
                        listBoxLog.Items.Add("Aobr[" + i.ToString() + "," + j.ToString() + "] = " + Aobr[i, j].ToString() + ";");
                    }
                    else
                    {
                        u1 = U(N, x);
                        u2 = U(n, x);
                        u3 = U(j, x);
                        u4 = U(j - 1, x);
                        u5 = U(n - i, x);
                        Aobr[i, j] = (double) R * (alpha * beta - 1) / (u1 - beta * u2) * MinusOne(i + j) * ((u3 - beta * u4) * u5);
                        listBoxLog.Items.Add("Aobr[" + i.ToString() + "," + j.ToString() + "] = " + Aobr[i, j].ToString() + ";");
                    }
                    pb.Increment(1);
                }
            listBoxLog.Items.Add("Матрица посчитана;");
            ksi0 = (double) tau * (((ro(tau) - ro(0)) / tau) - (G * (fi(1) - fi(0)) / h));
            listBoxLog.Items.Add("ksi0 = tau * ((ro1 - ro0) / tau - G * (fi(1) - fi(0)) / h);");
            listBoxLog.Items.Add("ksi0 = " + ksi0.ToString() + ";");
            double[] ksi = new double[n+1]; // вектор кси
            pb.Increment(1);
            listBoxLog.Items.Add("Cчитаем координаты вектора...");
            for (int i = 0; i <= n; i++)
            {
                if (i == 0)
                    ksi[i] = ksi0;
                ksi[i] = (double) ksiI(i, G);
                listBoxLog.Items.Add("ksi[" + i.ToString() + "] = ksiI(" + i.ToString() + ", " + G.ToString() + ") ;\n ksi[" + i.ToString() + "] = " + 
                    ksi[i].ToString() + ";");
                pb.Increment(1);
            }
            listBoxLog.Items.Add("Координаты вектора посчитаны;");
            listBoxLog.Items.Add("Cчитаем значение функционала...");
            pb.Increment(1);
            J = (double) scalarProd(matrixVectorProd(Aobr, ksi, n), ksi);
            listBoxLog.Items.Add("Значение функционала = " + J.ToString() + ".");
            temp += "N = " + N.ToString() + ";\n" + "Tau = " + T.ToString() + ";\n" + "Gamma = " + G.ToString() + ";\n" + "lambda = " + L.ToString() + ";\n"
                + "ro = " + f_Ro + ";\n"+ "fi = " + f_Fi + ";\n";
            //listBoxTable.Items.Add("Омега = " + omega.ToString() + ";");
            //temp += "Омега = " + Convert.ToString(omega) + ";\n";
            setLabelOmega();
            //listBoxTable.Items.Add("Значение функционала = " + J.ToString() + ".");
            textBoxResult.Text = J.ToString();
            temp += "Значение функционала = " + J.ToString() + ".\n";
            //tempList.Add("N = " + N.ToString() + ";\n" + "Tau = " + T.ToString() + ";\n" + "Gamma = " + G.ToString() + ";\n" + "lambda = " + L.ToString() + ";\n"
            //    + "ro = " + f_Ro + ";\n"+ "fi = " + f_Fi + ";\n" + "Значение функционала = " + J.ToString() + ". \n\n");
            tempList.Add("N = " + N.ToString() + ";");
            tempList.Add("Tau = " + T.ToString() + ";");
            tempList.Add("Gamma = " + G.ToString() + ";");
            tempList.Add("lambda = " + L.ToString() + ";");
            tempList.Add("ro = " + f_Ro + ";");
            tempList.Add("fi = " + f_Fi + ";");
            tempList.Add("Значение функционала = " + J.ToString() + ";");
            tempList.Add("|||||||||||||||||||||||||||||||||||||||||");
            ALLJ.Add(J);
            ALLOMEGA.Add(omega);


            //WriteTxtFile(@"C:\Users\Николай\Desktop\aprox\");
        }

        void setLabelOmega()
        {
            double o = 2 * lambda - 1;
            labelOmega.Text = "Omega = " + o.ToString();
        }

        double GetAllParam(double L, int N, double G) // L - лямбда, N - N, G - гамма
        {
            listBoxLog.Items.Add("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            listBoxLog.Items.Add("");
            listBoxLog.Items.Add("lambda = " + L.ToString() + "; N = " + N.ToString() + "; gamma = " + G.ToString() + "; tau = " + tau.ToString()+".");
            listBoxLog.Items.Add("");
            listBoxLog.Items.Add("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            double T = tau;
            n = N - 1;
            listBoxLog.Items.Add("n = N - 1 ; n = " + n.ToString() + ";");
            double mu = 1 - L;
            listBoxLog.Items.Add("mu = 1 - L ; mu = " + mu.ToString() + ";");
            omega = L - mu;
            listBoxLog.Items.Add("omega = L - mu ; omega = " + omega.ToString() + ";");
            h = (double)1 / N;
            listBoxLog.Items.Add("h = 1 / N ; h = " + h.ToString() + ";");
            teta = (double)(G * T / h); // по условию должна быть по модулю больше 1
            listBoxLog.Items.Add("teta = G * tau / h ; teta = " + teta.ToString() + ";");
            R = (double)-0.25 * ((1 + teta * teta) * omega * omega - 4 * teta * omega + 3 * teta * teta - 1);// д.б. меньше 0
            listBoxLog.Items.Add("R = -(1 / 4) * ((1 + teta * teta) * omega * omega - 4 * teta * omega + 3 * teta - 1); ");
            listBoxLog.Items.Add("R = " + R.ToString() + ";");
            x = (double)(1 - R) / R;
            listBoxLog.Items.Add("x = (1 - R) / R; x = " + x.ToString() + ";");
            alpha = (double)(1 - R - teta) / R;
            listBoxLog.Items.Add("alpha = (1 - R - teta) / R;");
            listBoxLog.Items.Add("alpha = " + alpha.ToString() + ";");
            beta = (double)(1 - R + teta) / R;
            listBoxLog.Items.Add("Произведение alpha и beta, и проверка равенства alpha + beta = х");
            listBoxLog.Items.Add(Convert.ToString(alpha * beta) + "\n" + Convert.ToString(alpha + beta) + "?" + Convert.ToString(2 * x));
            listBoxLog.Items.Add("beta = (1 - R + teta) / R;");
            listBoxLog.Items.Add("beta = " + beta.ToString() + ";");
            double abmo = alpha * beta - 1;
            listBoxLog.Items.Add("alpha*beta - 1 > 0: " + abmo.ToString());
            double[,] Aobr = new double[n + 1, n + 1]; // обратная матрица 
            listBoxLog.Items.Add("Считаем элементы матрицы...");
            double u1, u2, u3, u4, u5;
            u1 = u2 = u3 = u4 = u5 = 0;
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= n; j++)
                {
                    if (i <= j)
                    {
                        u1 = U(N, x);
                        u2 = U(n, x);
                        u3 = U(i, x);
                        u4 = U(i - 1, x);
                        u5 = U(n - j, x);
                        Aobr[i, j] = (double)R * (alpha * beta - 1) / (u1 - beta * u2) * MinusOne(i + j) * ((u3 - beta * u4) * u5);
                        listBoxLog.Items.Add("Aobr[" + i.ToString() + "," + j.ToString() + "] = " + Aobr[i, j].ToString() + ";");
                    }
                    else
                    {
                        u1 = U(N, x);
                        u2 = U(n, x);
                        u3 = U(j, x);
                        u4 = U(j - 1, x);
                        u5 = U(n - i, x);
                        Aobr[i, j] = (double)R * (alpha * beta - 1) / (u1 - beta * u2) * MinusOne(i + j) * ((u3 - beta * u4) * u5);
                        listBoxLog.Items.Add("Aobr[" + i.ToString() + "," + j.ToString() + "] = " + Aobr[i, j].ToString() + ";");
                    }
                }
            listBoxLog.Items.Add("Матрица посчитана;");
            ksi0 = (double)tau * (((ro(tau) - ro(0)) / tau) - (G * (fi(1) - fi(0)) / h));
            listBoxLog.Items.Add("ksi0 = tau * ((ro1 - ro0) / tau - G * (fi(1) - fi(0)) / h);");
            listBoxLog.Items.Add("ksi0 = " + ksi0.ToString() + ";");
            double[] ksi = new double[n + 1]; // вектор кси
            listBoxLog.Items.Add("Cчитаем координаты вектора...");
            for (int i = 0; i <= n; i++)
            {
                if (i == 0)
                    ksi[i] = ksi0;
                ksi[i] = (double)ksiI(i, G);
                listBoxLog.Items.Add("ksi[" + i.ToString() + "] = ksiI(" + i.ToString() + ", " + G.ToString() + ") ;\n ksi[" + i.ToString() + "] = " +
                    ksi[i].ToString() + ";");
                pb.Increment(1);
            }
            listBoxLog.Items.Add("Координаты вектора посчитаны;");
            listBoxLog.Items.Add("Cчитаем значение функционала...");
            J = (double)scalarProd(matrixVectorProd(Aobr, ksi, n), ksi);
            listBoxLog.Items.Add("Значение функционала = " + J.ToString() + ".");
            temp += "N = " + N.ToString() + ";\n" + "Tau = " + T.ToString() + ";\n" + "Gamma = " + G.ToString() + ";\n" + "lambda = " + L.ToString() + ";\n"
                + "ro = " + f_Ro + ";\n" + "fi = " + f_Fi + ";\n";
            //listBoxTable.Items.Add("Омега = " + omega.ToString() + ";");
            //temp += "Омега = " + Convert.ToString(omega) + ";\n";
            setLabelOmega();
            //listBoxTable.Items.Add("Значение функционала = " + J.ToString() + ".");
            textBoxResult.Text = J.ToString();
            temp += "Значение функционала = " + J.ToString() + ".\n";
            //tempList.Add("N = " + N.ToString() + ";\n" + "Tau = " + T.ToString() + ";\n" + "Gamma = " + G.ToString() + ";\n" + "lambda = " + L.ToString() + ";\n"
            //    + "ro = " + f_Ro + ";\n"+ "fi = " + f_Fi + ";\n" + "Значение функционала = " + J.ToString() + ". \n\n");
            tempList.Add("N = " + N.ToString() + ";");
            tempList.Add("Tau = " + T.ToString() + ";");
            tempList.Add("Gamma = " + G.ToString() + ";");
            tempList.Add("lambda = " + L.ToString() + ";");
            tempList.Add("ro = " + f_Ro + ";");
            tempList.Add("fi = " + f_Fi + ";");
            tempList.Add("Значение функционала = " + J.ToString() + ";");
            tempList.Add("|||||||||||||||||||||||||||||||||||||||||");
            ALLJ.Add(J);
            ALLOMEGA.Add(omega);
            return J;
        }

        //произведение матрицы на вектор
        double[] matrixVectorProd(double[,] matrix, double[] vect, int rasmernostVect)
        {
            //listBoxLog.Items.Add("Считаем произведение матрицы на вектор;");
            double[] glas = new double[rasmernostVect + 1];
            for (int i = 0; i <= rasmernostVect; i++)
            {
                for (int j = 0; j <= rasmernostVect; j++)
                {
                    glas[i] += (double)matrix[i, j] * vect[j];
                }
            }
            return glas;
        }

        //скалярное произведение векторов
        double scalarProd(double[] vect1, double[] vect2)
        {
            double glas = 0;
            if (vect1.Length == vect2.Length)
                for (int i = 0; i <= vect1.Length - 1; i++)
                {
                    glas += (double)vect1[i] * vect2[i];
                }
            else
            {
                error = "Невозможно выполнить: скалярное произведение (вектора разных размерностей)";
                MessageBox.Show(error);
            }
            return glas;
        }

        double fi(int j)
        {
            return p.Evaluate(f_Fi, (double)j * h);
        }

        double ro(double t)
        {
            return p.Evaluate(f_Ro, t);
        }

        double ksiI(int i, double gamma) // i = 1..n
        {
            return (double) gamma * tau * ((fi(i) - fi(i - 1)) / h - (fi(i + 1) - fi(i)) / h);
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
            for (int l = -1; l <= k; l++)
            {
                if (l == -1)
                    Uglas = (2 * x * U0 - Uminus1);
                else if (l == 0)
                    Uglas = (2 * x * Uglas - U0);
                else
                    Uglas = (2 * x * U(l - 1, x) - U(l - 2, x));
            }
            return Uglas;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pb.Value = 0;
            pb.Style = ProgressBarStyle.Continuous;
            listBoxLog.Items.Clear();
            WriteNewNumExp();
            pb.Increment(1);
            temp = "";
            for (int i = 1; i <= kol; i++ )
                GetAllParam(lambda, i, gamma, tau);
            //MessageBox.Show(temp);
            listBoxLog.Items.Clear();
            for (int i = 0; i < tempList.Count; i++ )
                listBoxLog.Items.Add(tempList[i]); 
            pb.Increment(1);

            WriteToAllExperimentsFile();
            pb.Increment(10);
            pb.Value = 100;
            //WriteTempTxtFile(@"C:\Users\Николай\Desktop\aprox\all"+Convert.ToString(kol)+".txt");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pb.Style = ProgressBarStyle.Marquee;
            temp = "";
            listBoxLog.Items.Clear();
            //listBoxTable.Items.Clear();
            tempList.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOne.Checked == true)
                radioButtonAll.Checked = false;
            else
                radioButtonAll.Checked = true;
            radioButtonsValues();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked == true)
                radioButtonOne.Checked = false;
            else
                radioButtonOne.Checked = true;
            radioButtonsValues();
        }

        void radioButtonsValues()
        {
            if (radioButtonOne.Checked)
            {
                textBoxN.ReadOnly = false;
                textBoxKol.ReadOnly = true;
                buttonOK.Enabled = false;
                buttonGo.Enabled = true;
            }
            else
            {
                textBoxN.ReadOnly = true;
                textBoxKol.ReadOnly = false;
                buttonOK.Enabled = true;
                buttonGo.Enabled = false;
            }
        }

        private void buttonProv(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
            pb.Style = ProgressBarStyle.Continuous;
            pb.Increment(10); 
            BuildAll();
            pb.Value = 100;
            MessageBox.Show("Готово!");
        }

        void BuildAll()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            //BuildRo(pane, zedGraphControl1);
            //BuildFi(pane, zedGraphControl1);
            pane.XAxis.Title.Text = "Ось абсцисс";
            pane.YAxis.Title.Text = "Ось ординат";
            pane.XAxis.Scale.Min = -1;
            pane.XAxis.Scale.Max = 1;
            string s = "J*[N](omega)";
            PointPairList list = new PointPairList();
            list = SetPoint();
            pb.Increment(30);
            LineItem myCurve;
            switch (listBoxTypeOfPoint.SelectedIndex)
            {
                case 0: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Circle); }
                    break;
                case 1: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Diamond); }
                    break;
                case 2: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Plus); }
                    break;
                case 3: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Square); }
                    break;
                case 4: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Star); }
                    break;
                case 5: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Triangle); }
                    break;
                case 6: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.TriangleDown); }
                    break;
                case 7: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.VDash); }
                    break;
                case 8: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.XCross); }
                    break;
                case 9: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.Default); }
                    break;
                case 10: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.None); }
                    break;
                default: { myCurve = pane.AddCurve(s, list, Color.Blue, SymbolType.None); }
                    break;
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        PointPairList SetPoint()
        {
            PointPairList list = new PointPairList();
            double l = 0;
            double j = 0;
            for (double omg = -1; omg <= 1; omg += eps)
            {
                l = (1+omg)/2;
                j = GetAllParam(l, N, gamma);
                list.Add(omg, j);
                listBoxLog.Items.Add("Омега = " + omg.ToString() + "; J* = " + j.ToString() + ".");
            }
            return list;
        }

        private void buttonBuildRo_Click(object sender, EventArgs e)
        {
            pb.Value = 0;
            pb.Style = ProgressBarStyle.Continuous;
            pb.Value = 100;
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            BuildRo(pane, zedGraphControl1);
            BuildFi(pane, zedGraphControl1);
        }

        void BuildRo(GraphPane paneRo, ZedGraphControl zgc)
        {
            LineItem CurveRo;
            PointPairList listRo = new PointPairList();
            double ep = eps;
            for (double i = -Math.PI; i <= Math.PI; i += ep)
            {
                listRo.Add(i, p.Evaluate(textBoxRo.Text, i));
            }
            CurveRo = paneRo.AddCurve("ro", listRo, Color.DarkGreen, SymbolType.None);
            zgc.AxisChange();
            zgc.Invalidate();
        }

        void BuildFi(GraphPane paneFi, ZedGraphControl zgc)
        {
            LineItem CurveFi;
            PointPairList listFi = new PointPairList();
            double ep = eps;
            for (double i = -Math.PI; i <= Math.PI; i += ep)
            {
                listFi.Add(i, p.Evaluate(textBoxFi.Text, i));
            }
            CurveFi = paneFi.AddCurve("fi", listFi, Color.Red, SymbolType.None);
            zgc.AxisChange();
            zgc.Invalidate();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteNewNumExp();
            StreamWriter outStream =
              new StreamWriter(Application.StartupPath + "\\txt\\" + DateTime.Now.ToString("yyyy.MM.dd HH_mm_ss") + "_сохраненный_список" + "_" + "ro=" + textBoxRo.Text + "_fi=" + textBoxFi.Text + "_N=" + Convert.ToString(N) + ".txt");
            for (int i = 0; i < listBoxLog.Items.Count; i++)
                outStream.WriteLine(listBoxLog.Items[i]);
            outStream.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        

        private void buttonShowLog_Click(object sender, EventArgs e)
        {
            if (buttonShowLog.Text == btnlog[1])
            {
                hideLog();
            }
            else if (buttonShowLog.Text == btnlog[0])
            {
                showLog();
            }
        }

        void hideLog()
        {
            groupBoxLog.Visible = false;
            buttonShowLog.Text = btnlog[0];
            Width = Size.Width - logX;
        }

        void showLog()
        {
            groupBoxLog.Visible = true;
            buttonShowLog.Text = btnlog[1];
            Width = Size.Width + logX;
        }

        void MySaveTxt()
        {
            string filename = Application.StartupPath + "\\txt\\" + GetNextExpNum().ToString() + ".txt";
            SaveFileDialog savedialog = saveFileDialog1;
            savedialog.Title = "Сохранить как ...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.FileName = filename;
            savedialog.Filter =
                "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";
            savedialog.ShowHelp = true;
            // If selected, save
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                // Get the user-selected file name
                string fileName = savedialog.FileName;
                // Get the extension
                string strFilExtn =
                    fileName.Remove(0, fileName.Length - 3);
                // Save file
                switch (strFilExtn)
                {
                    case "txt":
                        saveToTxt(fileName);
                        break;
                    default:
                        break;
                }
            }
        }

        void saveToTxt(string filename)
        {
            StreamWriter outStream;
            //для дописывания в конец файла
            FileInfo fi = new FileInfo(filename);
            outStream = fi.AppendText();
            outStream.WriteLine("Запись №" + GetNextExpNum().ToString());
            outStream.WriteLine("Время записи:" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
            for (int i = 0; i < listBoxLog.Items.Count; i++)
            {
                outStream.WriteLine(listBoxLog.Items[i]);
            }
            outStream.Close();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySaveTxt();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Пискарев Николай Сергеевич\nУдГУ ФИТиВТ 3 курс.\ne-mail:piskarew_nikolay@rambler.ru\nICQ:390247362.", "Разработчик", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void listBoxLog_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listBoxLog.Items[listBoxLog.SelectedIndex].ToString(),TextDataFormat.Text);
            lbl.Text = "Текст строки (" + listBoxLog.Items[listBoxLog.SelectedIndex].ToString() + ") скопирован в буффер";
            //MessageBox.Show(listBoxLog.Items[listBoxLog.SelectedIndex].ToString());
        }

        

    }
}
