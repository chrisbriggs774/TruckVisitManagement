# Ubiquitous Language — Truck Visit Management Domain

| | |
|---|---|
| **Version** | 1.0 |
| **Status** | Draft |
| **Document Type** | Domain-Driven Design (DDD) |
| **Audience** | Product Owners, Business Analysts, Subject Matter Experts, Architects, Developers, QA Engineers |

---

## Table of Contents

1. [Purpose](#1-purpose)
2. [Language Principles](#2-language-principles)
3. [Core Domain Terminology](#3-core-domain-terminology)
4. [Actors](#4-actors)
5. [Vehicle Terminology](#5-vehicle-terminology)
6. [Driver Identification Terminology](#6-driver-identification-terminology)
7. [Visit Activity Terminology](#7-visit-activity-terminology)
8. [Visit Lifecycle Terminology](#8-visit-lifecycle-terminology)
9. [Audit Terminology](#9-audit-terminology)
10. [Operational Terminology](#10-operational-terminology)
11. [Security Terminology](#11-security-terminology)
12. [Search Terminology](#12-search-terminology)
13. [Domain Events Vocabulary](#13-domain-events-vocabulary)
14. [Aggregate Vocabulary](#14-aggregate-vocabulary)
15. [Business Glossary](#15-business-glossary)
16. [Decisions Captured in This Document](#decisions-captured-in-this-document)

---

## 1. Purpose

This document establishes the official business vocabulary for the **Truck Visit Management** domain.

Its purpose is to ensure that all stakeholders use consistent terminology when discussing requirements, business processes, APIs, architecture, testing, user stories, and operational procedures.

> All future documentation, source code, API contracts, database schemas, user interfaces, and operational support materials should align with the language defined in this document.

---

## 2. Language Principles

### Single Meaning Rule
Each term must have exactly one meaning within the domain. A term must not represent different concepts in different contexts.

### Business First Rule
Terms should reflect business language rather than technical implementation details.

| ✅ Use | ❌ Avoid |
|---|---|
| `Visit` | `VisitEntity` |
| `Visit` | `VisitRecord` |

### Shared Understanding Rule
Business users and technical teams should be able to communicate using the same terminology.

---

## 3. Core Domain Terminology

### Visit

**Definition**
A single truck movement through a terminal, from registration until departure.

**Business Meaning**
A Visit represents the complete lifecycle of a truck entering, operating within, and leaving a terminal.

**Examples**
- Collecting a container
- Delivering cargo
- Completing multiple collections and deliveries

**Notes**
A Visit is the primary business concept within the domain.

### Terminal

**Definition**
A physical operational location that manages truck movements.

**Business Meaning**
A location where visits occur.

**Examples**
- Immingham Terminal
- Rotterdam Terminal

**Identifier**: `TerminalId`

### Smart Gate

**Definition**
An automated access-control solution used to manage truck entry and exit.

**Business Meaning**
The primary interface between drivers and the terminal.

**Responsibilities**
- Driver interaction
- Identity verification
- Vehicle validation
- Visit lookup
- Visit status progression

> The Smart Gate is assumed to be the primary operating model.

---

## 4. Actors

### Driver

**Definition**
The individual operating a truck during a visit.

**Business Meaning**
The Driver is the primary operational actor — a contracted haulier arriving to collect cargo.

**Responsibilities**
- Identify themselves
- Provide vehicle details
- Provide trailer details
- Follow Smart Gate instructions
- Complete self-service processing

### Gate Operator

**Definition**
A terminal employee responsible for handling exceptions that cannot be processed automatically.

**Business Meaning**
A support actor rather than the primary actor.

**Responsibilities**
- Assist drivers
- Resolve validation issues
- Handle operational exceptions
- Support manual intervention workflows

> Gate Operators do not perform routine visit processing.

### Administrator

**Definition**
A user responsible for system administration and operational management.

---

## 5. Vehicle Terminology

### Truck

**Definition**
The powered vehicle performing a visit.

**Business Meaning**
The transport vehicle associated with the Driver.

**Attributes**
- Unit Number
- License Plate

**Business Rules**
Values must:
- Be uppercase
- Contain no whitespace

**Examples**

| Valid | Invalid |
|---|---|
| `AB123` | `ab123` |
| `YX22ABC` | `YX 22 ABC` |

#### Unit Number

**Definition**
The operational identifier assigned to a truck.

**Business Meaning**
An identifier used by hauliers and terminal operators.

#### License Plate

**Definition**
The legal registration identifier displayed on a vehicle.

**Business Meaning**
The externally recognised identifier of the truck.

### Trailer

**Definition**
A non-powered vehicle attached to a truck.

**Business Meaning**
Equipment used to transport cargo through the terminal.

**Attributes**
- Trailer Number
- Trailer Registration

**Business Rule**
All identifying values must be normalised to uppercase without whitespace.

---

## 6. Driver Identification Terminology

### Driver Identity

**Definition**
Information used to establish the driver's identity.

**Examples**
- Driver Licence
- Driver Identification Card
- Passport
- Company Identification

### Driver Verification

**Definition**
The process of validating the identity information provided by a Driver.

**Business Meaning**
A verification activity performed by Smart Gate or an integrated system.

---

## 7. Visit Activity Terminology

### Collection

**Definition**
Cargo, equipment, container, or goods being removed from the terminal.

**Business Meaning**
An outbound movement from terminal custody.

**Example**: A truck collects a shipping container for transport.

### Delivery

**Definition**
Cargo, equipment, container, or goods being brought into the terminal.

**Business Meaning**
An inbound movement into terminal custody.

**Example**: A truck delivers an import container.

### Movement

**Definition**
A business activity associated with a Visit.

**Business Meaning**
A Collection or Delivery.

**Formula**
```
Movement = Collection OR Delivery
```

---

## 8. Visit Lifecycle Terminology

### Visit Status

**Definition**
The current stage of a Visit.

**Business Meaning**
Represents where the truck is within the terminal journey.

### Lifecycle Stages

| Status | Definition | Typical Activities / State |
|---|---|---|
| **Pre-Registered** | The Visit has been created but the truck has not yet arrived. | Expected arrival |
| **At Gate** | The truck has arrived at the terminal gate and is undergoing entry processing. | Identity verification, Vehicle verification, Visit validation |
| **On Site** | The truck has successfully entered the terminal. | Collections, Deliveries, Yard movements, Operational processing |
| **Completed** | The truck has exited the terminal and the Visit has concluded. | Visit is closed — no additional status progression is permitted |

---

## 9. Audit Terminology

### Status Transition

**Definition**
A change from one Visit Status to another.

**Examples**
- Pre-Registered → At Gate
- At Gate → On Site
- On Site → Completed

### Status History

**Definition**
The complete historical record of all Visit Status transitions.

**Business Meaning**
An immutable representation of a Visit's lifecycle.

**Business Rule**
Status History cannot be modified or deleted.

### Audit Record

**Definition**
A single entry within Status History.

**Contains**
- Previous Status
- New Status
- Timestamp
- Actor

### Audit Trail

**Definition**
The complete sequence of Audit Records associated with a Visit.

---

## 10. Operational Terminology

### Check-In

**Definition**
The process by which a Driver arrives at the Smart Gate and begins visit processing.

**Typical Outcome**
Status changes to **At Gate**.

### Check-Out

**Definition**
The process by which a Driver leaves the terminal.

**Typical Outcome**
Status changes to **Completed**.

### Exception

**Definition**
A condition preventing successful automated processing.

**Examples**
- Missing visit
- Invalid identity
- Validation failure
- Hardware issue
- Data mismatch

### Exception Resolution

**Definition**
The process of resolving an Exception through manual intervention.

**Primary Actor**: Gate Operator

---

## 11. Security Terminology

### Authentication

**Definition**
Verification of identity for a user or system.

**Examples**
- OAuth2
- OpenID Connect
- JWT

### Authorization

**Definition**
Verification that an authenticated actor is permitted to perform an action.

### Terminal Access

**Definition**
A permission granting access to information belonging to a specific Terminal.

**Business Rule**
Users may only access terminals they have been authorised to view or manage.

---

## 12. Search Terminology

### Visit Search

**Definition**
The capability to retrieve Visits using business criteria.

### Current Status Filter

**Definition**
A search condition based on the Visit's active status.

### Movement Window

**Definition**
A period used to identify Visits associated with operational movements.

**Parameters**
- `MovementFrom`
- `MovementTo`

### Creation Window

**Definition**
A period used to identify Visits based on creation date and time.

**Parameters**
- `CreatedTimeFrom`
- `CreatedTimeTo`

---

## 13. Domain Events Vocabulary

| Event | Definition |
|---|---|
| **Visit Created** | A new Visit is registered within the system. |
| **Driver Verified** | The Driver's identity has been successfully validated. |
| **Truck Validated** | The truck details have passed validation checks. |
| **Visit Arrived At Gate** | The Visit status becomes At Gate. |
| **Visit Entered Site** | The Visit status becomes On Site. |
| **Visit Completed** | The Visit status becomes Completed. |
| **Exception Raised** | An automated process encounters a condition requiring intervention. |
| **Exception Resolved** | An Exception has been successfully addressed. |

---

## 14. Aggregate Vocabulary

### Aggregate

**Definition**
A consistency boundary used to enforce business rules.

**Current Aggregate**: `Visit`

### Aggregate Root

**Definition**
The entity responsible for protecting business invariants.

**Current Aggregate Root**: `Visit`

### Child Entity

**Definition**
An entity owned by an Aggregate Root.

**Examples**
- Truck
- Driver
- Collection
- Delivery
- Status History

---

## 15. Business Glossary

| Term | Definition |
| ---- | ---------- |
| Visit | Complete truck journey through a terminal |
| Terminal | Physical location where visits occur |
| Smart Gate | Automated entry/exit solution |
| Driver | Primary actor performing self-service processing |
| Gate Operator | Support actor handling exceptions |
| Truck | Powered vehicle making a visit |
| Trailer | Associated transport equipment |
| Collection | Goods removed from terminal |
| Delivery | Goods brought into terminal |
| Movement | Collection or Delivery activity |
| Visit Status | Current stage of a visit |
| Status Transition | Change from one status to another |
| Status History | Immutable lifecycle history |
| Audit Record | Single status change entry |
| Check-In | Driver arrival process |
| Check-Out | Driver departure process |
| Exception | Failed automated process |
| Terminal Access | Permission to access terminal data |
| Aggregate Root | Business consistency boundary owner |

---

## Decisions Captured in This Document

| ID | Decision |
|---|---|
| **D-001** | The Driver is the primary business actor. |
| **D-002** | The Gate Operator exists primarily for exception management and assisted processing. |
| **D-003** | The Smart Gate is considered the primary operational channel. |
| **D-004** | A Visit is the central domain concept and aggregate root around which all other business concepts are organised. |
