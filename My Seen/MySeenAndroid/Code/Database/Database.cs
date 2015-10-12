﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using System.Collections.Generic;
using SQLite;
using MySeenLib;
using System.IO;

namespace MySeenAndroid
{
    public class DatabaseHelper
    {
        private const int FILMS_VERSION = 1;
        private const int SERIALS_VERSION = 1;

        private static string LogTAG = "MySeenAndroid_DATABASE";
        private SQLiteConnection connection;
        private string DBName;
        private string databaseFilePath;

        public DatabaseHelper()
        {
            DBName = "myseen.db3";
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            databaseFilePath = System.IO.Path.Combine(folder, DBName);

            Log.Warn(LogTAG, "databaseFilePath=" + databaseFilePath);


            bool needCreateDB = !File.Exists(databaseFilePath);
            Log.Warn(LogTAG, "needCreateDB=" + needCreateDB.ToString());

            connection = new SQLiteConnection(databaseFilePath);

            if (needCreateDB) CreateTables();
            else
            {
                TablesVersion tv=connection.Table<TablesVersion>().Where(t => t.TableName == "Films").First();
                Log.Warn(LogTAG, "table Films version=" + tv.Version.ToString() + " current version=" + FILMS_VERSION.ToString());
                if (tv.Version < FILMS_VERSION)
                {
                    Log.Warn(LogTAG, "table films is OLD RECREAT");
                    connection.DropTable<Films>();
                    connection.CreateTable<Films>();
                    tv.Version = FILMS_VERSION;
                    connection.Update(tv);
                }
                tv = connection.Table<TablesVersion>().Where(t => t.TableName == "Serials").First();
                Log.Warn(LogTAG, "table Serials version=" + tv.Version.ToString() + " current version=" + SERIALS_VERSION.ToString());
                if (tv.Version < SERIALS_VERSION)
                {
                    Log.Warn(LogTAG, "table Serials is OLD RECREAT");
                    connection.DropTable<Serials>();
                    connection.CreateTable<Serials>();
                    tv.Version = SERIALS_VERSION;
                    connection.Update(tv);
                }
            }

            Log.Warn(LogTAG, "END DatabaseHelper()");
        }

        public void CreateTables()
        {
            Log.Warn(LogTAG, "CreateTables Create tables begin");
            connection.CreateTable<Films>();
            connection.CreateTable<Serials>();

            connection.CreateTable<TablesVersion>();
            connection.Insert(new TablesVersion { TableName = "Films", Version = FILMS_VERSION });
            connection.Insert(new TablesVersion { TableName = "Serials", Version = SERIALS_VERSION });
        }
        public int GetSerialsCount()
        {
            return connection.Table<Serials>().Count();
        }
        public int GetFilmsCount()
        {
            return connection.Table<Films>().Count();
        }
        public void Add(Films film)
        {
            connection.Insert(film);
        }
        public void Update(Films film)
        {
            connection.Update(film);
        }
        public bool isFilmExist(string name)
        {
            return connection.Table<Films>().Where(f => f.Name == name).Count() != 0;
        }
        public bool isFilmExistAndNotSame(string name,int id)
        {
            return connection.Table<Films>().Where(f => f.Name == name && f.Id != id).Count() != 0;
        }
        
        public bool isSerialExist(string name)
        {
            return connection.Table<Serials>().Where(f => f.Name == name).Count() != 0;
        }
        public void Add(Serials film)
        {
            //Log.Warn(LogTAG, "Add Serials begin name=" + film.Name);
            connection.Insert(film);
            //Log.Warn(LogTAG, "Add Serials end id=" + film.Id.ToString());
        }
        public IEnumerable<Films> GetFilms()
        {
            //Log.Warn(LogTAG, "GetFilms()");
            return connection.Table<Films>().OrderByDescending(f => f.DateSee);
        }
        public Films GetFilmById(int id)
        {
            return connection.Table<Films>().Where(f => f.Id == id).First();
        }
        public IEnumerable<Serials> GetSerials()
        {
            //Log.Warn(LogTAG, "GetSerials()");
            return connection.Table<Serials>().OrderByDescending(f=>f.DateLast);
        }
    }
}