using AutoMapper;
using CventSalesforceSyncApi.Domain.DTO;
using SyncBridge.Domain.DTOs;
using SyncBridge.Domain.Models;
using SyncBridge.Domain.Models.CVENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Venue, VenueDto>();

            CreateMap<EventEntity, EventDto>()
             .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.ExternalEventId, opt => opt.MapFrom(src => src.ExternalEventId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
            .ForMember(dest => dest.Conference, opt => opt.MapFrom(src => src.Conference))
            .ForMember(dest => dest.EventFormat, opt => opt.MapFrom(src => src.EventFormat))
            .ForMember(dest => dest.LaunchDate, opt => opt.MapFrom(src => DateOnly.FromDateTime((DateTime)src.LaunchDate)))
            .ForMember(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.SFEventId, opt => opt.MapFrom(src => src.SFEventId));



            CreateMap<Attendee, AttendeeDto>()
            .ForMember(des => des.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(des => des.ExternalAttendeeId, opt => opt.MapFrom(src => src.ExternalAttendeeId))
            .ForMember(des => des.ExternalEventId, opt => opt.MapFrom(src => src.ExternalEventId))
            .ForMember(des => des.ExternalTicketTypeId, opt => opt.MapFrom(src => src.ExternalTicketTypeId))
            .ForMember(des => des.ExternalContactId, opt => opt.MapFrom(src => src.ExternalContactId))
            .ForMember(des => des.AttendeeName, opt => opt.MapFrom(src => src.AttendeeName))
            .ForMember(des => des.RegistrationDate, opt => opt.MapFrom(src => DateOnly.FromDateTime((DateTime)src.RegistrationDate)))
            .ForMember(des => des.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(des => des.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(des => des.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(des => des.SFEventId, opt => opt.MapFrom(src => src.SFEventId))
            .ForMember(des => des.SFTicketTypeId, opt => opt.MapFrom(src => src.SFTicketTypeId))
            .ForMember(des => des.SFContactId, opt => opt.MapFrom(src => src.SFContactId));


            CreateMap<TicketType, TicketTypeDto>()
                .ForMember(des => des.id, opt => opt.MapFrom(src => src.id))
                .ForMember(des => des.ExternalTicketTypeId, opt => opt.MapFrom(src => src.ExternalTicketTypeId))
                .ForMember(des => des.ExternalEventId, opt => opt.MapFrom(src => src.ExternalEventId))
                .ForMember(des => des.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(des => des.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(des => des.FeeId, opt => opt.MapFrom(src => src.FeeId))
                .ForMember(des => des.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(des => des.IncomeAccount, opt => opt.MapFrom(src => src.IncomeAccount))
                .ForMember(des => des.SFEventId, opt => opt.MapFrom(src => src.SFEventId))
                .ForMember(des => des.SFTicketTypeId, opt => opt.MapFrom(src => src.SFTicketTypeId));

            // CreateMap<ReceiptLines, ReceiptLine>();
            CreateMap<Receipt, ReceiptDto>()
                .ForMember(des => des.id, opt => opt.MapFrom(src => src.id))
                .ForMember(des => des.EventId, opt => opt.MapFrom(src => src.EventId))
                .ForMember(des => des.TransactionType, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(des => des.OrderTransactionType, opt => opt.MapFrom(src => src.OrderTransactionType))
                .ForMember(des => des.TransactionNumber, opt => opt.MapFrom(src => src.TransactionNumber))
                .ForMember(des => des.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(des => des.TransactionName, opt => opt.MapFrom(src => src.TransactionName))
                .ForMember(des => des.TransactionBatchNumber, opt => opt.MapFrom(src => src.TransactionBatchNumber))
                .ForMember(des => des.AuthenticationCode, opt => opt.MapFrom(src => src.AuthenticationCode))
                .ForMember(des => des.PostedDate, opt => opt.MapFrom(src => src.PostedDate))
                .ForMember(des => des.ExternalReceiptId, opt => opt.MapFrom(src => src.ExternalReceiptId))
                .ForMember(des => des.SFReceiptId, opt => opt.MapFrom(src => src.SFReceiptId))
                .ForMember(des => des.SFContactId, opt => opt.MapFrom(src => src.SFContactId))
                .ForMember(des => des.ExternalOrderId, opt => opt.MapFrom(src => src.ExternalOrderId))
                .ForMember(des => des.SFOrderId, opt => opt.MapFrom(src => src.SFOrderId))
                .ForMember(des => des.ReceiptLines, opt => opt.MapFrom(src => src.ReceiptLines))
                .ForMember(des => des.PaymentMethodDescription, opt => opt.MapFrom(src => src.PaymentMethodDescription));

            CreateMap<EventEntity, QueueModel>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.module, opt => opt.MapFrom(_ => "Event"))
            .ForMember(dest => dest.recordId, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.action, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SFEventId) ? "Create" : "Update"))
            .ForMember(dest => dest.moduleCreatedAt, opt => opt.MapFrom(src => src.CreatedDT))
            .ForMember(dest => dest.moduleUpdatedAt, opt => opt.MapFrom(src => src.ModifiedDT))
            .ForMember(dest => dest.moduleCreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.moduleUpdatedBy, opt => opt.MapFrom(src => src.ModifiedBy));

            CreateMap<Attendee, QueueModel>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.module, opt => opt.MapFrom(_ => "Attendee"))
                .ForMember(dest => dest.recordId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.action, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SFContactId) ? "Create" : "Update"))
               .ForMember(dest => dest.moduleCreatedAt, opt => opt.MapFrom(src => src.CreatedDT))
            .ForMember(dest => dest.moduleUpdatedAt, opt => opt.MapFrom(src => src.ModifiedDT))
            .ForMember(dest => dest.moduleCreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.moduleUpdatedBy, opt => opt.MapFrom(src => src.ModifiedBy));

            CreateMap<TicketType, QueueModel>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.module, opt => opt.MapFrom(_ => "TicketType"))
                .ForMember(dest => dest.recordId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.action, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SFTicketTypeId) ? "Create" : "Update"))
               .ForMember(dest => dest.moduleCreatedAt, opt => opt.MapFrom(src => src.CreatedDT))
            .ForMember(dest => dest.moduleUpdatedAt, opt => opt.MapFrom(src => src.ModifiedDT))
            .ForMember(dest => dest.moduleCreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.moduleUpdatedBy, opt => opt.MapFrom(src => src.ModifiedBy));

            CreateMap<Receipt, QueueModel>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.module, opt => opt.MapFrom(_ => "Receipt"))
                .ForMember(dest => dest.recordId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.action, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SFReceiptId) ? "Create" : "Update"))
               .ForMember(dest => dest.moduleCreatedAt, opt => opt.MapFrom(src => src.CreatedDT))
            .ForMember(dest => dest.moduleUpdatedAt, opt => opt.MapFrom(src => src.ModifiedDT))
            .ForMember(dest => dest.moduleCreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.moduleUpdatedBy, opt => opt.MapFrom(src => src.ModifiedBy));

            CreateMap<SalesOrder, QueueModel>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.module, opt => opt.MapFrom(_ => "SalesOrder"))
                .ForMember(dest => dest.recordId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.action, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SFOrderId) ? "Create" : "Update"))
               .ForMember(dest => dest.moduleCreatedAt, opt => opt.MapFrom(src => src.CreatedDT))
            .ForMember(dest => dest.moduleUpdatedAt, opt => opt.MapFrom(src => src.ModifiedDT))
            .ForMember(dest => dest.moduleCreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.moduleUpdatedBy, opt => opt.MapFrom(src => src.ModifiedBy));

        }
    }
}
