using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SuperTroyanHourse
{
    internal class base64
    {
        public static void EncodeFile(string sourceFilePath, string destinationFilePath)
        {
            FileStream fs_src = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader br_src = new BinaryReader(fs_src);
            FileStream fs_dst = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Write);
            BinaryWriter bw_dst = new BinaryWriter(fs_dst);
            string temp;
            ASCIIEncoding asci = new ASCIIEncoding();
            while (fs_src.Length > fs_src.Position)
            {
                temp = string.Empty;
                temp += (char)br_src.ReadByte();
                if (fs_src.Position + 1 < fs_src.Length)
                    temp += (char)br_src.ReadByte();
                if (fs_src.Position + 1 < fs_src.Length)
                    temp += (char)br_src.ReadByte();
                bw_dst.Write(Convert.ToBase64String(asci.GetBytes(temp)));
                
            }
            bw_dst.Close();
            bw_dst.Dispose();
            br_src.Close();
            br_src.Dispose();
            fs_src.Close();
            fs_src.Dispose();
            fs_dst.Close();
            fs_dst.Dispose();
            temp = null;

        }
    }
}
