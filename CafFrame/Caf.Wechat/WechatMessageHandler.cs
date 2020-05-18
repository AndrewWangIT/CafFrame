using Senparc.NeuChar;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.IO;
using System.Xml.Linq;

namespace Caf.Wechat
{
    public delegate IResponseMessageBase SubscribeEventDelete(WechatMessageHandler sender, RequestMessageEvent_Subscribe request);
    public delegate IResponseMessageBase ScanEventDelete(WechatMessageHandler sender, RequestMessageEvent_Scan request);
    public delegate IResponseMessageBase TextEventDelete(WechatMessageHandler sender, RequestMessageText request);
    public delegate IResponseMessageBase ImageEventDelete(WechatMessageHandler sender, RequestMessageImage request);
    public delegate IResponseMessageBase VoiceEventDelete(WechatMessageHandler sender, RequestMessageVoice request);
    public delegate IResponseMessageBase VideoEventDelete(WechatMessageHandler sender, RequestMessageVideo request);
    public delegate IResponseMessageBase UnsubscribeEventDelete(WechatMessageHandler sender, RequestMessageEvent_Unsubscribe request);
    public delegate IResponseMessageBase MassageEventDelete(WechatMessageHandler sender, RequestMessageEvent_MassSendJobFinish request);

    public class WechatMessageHandler : MessageHandler<WechatMessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        //yyf - 20180803
        /// <summary>
        /// Execute方法执行之前可执行此委托.
        /// </summary>
        public event Action<WechatMessageHandler> OnRequestExecuting;

        public event SubscribeEventDelete OnSubscribeEvent;
        public event UnsubscribeEventDelete OnUnsubscribeEvent;
        public event ScanEventDelete OnScanEvent;
        public event TextEventDelete OnTextEvent;
        public event ImageEventDelete OnImageEvent;
        public event VoiceEventDelete OnVoiceEvent;
        public event VideoEventDelete OnVideoEvent;
        public event MassageEventDelete OnMassageSendJobFinishEvent;
        public string AppId { get; set; }

        public WechatMessageHandler(Stream inputStream, PostModel model, int maxRecordCount = 0)
            : base(inputStream, model, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            GlobalMessageContext.ExpireMinutes = 3;
            this.AppId = model.AppId;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            IResponseMessageBase response = OnTextEvent?.Invoke(this, requestMessage);
            if (response != null)
            {
                return response;
            }

            return base.OnTextRequest(requestMessage);
        }

        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            IResponseMessageBase response = OnImageEvent?.Invoke(this, requestMessage);
            if (response != null)
            {
                return response;
            }

            return base.OnImageRequest(requestMessage);
        }

        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {           
            IResponseMessageBase response = OnVoiceEvent?.Invoke(this, requestMessage);
            if (response != null)
            {
                return response;
            }

            return base.OnVoiceRequest(requestMessage);
        }

        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            IResponseMessageBase response = OnVideoEvent?.Invoke(this, requestMessage);
            if (response != null)
            {
                return response;
            }

            return base.OnVideoRequest(requestMessage);
        }

        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            IResponseMessageBase response = null;
            switch (requestMessage.Event)
            {
                case Event.ENTER:
                    break;
                case Event.LOCATION:
                    break;
                case Event.subscribe: //关注
                    response = OnSubscribeEvent?.Invoke(this, requestMessage as RequestMessageEvent_Subscribe);
                    break;
                case Event.unsubscribe:
                    response = OnUnsubscribeEvent?.Invoke(this, requestMessage as RequestMessageEvent_Unsubscribe);
                    break;
                case Event.CLICK:
                    break;
                case Event.scan://扫码
                    response = OnScanEvent?.Invoke(this, requestMessage as RequestMessageEvent_Scan);
                    break;
                case Event.VIEW:
                    break;
                case Event.MASSSENDJOBFINISH://模板消息推送结果回调
                    response = OnMassageSendJobFinishEvent?.Invoke(this, requestMessage as RequestMessageEvent_MassSendJobFinish);
                    break;
                case Event.TEMPLATESENDJOBFINISH:
                    break;
                case Event.scancode_push:
                    break;
                case Event.scancode_waitmsg:
                    break;
                case Event.pic_sysphoto:
                    break;
                case Event.pic_photo_or_album:
                    break;
                case Event.pic_weixin:
                    break;
                case Event.location_select:
                    break;
                case Event.card_pass_check:
                    break;
                case Event.card_not_pass_check:
                    break;
                case Event.user_get_card:
                    break;
                case Event.user_del_card:
                    break;
                case Event.kf_create_session:
                    break;
                case Event.kf_close_session:
                    break;
                case Event.kf_switch_session:
                    break;
                case Event.poi_check_notify:
                    break;
                case Event.WifiConnected:
                    break;
                case Event.user_consume_card:
                    break;
                case Event.user_view_card:
                    break;
                case Event.user_enter_session_from_card:
                    break;
                case Event.merchant_order:
                    break;
                case Event.submit_membercard_user_info:
                    break;
                case Event.ShakearoundUserShake:
                    break;
                case Event.user_gifting_card:
                    break;
                case Event.user_pay_from_pay_cell:
                    break;
                case Event.update_member_card:
                    break;
                case Event.card_sku_remind:
                    break;
                case Event.card_pay_order:
                    break;
                case Event.qualification_verify_success:
                    break;
                case Event.qualification_verify_fail:
                    break;
                case Event.naming_verify_success:
                    break;
                case Event.naming_verify_fail:
                    break;
                case Event.annual_renew:
                    break;
                case Event.verify_expired:
                    break;
                case Event.weapp_audit_success:
                    break;
                case Event.weapp_audit_fail:
                    break;
                default:
                    break;
            }

            if (response != null)
            {
                return response;
            }
            return base.OnEventRequest(requestMessage);
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageNoResponse>();
            //直接返回null，否则提示无法提供服务
            return null;
        }

        public override void OnExecuting()
        {
            this.OnRequestExecuting?.Invoke(this);
            base.OnExecuting();
        }


    }
}
