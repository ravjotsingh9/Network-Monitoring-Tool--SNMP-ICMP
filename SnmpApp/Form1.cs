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
using System.Globalization;
using System.Diagnostics;

namespace SnmpApp
{
    public partial class Form1 : Form
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SNMP
        Thread snmp_thread1;
        string snmp_ip;
        string snmp_para;
        string snmp_interfacenum;
        ThreadStart snmp_threadstart;


        snmp snmp_conn = new snmp();
        byte[] snmp_response = new byte[1024];
        int snmp_commlength, snmp_miblength, snmp_datatype, snmp_datalength,snmp_datastart;
        int snmp_value = 0;
        byte[] snmp_response1 = new byte[8192];
        int snmp_commlength1, snmp_miblength1, snmp_datatype1, snmp_datalength1, snmp_datastart1;
        int snmp_value1 = 0;
        string snmp_output1;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //Form3 f3 = new Form3();
        Ping p=new Ping();
        Thread thread1;
        PingReply pr;
        string appPath=Path.GetFullPath(Application.StartupPath);
        string path_p;
        static string path_np;
        string tosend;
        string tosnd;
        int Currently_Pinging = 0;
        int Currently_nPinging = 0;
        
        string[] lines;
        Thread th;
        ThreadStart ts;
        delegate void datadelegate();
        datadelegate datadel;
        delegate void n_datadelegate();
        n_datadelegate n_datadel;
        Ping p1 = new Ping();
        PingReply pr1;
        string[] alive;
        string[] nalive;
        long x;
        int a = 0;
        int n = 0;
        int num = 0;


        Thread pth;
        ThreadStart pthstart;
        delegate void deleg();
        deleg pdel;
        int no_of_ping=0;
        Ping p2 = new Ping();
        PingReply pr2;
        long time_to_return = 0;
        int gap1 = 1;
        float glen = 15;
      
        ///////////////////////////////////////////////////////////////task scheduler
        string appPath_log = Path.GetFullPath(Application.StartupPath);
        int log_counter = 0;
        Thread plog;
        ThreadStart plogstr;
        //ThreadStart saveinfilestart;
        //Thread saveinfilethread;
        string[] log_content = new string[30];
        /////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
            ////////////////////////////////////////////////////task sceduler
            _scheduler = new scheduler();
            dateTimePickerStartDate.Value = DateTime.Today;
            dateTimePickerEndDate.Value = DateTime.Today.AddYears(1);
            /////////////////////////////////////////////////////
            try
            {
                //Icon = SnmpApp.Properties.Resources.ajax_loader__3_;
                btn_fcontent.Enabled = false;
                btn_chk_resume.Enabled = false;
                btn_chk_start.Enabled = false;
                btn_chk_stop.Enabled = false;
                path_p = string.Copy(appPath + "\\Currently_Pinging.txt");
                path_np = string.Copy(appPath + "\\Currently_nPinging.txt");
                System.IO.StreamWriter file1 = new System.IO.StreamWriter(path_p);
                file1.Close();
                System.IO.StreamWriter file2 = new System.IO.StreamWriter(path_np);
                file2.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem in Temporary File creation: Exit and Try to Restart the Application");
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tosnd = this.txt_ip.Text;
            Currently_Pinging = 0;
            Currently_nPinging = 0;
            int pno_of_occurence = 0;
            int npno_of_occurence = 0;
            string line;

            if (tosnd != "")
            {
                StreamReader r = new StreamReader(path_p);
                while ((line=r.ReadLine()) != null)
                {
                    if (line == tosnd)
                    {
                        Currently_Pinging = 1;
                        pno_of_occurence++;
                    }
                }
                r.Close();

                StreamReader r1 = new StreamReader(path_np);
                while ((line=r1.ReadLine()) != null)
                {
                    if (line == tosnd)
                    {
                        Currently_nPinging = 1;
                        npno_of_occurence++;
                    }
                }
                r1.Close();
                if (((Currently_Pinging == 1) && (Currently_nPinging == 1) && (pno_of_occurence == npno_of_occurence)) || ((Currently_Pinging == 0) && (Currently_nPinging == 0)))
                {
                    try
                    {
                        if (comboBox2.SelectedItem == "1 Sec")
                        {
                            gap1 = 1;
                            glen = 15;
                        }
                        else
                        {
                            if (comboBox2.SelectedItem == "5 Sec")
                            {
                                gap1 = 5;
                                glen = 15;
                            }
                            else
                            {
                                if (comboBox2.SelectedItem == "30 Sec")
                                {
                                    gap1 = 30;
                                    glen = 15;
                                }
                                else
                                {
                                    if (comboBox2.SelectedItem == "5 Min")
                                    {
                                        gap1 = 300;
                                        if (comboBox4.SelectedItem == "1 Hour")
                                        {
                                            glen = 0.2F;
                                        }
                                        else
                                        {
                                            if (comboBox4.SelectedItem == "2 Hour")
                                            {
                                                glen = 0.4F;
                                            }
                                            else
                                            {
                                                if (comboBox4.SelectedItem == "3 Hour")
                                                {
                                                    glen = 0.6F;
                                                }
                                                else
                                                {
                                                    if (comboBox4.SelectedItem == "4 Hour")
                                                    {
                                                        glen = 0.8F;
                                                    }
                                                    else
                                                    {
                                                        if (comboBox4.SelectedItem == "5 Hour")
                                                        {
                                                            glen = 1.0F;
                                                        }
                                                        else
                                                        {
                                                            if (comboBox4.SelectedItem == "6 Hour")
                                                            {
                                                                glen = 1.2F;
                                                            }
                                                            else
                                                            {
                                                                if (comboBox4.SelectedItem == "12 Hour")
                                                                {
                                                                    glen = 2.4F;
                                                                }
                                                                else
                                                                {
                                                                    if (comboBox4.SelectedItem == "18 Hour")
                                                                    {
                                                                        glen = 3.6F;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (comboBox4.SelectedItem == "24 Hour")
                                                                        {
                                                                            glen = 7.2F;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (comboBox4.SelectedItem == "48 Hour")
                                                                            {
                                                                                glen = 9.6F;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        label24.Visible = false;
                        comboBox4.Visible = false;
                        pr = p.Send(tosnd);
                        ThreadStart threadstart = new ThreadStart(startmethod);
                        thread1 = new Thread(threadstart);
                        Currently_Pinging = 0;
                        Currently_nPinging = 0;
                        thread1.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    if (((Currently_Pinging == 1) && (Currently_nPinging == 0))||(pno_of_occurence>npno_of_occurence))
                    {
                        MessageBox.Show("This IP is Already Pinging");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ping Needs IP Address");
            }
        }        
        private void startmethod()
        {
            try
            {
                Form2 f2 = new Form2(ref path_p, ref path_np,ref gap1,ref glen);
                f2.interfaceforip = tosnd;
                f2.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
         }       

        private void btn_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txt_path.Text = openFileDialog1.FileName;
                btn_fcontent.Enabled = true;
            }
            /*
            if (f3.Created == false)
            {   
                btn_fcontent.Enabled = true;
            }
             */ 
        }

        private void btn_fcontent_Click(object sender, EventArgs e)
        {
            btn_fcontent.Enabled = false;
            tosend = txt_path.Text;
            
            //f3.interfaceforpath = tosend;
            //f3.Show();
            txt_fcontent.Clear();
            //txt_alive.Clear();
            //txt_nalive.Clear();

            lines = System.IO.File.ReadAllLines(tosend);
            foreach (string line in lines)
            {
                txt_fcontent.AppendText(line + "\r\n");
                num++;
            }
            alive = new string[num];
            nalive = new string[num];
            btn_chk_start.Enabled = true;
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
           
            try
            {
                string line1;
                int running = 0;
                int nrunning = 0;
                StreamReader r = new StreamReader(path_p);
                while ((line1 = r.ReadLine()) != null)
                {
                    running++;
                }
                r.Close();

                StreamReader r1 = new StreamReader(path_np);
                while ((line1 = r1.ReadLine()) != null)
                {

                    nrunning++;
                }
                r1.Close();
                if (running == nrunning)
                {
                    File.Delete(path_p);
                    File.Delete(path_np);
                    try
                    {
                        Process[] proc = Process.GetProcessesByName("New_ping");
                        foreach (Process p in proc)
                        {
                            p.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                    //thread1.Abort();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cannot Exit! There are some IPs Pinging");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("This Application may already be running.Close all of them and try again.");
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txt_ip_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (txt_snmp_ip.Text == "" || txt_cs.Text== "")
            {
                MessageBox.Show("Please Provide both IP Address and Community String");
                return;
            }
            comboBox9.Items.Clear();
            comboBox10.Items.Clear();
            snmp_value = 0;
            string[] oids = new string[5];
            oids[0] = "1.3.6.1.2.1.2.1.0";
            snmp_response = snmp_conn.get("get", txt_snmp_ip.Text, txt_cs.Text, oids, 1);
            if (snmp_response[0] == 0xff)
            {
                Console.WriteLine("No response");
                return;
            }
            snmp_commlength = Convert.ToInt16(snmp_response[6]);
            snmp_miblength = Convert.ToInt16(snmp_response[23 + snmp_commlength]);
            snmp_datatype = Convert.ToInt16(snmp_response[24 + snmp_commlength + snmp_miblength]);
            snmp_datalength = Convert.ToInt16(snmp_response[25 + snmp_commlength + snmp_miblength]);
            snmp_datastart = 26 + snmp_commlength + snmp_miblength;
            while (snmp_datalength > 0)
            {
                snmp_value = (snmp_value << 8) + snmp_response[snmp_datastart++];
                snmp_datalength--;
            }
            int no_of_39=0, remainder = 0;
            if (snmp_value > 39)
            {
                no_of_39 = snmp_value / 39;
                remainder = snmp_value % 39;
            }
            else
            {
                remainder = snmp_value % 39;
            }
            int msglen = 0, getpdulen = 0, seqlen = 0;
            int objid1, obj1, dstart1, dlen1, dlenbyte = 0, objidlen = 0, interface_num = 0, interface_num_len = 0;
            
            
            
            for (int n = 0; n < no_of_39; n++)
            {
                for (int l = 0; l < 1; l++)
                {
                    oids[l] = "1.3.6.1.2.1.2.2.1.2." + interface_num;
                }
                snmp_response1 = snmp_conn.getbulk(txt_snmp_ip.Text, txt_cs.Text, oids, 1, 39);

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (snmp_response1[0] == 0xff)
                {
                    Console.WriteLine("No response");
                    textBox2.AppendText("\r\n" + snmp_conn.console);
                    textBox2.AppendText("\r\nno response");
                    return;
                }
                msglen = 0; getpdulen = 0; seqlen = 0;
                if (snmp_response1[1] > 127)
                {
                    msglen = snmp_response1[1] - 128;
                }

                snmp_commlength1 = Convert.ToInt16(snmp_response1[6 + msglen]);
                if (snmp_response1[8 + snmp_commlength1 + msglen] > 127)
                {
                    getpdulen = snmp_response1[8 + snmp_commlength1 + msglen] - 128;
                }
                if (snmp_response1[19 + snmp_commlength1 + msglen + getpdulen] > 127)
                {
                    seqlen = snmp_response1[19 + snmp_commlength1 + msglen + getpdulen] - 128;
                }


                objid1 = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; dlenbyte = 0; objidlen = 0; interface_num = 0; interface_num_len = 0;
                snmp_datastart1 = 20 + snmp_commlength1 + msglen + getpdulen + seqlen;
                for (int l = 0; l < 39; l++)
                {
                    snmp_value1 = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; objid1 = 0; dlenbyte = 0; objidlen = 0; interface_num = 0; interface_num_len = 0;
                    objid1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 1]);
                    if (objid1 > 127)
                    {
                        objidlen = objid1 - 128;
                        objid1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 1 + 1]);
                    }
                    obj1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen]);
                    interface_num = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1]);
                    if (interface_num > 127)
                    {
                        interface_num_len = interface_num - 128;
                        interface_num = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1 + 1]);
                        if (interface_num_len >= 2)
                        {
                            if (interface_num_len > 2)
                            {
                                interface_num = interface_num * 256;
                                interface_num = interface_num + Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1 + 1 + 1]);
                            }
                            else
                            {
                                interface_num = interface_num * 256;
                            }
                        }
                    }
                    snmp_datatype1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 1 + objidlen + interface_num_len]);
                    snmp_datalength1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 2 + objidlen + interface_num_len]);
                    if (snmp_datalength1 > 127)
                    {
                        dlenbyte = snmp_datalength1 - 128;
                        snmp_datalength1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 2 + objidlen + interface_num_len + 1]);
                    }
                    snmp_datastart1 = snmp_datastart1 + 3 + obj1 + 3 + dlenbyte + objidlen + interface_num_len;
                    dstart1 = snmp_datastart1;
                    //textBox2.AppendText("\r\nStartbyte " + dstart1.ToString());
                    dlen1 = snmp_datalength1;
                    //textBox2.AppendText("\r\nlength " + dlen1.ToString());
                    while (snmp_datalength1 > 0)
                    {
                        snmp_value1 = (snmp_value1 << 8) + snmp_response1[snmp_datastart1++];
                        snmp_datalength1--;
                    }

                    snmp_output1 = Encoding.ASCII.GetString(snmp_response1, dstart1, dlen1);
                    textBox2.AppendText("\r\n" + interface_num+" "+snmp_output1);
                    //comboBox4.Items.Clear();
                    comboBox9.Items.AddRange(new object[] {interface_num});
                    comboBox10.Items.AddRange(new object[] { interface_num });
                    //textBox2.AppendText("\r\n" + snmp_value1 );
                    textBox2.AppendText("\r\n" + "---------------------------------------------------------------------------------------------------------------------");
                }
            }

            //Remainder

            for (int l = 0; l < 1; l++)
            {
                oids[l] = "1.3.6.1.2.1.2.2.1.2." + interface_num;
            }
            snmp_response1 = snmp_conn.getbulk(txt_snmp_ip.Text, txt_cs.Text, oids, 1, remainder);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (snmp_response1[0] == 0xff)
            {
                Console.WriteLine("No response");
                textBox2.AppendText("\r\n" + snmp_conn.console);
                textBox2.AppendText("\r\nno response");
                return;
            }
            msglen = 0; getpdulen = 0; seqlen = 0;
            if (snmp_response1[1] > 127)
            {
                msglen = snmp_response1[1] - 128;
            }

            snmp_commlength1 = Convert.ToInt16(snmp_response1[6 + msglen]);
            if (snmp_response1[8 + snmp_commlength1 + msglen] > 127)
            {
                getpdulen = snmp_response1[8 + snmp_commlength1 + msglen] - 128;
            }
            if (snmp_response1[19 + snmp_commlength1 + msglen + getpdulen] > 127)
            {
                seqlen = snmp_response1[19 + snmp_commlength1 + msglen + getpdulen] - 128;
            }


            //int objid1, obj1, dstart1, dlen1, dlenbyte = 0, objidlen = 0, interface_num = 0, interface_num_len = 0;
            objid1 = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; dlenbyte = 0; objidlen = 0; interface_num = 0; interface_num_len = 0;
            snmp_datastart1 = 20 + snmp_commlength1 + msglen + getpdulen + seqlen;
            for (int l = 0; l < remainder; l++)
            {
                snmp_value1 = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; objid1 = 0; dlenbyte = 0; objidlen = 0; interface_num = 0; interface_num_len = 0;
                objid1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 1]);
                if (objid1 > 127)
                {
                    objidlen = objid1 - 128;
                    objid1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 1 + 1]);
                }
                obj1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen]);
                interface_num = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1]);
                if (interface_num > 127)
                {
                    interface_num_len = interface_num - 128;
                    interface_num = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1 + 1]);
                    if (interface_num_len >= 2)
                    {
                        if (interface_num_len > 2)
                        {
                            interface_num = interface_num * 256;
                            interface_num = interface_num + Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1 + 1 + 1]);
                        }
                        else
                        {
                            interface_num = interface_num * 256;
                        }
                    }
                }
                snmp_datatype1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 1 + objidlen + interface_num_len]);
                snmp_datalength1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 2 + objidlen + interface_num_len]);
                if (snmp_datalength1 > 127)
                {
                    dlenbyte = snmp_datalength1 - 128;
                    snmp_datalength1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 2 + objidlen + interface_num_len + 1]);
                }
                snmp_datastart1 = snmp_datastart1 + 3 + obj1 + 3 + dlenbyte + objidlen + interface_num_len;
                dstart1 = snmp_datastart1;
                //textBox2.AppendText("\r\nStartbyte " + dstart1.ToString());
                dlen1 = snmp_datalength1;
                //textBox2.AppendText("\r\nlength " + dlen1.ToString());
                while (snmp_datalength1 > 0)
                {
                    snmp_value1 = (snmp_value1 << 8) + snmp_response1[snmp_datastart1++];
                    snmp_datalength1--;
                }

                snmp_output1 = Encoding.ASCII.GetString(snmp_response1, dstart1, dlen1);
                textBox2.AppendText("\r\n" +interface_num+" "+ snmp_output1);
                comboBox9.Items.AddRange(new object[] { interface_num });
                comboBox10.Items.AddRange(new object[] { interface_num });
                //textBox2.AppendText("\r\n" + snmp_value1 );
                textBox2.AppendText("\r\n" + "---------------------------------------------------------------------------------------------------------------------");
            }
        }
        int snmp_gap = 10;
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox3.SelectedItem == null || comboBox9.SelectedItem == null)
            {
                MessageBox.Show("Please Select Value in all the Three Dropdowns");
                return;
            }
            snmp_ip = this.txt_snmp_ip.Text;

            if (comboBox1.SelectedItem == "IN Bits")
            {
                snmp_interfacenum = comboBox9.SelectedItem.ToString();
                snmp_para = "1.3.6.1.2.1.2.2.1.10." + snmp_interfacenum;

            }
            else
            {
                if (comboBox1.SelectedItem == "OUT Bits")
                {
                    snmp_interfacenum = comboBox9.SelectedItem.ToString();
                    snmp_para = "1.3.6.1.2.1.2.2.1.16." + snmp_interfacenum;
                }
                else
                {
                    if (comboBox1.SelectedItem == "Interface Utilization Percentage")
                    {
                        snmp_para = "4" + comboBox9.SelectedItem.ToString();

                    }
                    else
                    {
                        if (comboBox1.SelectedItem == "IN Interface Utillization Percentage")
                        {
                            snmp_para = "2" + comboBox9.SelectedItem.ToString();

                        }
                        else if (comboBox1.SelectedItem == "OUT Interface Utillization Percentage")
                        {
                            snmp_para = "3" + comboBox9.SelectedItem.ToString();

                        }
                    }
                }
            }
            if (comboBox3.SelectedItem == "10 Sec")
            {
                snmp_gap = 5;
            }
            else
            {
                if (comboBox3.SelectedItem == "30 Sec")
                {
                    snmp_gap = 25;
                }
                else
                {
                    if (comboBox3.SelectedItem == "1 Min")
                    {
                        snmp_gap = 55;
                    }
                    else
                    {
                        if (comboBox3.SelectedItem == "5 Min")
                        {
                            snmp_gap = 295;
                        }
                    }
                }
            }
            snmp_threadstart = new ThreadStart(snmp_startmethod);
            thread1 = new Thread(snmp_threadstart);
            thread1.Start();

        }

        private void snmp_startmethod()
        {
            try
            {
                string cs = txt_cs.Text;
                Form4 f4 = new Form4(ref snmp_ip, ref snmp_para, ref cs,ref snmp_gap);
                //f2.interfaceforoidpass = para;
                f4.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("f4 start " + ex.Message);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBox7.Text == "" || textBox8.Text == "")
            {
                MessageBox.Show("Please Recheck :- IP Address or Community String or Object ID");
                return;
            }
            byte[] res = new byte[1024];
            int commlength=0, miblength=0, datalength=0, datastart=0, value=0,m=0,obj=0;
            res = snmp_conn.get("get", textBox6.Text, textBox7.Text, textBox8.Text);
            if (res[0] == 0xff)
            {
                Console.WriteLine("No response");
                return;
            }


            int msglen = 0, getpdulen = 0, seqlen = 0,d=0;
            if (res[1] > 127)
            {
                msglen = res[1] - 128;
            }

            commlength = Convert.ToInt16(res[6 + msglen]);
            if (res[8 + commlength + msglen] > 127)
            {
                getpdulen = res[8 + commlength + msglen] - 128;
            }
            if (res[19 + commlength + msglen + getpdulen] > 127)
            {
                seqlen = res[19 + commlength + msglen + getpdulen] - 128;
            }
            if (res[21 + commlength + msglen + getpdulen+ seqlen] > 127)
            {
                obj = res[21 + commlength + msglen + getpdulen +seqlen] - 128;
            }

            //int objid1, obj1, dstart1, dlen1, dlenbyte = 0, objidlen = 0;
            //snmp_datastart1 = 20 + snmp_commlength1 + msglen + getpdulen + seqlen;


            //commlength = Convert.ToInt16(res[6]);
            miblength = Convert.ToInt16(res[23 + commlength + msglen + getpdulen + seqlen +obj]);
            if (miblength > 127)
            {
                m = miblength - 128;
                miblength = Convert.ToInt16(res[24 + commlength + msglen + getpdulen + seqlen + obj]);
            }
            //datatype = Convert.ToInt16(snmp_response[24 + snmp_commlength + snmp_miblength]);
            datalength = Convert.ToInt16(res[25 + commlength + miblength + msglen + getpdulen + seqlen + m + obj]);
            if (datalength > 127)
            {
                d = datalength - 128;
                datalength = Convert.ToInt16(res[26 + commlength + miblength + msglen + getpdulen + seqlen + m + obj]);
            }
            //textBox9.AppendText("\r\nData length " + datalength);
            datastart = 26 + commlength + miblength + msglen + getpdulen + seqlen + m + d + obj;
            string output = Encoding.ASCII.GetString(res, datastart, datalength);
            while (datalength > 0)
            {
                value = (value << 8) + res[datastart++];
                datalength--;
            }
            //textBox9.AppendText("\r\n" + miblength);

            textBox9.AppendText("\r\n"+output + "\t\t\t\t" + value);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBox7.Text == "" || textBox8.Text == "")
            {
                MessageBox.Show("Please Recheck :- IP Address or Community String or Object ID");
                return;
            }
            byte[] res_n = new byte[1024];
            int commlength = 0, miblength = 0, datalength = 0, datastart = 0, value = 0;
            res_n = snmp_conn.get("getnext", textBox6.Text, textBox7.Text, textBox8.Text);
            if (res_n[0] == 0xff)
            {
                Console.WriteLine("No response");
                return;
            }
            /*
            commlength = Convert.ToInt16(res[6]);
            miblength = Convert.ToInt16(res[23 + commlength]);
            //datatype = Convert.ToInt16(snmp_response[24 + snmp_commlength + snmp_miblength]);
            datalength = Convert.ToInt16(res[25 + commlength + miblength]);
            datastart = 26 + commlength + miblength;
            string output = Encoding.ASCII.GetString(res, datastart, datalength);
            while (datalength > 0)
            {
                value = (value << 8) + res[datastart++];
                datalength--;
            }
             */

            int msglen = 0, getpdulen = 0, seqlen = 0, d = 0,obj=0,m=0;
            if (res_n[1] > 127)
            {
                msglen = res_n[1] - 128;
            }

            commlength = Convert.ToInt16(res_n[6 + msglen]);
            if (res_n[8 + commlength + msglen] > 127)
            {
                getpdulen = res_n[8 + commlength + msglen] - 128;
            }
            if (res_n[19 + commlength + msglen + getpdulen] > 127)
            {
                seqlen = res_n[19 + commlength + msglen + getpdulen] - 128;
            }
            if (res_n[21 + commlength + msglen + getpdulen + seqlen] > 127)
            {
                obj = res_n[21 + commlength + msglen + getpdulen + seqlen] - 128;
            }

            //int objid1, obj1, dstart1, dlen1, dlenbyte = 0, objidlen = 0;
            //snmp_datastart1 = 20 + snmp_commlength1 + msglen + getpdulen + seqlen;


            //commlength = Convert.ToInt16(res[6]);
            miblength = Convert.ToInt16(res_n[23 + commlength + msglen + getpdulen + seqlen + obj]);
            if (miblength > 127)
            {
                m = miblength - 128;
                miblength = Convert.ToInt16(res_n[24 + commlength + msglen + getpdulen + seqlen + obj]);
            }
            //datatype = Convert.ToInt16(snmp_response[24 + snmp_commlength + snmp_miblength]);
            datalength = Convert.ToInt16(res_n[25 + commlength + miblength + msglen + getpdulen + seqlen + m + obj]);
            if (datalength > 127)
            {
                d = datalength - 128;
                datalength = Convert.ToInt16(res_n[26 + commlength + miblength + msglen + getpdulen + seqlen + m + obj]);
            }
            //textBox9.AppendText("\r\nData length " + datalength);
            datastart = 26 + commlength + miblength + msglen + getpdulen + seqlen + m + d + obj;
            string output = Encoding.ASCII.GetString(res_n, datastart, datalength);
            while (datalength > 0)
            {
                value = (value << 8) + res_n[datastart++];
                datalength--;
            }


            textBox10.AppendText("\r\n" + output + "\t\t" + value);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox9.Text = "Get:\r\n====================================================================================================================";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox10.Text = "Get Next:\r\n====================================================================================================================";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox11.Text = "Multiple Nexts:\r\n====================================================================================================================";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBox7.Text == "" || textBox8.Text == "")
            {
                MessageBox.Show("Please Recheck :- IP Address or Community String or Object ID");
                return;
            }
            string[] oid=new string[5];
            oid[0] = textBox8.Text;
            byte[] resp = new byte[4096];
            int num_of_next = Convert.ToInt16(comboBox8.SelectedItem);
            if (comboBox8.SelectedItem == null)
            {
                MessageBox.Show("Please Select Number of Next Objects to Retrieve");
                return;
            }
            resp = snmp_conn.getbulk(textBox6.Text, textBox7.Text, oid, 1,num_of_next);
            
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (resp[0] == 0xff)
            {
                Console.WriteLine("No response");
                textBox11.AppendText("\r\n" + snmp_conn.console);
                textBox11.AppendText("\r\nno response");
                return;
            }
            int commlength = 0, miblength = 0, datalength = 0, datastart = 0, value = 0;
            int msglen = 0, getpdulen = 0, seqlen = 0;
            if (resp[1] > 127)
            {
                msglen = resp[1] - 128;
            }

            commlength = Convert.ToInt16(resp[6 + msglen]);
            if (resp[8 + commlength + msglen] > 127)
            {
                getpdulen = resp[8 + commlength + msglen] - 128;
            }
            if (resp[19 + commlength + msglen + getpdulen] > 127)
            {
                seqlen = resp[19 + commlength + msglen + getpdulen] - 128;
            }
            /*
            int obj1, dstart1, dlen1,obj_oid_len,obj_oid=0,d=0;
            datastart = 20 + commlength + msglen + getpdulen + seqlen;
            for (int l = 0; l < num_of_next; l++)
            {
                value = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; obj_oid = 0; d = 0;
                obj_oid_len =Convert.ToInt16(resp[datastart + 1 +obj_oid +d]);
                if (obj_oid_len > 127)
                {
                    obj_oid = obj_oid_len - 128 +obj_oid;
                    obj_oid_len = Convert.ToInt16(resp[datastart + 1 + 1 + obj_oid + d]);
                }
                obj1 = Convert.ToInt16(resp[datastart + 3 + obj_oid +d]);
                //datatype = Convert.ToInt16(resp[datastart + 3 + obj1 + 1]);
                datalength = Convert.ToInt16(resp[datastart + 3 + obj1 + 2 +obj_oid +d]);
                if (datalength > 127)
                {
                    d = datalength - 128 +d;
                    datalength = Convert.ToInt16(resp[datastart + 3 + obj1 + 2 + obj_oid + 1 +d]);
                }
                datastart = datastart + 3 + obj1 + 3 + obj_oid +d;
                dstart1 = datastart;
                //textBox2.AppendText("\r\nStartbyte " + dstart1.ToString());
                dlen1 = datalength;
                //textBox2.AppendText("\r\nlength " + dlen1.ToString());


                while (datalength > 0)
                {
                    value = (value << 8) + resp[datastart++];
                    datalength--;
                }
             * */

            int objid1, obj1, dstart1, dlen1, dlenbyte = 0, objidlen = 0;
            datastart = 20 + commlength + msglen + getpdulen + seqlen;
            for (int l = 0; l < num_of_next; l++)
            {
                value = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; objid1 = 0; dlenbyte = 0; objidlen = 0;
                objid1 = Convert.ToInt16(resp[datastart + 1]);
                if (objid1 > 127)
                {
                    objidlen = objid1 - 128;
                    objid1 = Convert.ToInt16(resp[datastart + 1 + 1]);
                }
                obj1 = Convert.ToInt16(resp[datastart + 3 + objidlen]);
                //datatype = Convert.ToInt16(resp[datastart + 3 + obj1 + 1 + objidlen]);
                datalength = Convert.ToInt16(resp[datastart + 3 + obj1 + 2 + objidlen]);
                if (datalength > 127)
                {
                    dlenbyte = datalength - 128;
                    datalength = Convert.ToInt16(resp[datastart + 3 + obj1 + 2 + objidlen + 1]);
                }
                datastart = datastart + 3 + obj1 + 3 + dlenbyte + objidlen;
                dstart1 = datastart;
                //textBox2.AppendText("\r\nStartbyte " + dstart1.ToString());
                dlen1 = datalength;
                //textBox2.AppendText("\r\nlength " + dlen1.ToString());
                while (datalength > 0)
                {
                    value = (value << 8) + resp[datastart++];
                    datalength--;
                }

                string output = Encoding.ASCII.GetString(resp, dstart1, dlen1);
                textBox11.AppendText("\r\n" + output +"\t"+ value);
                textBox11.AppendText("\r\n" + "----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                //textBox2.AppendText("\r\n" + snmp_value1 +l);
            }
        }

        private void btn_chk_start_Click(object sender, EventArgs e)
        {
            a = 0; n = 0; x = 0;
            txt_alive.Text = "";
            txt_nalive.Text = "";
            btn_chk_stop.Enabled = true;
            btn_chk_resume.Enabled = false;
            btn_chk_start.Enabled = false;
            ts = new ThreadStart(thread_availability);
            th = new Thread(ts);
            datadel =new datadelegate(data);
            n_datadel = new n_datadelegate(n_data);
            th.Start();  
             
           // Form3 f3 = new Form3(ref num, ref lines);
            //f3.ShowDialog();
        }

        public void data()
        {
            txt_alive.AppendText(alive[a] + "\r\n");          
        }

        public void n_data()
        {
            txt_nalive.AppendText(nalive[n] + "\r\n");
        }

        private void thread_availability()
        {

            DialogResult r;
            r = MessageBox.Show("This may take few minutes.Please wait", "Wait", MessageBoxButtons.OKCancel);
            if (r == DialogResult.OK)
            {
                foreach (string line in lines)
                {
                    try
                    {
                        pr1 = p1.Send(line);
                        if ((pr1.Status == IPStatus.DestinationNetworkUnreachable) || (pr1.Status == IPStatus.HardwareError) || (pr1.Status == IPStatus.DestinationUnreachable))
                        {
                            MessageBox.Show("Please check whether you are connected to Network");
                        }
                        else
                        {
                            if (pr1.Status == IPStatus.Success)
                            {
                                x = pr1.RoundtripTime;
                                alive[a] = string.Copy(line + "\t" + x);
                                //txt_alive.AppendText(alive[a] + "\r\n");
                                txt_alive.Invoke(datadel);
                                a++;
                            }
                            if (pr1.Status == IPStatus.TimedOut)
                            {
                                nalive[n] = string.Copy(line + "\t" + "Timed Out");
                                //txt_nalive.AppendText(nalive[n] + "\r\n");
                                txt_nalive.Invoke(n_datadel);
                                n++;
                            }
                            if (pr1.Status.ToString() == Convert.ToString(1231))
                            {
                                nalive[n] = string.Copy(line + "\t" + "Transmit Failed");
                                //txt_nalive.AppendText(nalive[n] + "\r\n");
                                txt_nalive.Invoke(n_datadel);
                                n++;
                            }
                        }

                    }
                    catch (PingException ex)
                    {
                        nalive[n] = string.Copy(line + " Ping Request couldnot find the hostname");
                        //txt_nalive.AppendText(nalive[n] + "\r\n");
                        txt_nalive.Invoke(n_datadel);
                        n++;
                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        nalive[n] = string.Copy(line + " Local Network Problem");
                        //txt_nalive.AppendText(nalive[n] + "\r\n");
                        txt_nalive.Invoke(n_datadel);
                        n++;
                    }
                }
                th.Suspend();
            }
        }

        private void btn_chk_stop_Click(object sender, EventArgs e)
        {
            if (th.IsAlive == true)
            {
                btn_chk_resume.Enabled = true;
                btn_chk_start.Enabled = true;
                btn_chk_stop.Enabled = false;
                th.Suspend();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            txt_fcontent.Text = "";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            txt_alive.Text = "";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            txt_nalive.Text = "";
        }

        private void btn_chk_resume_Click(object sender, EventArgs e)
        {
            //btn_chk_start.Enabled = true;
            
            if (th.IsAlive == true)
            {
                btn_chk_stop.Enabled = true;
                btn_chk_resume.Enabled = false;
                btn_chk_start.Enabled = false;
                th.Resume();
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            textBox2.Text = "INTERFACES:\r\n==========================================================";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = "PING RESULT:\r\n==================================================================";
        }
        private void btn_ping_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" )
            {
                MessageBox.Show("Please Provider IP Address");
                return;
            }
            if (textBox1.Text == "" || textBox5.Text == "")
            {
                textBox1.Text = "1";
                textBox5.Text = "1000";
            }
            textBox4.AppendText("Pinging "+ textBox3.Text +": \r\n");
            pthstart=new ThreadStart(pingntimes);
            pth=new Thread(pthstart);
            pdel=new deleg(pdata);
            pth.Start();
        }
        public void pingntimes()
        {
            int i=0;
            int gap=Convert.ToInt16(textBox5.Text); 
            no_of_ping =Convert.ToInt16( textBox1.Text);
            while (i < no_of_ping)
            {
                Thread.Sleep(gap);
                try
                {
                    pr2 = p2.Send(textBox3.Text, 5000);
                    time_to_return = pr2.RoundtripTime;
                    if ((pr2.Status == IPStatus.DestinationNetworkUnreachable) || (pr2.Status == IPStatus.HardwareError) || (pr2.Status == IPStatus.DestinationUnreachable))
                    {
                        MessageBox.Show("Please check whether you are connected to Network");
                    }
                    else
                    {
                        textBox4.Invoke(pdel);
                    }
                }
                catch (PingException ex)
                {
                    time_to_return = -1;
                    textBox4.Invoke(pdel);
                    
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    
                }
                
                i++;
                
            }
        }
        public void pdata()
        {
            if (time_to_return == -1)
            {
                textBox4.AppendText("Ping request could not find host "+textBox3.Text+". Please check the name and try again " + "\r\n");
                pth.Suspend();
            }
            else
            {
                if (pr2.Status == IPStatus.Success)
                {
                    textBox4.AppendText("Reply From " + textBox3.Text + " came in " + time_to_return + " ms" + "\r\n");
                }
                if (pr2.Status == IPStatus.TimedOut)
                {
                    textBox4.AppendText("Request Timed Out" + "\r\n");
                }
                if (pr2.Status.ToString() == Convert.ToString(1231))
                {
                    textBox4.AppendText("Ping Request to " + textBox3.Text + " couldnot be Transmitted" + "\r\n");
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == "5 Min")
            {
                label24.Visible = true;
                comboBox4.Visible = true;
                comboBox4.Items.Clear();
                comboBox4.Items.AddRange(new object[] { "1 Hour", "2 Hour", "3 Hour", "4 Hour", "5 Hour", "6 Hour", "12 Hour", "18 Hour", "24 Hour", "48 Hour" });
            }
            else
            {
                label24.Visible = false;
                comboBox4.Visible = false;
            }
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
 /////////////////////////////////////////////////////////////////////////Task Scheduler///////////////////////////////////////////////////////////////////////////////////////      
 
       

        private scheduler _scheduler;
        
        private void UpdateTaskList()
        {
            listViewItems.Items.Clear();
            foreach (scheduler.TriggerItem item in _scheduler.TriggerItems)
            {
                ListViewItem listItem = listViewItems.Items.Add(item.Tag.ToString());
                listItem.Tag = item;
                DateTime nextDate = item.GetNextTriggerTime();
                if (nextDate != DateTime.MaxValue)
                    listItem.SubItems.Add(nextDate.ToString());
                else
                    listItem.SubItems.Add("Never");
            }
        }

        private void CreateSchedulerItem()
        {
            scheduler.TriggerItem triggerItem = new scheduler.TriggerItem();
            triggerItem.Tag = textBoxlabelOneTimeOnlyTag.Text;
            triggerItem.StartDate = dateTimePickerStartDate.Value;
            triggerItem.EndDate = dateTimePickerEndDate.Value;
            triggerItem.TriggerTime = dateTimePickerTriggerTime.Value;
            triggerItem.OnTrigger += new scheduler.TriggerItem.OnTriggerEventHandler(triggerItem_OnTrigger); // And the trigger-Event :)

            // Set OneTimeOnly - Active and Date
            triggerItem.TriggerSettings.OneTimeOnly.Active = checkBoxOneTimeOnlyActive.Checked;
            triggerItem.TriggerSettings.OneTimeOnly.Date = dateTimePickerOneTimeOnlyDay.Value.Date;

            // Set the interval for daily trigger
            triggerItem.TriggerSettings.Daily.Interval = (ushort)numericUpDownDaily.Value;

            // Set the active days for weekly trigger
            for (byte day = 0; day < 7; day++) // Set the active Days
                triggerItem.TriggerSettings.Weekly.DaysOfWeek[day] = checkedListBoxWeeklyDays.GetItemChecked(day);

            // Set the active months for monthly trigger
            for (byte month = 0; month < 12; month++)
                triggerItem.TriggerSettings.Monthly.Month[month] = checkedListBoxMonthlyMonths.GetItemChecked(month);

            // Set active Days (0..30 = Days, 31=last Day) for monthly trigger
            for (byte day = 0; day < 32; day++)
                triggerItem.TriggerSettings.Monthly.DaysOfMonth[day] = checkedListBoxMonthlyDays.GetItemChecked(day);

            // Set the active weekNumber and DayOfWeek for monthly trigger
            // f.e. the first monday, or the last friday...
            for (byte weekNumber = 0; weekNumber < 5; weekNumber++) // 0..4: first, second, third, fourth or last week
                triggerItem.TriggerSettings.Monthly.WeekDay.WeekNumber[weekNumber] = checkedListBoxMonthlyWeekNumber.GetItemChecked(weekNumber);
            for (byte day = 0; day < 7; day++)
                triggerItem.TriggerSettings.Monthly.WeekDay.DayOfWeek[day] = checkedListBoxMonthlyWeekDay.GetItemChecked(day);

            triggerItem.Enabled = true; // Set the Item-Active - State
            _scheduler.AddTrigger(triggerItem); // Add the trigger to List
            _scheduler.Enabled = checkBoxEnabled.Checked; // Start the Scheduler
            UpdateTaskList();
        }

        private void ShowAllTriggerDates()
        {
            if (listViewItems.SelectedItems.Count > 0)
            {
                scheduler.TriggerItem item = (scheduler.TriggerItem)listViewItems.SelectedItems[0].Tag;
                Form form = new Form();
                ListView listView = new ListView();
                listView.FullRowSelect = true;

                form.Text = "Full list for Task: "+item.Tag.ToString();
                form.Width = 400;
                form.Height = 450;

                listView.Parent = form;
                listView.Dock = DockStyle.Fill;
                listView.View = View.Details;
                listView.Columns.Add("Date", 200);

                DateTime date = dateTimePickerStartDate.Value.Date;
                while (date <= dateTimePickerEndDate.Value.Date)
                {
                    if (item.CheckDate(date)) // probe this date
                        listView.Items.Add(date.ToLongDateString());
                    date = date.AddDays(1);
                }
                form.Show();
            }
            else
                MessageBox.Show("Please select a trigger!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToXML()
        {
            if (listViewItems.SelectedItems.Count > 0)
            {
                scheduler.TriggerItem item = (scheduler.TriggerItem)listViewItems.SelectedItems[0].Tag;
                textBoxEvents.Text = "Events\r\n==============================================================\r\n";
                textBoxEvents.AppendText(item.ToXML()); // Save the configuration to XML
            }
            else
                MessageBox.Show("Please select a trigger!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        int gapsec = 1;
        void triggerItem_OnTrigger(object sender, scheduler.OnTriggerEventArgs e)
        {
            textBoxEvents.AppendText(e.TriggerDate.ToString() + ": " + e.Item.Tag + ", next trigger: "+e.Item.GetNextTriggerTime().DayOfWeek.ToString()+", " + e.Item.GetNextTriggerTime().ToString()+"\r\n");
            UpdateTaskList();

            
            if (comboBox6.SelectedItem == "1 Sec")
            {
                gapsec = 1;
            }
            else
            {
                if (comboBox6.SelectedItem == "5 Sec")
                {
                    gapsec = 5;
                }
                else
                {
                    if (comboBox6.SelectedItem == "30 Sec")
                    {
                        gapsec = 30;
                    }
                    else
                    {
                        if (comboBox6.SelectedItem == "5 Min")
                        {
                            gapsec = 300;
                        }
                    }
                }
            }
            if (comboBox5.SelectedItem == "15 Min")
            {
                no_of_ping = (15 * 60) / gapsec;
            }
            else
            {
                if (comboBox5.SelectedItem == "30 Min")
                {
                    no_of_ping = (30 * 60) / gapsec;
                }
                else
                {
                    if (comboBox5.SelectedItem == "1 Hour")
                    {
                        no_of_ping = (60 * 60) / gapsec;
                    }
                    else
                    {
                        if (comboBox5.SelectedItem == "2 Hour")
                        {
                            no_of_ping = (120 * 60) / gapsec;
                        }
                        else
                        {
                            if (comboBox5.SelectedItem == "3 Hour")
                            {
                                no_of_ping = (3 * 3600) / gapsec;
                            }
                            else
                            {
                                if (comboBox5.SelectedItem == "4 Hour")
                                {
                                    no_of_ping = (4 * 3600) / gapsec;
                                }
                                else
                                {
                                    if (comboBox5.SelectedItem == "5 Hour")
                                    {
                                        no_of_ping = (5 * 3600) / gapsec;
                                    }
                                    else
                                    {
                                        if (comboBox5.SelectedItem == "6 Hour")
                                        {
                                            no_of_ping = (6 * 3600) / gapsec;
                                        }
                                        else
                                        {
                                            if (comboBox5.SelectedItem == "7 Hour")
                                            {
                                                no_of_ping = (7 * 3600) / gapsec;
                                            }
                                            else
                                            {
                                                if (comboBox5.SelectedItem == "8 Hour")
                                                {
                                                    no_of_ping = (8 * 3600) / gapsec;
                                                }
                                                else
                                                {
                                                    if (comboBox5.SelectedItem == "9 Hour")
                                                    {
                                                        no_of_ping = (9 * 3600) / gapsec;
                                                    }
                                                    else
                                                    {
                                                        if (comboBox5.SelectedItem == "10 Hour")
                                                        {
                                                            no_of_ping = (10 * 3600) / gapsec;
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Please Recheck Time-Span");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            textBoxEvents.AppendText("Creating Log For " + textBoxlabelOneTimeOnlyTag.Text + "\r\n");
            plogstr = new ThreadStart(ping_log);
            plog = new Thread(plogstr);
            plog.Start();
        }

        public void ping_log()
        {
            int i = 0;
            
            while (i < no_of_ping)
            {
                Thread.Sleep(gapsec*1000);
                DateTime d=DateTime.Now;
                try
                {
                    pr2 = p2.Send(textBoxlabelOneTimeOnlyTag.Text, 5000);
                    
                    if ((pr2.Status == IPStatus.DestinationNetworkUnreachable) || (pr2.Status == IPStatus.HardwareError) || (pr2.Status == IPStatus.DestinationUnreachable))
                    {
                        MessageBox.Show("Please check whether you are connected to Network");
                    }
                    else
                    {
                        if (pr2.Status == IPStatus.Success)
                        {
                            time_to_return = pr2.RoundtripTime;
                            log_content[log_counter] = d + "\t" + "Success" + "\t" + time_to_return + " " + "ms";
                        }
                        if (pr2.Status == IPStatus.TimedOut)
                        {
                            log_content[log_counter] = d + "\t" + "Request Timed Out" ;
                        }
                        if (pr2.Status.ToString() == Convert.ToString(1231))
                        {
                            log_content[log_counter] = d + "\t" + "Couldnot be Transmitted";
                        }
                        
                    }
                }
                catch (PingException ex)
                {
                    log_content[log_counter] = d + "\t" + "Ping request could not find host "+textBoxlabelOneTimeOnlyTag.Text+". Please check the name and try again ";

                }
                catch (System.Net.Sockets.SocketException ex)
                {

                }
                log_counter++;
                i++;
                if (log_counter == 30)
                {
                    int count = 0;
                    if (log_counter == 30)
                    {
                        count = 30;
                    }
                    log_counter = 0;
                    saveinfile(log_content,count);
                }
            }
            plog.Suspend();
        }
        private void saveinfile(string[] log_content,int log_count)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(appPath_log + "\\" + textBoxlabelOneTimeOnlyTag.Text + "_log.txt", true);
            for (int w = 0; w <log_count; w++)
            {
                file.WriteLine(log_content[w]);
            }
            log_counter = 0;
            file.Close();
            //saveinfilethread.Abort();
        }

        private void buttonCreateTrigger_Click(object sender, EventArgs e)
        {
            CreateSchedulerItem();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _scheduler.Enabled = false;
            _scheduler.TriggerItems.Clear();
            UpdateTaskList();
            textBoxEvents.Text = "Events\r\n==============================================================\r\n";
        }

        private void buttonShowAllTrigger_Click(object sender, EventArgs e)
        {
            ShowAllTriggerDates();
        }

        private void buttonToXML_Click(object sender, EventArgs e)
        {
            ExportToXML(); // Use the static method scheduler.TriggerItem.FromXML to load a TriggerItem
        }

        private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _scheduler.Enabled = checkBoxEnabled.Checked;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox13.Text = openFileDialog1.FileName;
            }
        }
        delegate void de();
        de d;
        private void button15_Click(object sender, EventArgs e)
        {
            /*
            int gapsec = 0;
            if (comboBox6.SelectedItem == "1 Sec")
            {
                gapsec = 1;
            }
            else
            {
                if (comboBox6.SelectedItem == "5 Sec")
                {
                    gapsec = 5;
                }
                else
                {
                    if (comboBox6.SelectedItem == "30 Sec")
                    {
                        gapsec = 30;
                    }
                    else
                    {
                        if (comboBox6.SelectedItem == "5 Min")
                        {
                            gapsec = 300;
                        }
                    }
                }
            }
            System.Windows.Forms.DataVisualization.Charting.Series Series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            
            System.IO.StreamReader file = new System.IO.StreamReader(textBox13.Text, true);
            string line;int row=0;
            
            while ((line = file.ReadLine())!=null)
            {
                int c = 0, timestartloc = 0, timeendloc = 0,res_start=0,res_end=0,val_s=0,val_e=0,val=0;
                string resp;
                DateTime x = new DateTime();
                while (line[c] != ' ')
                {
                    c++;
                }
                c++;
                while (line[c] != ' ')
                {
                    c++;
                }
                c = c + 3;
                timeendloc = c;
                x=Convert.ToDateTime(line.Substring(0,c));
                res_start = c +1;
                int l = 0;
                resp = line.Substring(res_start, 7);
                if (resp == "Success")
                {
                    val_s = res_start+7 + 1;
                    c = val_s;
                    while (line[c] != 'm')
                    {
                        c++;
                    }
                    val_e = c - 1;
                    l=val_e-val_s;
                    val = Convert.ToInt16(line.Substring(val_s,l));
                }
                else
                {
                }
                if (row == 0)
                {
                    chart1.ChartAreas[0].AxisX.Minimum = x.ToOADate();
                    chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(x.ToOADate()).AddMinutes(300 * (gapsec) / 60).ToOADate();
                    Series1.Points.AddXY(x.ToOADate(), val);
                }
                else
                {
                    Series1.Points.AddXY(x.ToOADate(), val);
                }
                row++;
            }
            file.Close();
            chart1.Invalidate();
             */
            d = new de(ch_render);
            chart1.Invoke(d);
        }

        public void ch_render()
        {
            int gapsec = 0;
            if (comboBox7.SelectedItem == "1 Sec")
            {
                gapsec = 1;
            }
            else
            {
                if (comboBox7.SelectedItem == "5 Sec")
                {
                    gapsec = 5;
                }
                else
                {
                    if (comboBox7.SelectedItem == "30 Sec")
                    {
                        gapsec = 30;
                    }
                    else
                    {
                        if (comboBox7.SelectedItem == "5 Min")
                        {
                            gapsec = 300;
                        }
                    }
                }
            }
            System.Windows.Forms.DataVisualization.Charting.Series Series1 = new System.Windows.Forms.DataVisualization.Charting.Series();

            System.IO.StreamReader file = new System.IO.StreamReader(textBox13.Text, true);
            string line; int row = 0;
            DateTime x = new DateTime();
            while ((line = file.ReadLine()) != null)
            {
                int c = 0, timestartloc = 0, timeendloc = 0, res_start = 0, res_end = 0, val_s = 0, val_e = 0, val = 0;
                string resp;
                
                while (line[c] != ' ')
                {
                    c++;
                }
                c++;
                while (line[c] != ' ')
                {
                    c++;
                }
                c = c + 3;
                timeendloc = c;
                x = Convert.ToDateTime(line.Substring(0, c));
                res_start = c + 1;
                int l = 0;
                resp = line.Substring(res_start, 7);
                if (resp == "Success")
                {
                    val_s = res_start + 7 + 1;
                    c = val_s;
                    while (line[c] != 'm')
                    {
                        c++;
                    }
                    val_e = c - 1;
                    l = val_e - val_s;
                    val = Convert.ToInt16(line.Substring(val_s, l));
                }
                else
                {
                }
                if (row == 0)
                {
                    chart1.ChartAreas[0].AxisX.Minimum = x.ToOADate();
                    chart1.Series[0].Points.AddXY(x.ToOADate(), val);
                    //chart1.Invalidate();
                }
                else
                {
                    chart1.Series[0].Points.AddXY(x.ToOADate(), val);
                    //chart1.Invalidate();
                }
                row++;
            }
            file.Close();
            
            chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(x.ToOADate()).AddSeconds(gapsec * row).ToOADate();
            chart1.Update();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            label42.Text = "";
            label31.Text = "";
            label33.Text = "";
            label35.Text = "";
            string[] oids = new string[5];
            
            int msglen = 0, getpdulen = 0, seqlen = 0;
            int objid1, obj1, dstart1, dlen1, dlenbyte = 0, objidlen = 0,  interface_num = Convert.ToInt16(comboBox10.SelectedItem), interface_num_len = 0;
            
            oids[0] = "1.3.6.1.2.1.2.2.1.2." + interface_num;
            oids[1] = "1.3.6.1.2.1.2.2.1.10." + interface_num;
            oids[2] = "1.3.6.1.2.1.2.2.1.16." + interface_num;
            oids[3] = "1.3.6.1.2.1.2.2.1.5." + interface_num;
            
                snmp_response1 = snmp_conn.get("get",txt_snmp_ip.Text, txt_cs.Text, oids, 4);

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (snmp_response1[0] == 0xff)
                {
                    Console.WriteLine("No response");
                    textBox2.AppendText("\r\n" + snmp_conn.console);
                    textBox2.AppendText("\r\nno response");
                    return;
                }
                msglen = 0; getpdulen = 0; seqlen = 0;
                if (snmp_response1[1] > 127)
                {
                    msglen = snmp_response1[1] - 128;
                }

                snmp_commlength1 = Convert.ToInt16(snmp_response1[6 + msglen]);
                if (snmp_response1[8 + snmp_commlength1 + msglen] > 127)
                {
                    getpdulen = snmp_response1[8 + snmp_commlength1 + msglen] - 128;
                }
                if (snmp_response1[19 + snmp_commlength1 + msglen + getpdulen] > 127)
                {
                    seqlen = snmp_response1[19 + snmp_commlength1 + msglen + getpdulen] - 128;
                }


                objid1 = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; dlenbyte = 0; objidlen = 0;
                interface_num_len = 0;
                snmp_datastart1 = 20 + snmp_commlength1 + msglen + getpdulen + seqlen;
                for (int l = 0; l < 4; l++)
                {
                    snmp_value1 = 0; obj1 = 0; dstart1 = 0; dlen1 = 0; objid1 = 0; dlenbyte = 0; objidlen = 0; interface_num_len = 0;
                    objid1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 1]);
                    if (objid1 > 127)
                    {
                        objidlen = objid1 - 128;
                        objid1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 1 + 1]);
                    }
                    obj1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen]);
                    interface_num = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1]);
                    if (interface_num > 127)
                    {
                        interface_num_len = interface_num - 128;
                        interface_num = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1 + 1]);
                        if (interface_num_len >= 2)
                        {
                            if (interface_num_len > 2)
                            {
                                interface_num = interface_num * 256;
                                interface_num = interface_num + Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + objidlen + obj1 + 1 + 1]);
                            }
                            else
                            {
                                interface_num = interface_num * 256;
                            }
                        }
                    }
                    snmp_datatype1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 1 + objidlen + interface_num_len]);
                    snmp_datalength1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 2 + objidlen + interface_num_len]);
                    if (snmp_datalength1 > 127)
                    {
                        dlenbyte = snmp_datalength1 - 128;
                        snmp_datalength1 = Convert.ToInt16(snmp_response1[snmp_datastart1 + 3 + obj1 + 2 + objidlen + interface_num_len + 1]);
                    }
                    snmp_datastart1 = snmp_datastart1 + 3 + obj1 + 3 + dlenbyte + objidlen + interface_num_len;
                    dstart1 = snmp_datastart1;
                    //textBox2.AppendText("\r\nStartbyte " + dstart1.ToString());
                    dlen1 = snmp_datalength1;
                    //textBox2.AppendText("\r\nlength " + dlen1.ToString());
                    while (snmp_datalength1 > 0)
                    {
                        snmp_value1 = (snmp_value1 << 8) + snmp_response1[snmp_datastart1++];
                        snmp_datalength1--;
                    }

                    snmp_output1 = Encoding.ASCII.GetString(snmp_response1, dstart1, dlen1);
                    //textBox2.AppendText("\r\n" + interface_num + " " + snmp_output1);
                    if (l == 0)
                    {
                        label42.Text = snmp_output1;
                    }
                    else
                    {
                        if (l == 1)
                        {
                            label31.Text = snmp_value1.ToString();
                        }
                        else
                        {
                            if (l == 2)
                            {
                                label33.Text = snmp_value1.ToString();
                            }
                            else
                            {
                                if (l == 3)
                                {
                                    label35.Text = snmp_value1.ToString();
                                }
                            }
                        }
                    }
                }

        }

       

        

        

    }
}
