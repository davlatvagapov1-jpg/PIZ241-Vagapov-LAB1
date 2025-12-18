using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DichotomySolver
{
    public partial class Form1 : Form
    {
        private MenuStrip menuStrip;
        private ToolStripMenuItem calculateMenuItem;
        private ToolStripMenuItem clearMenuItem;
        private ToolStripMenuItem exitMenuItem;

        private TextBox txtA;
        private TextBox txtB;
        private TextBox txtEpsilon;
        private TextBox txtFunction;
        private TextBox txtResult;

        private Label lblA;
        private Label lblB;
        private Label lblEpsilon;
        private Label lblFunction;
        private Label lblResult;

        private PictureBox graphPictureBox;
        private Panel inputPanel;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Dichotomy Method - Finding Minimum";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            menuStrip = new MenuStrip();

            calculateMenuItem = new ToolStripMenuItem("Рассчитать");
            calculateMenuItem.Click += CalculateMenuItem_Click;

            clearMenuItem = new ToolStripMenuItem("Очистить");
            clearMenuItem.Click += ClearMenuItem_Click;

            exitMenuItem = new ToolStripMenuItem("Выход");
            exitMenuItem.Click += (s, e) => Application.Exit();

            menuStrip.Items.AddRange(new ToolStripItem[] {
                calculateMenuItem,
                clearMenuItem,
                exitMenuItem
            });

            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            inputPanel = new Panel();
            inputPanel.Location = new Point(10, 40);
            inputPanel.Size = new Size(300, 250);
            inputPanel.BorderStyle = BorderStyle.FixedSingle;

            int yPos = 20;

            lblFunction = new Label();
            lblFunction.Text = "Функция f(x):";
            lblFunction.Location = new Point(10, yPos);
            lblFunction.Size = new Size(100, 20);
            inputPanel.Controls.Add(lblFunction);

            txtFunction = new TextBox();
            txtFunction.Location = new Point(120, yPos);
            txtFunction.Size = new Size(160, 20);
            txtFunction.Text = "x^2 - 4*x + 4";
            inputPanel.Controls.Add(txtFunction);

            yPos += 40;

            lblA = new Label();
            lblA.Text = "Начало интервала (a):";
            lblA.Location = new Point(10, yPos);
            lblA.Size = new Size(100, 20);
            inputPanel.Controls.Add(lblA);

            txtA = new TextBox();
            txtA.Location = new Point(120, yPos);
            txtA.Size = new Size(160, 20);
            txtA.Text = "0";
            inputPanel.Controls.Add(txtA);

            yPos += 40;

            lblB = new Label();
            lblB.Text = "Конец интервала (b):";
            lblB.Location = new Point(10, yPos);
            lblB.Size = new Size(100, 20);
            inputPanel.Controls.Add(lblB);

            txtB = new TextBox();
            txtB.Location = new Point(120, yPos);
            txtB.Size = new Size(160, 20);
            txtB.Text = "5";
            inputPanel.Controls.Add(txtB);

            yPos += 40;

            lblEpsilon = new Label();
            lblEpsilon.Text = "Точность (e):";
            lblEpsilon.Location = new Point(10, yPos);
            lblEpsilon.Size = new Size(100, 20);
            inputPanel.Controls.Add(lblEpsilon);

            txtEpsilon = new TextBox();
            txtEpsilon.Location = new Point(120, yPos);
            txtEpsilon.Size = new Size(160, 20);
            txtEpsilon.Text = "0.001";
            inputPanel.Controls.Add(txtEpsilon);

            Panel resultPanel = new Panel();
            resultPanel.Location = new Point(10, 300);
            resultPanel.Size = new Size(300, 100);
            resultPanel.BorderStyle = BorderStyle.FixedSingle;

            lblResult = new Label();
            lblResult.Text = "Результат:";
            lblResult.Location = new Point(10, 10);
            lblResult.Size = new Size(280, 20);
            resultPanel.Controls.Add(lblResult);

            txtResult = new TextBox();
            txtResult.Location = new Point(10, 40);
            txtResult.Size = new Size(280, 60);
            txtResult.Multiline = true;
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.ReadOnly = true;
            resultPanel.Controls.Add(txtResult);

            graphPictureBox = new PictureBox();
            graphPictureBox.Location = new Point(320, 40);
            graphPictureBox.Size = new Size(550, 510);
            graphPictureBox.BorderStyle = BorderStyle.FixedSingle;
            graphPictureBox.BackColor = Color.White;

            this.Controls.Add(inputPanel);
            this.Controls.Add(resultPanel);
            this.Controls.Add(graphPictureBox);
        }

        private double DichotomyMethod(Func<double, double> f, double a, double b, double epsilon)
        {
            double delta = epsilon / 2;

            while (Math.Abs(b - a) > epsilon)
            {
                double x1 = (a + b - delta) / 2;
                double x2 = (a + b + delta) / 2;

                double f1 = f(x1);
                double f2 = f(x2);

                if (f1 < f2)
                {
                    b = x2;
                }
                else
                {
                    a = x1;
                }
            }

            return (a + b) / 2;
        }

        private string ConvertPowerToMultiplication(string expression)
        {
            string result = expression;

            string pattern = @"([a-zA-Z0-9\.\(\)]+)\^(\d+)";

            MatchCollection matches = Regex.Matches(result, pattern);

            List<Match> matchList = new List<Match>();
            foreach (Match match in matches)
            {
                matchList.Add(match);
            }

            for (int i = matchList.Count - 1; i >= 0; i--)
            {
                Match match = matchList[i];
                string baseExpr = match.Groups[1].Value;
                int exponent = int.Parse(match.Groups[2].Value);

                string multiplication = "";
                for (int j = 0; j < exponent; j++)
                {
                    if (j > 0) multiplication += "*";
                    multiplication += baseExpr;
                }

                if (exponent == 1)
                {
                    multiplication = baseExpr;
                }
                else if (exponent == 0)
                {
                    multiplication = "1";
                }

                result = result.Remove(match.Index, match.Length);
                result = result.Insert(match.Index, multiplication);
            }

            return result;
        }

        private string PrepareExpression(string expression, double xValue)
        {
            string result = expression;

            result = result.Replace(" ", "");

            result = ConvertPowerToMultiplication(result);

            result = result.Replace("x", xValue.ToString(System.Globalization.CultureInfo.InvariantCulture));

            result = Regex.Replace(result, @"(\d),(\d)", "$1.$2");

            return result;
        }

        private Func<double, double> ParseFunction(string functionString)
        {
            return x =>
            {
                try
                {
                    string expression = PrepareExpression(functionString, x);

                    object result = new DataTable().Compute(expression, null);

                    if (result is double)
                        return (double)result;
                    else if (result is decimal)
                        return (double)(decimal)result;
                    else if (result is int)
                        return (double)(int)result;
                    else
                        return Convert.ToDouble(result);
                }
                catch
                {
                    throw new ArgumentException("Некорректное выражение функции");
                }
            };
        }

        private void DrawGraph(Func<double, double> f, double a, double b, double minX)
        {
            Bitmap bitmap = new Bitmap(graphPictureBox.Width, graphPictureBox.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                double step = (b - a) / 100;
                double minY = double.MaxValue;
                double maxY = double.MinValue;

                for (double x = a; x <= b; x += step)
                {
                    try
                    {
                        double y = f(x);
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                    catch { }
                }

                if (double.IsInfinity(minY) || double.IsNaN(minY) ||
                    double.IsInfinity(maxY) || double.IsNaN(maxY) ||
                    Math.Abs(maxY - minY) < 1e-10)
                {
                    minY = -10;
                    maxY = 10;
                }

                double yRange = maxY - minY;
                minY -= yRange * 0.1;
                maxY += yRange * 0.1;
                yRange = maxY - minY;

                float scaleX = graphPictureBox.Width / (float)(b - a);
                float scaleY = graphPictureBox.Height / (float)yRange;

                Pen axisPen = new Pen(Color.Black, 1);

                float yZero = graphPictureBox.Height - (float)(-minY) * scaleY;
                if (yZero > 0 && yZero < graphPictureBox.Height)
                {
                    g.DrawLine(axisPen, 0, yZero, graphPictureBox.Width, yZero);

                    for (double x = a; x <= b; x += (b - a) / 10)
                    {
                        float screenX = (float)(x - a) * scaleX;
                        g.DrawLine(axisPen, screenX, yZero - 3, screenX, yZero + 3);
                        g.DrawString(x.ToString("F1"), new Font("Arial", 7),
                            Brushes.Black, screenX - 10, yZero + 5);
                    }
                }

                float xZero = (float)(-a) * scaleX;
                if (xZero > 0 && xZero < graphPictureBox.Width)
                {
                    g.DrawLine(axisPen, xZero, 0, xZero, graphPictureBox.Height);

                    for (double y = minY; y <= maxY; y += yRange / 10)
                    {
                        float screenY = graphPictureBox.Height - (float)(y - minY) * scaleY;
                        g.DrawLine(axisPen, xZero - 3, screenY, xZero + 3, screenY);
                        g.DrawString(y.ToString("F1"), new Font("Arial", 7),
                            Brushes.Black, xZero + 5, screenY - 7);
                    }
                }

                Pen graphPen = new Pen(Color.Blue, 2);
                PointF? prevPoint = null;

                for (double x = a; x <= b; x += step)
                {
                    try
                    {
                        double y = f(x);
                        if (double.IsNaN(y) || double.IsInfinity(y))
                            continue;

                        float screenX = (float)(x - a) * scaleX;
                        float screenY = graphPictureBox.Height - (float)(y - minY) * scaleY;

                        PointF currentPoint = new PointF(screenX, screenY);

                        if (prevPoint.HasValue)
                        {
                            g.DrawLine(graphPen, prevPoint.Value, currentPoint);
                        }

                        prevPoint = currentPoint;
                    }
                    catch
                    {
                        prevPoint = null;
                    }
                }

                if (minX >= a && minX <= b)
                {
                    try
                    {
                        double minYValue = f(minX);
                        float screenMinX = (float)(minX - a) * scaleX;
                        float screenMinY = graphPictureBox.Height - (float)(minYValue - minY) * scaleY;

                        g.FillEllipse(Brushes.Red, screenMinX - 4, screenMinY - 4, 8, 8);

                        string label = $"min: ({minX:F3}, {minYValue:F3})";
                        g.DrawString(label, new Font("Arial", 8), Brushes.Red,
                            screenMinX + 5, screenMinY - 10);
                    }
                    catch { }
                }

                g.DrawString($"f(x) = {txtFunction.Text}",
                    new Font("Arial", 10), Brushes.Black, 10, 10);
                g.DrawString($"Интервал: [{a:F2}, {b:F2}]",
                    new Font("Arial", 9), Brushes.Black, 10, 30);
                g.DrawString($"Точность: {txtEpsilon.Text}",
                    new Font("Arial", 9), Brushes.Black, 10, 50);
            }

            graphPictureBox.Image = bitmap;
        }

        private void CalculateMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFunction.Text))
                {
                    MessageBox.Show("Введите функцию f(x)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(txtA.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение a", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(txtB.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение b", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(txtEpsilon.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double epsilon))
                {
                    MessageBox.Show("Некорректное значение точности e", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (epsilon <= 0)
                {
                    MessageBox.Show("Точность должна быть положительным числом", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("a должно быть меньше b", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Func<double, double> function = ParseFunction(txtFunction.Text);

                try
                {
                    double test1 = function(a);
                    double test2 = function(b);
                    double test3 = function((a + b) / 2);

                    if (double.IsNaN(test1) || double.IsInfinity(test1) ||
                        double.IsNaN(test2) || double.IsInfinity(test2) ||
                        double.IsNaN(test3) || double.IsInfinity(test3))
                    {
                        throw new Exception("Функция возвращает нечисловое значение");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка в функции: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double minX = DichotomyMethod(function, a, b, epsilon);
                double minY = function(minX);

                txtResult.Text = $"Найденный минимум:" + Environment.NewLine +
                               $"x = {minX:F6}" + Environment.NewLine +
                               $"f(x) = {minY:F6}" + Environment.NewLine +
                               $"Точность: {epsilon}" + Environment.NewLine +
                               $"Интервал: [{a}, {b}]";

                DrawGraph(function, a, b, minX);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            txtFunction.Text = "x^2 - 4*x + 4";
            txtA.Text = "0";
            txtB.Text = "5";
            txtEpsilon.Text = "0.001";
            txtResult.Clear();

            if (graphPictureBox.Image != null)
            {
                graphPictureBox.Image.Dispose();
                graphPictureBox.Image = null;
            }

            using (Graphics g = graphPictureBox.CreateGraphics())
            {
                g.Clear(Color.White);
            }
        }
    }
}