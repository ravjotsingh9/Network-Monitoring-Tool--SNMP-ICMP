using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Windows.Forms.DataVisualization.Charting;
using System.Timers;
using System.Threading;
using System.IO;

namespace SnmpApp
{
    public partial class Form2 : Form
    {
        Int64 counter = 0;
        int aflag = 0;
         
        string appPath = Path.GetFullPath(Application.StartupPath);
        int flag = 1;
        private string ip;
        ThreadStart addDataThreadStart;
        Thread addDataRunner;
        delegate void AddDataDelegate();
        AddDataDelegate addDataDel;
        ThreadStart saveinfilestart;
        Thread saveinfilethread;
        Ping p=new Ping();
        PingReply pr;
        int x;
        string path_p;
        string path_np;
        string[] tmp = new string[10];
        Int64 max = 0, min = 0;
        Int64 tps = 0, tpr = 0, tpl = 0,alert = 0;
        double avg = 0;
        int gap = 1;//passed in seconds
        float glen = 900;
        public string interfaceforip
        {
           get
            {
                return ip;
            }
            set
            {
               ip=value;
            }
        }
        
        public Form2(ref string path_p1, ref string path_np1,ref int gap1,ref float glen1)
        {
            
            try
            {
                InitializeComponent();
                //Icon = New_ping.Properties.Resources.ajax_loader__3_;
                path_p = path_p1;
                path_np = path_np1;
                gap = gap1;
                glen = glen1;
                btn_resume.Enabled = false;
                addDataThreadStart = new ThreadStart(AddDataThreadLoop);
                addDataRunner = new Thread(addDataThreadStart);
                addDataDel += new AddDataDelegate(AddData);
                addDataRunner.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void AddDataThreadLoop()
        {
            try
            {
                if (interfaceforip == "")
                {
                    MessageBox.Show("Please try this IP again");
                    this.Close();
                }
                pr = p.Send(interfaceforip, 5000);
                if (pr == null)
                {
                    MessageBox.Show("Please try this IP again");
                    this.Close();
                }
                tps++;
                //Thread.Sleep(1000);
                while (flag == 1)
                {
                    chart1.Invoke(addDataDel);
                    pr = p.Send(interfaceforip, 10000);
                    tps++;
                    Thread.Sleep(gap*1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please try this IP again");
            }
        }
        public void AddData()
        {
            DateTime timeStamp = DateTime.Now;
            foreach (Series ptSeries in chart1.Series)
            {
                AddNewPoint(timeStamp, ptSeries);

            }
        }
        public void AddNewPoint(DateTime timeStamp, System.Windows.Forms.DataVisualization.Charting.Series ptSeries)
        {
            double newVal=0;
            string s;
            
            if (ptSeries.Points.Count >= 0)
            {
                x = Convert.ToInt16(pr.RoundtripTime);
                newVal = x;
            }
            if (tps == 1)
            {
                min = x;
            }
            if (pr.Status == IPStatus.Success)
            {
                tpr++;
                aflag = 0;
            }
            else
            {
                if (pr.Status == IPStatus.TimedOut)
                {
                    tpl++;
                    alert = tpl;
                    if (alert == 3 && aflag==0)
                    {
                        
                        try
                        {
                            // create the ProcessStartInfo using "cmd" as the program to be run,
                            // and "/c " as the parameters.
                            // Incidentally, /c tells cmd that we want it to execute the command that follows,
                            // and then exit.
                            string command = "opcmsg -id s=normal o=test msg_grp=test msg_text=testing a=test";
                            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                            // The following commands are needed to redirect the standard output.
                            // This means that it will be redirected to the Process.StandardOutput StreamReader.
                            procStartInfo.RedirectStandardOutput = true;
                            procStartInfo.UseShellExecute = false;
                            // Do not create the black window.
                            procStartInfo.CreateNoWindow = true;
                            // Now we create a process, assign its ProcessStartInfo and start it
                            System.Diagnostics.Process proc = new System.Diagnostics.Process();
                            proc.StartInfo = procStartInfo;
                            proc.Start();
                            // Get the output into a string
                            string result = proc.StandardOutput.ReadToEnd();
                            // Display the command output.
                            Console.WriteLine(result);
                            aflag = 1;
                        }
                        catch (Exception objException)
                        {
                            // Log the exception
                        }
                    }
                    
                }
                if (pr.Status == IPStatus.DestinationUnreachable)
                {
                }
            }
            s = pr.Status.ToString(); 
            if (newVal < 0)
                newVal = 0;                      
            ptSeries.Points.AddXY(timeStamp.ToOADate(), x);
            txt_log.Text = timeStamp.ToString() + " " + s + " " + x.ToString() + "\r\n" + txt_log.Text;
            tmp[counter] = ip + "\t" +  timeStamp.ToString()+"\t"+s+"\t"+x.ToString();
            counter++;
            if (counter == 10)
            {
                saveinfilestart = new ThreadStart(saveinfile);
                saveinfilethread = new Thread(saveinfilestart);
                saveinfilethread.Start();
            }
            if (x > max)
            {
                max = x;
            }
            if (x < min)
            {
                min = x;
            }
            avg = ((avg * (tps - 1)) + x) / tps;
            label9.Text = max.ToString();
            label10.Text = min.ToString();
            label11.Text = avg.ToString();
            label12.Text = tps.ToString();
            label13.Text = tpr.ToString();
            label14.Text = tpl.ToString();
            // remove all points from the source series older than 1.5 minutes.
            double removeBefore = timeStamp.AddSeconds((double)((glen*60)*gap) * (-1)).ToOADate();
            //remove oldest values to maintain a constant number of data points
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }

            chart1.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(glen*gap).ToOADate();
            chart1.Invalidate();
        }
        private void saveinfile()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(appPath+"\\" + interfaceforip + ".txt", true);
            for (int w = 0; w <counter; w++)
            {
                file.WriteLine(tmp[w]);
            }
            counter = 0;
            file.Close();
            saveinfilethread.Abort();
        }
        private void btn_exit_Click(object sender, EventArgs e)
        {
            DialogResult r;
            
            r = MessageBox.Show("Are you sure you want to exit?", ip,MessageBoxButtons.YesNo);
            if (r == DialogResult.Yes)
            {
                flag = 0;
                System.IO.StreamWriter file = new System.IO.StreamWriter(appPath+"\\" + interfaceforip + ".txt", true);
                for (int i = 0; i <= counter; i++)
                {
                    file.WriteLine(tmp[i]);
                }
                file.Close();
                if (File.Exists(path_np))
                {
                    System.IO.StreamWriter f = new System.IO.StreamWriter(path_np, true);
                    f.WriteLine(interfaceforip);
                    f.Close();
                }
                
                
                if (addDataRunner.IsAlive == true)
                {
                    addDataRunner.Suspend();
                }
                this.Close();
            }
         }

        private void btn_suspend_Click(object sender, EventArgs e)
        {
            btn_suspend.Enabled = false;
            btn_resume.Enabled = true;
            System.IO.StreamWriter file = new System.IO.StreamWriter(appPath+"\\" + interfaceforip + ".txt", true);
            for (int i = 0; i <= counter; i++)
            {
                file.WriteLine(tmp[i]);             
            }
            counter = 0;
            file.Close();
            if (addDataRunner.IsAlive == true)
            {
                addDataRunner.Suspend();

            }
        }

        private void btn_resume_Click(object sender, EventArgs e)
        {
            btn_resume.Enabled = false;
            btn_suspend.Enabled = true;
            if (addDataRunner.IsAlive == true)
            {
                addDataRunner.Resume();
            }
          
        }
       
        private void Form2_Load(object sender, EventArgs e)
        {
            lbl_ip.Text = interfaceforip;
            System.IO.StreamWriter file = new System.IO.StreamWriter(path_p, true);
            file.WriteLine(interfaceforip);
            file.Close();
            
           
        }

        
    }
}
