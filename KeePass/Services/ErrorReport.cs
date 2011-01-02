﻿using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;

namespace KeePass.Services
{
    internal static class ErrorReport
    {
        public static void Report(Exception ex)
        {
            var sb = new StringBuilder();

            AddErrorDetails(sb, ex);
            sb.AppendLine();

            AddDbInfo(sb);
            sb.AppendLine();

            AddDeviceInfo(sb);
            sb.AppendLine();

            AddAppInfo(sb);

            new EmailComposeTask
            {
                To = "wp7pass@hotmail.com",
                Subject = "Report: Open database error",
                Body = sb.ToString()
            }.Show();
        }

        private static void AddAppInfo(StringBuilder sb)
        {
            sb.AppendLine("7Pass info:");

            sb.Append("Version: ");
            sb.AppendLine(typeof(ErrorReport).Assembly.FullName);
        }

        private static void AddDbInfo(StringBuilder sb)
        {
            sb.AppendLine("Database info:");

            using (var store = IsolatedStorageFile
                .GetUserStoreForApplication())
            {
                if (!store.FileExists(Consts.DATABASE))
                {
                    sb.AppendLine("No database");
                    return;
                }

                using (var fs = store.OpenFile(
                    Consts.DATABASE, FileMode.Open))
                {
                    sb.Append("Database: ");
                    sb.Append(fs.Length);
                    sb.AppendLine(" bytes");
                }
            }
        }

        private static void AddDeviceInfo(StringBuilder sb)
        {
            sb.AppendLine("Device info:");

            sb.Append("Manufacturer: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceManufacturer"));

            sb.Append("Device: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceName"));

            sb.Append("Device ID: ");
            sb.AppendLine(Convert.ToBase64String(
                (byte[])DeviceExtendedProperties
                    .GetValue("DeviceUniqueId")));

            sb.Append("Firmware: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceFirmwareVersion"));

            sb.Append("Hardware: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceHardwareVersion"));

            sb.Append("Memory: ");
            sb.Append(DeviceExtendedProperties
                .GetValue("ApplicationCurrentMemoryUsage"));
            sb.Append(" (Max: ");

            sb.Append(DeviceExtendedProperties
                .GetValue("ApplicationPeakMemoryUsage"));

            sb.Append(")/");
            sb.Append(DeviceExtendedProperties
                .GetValue("DeviceTotalMemory"));
            sb.AppendLine();
        }

        private static void AddErrorDetails(
            StringBuilder sb, Exception ex)
        {
            sb.AppendLine("Error details:");

            sb.AppendLine(ex.GetType().FullName);
            
            sb.AppendLine(ex.Message);
            sb.AppendLine();

            sb.AppendLine("Stack Trace:");
            sb.AppendLine(ex.StackTrace);
        }
    }
}