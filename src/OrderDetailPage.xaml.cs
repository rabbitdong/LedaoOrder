using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LedaoOrder.Models;

namespace LedaoOrder
{
    /// <summary>
    /// Interaction logic for OrderDetail.xaml
    /// </summary>
    public partial class OrderDetailPage : Page
    {
        public bool hasNextPageOrderDetails = false;
        public bool hasPrePageOrderDetails = false;

        //当前页索引，从0开始
        private int currentPageIndex = 0;
        private int totalPageCount;

        //每页显示的商品项数目
        private const int eachPagedItemNumber = 9;

        private OrderItem order;
        private OrderDetailViewModel detailModel;

        public OrderDetailPage(int orderID)
        {
            InitializeComponent();

            order = OrderProvider.GetOrder(orderID);

            if (order.Status != OrderStatus.Ordered)
            {
                btnSendOrder.Visibility = System.Windows.Visibility.Hidden;
                btnPrint.Content = "补打订单";
            }

            //转换成页面模型进行处理
            detailModel = new OrderDetailViewModel();
            detailModel.OrderID = order.OrderID;
            detailModel.Receiver = order.Receiver;
            detailModel.ReceiverPhone = order.Phone;
            detailModel.UserName = order.UserName;
            detailModel.TotalPrice = order.TotalPrice;
            detailModel.Address = order.Address;
            detailModel.OrderTime = order.OrderTime;
            detailModel.Remark = order.Remark;
            detailModel.DetailItems = new List<OrderDetailItemViewModel>();

            totalPageCount = (order.OrderDetails.Count + eachPagedItemNumber - 1) / eachPagedItemNumber;
            ShowPagedOrderDetail();

            btnPrePage.DataContext = this;
            btnNextPage.DataContext = this;
        }

        private void ShowPagedOrderDetail()
        {
            int beginIndex = currentPageIndex * eachPagedItemNumber;
            hasNextPageOrderDetails = order.OrderDetails.Count > (beginIndex + eachPagedItemNumber);
            hasPrePageOrderDetails = currentPageIndex > 0;
            int endIndex = hasNextPageOrderDetails ? (beginIndex + eachPagedItemNumber) : order.OrderDetails.Count;

            string amountString = string.Empty;
            detailModel.DetailItems.Clear();
            for (int i = beginIndex; i < endIndex; ++i)
            {
                OrderDetailItem orderDetail = order.OrderDetails[i];
                if (orderDetail.Unit == "斤")
                    amountString = string.Format("{0}{1}", orderDetail.Amount, orderDetail.Unit);
                else
                    amountString = string.Format("{0:d}{1}", (int)(orderDetail.Amount), orderDetail.Unit);
                detailModel.DetailItems.Add(new OrderDetailItemViewModel
                {
                    ProductName = orderDetail.ProductName,
                    ProductArea = orderDetail.ProducingArea,
                    AmountString = amountString,
                    UnitPrice = orderDetail.UnitPrice.ToString(),
                    TotalPrice = (orderDetail.UnitPrice * orderDetail.Amount).ToString("0.00")
                });
            }

            int addCount = eachPagedItemNumber - (endIndex - beginIndex);
            if (addCount < eachPagedItemNumber)
            {
                for (int i = 0; i < addCount; ++i)
                {
                    detailModel.DetailItems.Add(new OrderDetailItemViewModel { ProductName = "", ProductArea = "", AmountString = "", TotalPrice = "", UnitPrice = "" });
                }
            }

            if (totalPageCount > 1)
                txtTitle.Text = string.Format("乐道(水果君)水果订单（{0}）", currentPageIndex + 1);

            tbOrderDetail.ItemsSource = detailModel.DetailItems;
            tbOrderDetail.Items.Refresh();

            txtOrderID.Text = string.Format("订 单 号：{0:00000000}", detailModel.OrderID);
            txtUserInfo.Text = string.Format("客　　户：{0}", detailModel.Receiver);
            txtReceiver.Text = string.Format("收 货 人：{0}", detailModel.Receiver);
            txtOrderDate.Text = string.Format("下单时间：{0:yyyy年M月d日 H时}", detailModel.OrderTime);
            txtSendAddress.Text = string.Format("送货地址：{0}", detailModel.Address);
            txtPhone.Text = string.Format("客户电话：{0}", detailModel.ReceiverPhone);
            txtTotalPrice.Text = string.Format("总金额：{0}元", detailModel.TotalPrice);
            txtRemark.Text = string.Format("备注：{0}", detailModel.Remark);
        }

        /// <summary>
        /// Print the order detail.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintTicket pt = printDialog.PrintTicket;
            pt.PageMediaSize = new PageMediaSize(24.1, 14.0);
            pt.PageOrientation = PageOrientation.Portrait;
            printDialog.PrintVisual(gdOrerDetailContent, "乐道订单");
        }

        /// <summary>
        /// The event of send the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrderProvider.SendOrder(detailModel.OrderID))
            {
                txtMsg.Text = "本订单已经发货！";
            }
        }

        /// <summary>
        /// show the order detail of next page(if there's a next page).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (hasNextPageOrderDetails)
                currentPageIndex++;
            ShowPagedOrderDetail();
        }

        /// <summary>
        /// show the order detail of previous page(if there's a previous page).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrePage_Click(object sender, RoutedEventArgs e)
        {
            if (hasPrePageOrderDetails)
                currentPageIndex--;
            ShowPagedOrderDetail();
        }    
    }
}
