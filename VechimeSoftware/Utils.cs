using Itenso.TimePeriod;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VechimeSoftware
{
    public enum HashType
    {
        MD5,
        SHA1,
        SHA512
    }

    public static class Utils
    {
        public static string HashFile(string filePath,HashType type)
        {
            switch(type)
            {
                case HashType.MD5:
                    {
                        byte[] hash = MD5.Create().ComputeHash(new FileStream(filePath, FileMode.Open));
                        StringBuilder s = new StringBuilder(hash.Length * 2);

                        for (int i = 0; i < hash.Length; i++)
                        {
                            s.Append(hash[i].ToString("X2").ToLower());
                        }

                        return s.ToString();
                    }
                    break;
                default:
                    return "";
                    break;
            }
        }

        public static string SHA1(string input)
        {
            byte[] hash;

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(input));
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.AppendFormat("{0:x2}", hash[i]);
            }

            return sb.ToString();
        }

        public static string GetMachineGuid()
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";

            using (RegistryKey localMachineX64View =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(
                            string.Format("Key Not Found: {0}", location));

                    object machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(
                            string.Format("Index Not Found: {0}", name));

                    return machineGuid.ToString();
                }
            }
        }

        public static DateDiff DateDiffFixed(DateTime date1, DateTime date2)
        {
            return new DateDiff(date1.Subtract(new TimeSpan(1, 0, 0, 0, 0)), date2);
        }
    }
}
