using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Models;
using System.Linq;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.freightpop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using System;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Models.Models.Misc;
using ordercloud.integrations.library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Commands
{
    public interface IOrderOrchestrationCommand
    {
        Task<string> GetOCAccessTokenAsync(string apiClientID);
        Task<string> GetFreightPopAccessTokenAsync(string supplierID);
        Task<string> GetFreightPopAccessTokenAsync();
        Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync();
        Task<List<ShipmentDetails>> GetFreightPopShipments(string freightPopToken, ShipmentSyncType shipmentSyncType);
        Task CreateShipmentInOrderCloudIfNeeded(ShipmentWorkItem shipmentWorkItem);
        Task CreateShipmentInOrderCloudIfNeeded(ShipmentDetails shipment);
    }

    public class OrderOrchestrationCommand : IOrderOrchestrationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly string _supplierID;
        private IFreightPopService _freightPopService;
        private IFreightPopService _freightPopServiceProd;
        private readonly AppSettings _appSettings;
        private readonly LogQuery _log;
        private readonly int NUMBER_OF_HISTORICAL_DAYS_TO_PULL_SHIPMENTS = 3;

        public OrderOrchestrationCommand(LogQuery log, AppSettings appSettings, string supplierID)
        {
			_oc = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = appSettings.OrderCloudSettings.ApiUrl,
                AuthUrl = appSettings.OrderCloudSettings.AuthUrl,
                ClientId = appSettings.OrderCloudSettings.ClientID,
                ClientSecret = appSettings.OrderCloudSettings.ClientSecret,
                Roles = new[]
                {
                    ApiRole.FullAccess
                }
            });
            _freightPopService = new FreightPopService(new FreightPopConfig()
            {
                BaseUrl = appSettings.FreightPopSettings.BaseUrl,
                Password = appSettings.FreightPopSettings.Password,
                Username = appSettings.FreightPopSettings.Username
            }); ;
            _freightPopServiceProd = new FreightPopService(new FreightPopConfig()
            {
                BaseUrl = appSettings.FreightPopSettingsProd.BaseUrl,
                Password = appSettings.FreightPopSettingsProd.Password,
                Username = appSettings.FreightPopSettingsProd.Username
            });
            _appSettings = appSettings;
            _supplierID = supplierID;
            _log = log;
        }

        string GetSupplierIDFromShipmentReference(string reference)
        {
            var orderID = reference.Split(':')[0];
            var supplierID = orderID.Split('0')[0];
            return supplierID;
        }

        public async Task<string> GetOCAccessTokenAsync(string apiClientID)
        {
            try { 
                var supplierOrderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
                {
                    ApiUrl = _appSettings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _appSettings.OrderCloudSettings.AuthUrl,
                    ClientId = apiClientID,
                    ClientSecret = _appSettings.OrderCloudSettings.ClientSecret,
                    Roles = new[]
                               {
                                     ApiRole.FullAccess
                                }
                });
                await supplierOrderCloudClient.AuthenticateAsync();
                var supplierToken = supplierOrderCloudClient.TokenResponse.AccessToken;
                return supplierToken;
            } catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(ex)
                {
                    RecordType = RecordType.Supplier,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.AuthenticateSupplierError,
                    Message = $"Failed to authenticate supplier with ApiClientID {apiClientID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.AuthenticateSupplierError, $"Error authenticating for ApiClient {apiClientID}");
            }
        }

        public async Task<string> GetFreightPopAccessTokenAsync()
        {
            try
            {
                // to replace this when two different freightpop environments are not needed
                return await _freightPopServiceProd.AuthenticateAync();
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(ex)
                {
                    RecordType = RecordType.Supplier,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.FreightPopAuthenticateError,
                    Message = $"Failed to authenticate for Four51 freightpop account. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.AuthenticateSupplierError, $"Failed to authenticate for Four51 freightpop account. Error: {ex.Message}");
            }
        }

        public async Task<string> GetFreightPopAccessTokenAsync(string supplierID)
        {
            try
            {
                // to replace this with logic to get different supplier account token
                return await _freightPopService.AuthenticateAync();
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(ex)
                {
                    RecordType = RecordType.Supplier,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.FreightPopAuthenticateError,
                    Message = $"Failed to authenticate in freightpop for {supplierID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.AuthenticateSupplierError, $"Failed to authenticate in freightpop for {supplierID}. Error: {ex.Message}");
            }
        }

        public async Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync()
        {
            // todo support over 100 suppliers
            var suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(pageSize: 100, filters: $"xp.SyncFreightPop=true");
            return suppliers.Items.ToList();
        }

        public async Task CreateShipmentInOrderCloudIfNeeded(ShipmentWorkItem shipmentWorkItem)
        {
            try
            {
                var orderCloudOrderID = await ValidateAndGetOrderCloudOrder(shipmentWorkItem);
                var buyerIDForOrder = await GetBuyerIDForSupplierOrder(orderCloudOrderID);
                await AddShipmentToOrderCloud(shipmentWorkItem, orderCloudOrderID, buyerIDForOrder);
            } catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded,
                    Message = $"Error creating ordercloud shipment for freightpop order id: {shipmentWorkItem.Shipment.ShipmentId}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded, $"Error creating ordercloud shipment for freightpop order id: {shipmentWorkItem.Shipment.ShipmentId}. Error: {ex.Message}");
            }
        }

        public async Task CreateShipmentInOrderCloudIfNeeded(ShipmentDetails shipment)
        {
            // this overload handles creating the shipment in ordercloud based solely on information on the shipment
            // used for Fedex Ship Manager sync
            try
            {
                var (supplierOrderID, lineItemID, buyerOrderID, supplierID) = await FindRelatedOCData(shipment);
                var buyerIDForOrder = await GetBuyerIDForSupplierOrder(supplierOrderID);
                var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID);
                var supplierOCAccessToken = await GetOCAccessTokenAsync(supplier.xp.ApiClientID);
                var shipmentWorkItem = new ShipmentWorkItem()
                {
                    Supplier = supplier,
                    SupplierOCToken = supplierOCAccessToken,
                    Shipment = shipment,
                };
                await AddShipmentToOrderCloud(shipmentWorkItem, supplierOrderID, buyerIDForOrder, lineItemID);
            }
            catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded,
                    Message = $"Error creating ordercloud shipment for freightpop order id: {shipment.ShipmentId}. Error: {ex.Message}",
                });
            }
        }

        private async Task AddShipmentToOrderCloud(ShipmentWorkItem shipmentWorkItem, string supplierOrderID, string buyerID, string lineItemID)
        {

            try
            {
                // for fedex ship manager sync
                // shipping all of a given line item

                var ocShipment = await _oc.Shipments.CreateAsync(OCShipmentMapper.Map(shipmentWorkItem.Shipment, buyerID), accessToken: shipmentWorkItem.SupplierOCToken);

                var lineItem = await _oc.LineItems.GetAsync(OrderDirection.Outgoing, supplierOrderID, lineItemID);
                await _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(lineItem, supplierOrderID), accessToken: shipmentWorkItem.SupplierOCToken);
            } catch (Exception ex)
                {
                    if(ex.Message == "IdExists: Object already exists.")
                    {
                        await _log.Save(new OrchestrationLog()
                        {
                            RecordType = RecordType.Order,
                            Level = LogLevel.Progress,
                            Action = Marketplace.Common.Models.Action.SyncShipments,
                            Message = $"Shipment already created: {shipmentWorkItem.Shipment.ShipmentId}",
                        });
                    }
                    else
                    {
                        throw new OrchestrationException(OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded, $"Error creating shipment in ordercloud. Error {ex.Message}");
                    }
            }
        }


        private async Task<Tuple<string, string, string, string>> FindRelatedOCData(ShipmentDetails shipment)
        {
            try
            {
                // expecting to parse reference field with format SEB000103-012:X001
                // OrderID:LineItemID
                // we are currently under the assumption that only one line item can be shipped and it must be fully shipped

                var supplierOrderID = shipment.Reference2.Split(':')[0];
                var lineItemID = shipment.Reference2.Split(':')[1];
                var buyerOrderID = supplierOrderID.Split('-')[0];
                var supplierID = supplierOrderID.Split('-')[1];
                await _oc.Orders.GetAsync(OrderDirection.Outgoing, supplierOrderID);
                await _oc.LineItems.GetAsync(OrderDirection.Outgoing, supplierOrderID, lineItemID);
                return new Tuple<string, string, string, string>(supplierOrderID, lineItemID, buyerOrderID, supplierID);
            }
            catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.ShipmentOrderIDParsingError,
                    Message = $"Error relating shipment to OC data, reference field not found or no related OC data exists.",
                    Current = JObject.FromObject(shipment)
                });
                throw new OrchestrationException(OrchestrationErrorType.ShipmentOrderIDParsingError, "Error relating shipment to OC data, reference field not found or no related OC data exists.");
            }
        }

        private async Task<string> ValidateAndGetOrderCloudOrder(ShipmentWorkItem shipmentWorkItem)
        {
            var ref1 = shipmentWorkItem.Shipment.Reference1;
            var potentialOrderCloudOrderID = "";
            if (ref1 != null && ref1.Length > 0)
            {
                var refPieces = ref1.Split('-');
                if(refPieces.Length == 3 || refPieces.Length == 2)
                {
                    // this will recognize a reference number for a FreightPOP order (OC order to a specific supplier location)
                    // or an othercloud order
                    potentialOrderCloudOrderID = refPieces[0] + '-' + refPieces[1];
                } else
                {
                    throw new OrchestrationException(OrchestrationErrorType.NoRelatedOrderCloudOrderFound, $"Could not find ordercloud order for freightpop ref1: {ref1}.");
                }
            }
            try
            {
                await _oc.Orders.GetAsync(OrderDirection.Incoming, potentialOrderCloudOrderID, shipmentWorkItem.SupplierOCToken);
            } catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.NoRelatedOrderCloudOrderFound, $"Could not find ordercloud order for freightpop ref1: {ref1}. Error: {ex.Message}");
            }
            return potentialOrderCloudOrderID;
        }

        public async Task<List<ShipmentDetails>> GetFreightPopShipments(string freightPopToken, ShipmentSyncType shipmentSyncType)
        {
            try
            {
                var daysToGetShipments = new List<int> { };
                for (var i = 0; i < NUMBER_OF_HISTORICAL_DAYS_TO_PULL_SHIPMENTS; i++)
                {
                    daysToGetShipments.Add(i);
                }
                var shipmentDetailsResponses = await Throttler.RunAsync(daysToGetShipments, 100, 2, (daysBack) =>
                {
                    // temporary ternary until we don't need to worry about multiple freightpop environments in one marketplace environment
                    return shipmentSyncType == ShipmentSyncType.SupplierFreightPopAccount ? _freightPopService.GetShipmentsByDate(daysBack, freightPopToken) : _freightPopServiceProd.GetShipmentsByDate(daysBack, freightPopToken);
                });
                // sometimes the values returned here are null for the response object which is not exactly what I would expect when there are no shipments
                // regardless we are filtering those out
                shipmentDetailsResponses = shipmentDetailsResponses.Where(shipmentDetailResponse => shipmentDetailResponse != null && shipmentDetailResponse.Data != null).ToList();
                if(shipmentDetailsResponses.Count() > 0)
                {
                    var shipmentsFromPastDays = shipmentDetailsResponses.SelectMany(response => response.Data).ToList();
                    return shipmentsFromPastDays;
                } else
                {
                    return new List<ShipmentDetails> { };
                }
            }
            catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.GetFreightPopShipments,
                    Message = $"Error retrieving shipments from FreightPOP. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.GetFreightPopShipments, $"Error retrieving shipments from FreightPOP. Error {ex.Message}");
            }
        }
        
        private async Task AddShipmentToOrderCloud(ShipmentWorkItem shipmentWorkItem, string supplierOrderID, string buyerID)
        {
            try
            {
                var ocShipment = await _oc.Shipments.CreateAsync(OCShipmentMapper.Map(shipmentWorkItem.Shipment, buyerID), accessToken: shipmentWorkItem.SupplierOCToken);

                await Throttler.RunAsync(shipmentWorkItem.Shipment.Items, 100, 1,
                (freightPopShipmentItem) =>
                {
                // currently this creates two items in ordercloud inadvertantely, platform bug is being resolved
                    return _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, supplierOrderID), accessToken: shipmentWorkItem.SupplierOCToken);
                });
            } catch (Exception ex)
            {
                if(ex.Message == "IdExists: Object already exists.")
                {
                    await _log.Save(new OrchestrationLog()
                    {
                        RecordType = RecordType.Order,
                        Level = LogLevel.Progress,
                        Action = Marketplace.Common.Models.Action.SyncShipments,
                        Message = $"Shipment already created: {shipmentWorkItem.Shipment.ShipmentId}",
                    });
                }
                else
                {
                    throw new OrchestrationException(OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded, $"Error creating shipment in ordercloud. Error {ex.Message}");
                }
            }
        }

        private async Task<string> GetBuyerIDForSupplierOrder(string orderCloudSupplierOrderID)
        {
            var buyerOrderID = orderCloudSupplierOrderID.Split("-").First();
            var relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            return relatedBuyerOrder.FromCompanyID;
        }

        private async Task<ListPage<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int page, string supplierToken)
        {
            var orders = await _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Incoming, page: page, pageSize: 100, filters: $"IsSubmitted=true&Status=Open&xp.StopShipSync=false", accessToken: supplierToken);
            if(orders.Meta.TotalCount > 0)
            {
                return orders;
            }
            return orders;
        }


    }
}
