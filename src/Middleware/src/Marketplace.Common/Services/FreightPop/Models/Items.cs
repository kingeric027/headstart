using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Services.FreightPop
{
    public class Item
    {
        // freightPOP docs indicate that description is only required for some carriers
        [Required]
        public string Description { get; set; }
        // freightPOP docs indicate that this is required for local shipment
        [Required]
        public decimal FreightClass { get; set; }
        [Required]
        public decimal Height { get; set; }
        [Required]
        public decimal Length { get; set; }
        [Required]
        public PackageType PackageType { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public Unit Unit { get; set; }
        [Required]
        public decimal Width { get; set; }
        public string PackageId { get; set; }
    }

    // same as item, but includes innerpieces
    public class ShipmentItem
    {
        public InnerPieces InnerPieces { get; set; }
        public string Description { get; set; }
        public decimal FreightClass { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public PackageType PackageType { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public Unit Unit { get; set; }
        public decimal Width { get; set; }
        public string PackageId { get; set; }
    }
    public class InnerPieces
    {
        public PackageType PackageType { get; set; }
        public int Quantity { get; set; }
    }
    public enum PackageType
    {
        Barrel,
        Box,
        Bundle,
        Container,
        Crat,
        DhlExpressEnvelope,
        FedexEnvelope,
        FedexParcel,
        Flatbed,
        Intermodal,
        Pail,
        Pallet,
        Truckload,
        UpsEnvelope,
        UpsParcel,
        Truckload_26ft,
        Ocean20ftCon,
        Ocean40fthcCon,
        Ocean40ftCon,
        Ocean45fthcCon
    }

    public enum Accessorial
    {
        DeliveryAppointment,
        DestinationExhibition,
        DestinationInsideDelivery,
        DestinationLiftGate,
        DestinationSortAndSegregatee,
        Freezable,
        Hazmat,
        KeepFromFreezing,
        LimitedAccessDelivery,
        LimitedAccessPickup,
        OriginExhibition,
        OriginInsidePickup,
        OriginLiftGate,
        OriginSortAndSegregatee,
        ResidentialDelivery,
        ResidentialPickup
    }
    public enum Unit
    {
        kg_cm,
        lbs_inch
    }
}
