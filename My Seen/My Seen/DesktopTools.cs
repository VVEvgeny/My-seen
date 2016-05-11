﻿using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using MySeenLib;
using System.IO;
using static MySeenLib.MySeenWebApi;

namespace My_Seen
{
    public static class WebApi
    {
        public static string CheckUser(string email)
        {
            if (email.Length == 0)
            {
                return Resource.EnterEmail;
            }
            try
            {
                var req = WebRequest.Create(
                    ApiHost + ApiUsers + Md5Tools.GetMd5Hash(email.ToLower()) 
                    + "/" 
                    + (int)SyncModesApiUsers.IsUserExists
                    + "/" + ApiVersion
                    );
                var answer = GetResponseAnswer(new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd());
                if (answer != null)
                {
                    if (answer.Value != SyncJsonAnswer.Values.Ok)
                    {
                        if (answer.Value == SyncJsonAnswer.Values.UserNotExist)
                        {
                            return Resource.UserNotExist;
                        }
                        return answer.Value == SyncJsonAnswer.Values.NoLongerSupportedVersion ? Resource.NoLongerSupportedVersion : answer.Value.ToString();
                    }
                    return Resource.UserOK;
                }
                req.GetResponse().Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return Resource.ApiError;
        }
        public static bool Sync(Users user)
        {
            ServicePointManager.Expect100Continue = false; 

            var films = new List<SyncJsonData>();
            var mc = new ModelContainer();
            //Буду отдавать ему ВСЁ, так надежнее
            films.AddRange(
                mc.FilmsSet.Where(f => f.UsersId == user.Id).Select(Map)
                .Union(mc.SerialsSet.Where(f => f.UsersId == user.Id).Select(Map))
                .Union(mc.BooksSet.Where(f => f.UsersId == user.Id).Select(Map))
                );
            WebRequest req;
            SyncJsonAnswer answer;
            if (films.Count != 0)
            {
                req = WebRequest.Create(
                    ApiHost + ApiSync + Md5Tools.GetMd5Hash(user.Email.ToLower()) 
                    + "/" + (int)SyncModesApiData.PostAll
                    + "/" + ApiVersion
                    );
                req.Method = "POST";
                req.Credentials = CredentialCache.DefaultCredentials;
                ((HttpWebRequest)req).UserAgent = "MySeen";
                ((HttpWebRequest)req).ProtocolVersion = HttpVersion.Version10;//для прокси
                req.ContentType = "application/json";
                var postData = SetResponse(films);
                var byteArray = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = byteArray.Length;
                var dataStream = req.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                answer = GetResponseAnswer((new StreamReader(req.GetResponse().GetResponseStream())).ReadToEnd());
                req.GetResponse().Close();
                if (answer == null)
                {
                    MessageBox.Show(Resource.ApiError);
                    return false;
                }
            }

            req = WebRequest.Create(
                ApiHost + ApiSync + Md5Tools.GetMd5Hash(user.Email.ToLower()) 
                + "/" + (int)SyncModesApiData.GetAll
                + "/" + ApiVersion
                );

            var data = (new StreamReader(req.GetResponse().GetResponseStream())).ReadToEnd();
            req.GetResponse().Close();

            answer = GetResponseAnswer(data);

            if (answer != null)
            {
                if (answer.Value == SyncJsonAnswer.Values.UserNotExist)
                {
                    MessageBox.Show(Resource.UserNotExist);
                }
                else if (answer.Value == SyncJsonAnswer.Values.BadRequestMode)
                {
                    MessageBox.Show(Resource.BadRequestMode);
                }
                else MessageBox.Show(answer.Value.ToString()); 
            }
            else
            {
                //Для 2х БД алгоритм хороший, но тут есть 3 БД, надо между всеми...
                mc.FilmsSet.RemoveRange(mc.FilmsSet.Where(f => f.UsersId == user.Id));
                mc.SerialsSet.RemoveRange(mc.SerialsSet.Where(f => f.UsersId == user.Id));
                mc.BooksSet.RemoveRange(mc.BooksSet.Where(f => f.UsersId == user.Id));
                mc.SaveChanges();
                foreach (var film in GetResponse(data))
                {
                    if (film.DataMode==(int)DataModes.Film)
                    {
                        mc.FilmsSet.Add(MapToFilm(film, user.Id));
                    }
                    else if (film.DataMode == (int)DataModes.Serial)
                    {
                        mc.SerialsSet.Add(MapToSerial(film, user.Id));
                    }
                    else
                    {
                        mc.BooksSet.Add(MapToBook(film, user.Id));
                    }
                }
            }
            mc.SaveChanges();
            MessageBox.Show(Resource.SyncOK);
            return true;//LoadItemsToListView();
        }

        private static SyncJsonData Map(Films model)
        {
            if (model == null) return new SyncJsonData();

            return new SyncJsonData
            {
                DataMode = (int)DataModes.Film,
                Id = model.Id_R,
                Name = model.Name,
                DateChange = model.DateChange,
                DateSee = model.DateSee,
                Genre = model.Genre,
                Rating = model.Rating
            };
        }

        private static SyncJsonData Map(Serials model)
        {
            if (model == null) return new SyncJsonData();

            return new SyncJsonData
            {
                DataMode = (int)DataModes.Serial,
                Id = model.Id_R,
                Name = model.Name,
                DateChange = model.DateChange,
                Genre = model.Genre,
                Rating = model.Rating,
                DateBegin = model.DateBegin,
                DateLast = model.DateLast,
                LastSeason = model.LastSeason,
                LastSeries = model.LastSeries,
            };
        }

        private static SyncJsonData Map(Books model)
        {
            if (model == null) return new SyncJsonData();

            return new SyncJsonData
            {
                DataMode = (int)DataModes.Book,
                Id = model.Id_R,
                Name = model.Name,
                DateChange = model.DateChange,
                Genre = model.Genre,
                Rating = model.Rating,
                Authors=model.Authors,
               DateRead=model.DateRead,
            };
        }

        private static Films MapToFilm(SyncJsonData model, int userId)
        {
            if (model == null) return new Films();

            return new Films
            {
                Id_R = model.Id,
                Name = model.Name,
                DateChange = model.DateChange,
                DateSee = model.DateSee,
                Genre = model.Genre,
                Rating = model.Rating,
                UsersId = userId
            };
        }

        private static Serials MapToSerial(SyncJsonData model, int userId)
        {
            if (model == null) return new Serials();

            return new Serials
            {
                Id_R = model.Id,
                Name = model.Name,
                DateChange = model.DateChange,
                Genre = model.Genre,
                Rating = model.Rating,
                DateBegin = model.DateBegin,
                DateLast = model.DateLast,
                LastSeason = model.LastSeason,
                LastSeries = model.LastSeries,
                UsersId = userId
            };
        }

        private static Books MapToBook(SyncJsonData model, int userId)
        {
            if (model == null) return new Books();

            return new Books
            {
                Id_R = model.Id,
                Name = model.Name,
                DateChange = model.DateChange,
                Genre = model.Genre,
                Rating = model.Rating,
                UsersId = userId,
                Authors=model.Authors, 
                DateRead=model.DateRead
            };
        }
    }

    public delegate void MySeenEventHandler();
    public class MySeenEvent
    {
        public event MySeenEventHandler Event;
        public void Exec()
        {
            Event?.Invoke();
        }
    }
    public static class ErrorProviderTools
    {
        public static bool IsValid(ErrorProvider errorProvider)
        {
          foreach (Control c in errorProvider.ContainerControl.Controls)
                if (!string.IsNullOrEmpty(errorProvider.GetError(c)))
                    return false;
            return true;
        }
    }

    public static class Md5Tools
    {
        public static string GetMd5Hash(string input)
        {
            try
            {
                var md5Hash = MD5.Create();
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch (Exception)
            {
                // ignored
            }
            return string.Empty;
        }
        public static bool VerifyMd5Hash(string input, string hash)
        {
            var hashOfInput = GetMd5Hash(input);
            var comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }
    }
}