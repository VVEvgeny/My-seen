﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Web.Mvc;

public enum BrowserTypes
{
    Chrome = 1,
    Firefox,
    Opera,
    Safari,
    Ie,
    InternetExplorer
}

#region MD5Tools
public static class MD5Tools
{
    public static string GetMd5Hash(string input)
    {
        MD5 md5Hash = MD5.Create();
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
    public static bool VerifyMd5Hash(string input, string hash)
    {
        string hashOfInput = GetMd5Hash(input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        if (0 == comparer.Compare(hashOfInput, hash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
#endregion
