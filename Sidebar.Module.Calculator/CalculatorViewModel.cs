using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Resources;

namespace Sidebar.Module.Calculator
{
    [DataContract]
    public class CalculatorViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.Calculator; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.ExtraLarge; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<CalculatorViewModel>();
        }

        #endregion

        #region Fields

        private bool newDisplayRequired = false;

        private CalculationModel calculation = new CalculationModel();

        #endregion

        #region Properties

        public string Operation
        {
            get { return calculation.Operation; }
            set { calculation.Operation = value; }
        }

        public string FirstOperand
        {
            get { return calculation.FirstOperand; }
            set { calculation.FirstOperand = value; }
        }

        public string SecondOperand
        {
            get { return calculation.SecondOperand; }
            set { calculation.SecondOperand = value; }
        }

        public string Result
        {
            get { return calculation.Result; }
        }

        public virtual string Display { get; set; }

        public virtual string Expression { get; set; }

        public virtual string LastOperation { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        public virtual ObservableCollection<String> History { get; set; }

        #endregion

        #region Commands

        public void CopyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        public bool CanPasteFromClipboard()
        {
            string text = Clipboard.GetText();
            double number = 0;

            return Double.TryParse(text, out number);
        }

        public void PasteFromClipboard()
        {
            Display = Clipboard.GetText();
        }

        public void DigitButtonPress(string button)
        {
            switch (button)
            {
                case "C":
                    Display = "0";
                    FirstOperand = String.Empty;
                    SecondOperand = String.Empty;
                    Operation = String.Empty;
                    LastOperation = String.Empty;
                    Expression = String.Empty;
                    break;

                case "CE":
                    Display = "0";
                    SecondOperand = String.Empty;
                    break;

                case "Del":
                    if (Display.StartsWith("-") && Display.Length > 2)
                        Display = Display.Remove(Display.Length - 1);
                    else if (!Display.StartsWith("-") && Display.Length > 1)
                        Display = Display.Remove(Display.Length - 1);
                    else
                        Display = "0";
                    break;

                case "+/-":
                    if (Display.StartsWith("-"))
                        Display = Display.Remove(0, 1);
                    else if (Display != "0")
                        Display = "-" + Display;
                    break;

                case ".":
                    if (newDisplayRequired)
                        Display = "0.";
                    else if (!Display.Contains("."))
                        Display = Display + ".";
                    break;

                default:
                    if (Display == "0" || newDisplayRequired)
                        Display = button;
                    else
                        Display = Display + button;
                    break;
            }

            newDisplayRequired = false;
        }

        public void OperationButtonPress(string operation)
        {
            newDisplayRequired = true;
            try
            {
                if (FirstOperand == String.Empty || LastOperation == "=")
                {
                    FirstOperand = Display;
                    LastOperation = operation;

                    Expression = calculation.ToHalfExpressionString(operation);
                }
                else
                {
                    SecondOperand = Display;
                    Operation = LastOperation;
                    calculation.CalculateResult();

                    Expression = calculation.ToFullExpressionString();
                    History.Add(String.Format("{0} {1}", Expression, Result));

                    LastOperation = operation;
                    Display = Result;
                    FirstOperand = Display;
                }
            }
            catch (Exception)
            {
                Display = Result == String.Empty ? "Error" : Result;
            }
        }

        public void SingularOperationButtonPress(string operation)
        {
            newDisplayRequired = true;
            try
            {
                Operation = operation;
                SecondOperand = Display;
                calculation.CalculateResult();

                Expression = calculation.ToFullExpressionString();
                History.Add(String.Format("{0} {1}", Expression, Result));

                Display = Result;
                SecondOperand = Display;
            }
            catch (Exception)
            {
                Display = Result == String.Empty ? "Error" : Result;
            }
        }

        #endregion

        public CalculatorViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Calculator;component/Assets/Calculator.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());

            Display = "0";
            Operation = String.Empty;
            FirstOperand = String.Empty;
            SecondOperand = String.Empty;
            LastOperation = String.Empty;
            Expression = String.Empty;

            History = new ObservableCollection<String>();
        }
    }
}
