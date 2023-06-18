using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nvenger.Common.BaseModel;

namespace Domain
{
    public class BaseResponse
    {
        public BaseResponse(string message,string status)
        {
            this.message = message;
            this.status = status;
        }

        public string message { get; set; }
        public string status { get; set; }
    }
    
    public class BadRequestResponse : BaseResponse
    {
        public BadRequestResponse(string message,string status) : base (message,status)
        {
        }
    }
    
    public class InternalErrorResponse : BaseResponse
    {
        public InternalErrorResponse(string message,string status) : base (message,status)
        {
        }
    }

    public class CompletedResponse<T> : BaseResponse where T : new()  
    {
        public CompletedResponse(T data) : base("OK", "success")
        {
            this.data = data;
        }

        public T data { get; set; }
    }
    
    public class FailedResponse<T> : BaseResponse where T : new()  
    {
        public FailedResponse(T data) : base("FAIL", "error")
        {
            this.data = data;
        }

        public T data { get; set; }
    }
    
    public class NotFoundResponse : BaseResponse
    {
        public NotFoundResponse() : base("NOTFOUND", "Not Found")
        {
        }
    }

    
    public class BaseApiController : ControllerBase
    {
        protected ActionResult<ModelResponse> ResponseModel(ModelResponse modelResponse)
        {
            if(modelResponse.GetStatus())
                return StatusCode(200,modelResponse);
            else
            {
                return StatusCode(400,modelResponse);
            }
                
        }
        protected IActionResult ResponseBadRequest(string message,string status = "fail")
        {
            return StatusCode(400,new BadRequestResponse(message, status));
        }
        
        protected IActionResult ResponseInternalError(string message,string status = "fail")
        {
            return StatusCode(500,new InternalErrorResponse(message, status));
        }
        
        protected ActionResult<ModelResponse> ResponseBadRequestModelResponse(string messages)
        {
            ModelResponse resp = new ModelResponse();
            resp.message = messages;
            
            return StatusCode(400,resp);
        }
        
        protected ActionResult<ModelResponse> ResponseBadRequestModelState(ModelStateDictionary pMdl)
        {
            String messages = String.Join(Environment.NewLine, pMdl.Values.SelectMany(v => v.Errors)
                .Select( v => v.ErrorMessage + " " + v.Exception));
            
            ModelResponse resp = new ModelResponse();
            resp.message = messages;
            
            return resp;
        }
        
        protected IActionResult ResponseBadRequestException(Exception pEx)
        {
            return ResponseBadRequest(RebuildMessageException(pEx),"fail");
        }
        
        protected IActionResult ResponseInternalException(Exception pEx)
        {
            return ResponseInternalError(RebuildMessageException(pEx),"fail");
        }
        
        protected IActionResult ResponseSuccess<T>(T data) where T : new()  
        {
            return Ok(new CompletedResponse<T>(data));
        }

        protected IActionResult ResponseFail<T>(T data) where T : new()  
        {
            return Ok(new FailedResponse<T>(data));
        }

        protected IActionResult ResponseFailWithStatusCode<T>(int statusCode,T data) where T : new()  
        {
            return StatusCode(statusCode,new FailedResponse<T>(data));
        }
        
        protected IActionResult ResponseNotFound()
        {
            return Ok(new NotFoundResponse());
        }

        protected string RebuildMessageException(Exception pException)
        {
            var _StackFrame = new StackTrace(pException, true);
            StackFrame _ErrorFrame = null;
            string[] SkipMethod =
                {"RebuildMessageException", "ResponseInternalException", "ResponseBadRequestException"};
            for (int _i = 0; _i < _StackFrame.GetFrames().Length; ++_i)
            {
                var _Frame = _StackFrame.GetFrame(_i);
                string _MethodName = _Frame.GetMethod().Name;
                if( SkipMethod.Contains(_MethodName) ) continue;
                _ErrorFrame = _Frame;
            }
            if (_ErrorFrame != null)
            {
                var _Frame = _StackFrame.GetFrame(1);
                string _FileName =  Path.GetFileName(_Frame.GetFileName()); ;
                string _MethodName = _Frame.GetMethod().Name;
                var _LineNumber = _Frame.GetFileLineNumber();
                return  $"[{_FileName}] Method: {_MethodName},Line: {_LineNumber} {pException.Message}";
            }
            return  $"{pException.Message}";
        }

        protected IQueryable<T> AddWhereList<T>(IQueryable<T> initQuery,object list)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            foreach (PropertyInfo pi in list.GetType().GetProperties())
            {
                var _value = pi.GetValue(list, null);
                if (_value != null)
                {
                    var _column_type = Expression.PropertyOrField(parameter, pi.Name).Type;
                    Expression _set_value;
                    
                    if (_column_type.IsGenericType &&
                        _column_type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        _set_value = Expression.Convert(Expression.Constant(_value), _column_type) ;
                    }
                    else
                    {
                        _set_value = Expression.Constant(_value);
                    }
                    
                    var predicate = Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(Expression.PropertyOrField(parameter, pi.Name), _set_value), parameter);
                    initQuery = initQuery.Where(predicate);
                }
            }
            return initQuery;
        }
        
        protected IQueryable<T> AddWhereMinMax<T>(IQueryable<T> initQuery,params object[] list)
        {
           if( list.Length != 2 )
               return initQuery;
           
           if( list[0] == null || list[1] == null )
               return initQuery;
           
            string _realColumnName = list[0].GetType().Name.Replace("Min","");
            var parameter = Expression.Parameter(typeof(T), "p");
            var _column_type = Expression.PropertyOrField(parameter, _realColumnName).Type;
            Expression _set_value_min;
            Expression _set_value_max;
            if (_column_type.IsGenericType &&
                _column_type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                _set_value_min = Expression.Convert(Expression.Constant(list[0]), _column_type);
                _set_value_max = Expression.Convert(Expression.Constant(list[1]), _column_type);
            }
            else
            {
                _set_value_min = Expression.Constant(list[0]);
                _set_value_max = Expression.Constant(list[1]);
            }
            
            var predicate_min = Expression.Lambda<Func<T, bool>>(
                Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, _realColumnName), _set_value_min ), parameter);
            var predicate_max = Expression.Lambda<Func<T, bool>>(
                Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, _realColumnName), _set_value_max ), parameter);
            initQuery = initQuery.Where(predicate_min);
            initQuery = initQuery.Where(predicate_max);
            
            return initQuery;
        }
        
        protected DateTime? NormalizeDate(DateTime? pDate,out DateTime oMidnightTime)
        {
            if (pDate == null)
            {
                pDate = DateTime.Now.Date;
            }
            else
            {
                pDate = pDate.Value.Date; 
            }

            oMidnightTime = pDate.Value.AddDays(1).AddMilliseconds(-1);
            return pDate;
        }
        
        protected DateTime? NormalizeMonth(DateTime? pDate,out DateTime oEndMonthDateTime)
        {
            if (pDate == null)
            {
                pDate = DateTime.Now.Date;
            }
            else
            {
                pDate = pDate.Value.Date; 
            }

            var _firstDayOfMonth = new DateTime(pDate.Value.Year, pDate.Value.Month, 1);
            oEndMonthDateTime = _firstDayOfMonth.AddMonths(1).AddMilliseconds(-1);
            return _firstDayOfMonth;
        }
    }
}