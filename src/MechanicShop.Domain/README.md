# MechanicShop – Domain Layer

## 1. Overview

The **Domain Layer** is the core of the MechanicShop application, containing the business logic, domain entities, business rules, value objects, domain events, and custom error definitions.

It follows the principles of **Clean Architecture** and **Domain-Driven Design (DDD)**, keeping business logic independent of external concerns such as databases, web frameworks, and API implementations.

### Design Principles

The Domain Layer follows these principles:

- **Encapsulation:** Entity properties use private setters or restricted accessors to prevent uncontrolled state modifications.
- **Factory Methods:** Entities are created through static factory methods to validate initial data and protect business rules.
- **Invariants Protection:** Entities enforce their own business rules and prevent invalid state transitions.
- **Rich Domain Model:** Business behavior is implemented inside domain entities rather than placing all business logic in external services.
- **Computed Properties:** Derived values such as subtotals and totals are calculated dynamically to avoid duplicated or inconsistent state.

---

## 2. Result Pattern

MechanicShop uses a custom **Result Pattern** to represent expected business outcomes without relying on exceptions for normal business failures.

Instead of throwing exceptions for expected situations, operations return a `Result<T>` containing either a successful value or one or more structured errors.

### Main Concepts

- **Result:** Represents the outcome of an operation.
- **IsError:** Indicates whether the operation failed.
- **Errors:** Contains the errors associated with a failed operation.
- **Error:** Represents a structured error with a code, description, and classification.
- **ErrorKind:** Categorizes errors, such as Validation, NotFound, Conflict, and other supported types.
- **Marker Types:** Types such as `Created`, `Updated`, and `Deleted` represent successful operations that do not require returning a domain entity.

### Example

```csharp
public Result<Updated> ApplyDiscount(decimal discountAmount)
{
    if (Status != InvoiceStatus.Unpaid)
        return InvoiceErrors.InvoiceLocked;

    if (discountAmount > Subtotal)
        return InvoiceErrors.DiscountExceedsSubtotal;

    DiscountAmount = discountAmount;

    return Result.Updated;
}
```

### Why Result Pattern?

The Result Pattern provides:

- Explicit success and failure outcomes.
- Structured and reusable error definitions.
- Clear separation between expected business failures and unexpected runtime exceptions.
- Consistent error handling across application operations.

**Important:** Result Pattern does not replace exception handling. Expected business failures are represented through Results, while unexpected technical or infrastructure failures are handled through exceptions.

---

## 3. Domain Structure

The Domain Layer is organized into business modules to keep related entities, rules, and events together.

### A. WorkOrders

**Namespace:** `MechanicShop.Domain.Workorders`

#### WorkOrder

The `WorkOrder` entity acts as an **Aggregate Root**, managing the lifecycle of a repair job.

Responsibilities include:

- Managing work order timing.
- Assigning technicians and workshop spots.
- Controlling valid state transitions.
- Protecting business rules during work order updates.

#### WorkOrder State Machine

Work orders follow controlled lifecycle transitions:

```text
Scheduled
    |
    v
InProgress
   / \
  v   v
Completed  Cancelled
```

**Enums:**

- `WorkOrderState`: Defines the current work order state.
- `Spot`: Represents available workshop workstation bays (`A`, `B`, `C`, `D`).

#### Billing

**Namespace:** `MechanicShop.Domain.Workorders.Billing`

Contains the billing-related domain entities.

- **Invoice:** Manages invoice status, subtotal, taxes, discounts, total calculations, and payment tracking.
- **InvoiceLineItem:** Represents an individual service or part included in an invoice.
- **InvoiceStatus:** Defines invoice states, including `Unpaid`, `Paid`, and `Refunded`.

#### Events

**Namespace:** `MechanicShop.Domain.Workorders.Events`

Contains domain events associated with work order activities.

- **WorkOrderCompleted:** Represents the completion of a work order and can notify other parts of the application.
- **WorkOrderCollectionModified:** Represents modifications to work order collections.

---

### B. RepairTasks

**Namespace:** `MechanicShop.Domain.RepairTasks`

Contains entities responsible for defining repair and maintenance tasks.

#### RepairTask

Represents a specific maintenance or repair operation.

Responsibilities include:

- Defining repair task details.
- Managing labor costs.
- Defining estimated repair durations.

#### Repair Duration

`RepairDurationInMinutes` represents supported repair duration values, ranging from 15 to 180 minutes.

#### Parts

**Namespace:** `MechanicShop.Domain.RepairTasks.Parts`

Contains the `Part` entity, responsible for representing parts used during repair operations.

Features include:

- Tracking part quantities.
- Managing unit costs.
- Synchronizing part collections through `UpsertParts`, supporting additions, updates, and removals.

---

### C. Common

**Namespace:** `MechanicShop.Domain.Common`

Contains shared domain abstractions and reusable components.

#### AuditableEntity

A base entity providing common properties:

- `Guid Id`: Unique entity identifier.
- Audit timestamps for tracking entity creation and updates.

#### Domain Errors

Domain errors are centralized in static error definition classes.

Examples:

- `InvoiceErrors`
- `RepairTaskErrors`
- `WorkOrderErrors`

These definitions provide reusable error codes, descriptions, and classifications for business rule violations.

---

## 4. Domain Layer Responsibilities

The Domain Layer is responsible for:

- Defining business entities and their behavior.
- Enforcing business rules and invariants.
- Managing valid entity state transitions.
- Protecting aggregate consistency.
- Defining domain events.
- Representing expected business failures through domain errors and Results.

The Domain Layer should remain independent of:

- Entity Framework Core.
- SQL Server and database implementations.
- ASP.NET Core and HTTP responses.
- External APIs and infrastructure services.

---

## 5. Design Philosophy

The main objective of the MechanicShop Domain Layer is to keep business rules close to the entities that own them.

By applying encapsulation, factory methods, aggregate boundaries, domain events, and the Result Pattern, the application maintains a domain model that is easier to understand, validate, maintain, and extend.

