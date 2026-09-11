---
description: Add a domain event and its handler(s) so other parts of the app (typically in-memory caches) react when a command changes state. Use when a create/update/delete handler needs to notify other services, not for request/response data flow.
applyTo: "*.Core/**/Events/**/*.cs"
---

# Add a Domain Event

Events decouple "something changed" from "who needs to react." See
`docs/03-feature-development-guide.md` §4 for a full generic example.

## Steps

1. **Create the event** in `<Domain>/Events/<EventName>/<EventName>Event.cs`, extending
   `ApplicationEventBase` (`Shared/Events/ApplicationEventBase.cs`). Keep it a thin carrier of the
   changed model — primary-constructor style is fine:
   ```csharp
   public class OrderChangedEvent(OrderModel model) : ApplicationEventBase
   {
       public OrderModel Model { get; } = model;
   }
   ```
2. **Create the handler** in `<EventName>Handler.cs`, implementing
   `INotificationHandler<TEvent>`. Constructor-inject the services that need to react — usually an
   `I<Domain>CacheService` (API-backed domains only). Guard with `.ThrowIfNull().Value`, and guard
   the event and its required members at the top of `Handle`. Keep the body a set of small, named
   private methods per concern (e.g. `EnsureNotifyXChanged`) rather than one long method.
3. **Publish it** from the command handler that made the change, after the write succeeds:
   `await _appBus.Publish(new OrderChangedEvent(model), token);`
4. No DI registration is needed for the handler itself (MediatR auto-discovers
   `INotificationHandler<T>` implementations from the `CoreMarker` assembly), but any *new* cache
   service it depends on must be registered as a singleton in `AddInfrastructureServices` — cache
   services are shared across all Blazor circuits and must not be scoped/transient.
5. **Add a test** for the handler's `Handle` method (Moq the dependent services, assert the right
   notify methods are called for the right conditions) — see `add-unit-test.instructions.md`.

## Gotchas

- Only write a handler if something actually needs to react. A DB-backed domain with no in-memory
  cache and no other reactive concern often doesn't need an event at all — don't publish "just in
  case."
- This pattern is most valuable for API-backed domains, where an in-memory cache needs
  invalidating after a write bypasses it. Don't introduce a domain event purely to keep symmetry
  with other domains if nothing subscribes to it.
