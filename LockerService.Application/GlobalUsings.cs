// Global using directives

global using System.Linq.Expressions;
global using System.Text.Json.Serialization;
global using AutoMapper;
global using FluentValidation;
global using LinqKit;
global using LockerService.Application.Common.Enums;
global using LockerService.Application.Common.Exceptions;
global using LockerService.Application.Common.Models.Request;
global using LockerService.Application.Common.Models.Response;
global using LockerService.Application.Common.Persistence;
global using LockerService.Application.Common.Services;
global using LockerService.Application.Common.Utils;
global using LockerService.Application.EventBus.Mqtt;
global using LockerService.Application.EventBus.Mqtt.Events;
global using LockerService.Application.EventBus.RabbitMq;
global using LockerService.Domain.Entities;
global using LockerService.Domain.Enums;
global using MediatR;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;