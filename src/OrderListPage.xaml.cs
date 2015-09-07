using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using LedaoOrder.Models;

namespace LedaoOrder
{
    /// <summary>
    /// Interaction logic for OrderList.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {
        private DispatcherTimer timer;
        IEnumerable<OrderListViewModel> model = null;
        private int currentPageIndex = 0;
        private int countEachPage = 20;

        public OrderListPage()
        {
            InitializeComponent();            

            cmbOrderStatus.ItemsSource = EnumConstantList.OrderStatusList;
            cmbOrderStatus.SelectedIndex = 0;

            int totalCount = 0;
            try
            {
                List<OrderItem> orders = OrderProvider.GetPagedOrders(OrderStatus.Ordered, currentPageIndex, countEachPage, out totalCount);
                model = orders.Select(o => new OrderListViewModel
                {
                    OrderID = o.OrderID,
                    UserName = o.UserName,
                    OrderStatus = o.Status,
                    Address = o.Address,
                    Receiver = o.Receiver,
                    OrderTime = o.OrderTime,
                    ReceiverPhone = o.Phone,
                    Remark = o.Remark,
                    TotalAmount = o.TotalPrice
                });

                dgOrder.ItemsSource = model;
                txtTotalCount.Text = string.Format("总订单数：{0}", totalCount);
                SetPagedButtonState(totalCount);
            }
            catch (Exception)
            {
                ;
            }

            timer = new DispatcherTimer();
            timer.Tick += dispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0,0,10);
            timer.Start();
        }

        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private int CalcTotalPageCount(int totalCount)
        {
            return (totalCount + countEachPage - 1) / countEachPage;
        }

        private void SetPagedButtonState(int totalCount)
        {
            int totalPageCount = CalcTotalPageCount(totalCount);
            if (currentPageIndex > 0)
                btnPreviousPage.IsEnabled = true;
            else
                btnPreviousPage.IsEnabled = false;

            if (currentPageIndex >= totalPageCount - 1)
                btnNextPage.IsEnabled = false;
            else
                btnNextPage.IsEnabled = true;
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            OrderListViewModel model = link.DataContext as OrderListViewModel;
            
            NavigationService.Navigate(new OrderDetailPage(model.OrderID));
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            BindedEnumItem item = cmbOrderStatus.SelectedItem as BindedEnumItem;
            if ((OrderStatus)item.enumValue != OrderStatus.Ordered)
                return;

            int totalCount = 0;
            try
            {
                List<OrderItem> orders = OrderProvider.GetPagedOrders(OrderStatus.Ordered, currentPageIndex, countEachPage, out totalCount);
                model = orders.Select(o => new OrderListViewModel
                {
                    OrderID = o.OrderID,
                    UserName = o.UserName,
                    OrderStatus = o.Status,
                    Address = o.Address,
                    Receiver = o.Receiver,
                    OrderTime = o.OrderTime,
                    ReceiverPhone = o.Phone,
                    TotalAmount = o.TotalPrice,
                    Remark = o.Remark
                });

                txtTotalCount.Text = string.Format("总订单数：{0}", totalCount);
                SetPagedButtonState(totalCount);
                dgOrder.ItemsSource = model;
                dgOrder.Items.Refresh();
            }
            catch (Exception)
            {
                ;
            }
        }

        private void btnGetOrder_Click(object sender, RoutedEventArgs e)
        {
            BindedEnumItem item = cmbOrderStatus.SelectedItem as BindedEnumItem;

            currentPageIndex = 0;
            OrderStatus orderStatus = (OrderStatus)item.enumValue;
            GetAndShowOrders(orderStatus);
        }

        private void GetAndShowOrders(OrderStatus orderStatus)
        {
            int totalCount = 0;
            try
            {
                List<OrderItem> orders = OrderProvider.GetPagedOrders(orderStatus, currentPageIndex, countEachPage, out totalCount);
                model = orders.Select(o => new OrderListViewModel
                {
                    OrderID = o.OrderID,
                    UserName = o.UserName,
                    OrderStatus = o.Status,
                    Address = o.Address,
                    Receiver = o.Receiver,
                    OrderTime = o.OrderTime,
                    ReceiverPhone = o.Phone,
                    TotalAmount = o.TotalPrice,
                    Remark = o.Remark
                });

                txtTotalCount.Text = string.Format("总订单数：{0}", totalCount);
                dgOrder.ItemsSource = model;
                dgOrder.Items.Refresh();
                SetPagedButtonState(totalCount);
            }
            catch (Exception)
            {
                ;
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {   
            currentPageIndex++;
            OrderStatus orderStatus = (OrderStatus)(cmbOrderStatus.SelectedItem as BindedEnumItem).enumValue;
            GetAndShowOrders(orderStatus);
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex == 0)
                return;

            currentPageIndex--;
            OrderStatus orderStatus = (OrderStatus)(cmbOrderStatus.SelectedItem as BindedEnumItem).enumValue;
            GetAndShowOrders(orderStatus);
        }
    }
}
