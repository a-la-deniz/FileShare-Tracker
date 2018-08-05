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


namespace FileSharerDownloader
{
    public partial class FileSharerDownloader : Form
    {
        public FileSharerDownloader()
        {
            Form.CheckForIllegalCrossThreadCalls = false; // For debugging purpose.
            InitializeComponent();
        }
        FolderBrowserDialog share_path;     //interactive folder browser for the shared files
        FolderBrowserDialog down_path;      //interactive folder browser for the files to be downloaded
        string sharePath;                   //path for shared files
        string downPath;                    //path for files to be downloaded
        string[] file_list;                 //full paths of shared files
        string[] file_names;                //names of shared files
        long[] file_sizes;                  //sizes of the shared files
        int tracker_port;                   //the port number which the trakcer is listening to
        IPAddress tracker_ip;               //ip address of the tracker
        int local_port;                     //local port number to listen to
        Socket listener;                    //socket to listen, accept and switch connections to different ports and sockets
        ThreadStart listens;                //threadstart for the conneciton accepting thread function
        Thread listen;                      //thread for accepting connections
        List<Socket> inc_connections;       //the sockets for the incoming connections (they are switched to these when the connection is accepted)
        Socket Tracker;                     //socket for the tracker
        FileInfo[] finfo;                   //informations about the shared files
        IPAddress local_ip;                 //local FileSharerDownloader ip address
        List<ThreadStart> inits;
        List<Thread> init;
        List<ThreadStart> part_sends;       //threadstarts for part sending thread function
        List<Thread> part_send;             //threads for sending parts requested
        List<ThreadStart> requests;         //threadstarts for request making thread function
        List<Thread> request;               //threads for requesting file downloads
        List<string> search_list;           //a list of file names that are searched previously
        List<List<ThreadStart>> get_files;  //matrix of threadstarts for thread function that gets different parts of different files
        List<List<Thread>> get_file;        //matrix of threads for getting different parts of different files
        Mutex text;                         //mutex to protect shared resource(the output box)
        List<byte[]> inc_files;             //full byte arrays to record the incoming parts, and then write to a file
        bool run_program;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void share_Click(object sender, EventArgs e)       //choose shared folder
        {
            share_path = new FolderBrowserDialog();
            share_path.Description = "Please choose the directory to share files from:";
            share_path.ShowDialog();
            sharePath = share_path.SelectedPath;   //get path for shared files
            if (sharePath == "")        //make sure a path is selected
                outbox.AppendText("Please choose a directory for shared files and try again.\n");
            else
            {
                share.Enabled = false;
                send.Enabled = true;
                file_list = Directory.GetFiles(sharePath);      //get the list for shared files
                int file_count = file_list.Length;
                file_names = new string[file_count];
                file_sizes = new long[file_count];
                finfo = new FileInfo[file_count];
                for (int i = 0; i < file_count; i++)
                {
                    file_names[i] = file_list[i].Substring(sharePath.Length + 1, file_list[i].Length - sharePath.Length - 1);
                    finfo[i] = new FileInfo(file_list[i]);
                    file_sizes[i] = finfo[i].Length;
                }       //initialize file informations and file names without the paths
            }
        }

        private void send_Click(object sender, EventArgs e)     //send list of shared files to tracker
        {
            try
            {
                run_program = true;
                tracker_ip = IPAddress.Parse(textBox1.Text);
                textBox1.Enabled = false;
                tracker_port = System.Convert.ToInt32(textBox2.Text);
                if (tracker_port >= 0 && tracker_port <= 65535)
                {
                    textBox2.Enabled = false;
                    local_port = System.Convert.ToInt32(textBox6.Text);
                    if (local_port >= 0 && local_port <= 65535)
                    {                                               //make sure port inputs are valid
                        textBox6.Enabled = false;
                        send.Enabled = false;
                        if (dir.Enabled == false)
                            down.Enabled = true;
                        Tracker = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Tracker.Connect(tracker_ip, tracker_port);
                        local_ip = ((IPEndPoint)Tracker.LocalEndPoint).Address;
                        outbox.AppendText("FileSharerDownloader (IP: " + local_ip.ToString() + ") connected to Tracker (IP: " + tracker_ip.ToString() + ") on port " + tracker_port + "\n");
                        byte[] code = new byte[1];
                        code[0] = 100;      //inform tracker that this is the first connection to tracker to send shared file names
                        Tracker.Send(code);
                        byte[] many = new byte[4];
                        many = BitConverter.GetBytes(file_list.Length);
                        Tracker.Send(many);
                        byte[] portN = new byte[4];
                        portN = BitConverter.GetBytes(local_port);     //byte representation
                        Tracker.Send(portN);
                        for (int i = 0; i < file_list.Length; i++)
                        {
                            byte[] file_name_size = new byte[1];    //a file name can be max 256 chars on NTFS, hence 1 byte is enough to state the number of bytes
                            file_name_size[0] = (BitConverter.GetBytes(file_names[i].Length))[0];
                            Tracker.Send(file_name_size);
                            Tracker.Send(Encoding.Default.GetBytes(file_names[i]));
                            byte[] file_size = new byte[8];
                            file_size = BitConverter.GetBytes(finfo[i].Length);     //byte representation
                            Tracker.Send(file_size);
                        } //all names and sizes are sent
                        outbox.AppendText("List of shared files is sent to Tracker (IP: " + tracker_ip.ToString() + ")\n");
                        text = new Mutex(); //initialize
                        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        listener.Bind(new IPEndPoint(IPAddress.Any, local_port));
                        listener.Listen(50);  //start listening
                        text.WaitOne();
                        outbox.AppendText("Started listening on port " + local_port + "\n");
                        text.ReleaseMutex();
                        text.WaitOne();
                        outbox.AppendText("Listening port is sent to Tracker (IP: " + tracker_ip.ToString() + ")\n");
                        text.ReleaseMutex();
                        Tracker.Close();
                        text.WaitOne();
                        outbox.AppendText("Connection to Tracker (IP: " + tracker_ip.ToString() + ") is closed\n");
                        text.ReleaseMutex();
                        inc_files = new List<byte[]>();                     //
                        get_files = new List<List<ThreadStart>>();          //
                        get_file = new List<List<Thread>>();                //
                        search_list = new List<string>();                   //
                        part_sends = new List<ThreadStart>();               // initializations
                        part_send = new List<Thread>();                     //
                        requests = new List<ThreadStart>();                 //
                        request = new List<Thread>();                       //
                        inc_connections = new List<Socket>();               //
                        inits = new List<ThreadStart>();
                        init = new List<Thread>();
                        listens = delegate { thr_lis_acc(); };
                        listen = new Thread(listens);
                        listen.Start();                                     //start a thread to accept connecitons
                    }
                    else
                    {
                        FormatException wrong_format = new FormatException("The local port number needs to be between 0 and 65535.");
                        throw wrong_format;
                    }
                }
                else
                {
                    FormatException wrong_format = new FormatException("The tracker port number needs to be between 0 and 65535.");
                    throw wrong_format;
                }
            }
            catch (ArgumentNullException argnullex)
            {
                outbox.AppendText(argnullex + "\n" + "Please write an IP Address for Tracker.\n");
            }
            catch (FormatException formatex)
            {
                if (textBox1.Enabled == false)
                    outbox.AppendText(formatex + "\n" + "Please write a valid (integer) port number between 0 and 65535.\n");
                else
                    outbox.AppendText(formatex + "\n" + "Please write a valid (like a.b.c.d) IP Address for Tracker, where a, b, c and d are integers between 0 and 255.\n");
            }
        }

        private void dir_Click(object sender, EventArgs e)          //set path for downlaods
        {
            down_path = new FolderBrowserDialog();
            down_path.Description = "Please choose the directory to download files to:";
            down_path.ShowDialog();
            downPath = down_path.SelectedPath;
            if (downPath == "")     //make sure a path is selected
                outbox.AppendText("Please choose a directory for downloaded files and try again.\n");
            else
            {
                if (send.Enabled == false)
                    down.Enabled = true;
                dir.Enabled = false;
            }
        }

        private void down_Click(object sender, EventArgs e)  //request a file download, parrallel requests supported
        {
            down.Enabled = false;
            search_list.Add(textBox7.Text);
            textBox7.AppendText("");
            int tmp = search_list.Count() - 1;
            requests.Add(delegate { thr_search_file(tmp); });
            request.Add(new Thread(requests[tmp]));
            request[tmp].Start();
            down.Enabled = true;
        }

        private void thr_lis_acc()
        {
            try
            {
                while (run_program)
                {
                    inc_connections.Add(listener.Accept());     //switch connection to different port
                    int tmp = inc_connections.Count() - 1;
                    byte[] pfcode = new byte[1];
                    inc_connections[tmp].Receive(pfcode);
                    if (pfcode[0] == 123)
                    {           //connection is from the tracker making a send request
                        text.WaitOne();
                        outbox.AppendText("Tracker (IP: " + tracker_ip.ToString() + ") connected to FileSharerDownloader (IP: " + local_ip.ToString() + ") on port: " + ((IPEndPoint)inc_connections[tmp].LocalEndPoint).Port.ToString() + "\n");
                        text.ReleaseMutex();
                        int x = part_sends.Count();
                        part_sends.Add(delegate { thr_part_send(tmp); });
                        part_send.Add(new Thread(part_sends[x]));
                        part_send[x].Start();
                    }
                    else
                    {           //connection is from another FileSharerDownloader to send a part of a requested file
                        text.WaitOne();
                        outbox.AppendText("FileSharerDownloader (IP: " + ((IPEndPoint)inc_connections[tmp].RemoteEndPoint).Address.ToString() + ") connected to FileSharerDownloader (IP: " + local_ip.ToString() + ") on port: " + ((IPEndPoint)inc_connections[tmp].RemoteEndPoint).Port.ToString() + "\n");
                        text.ReleaseMutex();
                        int inict = inits.Count();
                        inits.Add(delegate { thr_init(tmp); });
                        init.Add(new Thread(inits[inict]));
                        init[inict].Start();
                    }
                }
            }
            catch (SocketException con_lost)       //connection is lost or cannot be made (to make the Close button function.
            {
                if (run_program)
                    outbox.AppendText(con_lost + "\n" + "There has been a problem in the connection please press \"Close\" and try again.\n");
            }
        }

        private void thr_init(int tmp)
        {
            byte[] search = new byte[4];
            byte[] part = new byte[4];
            inc_connections[tmp].Receive(search);       //which file
            inc_connections[tmp].Receive(part);         //which part of it
            int search_ind = BitConverter.ToInt32(search, 0);
            int part_ind = BitConverter.ToInt32(part, 0);
            get_files[search_ind][part_ind] = new ThreadStart(delegate { thr_get_part(tmp, search_ind, part_ind); });
            get_file[search_ind][part_ind] = new Thread(get_files[search_ind][part_ind]);
            get_file[search_ind][part_ind].Start();
        }

        private void thr_get_part(int index, int search_index, int part_index)
        {
            if (part_index == 0)
            {
                byte[] startp = new byte[8];
                inc_connections[index].Receive(startp);
                long start = BitConverter.ToInt64(startp, 0);
                byte[] finishp = new byte[8];
                inc_connections[index].Receive(finishp);
                long finish = BitConverter.ToInt64(finishp, 0);
                long part_size = finish - start + 1;
                byte[] part = new byte[part_size];
                inc_connections[index].Receive(part);
                for (int i = 0; i < part_size; i++)
                    inc_files[search_index][start + i] = part[i];
                File.WriteAllBytes(downPath + "\\" + search_list[search_index] + "." + (part_index + 1).ToString(), part);
                text.WaitOne();
                outbox.AppendText(search_list[search_index] + " bytes: " + start + " - " + finish + " is recieved\n");
                text.ReleaseMutex();
                inc_connections[index].Disconnect(false);
                text.WaitOne();
                outbox.AppendText("Connection to FileSharerDownloader (IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + ") is closed\n");
                text.ReleaseMutex();
                inc_connections[index].Close();
            }
            else if (part_index == 1)
            {
                byte[] startp = new byte[8];
                inc_connections[index].Receive(startp);
                long start = BitConverter.ToInt64(startp, 0);
                byte[] finishp = new byte[8];
                inc_connections[index].Receive(finishp);
                long finish = BitConverter.ToInt64(finishp, 0);
                long part_size = finish - start + 1;
                byte[] part = new byte[part_size];
                inc_connections[index].Receive(part);
                for (int i = 0; i < part_size; i++)
                    inc_files[search_index][start + i] = part[i];
                File.WriteAllBytes(downPath + "\\" + search_list[search_index] + "." + (part_index + 1).ToString(), part);
                text.WaitOne();
                outbox.AppendText(search_list[search_index] + " bytes: " + start + " - " + finish + " is recieved\n");
                text.ReleaseMutex();
                inc_connections[index].Disconnect(false);
                text.WaitOne();
                outbox.AppendText("Connection to FileSharerDownloader (IP: " + ((IPEndPoint)inc_connections[index].RemoteEndPoint).Address.ToString() + ") is closed\n");
                text.ReleaseMutex();
                inc_connections[index].Close();
            }
        }

        private void thr_search_file(int index)
        {
            string file = search_list[index];
            byte[] code = new byte[1];
            code[0] = 200;      //means the connection is for a file request
            byte[] search_index = BitConverter.GetBytes(index);
            byte[] file_name_size = new byte[1];
            file_name_size[0] = (BitConverter.GetBytes(file.Length))[0];
            byte[] file_name = Encoding.Default.GetBytes(file);
            Socket request_file = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            request_file.Connect(tracker_ip, tracker_port);
            request_file.Send(code);
            request_file.Send(search_index);
            request_file.Send(file_name_size);
            request_file.Send(file_name);
            byte[] answer = new byte[4];
            request_file.Receive(answer);
            int ack = BitConverter.ToInt32(answer, 0);
            long size;
            if (ack == 0)
            {
                text.WaitOne();
                outbox.AppendText("Negative ack recieved from Tracer for " + file);
                size = 0;
            }
            else
            {
                byte[] file_size = new byte[8];
                request_file.Receive(file_size);
                size = BitConverter.ToInt64(file_size, 0);
                byte[] myport = BitConverter.GetBytes(local_port);
                request_file.Send(myport);
                text.WaitOne();
                outbox.AppendText("Positive ack recieved from Tracer for " + file + "size: " + size);
            }
            outbox.AppendText(" (" + ack + " FileSharerDownloader");
            if (ack > 1)
                outbox.AppendText("s");
            get_files.Add(new List<ThreadStart>());
            get_file.Add(new List<Thread>());
            int plk = get_files.Count() - 1;
            for (int jk = 0; jk < ack; jk++)
            {
                get_files[plk].Add(new ThreadStart(delegate { nothing(); }));
                get_file[plk].Add(new Thread(get_files[plk][jk]));
            }
            inc_files.Add(new byte[size]);
            outbox.AppendText(")\n");
            text.ReleaseMutex();
            request_file.Close();
            text.WaitOne();
            outbox.AppendText("Connection to Tracker (IP: " + tracker_ip.ToString() + ") is closed\n");
            text.ReleaseMutex();
            if (ack > 0)
            {
                int i = 0;
                while (i < ack)
                {
                    if (get_file[index][i].ThreadState == ThreadState.Unstarted)
                        Thread.Sleep(50); //make sure all the threads have started, avoid needless sleep time
                    else
                    {
                        get_file[index][i].Join();   //wait for a started thread to finish
                        i++;
                    }
                }
                File.WriteAllBytes(downPath + "\\" + file, inc_files[index]);
                text.WaitOne();
                outbox.AppendText("Reconstruction of " + search_list[index] + " is completed\n");
                text.ReleaseMutex();
            }
        }

        private void thr_part_send(int index)
        {
            byte[] file_name_size = new byte[1];
            inc_connections[index].Receive(file_name_size);
            int name_size = file_name_size[0];
            byte[] name = new byte[name_size];
            inc_connections[index].Receive(name);
            string fileName = Encoding.Default.GetString(name);
            byte[] search_index = new byte[4];
            inc_connections[index].Receive(search_index);
            byte[] part_index = new byte[4];
            inc_connections[index].Receive(part_index);
            byte[] startp = new byte[8];
            inc_connections[index].Receive(startp);
            long start = BitConverter.ToInt64(startp, 0);
            byte[] finishp = new byte[8];
            inc_connections[index].Receive(finishp);
            long finish = BitConverter.ToInt64(finishp, 0);
            byte[] ip_length = new byte[1];
            inc_connections[index].Receive(ip_length);
            int ip_len = ip_length[0];
            byte[] get_ip = new byte[ip_len];
            inc_connections[index].Receive(get_ip);
            byte[] get_port = new byte[4];
            inc_connections[index].Receive(get_port);
            int file_port = BitConverter.ToInt32(get_port, 0);
            string ip_str = Encoding.Default.GetString(get_ip);
            IPAddress file_ip = IPAddress.Parse(ip_str);
            text.WaitOne();
            outbox.AppendText("Request recieved: IP: " + ip_str + ":" + file_port + " requests " + fileName + " bytes: " + start + " - " + finish + "\n");
            text.ReleaseMutex();
            byte[] file_part = new byte[finish - start + 1];
            string filePath = sharePath + "\\" + fileName;
            byte[] fullFile = File.ReadAllBytes(filePath);
            long part_size = finish - start + 1;
            for (int i = 0; i < part_size; i++)
                file_part[i] = fullFile[start + i];
            Socket sendparts = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sendparts.Connect(file_ip, file_port);
            byte[] pfcode = new byte[1];
            pfcode[0] = 111;
            sendparts.Send(pfcode);
            sendparts.Send(search_index);
            sendparts.Send(part_index);
            sendparts.Send(startp);
            sendparts.Send(finishp);
            sendparts.Send(file_part);
            text.WaitOne();
            outbox.AppendText(fileName + " bytes: " + start + " - " + finish + " is sent to IP: " + ip_str + ":" + file_port + "\n");
            text.ReleaseMutex();
            sendparts.Close();
            text.WaitOne();
            outbox.AppendText("Connection to FileSharerDownloader (IP: " + ip_str + ") is closed\n");
            text.ReleaseMutex();
            inc_connections[index].Close();
            text.WaitOne();
            outbox.AppendText("Connection to Tracker (IP: " + tracker_ip.ToString() + ") is closed\n");
            text.ReleaseMutex();
        }

        private void closer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nothing()
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            run_program = false;
            if(listener!=null)
                listener.Close();
        }
    }
}
