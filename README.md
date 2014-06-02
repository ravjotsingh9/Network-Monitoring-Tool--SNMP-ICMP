Network-Monitoring-Tool--SNMP-ICMP(May'2012- June'2012)
=======================================================

Description:	
------------
Network Monitoring Tool is a windows based software (prototype) which helps in monitoing devices using SNMPv2(without security). It uses ICMP message to check the availibility of a device on the network.
This project includes six tabs:
  1. SNMP Command:By entering IP address and community string of a managed devices (one with SNMP agent installed), Get, GetNextMib, GetBulk requests can be sent for a given Oid.
  2. SNMP Plotter:This tab helps in plotting a real-time graph for different parameters of a particular interface on a given device.
  3. Single Address Ping: It provides two options for checking availibility of a device on the network. Either a recursive ping command can be run OR a real-time graph can be plotted which will also save the data in form of logs.
  4. Multiple Address Ping: Given a text file consisting of multiple domain-names/IP addresses (one in each line), it check if the devices are alive on the network, by pinging them one by one.
  5. Log Scheduler: It uses a class (authored by Lothar Perr on codeproject.com) to create a trigger to autostart loging on a given time and date for given period of time.
  6. Log Plotter: To analyze the Logs generated by scheduled triggers, this tab plots them in the form of graph. 


Prerequisite:
-------------
To run this solution, a reference to DataVisualisation dll is required.


IDE Used:	
--------
Developed on: Visual Studio 2010
Later Updated for: Visual studio 2013


Language Used:
--------------
C#


Conclusions/Remarks:
-------------------
Self-Implemented: SNMP Get, GetNextMib and GetBulk Requests.


Errors/Bugs:
------------
Since the project is a prototype, it does not handle all the possible error cases.


References:
-----------
  1. http://www.java2s.com/Code/CSharp/Network/SimpleSNMP.htm
  2. http://www.codeproject.com/Articles/38553/TaskScheduler
