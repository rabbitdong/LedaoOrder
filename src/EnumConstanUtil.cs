using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LedaoOrder
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class EnumDisplayAttribute : Attribute
    {
        /// <summary>
        /// 要显示的名称
        /// </summary>
        public string DisplayStr;

        public EnumDisplayAttribute() { }

        /// <summary>
        /// Constructor of EnumDisplayAttribute
        /// </summary>
        /// <param name="displayStr">The display string.</param>
        public EnumDisplayAttribute(string displayStr)
        {
            this.DisplayStr = displayStr;
        }
    }

    /// <summary>
    /// 枚举名称显示类
    /// </summary>
    public static class EnumDisplay
    {
        private static readonly string JsonFmt = "{{\"value\":\"{0}\",\"name\":\"{1}\"}}";

        /// <summary>
        /// 显示该枚举的显示内容
        /// </summary>
        /// <param name="myenum">要显示的枚举值</param>
        /// <returns>该枚举的显示内容</returns>
        public static string ToDisplayStr(this Enum myenum)
        {
            FieldInfo fieldInfo = myenum.GetType().GetField(myenum.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                return fieldInfo.Name;

            return descriptionAttributes[0].DisplayStr;
        }

        /// <summary>
        /// 获取该枚举的Json表达式字符串
        /// </summary>
        /// <param name="myenum">要显示的枚举值</param>
        /// <returns>Json表达式内容</returns>
        public static string ToJsonStr(this Enum myenum)
        {
            string showstr = string.Empty;
            var fieldInfo = myenum.GetType().GetField(myenum.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                showstr = fieldInfo.Name;
            else
                showstr = descriptionAttributes[0].DisplayStr;

            return string.Format(JsonFmt, (int)fieldInfo.GetValue(myenum), showstr);
        }
    }

    #region 送货方式  DeliverGoodsMethod
    /// <summary>
    /// 送货方式
    /// </summary>
    public enum DeliverWay
    {
        /// <summary>
        /// 送货上门
        /// </summary>
        [EnumDisplay("送货上门")]
        ByManual,

        /// <summary>
        /// 快递
        /// </summary>
        [EnumDisplay("快递")]
        ByExpress,

        /// <summary>
        /// 平邮
        /// </summary>
        [EnumDisplay("平邮")]
        ByPost
    }
    #endregion

    #region 订单的付款方式  PaymentType
    /// <summary>
    /// 订单的付款方式
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// 货到付款
        /// </summary>
        [EnumDisplay("货到付款")]
        CashOnDelivery = 0,

        /// <summary>
        /// 支付宝支付
        /// </summary>
        [EnumDisplay("支付宝支付")]
        OnLine = 1,

        /// <summary>
        /// 银行转账
        /// </summary>
        [EnumDisplay("银行转账")]
        Transfer = 2
    }
    #endregion

    #region 订单状态  OrderStatus
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 一个特殊的状态，可以表示所有状态
        /// </summary>
        [EnumDisplay("所有状态")]
        AllStatus = -1,

        /// <summary>
        /// 下订单，还没有发货
        /// </summary>
        [EnumDisplay("未发货")]
        Ordered,

        /// <summary>
        /// 订单被取消
        /// </summary>
        [EnumDisplay("已取消")]
        Cancel,

        /// <summary>
        /// 已经发货，在送货过程中
        /// </summary>
        [EnumDisplay("已发货")]
        Sending,

        /// <summary>
        /// 用户已经收到商品
        /// </summary>
        [EnumDisplay("已收货")]
        Received,

        /// <summary>
        /// 订单被退货
        /// </summary>
        [EnumDisplay("已退货")]
        Rejected
    }
    #endregion


    /// <summary>
    /// The enum item used for the binding.
    /// </summary>
    public class BindedEnumItem
    {
        public Enum enumValue;

        public override string ToString()
        {
            return enumValue.ToDisplayStr();
        }
    }

    public static class EnumConstantList
    {
        /// <summary>
        /// Get all the order status enumeration.
        /// </summary>
        public static BindedEnumItem[] OrderStatusList = {new BindedEnumItem{enumValue=OrderStatus.Ordered},
                                                             new BindedEnumItem{enumValue=OrderStatus.Sending},
                                                             new BindedEnumItem{ enumValue=OrderStatus.Cancel},                                                         
                                                         new BindedEnumItem{enumValue=OrderStatus.Received}
                                                         };
    }
}
