﻿using Delpin.Mvc.Helpers;
using System.Threading.Tasks;

namespace Delpin.MVC.Services
{
    public interface IHttpService
    {
        Task<HttpResponseWrapper<T>> Get<T>(string url);
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data);
    }
}