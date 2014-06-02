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
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;


namespace SnmpApp
{
    public partial class Form4 : Form
    {
        /*
        class snmpclass
        {
            public string console;

            public snmpclass()
            {

            }

            public byte[] get(string request, string host, string community, string mibstring)
            {
                byte[] packet = new byte[1024];
                byte[] mib = new byte[1024];
                int snmplen;
                int comlen = community.Length;
                string[] mibvals = mibstring.Split('.');
                int miblen = mibvals.Length;
                int cnt = 0, temp, i;
                int orgmiblen = miblen;
                int pos = 0;

                // Convert the string MIB into a byte array of integer values
                // Unfortunately, values over 128 require multiple bytes
                // which also increases the MIB length
                for (i = 0; i < orgmiblen; i++)
                {
                    temp = Convert.ToInt16(mibvals[i]);
                    if (temp > 127)
                    {
                        mib[cnt] = Convert.ToByte(128 + (temp / 128));
                        mib[cnt + 1] = Convert.ToByte(temp - ((temp / 128) * 128));
                        cnt += 2;
                        miblen++;
                    }
                    else
                    {
                        mib[cnt] = Convert.ToByte(temp);
                        cnt++;
                    }
                }
                snmplen = 29 + comlen + miblen - 1;  //Length of entire SNMP packet

                //The SNMP sequence start
                packet[pos++] = 0x30; //Sequence start
                packet[pos++] = Convert.ToByte(snmplen - 2);  //sequence size

                //SNMP version
                packet[pos++] = 0x02; //Integer type
                packet[pos++] = 0x01; //length
                packet[pos++] = 0x00; //SNMP version 1

                //Community name
                packet[pos++] = 0x04; // String type
                packet[pos++] = Convert.ToByte(comlen); //length
                //Convert community name to byte array
                byte[] data = Encoding.ASCII.GetBytes(community);
                for (i = 0; i < data.Length; i++)
                {
                    packet[pos++] = data[i];
                }

                //Add GetRequest or GetNextRequest value
                if (request == "get")
                    packet[pos++] = 0xA0;
                else
                    packet[pos++] = 0xA1;

                packet[pos++] = Convert.ToByte(20 + miblen - 1); //Size of total MIB

                //Request ID
                packet[pos++] = 0x02; //Integer type
                packet[pos++] = 0x04; //length
                packet[pos++] = 0x00; //SNMP request ID
                packet[pos++] = 0x00;
                packet[pos++] = 0x00;
                packet[pos++] = 0x01;

                //Error status
                packet[pos++] = 0x02; //Integer type
                packet[pos++] = 0x01; //length
                packet[pos++] = 0x00; //SNMP error status

                //Error index
                packet[pos++] = 0x02; //Integer type
                packet[pos++] = 0x01; //length
                packet[pos++] = 0x00; //SNMP error index

                //Start of variable bindings
                packet[pos++] = 0x30; //Start of variable bindings sequence

                packet[pos++] = Convert.ToByte(6 + miblen - 1); // Size of variable binding

                packet[pos++] = 0x30; //Start of first variable bindings sequence
                packet[pos++] = Convert.ToByte(6 + miblen - 1 - 2); // size
                packet[pos++] = 0x06; //Object type
                packet[pos++] = Convert.ToByte(miblen - 1); //length

                //Start of MIB
                packet[pos++] = 0x2b;
                //Place MIB array in packet

                for (i = 2; i < miblen; i++)
                    packet[pos++] = Convert.ToByte(mib[i]);
                packet[pos++] = 0x05; //Null object value
                packet[pos++] = 0x00; //Null

                //Send packet to destination
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
                IPHostEntry ihe = Dns.Resolve(host);
                IPEndPoint iep = new IPEndPoint(ihe.AddressList[0], 161);
                EndPoint ep = (EndPoint)iep;
                sock.SendTo(packet, snmplen, SocketFlags.None, iep);
                //Receive response from packet
                try
                {
                    int recv, j = 0;
                    do
                    {
                        recv = sock.ReceiveFrom(packet, ref ep);
                        j++;
                    } while (sock.Available > 0);
                    Console.Write(recv);
                    Console.Write("\n" + j);
                }
                catch (Exception ex)
                {
                    packet[0] = 0xff;
                    Console.Write(ex);
                    console = ex.ToString();
                }
                return packet;

            }

            public string getnextMIB(byte[] mibin)
            {
                string output = "1.3";
                int commlength = mibin[6];
                int mibstart = 6 + commlength + 17; //find the start of the mib section
                //The MIB length is the length defined in the SNMP packet
                // minus 1 to remove the ending .0, which is not used
                int miblength = mibin[mibstart] - 1;
                mibstart += 2; //skip over the length and 0x2b values
                int mibvalue;

                for (int i = mibstart; i < mibstart + miblength; i++)
                {
                    mibvalue = Convert.ToInt16(mibin[i]);
                    if (mibvalue > 128)
                    {
                        mibvalue = (mibvalue / 128) * 128 + Convert.ToInt16(mibin[i + 1]);
                        //ERROR here, it should be mibvalue = (mibvalue-128)*128 + Convert.ToInt16(mibin[i+1]);
                        //for mib values greater than 128, the math is not adding up correctly   

                        i++;
                    }
                    output += "." + mibvalue;
                }
                return output;
            }
        }
*/
        Int64 counter = 0;        
        string appPath = Path.GetFullPath(Application.StartupPath);
        int flag = 1;
        public string ipadd;
        public string[] oid=new string[5];
        ThreadStart addDataThreadStart;
        Thread addDataRunner;
        delegate void AddDataDelegate();
        AddDataDelegate addDataDel;
        snmp conn = new snmp();
        snmp conn3 = new snmp();
        snmp conn4 = new snmp();
        byte[] response = new byte[1024];
        byte[] response3 = new byte[1024];
        byte[] response4 = new byte[1024];
        double x,xpre=0,xnow=0;
        double x1,x2, xpre1 = 0, xnow1 = 0, speed1;
        DateTime tpre, tnow;
        DateTime tpre2, tnow2;
        double xpre2=0, ypre2=0, xnow2=0, ynow2=0;
        
        string[] tmp = new string[10];
        int commlength, miblength, datatype, datalength, datastart,snmp_value = 0;
      
        string iface,cs;
        
        Thread addDataRunner1;
        ThreadStart addDataThreadStart1;
        AddDataDelegate addDataDel1;

        Thread addDataRunner2;
        ThreadStart addDataThreadStart2;
        AddDataDelegate addDataDel2;

        
        string interfaceforoid;
        int threadno = -1;
        int snmp_gap = 0;

        double t = 0, dt = 0;
        public Form4(ref string ip,ref string interfaceforoidpass,ref string cstring,ref int snmp_gap1)
        {
            InitializeComponent();
            ipadd = ip;
            cs = cstring;
            snmp_gap = snmp_gap1;
            interfaceforoid = interfaceforoidpass;
            if (interfaceforoid.Substring(0, 1) == "2")
            {
                iface = interfaceforoid.Substring(1);
                try
                {
                    btn_resume.Enabled = false;
                    response = conn.get("get", ipadd, cs, "1.3.6.1.2.1.2.2.1.5." + iface);
                    speed1 = packetoutputval(response);
                    if (speed1 == 0)
                    {
                        response = conn.get("get", ipadd, cs, "1.3.6.1.2.1.2.2.1.5." + iface);
                        speed1 = packetoutputval(response);
                        if (speed1 == 0)
                        {
                            MessageBox.Show("Please try again.Sorry for inconvenience");
                            this.Close();
                        }
                    }
                    speed1 = speed1 / 8;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("1 " + ex.Message);
                }
                threadno = 1;
                interfaceforoid = "1.3.6.1.2.1.2.2.1.10." + iface;
                addDataThreadStart1 = new ThreadStart(AddDataThreadLoop1);
                addDataRunner1 = new Thread(addDataThreadStart1);
                addDataDel1 += new AddDataDelegate(AddData1);
                addDataRunner1.Start();
            }
            else
            {
                if (interfaceforoid.Substring(0, 1) == "3")
                {
                    iface = interfaceforoid.Substring(1);
                    try
                    {
                        //Icon = New_ping.Properties.Resources.ajax_loader__3_;
                        btn_resume.Enabled = false;
                        response = conn.get("get", ipadd, cs, "1.3.6.1.2.1.2.2.1.5." + iface);
                        speed1 = packetoutputval(response);
                        if (speed1 == 0)
                        {
                            response = conn.get("get", ipadd, cs, "1.3.6.1.2.1.2.2.1.5." + iface);
                            speed1 = packetoutputval(response);
                            if (speed1 == 0)
                            {
                                MessageBox.Show("Please try again.Sorry for inconvenience");
                                this.Close();
                            }
                        }
                        speed1 = speed1 / 8;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("1 " + ex.Message);
                    }
                    interfaceforoid = "1.3.6.1.2.1.2.2.1.16." + iface;
                    threadno = 1;
                    addDataThreadStart1 = new ThreadStart(AddDataThreadLoop1);
                    addDataRunner1 = new Thread(addDataThreadStart1);
                    addDataDel1 += new AddDataDelegate(AddData1);
                    addDataRunner1.Start();
                }
                else
                {
                    if (interfaceforoid.Substring(0, 1) == "4")
                    {
                        iface = interfaceforoid.Substring(1);
                        try
                        {
                            //Icon = New_ping.Properties.Resources.ajax_loader__3_;
                            btn_resume.Enabled = false;
                            response = conn.get("get", ipadd, cs, "1.3.6.1.2.1.2.2.1.5." + iface);
                            speed1 = packetoutputval(response);
                            if (speed1 == 0)
                            {
                                response = conn.get("get", ipadd, cs, "1.3.6.1.2.1.2.2.1.5." + iface);
                                speed1 = packetoutputval(response);
                                if (speed1 == 0)
                                {
                                    MessageBox.Show("Please try again.Sorry for inconvenience");
                                    this.Close();
                                }
                            }
                            speed1 = speed1 / 8;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("1 " + ex.Message);
                        }
                        threadno = 2;
                        addDataThreadStart2 = new ThreadStart(AddDataThreadLoop2);
                        addDataRunner2 = new Thread(addDataThreadStart2);
                        addDataDel2 += new AddDataDelegate(AddData2);
                        addDataRunner2.Start();
                    }
                    else
                    {
                        try
                        {
                            btn_resume.Enabled = false;
                            threadno = 0;
                            addDataThreadStart = new ThreadStart(AddDataThreadLoop);
                            addDataRunner = new Thread(addDataThreadStart);
                            addDataDel += new AddDataDelegate(AddData);
                            addDataRunner.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("1 " + ex.Message);
                        }
                    }
                }
            }
        }
        private void AddDataThreadLoop()
        {
            try
            {
                if (interfaceforoid == "")
                {
                    MessageBox.Show("Please try this OID again");
                    this.Close();
                }
                response = conn.get("get", ipadd, cs, interfaceforoid);
                xnow = packetoutputval(response);
                while (flag == 1)
                {
                    chart1.Invoke(addDataDel);
                    response = conn.get("get", ipadd, cs, interfaceforoid);
                    Thread.Sleep(snmp_gap *1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("2 " + ex.Message);
            }
        }
        private void AddDataThreadLoop1()
        {
            try
            {
                if (interfaceforoid == "")
                {
                    MessageBox.Show("Please try this OID again");
                    this.Close();
                }
                tnow = DateTime.Now;
                response = conn.get("get", ipadd, cs, interfaceforoid);
                xnow1 = packetoutputval(response);
                while (flag == 1)
                {
                    chart1.Invoke(addDataDel1);
                    response = conn.get("get", ipadd, cs, interfaceforoid);
                    Thread.Sleep(snmp_gap * 1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("22 " + ex.Message);
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
            snmp_value = 0;
            xpre = xnow;
            xnow = packetoutputval(response);
            if (ptSeries.Points.Count >= 0)
            {
                x = xnow-xpre;
                newVal = x;
            }
            s = x.ToString();
            if (newVal < 0)
                newVal = 0;
            ptSeries.Points.AddXY(timeStamp.ToOADate(), x);
            txt_log.Text = timeStamp.ToString() + " " + x.ToString() + "\r\n" + txt_log.Text;
            tmp[counter] = interfaceforoid + "\t" +  timeStamp.ToString()+"\t"+x.ToString();
            double removeBefore = timeStamp.AddSeconds((double)(900) * (-1)).ToOADate();
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }
            chart1.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(15).ToOADate();
            chart1.Invalidate();
        }
        private void btn_exit_Click(object sender, EventArgs e)
        {
            flag = 0;
            if (threadno==1)
            {
                if (addDataRunner1.IsAlive == true)
                {
                    addDataRunner1.Suspend();
                }
                this.Close();    
            }
            else
            {
                if (threadno == 2)
                {
                    if (addDataRunner2.IsAlive == true)
                    {
                        addDataRunner2.Suspend();
                    }
                    this.Close();
                }
                else
                {
                    if (addDataRunner.IsAlive == true)
                    {
                        addDataRunner.Suspend();
                    }
                    this.Close();
                    /*
                                        try
                                        {
                                            Process[] proc = Process.GetProcessesByName("Snmp_utility");
                                            proc[0].Kill();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("1 "+ex.Message.ToString());
                                        }
                     */
                }
            }
         }
        private void btn_suspend_Click(object sender, EventArgs e)
        {
            btn_suspend.Enabled = false;
            btn_resume.Enabled = true;
            if (threadno==1)
            {
                if (addDataRunner1.IsAlive == true)
                {
                    addDataRunner1.Suspend();
                }

            }
            else
            {
                if (threadno==2)
                {
                    if (addDataRunner2.IsAlive == true)
                    {
                        addDataRunner2.Suspend();
                    }

                }
                else
                {
                    if (addDataRunner.IsAlive == true)
                    {
                        addDataRunner.Suspend();
                    }

                }
            }       
        }
        private void btn_resume_Click(object sender, EventArgs e)
        {
            btn_resume.Enabled = false;
            btn_suspend.Enabled = true;
            if (threadno == 1)
            {
                if (addDataRunner1.IsAlive == true)
                {
                    addDataRunner1.Resume();
                }

            }
            else
            {
                if (threadno == 2)
                {
                    if (addDataRunner2.IsAlive == true)
                    {
                        addDataRunner2.Resume();
                    }

                }
                else
                {
                    if (addDataRunner.IsAlive == true)
                    {
                        addDataRunner.Resume();
                    }

                }
            }
          
        }
        private void Form4_Load(object sender, EventArgs e)
        {
            lbl_ip.Text = interfaceforoid;
        }
        public void AddData1()
        {
            if (xnow1 != -1)
            {
                tpre = tnow;
            }
            DateTime timeStamp = DateTime.Now;
            tnow = timeStamp;
            foreach (Series ptSeries in chart1.Series)
            {
                t = Convert.ToDouble(((tnow.AddMilliseconds(tpre.Millisecond*(-1))).Millisecond)/1000)+ Convert.ToDouble((tnow.AddSeconds(tpre.Second*(-1))).Second) + Convert.ToDouble(((tnow.AddMinutes((tpre.AddMinutes(1)).Minute *(-1))).Minute)*60);
                //t = Convert.ToDouble((tnow.AddTicks(tpre.Ticks * (-1))).Second);
                AddNewPoint1(timeStamp, ptSeries,t);    
            }
        }
        public void AddNewPoint1(DateTime timeStamp, System.Windows.Forms.DataVisualization.Charting.Series ptSeries,double t)
        {
            double newVal = 0;
            xpre1 = xnow1;
            xnow1 = packetoutputval(response);
            if (ptSeries.Points.Count >= 0)
            {
                if (xpre1 != -1 && xnow1!=-1)
                {
                    x1 = ((xnow1 - xpre1) /t) * 100 / speed1;
                }
                else
                {
                    x1 = 0;
                }
                newVal = x1;
            }
            if (newVal < 0)
                newVal = 0;
            if (xpre1 != -1 && xnow1 != -1)
            {
                ptSeries.Points.AddXY(timeStamp.ToOADate(), x1);
            }
            if (xnow1 == -1)
            {
                txt_log.Text = timeStamp.ToString() + " " + "No Response" + "\r\n" + txt_log.Text;
                tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + "No Response";
            }
            else
            {
                if (xpre1 == -1)
                {
                    txt_log.Text = timeStamp.ToString() + " " + "Previous Value Not Available" + "\r\n" + txt_log.Text;
                    tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + "Previous Value Not Available";
                }
                else
                {
                    txt_log.Text = timeStamp.ToString() + "  " + x1.ToString() +"\r\n" + txt_log.Text;
                    tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + x1.ToString();
                }
            }
            double removeBefore = timeStamp.AddSeconds((double)(900) * (-1)).ToOADate();
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }
            chart1.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(15).ToOADate();
            chart1.Invalidate();
        }
        private void AddDataThreadLoop2()
        {
            try
            {
                if (interfaceforoid == "")
                {
                    MessageBox.Show("Please try this OID again");
                    this.Close();
                }
                oid[0] = "1.3.6.1.2.1.2.2.1.10."+iface;
                oid[1] = "1.3.6.1.2.1.2.2.1.16."+iface;
                response = conn.get("get", ipadd, cs, oid, 2);
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                packetoutputvals(response);
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                while (flag == 1)
                {
                    chart1.Invoke(addDataDel2);
                    response = conn.get("get", ipadd, cs, oid, 2);
                    Thread.Sleep((snmp_gap-5) * 1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("2 " + ex.Message);
            }
        }
        /*
        private void AddDataThreadLoop3()
        {
            snmp_value3 = 0;
            interfaceforoid3 = "1.3.6.1.2.1.2.2.1.10." + iface;
            response3 = conn3.get("get", ipadd, cs, interfaceforoid3);
            tpre3 = tnow3;
            tnow3 = DateTime.Now;
            if (response3[0] == 0xff)
            {
                Console.WriteLine("No response");
                x3 = 0;
            }
            else
            {
                commlength3 = Convert.ToInt16(response3[6]);
                miblength3 = Convert.ToInt16(response3[23 + commlength3]);
                datatype3 = Convert.ToInt16(response3[24 + commlength3 + miblength3]);
                datalength3 = Convert.ToInt16(response3[25 + commlength3 + miblength3]);
                datastart3 = 26 + commlength3 + miblength3;
                while (datalength3 > 0)
                {
                    snmp_value3 = (snmp_value3 << 8) + response3[datastart3++];
                    datalength3--;
                }

                xpre3 = xnow3;
                xnow3 = snmp_value3;
                x3 = xnow3 - xpre3;
                t3 = Convert.ToDouble((tnow3.AddSeconds(tpre3.Second * (-1))).Second);
                x3 = x3 / t3;
                addDataRunner3.Abort();
            }
        }
        private void AddDataThreadLoop4()
        {
            snmp_value4 = 0;
            
            interfaceforoid4 = "1.3.6.1.2.1.2.2.1.16." + iface;
            response4 = conn4.get("get", ipadd, cs, interfaceforoid4);
            tpre4 = tnow4;
            tnow4 = DateTime.Now;
            if (response4[0] == 0xff)
            {
                Console.WriteLine("No response");
                x4 = 0;
            }
            else
            {
                commlength4 = Convert.ToInt16(response4[6]);
                miblength4 = Convert.ToInt16(response4[23 + commlength4]);
                datatype4 = Convert.ToInt16(response4[24 + commlength4 + miblength4]);
                datalength4 = Convert.ToInt16(response4[25 + commlength4 + miblength4]);
                datastart4 = 26 + commlength4 + miblength4;
                while (datalength4 > 0)
                {
                    snmp_value4 = (snmp_value4 << 8) + response4[datastart4++];
                    datalength4--;
                }
                xpre4 = xnow4;
                xnow4 = snmp_value4;
                x4 = xnow4 - xpre4;
                t4 = Convert.ToDouble((tnow4.AddSeconds(tpre4.Second * (-1))).Second);
                x4 = x4 / t4;
                addDataRunner4.Abort();
            }
        }
         */ 
        public void AddData2()
        {
            if (xnow2 != -1 && ynow2 != -1)
            {
                tpre2 = tnow2;
                xpre2 = xnow2;
                ypre2 = ynow2;
            }
            DateTime timeStamp = DateTime.Now;
            tnow2 = timeStamp;
            foreach (Series ptSeries in chart1.Series)
            {
                //dt = Convert.ToDouble((tnow2.AddSeconds(tpre2.Second * (-1))).Second);
                dt = Convert.ToDouble(((tnow2.AddMilliseconds(tpre2.Millisecond * (-1))).Millisecond) / 1000) + Convert.ToDouble((tnow2.AddSeconds(tpre2.Second * (-1))).Second) + Convert.ToDouble(((tnow2.AddMinutes(tpre2.Minute * (-1))).Minute) * 60);
                AddNewPoint2(timeStamp, ptSeries,dt);
            }
        }
        public void AddNewPoint2(DateTime timeStamp, System.Windows.Forms.DataVisualization.Charting.Series ptSeries,double dt)
        {
            double newVal = 0;
            string s;
            double dx = 0, dy = 0;
            
            //xnow = packetoutputval(response);
            ///////////////////////////////////////////////////////////////////////////////////
            packetoutputvals(response);
            ///////////////////////////////////////////////////////////////////////////////////
            dx = xnow2 - xpre2;
            dy = ynow2 - ypre2;
            if (ptSeries.Points.Count >= 0)
            {
                if (xpre2 != -1 && xnow2 != -1 && ypre2 != -1 && ynow2 != -1)
                {
                    x2 = ((dx + dy) / dt) * (100/speed1);
                }
                else
                {
                    x2 = 0;
                }
                newVal = x2;
            }
            s = x2.ToString();
            if (newVal < 0)
                newVal = 0;
            
            //txt_log.Text = timeStamp.ToString() + " " + x2.ToString() + "\r\n" + txt_log.Text;
            //tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + x2.ToString();


            if (xpre2 != -1 && xnow2 != -1 && ypre2 != -1 && ynow2 != -1)
            {
                ptSeries.Points.AddXY(timeStamp.ToOADate(), x2);
            }
            if (xnow1 == -1 || ynow2==-1)
            {
                txt_log.Text = timeStamp.ToString() + " " + "No Response" + "\r\n" + txt_log.Text;
                tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + "No Response";
            }
            else
            {
                if (xpre1 == -1 ||ypre2==-1)
                {
                    txt_log.Text = timeStamp.ToString() + " " + "Previous Value Not Available" + "\r\n" + txt_log.Text;
                    tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + "Previous Value Not Available";
                }
                else
                {
                    txt_log.Text = timeStamp.ToString() + "  " + x2.ToString()  + "\r\n" + txt_log.Text;
                    tmp[counter] = interfaceforoid + "\t" + timeStamp.ToString() + "\t" + x2.ToString();
                }
            }








            double removeBefore = timeStamp.AddSeconds((double)(900) * (-1)).ToOADate();
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }

            chart1.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(15).ToOADate();
            chart1.Invalidate();
        }
        public double packetoutputval(byte[] resp)
        {
            snmp_value = 0;
            if (resp[0] == 0xff)
            {
                Console.WriteLine("No response");
                //--------------------------------txt_log.AppendText("\r\n" + conn.console);
                //-------------------------------txt_log.AppendText("\r\nno response");
                if (threadno == -1)
                {
                    return 0;
                }
                else
                {
                    if (threadno == 0)
                    {
                        return xnow;
                    }
                    else
                    {
                        if (threadno == 1)
                        {
                            return -1;
                        }
                        else
                        {
                            if (threadno == 2)
                            {
                                return -1;
                            }
                        }
                    }
                }
            }

            // Get the community and MIB lengths of the response
            commlength = Convert.ToInt16(resp[6]);
            miblength = Convert.ToInt16(resp[23 + commlength]);

            // Extract the MIB data from the SNMp response
            datatype = Convert.ToInt16(resp[24 + commlength + miblength]);
            datalength = Convert.ToInt16(resp[25 + commlength + miblength]);
            //textBox4.Text = datalength.ToString();
            datastart = 26 + commlength + miblength;
            int dstart = datastart;
            //txt_log.AppendText("\r\nStartbyte " + dstart.ToString());
            int dlen = datalength;
            //txt_log.AppendText("\r\nlength " + dlen.ToString());

            byte[] barray = new byte[datalength];
            // The syssnmp_value value may by a multi-byte integer
            // Each byte read must be shifted to the higher byte order
            while (datalength > 0)
            {
                snmp_value = (snmp_value << 8) + resp[datastart++];
                datalength--;
            }
            return snmp_value;
        }

        public void packetoutputvals(byte[] response)
        {
            if (response[0] == 0xff)
            {
                Console.WriteLine("No response");
                xnow2 = -1;
                ynow2 = -1;
                //textBox11.AppendText("\r\n" + conn.console);
                //textBox11.AppendText("\r\nno response");
                //return 0;
            }
            int commlength = 0, miblength = 0, datalength = 0, datastart = 0, value = 0;
            int msglen = 0, getpdulen = 0, seqlen = 0;
            if (response[1] > 127)
            {
                msglen = response[1] - 128;
            }

            commlength = Convert.ToInt16(response[6 + msglen]);
            if (response[8 + commlength + msglen] > 127)
            {
                getpdulen = response[8 + commlength + msglen] - 128;
            }
            if (response[19 + commlength + msglen + getpdulen] > 127)
            {
                seqlen = response[19 + commlength + msglen + getpdulen] - 128;
            }

            int obj1, dstart1, dlen1;
            datastart = 20 + commlength + msglen + getpdulen + seqlen;
            for (int l = 0; l < 2; l++)
            {
                value = 0; obj1 = 0; dstart1 = 0; dlen1 = 0;
                obj1 = response[datastart + 3];
                //datatype = Convert.ToInt16(resp[datastart + 3 + obj1 + 1]);
                datalength = Convert.ToInt16(response[datastart + 3 + obj1 + 2]);
                datastart = datastart + 3 + obj1 + 3;
                dstart1 = datastart;
                //textBox2.AppendText("\r\nStartbyte " + dstart1.ToString());
                dlen1 = datalength;
                //textBox2.AppendText("\r\nlength " + dlen1.ToString());


                while (datalength > 0)
                {
                    value = (value << 8) + response[datastart++];
                    datalength--;
                }
                string output = Encoding.ASCII.GetString(response, dstart1, dlen1);
                //textBox11.AppendText("\r\n" + l + " " + output);
                //textBox2.AppendText("\r\n" + snmp_value1 +l);
                if (l == 0)
                {
                    xnow2 = value;
                }
                else
                {
                    ynow2 = value;
                }
            }
        }

        
    }
}
