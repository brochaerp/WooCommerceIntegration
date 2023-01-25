using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TkoModel;

namespace AcendaService
{
    public static class SalesOrderExtension
    {
        public static void SetCustomer(this SalesOrder order, Customer customer)
        {
            order.CustomerId.CurrentValue = customer.CustomerId.CurrentValue;
            order.AccountNumber.CurrentValue = customer.AccountNumber.CurrentValue;
            order.IsTaxable.CurrentValue = customer.IsTaxable.CurrentValue;
            order.TermId.UserValue = customer.TermId.CurrentValue;

            order.SalesForceId.CurrentValue = customer.SalesForceId.CurrentValue;
            order.FieldSalesForceId.CurrentValue = customer.FieldSalesForceId.CurrentValue;

            var location = customer.MyLocations.FirstOrDefault();
            order.LocationId.CurrentValue = (location != null) ? location.LocationId.CurrentValue : Guid.Empty;

            order.CarrierServiceId.CurrentValue = customer.CarrierServiceId.CurrentValue;
            order.FreightPaymentMethod.CurrentValue = customer.FreightPaymentMethod.CurrentValue;
            order.CreditLimit.CurrentValue = customer.CreditLimit.CurrentValue;
            order.OrderLimit.CurrentValue = customer.OrderLimit.CurrentValue;
            order.DaysPastDue.CurrentValue = customer.DaysPastDue.CurrentValue;
            order.MinimumSalesMargin.CurrentValue = customer.MinimumSalesMargin.CurrentValue;
            order.OpenDocumentBalance.CurrentValue = customer.OpenDocumentBalance.CurrentValue;
            order.PastDueHoldDays.CurrentValue = customer.PastDueHoldDays.CurrentValue;
            order.WarehouseId.CurrentValue = customer.WarehouseId.CurrentValue;
            order.AllowBackorder.CurrentValue = customer.AllowBackorder.CurrentValue;
            order.IsBlindShip.CurrentValue = customer.IsBlindShip.CurrentValue;
            order.IsShipComplete.CurrentValue = customer.IsShipComplete.CurrentValue;
            order.IsWillCall.CurrentValue = customer.IsWillCall.CurrentValue;
            order.FreightAllowanceId.CurrentValue = customer.FreightAllowanceId.CurrentValue;
            order.FreightChargeId.CurrentValue = customer.FreightChargeId.CurrentValue;
            order.OnlyPrintTotals.CurrentValue = customer.OnlyPrintTotals.CurrentValue;
            order.LateAfterDate.CurrentValue = ((DateTime)order.OrderedOnDate.CurrentValue).AddDays(customer.OrderLateDays.CurrentValue);
            order.PickOnDate.CurrentValue = ((DateTime)order.OrderedOnDate.CurrentValue).AddDays(customer.OrderPickDays.CurrentValue);
            order.CurrencyCode.CurrentValue = customer.CurrencyCode.CurrentValue;
            order.FOB.CurrentValue = customer.FOB.CurrentValue;

            if (!String.IsNullOrEmpty(customer.StaticPO.CurrentValue))
            {
                DateTime staticPOExpires = DateTime.Today.AddDays(1);

                if (customer.StaticPOExpires.CurrentValue != null)
                    staticPOExpires = (DateTime)customer.StaticPOExpires.CurrentValue;

                if (staticPOExpires > DateTime.Today)
                    order.CustomerPO.CurrentValue = customer.StaticPO.CurrentValue;
            }

            order.ShouldRecalculate = true;
        }


    }
}
