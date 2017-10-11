using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Agebull.EntityModel.Config;
using Agebull.EntityModel.RobotCoder;

namespace Agebull.EntityModel.Designer.WebApi
{


    public sealed class ApiLogicalBuilder : EntityCoderBase
    {
        /// <summary>
        /// 是否可写
        /// </summary>
        protected override bool CanWrite => true;

        /// <summary>
        /// 名称
        /// </summary>
        protected override string FileSaveConfigName => "File_API_Logical_cs";
        /// <summary>
        /// 是否客户端代码
        /// </summary>
        protected override bool IsClient => true;

        /// <summary>
        ///     生成实体代码
        /// </summary>
        protected override void CreateBaCode(string path)
        {
            string file = Path.Combine(path, Entity.Name + "ApiLogical.cs");

            string code = $@"using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Agebull.Common.Logging;
using Gboxt.Common.DataModel;
using Yizuan.Service.Api;
using {NameSpace}.BusinessLogic;

namespace {NameSpace}.WebApi.EntityApi
{{
    /// <summary>
    /// {Entity.Description}实体操作API实现
    /// </summary>
    public partial class {Entity.Name}ApiLogical : I{Entity.Name}Api
    {{
        
        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name=""src"">参数</param>
        /// <returns>数据库对象</returns>
        private {Entity.Name}Data ToData({Entity.Name} src)
        {{
            return new {Entity.Name}Data
            {{
{CopyCode()}
            }};
        }}

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name=""src"">数据库对象</param>
        /// <returns>参数</returns>
        private {Entity.Name} FromData({Entity.Name}Data src)
        {{
            return new {Entity.Name}
            {{
{CopyCode()}
            }};
        }}
        /// <summary>
        ///     新增
        /// </summary>
        /// <param name=""data"">数据</param>
        /// <returns>如果为真并返回结果数据</returns>
        ApiResult<{Entity.Name}> I{Entity.Name}Api.AddNew({Entity.Name} data)
        {{
            return AddNew(data);
        }}

        /// <summary>
        ///     修改
        /// </summary>
        /// <param name=""data"">数据</param>
        /// <returns>如果为真并返回结果数据</returns>
        ApiResult<{Entity.Name}> I{Entity.Name}Api.Update({Entity.Name} data)
        {{
            return Update(data);
        }}

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name=""dataKey"">数据主键</param>
        /// <returns>如果为否将阻止后续操作</returns>
        ApiResult I{Entity.Name}Api.Delete(Argument<{Entity.PrimaryColumn.LastCsType}> dataKey)
        {{
            return Delete(dataKey);
        }}

        /// <summary>
        ///     分页
        /// </summary>
        ApiResult<ApiPageData<{Entity.Name}>> I{Entity.Name}Api.Query(PageArgument arg)
        {{
            return Query(arg);
        }}

        /// <summary>
        ///     新增
        /// </summary>
        /// <param name=""arg"">数据</param>
        /// <returns>如果为真并返回结果数据</returns>
        private ApiResult<{Entity.Name}> AddNew({Entity.Name} arg)
        {{
            try
            {{
                var data = ToData(arg);
                var vr = data.Validate();
                if(!vr.succeed)
                    return ApiResult<{Entity.Name}>.ErrorResult(ErrorCode.ArgumentError, vr.Messages.LinkToString(""；""));
                using (BusinessContextScope.CreateScope())
                {{
                    var bl = new {Entity.Name}BusinessLogic();
                    if (bl.AddNew(data))
                        return ApiResult<{Entity.Name}>.ErrorResult(ErrorCode.InnerError, BusinessContext.Current.LastMessage);
                    return ApiResult<{Entity.Name}>.Succees(FromData(data));
                }}
            }}
            catch (Exception e)
            {{
                LogRecorder.Exception(e);
                return ApiResult<{Entity.Name}>.ErrorResult(ErrorCode.InnerError,""内部错误"");
            }}
        }}

        /// <summary>
        ///     修改
        /// </summary>
        /// <param name=""arg"">数据</param>
        /// <returns>如果为真并返回结果数据</returns>
        private ApiResult<{Entity.Name}> Update({Entity.Name} arg)
        {{
            try
            {{
                var data = ToData(arg);
                var vr = data.Validate();
                if(!vr.succeed)
                    return ApiResult<{Entity.Name}>.ErrorResult(ErrorCode.ArgumentError, vr.Messages.LinkToString(""；""));
                using (BusinessContextScope.CreateScope())
                {{
                    var bl = new {Entity.Name}BusinessLogic();
                    if (bl.Update(data))
                        return ApiResult<{Entity.Name}>.ErrorResult(ErrorCode.InnerError, BusinessContext.Current.LastMessage);
                    return ApiResult<{Entity.Name}>.Succees(FromData(data));
                }}
            }}
            catch (Exception e)
            {{
                LogRecorder.Exception(e);
                return ApiResult<{Entity.Name}>.ErrorResult(ErrorCode.InnerError,""内部错误"");
            }}
        }}

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name=""arg"">数据主键</param>
        /// <returns>如果为否将阻止后续操作</returns>
        private ApiResult Delete(Argument<{Entity.PrimaryColumn.LastCsType}> arg)
        {{
            try
            {{
                using (BusinessContextScope.CreateScope())
                {{
                    var bl = new {Entity.Name}BusinessLogic();
                    if (bl.Delete(arg.Value))
                        return ApiResult.Error(ErrorCode.InnerError, BusinessContext.Current.LastMessage);
                    return ApiResult.Succees();
                }}
            }}
            catch (Exception e)
            {{
                LogRecorder.Exception(e);
                return ApiResult.Error(ErrorCode.InnerError,""内部错误"");
            }}
        }}

        /// <summary>
        ///     分页
        /// </summary>
        private ApiResult<ApiPageData<{Entity.Name}>> Query(PageArgument arg)
        {{
            try
            {{
                string message;
                if (!arg.Validate(out message))
                    return ApiResult<ApiPageData<{Entity.Name}>>.ErrorResult(ErrorCode.ArgumentError, message);
                using (BusinessContextScope.CreateScope())
                {{
                    var bl = new {Entity.Name}BusinessLogic();
                    var data = bl.PageData(arg.Page, arg.PageSize, arg.Order, arg.Desc, null);
                    return new ApiPageResult<{Entity.Name}>
                    {{
                        Result = true,
                        ResultData = new ApiPageData<{Entity.Name}>
                        {{
                            PageSize = arg.PageSize,
                            PageIndex = arg.Page,
                            RowCount = data.Total,
                            Rows = data.Data.Select(FromData).ToList()
                        }}
                    }};
                }}
            }}
            catch (Exception e)
            {{
                LogRecorder.Exception(e);
                return ApiResult<ApiPageData<{Entity.Name}>>.ErrorResult(ErrorCode.InnerError,""内部错误"");
            }}
        }}
    }}
}}";
            SaveCode(file, code);
        }

        string CopyCode()
        {
            StringBuilder code = new StringBuilder();
            bool isFirst = true;
            foreach (var property in Entity.Properties.Where(p => p.CanUserInput))
            {
                if (isFirst)
                    isFirst = false;
                else
                    code.AppendLine(",");
                code.Append($"                {property.Name} = src.{property.Name}");
            }
            return code.ToString();
        }
    }

}

