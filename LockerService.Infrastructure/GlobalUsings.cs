// Global using directives

global using System.Linq.Expressions;
global using System.Text.Json;
global using LockerService.Application.Common.Constants;
global using LockerService.Application.Common.Persistence;
global using LockerService.Application.EventBus.Mqtt;
global using LockerService.Application.EventBus.Mqtt.Events;
global using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
global using LockerService.Application.EventBus.RabbitMq.Events.Orders;
global using LockerService.Domain.Entities;
global using MassTransit;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;