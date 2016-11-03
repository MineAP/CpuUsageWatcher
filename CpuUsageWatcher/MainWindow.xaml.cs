using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CpuUsageWatcher
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private BackgroundWorker worker;
        private PerformanceCounter cpuUsageCounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.Top = 0;
            this.Left = 0;
            
            foreach (PerformanceCounterCategory c in PerformanceCounterCategory.GetCategories())
            {
                if (c.CategoryName == "Processor")
                {
                    foreach (PerformanceCounter pc in c.GetCounters("_Total"))
                    {
                        
                        if (pc.CounterName == "% Processor Time")
                        {
                            this.cpuUsageCounter = pc;

                            Console.WriteLine("CategoryName:{0:s}", pc.CategoryName);
                            Console.WriteLine("CounterName:{0:s}", pc.CounterName);
                            Console.WriteLine("InstanceName:{0:s}", pc.InstanceName);

                            break;
                        }
                    }
                }
            }

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync();
            
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelCpuUsage.Content = String.Format("{0:d}%", e.ProgressPercentage); 
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            while (true)
            {

                if (worker.CancellationPending)
                {
                    return;
                }

                Thread.Sleep(1000);

                if (this.cpuUsageCounter != null)
                {
                    float value = cpuUsageCounter.NextValue();
                    Console.WriteLine(String.Format("現在のCPU使用率は {0:f}% です。", value));
                    worker.ReportProgress((int)value);
                }

            }
            
        }
        
    }
}
