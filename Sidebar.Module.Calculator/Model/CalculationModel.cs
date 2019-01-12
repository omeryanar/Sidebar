using System;
using System.Globalization;

namespace Sidebar.Module.Calculator
{
    public class CalculationModel
    {
        #region Constructors

        public CalculationModel(string firstOperand, string secondOperand, string operation)
        {
            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            Operation = operation;
            Result = String.Empty;
        }

        public CalculationModel(string firstOperand, string operation)
        {
            FirstOperand = firstOperand;
            SecondOperand = String.Empty;
            Operation = operation;
            Result = String.Empty;
        }

        public CalculationModel()
        {
            FirstOperand = String.Empty;
            SecondOperand = String.Empty;
            Operation = String.Empty;
            Result = String.Empty;
        }

        #endregion

        #region Properties

        public string FirstOperand { get; set; }

        public string SecondOperand { get; set; }

        public string Operation { get; set; }

        public string Result { get; private set; }

        #endregion

        public void CalculateResult()
        {
            try
            {
                switch (Operation)
                {
                    case ("+"):
                        Result = (FirstOperand.ToDouble() + SecondOperand.ToDouble()).ToDisplayString(); 
                        break;

                    case ("-"):
                        Result = (FirstOperand.ToDouble() - SecondOperand.ToDouble()).ToDisplayString();
                        break;

                    case ("*"):
                        Result = (FirstOperand.ToDouble() * SecondOperand.ToDouble()).ToDisplayString();
                        break;

                    case ("/"):
                        Result = (FirstOperand.ToDouble() / SecondOperand.ToDouble()).ToDisplayString();
                        break;

                    case ("%"):
                        Result = (FirstOperand.ToDouble() * SecondOperand.ToDouble() / 100D).ToDisplayString();
                        break;

                    case ("√"):
                        Result = Math.Sqrt(SecondOperand.ToDouble()).ToDisplayString();
                        break;

                    case ("1/x"):
                        Result = (1 / (SecondOperand.ToDouble())).ToDisplayString();
                        break;
                }
            }
            catch (Exception)
            {
                Result = "Error";
            }
        }
    }

    public static class Extensions
    {
        public static double ToDouble(this string text)
        {
            if (text == "∞")
                return Double.PositiveInfinity;
            if (text == "-∞")
                return Double.NegativeInfinity;
            if (text == "?")
                return Double.NaN;

            return Convert.ToDouble(text, CultureInfo.InvariantCulture);
        }

        public static string ToDisplayString(this double number)
        {
            if (Double.IsPositiveInfinity(number))
                return "∞";
            if (Double.IsNegativeInfinity(number))
                return "-∞";
            if (Double.IsNaN(number))
                return "?";

            return Math.Round(number, 10).ToString(CultureInfo.InvariantCulture);
        }

        public static string ToHalfExpressionString(this CalculationModel model)
        {
            return model.ToHalfExpressionString(model.Operation);
        }

        public static string ToHalfExpressionString(this CalculationModel model, string operation)
        {
            if (operation != String.Empty && operation != "=")
                return String.Format("{0} {1}", model.FirstOperand, operation);
            else
                return String.Empty;
        }

        public static string ToFullExpressionString(this CalculationModel model)
        {
            switch (model.Operation)
            {
                case "1/x":
                    return String.Format("1 / {0} =", model.SecondOperand);
                case "√":
                    return String.Format("{0}({1}) =", model.Operation, model.SecondOperand);
                default:
                    return String.Format("{0} {1} {2} =", model.FirstOperand, model.Operation, model.SecondOperand);
            }
        }
    }
}
