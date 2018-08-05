using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;

namespace Tracker
{
    public partial class Tracker : Form
    {
        public Tracker()
        {
            Form.CheckForIllegalCrossThreadCalls = false; // For debugging purpose.
            InitializeComponent();
        }

        class file_list : IComparable
        {
            public string fName;
            public long fSize;
            public List<IPEndPoint> fEndpoints;
            public file_list() { }
            public file_list(string file_name, long file_size, List<IPEndPoint> file_endpoints)
            {
                fName = file_name;
                fSize = file_size;
                fEndpoints = file_endpoints;
            }
            public void insertEndPoint(IPEndPoint ipEnd)
            {
                fEndpoints.Add(ipEnd);
            }
            public static bool operator >(file_list fl1, file_list fl2)
            {
                if (fl1.fName.CompareTo(fl2.fName) > 0)
                    return true;
                else
                    return false;
            }
            public static bool operator <(file_list fl1, file_list fl2)
            {
                if (fl1.fName.CompareTo(fl2.fName) < 0)
                    return true;
                else
                    return false;
            }
            public static bool operator >=(file_list fl1, file_list fl2)
            {
                if (fl1.fName.CompareTo(fl2.fName) >= 0)
                    return true;
                else
                    return false;
            }
            public static bool operator <=(file_list fl1, file_list fl2)
            {
                if (fl1.fName.CompareTo(fl2.fName) <= 0)
                    return true;
                else
                    return false;
            }
            public static bool operator ==(file_list fl1, file_list fl2)
            {
                if (fl1.fName == fl2.fName)
                    return true;
                else
                    return false;
            }
            public static bool operator !=(file_list fl1, file_list fl2)
            {
                if (fl1.fName != fl2.fName)
                    return true;
                else
                    return false;
            }
            public override bool Equals(object o)
            {
                if (this.fName == o.ToString())
                    return true;
                else
                    return false;
            }
            public override int GetHashCode()
            {
                return this.fName.GetHashCode();
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                file_list compareIt = (file_list)obj;
                int ret;
                if (this.fName.CompareTo(compareIt.fName) > 1)
                    ret = 1;
                else if (this.fName == compareIt.fName)
                    ret = 0;
                else
                    ret = -1;
                return ret;
            }

            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        List<file_list> available_files;
        IPAddress local_ip;
        int local_port;
        Mutex text;
        ThreadStart listeners;
        Thread listener;
        Socket listen;
        List<Socket> inc_connections;
        List<ThreadStart> do_ops;
        List<Thread> do_op;
        Mutex file_acc;
        bool run_program;


        private void startTracking_Click(object sender, EventArgs e)
        {
            try
            {
                local_port = System.Convert.ToInt32(textBox1.Text);
                if (local_port >= 0 && local_port <= 65535)
                {
                    run_program = true;
                    file_acc = new Mutex();
                    textBox1.Enabled = false;
                    startTracking.Enabled = false;
                    available_files = new List<file_list>();
                    text = new Mutex();
                    inc_connections = new List<Socket>();
                    listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listen.Bind(new IPEndPoint(IPAddress.Any, local_port));
                    listen.Listen(50);
                    local_ip = ((IPEndPoint)listen.LocalEndPoint).Address;
                    text.WaitOne();
                    outbox.AppendText("Tracker (IP: " + local_ip.ToString() + " is listening on port " + local_port + "\n");
                    text.ReleaseMutex();
                    do_ops = new List<ThreadStart>();
                    do_op = new List<Thread>();
                    listeners = new ThreadStart(delegate { thr_lis_acc(); });
                    listener = new Thread(listeners);
                    listener.Start();
                }
                else
                {
                    FormatException wrong_format = new FormatException("The tracker port number needs to be between 0 and 65535.");
                    throw wrong_format;
                }
            }
            catch (FormatException formatex)
            {
                text.WaitOne();
                outbox.AppendText(formatex + "\n" + "Please write a valid (integer) port number between 0 and 65535.\n");
                text.ReleaseMutex();
            }
        }

        private void thr_lis_acc()
        {
            try
            {
                while (run_program)
                {
                    inc_connections.Add(listen.Accept());     //switch connection to different port
                    int tmp = inc_connections.Count() - 1;
                    text.WaitOne();
                    outbox.AppendText("IP: " + ((IPEndPoint)inc_connections[tmp].RemoteEndPoint).Address.ToString() + " connected to Tracker on port " + local_port + "\n");
                    text.ReleaseMutex();
                    byte[] code = new byte[1];
                    inc_connections[tmp].Receive(code);
                    int coded = 0;
                    if (code[0] == 100)
                        coded = 100;
                    else if (code[0] == 200)
                        coded = 200;
                    do_ops.Add(delegate { thr_ops(tmp, coded); });
                    do_op.Add(new Thread(do_ops[tmp]));
                    do_op[tmp].Start();
                }
            }
            catch (SocketException con_lost)       //connection is lost or cannot be made (to make the Close button function.
            {
                text.WaitOne();
                if (run_program)
                    outbox.AppendText(con_lost + "\n" + "There has been a problem in the connection please press \"Close\" and try again.\n");
                text.ReleaseMutex();
            }
        }

        private void thr_ops(int index, int op)
        {
            if (op == 100)
            {
                byte[] many = new byte[4];
                inc_connections[index].Receive(many);
                int file_nums = BitConverter.ToInt32(many, 0);
                IPEndPoint remote = (IPEndPoint)inc_connections[index].RemoteEndPoint;
                byte[] portN = new byte[4];
                inc_connections[index].Receive(portN);
                int portnum = BitConverter.ToInt32(portN, 0);
                remote.Port = portnum;
                for (int i = 0; i < file_nums; i++)
                {
                    byte[] file_name_size = new byte[1];    //a file name can be max 256 chars on NTFS, hence 1 byte is enough to state the number of bytes
                    inc_connections[index].Receive(file_name_size);
                    int name_size = file_name_size[0];
                    byte[] file_name = new byte[name_size];
                    inc_connections[index].Receive(file_name);
                    string file_name_string = Encoding.Default.GetString(file_name);
                    byte[] file_size = new byte[8];
                    inc_connections[index].Receive(file_size);
                    long file_size_long = BitConverter.ToInt64(file_size, 0);
                    file_list new_file = new file_list();
                    new_file.fName = file_name_string;
                    file_acc.WaitOne();
                    int ind = binarySFL(available_files, new_file.fName);
                    if (ind >= 0)
                    {
                        available_files[ind].fEndpoints.Add(remote);
                        string new3rdrow = "";
                        int j;
                        for (j = 0; j < available_files[ind].fEndpoints.Count() - 1; j++)
                            new3rdrow += available_files[ind].fEndpoints[j].Address.ToString() + ":" + available_files[ind].fEndpoints[j].Port + ", ";
                        new3rdrow += available_files[ind].fEndpoints[j].Address.ToString() + ":" + available_files[ind].fEndpoints[j].Port;
                        fileListView.Rows[ind].SetValues(file_name_string, file_size_long.ToString(), new3rdrow);
                    }
                    else
                    {
                        new_file.fSize = file_size_long;
                        new_file.fEndpoints = new List<IPEndPoint>();
                        new_file.fEndpoints.Add(remote);
                        available_files.Add(new_file);
                        quicksort(ref available_files);
                        ind = binarySFL(available_files, new_file.fName);
                        fileListView.Rows.Add(file_name_string, file_size_long.ToString(), available_files[ind].fEndpoints[0].Address.ToString() + ":" + available_files[ind].fEndpoints[0].Port);
                        fileListView.Sort(Column1, ListSortDirection.Ascending);
                    }
                    file_acc.ReleaseMutex();

                }
                text.WaitOne();
                outbox.AppendText("List of shared files of IP: " + remote.Address.ToString() + " is recieved\n");
                text.ReleaseMutex();
                text.WaitOne();
                outbox.AppendText("Listening port of IP: " + remote.Address.ToString() + " is " + remote.Port.ToString() + "\n");
                text.ReleaseMutex();
                inc_connections[index].Close();
                text.WaitOne();
                outbox.AppendText("Connection to IP: " + remote.Address.ToString() + " is closed\n");
                text.ReleaseMutex();
            }
            else if (op == 200)
            {
                byte[] search_index = new byte[4];
                inc_connections[index].Receive(search_index);
                byte[] file_name_size = new byte[1];    //a file name can be max 256 chars on NTFS, hence 1 byte is enough to state the number of bytes
                inc_connections[index].Receive(file_name_size);
                int name_size = file_name_size[0];
                byte[] file_name_b = new byte[name_size];
                inc_connections[index].Receive(file_name_b);
                string file_name = Encoding.Default.GetString(file_name_b);
                text.WaitOne();
                outbox.AppendText("Download request for " + file_name + " is recieved from IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + "\n");
                text.ReleaseMutex();
                file_acc.WaitOne();
                int found = binarySFL(available_files, file_name);
                byte[] answer;
                int count = 0;
                if (found >= 0)
                {
                    file_list tmp = available_files[found];
                    file_acc.ReleaseMutex();
                    List<ThreadStart> reqs;
                    List<Thread> req;
                    count = tmp.fEndpoints.Count();
                    answer = BitConverter.GetBytes(count);
                    inc_connections[index].Send(answer);
                    byte[] sizeb = BitConverter.GetBytes(tmp.fSize);
                    inc_connections[index].Send(sizeb);
                    byte[] portToSend = new byte[4];
                    inc_connections[index].Receive(portToSend);
                    int rem_port = BitConverter.ToInt32(portToSend, 0);
                    text.WaitOne();
                    outbox.AppendText("Positive ack is sent to IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + " for " + file_name + "(" + tmp.fEndpoints.Count.ToString() + " FileSharerDownloader(s))" + "\n");
                    text.ReleaseMutex();
                    IPEndPoint who = ((IPEndPoint)inc_connections[index].RemoteEndPoint);
                    who.Port = rem_port;
                    text.WaitOne();
                    outbox.AppendText(count.ToString() + " FileSharerDownloader(s) have " + tmp.fName + " of size: " + tmp.fSize.ToString() + " bytes - IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + "\n");
                    text.ReleaseMutex();
                    inc_connections[index].Disconnect(false);
                    text.WaitOne();
                    outbox.AppendText("Connection to IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + " is closed\n");
                    text.ReleaseMutex();
                    inc_connections[index].Close();
                    reqs = new List<ThreadStart>();
                    req = new List<Thread>();
                    for (int i = 0; i < count; i++)
                    {
                        int a = i;
                        reqs.Add(delegate { thr_send_reqs(tmp, a, search_index, who); });
                        req.Add(new Thread(reqs[i]));
                        req[i].Start();
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    file_acc.ReleaseMutex();
                    answer = BitConverter.GetBytes(count);
                    inc_connections[index].Send(answer);
                    text.WaitOne();
                    outbox.AppendText("Negative ack is sent to IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + " for " + file_name + "(0 FileSharerDownloader)" + "\n");
                    text.ReleaseMutex();
                    inc_connections[index].Disconnect(false);
                    text.WaitOne();
                    outbox.AppendText("Connection to IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + " is closed\n");
                    text.ReleaseMutex();
                    inc_connections[index].Close();
                }
            }
        }

        private void thr_send_reqs(file_list file, int server, byte[] search_index, IPEndPoint who)
        {
            Socket serq = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serq.Connect(file.fEndpoints[server]);
            text.WaitOne();
            outbox.AppendText("Tracker conncted to IP: " + file.fEndpoints[server].Address.ToString() + " on port " + file.fEndpoints[server].Port.ToString() + "\n");
            text.ReleaseMutex();
            byte[] pfcode = new byte[1];
            pfcode[0] = 123;
            serq.Send(pfcode);
            byte[] file_name_size = new byte[1];
            file_name_size[0] = (BitConverter.GetBytes(file.fName.Length))[0];
            serq.Send(file_name_size);
            byte[] name = Encoding.Default.GetBytes(file.fName);
            serq.Send(name);
            serq.Send(search_index);
            byte[] part_index = BitConverter.GetBytes(server);
            serq.Send(part_index);
            long mod = file.fSize % file.fEndpoints.Count();
            long sizeOfFileMod;
            if (mod != 0)
                sizeOfFileMod = file.fSize + (file.fEndpoints.Count() - (mod));
            else
                sizeOfFileMod = file.fSize;
            long startp = server * sizeOfFileMod / file.fEndpoints.Count(); // start byte of the file for the specific server.
            long finishp = ((server + 1) * sizeOfFileMod / file.fEndpoints.Count()) - 1; // finish byte of the file for the specific server.
            if (server == file.fEndpoints.Count() - 1)
            {
                if (finishp != file.fSize - 1)          //means the last part!!
                    finishp = file.fSize - 1;
            }
            byte[] startb = BitConverter.GetBytes(startp);
            byte[] finishb = BitConverter.GetBytes(finishp);
            serq.Send(startb);
            serq.Send(finishb);
            byte[] ip_length = new byte[1];
            ip_length[0] = (BitConverter.GetBytes(who.Address.ToString().Length))[0];
            byte[] get_ip = Encoding.Default.GetBytes(who.Address.ToString());
            byte[] get_port = BitConverter.GetBytes(who.Port);
            serq.Send(ip_length);
            serq.Send(get_ip);
            serq.Send(get_port);
            text.WaitOne();
            outbox.AppendText("IP: " + who.Address.ToString() + ":" + who.Port.ToString() + " requests " + file.fName + " bytes: " + startp.ToString() + " - " + finishp.ToString() + " is sent to IP: " + file.fEndpoints[server].Address.ToString() + ":" + file.fEndpoints[server].Port.ToString() + "\n");
            text.ReleaseMutex();
            serq.Close();
            text.WaitOne();
            outbox.AppendText("Connection to IP: " + file.fEndpoints[server].Address.ToString() + " is closed\n");
            text.ReleaseMutex();
        }

        private int binarySFL(List<file_list> lookup, string searchN)
        {
            return binarySFL(0, lookup.Count() - 1, lookup, searchN);
        }

        private int binarySFL(int low, int high, List<file_list> lookup, string searchN)
        {
            while (low <= high)
            {
                int mid = (low + high) / 2;  // compute mid point.
                if (searchN.CompareTo(lookup[mid].fName) > 0)
                    low = mid + 1;  // repeat search in top half.
                else if (searchN.CompareTo(lookup[mid].fName) < 0)
                    high = mid - 1; // repeat search in bottom half.
                else
                    return mid;     // found it. return position /////
            }
            return -1;    // failed to find key
        }

        private void quicksort(ref List<file_list> a)
        {
            quicksort(ref a, 0, a.Count() - 1);
        }

        private void quicksort(ref List<file_list> a, int left, int right)
        {
            if (left + 10 <= right)
            {
                file_list pivot = median3(ref a, left, right);// Begin partitioning
                int i = left, j = right - 1;
                for (; ; )
                {
                    while (a[++i] < pivot) { }
                    while (pivot < a[--j]) { }
                    if (i < j)
                        swap(ref a, i, j);
                    else
                        break;
                }
                swap(ref a, i, right - 1);  // Restore pivot

                quicksort(ref a, left, i - 1);     // Sort small elements
                quicksort(ref a, i + 1, right);    // Sort large elements
            }
            else  // Do an insertion sort on the subarray
                insertionSort(ref a, left, right);
        }

        private void swap(ref List<file_list> a, int i, int j)
        {
            file_list tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
        }

        private file_list median3(ref List<file_list> a, int left, int right)
        {
            int center = (left + right) / 2;
            if (a[center] < a[left])
                swap(ref a, left, center);
            if (a[right] < a[left])
                swap(ref a, left, right);
            if (a[right] < a[center])
                swap(ref a, center, right);
            // Place pivot at position right - 1
            swap(ref a, center, right - 1);
            return a[right - 1];
        }

        private void insertionSort(ref List<file_list> a, int left, int right)
        {
            for (int p = left + 1; p <= right; p++)
            {
                file_list tmp = a[p];
                int j;

                for (j = p; j > left && tmp < a[j - 1]; j--)
                    a[j] = a[j - 1];
                a[j] = tmp;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            run_program = false;
            if(listen != null)
                listen.Close();
        }
    }
}
