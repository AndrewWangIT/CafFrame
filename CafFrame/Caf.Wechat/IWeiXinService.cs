using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Caf.Core.DependencyInjection;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.Analysis;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using Senparc.Weixin.MP.AdvancedAPIs.Media;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AdvancedAPIs.UserTag;
using Senparc.Weixin.MP.Helpers;

namespace Caf.Wechat
{
    public interface IWeiXinService : IScoped
    {
        Task<bool> CreateMenuAsync(string appId, string menus, bool conditional = false);

        string GetAccessToken(string appId, bool getNew = false);
        Task<object> GetAccessTokenAsync(string appId);
        Task<AnalysisResultJson<ArticleSummaryItem>> GetArticleSummaryAsync(string appId, string start, string end);
        Task<OpenIdResultJson> GetBatchUserAsync(string appId, string nextOpenId);
        Task<BatchGetUserInfoJsonResult> GetBatchUserInfoAsync(string appId, List<string> openIdList);
        Task<Stream> GetForeverMediaAsync(string appId, string mediaId);
        Task<GetForeverMediaVideoResultJson> GetForeverMediaVideoAsync(string appId, string mediaId);
        Task<MediaList_OthersResult> GetOthersMediaListAsync(string appId, UploadMediaFileType mediaFileType, int offset, int count);
        Task<GetMediaCountResultJson> GetMediaCountAsync(string appId);
        Task<GetNewsResultJson> GetForeverNewsAsync(string appId, string mediaId);
        Task<CreateQrCodeResult> GetForeverQrCodeUrlAsync(string token, string sceneStr);
        Task<string> GetJsApiTicket(string appId);
        JsSdkUiPackage GetJsSdkUiPackage(string appId, string appSecret, string url);
        Task<object> GetMenus(string appId);
        Task<OAuthAccessTokenResult> GetOAuthTokenAsync(string appId, string appSecret, string code);

        Task<CreateQrCodeResult> GetTempQRCodeAsync(string appId, string sceneStr, int expireSeconds = 3600);
        Task<AnalysisResultJson<UserCumulateItem>> GetUserCumulateAsync(string token, string start, string end);
        Task<UserInfoJson> GetUserInfoAsync(string appId, string openId);
        Task<AnalysisResultJson<UserSummaryItem>> GetUserSummaryAsync(string token, string start, string end);

        Task<SendResult> SendNewsPreviewAsync(string appId, string openId, string mediaId);
        Task<SendResult> SendNewsToAllUsersAsync(string appId, string mediaId);

        Task<SendResult> SendNewsToGroupUsersAsync(string appId, string mediaId, string[] opendis);

        Task<WxJsonResult> SendTemplateMessageAsync(string appId, string openId, string templateId, string jsonData, string url = null);
        Task<int> SendTemplateMessagesAsync(string appId, string[] openIds, string templateId, Dictionary<string, TemplateDataItem> data, string url = null);
        Task<WxJsonResult> SendTemplateMessageByTokenAsync(string token, string openId, string templateId, string jsonData, string url = null);

        Task<UploadForeverMediaResult> UploadForeverMediaAsync(string access_token, Stream stream, UploadMediaFileType mediaType, string fileName = null, string contentType = null);
        Task<UploadForeverMediaResult> UploadForeverMediaAsync(string appId, string filePath);
        Task<UploadImgResult> UploadImgAsync(string appId, string filePath);
        Task<WxJsonResult> DelForeverMediaAsync(string appid, string mediaId);
        Task<UploadForeverMediaResult> UploadNewsAsync(string appid, string newsstr, int timeOut = 10000);
        Task<UploadForeverMediaResult> UploadNewsAsync(string access_token, NewsModel[] news, int timeOut = 10000);
        Task<WxJsonResult> UpdateForeverNewsAsync(string appid, string mediaId, NewsModel news, int timeOut = 10000);
        Task<object> MatchMenus(string appId, string openIdPara);
        Task<UploadForeverMediaResult> uploadVideopasync(string appId, string filePath);
        Task<SendResult> SendGroupTextMessageByTagIdAsync(string appId, string tagId, string content, string clientmsgid);
        Task<SendResult> SendGroupTextMessageByOpenIdsAsync(string appId, string content, string clientmsgid, params string[] openIds);
        Task<SendResult> SendGroupImageMessageByOpenIdAsync(string appId, string mediaId, params string[] openIds);
        Task<SendResult> SendGroupNewsMessageByOpenIdAsync(string appId, string mediaId, params string[] openIds);
        Task<WxJsonResult> SendCustomTextMessageAsync(string appId, string openId, string content);
        Task<bool> DelConditional(string appId, string menuId);
        Task<UploadTemporaryMediaResult> UploadTempNewsAsync(string appid, NewsModel[] news, int timeOut = 10000);

        /// <summary>
        /// 获取微信文章list
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        Task<MediaList_NewsResult> GetForeverNewsListAsync(string appId, int offset, int count);
        Task<SendResult> SendNewsByTagIdAsync(string appId, string tagId, string mediaId, string clientmsgid);

        Task<OAuthUserInfo> GetWechatUserByCodeAsync(string appId, string code, string state);
        Task<bool> RegisterWechatAccountAsync(string appId, string appSecret);

  
         (string, string, string,string) getJsSdkSign(string url);
        Task<object> GetMPAccessToken(string appId, string appSecret);
    }
}