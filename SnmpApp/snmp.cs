using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace SnmpApp
{
    class snmp
    {
        public string console;

        public snmp()
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
          
        public byte[] getbulk(string host, string community, string[] mibstring, int no_of_oid, int max_repeaters)//all double bytes
        {
            byte[] packet = new byte[8192];
            byte[] mib = new byte[1024];
            int snmplen;
            int comlen = community.Length;
            int[] miblen = new int[no_of_oid];
            string[][] mibvals = new string[no_of_oid][];
            int[] orgmiblen = new int[no_of_oid];
            for (int l = 0; l < no_of_oid; l++)
            {
                mibvals[l] = mibstring[l].Split('.');
                miblen[l] = mibvals[l].Length;
                orgmiblen[l] = miblen[l];
            }
            int cnt = 0, temp, i;
            int pos = 0;

            //Snmp packet length
            int totalmiblen = 0;
            int msglen = 0, getpdulen = 0, seqlen = 0;
            for (int l = 0; l < no_of_oid; l++)
            {
                totalmiblen = miblen[l] + totalmiblen;
            }

            //Bytes for seq length
            temp = totalmiblen - no_of_oid + 6 * no_of_oid;
            if ((temp) > 127)
            {
                if (temp > 256)
                {
                    if (temp % 256 == 0)
                    {
                        seqlen = 1;
                    }
                    else
                    {
                        seqlen = 2;
                    }
                }
                else
                {
                    seqlen = 1;
                }
            }

            //Byte for req length
            temp = 14 + totalmiblen - no_of_oid + 6 * no_of_oid + seqlen;
            if ((temp) > 127)
            {
                if (temp > 256)
                {
                    if (temp % 256 == 0)
                    {
                        getpdulen = 1;
                    }
                    else
                    {
                        getpdulen = 2;
                    }
                }
                else
                {
                    getpdulen = 1;
                }
            }
            // Byte for msg length
            temp = 6 + 14 + totalmiblen - no_of_oid + 6 * no_of_oid + seqlen + getpdulen;
            if ((temp) > 127)
            {
                if (temp > 256)
                {
                    if (temp % 256 == 0)
                    {
                        msglen = 1;
                    }
                    else
                    {
                        msglen = 2;
                    }
                }
                else
                {
                    msglen = 1;
                }
            }
            snmplen = 23 + comlen + totalmiblen - no_of_oid + 6 * no_of_oid + msglen + getpdulen + seqlen;
            //snmplen = 6 + 14 + totalmiblen - no_of_oid + 6 * no_of_oid + seqlen + getpdulen +msglen +2;

            //The SNMP sequence start
            packet[pos++] = 0x30; //Sequence start

            //Sequence Size

            temp = 0;
            temp = 7 + 14 + totalmiblen + comlen - no_of_oid + 6 * no_of_oid + seqlen + getpdulen;
            if (temp > 127)
            {
                if (temp > 256)
                {
                    int no_of_full_bytes = temp / 256;
                    packet[pos++] = Convert.ToByte(128 + 2);
                    packet[pos++] = Convert.ToByte(no_of_full_bytes);
                    packet[pos++] = Convert.ToByte(temp - (no_of_full_bytes * 256));
                }
                else
                {
                    packet[pos++] = Convert.ToByte(128 + (temp / 128));
                    packet[pos++] = Convert.ToByte(temp - ((temp / 128) * 128));
                }
            }
            else
            {
                packet[pos++] = Convert.ToByte(temp);
            }

            //packet[pos++] = Convert.ToByte(snmplen - 2);  //sequence size

            //SNMP version
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x01; //length
            packet[pos++] = 0x01; //SNMP version 2
            //Community name
            packet[pos++] = 0x04; // String type
            packet[pos++] = Convert.ToByte(comlen); //length
            //Convert community name to byte array
            byte[] data = Encoding.ASCII.GetBytes(community);
            for (i = 0; i < data.Length; i++)
            {
                packet[pos++] = data[i];
            }

            //Add Getbulk
            packet[pos++] = 0xA5;

            //Size of total Mib

            temp = 0;
            temp = 14 + totalmiblen - no_of_oid + 6 * no_of_oid;
            if (temp > 127)
            {
                if (temp > 256)
                {
                    int no_of_full_bytes = temp / 256;
                    packet[pos++] = Convert.ToByte(128 + 2);
                    packet[pos++] = Convert.ToByte(no_of_full_bytes);
                    packet[pos++] = Convert.ToByte(temp - (no_of_full_bytes * 256));
                }
                else
                {
                    packet[pos++] = Convert.ToByte(128 + (temp / 128));
                    packet[pos++] = Convert.ToByte(temp - ((temp / 128) * 128));
                }
            }
            else
            {
                packet[pos++] = Convert.ToByte(temp);
            }


            //packet[pos++] = Convert.ToByte(14 + totalmiblen - no_of_oid + 6 * no_of_oid); //Size of total MIB

            //Request ID
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x04; //length
            packet[pos++] = 0x00; //SNMP request ID
            packet[pos++] = 0x00;
            packet[pos++] = 0x00;
            packet[pos++] = 0x01;

            //Non Repeaters //Error status
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x01; //length
            packet[pos++] = 0x00; //SNMP error status

            //Max Repeaters //Error index
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x01; //length
            packet[pos++] = Convert.ToByte(max_repeaters);//0x02; //SNMP error index


            //Start of variable bindings
            packet[pos++] = 0x30; //Start of variable bindings sequence

            //Size of variable binding

            temp = 0;
            temp = totalmiblen - no_of_oid + 6 * no_of_oid;
            if (temp > 127)
            {
                if (temp > 256)
                {
                    int no_of_full_bytes = temp / 256;
                    packet[pos++] = Convert.ToByte(128 + 2);
                    packet[pos++] = Convert.ToByte(no_of_full_bytes);
                    packet[pos++] = Convert.ToByte(temp - (no_of_full_bytes * 256));
                }
                else
                {
                    packet[pos++] = Convert.ToByte(128 + (temp / 128));
                    packet[pos++] = Convert.ToByte(temp - ((temp / 128) * 128));
                }
            }
            else
            {
                packet[pos++] = Convert.ToByte(temp);
            }

            //packet[pos++] = Convert.ToByte(totalmiblen - no_of_oid + 6 * no_of_oid); // Size of variable binding

            //Add Variable Object Values
            for (int l = 0; l < no_of_oid; l++)
            {
                cnt = 0; temp = 0;
                for (i = 0; i < orgmiblen[l]; i++)
                {
                    temp = Convert.ToInt16(mibvals[l][i]);
                    if (temp > 127)
                    {
                        mib[cnt] = Convert.ToByte(128 + (temp / 128));
                        mib[cnt + 1] = Convert.ToByte(temp - ((temp / 128) * 128));
                        cnt += 2;
                        miblen[l]++;
                    }
                    else
                    {
                        mib[cnt] = Convert.ToByte(temp);
                        cnt++;
                    }
                }
                packet[pos++] = 0x30; //Start of first variable bindings sequence
                packet[pos++] = Convert.ToByte(6 + miblen[l] - 1 - 2); // size
                packet[pos++] = 0x06; //Object type
                packet[pos++] = Convert.ToByte(miblen[l] - 1); //length

                //Start of MIB
                packet[pos++] = 0x2b;
                //Place MIB array in packet
                for (i = 2; i < miblen[l]; i++)
                    packet[pos++] = Convert.ToByte(mib[i]);
                packet[pos++] = 0x05; //Null object value
                packet[pos++] = 0x00; //Null
            }



            //Send packet to destination
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                            ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket,
                            SocketOptionName.ReceiveTimeout, 5000);
            IPHostEntry ihe = Dns.Resolve(host);
            IPEndPoint iep = new IPEndPoint(ihe.AddressList[0], 161);
            EndPoint ep = (EndPoint)iep;
            sock.SendTo(packet, snmplen, SocketFlags.None, iep);

            //Receive response from packet
            try
            {
                int recv = sock.ReceiveFrom(packet, ref ep);
            }
            catch (SocketException ex)
            {
                packet[0] = 0xff;
                console = ex.ToString();
            }
            return packet;
        }
        public byte[] get(string request, string host, string community, string[] mibstring, int no_of_oid)
        {
            byte[] packet = new byte[1024];
            byte[] mib = new byte[1024];
            int snmplen;
            int comlen = community.Length;
            int[] miblen = new int[no_of_oid];
            string[][] mibvals = new string[no_of_oid][];
            int[] orgmiblen = new int[no_of_oid];
            for (int l = 0; l < no_of_oid; l++)
            {
                mibvals[l] = mibstring[l].Split('.');
                miblen[l] = mibvals[l].Length;
                orgmiblen[l] = miblen[l];
            }
            int cnt = 0, temp, i;
            int pos = 0;

            //Snmp packet length
            int totalmiblen = 0;
            for (int l = 0; l < no_of_oid; l++)
            {
                totalmiblen = miblen[l] + totalmiblen;
            }
            snmplen = 23 + comlen + totalmiblen - no_of_oid + 6 * no_of_oid;


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

            packet[pos++] = Convert.ToByte(14 + totalmiblen - no_of_oid + 6 * no_of_oid); //Size of total MIB
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
            packet[pos++] = Convert.ToByte(totalmiblen - no_of_oid + 6 * no_of_oid); // Size of variable binding

            //Add Variable Object Values
            for (int l = 0; l < no_of_oid; l++)
            {
                cnt = 0; temp = 0;
                for (i = 0; i < orgmiblen[l]; i++)
                {
                    temp = Convert.ToInt16(mibvals[l][i]);
                    if (temp > 127)
                    {
                        mib[cnt] = Convert.ToByte(128 + (temp / 128));
                        mib[cnt + 1] = Convert.ToByte(temp - ((temp / 128) * 128));
                        cnt += 2;
                        miblen[l]++;
                    }
                    else
                    {
                        mib[cnt] = Convert.ToByte(temp);
                        cnt++;
                    }
                }
                packet[pos++] = 0x30; //Start of first variable bindings sequence
                packet[pos++] = Convert.ToByte(6 + miblen[l] - 1 - 2); // size
                packet[pos++] = 0x06; //Object type
                packet[pos++] = Convert.ToByte(miblen[l] - 1); //length

                //Start of MIB
                packet[pos++] = 0x2b;
                //Place MIB array in packet
                for (i = 2; i < miblen[l]; i++)
                    packet[pos++] = Convert.ToByte(mib[i]);
                packet[pos++] = 0x05; //Null object value
                packet[pos++] = 0x00; //Null
            }



            //Send packet to destination
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                            ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket,
                            SocketOptionName.ReceiveTimeout, 5000);
            IPHostEntry ihe = Dns.Resolve(host);
            IPEndPoint iep = new IPEndPoint(ihe.AddressList[0], 161);
            EndPoint ep = (EndPoint)iep;
            sock.SendTo(packet, snmplen, SocketFlags.None, iep);

            //Receive response from packet
            try
            {
                int recv = sock.ReceiveFrom(packet, ref ep);
            }
            catch (SocketException ex)
            {
                packet[0] = 0xff;
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
}
