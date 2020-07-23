/* Title:           Calculating Permit Pricing
 * Date:            7-12-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to calculate the price */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewEventLogDLL;
using DesignPermitsDLL;

namespace CalculatingPermitPricing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DesignPermitsClass TheDesignPermitsClass = new DesignPermitsClass();

        //setting up the data
        DesignPermitsDataSet TheDesignPermitsDataSet = new DesignPermitsDataSet();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheDesignPermitsDataSet = TheDesignPermitsClass.GetDesignPermitsInfo();

            dgrResults.ItemsSource = TheDesignPermitsDataSet.designpermits;
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            decimal decPermitCost;
            decimal decPermitPrice;
            int intTransactionID;
            bool blnFatalError = false;

            try
            {
                intNumberOfRecords = TheDesignPermitsDataSet.designpermits.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheDesignPermitsDataSet.designpermits[intCounter].IsPermitCostNull() == false)
                    {
                        decPermitCost = TheDesignPermitsDataSet.designpermits[intCounter].PermitCost;
                        intTransactionID = TheDesignPermitsDataSet.designpermits[intCounter].TransactionID;

                        decPermitPrice = decPermitCost + (decPermitCost * Convert.ToDecimal(.15));
                        decPermitPrice = Math.Round(decPermitPrice, 2);

                        blnFatalError = TheDesignPermitsClass.UpdateDesignProjectPermitCost(intTransactionID, decPermitCost, decPermitPrice);

                        if (blnFatalError == true)
                            throw new Exception();

                    }
                }

                TheDesignPermitsDataSet = TheDesignPermitsClass.GetDesignPermitsInfo();

                dgrResults.ItemsSource = TheDesignPermitsDataSet.designpermits;

                TheMessagesClass.InformationMessage("Records Have Been Updated");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Calculating Permit Pricing // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
