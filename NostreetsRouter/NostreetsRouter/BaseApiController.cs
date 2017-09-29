﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using NostreetsORM.Interfaces;
using NostreetsRouter.Interfaces;
using System.Net.Http;
using NostreetsORM;
using NostreetsRouter.Models.Responses;
using System.Net;
using NostreetsEntities;
using Nostreets_Services.Utilities;

namespace NostreetsRouter
{


    [RoutePrefix("api")]
    public class BaseApiController<T, IdType> : ApiController, IApiController<T, IdType> where T : class
    {
        public BaseApiController()
        {
            _typeSrv = new EFDBService<T, IdType>();

            Dictionary<object, Type> attr = new Dictionary<object, Type>();
            attr.Add(typeof(T).Name + "/{action}", typeof(string));

            this.AddAttribute<RouteAttribute>(false, attr, GetType().GetFields());
        }

        public BaseApiController(string connectionKey)
        {
            _typeSrv = new EFDBService<T, IdType>(connectionKey);

            Dictionary<object, Type> attr = new Dictionary<object, Type>();
            attr.Add(typeof(T).Name + "/{action}", typeof(string));

            this.AddAttribute<RouteAttribute>(false, attr, GetType().GetFields());
        }

        public BaseApiController(string connectionKey,  string serviceType)
        {
            switch (serviceType)
            {
                case "ado":
                    _typeSrv = new DBService<T, IdType>(connectionKey);
                    break;

                case "ef":
                    _typeSrv = new EFDBService<T, IdType>(connectionKey);
                    break;

                default:
                    _typeSrv = new EFDBService<T, IdType>(connectionKey);
                    break;
            }

            Dictionary<object, Type> attr = new Dictionary<object, Type>();
            attr.Add(typeof(T).Name + "/{action}", typeof(string));

            this.AddAttribute<RouteAttribute>(false, attr, GetType().GetFields());
        }

        IDBService<T, IdType> _typeSrv = null;

        [HttpDelete]
        public HttpResponseMessage Delete(IdType id)
        {
            try
            {
                _typeSrv.Delete(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(IdType id)
        {
            try
            {
                _typeSrv.Get(id);
                ItemResponse<T> response = new ItemResponse<T>();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                _typeSrv.GetAll();
                ItemResponse<T> response = new ItemResponse<T>();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        public HttpResponseMessage Insert(T data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdType id = _typeSrv.Insert(data);
                    ItemResponse<T> response = new ItemResponse<T>();
                    return Request.CreateResponse(HttpStatusCode.OK, id); 
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPut]
        public HttpResponseMessage Update(T data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _typeSrv.Update(data);
                    ItemResponse<T> response = new ItemResponse<T>();
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
    }

}