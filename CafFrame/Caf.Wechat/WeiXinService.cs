using Caf.Cache;
using Caf.Core;
using Caf.Core.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Senparc.CO2NET.HttpUtility;
using Senparc.Weixin;
using Senparc.Weixin.CommonAPIs;
using Senparc.Weixin.Containers;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Analysis;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using Senparc.Weixin.MP.AdvancedAPIs.Media;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AdvancedAPIs.UserTag;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Wechat
{
    public class WeiXinService : IWeiXinService
    {
        private readonly IHttpSender _httpSender;
        private readonly IOptions<WechatAccountOptions> _options;
        public WeiXinService(IHttpSender httpSender, IOptions<WechatAccountOptions> options)
        {
            _httpSender = httpSender;
            _options = options;
        }

        #region 二维码

        /// <summary>
        /// 获取永久二维码地址：永久二维码最多10万个
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sceneStr">二维码参数，长度限制1到64</param>
        /// <returns></returns>
        public async Task<CreateQrCodeResult> GetForeverQrCodeUrlAsync(string token, string sceneStr)
        {
            var codeResult = await QrCodeApi.CreateAsync(token, 0, 0, QrCode_ActionName.QR_LIMIT_STR_SCENE, sceneStr);
            return codeResult;
        }

        /// <summary>
        /// 获取临时二维码地址
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sceneStr">二维码参数，长度限制1到64</param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        public async Task<CreateQrCodeResult> GetTempQRCodeAsync(string appId, string sceneStr, int expireSeconds = 3600)
        {
            var codeResult = await QrCodeApi.CreateAsync(appId, expireSeconds, 0, QrCode_ActionName.QR_STR_SCENE, sceneStr);
            return codeResult;
        }

        #endregion

        #region 菜单

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="menus"></param>
        /// <param name="conditional"></param>
        /// <returns></returns>
        public async Task<bool> CreateMenuAsync(string appId, string menus, bool conditional = false)
        {
            if (appId == null || menus == null)
            {
                return false;
            }
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(menus);
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    string token = AccessTokenContainer.GetAccessToken(appId);

                    var url = "https://api.weixin.qq.com/cgi-bin/menu/addconditional?access_token=" + token;
                    if (!conditional)
                    {
                        url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;
                    }
                    var wxResult = await Post.PostGetJsonAsync<WxJsonResult>(url, null, ms);
                    return wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<object> GetMenus(string appId)
        {
            var token = await AccessTokenContainer.GetAccessTokenResultAsync(appId);

            var url = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token=" + token.access_token;
            return Get.GetJson<object>(url);
        }

        public async Task<bool> DelConditional(string appId, string menuId)
        {
            if (appId == null || menuId == null)
            {
                return false;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var bytes = Encoding.UTF8.GetBytes($"{{\"menuid\":\"{menuId}\"}}");
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    string token = AccessTokenContainer.GetAccessToken(appId);

                    var url = "https://api.weixin.qq.com/cgi-bin/menu/delconditional?access_token=" + token;
                    var wxResult = await Post.PostGetJsonAsync<WxJsonResult>(url, null, ms);
                    return wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        #endregion

        #region 模板消息

        /// <summary>
        /// 单个发送模板消息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="openId"></param>
        /// <param name="templateId"></param>
        /// <param name="jsonData">{first:{value:null,color:null},...,remark:null}</param>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<WxJsonResult> SendTemplateMessageAsync(string appId, string openId, string templateId,
            string jsonData, string url = null)
        {
            var data = JObject.Parse(jsonData);
            var kvData = new Dictionary<string, TemplateDataItem>();
            foreach (var item in data.Properties())
            {
                //#173177
                var value = item.Value["value"]?.ToString() ?? "";
                var color = item.Value["color"]?.ToString() ?? "#173177";
                kvData.Add(item.Name, new TemplateDataItem(value, color));
            }

            var msgModel = new TempleteModel
            {
                touser = openId,
                template_id = templateId,
                url = url,
                data = kvData
            };
            string token = AccessTokenContainer.GetAccessToken(appId);
            string apiUrl = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";
            var wxResult = await CommonJsonSend.SendAsync(token, apiUrl, msgModel);
            return wxResult;
        }

        /// <summary>
        /// 批量发送模板消息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openIds"></param>
        /// <param name="templateId"></param>
        /// <param name="data"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<int> SendTemplateMessagesAsync(string appId, string[] openIds, string templateId,
            Dictionary<string, TemplateDataItem> data, string url = null)
        {
            int errorCount = 0;
            foreach (var openId in openIds)
            {
                try
                {
                    var result = await TemplateApi.SendTemplateMessageAsync(appId, openId, templateId, url, data);
                    if (result.errcode != ReturnCode.请求成功)
                    {
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                }
            }

            return errorCount;
        }

        /// <summary>
        /// 使用Token发送模板消息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="openId"></param>
        /// <param name="templateId"></param>
        /// <param name="jsonData">{first:{value:null,color:null},...,remark:null}</param>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<WxJsonResult> SendTemplateMessageByTokenAsync(string token, string openId, string templateId,
            string jsonData, string url = null)
        {
            var data = JObject.Parse(jsonData);
            var kvData = new Dictionary<string, TemplateDataItem>();
            foreach (var item in data.Properties())
            {
                //#173177
                var value = item.Value["value"]?.ToString() ?? "";
                var color = item.Value["color"]?.ToString() ?? "#173177";
                kvData.Add(item.Name, new TemplateDataItem(value, color));
            }

            var msgModel = new TempleteModel
            {
                touser = openId,
                template_id = templateId,
                url = url,
                data = kvData
            };
            string apiUrl = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";
            var wxResult = await CommonJsonSend.SendAsync(token, apiUrl, msgModel);
            //return HttpHelper.Post<SendTemplateMessageResult>(apiUrl, null, msg);//不能直接发送JObject

            return wxResult;
        }

        #endregion

        #region 客服接口
        //yyf - 20180814
        public async Task<WxJsonResult> SendCustomTextMessageAsync(string appId, string openId, string content)
        {
            //string token = AccessTokenContainer.GetAccessToken(appId);
            //string apiUrl = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";
            //var wxResult = await CommonJsonSend.SendAsync(token, apiUrl, msgModel);
            //return HttpHelper.Post<SendTemplateMessageResult>(apiUrl, null, msg);//不能直接发送JObject

            var wxResult = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendTextAsync(appId, openId, content);

            return wxResult;
        }

        //yyf - 20180814
        public async Task<WxJsonResult> SendArtcleMessageAsync(string appId, string openId, List<Senparc.NeuChar.Entities.Article> articles)
        {
            string token = AccessTokenContainer.GetAccessToken(appId);
            string apiUrl = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";
            var wxResult = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendNewsAsync(token, apiUrl, articles);

            return wxResult;
        }
        #endregion 客服接口


        #region 群发接口
        public async Task<SendResult> SendGroupTextMessageByTagIdAsync(string appId, string tagId, string content, string clientmsgid)
        {
            return await GroupMessageApi.SendGroupMessageByTagIdAsync(appId, tagId, content, GroupMessageType.text, false, false, clientmsgid, 10000);
        }

        public async Task<SendResult> SendGroupTextMessageByOpenIdsAsync(string appId, string content, string clientmsgid, params string[] openIds)
        {
            return await GroupMessageApi.SendGroupMessageByOpenIdAsync(appId,
                    GroupMessageType.text, content, clientmsgid, 10000, openIds);
        }

        public async Task<SendResult> SendGroupImageMessageByOpenIdAsync(string appId, string mediaId, params string[] openIds)
        {
            return await GroupMessageApi.SendGroupMessageByOpenIdAsync(appId, GroupMessageType.image, mediaId, ""
                , 10000, openIds);
        }
        public async Task<SendResult> SendGroupNewsMessageByOpenIdAsync(string appId, string mediaId, params string[] openIds)
        {
            return await GroupMessageApi.SendGroupMessageByOpenIdAsync(appId, GroupMessageType.mpnews, mediaId, null, 10000, openIds);
        }
        #endregion 群发接口

        #region 用户&标签

        /// <summary>
        /// 获取微信用户信息：昵称，头像等
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<UserInfoJson> GetUserInfoAsync(string appId, string openId)
        {
            return await UserApi.InfoAsync(appId, openId);
        }

        /// <summary>
        /// 批量获取微信用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<BatchGetUserInfoJsonResult> GetBatchUserInfoAsync(string appId, List<string> openIdList)
        {
            return await UserApi.BatchGetUserInfoAsync(appId, openIdList.Select(p => new BatchGetUserInfoData
            {
                openid = p,
                LangEnum = Language.zh_CN
            }).ToList());
        }

        /// <summary>
        /// 批量获取微信用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<OpenIdResultJson> GetBatchUserAsync(string appId, string nextOpenId)
        {
            return await UserApi.GetAsync(appId, nextOpenId);
        }

        #endregion

        #region 素材&图文

        /// <summary>
        /// 群发图文消息给所有用户
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="nextOpenId"></param>
        /// <returns></returns>
        public async Task<SendResult> SendNewsToAllUsersAsync(string appId, string mediaId)
        {
            return await GroupMessageApi.SendGroupMessageByGroupIdAsync(appId, null, mediaId,
                    GroupMessageType.mpnews, true, true);
        }

        public async Task<SendResult> SendNewsByTagIdAsync(string appId, string tagId, string mediaId, string clientmsgid)
        {
            return await GroupMessageApi.SendGroupMessageByTagIdAsync(appId, tagId, mediaId, GroupMessageType.mpnews, true, false, clientmsgid, 10000);
        }

        /// <summary>
        /// 群发图文消息给openidlsit用户
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="nextOpenId"></param>
        /// <returns></returns>
        public async Task<SendResult> SendNewsToGroupUsersAsync(string appId, string mediaId, string[] opendis)
        {
            return await GroupMessageApi.SendGroupMessageByOpenIdAsync(appId, GroupMessageType.mpnews, mediaId, null, 1000, opendis);
        }
        /// <summary>
        /// 预览图文消息-每日100次上限
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public async Task<SendResult> SendNewsPreviewAsync(string appId, string openId, string mediaId)
        {
            var wxRet = await GroupMessageApi.SendGroupMessagePreviewAsync(appId, GroupMessageType.mpnews, mediaId, openId);
            return wxRet;
        }

        /// <summary>
        /// 获取微信图片、音频素材（除了图文、视频）
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public async Task<Stream> GetForeverMediaAsync(string appId, string mediaId)
        {
            MemoryStream stream = new MemoryStream();
            {
                await MediaApi.GetForeverMediaAsync(appId, mediaId, stream);
                stream.Position = 0;

                return stream;
            }
        }

        /// <summary>
        /// 获取微信视频
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public async Task<GetForeverMediaVideoResultJson> GetForeverMediaVideoAsync(string appId, string mediaId)
        {
            return await MediaApi.GetForeverVideoAsync(appId, mediaId);
        }

        /// <summary>
        /// 获取素材数量（图文、图片、视频、音频）
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<GetMediaCountResultJson> GetMediaCountAsync(string appId)
        {
            var wxRet = await MediaApi.GetMediaCountAsync(appId);
            return wxRet;
        }

        /// <summary>
        /// 获取素材列表（图片、视频、音频）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mediaFileType"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<MediaList_OthersResult> GetOthersMediaListAsync(string appId, UploadMediaFileType mediaFileType, int offset, int count)
        {
            var wxRet = await MediaApi.GetOthersMediaListAsync(appId, mediaFileType, offset, count);
            return wxRet;
        }

        /// <summary>
        /// 获取图文消息列表
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public async Task<MediaList_NewsResult> GetForeverNewsListAsync(string appId, int offset, int count)
        {
            var wxRet = await MediaApi.GetNewsMediaListAsync(appId, offset, count);
            return wxRet;
        }

        /// <summary>
        /// 获取微信文章
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public async Task<GetNewsResultJson> GetForeverNewsAsync(string appId, string mediaId)
        {
            var wxRet = await MediaApi.GetForeverNewsAsync(appId, mediaId);
            return wxRet;
        }

        /// <summary>
        /// 粉丝总数：最大跨度7天。最多只能获取最近7天数据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<AnalysisResultJson<UserCumulateItem>> GetUserCumulateAsync(string token, string start, string end)
        {
            return await AnalysisApi.GetUserCumulateAsync(token, start, end);
        }

        /// <summary>
        /// 粉丝增减数据：一天可能有多条，根据user_source分组
        /// </summary>
        /// <param name="token"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<AnalysisResultJson<UserSummaryItem>> GetUserSummaryAsync(string token, string start, string end)
        {
            return await AnalysisApi.GetUserSummaryAsync(token, start, end);
        }

        #region 文章处理
        /// <summary>
        /// 文章阅读数
        /// </summary>
        /// <param name="token"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<AnalysisResultJson<ArticleSummaryItem>> GetArticleSummaryAsync(string appId, string start, string end)
        {
            return await AnalysisApi.GetArticleSummaryAsync(appId, start, end);
        }
        #endregion

        /// <summary>
        /// 上传素材(图片，语音，缩略图)
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<UploadForeverMediaResult> UploadForeverMediaAsync(string appId, string filePath)
        {
            var wxRet = await MediaApi.UploadForeverMediaAsync(appId, filePath);
            return wxRet;
        }
        /// <summary>
        /// 上传图文素材中的图片获取url
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<UploadImgResult> UploadImgAsync(string appId, string filePath)
        {
            var wxRet = await MediaApi.UploadImgAsync(appId, filePath);
            return wxRet;
        }
        /// <summary>
        /// 上传视频
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<UploadForeverMediaResult> uploadVideopasync(string appId, string filePath)
        {
            var wxRet = await MediaApi.UploadForeverVideoAsync(appId, filePath,"武田制作","武田出品");
            return wxRet;
        }
   
        /// <summary>
        /// 删除永久素材
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<WxJsonResult> DelForeverMediaAsync(string appid, string mediaId)
        {
            var wxRet = await MediaApi.DeleteForeverMediaAsync(appid, mediaId);
            return wxRet;
        }



        /// <summary>
        /// 上传永久素材-除了图文
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="stream"></param>
        /// <param name="mediaType"></param>
        /// <param name="inArticle"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<UploadForeverMediaResult> UploadForeverMediaAsync(string access_token, Stream stream,
           UploadMediaFileType mediaType, string fileName = null, string contentType = null)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={access_token}&type={mediaType.ToString()}";

            var fileList = new List<FormData>();
            fileList.Add(new FormData("type", "image"));

            if (stream != null)
            {
                fileList.Add(new FormData
                {
                    Name = "media",
                    IsFile = true,
                    FileName = fileName,
                    Stream = stream,
                    ContentType = contentType
                });
            }

            var wxRet = await _httpSender.UploadFileAsync<UploadForeverMediaResult>(url, null, fileList);
            return wxRet;
        }

        /// <summary>
        /// 上传永久图文素材
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="timeOut"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public async Task<UploadForeverMediaResult> UploadNewsAsync(string access_token, string newsstr, int timeOut = 10000)
        {

            var news = default(NewsModel[]);
            try
            {
                JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(newsstr);
                JArray jArray = (JArray)(jObject["articles"]);
                news = new NewsModel[jArray.Count];
                for (int i = 0; i < jArray.Count; i++)
                {
                    news[i] = jArray[i].ToObject<NewsModel>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Json格式错误！");
            }
            return await MediaApi.UploadNewsAsync(access_token, timeOut, news);
        }
        /// <summary>
        /// 上传永久图文素材
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="timeOut"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public async Task<UploadForeverMediaResult> UploadNewsAsync(string appid, NewsModel[] news, int timeOut = 10000)
        {
            var access_token = this.GetAccessToken(appid);

            return await MediaApi.UploadNewsAsync(access_token, timeOut, news);
        }
        /// <summary>
        /// 上传临时图文素材
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="timeOut"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public async Task<UploadTemporaryMediaResult> UploadTempNewsAsync(string appid, NewsModel[] news, int timeOut = 10000)
        {
            var access_token = this.GetAccessToken(appid);

            return await MediaApi.UploadTemporaryNewsAsync(access_token, timeOut, news);
        }
        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="timeOut"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public async Task<WxJsonResult> UpdateForeverNewsAsync(string appid, string mediaId, NewsModel news, int timeOut = 10000)
        {

            return await MediaApi.UpdateForeverNewsAsync(appid, mediaId, 0, news);


        }

        #endregion

        #region Ticket & Token

        /// <summary>
        /// 网页授权：根据code换取access_token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<OAuthAccessTokenResult> GetOAuthTokenAsync(string appId, string appSecret, string code)
        {
            return await OAuthApi.GetAccessTokenAsync(appId, appSecret, code);
        }

        /// <summary>
        /// 获取可用 jsApiTicket
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<string> GetJsApiTicket(string appId)
        {
            return await JsApiTicketContainer.GetJsApiTicketAsync(appId);
        }

        public JsSdkUiPackage GetJsSdkUiPackage(string appId, string appSecret, string url)
        {
            return JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
        }

        /// <summary>
        /// 接口调用access_token。统一管理access_token，防止滥用；
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<object> GetAccessTokenAsync(string appId)
        {
            try
            {
                var ret = (Senparc.Weixin.MP.Entities.AccessTokenResult)await AccessTokenContainer.GetAccessTokenResultAsync(appId);
                if (ret.errcode == ReturnCode.请求成功)
                {
                    return new
                    {
                        ret.access_token,
                        ret.expires_in
                    };
                }
                else
                {
                    return new
                    {
                        ret.errcode,
                        ret.errmsg
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    errcode = "获取Token出错",
                    ex.Message
                };
            }
        }

        public string GetAccessToken(string appId, bool getNew = false)
        {
            return AccessTokenContainer.GetAccessToken(appId, getNew);
        }

        public async Task<object> MatchMenus(string appId, string openIdPara)
        {
            using (var ms = new MemoryStream())
            {
                var bytes = Encoding.UTF8.GetBytes(openIdPara);
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                var token = await AccessTokenContainer.GetAccessTokenResultAsync(appId);
                var url = "https://api.weixin.qq.com/cgi-bin/menu/trymatch?access_token=" + token.access_token;
                var result = Post.PostGetJson<object>(url, null, ms);
                return result;
            }
        }

        public async Task<OAuthUserInfo> UpdateUserInfoFromWechat(string openId, string web_access_token, string appId)
        {
            if (!string.IsNullOrWhiteSpace(web_access_token))
            {
                OAuthUserInfo userInfo = await OAuthApi.GetUserInfoAsync(web_access_token, openId);             

                return userInfo;
            }

            return null;
        }

        #endregion

        /// <summary>
        /// 注册微信账号到AccessTokenContainer容器中
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public async Task<bool> RegisterWechatAccountAsync(string appId, string appSecret)
        {
            try
            {
                AccessTokenContainer.Register(appId, appSecret);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据微信网页授权code获取用户信息。并分配sanofiId
        /// </summary>
        /// <param name="code">微信oauth返回的code</param>
        /// <param name="appId">渠道appId</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<OAuthUserInfo> GetWechatUserByCodeAsync(string appId, string code, string state)
        {
            try
            {
                string channelAppId = appId; //渠道appId
                //获取openId      
                var wxRet = await GetOAuthTokenAsync("wx811c8f96fde28591", "d4e49dbee622f80e622b6c3b60a5e8da", code);
                if (wxRet.errcode == ReturnCode.请求成功)
                {
                    OAuthUserInfo oAuthUserInfo = null;
                    try
                    {
                        //尝试获取用户昵称，头像等
                        oAuthUserInfo = await UpdateUserInfoFromWechat(wxRet.openid, wxRet.access_token, appId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    //静默授权只能获取到openId，获取不到unionId
                    return oAuthUserInfo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<object> GetMPAccessToken(string appId, string appsecret)
        {

            MPAccessTokenInfo result = new MPAccessTokenInfo();
            try
            {
                DateTime dateTime = DateTime.Now;
                var accessTokenResult = await AccessTokenContainer.GetAccessTokenResultAsync(_options.Value.AppId);
                var accessTokenBag =  BaseContainer<AccessTokenBag>.TryGetItem(_options.Value.AppId);
                result.access_token = accessTokenResult.access_token;
                DateTime expireTime = accessTokenBag.AccessTokenExpireTime.DateTime;
                result.expires_in = (long)(expireTime - dateTime).TotalSeconds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public (string,string,string,string) getJsSdkSign(string url)
        {
            var timestamp = JSSDKHelper.GetTimestamp();
            var nonceStr = JSSDKHelper.GetNoncestr();
            string jsapi_ticket = JsApiTicketContainer.TryGetJsApiTicket(_options.Value.AppId, _options.Value.AppSecret);
            string strurl = url;
            var signature = JSSDKHelper.GetSignature(jsapi_ticket, nonceStr, timestamp, strurl);
            return (timestamp,nonceStr,signature, _options.Value.AppId);



        }
    }
}
